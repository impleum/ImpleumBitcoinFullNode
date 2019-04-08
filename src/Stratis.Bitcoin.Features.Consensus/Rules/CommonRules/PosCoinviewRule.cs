using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Stratis.Bitcoin.Consensus;
using Stratis.Bitcoin.Consensus.Rules;
using Stratis.Bitcoin.Features.Consensus.Interfaces;
using Stratis.Bitcoin.Utilities;

namespace Stratis.Bitcoin.Features.Consensus.Rules.CommonRules
{
    /// <summary>
    /// Proof of stake override for the coinview rules - BIP68, MaxSigOps and BlockReward checks.
    /// </summary>
    public sealed class PosCoinviewRule : CoinViewRule
    {
        /// <summary>Provides functionality for checking validity of PoS blocks.</summary>
        private IStakeValidator stakeValidator;

        /// <summary>Database of stake related data for the current blockchain.</summary>
        private IStakeChain stakeChain;

        /// <summary>The consensus of the parent Network.</summary>
        private IConsensus consensus;

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            this.consensus = this.Parent.Network.Consensus;
            var consensusRules = (PosConsensusRuleEngine)this.Parent;

            this.stakeValidator = consensusRules.StakeValidator;
            this.stakeChain = consensusRules.StakeChain;
        }

        /// <inheritdoc />
        /// <summary>Compute and store the stake proofs.</summary>
        public override async Task RunAsync(RuleContext context)
        {
            this.CheckAndComputeStake(context);

            await base.RunAsync(context).ConfigureAwait(false);
            var posRuleContext = context as PosRuleContext;
            this.stakeChain.Set(context.ValidationContext.ChainedHeaderToValidate, posRuleContext.BlockStake);
        }

        /// <inheritdoc />
        public override void CheckBlockReward(RuleContext context, Money fees, int height, Block block)
        {
            if (BlockStake.IsProofOfStake(block))
            {
                var posRuleContext = context as PosRuleContext;
                Money stakeReward = block.Transactions[1].TotalOut - posRuleContext.TotalCoinStakeValueIn;
                Money calcStakeReward = fees + this.GetProofOfStakeReward(height, posRuleContext.CoinAge);
               

                this.Logger.LogTrace("Block stake reward is {0}, calculated reward is {1}.", stakeReward, calcStakeReward);
                if (stakeReward > calcStakeReward)
                {
                    this.Logger.LogTrace("(-)[BAD_COINSTAKE_AMOUNT]");
                    ConsensusErrors.BadCoinstakeAmount.Throw();
                }
            }
            else
            {
                Money blockReward = fees + this.GetProofOfWorkReward(height);
                this.Logger.LogTrace("Block reward is {0}, calculated reward is {1}.", block.Transactions[0].TotalOut, blockReward);
                if (block.Transactions[0].TotalOut > blockReward)
                {
                    this.Logger.LogTrace("(-)[BAD_COINBASE_AMOUNT]");
                    ConsensusErrors.BadCoinbaseAmount.Throw();
                }
            }
        }

        /// <inheritdoc />
        public override void UpdateCoinView(RuleContext context, Transaction transaction)
        {
            var posRuleContext = context as PosRuleContext;

            UnspentOutputSet view = posRuleContext.UnspentOutputSet;

            if (transaction.IsCoinStake)
            {
                posRuleContext.TotalCoinStakeValueIn = view.GetValueIn(transaction);
                posRuleContext.CoinStakePrevOutputs = transaction.Inputs.ToDictionary(txin => txin, txin => view.GetOutputFor(txin));
            }

            base.UpdateUTXOSet(context, transaction);
        }

        /// <inheritdoc />
        public override void CheckMaturity(UnspentOutputs coins, int spendHeight)
        {
            base.CheckCoinbaseMaturity(coins, spendHeight);

            if (coins.IsCoinstake)
            {
                if ((spendHeight - coins.Height) < this.consensus.CoinbaseMaturity)
                {
                    this.Logger.LogTrace("Coinstake transaction height {0} spent at height {1}, but maturity is set to {2}.", coins.Height, spendHeight, this.consensus.CoinbaseMaturity);
                    this.Logger.LogTrace("(-)[COINSTAKE_PREMATURE_SPENDING]");
                    ConsensusErrors.BadTransactionPrematureCoinstakeSpending.Throw();
                }
            }
        }

