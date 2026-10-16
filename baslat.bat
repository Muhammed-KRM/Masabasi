@echo off
setlocal EnableDelayedExpansion
cd /d "%~dp0"
cls

echo ========================================================
echo       Masabasi Oyun Kulubu Sistemi Baslatiliyor...
echo ========================================================
echo.

set "NG_CLI_ANALYTICS=false"

echo [1/5] Eski acik servisler temizleniyor (Eger varsa)...
taskkill /FI "WINDOWTITLE eq Masabasi-*" /F /T > nul 2>&1
taskkill /F /IM "Masabasi.API.exe" /T > nul 2>&1
taskkill /F /IM "cloudflared.exe" /T > nul 2>&1

if not exist ".env" (
    if exist ".env.example" (
        echo [.env] .env dosyasi bulunamadi, .env.example dosyasindan olusturuluyor...
        copy ".env.example" ".env" > nul
    )
)

if exist ".env" (
    echo [.env] Ortam degiskenleri yukleniyor...
    for /f "usebackq tokens=1* delims==" %%a in (`findstr /v "^#" ".env"`) do (
        if not "%%a"=="" set "%%a=%%b"
    )
)
echo.

echo [2/5] Veritabani servisi kontrol ediliyor...
docker compose up -d postgres > nul 2>&1
if %errorlevel% equ 0 (
    echo [Docker] PostgreSQL 17 konteyneri basariyla aktif edildi.
) else (
    echo [Bilgi] Docker servisi kapali veya kurulu degil; API statik snapshot moduyla calisabilir.
)

echo.
echo [3/5] .NET 10 Cozumu derleniyor (Masabasi.slnx)...
dotnet build "Masabasi.slnx" -c Debug
if %errorlevel% neq 0 (
    echo [HATA] .NET 10 Cozumu derlenemedi!
    pause
    exit /b %errorlevel%
)

echo.
echo [4/5] Servis konsol pencereleri aciliyor...

start "Masabasi-API" cmd /k "cd /d "%~dp0src\Masabasi.API" && dotnet run --no-build"
start "Masabasi-UI" cmd /k "cd /d "%~dp0src\Masabasi.Frontend" && npm start"

if exist "cloudflared.exe" (
    start "Masabasi-Tunnel" cmd /k "cd /d "%~dp0" && .\cloudflared.exe tunnel --url http://localhost:4000"
)

echo.
echo [5/5] Arayuzun hazir olmasi bekleniyor ve tarayici aciliyor...
powershell -NoProfile -Command "for ($i=0; $i -lt 30; $i++) { try { $r = (Invoke-WebRequest -Uri 'http://localhost:4000' -UseBasicParsing -TimeoutSec 1).StatusCode; if ($r -ge 200 -and $r -lt 400) { break } } catch {}; Start-Sleep -Seconds 1 }"
start http://localhost:4000
start http://localhost:5000/scalar/v1

echo.
echo ========================================================
echo Masabasi Oyun Kulubu Sistemi Basariyla Baslatildi!
echo --------------------------------------------------------
echo Web Vitrin (Angular SSR): http://localhost:4000 (veya :4200)
echo Oyun Kutuphanesi:         http://localhost:4000/oyun-kutuphanesi
echo FRP D ve D Rehberi:       http://localhost:4000/frp-dnd-rehberi
echo Topic Cluster Blog:       http://localhost:4000/blog
echo Izole Admin Paneli:       http://localhost:4000/admin
echo .NET 10 API (Scalar):     http://localhost:5000/scalar/v1
echo API Saglik Durumu:        http://localhost:5000/health
echo Cloudflare Tuneli:        "Masabasi-Tunnel" penceresinde acik
echo ========================================================
echo Bu pencereyi kapatabilirsiniz. Servisler arka planda calismaktadir.
pause
