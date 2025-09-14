# Backend Samples Quick Run

This folder contains the Lobby sample server and client built on PinionCore.Remote.

## Windows PowerShell

Together (server + client)
- Script: `powershell -File game/backend/scripts/run-lobby-both.ps1 -TcpPort 7000 -Username alice`
- With logs: add `-Logs`

Server only
- Script: `powershell -File game/backend/scripts/run-lobby-server.ps1 -TcpPort 7000`
- Or raw: `dotnet run --project game/backend/PinionCore.Showcases.Lobby.Server -- --tcp-port 7000`

Client only
- Script (TCP): `powershell -File game/backend/scripts/run-lobby-client.ps1 -Tcp 127.0.0.1:7000 -Username alice`
- Script (WS): `powershell -File game/backend/scripts/run-lobby-client.ps1 -Ws ws://127.0.0.1:8080/ -Username alice`
- Or raw TCP: `dotnet run --project game/backend/PinionCore.Showcases.Lobby.Client -- --tcp 127.0.0.1:7000 --username alice`

## macOS/Linux (bash/zsh)

- Together (server + client)
  - Script (TCP): `TCP_PORT=7000 USERNAME=alice bash game/backend/scripts/run-lobby-both.sh`
  - Script (WS): `WS_PREFIX=http://127.0.0.1:8080/ USERNAME=alice bash game/backend/scripts/run-lobby-both-ws.sh`
  - Background jobs (raw):
  ```
  (dotnet run --project game/backend/PinionCore.Showcases.Lobby.Server -- --tcp-port 7000 &)
  sleep 1
  (dotnet run --project game/backend/PinionCore.Showcases.Lobby.Client -- --tcp 127.0.0.1:7000 --username alice &)
  # later
  pkill -f "PinionCore.Showcases.Lobby.Client"
  pkill -f "PinionCore.Showcases.Lobby.Server"
  ```

- Server only
  - Script: `TCP_PORT=7000 TCP_BACKLOG=10 WS_PREFIX= bash game/backend/scripts/run-lobby-server.sh`
  - Or raw: `dotnet run --project game/backend/PinionCore.Showcases.Lobby.Server -- --tcp-port 7000`

- Client only
  - Script (TCP): `TCP=127.0.0.1:7000 USERNAME=alice bash game/backend/scripts/run-lobby-client.sh`
  - Script (WS): `WS=ws://127.0.0.1:8080/ USERNAME=alice bash game/backend/scripts/run-lobby-client.sh`

## WebSocket Variant

- Server (Windows/macOS/Linux):
  - `dotnet run --project game/backend/PinionCore.Showcases.Lobby.Server -- --ws-prefix "http://127.0.0.1:8080/"`
- Client:
  - `dotnet run --project game/backend/PinionCore.Showcases.Lobby.Client -- --ws ws://127.0.0.1:8080/ --username alice`

Notes
- On Windows, binding `http://+:8080/` may require URL ACL: `netsh http add urlacl url=http://+:8080/ user=DOMAIN\User`
- Prefer explicit hosts like `http://127.0.0.1:8080/` during local development.
- The server blocks by design; use a second terminal or background processes to run the client concurrently as shown above.
