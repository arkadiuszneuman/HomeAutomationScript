FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 5000

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["DataCollector/DataCollector.csproj", "DataCollector/"]
RUN dotnet restore "DataCollector/DataCollector.csproj"
COPY . .
WORKDIR "/src/DataCollector"
RUN dotnet build "DataCollector.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "DataCollector.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "DataCollector.dll"]