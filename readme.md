# MCworldEditor

**MCworldEditor** is a command-line interface (CLI) tool for editing Minecraft Beta world data.  
It allows reading and modifying data from `level.dat` and region files, including operations on the player's inventory, health, position, spawn point, world seed, time, and more.

>  This tool is designed for **Minecraft Beta worlds only**


## Technologies & Libraries

- **Language**: C# 13 (.NET 9)
- **Used Libraries**:
  - [`fNbt`](https://github.com/fragmer/fNbt): for reading and writing NBT files
  - `System.CommandLine`: for parsing command-line arguments
  - `Microsoft.Extensions.DependencyInjection`: DI container used to structure the project


## Installation & Usage

1. Clone the repository

```bash
git clone https://github.com/VVlktor/MCworldEditor.git
cd MCworldEditor/MCworldEditor
```
2. Build the project
```bash
dotnet build -c Release
```
3. Run with CLI (example)
```bash
dotnet run -- --world 1 player position read
```
All commands require specifying the world number via --world (1–5).
The rest of the arguments are subcommands grouped under categories like player, map, or inventory.

---

Command Overview - examples:
### Read player's position in World 2
```
--world 2 player position read
```
### Move player to specific coordinates (safe check)
```
--world 3 player position set 10 65 -20 --safe
```
### Add 20 items (ID 3 - dirt) to inventory
```
--world 1 inventory add 46 --count 20
```
### Display seed of World 4
```
--world 4 map seed read
```
---

World Requirements: 
All worlds must be located in the default Minecraft Beta folder: ``` %appdata%\.minecraft\saves\World{1-5} ```
The tool does not support custom world folders or modern Minecraft formats.

Notes & Limitations:
All changes are made directly to level.dat and chunk files. Make backups before use (seriously).
The app relies on internal Minecraft Beta chunk structure — later Minecraft versions will not work.

License: 
This project is provided as-is for educational and personal use.

---

## MCworldEditor – Command Reference

All commands must be used with the `--world` option (e.g., `--world 1`).  

Below are grouped commands by category:

---

## Inventory Commands

### `inventory add <itemId> --count <amount>`
Adds an item to player's inventory.

- `itemId` – numeric ID of the item (e.g. 46 for TNT)
- `--count` / `--amount` – number of items to add (max 127 total)

### `inventory clear`
Clears entire inventory.  
Alias: `clean`

---

## Player Commands
 
### `player position read [--specific]`
Displays player's current position.

- `--specific` / `--exact` – shows precise decimal coordinates instead of rounded ones


### `player position set <x> <y> <z> [--safe]`
Moves player to a given location.

- `--safe` – ensures the target location is not inside a block


### `player spawn read`
Displays current spawn coordinates.  
Alias: `check`

### `player spawn set <x> <y> <z> [--safe]`
Changes the world spawn point. Fails if new location is unsafe.

- `--safe` – checks for block collisions above new spawnpoint

### `player health set <hpNumber>`
Sets player's health.

- `hpNumber` – short value, where 20 = full health (each unit = half heart)

### `player save`
Attempts to save player from dying:
- Restores full HP
- Moves player above lava if detected
- Patches hole under the player and removes fall damage

Will not take effect if the player is already dead.

---

## Map/World Commands

### `map time read [--raw]`
Displays time spent in the world in `HH:MM:SS` format.

- `--raw` – displays raw tick count (20 ticks = 1 second)

### `map seed read`
Displays world seed.

### `map seed set <seedId>`
Sets a new seed value.

Warning: Changing the seed may cause terrain generation bugs in unexplored areas.

### `map dimensions [-x <x>] [-z <z>]`
Displays boundaries of the chunk that includes given coordinates.

- If `-x` or `-z` is not given, player's current position is used.
- Alias: `corners`

### `map count <blockId> [-x <x>] [-z <z>]`
Counts number of specific blocks in a chunk at given position.

- `blockId` – block ID to search for
- If `-x` / `-z` not provided, uses player’s position


