using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBitcoin.BouncyCastle.Math;
using NBitcoin.DataEncoders;
using NBitcoin.Protocol;
using Stratis.Bitcoin.Networks;

namespace NBitcoin.Networks
{
    public class ImpleumRegTest : ImpleumMain
    {
        public ImpleumRegTest()
        {
            var messageStart = new byte[4];
            messageStart[0] = 0xcd;
            messageStart[1] = 0xf2;
            messageStart[2] = 0xc0;
            messageStart[3] = 0xef;
            var magic = BitConverter.ToUInt32(messageStart, 0); // 0xefc0f2cd

            this.Name = nameof(ImpleumRegTest);
            this.Magic = magic;
            this.DefaultPort = 19444;
            this.RPCPort = 19442;
            this.MinTxFee = 0;
            this.FallbackFee = 0;
            this.MinRelayTxFee = 0;

            this.Consensus.PowAllowMinDifficultyBlocks = true;
            this.Consensus.PowNoRetargeting = true;
            this.Consensus.PowLimit = new Target(new uint256("7fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff"));
            this.Consensus.DefaultAssumeValid = null; // turn off assumevalid for regtest.

            this.Base58Prefixes[(int)Base58Type.PUBKEY_ADDRESS] = new byte[] { (102) };
            this.Base58Prefixes[(int)Base58Type.SCRIPT_ADDRESS] = new byte[] { (196) };
            this.Base58Prefixes[(int)Base58Type.SECRET_KEY] = new byte[] { (65 + 128) };

            this.Checkpoints = new Dictionary<int, CheckpointInfo>();
            this.DNSSeeds = new List<DNSSeedData>();
            this.SeedNodes = new List<NetworkAddress>();

            // Create the genesis block.
            this.GenesisTime = 1527013886;
            this.GenesisNonce = 2112374;
            this.GenesisBits = 0x1e0fffff;
            this.GenesisVersion = 1;
            this.GenesisReward = Money.Zero;

            this.Genesis = CreateImpleumGenesisBlock(this.Consensus.ConsensusFactory, this.GenesisTime, this.GenesisNonce, this.GenesisBits, this.GenesisVersion, this.GenesisReward);
            this.Genesis.Header.Time = 1527013886;
            this.Genesis.Header.Nonce = 2112374;
            this.Genesis.Header.Bits = this.Consensus.PowLimit;
            this.Consensus.HashGenesisBlock = this.Genesis.GetHash();
            //TODO run reg tests
            Network.Assert(this.Consensus.HashGenesisBlock == uint256.Parse("0x93925104d664314f581bc7ecb7b4bad07bcfabd1cfce4256dbd2faddcf53bd1f"));
        }
    }
}
