#!/bin/bash
set -uo pipefail

SRC_DIR="/home/arek/automation"
MOUNT_POINT="/mnt/syno/nuc"
BACKUP_ROOT="${MOUNT_POINT}/automation-backup"
DAILY_DIR="${BACKUP_ROOT}/daily"
WEEKLY_DIR="${BACKUP_ROOT}/weekly"
EXCLUDE_FILE="/usr/local/etc/automation-backup.exclude"
DAILY_KEEP=7
WEEKLY_KEEP=4

DATE="$(date +%Y-%m-%d)"
FINAL_NAME="automation-${DATE}.tar.zst"
TMP_NAME=".tmp-automation-${DATE}.tar.zst"

log() {
    echo "[automation-backup] $*"
}

die() {
    echo "[automation-backup] ERROR: $*" >&2
    exit 1
}

[ "$(id -u)" -eq 0 ] || die "must run as root (needed to read root-owned Home Assistant files)"

if ! mountpoint -q "$MOUNT_POINT"; then
    log "mounting ${MOUNT_POINT}"
    mount "$MOUNT_POINT" || die "could not mount ${MOUNT_POINT}, aborting before touching retention"
fi

mkdir -p "$DAILY_DIR" "$WEEKLY_DIR" || die "could not create backup directories on ${MOUNT_POINT}"

TMP_PATH="${DAILY_DIR}/${TMP_NAME}"
FINAL_PATH="${DAILY_DIR}/${FINAL_NAME}"

log "creating archive ${FINAL_NAME}"
tar --exclude-from="$EXCLUDE_FILE" -C "$SRC_DIR" -cf - . | zstd -3 -T0 -q -o "$TMP_PATH"
TAR_STATUS="${PIPESTATUS[0]}"

if [ "$TAR_STATUS" -ge 2 ]; then
    rm -f "$TMP_PATH"
    die "tar failed with status ${TAR_STATUS}"
fi
if [ "$TAR_STATUS" -eq 1 ]; then
    log "tar reported status 1 (files changed while reading, expected for a live HA install) - archive is still valid"
fi

[ -s "$TMP_PATH" ] || die "archive ${TMP_PATH} is empty, aborting"

mv "$TMP_PATH" "$FINAL_PATH" || die "could not finalize ${FINAL_PATH}"
log "archive written: ${FINAL_PATH} ($(du -h "$FINAL_PATH" | cut -f1))"

if [ "$(date +%u)" -eq 7 ]; then
    WEEKLY_PATH="${WEEKLY_DIR}/${FINAL_NAME}"
    if [ ! -e "$WEEKLY_PATH" ]; then
        ln "$FINAL_PATH" "$WEEKLY_PATH" && log "linked weekly copy: ${WEEKLY_PATH}"
    fi
fi

log "applying retention: daily=${DAILY_KEEP}, weekly=${WEEKLY_KEEP}"
find "$DAILY_DIR" -maxdepth 1 -name 'automation-*.tar.zst' -printf '%T@ %p\n' \
    | sort -rn | tail -n +$((DAILY_KEEP + 1)) | cut -d' ' -f2- \
    | while IFS= read -r f; do log "removing old daily backup: ${f}"; rm -f -- "$f"; done

find "$WEEKLY_DIR" -maxdepth 1 -name 'automation-*.tar.zst' -printf '%T@ %p\n' \
    | sort -rn | tail -n +$((WEEKLY_KEEP + 1)) | cut -d' ' -f2- \
    | while IFS= read -r f; do log "removing old weekly backup: ${f}"; rm -f -- "$f"; done

log "done"
