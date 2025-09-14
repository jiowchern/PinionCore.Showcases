PinionCore Showcases Lobby Server

Overview

- Hosts the Lobby service over TCP and/or WebSocket.
- Uses a single Service with a MultiListener to aggregate multiple transports safely.
- Stop with Ctrl+C; the app closes listeners and disposes the service gracefully.

Options

- --tcp-port <int>            TCP listen port; omit to disable TCP.
- --tcp-backlog <int>         TCP listen backlog; default 10.
- --ws-prefix <string>        HttpListener prefix for WebSocket, e.g. http://127.0.0.1:8080/.

Examples

- TCP only
  - dotnet run --project game/backend/PinionCore.Showcases.Lobby.Server -- --tcp-port 7000

- WebSocket only
  - dotnet run --project game/backend/PinionCore.Showcases.Lobby.Server -- --ws-prefix "http://127.0.0.1:8080/"

- TCP + WebSocket
  - dotnet run --project game/backend/PinionCore.Showcases.Lobby.Server -- --tcp-port 7000 --ws-prefix "http://127.0.0.1:8080/"

Notes

- Safety: Single Service + MultiListener avoids concurrent calls into Entry from multiple threads.
- WebSocket prefix: Prefer explicit hosts like http://127.0.0.1:PORT/.
  - Binding to http://+:PORT/ or http://*:PORT/ may require URL ACL on Windows.
    - Example: netsh http add urlacl url=http://+:8080/ user=DOMAIN\\User
- TLS: HttpListener here is plain HTTP; place behind a TLS reverse proxy (e.g., Nginx/Caddy/IIS/Kestrel) for encryption.
- Firewall: Open only the ports you need; prefer loopback during local development.

