PinionCore Showcases Lobby Client

Overview

- Console client for the Lobby sample.
- Supports three modes: Standalone, TCP, WebSocket.
- Stop with Ctrl+C.

Options

- --servicefile <path>        Path to a server assembly containing an IEntry implementation (Standalone mode).
- --tcp <host:port>           Remote TCP endpoint.
- --ws <ws://...>             Remote WebSocket URL.
- --username <name>           Login username; default "player".

Usage

- Standalone (runs client and a local in‑memory server)
  - Build server library first: dotnet build -c Release game/backend/PinionCore.Showcases.Lobby
  - Run: dotnet run --project game/backend/PinionCore.Showcases.Lobby.Client -- --servicefile "game/backend/PinionCore.Showcases.Lobby/bin/Release/net8.0/PinionCore.Showcases.Lobby.dll" --username alice

- Remote TCP
  - Start server (example): dotnet run --project game/backend/PinionCore.Showcases.Lobby.Server -- --tcp-port 7000
  - Run client: dotnet run --project game/backend/PinionCore.Showcases.Lobby.Client -- --tcp 127.0.0.1:7000 --username alice

- Remote WebSocket
  - Start server (example): dotnet run --project game/backend/PinionCore.Showcases.Lobby.Server -- --ws-prefix "http://127.0.0.1:8080/"
  - Run client: dotnet run --project game/backend/PinionCore.Showcases.Lobby.Client -- --ws ws://127.0.0.1:8080/ --username alice

Behavior

- Obtains IUser via notifier, subscribes to events, and logs in when ILogin is supplied.
- Username can be customized with --username.

Notes

- Specify exactly one of: --servicefile, or one of --tcp/--ws.
- WebSocket URL should match the server prefix; when the server binds http://127.0.0.1:8080/, the client connects to ws://127.0.0.1:8080/.

