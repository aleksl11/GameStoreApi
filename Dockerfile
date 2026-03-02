FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["GameStore.Api/GameStore.Api.csproj", ""]
RUN dotnet restore 'GameStore.Api.csproj'

COPY ["GameStore.Api/", ""]
RUN dotnet restore 'GameStore.Api.csproj'
RUN dotnet build 'GameStore.Api.csproj' -c Release -o /app/build

FROM build AS publish
RUN dotnet publish 'GameStore.Api.csproj' -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/sdk:10.0
ENV ASPNETCORE_HTTP_PORTS=5001
EXPOSE 5001
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "GameStore.Api.dll" ]