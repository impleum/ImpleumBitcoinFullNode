using System;
using System.Collections.Generic;
using System.Linq;
using NBitcoin;
using NBitcoin.BouncyCastle.Math;
using NBitcoin.DataEncoders;
using Stratis.Bitcoin.Networks.Deployments;

namespace Stratis.Bitcoin.Networks
{
    public class ImpleumMain : Network
    {
        /// <summary> Impleum maximal value for the calculated time offset. If the value is over this limit, the time syncing feature will be switched off. </summary>
        public const int ImpleumMaxTimeOffsetSeconds = 25 * 60;

        /// <summary> Impleum default value for the maximum tip age in seconds to consider the node in initial block download (2 hours). </summary>
        public const int ImpleumDefaultMaxTipAgeInSeconds = 24 * 60 * 60;

        /// <summary> The name of the root folder containing the different Impleum blockchains (ImpleumMain, ImpleumTest, ImpleumRegTest). </summary>
        public const string ImpleumRootFolderName = "Impleum";

        /// <summary> The default name used for the Impleum configuration file. </summary>
        public const string ImpleumDefaultConfigFilename = "Impleum.conf";

        public ImpleumMain()
        {
            // The message start string is designed to be unlikely to occur in normal data.
            // The characters are rarely used upper ASCII, not valid as UTF-8, and produce
            // a large 4-byte int at any alignment.
            var messageStart = new byte[4];
            messageStart[0] = 0x51;
            messageStart[1] = 0x11;
            messageStart[2] = 0x41;
            messageStart[3] = 0x31;
            var magic = BitConverter.ToUInt32(messageStart, 0); //0x5223570;

            this.Name = nameof(ImpleumMain);
            this.RootFolderName = ImpleumRootFolderName;
            this.DefaultConfigFilename = ImpleumDefaultConfigFilename;
            this.Magic = magic;
            this.DefaultPort = 16171;
            this.DefaultMaxOutboundConnections = 16;
            this.DefaultMaxInboundConnections = 109;
            this.RPCPort = 16172;
            this.MinTxFee = 10000;
            this.FallbackFee = 60000;
            this.MinRelayTxFee = 10000;
            this.MaxTimeOffsetSeconds = ImpleumMaxTimeOffsetSeconds;
            this.MaxTipAge = ImpleumDefaultMaxTipAgeInSeconds;
            this.CoinTicker = "IMPL";

            var consensusFactory = new PosConsensusFactory();

            // Create the genesis block.
            this.GenesisTime = 1523364655;
            this.GenesisNonce = 2380297;
            this.GenesisBits = 0x1e0fffff;
            this.GenesisVersion = 1;
            this.GenesisReward = Money.Zero;

            Block genesisBlock = CreateImpleumGenesisBlock(consensusFactory, this.GenesisTime, this.GenesisNonce, this.GenesisBits, this.GenesisVersion, this.GenesisReward);

            this.Genesis = genesisBlock;

            // Taken from StratisX.
            var consensusOptions = new PosConsensusOptions(
                maxBlockBaseSize: 1_000_000,
                maxStandardVersion: 2,
                maxStandardTxWeight: 100_000,
                maxBlockSigopsCost: 20_000,
                maxStandardTxSigopsCost: 20_000 / 5
            );


            var buriedDeployments = new BuriedDeploymentsArray
            {
                [BuriedDeployments.BIP34] = 0,
                [BuriedDeployments.BIP65] = 0,
                [BuriedDeployments.BIP66] = 0
            };

            var bip9Deployments = new ImpleumBIP9Deployments()
            {
                [StratisBIP9Deployments.ColdStaking] = new BIP9DeploymentsParameters(2,
                    new DateTime(2018, 12, 1, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2019, 12, 1, 0, 0, 0, DateTimeKind.Utc))
            };


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
                defaultAssumeValid: new uint256("0x04c2b9fe7e52e0c6d54fbdf5018fcda8709457b1c12b0dc8eae185b2018de19a"), // 215001
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

            this.Base58Prefixes = new byte[12][];
            this.Base58Prefixes[(int)Base58Type.PUBKEY_ADDRESS] = new byte[] { (102) };
            this.Base58Prefixes[(int)Base58Type.SCRIPT_ADDRESS] = new byte[] { (125) };
            this.Base58Prefixes[(int)Base58Type.SECRET_KEY] = new byte[] { (63 + 128) };
            this.Base58Prefixes[(int)Base58Type.ENCRYPTED_SECRET_KEY_NO_EC] = new byte[] { 0x01, 0x42 };
            this.Base58Prefixes[(int)Base58Type.ENCRYPTED_SECRET_KEY_EC] = new byte[] { 0x01, 0x43 };
            this.Base58Prefixes[(int)Base58Type.EXT_PUBLIC_KEY] = new byte[] { (0x04), (0x88), (0xB2), (0x1E) };
            this.Base58Prefixes[(int)Base58Type.EXT_SECRET_KEY] = new byte[] { (0x04), (0x88), (0xAD), (0xE4) };
            this.Base58Prefixes[(int)Base58Type.PASSPHRASE_CODE] = new byte[] { 0x2C, 0xE9, 0xB3, 0xE1, 0xFF, 0x39, 0xE2 };
            this.Base58Prefixes[(int)Base58Type.CONFIRMATION_CODE] = new byte[] { 0x64, 0x3B, 0xF6, 0xA8, 0x9A };
            this.Base58Prefixes[(int)Base58Type.STEALTH_ADDRESS] = new byte[] { 0x2a };
            this.Base58Prefixes[(int)Base58Type.ASSET_ID] = new byte[] { 23 };
            this.Base58Prefixes[(int)Base58Type.COLORED_ADDRESS] = new byte[] { 0x13 };

            this.Checkpoints = new Dictionary<int, CheckpointInfo>
            {
                { 0, new CheckpointInfo(new uint256("0x02a8be139ec629b13df22e7abc7f9ad5239df39efaf2f5bf3ab5e4d102425dbe"), new uint256("0x0000000000000000000000000000000000000000000000000000000000000000")) },
                { 2, new CheckpointInfo(new uint256("0x49c9ce21a7916a4026e782a4728ba626b02cc6caf35615c4d7c9400ad22b5282"), new uint256("0xeda93767b1d501313d9dd2cc77e1dbb256101b351eb17e3a4ab9663d3f0a3cd3")) }, // Premine
                { 35, new CheckpointInfo(new uint256("0x477d36da0993b3a5e279fd0eba7ab4825b4ff54f0c3e55b8df4e7e6c1afe6939"), new uint256("0xa5bef352cb2182f7ca80f5d6d7a4e6ce4325bcd78bab63979d4ec8871e95a53d")) },
                { 2500, new CheckpointInfo(new uint256("0x49a2d1719097b5d9ec81d89627eaa71dfefb158cb0bc0ac58051d5ca0089dd98"), new uint256("0xf6494f64e49e8e9f6092686c78af20b7eb868bee6f0ae6a97da40b4dc06e84a7")) },
                { 4000, new CheckpointInfo(new uint256("0xbd4c0a8c11431012f1b59be225b5913a1f06e1225e85a10216f2be5db1b4c0f1"), new uint256("0x79dca584714897d88de42e9540e1bdabe8df0e5fa17473014c529385b64f7c1e")) },
                { 26500, new CheckpointInfo(new uint256("0x089f81d221d96e69609de4c5c20bfa9e477e9031599be0bf034d833fa2cd2d98"), new uint256("0x6caa0eb578196f8e0cf9fcea72d9cf4db2c75927cde1997b49bb635dc81e63a5")) }
            };

            var encoder = new Bech32Encoder("bc");
            this.Bech32Encoders = new Bech32Encoder[2];
            this.Bech32Encoders[(int)Bech32Type.WITNESS_PUBKEY_ADDRESS] = encoder;
            this.Bech32Encoders[(int)Bech32Type.WITNESS_SCRIPT_ADDRESS] = encoder;

            this.DNSSeeds = new List<DNSSeedData>
            {
                new DNSSeedData("impleum.com", "impleum.com"),
                new DNSSeedData("explorer.impleum.com", "explorer.impleum.com")
            };

            string[] seedNodes = { "109.108.77.134", "62.80.181.141" };
            this.SeedNodes = this.ConvertToNetworkAddresses(seedNodes, this.DefaultPort).ToList();



            //MineGenesis(genesis,consensus);

            Assert(this.Consensus.HashGenesisBlock == uint256.Parse("0x02a8be139ec629b13df22e7abc7f9ad5239df39efaf2f5bf3ab5e4d102425dbe"));
            Assert(this.Genesis.Header.HashMerkleRoot == uint256.Parse("0xbd3233dd8d4e7ce3ee8097f4002b4f9303000a5109e02a402d41d2faf74eb244"));


        }

