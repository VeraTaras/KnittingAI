# --- etap build ---
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 1) tylko csproj — cache’ujemy restore
COPY backend/Api/PlatinumDev.KnittingAIWebAPI.csproj backend/Api/
RUN dotnet restore backend/Api/PlatinumDev.KnittingAIWebAPI.csproj

# 2) teraz cały kod
COPY . .

# 2.1) szybka diagnostyka: upewniamy się, że Program.cs istnieje
RUN test -f backend/Api/Program.cs || (echo "BRAK Program.cs"; ls -la backend/Api; exit 1)

WORKDIR /src/backend/Api
RUN dotnet publish PlatinumDev.KnittingAIWebAPI.csproj -c Release -o /out

# --- etap runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /out ./
EXPOSE 8080
# Aby nasłuchiwać na porcie 8080 wewnątrz kontenera
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENTRYPOINT ["dotnet", "PlatinumDev.KnittingAIWebAPI.dll"]
