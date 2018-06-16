using System;
using System.Linq;
using NBitcoin.DataEncoders;
using NBitcoin.Networks;

namespace NBitcoin
{
    public partial class Network
    {
        /// <summary> Bitcoin maximal value for the calculated time offset. If the value is over this limit, the time syncing feature will be switched off. </summary>
        public const int BitcoinMaxTimeOffsetSeconds = 70 * 60;

        /// <summary> Stratis maximal value for the calculated time offset. If the value is over this limit, the time syncing feature will be switched off. </summary>
        public const int StratisMaxTimeOffsetSeconds = 25 * 60;

        /// <summary> Impleum maximal value for the calculated time offset. If the value is over this limit, the time syncing feature will be switched off. </summary>
        public const int ImpleumMaxTimeOffsetSeconds = 25 * 60;

        /// <summary> Bitcoin default value for the maximum tip age in seconds to consider the node in initial block download (24 hours). </summary>
        public const int BitcoinDefaultMaxTipAgeInSeconds = 24 * 60 * 60;

        /// <summary> Stratis default value for the maximum tip age in seconds to consider the node in initial block download (2 hours). </summary>
        public const int StratisDefaultMaxTipAgeInSeconds = 2 * 60 * 60;

        /// <summary> Impleum default value for the maximum tip age in seconds to consider the node in initial block download (2 hours). </summary>
        public const int ImpleumDefaultMaxTipAgeInSeconds = 24 * 60 * 60;

        /// <summary> The name of the root folder containing the different Bitcoin blockchains (Main, TestNet, RegTest). </summary>
        public const string BitcoinRootFolderName = "bitcoin";

        /// <summary> The default name used for the Bitcoin configuration file. </summary>
        public const string BitcoinDefaultConfigFilename = "bitcoin.conf";

        /// <summary> The name of the root folder containing the different Stratis blockchains (StratisMain, StratisTest, StratisRegTest). </summary>
        public const string StratisRootFolderName = "stratis";

        /// <summary> The default name used for the Stratis configuration file. </summary>
        public const string StratisDefaultConfigFilename = "stratis.conf";

        /// <summary> The name of the root folder containing the different Impleum blockchains (ImpleumMain, ImpleumTest, ImpleumRegTest). </summary>
        public const string ImpleumRootFolderName = "Impleum";


        /// <summary> The default name used for the Impleum configuration file. </summary>
        public const string ImpleumDefaultConfigFilename = "Impleum.conf";

        public static Network Main => Network.GetNetwork("Main") ?? Register(new BitcoinMain());

        public static Network TestNet => GetNetwork("TestNet") ?? Register(new BitcoinTest());

        public static Network RegTest => GetNetwork("RegTest") ?? Register(new BitcoinRegTest());

        public static Network StratisMain => GetNetwork("StratisMain") ?? Register(new StratisMain());

        public static Network StratisTest => GetNetwork("StratisTest") ?? Register(new StratisTest());

        public static Network StratisRegTest => GetNetwork("StratisRegTest") ?? Register(new StratisRegTest());

        public static Network ImpleumMain => Network.GetNetwork(nameof(ImpleumMain)) ?? Register(new ImpleumMain());

        public static Network ImpleumTest => Network.GetNetwork(nameof(ImpleumTest)) ?? Register(new ImpleumTest());

        public static Network ImpleumRegTest => Network.GetNetwork(nameof(ImpleumRegTest)) ?? Register(new ImpleumRegTest());

        protected static Block CreateGenesisBlock(ConsensusFactory consensusFactory, uint nTime, uint nNonce, uint nBits, int nVersion, Money genesisReward)
        {
            string pszTimestamp = "The Times 03/Jan/2009 Chancellor on brink of second bailout for banks";
            var genesisOutputScript = new Script(Op.GetPushOp(Encoders.Hex.DecodeData("04678afdb0fe5548271967f1a67130b7105cd6a828e03909a67962e0ea1f61deb649f6bc3f4cef38c4f35504e51ec112de5c384df7ba0b8d578a4c702b6bf11d5f")), OpcodeType.OP_CHECKSIG);

            Transaction txNew = consensusFactory.CreateTransaction();
            txNew.Version = 1;
            txNew.AddInput(new TxIn()
            {
                ScriptSig = new Script(Op.GetPushOp(486604799), new Op()
                {
                    Code = (OpcodeType)0x1,
                    PushData = new[] { (byte)4 }
                }, Op.GetPushOp(Encoders.ASCII.DecodeData(pszTimestamp)))
            });
            txNew.AddOutput(new TxOut()
            {
                Value = genesisReward,
                ScriptPubKey = genesisOutputScript
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

        protected static Block CreateStratisGenesisBlock(ConsensusFactory consensusFactory, uint nTime, uint nNonce,
            uint nBits, int nVersion, Money genesisReward)
        {
            string pszTimestamp =
                "http://www.theonion.com/article/olympics-head-priestess-slits-throat-official-rio--53466";

            Transaction txNew = consensusFactory.CreateTransaction();
            txNew.Version = 1;
            txNew.Time = nTime;
            txNew.AddInput(new TxIn()
            {
                ScriptSig = new Script(Op.GetPushOp(0), new Op()
                {
                    Code = (OpcodeType)0x1,
                    PushData = new[] { (byte)42 }
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


        private static void MineGenesis(Block genesis)
        {

            // This will figure out a valid hash and Nonce if you're creating a different genesis block:
            uint256 hashTarget = new uint256("00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff");
            //Target
            uint256 newhash = genesis.GetHash();
            uint256 besthash = new uint256(Enumerable.Repeat((byte)0xFF, 32).ToArray());
            while (newhash > hashTarget)
            {
                ++genesis.Header.Nonce;
                if (genesis.Header.Nonce == 0)
                {
                    //NONCE WRAPPED incrementing time
                    ++genesis.Header.Time;
                }

                newhash = genesis.GetHash();
                if (newhash < besthash)
                {
                    besthash = newhash;
                    //New best hex
                }
            }

            Console.WriteLine($"Found Genesis, Nonce: {genesis.Header.Nonce}, Hash: {genesis.GetHash()}\n");
            Console.WriteLine($"Gensis Hash Merkle: {genesis.GetMerkleRoot().Hash}");
        }

    }
}