        /// <inheritdoc />
        protected override void CheckInputValidity(Transaction transaction, UnspentOutputs coins)
        {
            // Transaction timestamp earlier than input transaction - main.cpp, CTransaction::ConnectInputs
            if (coins.Time > transaction.Time)
                ConsensusErrors.BadTransactionEarlyTimestamp.Throw();
        }

        /// <summary>
        /// Checks and computes stake.
        /// </summary>
        /// <param name="context">Context that contains variety of information regarding blocks validation and execution.</param>
        /// <exception cref="ConsensusErrors.PrevStakeNull">Thrown if previous stake is not found.</exception>
        /// <exception cref="ConsensusErrors.SetStakeEntropyBitFailed">Thrown if failed to set stake entropy bit.</exception>
        private void CheckAndComputeStake(RuleContext context)
        {
            ChainedHeader chainedHeader = context.ValidationContext.ChainedHeaderToValidate;
            Block block = context.ValidationContext.BlockToValidate;

            var posRuleContext = context as PosRuleContext;
            if (posRuleContext.BlockStake == null)
                posRuleContext.BlockStake = BlockStake.Load(context.ValidationContext.BlockToValidate);

            BlockStake blockStake = posRuleContext.BlockStake;

            // Verify hash target and signature of coinstake tx.
            if (BlockStake.IsProofOfStake(block))
            {
                ChainedHeader prevChainedHeader = chainedHeader.Previous;

                BlockStake prevBlockStake = this.stakeChain.Get(prevChainedHeader.HashBlock);
                if (prevBlockStake == null)
                    ConsensusErrors.PrevStakeNull.Throw();

                // Only do proof of stake validation for blocks that are after the assumevalid block or after the last checkpoint.
                if (!context.SkipValidation)
                {
                    this.stakeValidator.CheckProofOfStake(posRuleContext, prevChainedHeader, prevBlockStake, block.Transactions[1], chainedHeader.Header.Bits.ToCompact());
                }
                else this.Logger.LogTrace("POS validation skipped for block at height {0}.", chainedHeader.Height);
            }

            // PoW is checked in CheckBlock().
            if (BlockStake.IsProofOfWork(block))
                posRuleContext.HashProofOfStake = chainedHeader.Header.GetPoWHash();

            // Compute stake entropy bit for stake modifier.
            if (!blockStake.SetStakeEntropyBit(blockStake.GetStakeEntropyBit()))
            {
                this.Logger.LogTrace("(-)[STAKE_ENTROPY_BIT_FAIL]");
                ConsensusErrors.SetStakeEntropyBitFailed.Throw();
            }

            // Record proof hash value.
            blockStake.HashProof = posRuleContext.HashProofOfStake;

            int lastCheckpointHeight = this.Parent.Checkpoints.GetLastCheckpointHeight();
            if (chainedHeader.Height > lastCheckpointHeight)
            {
                // Compute stake modifier.
                ChainedHeader prevChainedHeader = chainedHeader.Previous;
                BlockStake blockStakePrev = prevChainedHeader == null ? null : this.stakeChain.Get(prevChainedHeader.HashBlock);
                blockStake.StakeModifierV2 = this.stakeValidator.ComputeStakeModifierV2(prevChainedHeader, blockStakePrev?.StakeModifierV2, blockStake.IsProofOfWork() ? chainedHeader.HashBlock : blockStake.PrevoutStake.Hash);
            }
            else if (chainedHeader.Height == lastCheckpointHeight)
            {
                // Copy checkpointed stake modifier.
                CheckpointInfo checkpoint = this.Parent.Checkpoints.GetCheckpoint(lastCheckpointHeight);
                blockStake.StakeModifierV2 = checkpoint.StakeModifierV2;
                this.Logger.LogTrace("Last checkpoint stake modifier V2 loaded: '{0}'.", blockStake.StakeModifierV2);
            }
            else this.Logger.LogTrace("POS stake modifier computation skipped for block at height {0} because it is not above last checkpoint block height {1}.", chainedHeader.Height, lastCheckpointHeight);
        }

