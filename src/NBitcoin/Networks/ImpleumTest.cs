using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using NBitcoin.BouncyCastle.Math;
using NBitcoin.DataEncoders;
using NBitcoin.Protocol;

namespace NBitcoin.Networks
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
            this.DefaultPort = 28171;
            this.RPCPort = 28172;

            this.Consensus.PowLimit = new Target(new uint256("0000ffff00000000000000000000000000000000000000000000000000000000"));
            this.Consensus.DefaultAssumeValid = null;

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
                new NetworkAddress(IPAddress.Parse("109.108.77.134"), this.DefaultPort),
                new NetworkAddress(IPAddress.Parse("62.80.181.141"), this.DefaultPort)
            };

            // Create the genesis block.
            this.GenesisTime = 1523364655;
            this.GenesisNonce = 2380297;
            this.GenesisBits = 0x1e0fffff;
            this.GenesisVersion = 1;
            this.GenesisReward = Money.Zero;

            this.Genesis = Network.CreateImpleumGenesisBlock(this.Consensus.ConsensusFactory, this.GenesisTime, this.GenesisNonce, this.GenesisBits, this.GenesisVersion, this.GenesisReward);
            this.Genesis.Header.Time = 1527405749;
            this.Genesis.Header.Nonce = 2833935;
            this.Genesis.Header.Bits = this.Consensus.PowLimit;
            this.Consensus.HashGenesisBlock = this.Genesis.GetHash();
            Network.Assert(this.Consensus.HashGenesisBlock == uint256.Parse("0xd73b0d8d3b98b638ff8b955bf9a74febba4b145081933dadd4e9cd8766df389b"));


        }
    }
}
