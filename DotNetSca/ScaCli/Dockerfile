FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ScaCli/ScaCli.csproj", "ScaCli/"]
RUN dotnet restore "ScaCli/ScaCli.csproj"
COPY . .
WORKDIR "/src/ScaCli"
RUN dotnet build "ScaCli.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ScaCli.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ScaCli.dll"]