        /// <inheritdoc />
        public override Money GetProofOfWorkReward(int height)
        {
            if (this.IsPremine(height))
                return this.consensus.PremineReward;
            if (height <= 40100)
                return this.consensus.ProofOfWorkReward;
            if (height <= 45000)
                return Money.Coins(24);
            if (height <= 50000)
                return Money.Coins(12);
            if (height <= 55000)
                return Money.Coins(6);
            if (height <= 60000)
                return Money.Coins(3);
            if (height <= 65000)
                return Money.Coins(1);
            if (height <= 70000)
                return Money.Coins(0);
            if (height >= 75000)
                return Money.Coins(0.48m);
            return Money.Coins(0);
        }

        /// <summary>
        /// Gets miner's coin stake reward.
        /// </summary>
        /// <param name="height">Target block height.</param>
        /// <param name="coinAge">Age of coin</param>
        /// <returns>Miner's coin stake reward.</returns>
        public Money GetProofOfStakeReward(int height, long? coinAge = null)
        {
            if (this.IsPremine(height))
                return this.consensus.PremineReward;

            if (coinAge.HasValue)
            {
                long nSubsidy = (coinAge.Value * 1 * Money.CENT * 33 / (365 * 33 + 8));
                if (height <= 1000)
                {
                    nSubsidy >>= (int)(nSubsidy / 100000);
                    return Money.Satoshis(nSubsidy);  //no substantial pos reward until block 1k
                }
                else if (height <= 7201)
                {
                    return Money.Satoshis(nSubsidy * 500);
                }
                else if (height > 7201 && height <= 14401)
                {
                    return Money.Satoshis(nSubsidy * 250);
                }
                else if (height > 14401 && height <= 21601)
                {
                    return Money.Satoshis(nSubsidy * 500);
                }
                else if (height > 21601 && height <= 28801)
                {
                    return Money.Satoshis(nSubsidy * 250);
                }
                else if (height > 28801 && height <= 32401)
                {
                    return Money.Satoshis(nSubsidy * 1000);
                }
                else if (height > 32401 && height <= 36001)
                {
                    return Money.Satoshis(nSubsidy * 500);
                }
                else if (height > 36001 && height <= 43201)
                {
                    return Money.Satoshis(nSubsidy * 250);
                }
                else if (height > 43201 && height <= 50401)
                {
                    return Money.Satoshis(nSubsidy * 500);
                }
                else if (height > 50401 && height <= 57601)
                {
                    return Money.Satoshis(nSubsidy * 250);
                }
                else if (height > 57601 && height <= 72001)
                {
                    return Money.Satoshis(nSubsidy * 250);
                }
                else if (height > 72001 && height <= 75601)
                {
                    return Money.Satoshis(nSubsidy * 500);
                }
                else if (height > 75601 && height <= 82801)
                {
                    return Money.Satoshis(nSubsidy * 250);
                }
                else if (height > 82801 && height <= 90001)
                {
                    return Money.Satoshis(nSubsidy * 750);
                }
                else if (height > 90001 && height <= 97201)
                {
                    return Money.Satoshis(nSubsidy * 250);
                }
                else if (height > 97201 && height <= 100801)
                {
                    return Money.Satoshis(nSubsidy * 500);
                }
                else if (height > 100801 && height <= 108001)
                {
                    return Money.Satoshis(nSubsidy * 250);
                }
                else if (height > 108001 && height <= 115201)
                {
                    return Money.Satoshis(nSubsidy * 500);
                }
                else if (height > 115201 && height <= 129601)
                {
                    return Money.Satoshis(nSubsidy * 250);
                }
                else if (height > 129601 && height <= 215000)
                {
                    return Money.Satoshis(nSubsidy * 10);
                }
                else
                {
                    return this.consensus.ProofOfStakeReward;
                }

            }
            return this.consensus.ProofOfStakeReward;
        }
    }
}
