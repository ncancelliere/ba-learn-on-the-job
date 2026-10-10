#!/usr/bin/env bash
# Runs the unit tests, builds the mod, and assembles dist/LearnOnTheJob/ in the layout the game
# expects: exactly one DLL at the root plus a Locales/ folder.
#
# Uses a local `dotnet` SDK if one is on PATH; otherwise runs the SDK in a throwaway podman
# container so nothing needs installing on the host.
set -euo pipefail

project_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
managed_dir="${BA_MANAGED_DIR:-/var/mnt/SamsungT9/SteamLibrary/steamapps/common/Big Ambitions/Big Ambitions_Data/Managed}"
dist_dir="${project_root}/dist/LearnOnTheJob"
sdk_image="mcr.microsoft.com/dotnet/sdk:8.0"

if [[ ! -f "${managed_dir}/BigAmbitions.dll" ]]; then
  echo "Big Ambitions DLLs not found in: ${managed_dir}" >&2
  echo "Set BA_MANAGED_DIR to '<game install>/Big Ambitions_Data/Managed'." >&2
  exit 1
fi

build_commands='
  set -euo pipefail
  dotnet test tests/LearnOnTheJob.Tests --nologo -v quiet
  dotnet build src/LearnOnTheJob -c Release --nologo -v quiet -o build/out
'

if command -v dotnet >/dev/null 2>&1; then
  (cd "${project_root}" && BA_MANAGED_DIR="${managed_dir}" bash -c "${build_commands}")
else
  podman run --rm \
    -v "${project_root}:/src:z" \
    -v "${managed_dir}:/managed:ro,z" \
    -v ba-learn-on-the-job-nuget:/root/.nuget/packages \
    -e BA_MANAGED_DIR=/managed \
    -w /src \
    "${sdk_image}" \
    bash -c "${build_commands}"
fi

rm -rf "${dist_dir}"
mkdir -p "${dist_dir}/Locales"
cp "${project_root}/build/out/LearnOnTheJob.dll" "${dist_dir}/"
cp "${project_root}/src/LearnOnTheJob/Locales/"*.json "${dist_dir}/Locales/"

echo "Built ${dist_dir}"
