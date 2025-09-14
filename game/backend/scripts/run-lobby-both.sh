#!/usr/bin/env bash
set -euo pipefail

TCP_PORT=${TCP_PORT:-7000}
USERNAME=${USERNAME:-player}
LOGS=${LOGS:-}

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SERVER_PROJ="$SCRIPT_DIR/../PinionCore.Showcases.Lobby.Server"
CLIENT_PROJ="$SCRIPT_DIR/../PinionCore.Showcases.Lobby.Client"

SERVER_ARGS=(run --project "$SERVER_PROJ" -- --tcp-port "$TCP_PORT")
CLIENT_ARGS=(run --project "$CLIENT_PROJ" -- --tcp "127.0.0.1:$TCP_PORT" --username "$USERNAME")

if [[ -n "$LOGS" ]]; then
  LOG_DIR="${TMPDIR:-/tmp}/lobby_logs"
  mkdir -p "$LOG_DIR"
  SERVER_OUT="$LOG_DIR/server.out.txt"; SERVER_ERR="$LOG_DIR/server.err.txt"
  CLIENT_OUT="$LOG_DIR/client.out.txt"; CLIENT_ERR="$LOG_DIR/client.err.txt"
  dotnet "${SERVER_ARGS[@]}" >"$SERVER_OUT" 2>"$SERVER_ERR" & SERVER_PID=$!
  sleep 1
  dotnet "${CLIENT_ARGS[@]}" >"$CLIENT_OUT" 2>"$CLIENT_ERR" & CLIENT_PID=$!
  echo "Server PID: $SERVER_PID  Client PID: $CLIENT_PID"
  echo "Logs in: $LOG_DIR"
else
  dotnet "${SERVER_ARGS[@]}" & SERVER_PID=$!
  sleep 1
  dotnet "${CLIENT_ARGS[@]}" & CLIENT_PID=$!
  echo "Server PID: $SERVER_PID  Client PID: $CLIENT_PID"
fi

read -rp "Press Enter to stop both..." _
kill "$CLIENT_PID" 2>/dev/null || true
kill "$SERVER_PID" 2>/dev/null || true
wait "$CLIENT_PID" 2>/dev/null || true
wait "$SERVER_PID" 2>/dev/null || true

