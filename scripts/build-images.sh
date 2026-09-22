#!/usr/bin/env bash
# Builds the five service images.
#
# Why not `podman-compose build`? On Windows podman-compose passes the Dockerfile
# to `-f` as an absolute path with backslashes, which podman's remote client
# cannot resolve inside the build context it ships to the WSL VM. Passing a
# relative, forward-slash path works, so we call `podman build` directly.
#
# It also builds in parallel; podman-compose builds one service at a time.
set -euo pipefail

cd "$(dirname "$0")/.."

SERVICES=(Clients.Api Accounts Notifications Reporting RiskEvaluator)
IMAGES=(clients.api accounts.service notifications.service reporting.service risk.evaluator.service)

# Build only what was asked for, or everything.
if [ $# -gt 0 ]; then
  wanted=("$@")
else
  wanted=("${SERVICES[@]}")
fi

pids=()
for i in "${!SERVICES[@]}"; do
  svc="${SERVICES[$i]}"
  img="${IMAGES[$i]}"
  for w in "${wanted[@]}"; do
    if [ "$w" = "$svc" ]; then
      echo ">> building $img from src/$svc/Dockerfile"
      podman build -f "src/$svc/Dockerfile" -t "$img" . > "/tmp/build-$svc.log" 2>&1 &
      pids+=("$!:$svc")
    fi
  done
done

failed=0
for entry in "${pids[@]}"; do
  pid="${entry%%:*}"; svc="${entry##*:}"
  if wait "$pid"; then
    echo "OK    $svc"
  else
    echo "FAIL  $svc  (see /tmp/build-$svc.log)"
    tail -20 "/tmp/build-$svc.log"
    failed=1
  fi
done

exit $failed
