# Vultron na czystym Dockerze

[Vultron](https://github.com/htomasz/vultron) jest dystrybuowany jako dodatek do
Home Assistant **Supervised**. Nasz HA działa jako goły kontener Dockera, więc
nie ma Supervisora, który normalnie dostarcza dodatkowi trzy rzeczy:

| Czego dodatek oczekuje | Czym to zastępujemy |
|---|---|
| `/data/options.json` z konfiguracją | plik `vultron/data/options.json` (montowany jako `/data`) |
| `SUPERVISOR_TOKEN` | long-lived access token HA, `HA_TOKEN` w `.env` |
| host `supervisor` z `/core/api` i `/core/websocket` | kontener `vultron-supervisor` (nginx) o aliasie sieciowym `supervisor`, przepisujący `/core/api/*` → `:8123/api/*` i `/core/websocket` → `:8123/api/websocket` |

Dzięki temu **kod dodatku nie jest modyfikowany** — obraz budowany jest wprost
z upstreamowego repozytorium, a aktualizacja to podbicie SHA w
`docker-compose.windows.yaml`.

## Konfiguracja

1. Wygeneruj w HA long-lived access token: *Profil → Bezpieczeństwo → Tokeny
   dostępu długoterminowego → Utwórz token*.
2. Wpisz go w `/home/arek/automation/.env` jako `HA_TOKEN=...`.
3. Uzupełnij dane logowania do EduVulcan w `vultron/data/options.json`.
4. `docker compose -f docker-compose.windows.yaml up -d --build vultron-supervisor vultron`

Karty Lovelace dodatek kopiuje sam do `homeassistant/www/vultron/` i rejestruje
przez WebSocket API. `vultron/data/` (baza SQLite, logi, options.json z hasłem)
jest w `.gitignore`.

## Aktualizacja

```bash
# podmień VULTRON_REF w docker-compose.windows.yaml na nowy tag/SHA, potem:
docker compose -f docker-compose.windows.yaml build --no-cache vultron
docker compose -f docker-compose.windows.yaml up -d vultron
```

## Pomijanie modułów dla konkretnego ucznia

Vultron bezwarunkowo próbuje pobrać 8 kategorii danych dla każdego ucznia.
Jeśli dla któregoś dziecka część endpointów EduVulcan systematycznie zwraca
błąd (np. przedszkolak nie ma ocen/frekwencji/terminarza), `vultron/run_wrapper.py`
podmienia te konkretne funkcje pobierające na no-opy dla wskazanego ucznia
(po `slug`, widocznym w nazwach encji `sensor.vultron_*_<slug>`) — bez
modyfikowania `vultron.py`. Lista wyjątków jest w `SKIP_MODULES` na górze
pliku.
