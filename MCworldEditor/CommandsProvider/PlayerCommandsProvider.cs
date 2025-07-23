using MCworldEditor.CommandsToCall;
using System.CommandLine;

namespace MCworldEditor.CommandsProvider
{
    public class PlayerCommandsProvider
    {
        private DatHelper _datHelper;
        private PlayerCommands _playerCommands;

        public PlayerCommandsProvider(DatHelper datHelper, PlayerCommands playerCommands)
        {
            _datHelper = datHelper;
            _playerCommands = playerCommands;
        }
        //komende na falldistance, komende na uratowanie - ustawienie hp na max, sprawdzenie czy nie jest w lawie, usuniecie potworow dookola etc.
        public void Register(RootCommand rootCommand, Option<int> worldOption)
        {
            Command playerCommand = new("player", "Commands realted to player.");
            rootCommand.Subcommands.Add(playerCommand);

            Argument<int> xPositionArgument = new("xPosition")
            {
                Description = "In-world X coordinates.",
            };
            Argument<int> yPositionArgument = new("yPosition")
            {
                Description = "In-world Y coordinates.",
            };
            Argument<int> zPositionArgument = new("zPosition")
            {
                Description = "In-world Z coordinates.",
            };

            Option<bool> safeNewPosition = new("--safe")
            {
                Description = "Command wont take effect if specified location is inside of a block."
            };

            RegisterPositionCommands(playerCommand, worldOption, xPositionArgument, yPositionArgument, zPositionArgument, safeNewPosition);
            RegisterSpawnCommands(playerCommand, worldOption, xPositionArgument, yPositionArgument, zPositionArgument, safeNewPosition);
            RegisterHealthCommand(playerCommand, worldOption);
        }

        private void RegisterHealthCommand(Command playerCommand, Option<int> worldOption)
        {
            Command healthCommand = new("health", "Commands related to health of player");
            playerCommand.Subcommands.Add(healthCommand);

            RegisterSetHealthCommand(healthCommand, worldOption);
        }

        private void RegisterSetHealthCommand(Command healthCommand, Option<int> worldOption)
        {
            Argument<short> hpCount = new("hpNumber")
            {
                Description = "Number of hp to set (1 = half hp)"
            };
            Command setHealth = new("set", "Sets hp of player.") { worldOption, hpCount };
            healthCommand.Subcommands.Add(setHealth);

            setHealth.SetAction(context => _playerCommands.SetHealth(context.GetValue(worldOption), context.GetValue(hpCount)));
        }

        private void RegisterSpawnCommands(Command playerCommand, Option<int> worldOption, Argument<int> xPositionArgument, Argument<int> yPositionArgument, Argument<int> zPositionArgument, Option<bool> safeNewPosition)
        {
            Command spawnCommand = new("spawn", "Commands related to spawn coordinates.");
            playerCommand.Subcommands.Add(spawnCommand);

            RegisterChangeSpawnPointCommand(spawnCommand, worldOption, xPositionArgument, yPositionArgument, zPositionArgument, safeNewPosition);
            RegisterReadSpawnPointCommand(spawnCommand, worldOption);
        }

        private void RegisterReadSpawnPointCommand(Command spawnCommand, Option<int> worldOption)
        {
            Command readSpawn = new("read", "Displays coordinates of spawn.");
            spawnCommand.Subcommands.Add(readSpawn);
            readSpawn.Aliases.Add("check");
            readSpawn.SetAction(context => _playerCommands.ReadSpawn(context.GetValue(worldOption)));
        }

        private void RegisterChangeSpawnPointCommand(Command spawnCommand, Option<int> worldOption, Argument<int> xPositionArgument, Argument<int> yPositionArgument, Argument<int> zPositionArgument, Option<bool> safeNewPosition)
        {
            Command changeSpawnPoint = new("set", "Sets new spawnpoint.") { xPositionArgument, yPositionArgument, zPositionArgument, safeNewPosition };
            changeSpawnPoint.Aliases.Add("change");
            spawnCommand.Subcommands.Add(changeSpawnPoint);
            changeSpawnPoint.SetAction(context => _playerCommands.SetSpawnPoint(context.GetValue(worldOption), context.GetValue(xPositionArgument), context.GetValue(yPositionArgument), context.GetValue(zPositionArgument), context.GetValue(safeNewPosition)));
        }

        private void RegisterPositionCommands(Command playerCommand, Option<int> worldOption, Argument<int> xPositionArgument, Argument<int> yPositionArgument, Argument<int> zPositionArgument, Option<bool> safeNewPosition)
        {
            Command positionCommand = new("position", "Operations on player's position");
            playerCommand.Subcommands.Add(positionCommand);

            Option<bool> specificReadPosition = new("--specific")
            {
                Description = "Defines wheather coordinates should be rounded."
            };
            specificReadPosition.Aliases.Add("--exact");
            specificReadPosition.Aliases.Add("--not-rounded");

            RegisterReadPositionCommand(positionCommand, worldOption, specificReadPosition);
            RegisterChangePositionCommand(positionCommand, worldOption, xPositionArgument, yPositionArgument, zPositionArgument, safeNewPosition);
        }

        private void RegisterChangePositionCommand(Command positionCommand, Option<int> worldOption, Argument<int> xPositionArgument, Argument<int> yPositionArgument, Argument<int> zPositionArgument, Option<bool> safeNewPosition)
        {
            Command changePosition = new("set", "Sets position of player to specified one") { xPositionArgument, yPositionArgument, zPositionArgument, safeNewPosition };
            changePosition.Aliases.Add("move");
            positionCommand.Subcommands.Add(changePosition);
            changePosition.SetAction(context => _playerCommands.SetPlayerPosition(context.GetValue(worldOption), context.GetValue(xPositionArgument), context.GetValue(yPositionArgument), context.GetValue(zPositionArgument), context.GetValue(safeNewPosition)));
        }

        private void RegisterReadPositionCommand(Command positionCommand, Option<int> worldOption, Option<bool> specificReadPosition)
        {
            Command readPosition = new("read", "Reads position of player and displays it") { specificReadPosition };
            readPosition.Aliases.Add("check");
            positionCommand.Subcommands.Add(readPosition);
            readPosition.SetAction(context => _playerCommands.ReadPosition(context.GetValue(worldOption), context.GetValue(specificReadPosition)));
        }
    }
}
