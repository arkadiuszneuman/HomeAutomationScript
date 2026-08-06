# automation-backup

Codzienny backup katalogu `/home/arek/automation` na Synology (NFS), z retencją 7 kopii dziennych + 4 tygodniowe. Uruchamiane przez systemd timer o 3:30 w nocy.

Backupowane są konfiguracje Home Assistant, Node-RED, Zigbee2MQTT, Grafany i sekrety serwisów (patrz komentarz w `automation-backup.exclude` co jest pomijane — m.in. MySQL, InfluxDB 1.x, ELK, `node_modules`, logi).

## Wymagania na docelowym hoście

- Punkt montowania NFS `/mnt/syno/nuc` wskazujący na `10.0.0.31:/volume2/nuc` (patrz sekcja fstab niżej).
- Na Synology: uprawnienie NFS dla folderu `nuc` obejmujące podsieć klienta (np. `10.0.0.0/24` — nie pojedynczy IP, bo DHCP na WiFi bywa niestabilne).
- Pakiety `nfs-common`, `tar`, `zstd` (standardowo dostępne na Ubuntu).

## Instalacja od zera (po formacie)

```bash
# 1. Wpis w /etc/fstab (dopisz na końcu, dostosuj IP Synology jeśli inne)
echo '10.0.0.31:/volume2/nuc /mnt/syno/nuc nfs rw,vers=3,tcp,soft,_netdev,noatime,nodiratime,rsize=1048576,wsize=1048576,nconnect=4 0 0' | sudo tee -a /etc/fstab

sudo mkdir -p /mnt/syno/nuc
sudo systemctl daemon-reload
sudo mount /mnt/syno/nuc

# 2. Pliki backupu
sudo mkdir -p /usr/local/etc /usr/local/sbin
sudo install -m 644 automation-backup.exclude /usr/local/etc/automation-backup.exclude
sudo install -m 755 automation-backup.sh /usr/local/sbin/automation-backup.sh
sudo install -m 644 automation-backup.service /etc/systemd/system/automation-backup.service
sudo install -m 644 automation-backup.timer /etc/systemd/system/automation-backup.timer

# 3. Aktywacja
sudo systemctl daemon-reload
sudo systemctl enable --now automation-backup.timer
```

## Weryfikacja

```bash
# uruchomienie ręczne + logi
sudo systemctl start automation-backup.service
sudo journalctl -u automation-backup -n 60 --no-pager

# najbliższe uruchomienie timera
systemctl list-timers automation-backup --no-pager

# zawartość backupów na Synology
ls -la /mnt/syno/nuc/automation-backup/daily/
ls -la /mnt/syno/nuc/automation-backup/weekly/
```

## Uwaga o źródle prawdy

Ten katalog to kopia plików z systemu — po każdej zmianie w `/usr/local/sbin/automation-backup.sh` lub jednostkach systemd trzeba ręcznie zsynchronizować kopię tutaj (albo odwrotnie: edytować tu i re-instalować przez `install` powyżej).
