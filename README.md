# Learn on the Job

A Big Ambitions mod that lets employees get better at their job just by doing it.

Subscribe on the [Steam Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3817894225).

In the base game, an employee hired at 19% stays at 19% forever unless you pay for training or
assign an HR manager. With this mod, every hour an employee spends on shift nudges their primary
skill upward:

| Skill range | Growth per step |
|-------------|-----------------|
| below 50%   | +2 points       |
| 50% to 70%  | +1 point        |
| 70% and up  | none; on-the-job learning stops, so formal training is the only way past "average" |

A "step" is a number of on-shift hours, set by a slider under **Options → Mods** (default 40, the
equivalent of one full-time week). At the default, a full-time 19% hire reaches 50% after about
15½ weeks on shift and 70% after another 20.

## How it works

- Hooks the game's own `GlobalEvents.onNewHour`, which fires once per in-game hour, including
  during sleep, fast-forward, and the time machine. No Harmony or runtime patching.
- Loading a save from inside the city clears every `GlobalEvents` subscriber without reloading
  city mods, so a small `HourlyHook` component re-attaches the handler when that happens.
- Counts an hour only when the game itself would: the employee has a shift right now, isn't in
  training, and isn't off sick.
- Growth is continuous rather than in jumps. Skill is a float that the game already saves, so
  partial progress persists in your save without the mod storing anything. The UI shows whole
  numbers, so below 50% you'll see +1 roughly every half-step.
- Only the employee's primary skill (the one shown next to their name) grows.
- Skills are never lowered, and anything already at or above 70% is left alone.

Side effects you might notice:

- Wages don't rise with on-the-job growth. Formal training still raises wages as usual.
- An employee whose skill crosses 50% becomes eligible for an extra job demand, the same as a
  trained employee would.
- Roles that are never scheduled on a shift in a building's schedule won't grow. Delivery drivers
  and the headquarters managers may fall into this group; this hasn't been checked in-game yet.

## Building

Requires either a local .NET SDK (8+) or `podman`. The build script uses `dotnet` if it's on
`PATH`, and otherwise runs the SDK in a throwaway container.

```sh
./scripts/build.sh     # runs unit tests, builds, assembles dist/LearnOnTheJob/
./scripts/install.sh   # copies it into the game's ModsLocal folder
```

Both scripts default to this machine's paths. Override with:

- `BA_MANAGED_DIR`: `<game install>/Big Ambitions_Data/Managed`
- `BA_MODS_LOCAL`: the game's `ModsLocal` folder. On Linux/Proton it lives under
  `steamapps/compatdata/1331550/pfx/drive_c/users/steamuser/AppData/LocalLow/Hovgaard Games/Big Ambitions/`.

The project compiles against the game's shipped DLLs rather than the official Unity-based SDK
(github.com/hovgaardgames/bigambitions). The SDK's packager produces the same output (one DLL at
the folder root plus `Locales/`) but needs the Unity 2022.3 editor. After a game update, rebuild
so the mod links against the new DLLs.

## Verifying in-game

Launch the game (mods in `ModsLocal` load automatically; there's no enable step) and load a save
with staffed businesses. Let a few in-game hours pass, then check `Player.log` (in the same `Hovgaard Games/Big Ambitions/` folder as `ModsLocal`):

```sh
grep -E "Learn on the Job|skill_" "<that folder>/Player.log"
```

You should see `Learn on the Job active: 40 hours on shift per step.`, followed by one line each
time an on-shift employee ticks up a whole point, e.g. `Jane Doe ba:skill_customerservice: 19 -> 20`.
After loading another save from inside the game, you should also see
`Re-attached hourly hook after the game reset its events.`

Loading a save that was made without mods creates a modded copy; the original save is untouched.

## Layout

```
src/LearnOnTheJob/
  SkillGrowthCurve.cs            pure tier math (unit tested)
  OnTheJobTrainer.cs             hourly pass over employees
  HourlyHook.cs                  keeps the hourly subscription alive across in-game loads
  LearnOnTheJobCityEntry.cs      subscribes to the hourly event while a save is loaded
  LearnOnTheJobOptionsEntry.cs   registers the options slider at startup
  Settings.cs                    reads the persisted slider value
  Locales/en.json
tests/LearnOnTheJob.Tests/       xUnit tests for SkillGrowthCurve
scripts/                         build and install
```
