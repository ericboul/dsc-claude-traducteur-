@echo off
setlocal
set "ROOT=%~dp0"
set "OLLAMA_MODELS=%ROOT%data\ollama\data"
set "OLLAMA_HOST=127.0.0.1:11434"
set "OLLAMA_FLASH_ATTENTION=1"
set "OLLAMA_KV_CACHE_TYPE=q8_0"
set "OLLAMA_NUM_PARALLEL=1"
set "OLLAMA_MAX_LOADED_MODELS=1"
set "OLLAMA_KEEP_ALIVE=30m"

powershell -NoProfile -Command "if (-not (Get-NetTCPConnection -LocalPort 11434 -State Listen -ErrorAction SilentlyContinue)) { Start-Process -FilePath '%ROOT%data\ollama\ollama.exe' -ArgumentList 'serve' -WindowStyle Hidden }" >nul 2>&1

"%ROOT%data\ollama\ollama.exe" run qwen3.5-9b-uncensored
