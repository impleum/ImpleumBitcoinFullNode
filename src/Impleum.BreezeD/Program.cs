using System;
using System.Linq;
using System.Threading.Tasks;
using NBitcoin.Protocol;
using Stratis.Bitcoin;
using Stratis.Bitcoin.Builder;
using Stratis.Bitcoin.Configuration;
using Stratis.Bitcoin.Features.Api;
using Stratis.Bitcoin.Features.BlockStore;
using Stratis.Bitcoin.Features.Consensus;
using Stratis.Bitcoin.Features.LightWallet;
using Stratis.Bitcoin.Features.Notifications;
using Stratis.Bitcoin.Networks;
using Stratis.Bitcoin.Utilities;

namespace Impleum.PrivacyD
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                bool isImpleum = args.Contains("impleum");

                NodeSettings nodeSettings;

                IFullNodeBuilder fullNodeBuilder = null;

                if (!args.Any(a => a.Contains("datadirroot")))
                    args = args.Concat(new[] { "-datadirroot=ImpleumNode" }).ToArray();

                if (isImpleum)
                {
                    nodeSettings = new NodeSettings(networksSelector: Networks.Impleum, protocolVersion: ProtocolVersion.PROVEN_HEADER_VERSION, agent: "Lite", args: args)
                    {
                        MinProtocolVersion = ProtocolVersion.ALT_PROTOCOL_VERSION
                    };

                    fullNodeBuilder = new FullNodeBuilder()
                                    .UseNodeSettings(nodeSettings)
                                    .UseApi()
                                    .UseBlockStore()
                                    .UsePosConsensus()
                                    .UseLightWallet()
                                    .UseBlockNotification()
                                    .UseTransactionNotification();
                }
                else
                {
                    nodeSettings = new NodeSettings(networksSelector: Networks.Bitcoin, agent: "Lite", args: args);

                    fullNodeBuilder = new FullNodeBuilder()
                                    .UseNodeSettings(nodeSettings)
                                    .UseApi()
                                    .UseBlockStore()
                                    .UsePowConsensus()
                                    .UseLightWallet()
                                    .UseBlockNotification()
                                    .UseTransactionNotification();
                }

                IFullNode node = fullNodeBuilder.Build();

                await node.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There was a problem initializing the node: '{ex}'");
            }
        }
    }
}
