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

To run the all services execute:

```console
sudo bash build-all.windows.sh
```