        protected static Block CreateImpleumGenesisBlock(ConsensusFactory consensusFactory, uint nTime, uint nNonce, uint nBits, int nVersion, Money genesisReward)
        {
            string pszTimestamp =
                "https://cryptocrimson.com/news/apple-payment-request-api-ripple-interledger-protocol";
            Transaction txNew = consensusFactory.CreateTransaction();
            txNew.Version = 1;
            txNew.Time = nTime;
            txNew.AddInput(new TxIn()
            {
                ScriptSig = new Script(Op.GetPushOp(0), new Op()
                {
                    Code = (OpcodeType)0x1,
                    PushData = new[] { (byte)63 }
                }, Op.GetPushOp(Encoders.ASCII.DecodeData(pszTimestamp)))
            });
            txNew.AddOutput(new TxOut()
            {
                Value = genesisReward,
            });
            Block genesis = consensusFactory.CreateBlock();
            genesis.Header.BlockTime = Utils.UnixTimeToDateTime(nTime);
            genesis.Header.Bits = nBits;
            genesis.Header.Nonce = nNonce;
            genesis.Header.Version = nVersion;
            genesis.Transactions.Add(txNew);
            genesis.Header.HashPrevBlock = uint256.Zero;
            genesis.UpdateMerkleRoot();
            return genesis;
        }
    }
}
