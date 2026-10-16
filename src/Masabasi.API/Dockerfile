# ==============================================================================
# Aşama 1: .NET 10 SDK ile Derleme (Build & Publish)
# ==============================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview-alpine AS build
WORKDIR /app

# Proje dosyalarını kopyala ve restore et (Docker layer caching için)
COPY src/Masabasi.Data/Masabasi.Data.csproj src/Masabasi.Data/
COPY src/Masabasi.Business/Masabasi.Business.csproj src/Masabasi.Business/
COPY src/Masabasi.API/Masabasi.API.csproj src/Masabasi.API/
RUN dotnet restore src/Masabasi.API/Masabasi.API.csproj

# Tüm kaynak kodları kopyala ve Release olarak derle
COPY src/Masabasi.Data/ src/Masabasi.Data/
COPY src/Masabasi.Business/ src/Masabasi.Business/
COPY src/Masabasi.API/ src/Masabasi.API/
RUN dotnet publish src/Masabasi.API/Masabasi.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# ==============================================================================
# Aşama 2: Hafif ASP.NET Core 10 Alpine Çalışma Zamanı (Runtime)
# ==============================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview-alpine AS runtime
WORKDIR /app

# Güvenlik için yetkisiz kullanıcı oluştur
RUN addgroup -S masabasi && adduser -S masabasi -G masabasi
USER masabasi

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["dotnet", "Masabasi.API.dll"]
