@echo off
chcp 65001 > nul
cls
echo ========================================================
echo       🎲 Masabaşı Oyun Kulübü Sistemi Başlatılıyor...
echo ========================================================
echo.

REM Angular CLI Analytics promptunu kapat
set "NG_CLI_ANALYTICS=false"

echo [1/5] Eski açık servisler temizleniyor (Eğer varsa)...
taskkill /FI "WINDOWTITLE eq Masabasi-*" /F /T > nul 2>&1
taskkill /F /IM "Masabasi.API.exe" /T > nul 2>&1
taskkill /F /IM "cloudflared.exe" /T > nul 2>&1

REM Eğer .env dosyası yoksa ve .env.example varsa kopyala
if not exist "%~dp0.env" (
    if exist "%~dp0.env.example" (
        echo [.env] .env dosyası bulunamadı, .env.example dosyasından oluşturuluyor...
        copy "%~dp0.env.example" "%~dp0.env" > nul
    )
)

REM Eğer .env dosyası varsa ortama aktar
if exist "%~dp0.env" (
    echo [.env] Ortam değişkenleri yükleniyor...
    for /f "usebackq tokens=1* delims==" %%a in (`findstr /v "^#" "%~dp0.env"`) do (
        if not "%%a"=="" set "%%a=%%b"
    )
)
echo.

echo [2/5] Veritabanı (PostgreSQL 17) servisi kontrol ediliyor...
docker compose up -d postgres > nul 2>&1
if %errorlevel% equ 0 (
    echo [Docker] PostgreSQL 17 konteyneri başarıyla aktif edildi.
) else (
    echo [Bilgi] Docker servisi kapalı veya kurulu değil; API statik/yerel modda devam edebilir.
)

echo.
echo [3/5] .NET 10 Çözümü derleniyor (Masabasi.slnx)...
dotnet build "%~dp0Masabasi.slnx" -c Debug
if %errorlevel% neq 0 (
    echo [HATA] .NET 10 Çözümü derlenemedi! Lütfen derleme hatalarını kontrol edin.
    pause
    exit /b %errorlevel%
)

echo.
echo [4/5] Servis konsol pencereleri açılıyor...

REM 1. .NET 10 Web API
start "Masabasi-API" cmd /k "cd /d "%~dp0src\Masabasi.API" && dotnet run --no-build"

REM 2. Angular 20/21 Frontend Vitrin ve Admin
start "Masabasi-UI" cmd /k "cd /d "%~dp0src\Masabasi.Frontend" && npm start"

REM 3. Cloudflare Quick Tunnel (Dış dünyaya canlı test tüneli)
if exist "%~dp0cloudflared.exe" (
    start "Masabasi-Tunnel" cmd /k "cd /d "%~dp0" && .\cloudflared.exe tunnel --url http://localhost:4200"
)

echo.
echo [5/5] Tarayıcı açılıyor...
ping 127.0.0.1 -n 7 > nul
start http://localhost:4200
start http://localhost:5000/scalar/v1

echo.
echo ========================================================
echo 🎲 Masabaşı Oyun Kulübü Sistemi Başarıyla Başlatıldı!
echo --------------------------------------------------------
echo 🌐 Web Vitrin (Angular): http://localhost:4200
echo 📚 Oyun Kütüphanesi:    http://localhost:4200/oyun-kutuphanesi
echo 🧙 FRP D&D 5e Rehberi:   http://localhost:4200/frp-dnd-rehberi
echo 📖 Topic Cluster Blog:  http://localhost:4200/blog
echo 🛡️ İzole Admin Paneli:   http://localhost:4200/admin
echo ⚙️ .NET 10 API & Scalar: http://localhost:5000/scalar/v1
echo ❤️ API Sağlık Durumu:   http://localhost:5000/health
echo ☁️ Cloudflare Tüneli:    "Masabasi-Tunnel" konsolunda açık
echo ========================================================
echo Bu pencereyi kapatabilirsiniz. Servisler arka planda çalışmaktadır.
pause
