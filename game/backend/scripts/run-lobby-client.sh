#!/usr/bin/env bash
set -euo pipefail

TCP=${TCP:-127.0.0.1:7000}
WS=${WS:-}
SERVICEFILE=${SERVICEFILE:-}
USERNAME=${USERNAME:-player}

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
CLIENT_PROJ="$SCRIPT_DIR/../PinionCore.Showcases.Lobby.Client"

ARGS=(run --project "$CLIENT_PROJ" --)
if [[ -n "$SERVICEFILE" ]]; then
  ARGS+=(--servicefile "$SERVICEFILE")
elif [[ -n "$WS" ]]; then
  ARGS+=(--ws "$WS")
else
  ARGS+=(--tcp "$TCP")
fi
ARGS+=(--username "$USERNAME")

echo "Starting Lobby Client..."
exec dotnet "${ARGS[@]}"

