@echo off
setlocal
title Installation de miz-translator (DCS)
cd /d "%~dp0"

set "DEST=%~dp0miz-translator"
set "ZIP=%~dp0miz-translator-release.zip"
set "URL=https://github.com/leonchen83/miz-translator/releases/latest/download/miz-translator-release.zip"
set "JAR=%DEST%\miz-translator\lib\miz-translator-fat.jar"

if exist "%JAR%" (
    echo.
    echo miz-translator est deja installe :
    echo   %DEST%\miz-translator
    echo.
    pause
    exit /b 0
)

echo.
echo === Installation de miz-translator ===
echo Telechargement depuis GitHub (~150 Mo)...
echo.

curl -L --fail --retry 2 -o "%ZIP%" "%URL%"
if errorlevel 1 goto download_failed

echo.
echo Extraction...
powershell -NoProfile -ExecutionPolicy Bypass -Command "$ErrorActionPreference='Stop'; $dest='%DEST%'; $zip='%ZIP%'; $nested=Join-Path $dest 'miz-translator'; Expand-Archive -Path $zip -DestinationPath $dest -Force; if (-not (Test-Path (Join-Path $nested 'lib\miz-translator-fat.jar')) -and (Test-Path (Join-Path $dest 'lib\miz-translator-fat.jar'))) { $tmp=Join-Path $dest '_tmp'; New-Item -ItemType Directory -Force -Path $tmp | Out-Null; Get-ChildItem -Path $dest -Force | Where-Object { $_.Name -ne '_tmp' } | Move-Item -Destination $tmp; Rename-Item -Path $tmp -NewName 'miz-translator' }"
if errorlevel 1 goto extract_failed

del "%ZIP%" >nul 2>&1

if not exist "%JAR%" goto extract_failed

echo.
echo Installation terminee.
echo   %DEST%\miz-translator
echo.
echo Relancez OpenClaude-DCS-Traducteur.exe
echo.
pause
exit /b 0

:download_failed
echo.
echo ECHEC du telechargement.
echo Verifiez votre connexion Internet et reessayez.
echo URL : %URL%
echo.
pause
exit /b 1

:extract_failed
echo.
echo ECHEC de l'extraction de l'archive.
echo Telechargez manuellement :
echo   %URL%
echo puis extrayez le dossier dans :
echo   %DEST%
echo.
pause
exit /b 1
