#!/usr/bin/env bash
set -euo pipefail

TCP_PORT=${TCP_PORT:-7000}
TCP_BACKLOG=${TCP_BACKLOG:-10}
WS_PREFIX=${WS_PREFIX:-}

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SERVER_PROJ="$SCRIPT_DIR/../PinionCore.Showcases.Lobby.Server"

ARGS=(run --project "$SERVER_PROJ" --)
if [[ -n "$WS_PREFIX" ]]; then
  ARGS+=(--ws-prefix "$WS_PREFIX")
fi
if [[ -n "$TCP_PORT" ]]; then
  ARGS+=(--tcp-port "$TCP_PORT" --tcp-backlog "$TCP_BACKLOG")
fi

echo "Starting Lobby Server..."
exec dotnet "${ARGS[@]}"

