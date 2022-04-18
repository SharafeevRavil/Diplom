FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ScaApi/ScaApi.csproj", "ScaApi/"]
RUN dotnet restore "ScaApi/ScaApi.csproj"
COPY . .
WORKDIR "/src/ScaApi"
RUN dotnet build "ScaApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ScaApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ScaApi.dll"]
