FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["GameStore.Api/GameStore.Api.csproj", "GameStore.Api/"]
COPY ["GameStore.Contracts/GameStore.Contracts.csproj", "GameStore.Contracts/"]

RUN dotnet restore "GameStore.Api/GameStore.Api.csproj"

COPY . .

WORKDIR "/src/GameStore.Api"
RUN dotnet publish "GameStore.Api.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

ENV ASPNETCORE_HTTP_PORTS=5001
EXPOSE 5001

WORKDIR /app

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GameStore.Api.dll"]