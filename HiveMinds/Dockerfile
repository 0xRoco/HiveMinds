FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["HiveMinds/HiveMinds.csproj", "HiveMinds/"]
RUN dotnet restore "HiveMinds/HiveMinds.csproj"
COPY . .
WORKDIR "/src/HiveMinds"
RUN dotnet build "HiveMinds.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HiveMinds.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HiveMinds.dll"]
