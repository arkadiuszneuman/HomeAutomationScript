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

## TLS/SSL

Execute given code in console generate certificate (after you run docker compose):

```console
sudo docker compose -f docker-compose.windows.yaml run --rm certbot certonly --webroot --webroot-path /var/www/certbot/ -d arekha.duckdns.org
```

To refresh certificate:

```console
docker compose -f docker-compose.windows.yaml  run --rm certbot renew
```

## Run

Production stack (`docker-compose.windows.yaml`, despite the name — it runs
on the NUC, see the repo's own notes) is managed by Komodo as the `automation`
stack (`http://10.0.0.115:9120`). Normal deploys go through Komodo, not this
script.

Manual fallback, if Komodo is unavailable:

```console
docker compose -p automation -f docker-compose.windows.yaml up -d
```

`vultron` has a `build:` block (no registry image) — pull-only deploys skip
it; build it explicitly first if its Dockerfile or the upstream ref changed:

```console
docker compose -p automation -f docker-compose.windows.yaml build vultron
```

`certbot` is a one-shot tool, not part of the running stack (`profiles:
["tools"]`) — it never starts on `up`, only when named explicitly, as in the
TLS/SSL commands above.
