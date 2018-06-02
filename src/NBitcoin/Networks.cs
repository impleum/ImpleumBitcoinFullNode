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

        public static Network Main => Network.GetNetwork("Main") ?? InitMain();

        public static Network TestNet => Network.GetNetwork("TestNet") ?? Register(new BitcoinTest());

        public static Network RegTest => Network.GetNetwork("RegTest") ?? Register(new BitcoinRegTest());

        public static Network StratisMain => Network.GetNetwork("StratisMain") ?? Register(new StratisMain());

        public static Network StratisTest => Network.GetNetwork("StratisTest") ?? Register(new StratisTest());

        public static Network StratisRegTest => Network.GetNetwork("StratisRegTest") ?? Register(new StratisRegTest());


        protected static Block CreateStratisGenesisBlock(ConsensusFactory consensusFactory, uint nTime, uint nNonce, uint nBits, int nVersion, Money genesisReward)
        {
            string pszTimestamp = "http://www.theonion.com/article/olympics-head-priestess-slits-throat-official-rio--53466";

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

        private static Block CreateImpleumGenesisBlock(ConsensusFactory consensusFactory, uint nTime, uint nNonce, uint nBits, int nVersion, Money genesisReward)
        {
            string pszTimestamp = "https://cryptocrimson.com/news/apple-payment-request-api-ripple-interledger-protocol";
            return CreateImpleumGenesisBlock(consensusFactory, pszTimestamp, nTime, nNonce, nBits, nVersion, genesisReward);
        }

        private static Block CreateImpleumGenesisBlock(ConsensusFactory consensusFactory, string pszTimestamp, uint nTime, uint nNonce, uint nBits, int nVersion, Money genesisReward)
        {
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
