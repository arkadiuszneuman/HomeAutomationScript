FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# copy everything and build the project
COPY . ./
RUN dotnet restore AutomationRunner/*.csproj
RUN dotnet publish AutomationRunner/*.csproj -c Release -o out

# build runtime image
FROM mcr.microsoft.com/dotnet/core/runtime:3.1
WORKDIR /app
COPY --from=build-env /app/out ./
ENTRYPOINT ["dotnet", "AutomationRunner.dll"]