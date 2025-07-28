# MCworldEditor

**MCworldEditor** is a command-line interface (CLI) tool for editing Minecraft Beta world data.  
It allows reading and modifying data from `level.dat` and region files, including operations on the player's inventory, health, position, spawn point, world seed, time, and more.

>  This tool is designed for **Minecraft Beta worlds (1.7.3 and earlier)** and assumes the default save location:  
> `%appdata%\.minecraft\saves\World{1-5}`

---

## Technologies & Libraries

- **Language**: C# 13 (.NET 9)
- **Used Libraries**:
  - [`fNbt`](https://github.com/fragmer/fNbt): for reading and writing NBT (Named Binary Tag) files
  - `System.CommandLine`: for parsing command-line arguments
  - `Microsoft.Extensions.DependencyInjection`: lightweight DI container used to structure the project

---