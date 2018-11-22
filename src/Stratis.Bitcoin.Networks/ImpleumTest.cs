using System;
using System.Collections.Generic;
using System.Net;
using NBitcoin;
using NBitcoin.BouncyCastle.Math;
using NBitcoin.Protocol;
using Stratis.Bitcoin.Networks.Deployments;

namespace Stratis.Bitcoin.Networks
{
    public class ImpleumTest : ImpleumMain
    {
        public ImpleumTest()
        {
            // The message start string is designed to be unlikely to occur in normal data.
            // The characters are rarely used upper ASCII, not valid as UTF-8, and produce
            // a large 4-byte int at any alignment.
            var messageStart = new byte[4];
            messageStart[0] = 0x51;
            messageStart[1] = 0x11;
            messageStart[2] = 0x41;
            messageStart[3] = 0x31;
            uint magic = BitConverter.ToUInt32(messageStart, 0); // 0x11213171;

            this.Name = nameof(ImpleumTest);
            this.Magic = magic;
            this.DefaultPort = 16271;
            this.RPCPort = 16272;
            this.CoinTicker = "TIMPL";

            var powLimit = new Target(new uint256("0000ffff00000000000000000000000000000000000000000000000000000000"));

            var consensusFactory = new PosConsensusFactory();

            // Create the genesis block.
            this.GenesisTime = 1523364655;
            this.GenesisNonce = 2380297;
            this.GenesisBits = 0x1e0fffff;
            this.GenesisVersion = 1;
            this.GenesisReward = Money.Zero;

            Block genesisBlock = CreateImpleumGenesisBlock(consensusFactory, this.GenesisTime, this.GenesisNonce, this.GenesisBits, this.GenesisVersion, this.GenesisReward);
            genesisBlock.Header.Time = 152336701;
            genesisBlock.Header.Nonce = 1503601;
            genesisBlock.Header.Bits = powLimit;

            this.Genesis = genesisBlock;


            // Taken from StratisX.
            var consensusOptions = new PosConsensusOptions(
                maxBlockBaseSize: 1_000_000,
                maxStandardVersion: 2,
                maxStandardTxWeight: 100_000,
                maxBlockSigopsCost: 20_000,
                maxStandardTxSigopsCost: 20_000 / 5,
                provenHeadersActivationHeight: 10_000_000 // TODO: Set it to the real value once it is known.
            );

            var buriedDeployments = new BuriedDeploymentsArray
            {
                [BuriedDeployments.BIP34] = 0,
                [BuriedDeployments.BIP65] = 0,
                [BuriedDeployments.BIP66] = 0
            };


            var bip9Deployments = new ImpleumBIP9Deployments();

            this.Consensus = new NBitcoin.Consensus(
                consensusFactory: consensusFactory,
                consensusOptions: consensusOptions,
                coinType: 769,
                hashGenesisBlock: genesisBlock.GetHash(),
                subsidyHalvingInterval: 210000,
                majorityEnforceBlockUpgrade: 750,
                majorityRejectBlockOutdated: 950,
                majorityWindow: 1000,
                buriedDeployments: buriedDeployments,
                bip9Deployments: bip9Deployments,
                bip34Hash: new uint256("0x000000000000024b89b42a942fe0d9fea3bb44ab7bd1b19115dd6a759c0808b8"),
                ruleChangeActivationThreshold: 1916,// 95% of 2016
                minerConfirmationWindow: 2016, // nPowTargetTimespan / nPowTargetSpacing
                maxReorgLength: 500,
                defaultAssumeValid: null, // 23000
                maxMoney: 100000000 * Money.COIN,
                coinbaseMaturity: 50,
                premineHeight: 2,
                premineReward: Money.Coins(1000000),
                proofOfWorkReward: Money.Coins(48),
                powTargetTimespan: TimeSpan.FromSeconds(14 * 24 * 60 * 60), // two weeks
                powTargetSpacing: TimeSpan.FromSeconds(10 * 60),
                powAllowMinDifficultyBlocks: false,
                powNoRetargeting: false,
                powLimit: new Target(new uint256("00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff")),
                minimumChainWork: null,
                isProofOfStake: true,
                lastPowBlock: 100000,
                proofOfStakeLimit: new BigInteger(uint256.Parse("00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff").ToBytes(false)),
                proofOfStakeLimitV2: new BigInteger(uint256.Parse("000000000000ffffffffffffffffffffffffffffffffffffffffffffffffffff").ToBytes(false)),
                proofOfStakeReward: Money.Coins(5m)
            );

            this.Base58Prefixes[(int)Base58Type.PUBKEY_ADDRESS] = new byte[] { (102) };
            this.Base58Prefixes[(int)Base58Type.SCRIPT_ADDRESS] = new byte[] { (196) };
            this.Base58Prefixes[(int)Base58Type.SECRET_KEY] = new byte[] { (65 + 128) };

            this.Checkpoints = new Dictionary<int, CheckpointInfo>
            {
            };

            this.DNSSeeds = new List<DNSSeedData>
            {
            };

            this.SeedNodes = new List<NetworkAddress>
            {
                new NetworkAddress(IPAddress.Parse("94.131.240.45"), this.DefaultPort),
                new NetworkAddress(IPAddress.Parse("62.80.181.141"), this.DefaultPort)
            };
            
            Assert(this.Consensus.HashGenesisBlock == uint256.Parse("dab06c78af72a1fb8a17581c631ec2d3df60a8029142962de5448b992b47467e"));
        }
    }
}
