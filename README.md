
Impleum Bitcoin
===============

https://impleum.com

Bitcoin Implementation in C#
----------------------------

Impleum is an implementation of the Bitcoin protocol in C# on the [.NET Core](https://dotnet.github.io/) platform.  
The node can run on the Bitcoin and Impleum networks.  
Impleum Bitcoin is based on the [NBitcoin](https://github.com/MetacoSA/NBitcoin) project.  

For Proof of Stake support on the Impleum token the node is using [NStratis](https://github.com/stratisproject/NStratis) which is a POS implementation of NBitcoin.  

[.NET Core](https://dotnet.github.io/) is an open source cross platform framework and enables the development of applications and services on Windows, macOS and Linux.  
Join our community on [discord](https://discord.gg/AyV8Ssa).  

The design
----------

**A Modular Approach**

A Blockchain is made of many components, from a FullNode that validates blocks to a Simple Wallet that track addresses.
The end goal is to develop a set of [Nuget](https://en.wikipedia.org/wiki/NuGet) packages from which an implementer can cherry pick what he needs.

* **NBitcoin**
* **Impleum.Bitcoin.Core**  - The bare minimum to run a pruned node.
* **Impleum.Bitcoin.Store** - Store and relay blocks to peers.
* **Impleum.Bitcoin.MemoryPool** - Track pending transaction.
* **Impleum.Bitcoin.Wallet** - Send and Receive coins
* **Impleum.Bitcoin.Miner** - POS or POW
* **Impleum.Bitcoin.Explorer**


Create a Blockchain in a .NET Core style programming
```
  var node = new FullNodeBuilder()
   .UseNodeSettings(nodeSettings)
   .UseConsensus()
   .UseBlockStore()
   .UseMempool()
   .AddMining()
   .AddRPC()
   .Build();

  node.Run();
```

What's Next
----------

We plan to add many more features on top of the Impleum Bitcoin blockchain:
Sidechains, Private/Permissioned blockchain, Compiled Smart Contracts, NTumbleBit/Breeze wallet and more...

Running a FullNode
------------------

Our full node is currently in alpha.  

```
git clone https://github.com/impleum/ImpleumBitcoinFullNode.git  
cd ImpleumBitcoinFullNode\src

dotnet build

```

To run on the Bitcoin network:
```
cd Impleum.BitcoinD
dotnet run
```  

To run on the Stratis network:
```
cd Impleum.ImpleumD
dotnet run
```  

Getting Started Guide
-----------
More details on getting started are available [here](https://github.com/impleum/ImpleumBitcoinFullNode/blob/master/Documentation/getting-started.md)

Development
-----------
Up for some blockchain development?

Check this guides for more info:
* [Contributing Guide](Documentation/contributing.md)
* [Coding Style](Documentation/coding-style.md)
* [Wiki Page](https://stratisplatform.atlassian.net/wiki/spaces/WIKI/overview)

There is a lot to do and we welcome contributers developers and testers who want to get some Blockchain experience.
You can find tasks at the issues/projects or visit our [C# dev](https://discord.gg/AyV8Ssa) discord channel.

Testing
-------
* [Testing Guidelines](Documentation/testing-guidelines.md)
