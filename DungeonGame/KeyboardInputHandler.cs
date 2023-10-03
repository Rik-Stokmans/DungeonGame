namespace DungeonGame;

public class KeyboardInputHandler
{
    
    public static void HandleInput(Player player, Dungeon dungeon)
    {
        //check if there is a key available
        if (!Console.KeyAvailable) return; 
        
        var map = dungeon.Map;
        var key = Console.ReadKey().Key;
        var currentTile = map.Tiles[map.TileMap[player.TileLocation.X, player.TileLocation.Y]];
        
        switch (key)
        {
            //Movement Up
            case ConsoleKey.W:
                switch (player.RelativeLocation.Y)
                {
                    case > 0 and <= 4:
                    {
                        if (currentTile[player.RelativeLocation.Y - 1, player.RelativeLocation.X] == "  ")
                        {
                            player.RelativeLocation.Y--;
                        }
                        break;
                    }
                    case 0:
                        player.RelativeLocation.Y = 4;
                        player.TileLocation.Y--;
                        break;
                }
                break;
            //Movement Down
            case ConsoleKey.S:
                switch (player.RelativeLocation.Y)
                {
                    case >= 0 and < 4:
                    {
                        if (currentTile[player.RelativeLocation.Y + 1, player.RelativeLocation.X] == "  ")
                        {
                            player.RelativeLocation.Y++;
                        }
                        break;
                    }
                    case 4:
                        player.RelativeLocation.Y = 0;
                        player.TileLocation.Y++;
                        break;
                }
                break;
            //Movement Left
            case ConsoleKey.A:
                switch (player.RelativeLocation.X)
                {
                    case > 0 and <= 4:
                    {
                        if (currentTile[player.RelativeLocation.Y, player.RelativeLocation.X - 1] == "  ")
                        {
                            player.RelativeLocation.X--;
                        }
                        break;
                    }
                    case 0:
                        player.RelativeLocation.X = 4;
                        player.TileLocation.X--;
                        break;
                }
                break;
            //Movement Right
            case ConsoleKey.D:
                switch (player.RelativeLocation.X)
                {
                    case >= 0 and < 4:
                    {
                        if (currentTile[player.RelativeLocation.Y, player.RelativeLocation.X + 1] == "  ")
                        {
                            player.RelativeLocation.X++;
                        }
                        break;
                    }
                    case 4:
                        player.RelativeLocation.X = 0;
                        player.TileLocation.X++;
                        break;
                }
                break;
        }
    }
}