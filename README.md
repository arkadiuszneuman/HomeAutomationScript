# Home automation stack

## Run

Production stack (`docker-compose.windows.yaml`, despite the name — it runs
on the NUC, see the repo's own notes) is managed by Dockerly as the `automation`
stack (`http://10.0.0.115:9130`): deploy, per-service versions, `.env` editing
and logs are there. It only checks for new images — automatic deploys are off for
this stack, so updates are applied with Deploy (or Services → Change version).

Manual fallback, if Dockerly is unavailable:

```console
docker compose -p automation -f docker-compose.windows.yaml up -d
```

## Host: container boot guard

After a power cut dockerd can mark a container as exited (255) and never start
it, despite `restart: unless-stopped`. That's how `mosquitto` stayed down on
2026-11-04 and took z2m (all of Zigbee) with it. `host/container-boot-guard.*`
is a systemd timer that, 2 min after boot and every 10 min after that, starts
any container with restart policy `always`/`unless-stopped` left exited with
code 255 (manual stops never end with 255, so they're left alone). Install or
update it on the NUC:

```console
sudo install -m 755 host/container-boot-guard.sh /usr/local/sbin/
sudo install -m 644 host/container-boot-guard.{service,timer} /etc/systemd/system/
sudo systemctl daemon-reload && sudo systemctl enable --now container-boot-guard.timer
```

Its log: `journalctl -t container-boot-guard`.

External access to Home Assistant goes through the Synology reverse proxy
(`10.0.0.115:8123`, i.e. directly to HA on the host, since it runs with
`network_mode: host`). The old `nginx`/`certbot`/TLS setup for
`arekha.duckdns.org` (unrenewed since 2024, no external traffic in years) was removed.
