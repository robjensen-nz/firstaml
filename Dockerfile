FROM mcr.microsoft.com/dotnet/sdk:10.0

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=1

WORKDIR /workspace

COPY global.json FirstAML.sln ./
COPY src/FirstAML/FirstAML.csproj src/FirstAML/
COPY tests/FirstAML.Tests/FirstAML.Tests.csproj tests/FirstAML.Tests/
RUN dotnet restore FirstAML.sln

COPY src/ src/
COPY tests/ tests/

CMD ["dotnet", "test", "FirstAML.sln", "--configuration", "Release", "--no-restore"]
