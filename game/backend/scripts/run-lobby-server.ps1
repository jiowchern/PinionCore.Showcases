param(
    [int]$TcpPort = 7000,
    [int]$TcpBacklog = 10,
    [string]$WsPrefix = ""
)

$serverProj = (Resolve-Path (Join-Path $PSScriptRoot "..\PinionCore.Showcases.Lobby.Server")).Path

$argsList = @("run", "--project", $serverProj, "--")
if ($TcpPort -gt 0) {
    $argsList += @("--tcp-port", $TcpPort, "--tcp-backlog", $TcpBacklog)
}
if ($WsPrefix) {
    $argsList += @("--ws-prefix", $WsPrefix)
}

Write-Host "Starting Lobby Server..."
dotnet @argsList

