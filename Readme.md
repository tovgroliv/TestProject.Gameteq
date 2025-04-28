# Test project for Gameteq

## Description

Console client-server chat application. The application operates on the TCP/IP protocol. The message history is a maximum of 10 messages during the lifetime of the application.

## Build

### Requirements to run or build 

 - .Net 8.0
 - Linux/Windows/macOS

### Run

Download from the Releases section or build it yourself using the following command.

```
dotnet run
```

### Build

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
