# Powtarza kroki instalacyjne z upstreamowego Dockerfile (alpine + zależności
# systemowe dla Selenium/Chromium), ale kod źródłowy (vultron.py, karty JS,
# requirements.txt) kopiuje z aktualnego stanu repo GitHub (kontekst
# "upstream" w docker-compose.windows.yaml -> build.additional_contexts) —
# nie jest forkowany ani zamrożony lokalnie. Doklejamy tylko run_wrapper.py.
#
# UWAGA przy aktualizacji: jeśli upstream zmieni bazowy obraz albo listę
# pakietów apk w swoim Dockerfile, trzeba to ręcznie przenieść tutaj —
# porównaj z https://github.com/htomasz/vultron/blob/main/vultron/Dockerfile
FROM alpine:latest

ENV PYTHONUNBUFFERED=1

RUN apk add --no-cache \
    python3 \
    py3-pip \
    chromium \
    chromium-chromedriver \
    xvfb \
    gcompat \
    libstdc++ \
    bash \
    tzdata \
    sqlite \
    curl

WORKDIR /app

COPY --from=upstream requirements.txt .
RUN pip3 install --break-system-packages --no-cache-dir -r requirements.txt

COPY --from=upstream . .
COPY run_wrapper.py .

CMD ["python3", "run_wrapper.py"]
