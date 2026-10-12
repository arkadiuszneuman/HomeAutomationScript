# Before run

## Elasticsearch

Execute given code in console to run elasticsearch properly on Windows:

```console
docker-machine ssh
sudo sysctl -w vm.max_map_count=262144
exit
```

On Linux you should execute:

```console
sudo sysctl -w vm.max_map_count=262144
```

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

`vultron` has a `build:` block (no registry image) — pull-only deploys skip
it; build it explicitly first if its Dockerfile or the upstream ref changed:

```console
docker compose -p automation -f docker-compose.windows.yaml build vultron
```

External access to Home Assistant goes through the Synology reverse proxy
(`10.0.0.115:8123`, i.e. directly to HA on the host, since it runs with
`network_mode: host`), not through this stack's `nginx` — the old
`certbot`/TLS setup for `arekha.duckdns.org` (unrenewed since 2024, no
external traffic in years) was removed. `nginx` currently has no real job
in this stack; kept for now in case it's needed again.
