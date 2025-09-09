# --- build stage ---
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 1) только csproj — кэшируем restore
COPY backend/Api/PlatinumDev.KnittingAIWebAPI.csproj backend/Api/
RUN dotnet restore backend/Api/PlatinumDev.KnittingAIWebAPI.csproj

# 2) теперь весь код
COPY . .

# 2.1) быстрая диагностика: убедиться, что Program.cs на месте
RUN test -f backend/Api/Program.cs || (echo "MISSING Program.cs"; ls -la backend/Api; exit 1)

WORKDIR /src/backend/Api
RUN dotnet publish PlatinumDev.KnittingAIWebAPI.csproj -c Release -o /out

# --- runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /out ./
EXPOSE 8080
# Чтобы слушать 8080 внутри контейнера
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENTRYPOINT ["dotnet", "PlatinumDev.KnittingAIWebAPI.dll"]
