FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/Producer/Producer.csproj", "src/Producer/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]
COPY ["src/Observability/Observability.csproj", "src/Observability/"]
RUN dotnet restore "src/Producer/Producer.csproj"
COPY . .
RUN dotnet build "src/Producer/Producer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/Producer/Producer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Producer.dll"]