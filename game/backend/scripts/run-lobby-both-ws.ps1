param(
    [string]$WsPrefix = "http://127.0.0.1:8080/",
    [string]$Username = "player",
    [switch]$Logs
)

$serverProj = (Resolve-Path (Join-Path $PSScriptRoot "..\PinionCore.Showcases.Lobby.Server")).Path
$clientProj = (Resolve-Path (Join-Path $PSScriptRoot "..\PinionCore.Showcases.Lobby.Client")).Path

$serverArgs = "run --project `"$serverProj`" -- --ws-prefix `"$WsPrefix`""
$wsUrl = $WsPrefix.Replace("http://","ws://").Replace("https://","wss://")
$clientArgs = "run --project `"$clientProj`" -- --ws `"$wsUrl`" --username $Username"

if ($Logs) {
    $logDir = Join-Path $env:TEMP "lobby_logs"
    New-Item -ItemType Directory -Force -Path $logDir | Out-Null
    $serverOut = Join-Path $logDir "server.out.txt"; $serverErr = Join-Path $logDir "server.err.txt"
    $clientOut = Join-Path $logDir "client.out.txt"; $clientErr = Join-Path $logDir "client.err.txt"
    $server = Start-Process dotnet -ArgumentList $serverArgs -PassThru -NoNewWindow -RedirectStandardOutput $serverOut -RedirectStandardError $serverErr
    Start-Sleep -Seconds 1
    $client = Start-Process dotnet -ArgumentList $clientArgs -PassThru -NoNewWindow -RedirectStandardOutput $clientOut -RedirectStandardError $clientErr
    Write-Host "Server PID: $($server.Id)  Client PID: $($client.Id)"
    Write-Host "Logs in: $logDir"
} else {
    $server = Start-Process dotnet -ArgumentList $serverArgs -PassThru -NoNewWindow
    Start-Sleep -Seconds 1
    $client = Start-Process dotnet -ArgumentList $clientArgs -PassThru -NoNewWindow
    Write-Host "Server PID: $($server.Id)  Client PID: $($client.Id)"
}

Write-Host "Press Enter to stop both..."
[void][System.Console]::ReadLine()

try { Stop-Process -Id $client.Id -Force -ErrorAction SilentlyContinue } catch {}
try { Stop-Process -Id $server.Id -Force -ErrorAction SilentlyContinue } catch {}

