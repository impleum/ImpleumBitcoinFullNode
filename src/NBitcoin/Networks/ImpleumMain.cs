using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBitcoin.BouncyCastle.Math;
using NBitcoin.DataEncoders;
using NBitcoin.Protocol;

namespace NBitcoin.Networks
{
    public class ImpleumMain : Network
    {
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
            this.RPCPort = 16172;
            this.MinTxFee = 10000;
            this.FallbackFee = 60000;
            this.MinRelayTxFee = 10000;
            this.MaxTimeOffsetSeconds = ImpleumMaxTimeOffsetSeconds;
            this.MaxTipAge = ImpleumDefaultMaxTipAgeInSeconds;
            this.CoinTicker = "IMP";

            this.Consensus.SubsidyHalvingInterval = 210000;
            this.Consensus.MajorityEnforceBlockUpgrade = 750;
            this.Consensus.MajorityRejectBlockOutdated = 950;
            this.Consensus.MajorityWindow = 1000;
            this.Consensus.BuriedDeployments[BuriedDeployments.BIP34] = 0;
            this.Consensus.BuriedDeployments[BuriedDeployments.BIP65] = 0;
            this.Consensus.BuriedDeployments[BuriedDeployments.BIP66] = 0;
            this.Consensus.BIP34Hash = new uint256("0x000000000000024b89b42a942fe0d9fea3bb44ab7bd1b19115dd6a759c0808b8");
            this.Consensus.PowLimit = new Target(new uint256("00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff"));
            this.Consensus.PowTargetTimespan = TimeSpan.FromSeconds(14 * 24 * 60 * 60); // two weeks
            this.Consensus.PowTargetSpacing = TimeSpan.FromSeconds(10 * 60);
            this.Consensus.PowAllowMinDifficultyBlocks = false;
            this.Consensus.PowNoRetargeting = false;
            this.Consensus.RuleChangeActivationThreshold = 1916; // 95% of 2016
            this.Consensus.MinerConfirmationWindow = 2016; // nPowTargetTimespan / nPowTargetSpacing
            this.Consensus.LastPOWBlock = 100000;
            this.Consensus.IsProofOfStake = true;
            this.Consensus.ConsensusFactory = new PosConsensusFactory() { Consensus = this.Consensus };
            this.Consensus.ProofOfStakeLimit = new BigInteger(uint256.Parse("00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff").ToBytes(false));
            this.Consensus.ProofOfStakeLimitV2 = new BigInteger(uint256.Parse("000000000000ffffffffffffffffffffffffffffffffffffffffffffffffffff").ToBytes(false));
            this.Consensus.CoinType = 769;
            this.Consensus.DefaultAssumeValid = new uint256("0xe8dd9ea41e9d8935f31a0f1bd8378a777434dc51549a1c685855a7ec4d5546b1"); // 23000
            this.Consensus.CoinbaseMaturity = 50;
            this.Consensus.PremineReward = Money.Coins(1000000);
            this.Consensus.PremineHeight = 2;
            this.Consensus.ProofOfWorkReward = Money.Coins(48);
            this.Consensus.ProofOfStakeReward = Money.COIN;
            this.Consensus.MaxReorgLength = 500;
            this.Consensus.MaxMoney = 100000000 * Money.COIN;

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

            Block genesis = CreateImpleumGenesisBlock(this.Consensus.ConsensusFactory, 1523364655, 2380297, 0x1e0fffff, 1, Money.Zero);
            this.Consensus.HashGenesisBlock = genesis.GetHash();
            this.Genesis = genesis;

            //MineGenesis(genesis,consensus);

            Network.Assert(this.Consensus.HashGenesisBlock == uint256.Parse("0x02a8be139ec629b13df22e7abc7f9ad5239df39efaf2f5bf3ab5e4d102425dbe"));
            Network.Assert(genesis.Header.HashMerkleRoot == uint256.Parse("0xbd3233dd8d4e7ce3ee8097f4002b4f9303000a5109e02a402d41d2faf74eb244"));


        }
    }
}
