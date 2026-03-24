# Development Setup

## Requirements

- Windows
- .NET SDK 9
- Going Medieval installed locally
- BepInEx 5 installed into the game directory

## Local GameDir Configuration

You can either pass `GameDir` on the command line:

```powershell
dotnet build .\runtime\SmartCarry.Runtime\SmartCarry.Runtime.csproj -c Release -p:GameDir="D:\SteamLibrary\steamapps\common\Going Medieval"
```

Or create a local, non-committed `Directory.Build.props` file in the workspace root:

```xml
<Project>
  <PropertyGroup>
    <GameDir>D:\SteamLibrary\steamapps\common\Going Medieval</GameDir>
  </PropertyGroup>
</Project>
```

## Build

```powershell
dotnet build .\smartcarry.slnx -c Release
```

## Test

```powershell
dotnet test .\runtime\SmartCarry.Runtime.Tests\SmartCarry.Runtime.Tests.csproj -c Release
```

## Deploy

Copy the built DLL to:

```text
<Going Medieval>\BepInEx\plugins\SmartCarry.Runtime\SmartCarry.Runtime.dll
```
