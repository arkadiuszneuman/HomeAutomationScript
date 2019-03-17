# Before run

## Elasticsearch

Execute given code in console to run elasticsearch properly on Windows:

```console
docker-machine ssh
sudo sysctl -w vm.max_map_count=262144
exit
```