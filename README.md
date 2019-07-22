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

Execute given code in console generate certificate:

```console
cd certbot/
wget https://dl.eff.org/certbot-auto
chmod a+x certbot-auto

sudo docker stop ha

./certbot-auto certonly --standalone --preferred-challenges http-01 --email your@email.address -d examplehome.duckdns.org

sudo chmod 755 /etc/letsencrypt/live/
sudo chmod 755 /etc/letsencrypt/archive/
```

If you need fully certification help go to <https://www.home-assistant.io/docs/ecosystem/certificates/lets_encrypt/>

## Run

To run the all services execute:

```console
sudo bash build-all.windows.sh
```
