FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AulaVirtualBenchmark/AulaVirtualBenchmark.csproj", "AulaVirtualBenchmark/"]
RUN dotnet restore "AulaVirtualBenchmark/AulaVirtualBenchmark.csproj"
COPY . .
WORKDIR "/src/AulaVirtualBenchmark"
RUN dotnet build "AulaVirtualBenchmark.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AulaVirtualBenchmark.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AulaVirtualBenchmark.dll"]
