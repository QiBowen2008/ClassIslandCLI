cd ClassIslandCLI
export OBJCOPY=/usr/bin/objcopy
dotnet publish -r linux-x64 -c Release
export OBJCOPY=/usr/bin/aarch64-linux-gnu-objcopy
dotnet publish -r linux-arm64 -c Release
export OBJCOPY=/usr/bin/objcopy
