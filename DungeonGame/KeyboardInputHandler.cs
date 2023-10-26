namespace DungeonGame;

public static class KeyboardInputHandler
{
    public static char LastMoveLeftRight = 'd';
    //returns true if the player loaded new chunks
    public static List<Location> HandleInput(Player player, Dungeon dungeon)
    {
        var map = dungeon.Map;
        var key = Console.ReadKey().Key;
        var currentTile = Map.Tiles[map.TileMap[player.TileLocation.X, player.TileLocation.Y]];

        Location tile;
        bool playerCanMakeMove;
        switch (key)
        {
            //Movement Up
            case ConsoleKey.W or ConsoleKey.UpArrow:
                Location location1Above;
                switch (player.RelativeLocation.Y)
                {
                    case > 0 and <= 4:
                    {
                        playerCanMakeMove = true;

                        //check if the tile above the player is empty
                        tile = player.TileLocation;
                        location1Above = new Location(player.RelativeLocation.X, player.RelativeLocation.Y - 1);

                        //checks if there is an enemy or wall above the player
                        foreach (var enemy in map.enemies)
                        {
                            if (enemy.TileLocation.isSameLocation(tile) &&
                                enemy.RelativeLocation.isSameLocation(location1Above)) playerCanMakeMove = false;
                        }

                        if (currentTile[player.RelativeLocation.Y - 1, player.RelativeLocation.X] == Map.WallTile)
                            playerCanMakeMove = false;

                        //makes the move
                        if (playerCanMakeMove) player.RelativeLocation.Y--;
                        break;
                    }
                    case 0:
                        playerCanMakeMove = true;

                        //check if the tile above the player is empty
                        tile = new Location(player.TileLocation.X, player.TileLocation.Y - 1);
                        ;
                        location1Above = new Location(player.RelativeLocation.X, 4);

                        foreach (var enemy in map.enemies)
                        {
                            if (enemy.TileLocation.isSameLocation(tile) &&
                                enemy.RelativeLocation.isSameLocation(location1Above)) playerCanMakeMove = false;
                        }

                        if (playerCanMakeMove)
                        {
                            player.RelativeLocation.Y = 4;
                            player.TileLocation.Y--;
                            return ChunksToBeLoaded(player, dungeon);
                        }
                        break;
                }
                break;
            
            //Movement Down
            case ConsoleKey.S or ConsoleKey.DownArrow:
                Location location1Below;
                switch (player.RelativeLocation.Y)
                {
                    case >= 0 and < 4:
                    {
                        playerCanMakeMove = true;

                        //check if the tile above the player is empty
                        tile = player.TileLocation;
                        location1Below = new Location(player.RelativeLocation.X, player.RelativeLocation.Y + 1);

                        //checks if there is an enemy or wall above the player
                        foreach (var enemy in map.enemies)
                        {
                            if (enemy.TileLocation.isSameLocation(tile) &&
                                enemy.RelativeLocation.isSameLocation(location1Below)) playerCanMakeMove = false;
                        }

                        if (currentTile[player.RelativeLocation.Y + 1, player.RelativeLocation.X] == Map.WallTile)
                            playerCanMakeMove = false;

                        //makes the move
                        if (playerCanMakeMove) player.RelativeLocation.Y++;
                        break;
                    }
                    case 4:
                        playerCanMakeMove = true;

                        //check if the tile above the player is empty
                        tile = new Location(player.TileLocation.X, player.TileLocation.Y + 1);
                        
                        location1Below = new Location(player.RelativeLocation.X, 0);

                        foreach (var enemy in map.enemies)
                        {
                            if (enemy.TileLocation.isSameLocation(tile) &&
                                enemy.RelativeLocation.isSameLocation(location1Below)) playerCanMakeMove = false;
                        }

                        if (playerCanMakeMove)
                        {
                            player.RelativeLocation.Y = 0;
                            player.TileLocation.Y++;
                            return ChunksToBeLoaded(player, dungeon);
                        }
                        break;
                }
                break;
            
            //Movement Left
            case ConsoleKey.A or ConsoleKey.LeftArrow:
                Location location1Left;
                switch (player.RelativeLocation.X)
                {
                    case > 0 and <= 4:
                    {
                        playerCanMakeMove = true;

                        //check if the tile above the player is empty
                        tile = player.TileLocation;
                        location1Left = new Location(player.RelativeLocation.X - 1, player.RelativeLocation.Y);

                        //checks if there is an enemy or wall above the player
                        foreach (var enemy in map.enemies)
                        {
                            if (enemy.TileLocation.isSameLocation(tile) &&
                                enemy.RelativeLocation.isSameLocation(location1Left)) playerCanMakeMove = false;
                        }

                        if (currentTile[player.RelativeLocation.Y, player.RelativeLocation.X - 1] == Map.WallTile)
                            playerCanMakeMove = false;

                        //makes the move
                        if (playerCanMakeMove) player.RelativeLocation.X--;
                        LastMoveLeftRight = 'a';
                        break;
                    }
                    case 0:
                        playerCanMakeMove = true;

                        //check if the tile above the player is empty
                        tile = new Location(player.TileLocation.X - 1, player.TileLocation.Y);
                        ;
                        location1Left = new Location(4, player.RelativeLocation.Y);

                        foreach (var enemy in map.enemies)
                        {
                            if (enemy.TileLocation.isSameLocation(tile) &&
                                enemy.RelativeLocation.isSameLocation(location1Left)) playerCanMakeMove = false;
                        }
                        
                        LastMoveLeftRight = 'a';

                        if (playerCanMakeMove)
                        {
                            player.RelativeLocation.X = 4;
                            player.TileLocation.X--;
                            return ChunksToBeLoaded(player, dungeon);
                        }
                        break;
                }
                break;
            
            //Movement Right
            case ConsoleKey.D or ConsoleKey.RightArrow:
                Location location1Right;
                switch (player.RelativeLocation.X)
                {
                    case >= 0 and < 4:
                    {
                        playerCanMakeMove = true;

                        //check if the tile above the player is empty
                        tile = player.TileLocation;
                        location1Right = new Location(player.RelativeLocation.X + 1, player.RelativeLocation.Y);

                        //checks if there is an enemy or wall above the player
                        foreach (var enemy in map.enemies)
                        {
                            if (enemy.TileLocation.isSameLocation(tile) &&
                                enemy.RelativeLocation.isSameLocation(location1Right)) playerCanMakeMove = false;
                        }

                        if (currentTile[player.RelativeLocation.Y, player.RelativeLocation.X + 1] == Map.WallTile)
                            playerCanMakeMove = false;

                        //makes the move
                        if (playerCanMakeMove) player.RelativeLocation.X++;
                        LastMoveLeftRight = 'd';
                        break;
                    }
                    case 4:
                        playerCanMakeMove = true;

                        //check if the tile above the player is empty
                        tile = new Location(player.TileLocation.X + 1, player.TileLocation.Y);
                        ;
                        location1Right = new Location(0, player.RelativeLocation.Y);

                        foreach (var enemy in map.enemies)
                        {
                            if (enemy.TileLocation.isSameLocation(tile) &&
                                enemy.RelativeLocation.isSameLocation(location1Right)) playerCanMakeMove = false;
                        }
                        
                        LastMoveLeftRight = 'd';
                        
                        if (playerCanMakeMove)
                        {
                            player.RelativeLocation.X = 0;
                            player.TileLocation.X++;
                            return ChunksToBeLoaded(player, dungeon);
                        }
                        break;
                }
                break;
        }
        return new List<Location>();
    }

    public static List<Location> ChunksToBeLoaded(Player player, Dungeon dungeon)
    {
        List<Location> chunksToBeLoaded = new List<Location>();
        Map map = dungeon.Map;
        
        for (var i = player.TileLocation.X - 1; i <= player.TileLocation.X + 1; i++)
        {
            for (var j = player.TileLocation.Y - 1; j <= player.TileLocation.Y + 1; j++)
            {
                //check if the chunk is inside the map
                if (i >= 0 && i < map.TileMap.GetLength(0) && j >= 0 && j < map.TileMap.GetLength(1))
                {
                    if (!map.LoadedChunks[i, j])
                    {
                        chunksToBeLoaded.Add(new Location(i, j));
                    }
                }
            }
        }
        return chunksToBeLoaded;
    }
}


















