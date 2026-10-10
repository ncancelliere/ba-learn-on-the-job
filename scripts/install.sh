#!/usr/bin/env bash
# Copies dist/LearnOnTheJob/ into the game's ModsLocal folder (inside the Proton prefix on Linux).
set -euo pipefail

project_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
dist_dir="${project_root}/dist/LearnOnTheJob"
mods_local="${BA_MODS_LOCAL:-/var/mnt/SamsungT9/SteamLibrary/steamapps/compatdata/1331550/pfx/drive_c/users/steamuser/AppData/LocalLow/Hovgaard Games/Big Ambitions/ModsLocal}"

if [[ ! -f "${dist_dir}/LearnOnTheJob.dll" ]]; then
  echo "Nothing to install; run scripts/build.sh first." >&2
  exit 1
fi

if [[ ! -d "${mods_local}" ]]; then
  echo "ModsLocal folder not found: ${mods_local}" >&2
  echo "Launch the game once, or set BA_MODS_LOCAL." >&2
  exit 1
fi

target="${mods_local}/LearnOnTheJob"
rm -rf "${target}"
cp -r "${dist_dir}" "${target}"
echo "Installed to ${target}"
