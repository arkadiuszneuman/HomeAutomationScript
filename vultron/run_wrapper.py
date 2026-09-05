"""
Nakładka na vultron.py, nieobecna w upstreamie.

Vultron bezwarunkowo próbuje pobrać 8 kategorii danych dla każdego ucznia
naraz (asyncio.gather w sync_diary_data). Dla Kingi (przedszkole) część
endpointów EduVulcan nie istnieje i zwraca HTTP 400 przy każdym cyklu —
żeby nie bombardować API nieudanymi zapytaniami (ryzyko zwrócenia uwagi
serwisu / zablokowania konta), podmieniamy te funkcje na no-opy zamiast
patchować sam plik dostawcy (utrudniłoby to aktualizacje).

Działa dzięki temu, że sync_diary_data() odwołuje się do _fetch_* po
nazwie w globalnym namespace modułu vultron — podmiana atrybutu modułu
przed startem main_loop() wystarczy, nie trzeba nic zmieniać w vultron.py.

Gdy Kinga pójdzie do szkoły, wystarczy usunąć jej wpis z SKIP_MODULES.
"""
import asyncio
import sys

import vultron

SKIP_MODULES = {
    "kinga_neuman": {
        "_fetch_timetable",     # terminarz
        "_fetch_remarks",       # uwagi
        "_fetch_frequency",     # frekwencja + statystyki
        "_fetch_achievements",  # osiągnięcia
        "_fetch_lucky_number",  # szczęśliwy numerek
    },
}


def _make_guard(fn_name, original):
    async def guarded(client, ha, base, s):
        if fn_name in SKIP_MODULES.get(s.get("slug"), ()):
            vultron.logger.debug(
                "[%s] pomijam %s – moduł niedostępny dla tego etapu edukacji",
                s.get("uczen"), fn_name,
            )
            return
        return await original(client, ha, base, s)
    return guarded


for _fn_name in {n for names in SKIP_MODULES.values() for n in names}:
    setattr(vultron, _fn_name, _make_guard(_fn_name, getattr(vultron, _fn_name)))


if __name__ == "__main__":
    try:
        asyncio.run(vultron.main_loop())
    except (KeyboardInterrupt, SystemExit):
        vultron.logger.info("Zamykanie…")
        sys.exit(0)
