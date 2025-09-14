param(
    [string]$Tcp = "127.0.0.1:7000",
    [string]$Ws = "",
    [string]$ServiceFile = "",
    [string]$Username = "player"
)

$clientProj = (Resolve-Path (Join-Path $PSScriptRoot "..\PinionCore.Showcases.Lobby.Client")).Path

$argsList = @("run", "--project", $clientProj, "--")
if ($ServiceFile) {
    $argsList += @("--servicefile", $ServiceFile)
} elseif ($Ws) {
    $argsList += @("--ws", $Ws)
} else {
    $argsList += @("--tcp", $Tcp)
}
$argsList += @("--username", $Username)

Write-Host "Starting Lobby Client..."
dotnet @argsList

