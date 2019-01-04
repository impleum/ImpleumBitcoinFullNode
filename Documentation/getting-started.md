

# Getting started - Building and running a Impleum Full Node 

---------------

## Supported Platforms

* <b>Windows</b> - works from Windows 7 and later, on both x86 and x64 architecture. Most of the development and testing is happening here.
* <b>Linux</b> - works and Ubuntu 14.04 and later (x64). It's been known to run on some other distros so your mileage may vary.
* <b>MacOS</b> - works from OSX 10.12 and later. 

## Prerequisites

To install and run the node, you need
* [.NET Core 2.1](https://www.microsoft.com/net/download/core)
* [Git](https://git-scm.com/)

## Build instructions

### Get the repository and its dependencies

```
git clone https://github.com/impleum/ImpleumBitcoinFullNode.git  
cd ImpleumBitcoinFullNode/src
```

### Build and run the code
With this node, you can connect to either the Impleum network or the Bitcoin network, either on MainNet or TestNet.
So you have 4 options:

1. To run a <b>Impleum</b> node on <b>MainNet</b>, do
```
cd Impleum.ImpleumD
dotnet run
```  

2. To run a <b>Impleum</b>  node on <b>TestNet</b>, do
```
cd Impleum.ImpleumD
dotnet run -testnet
```  

3. To run a <b>Bitcoin</b> node on <b>MainNet</b>, do
```
cd Impleum.BitcoinD
dotnet run
```  

4. To run a <b>Bitcoin</b> node on <b>TestNet</b>, do
```
cd Impleum.BitcoinD
dotnet run -testnet
```  

### Advanced options

You can get a list of command line arguments to pass to the node with the -help command line argument. For example:
```
cd Impleum.ImpleumD
dotnet run -help
```  

### Script
We have a nifty little script that can execute all the previous commands for you, including starting the node.  
You just need to edit the file and specify whether you want to run a Impleum or a Bitcoin node, on MainNet or Testnet.  
It's located [here](https://gist.github.com/bokobza/e68832f5d7d4102bcb33fcde8d9a72fb#file-build-and-run-a-stratis-node-ps1).

### Faucet
If you need testnet funds (TIMPL) for testing there is a faucet located [here](https://faucet.impleum.com/).

Docker Containers
-------------------

Two containers are available [here](https://hub.docker.com/u/stratisplatform/)

- impleum-node: Run on the Bitcoin Main or Test networks
- impleum-node-sim: Join our simulation network

Swagger Endpoints
-------------------

Once the node is running, a Swagger interface (web UI for testing an API) is available.

* For Bitcoin: http://localhost:37220/swagger/
* For Impleum: http://localhost:38222/swagger/
* For Bitcoin Testnet: http://localhost:38220/swagger/
* For Impleum Testnet: http://localhost:39222/swagger/
