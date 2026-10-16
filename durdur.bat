@echo off
cd /d "%~dp0"
cls

echo ========================================================
echo       Masabasi Oyun Kulubu Sistemi Durduruluyor...
echo ========================================================
echo.

echo [1/3] Docker servisleri durduruluyor...
docker compose stop > nul 2>&1

echo.
echo [2/3] Calisan Masabasi konsol pencereleri kapatiliyor...
taskkill /FI "WINDOWTITLE eq Masabasi-API*" /F /T > nul 2>&1
taskkill /FI "WINDOWTITLE eq Masabasi-UI*" /F /T > nul 2>&1
taskkill /FI "WINDOWTITLE eq Masabasi-Tunnel*" /F /T > nul 2>&1
taskkill /FI "WINDOWTITLE eq Masabasi-*" /F /T > nul 2>&1
taskkill /F /IM "cloudflared.exe" /T > nul 2>&1
taskkill /F /IM "Masabasi.API.exe" /T > nul 2>&1

echo.
echo [3/3] Arka plandaki ilgili gelistirme prosesleri temizleniyor...
taskkill /F /FI "IMAGENAME eq dotnet.exe" /T > nul 2>&1
taskkill /F /FI "IMAGENAME eq node.exe" /T > nul 2>&1

echo.
echo ========================================================
echo Masabasi Oyun Kulubu Sistemi basariyla durduruldu.
echo ========================================================
pause
