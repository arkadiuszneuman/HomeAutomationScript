#!/usr/bin/env bash
#
# Starts containers that Docker left dead after an unclean shutdown.
#
# Why this exists: on 2026-11-04 the NUC lost power (no shutdown in the
# journal). When dockerd came back it marked `mosquitto` as exited with code
# 255 and never started it, despite `restart: unless-stopped`, and the next
# clean reboot didn't bring it back either. Without the broker z2m exits on
# start (`getaddrinfo EAI_AGAIN mosquitto`), so all of Zigbee was down until
# someone noticed.
#
# Exit code 255 is what dockerd records for a container it finds dead at
# startup. A manual `docker stop`/`compose stop`/`kill` ends with 0, 137 or 143,
# and a container that crashes at runtime with 255 is restarted by Docker
# itself (it shows as "restarting", not "exited"). So "exited + 255 + restart
# policy always/unless-stopped" only matches containers Docker should have
# restarted and didn't, which makes this safe to run on a timer.

set -uo pipefail

log() { logger -t container-boot-guard -- "$*"; echo "container-boot-guard: $*"; }

ids=$(docker ps -aq --filter status=exited) || { log "docker ps failed"; exit 1; }
[ -z "$ids" ] && exit 0

rc=0
while read -r id name policy code; do
    case "$policy" in always|unless-stopped) ;; *) continue ;; esac
    [ "$code" = 255 ] || continue
    if docker start "$id" >/dev/null; then
        log "started ${name#/} (was exited 255, restart=$policy)"
    else
        log "failed to start ${name#/}"
        rc=1
    fi
done < <(docker inspect -f '{{.Id}} {{.Name}} {{.HostConfig.RestartPolicy.Name}} {{.State.ExitCode}}' $ids)

exit $rc
