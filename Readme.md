# Test project for Gameteq

## Description

Console client-server chat application. The application operates on the TCP/IP protocol. The message history is a maximum of 10 messages during the lifetime of the application.

## Build

### Requirements to run or build 

 - .Net 8.0
 - Linux/Windows/macOS

### Run from release

Download the required build for your architecture from the Releases section. Then start the server and client one by one. Uses `127.0.0.1:3333` address by default.

To run on the Windows platform, exe files are used. For other platforms or if you want to change the default address, use the following commands.

Run the server.

```
./TestProject.Gameteq.Server 127.0.0.1 3333
```

Run the client.

```
./TestProject.Gameteq 127.0.0.1 3333
```

### Run without build

To run without assembly, you need to install .NET 8 from the official website and clone this repository. Then use the following commands.

Run the server.

```
cd TestProject.Gameteq && cd TestProject.Gameteq.Server
dotnet run 127.0.0.1 3333
```

Run the client.

```
cd TestProject.Gameteq && cd TestProject.Gameteq
dotnet run 127.0.0.1 3333
```

### Build from source

#### Window

```
dotnet publish -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish
```

#### Linux

```
dotnet publish -r linux-x64 --self-contained true -p:PublishSingleFile=true -o publish
```

#### macOS x64

```
dotnet publish -r osx-x64 --self-contained true -p:PublishSingleFile=true -o publish
```

#### macOS arm64
```
dotnet publish -r osx-arm64 --self-contained true -p:PublishSingleFile=true -o publish
```
