namespace DungeonGame;

public static class KeyboardInputHandler
{
    public static bool IsLastMoveLeft = false;
    //returns true if the player loaded new chunks
    public static void HandleInput(Player player, Dungeon dungeon)
    {
        var map = Dungeon.Map;
        var key = Console.ReadKey().Key;

        bool playerCanMakeMove;
        switch (key)
        {
            //Movement Up
            case ConsoleKey.W or ConsoleKey.UpArrow:

                playerCanMakeMove = map.MapSquareMap[player.Position.X, player.Position.Y - 1].IsWalkable;

                //makes the move
                if (playerCanMakeMove)
                {
                    map.MapSquareMap[player.Position.X, player.Position.Y].HasPlayer = false;
                    map.MapSquareMap[player.Position.X, player.Position.Y].IsWalkable = true;
                    player.Position.Y--;
                    map.MapSquareMap[player.Position.X, player.Position.Y].HasPlayer = true;
                    map.MapSquareMap[player.Position.X, player.Position.Y].IsWalkable = false;
                }
                break;
            
            //Movement Down
            case ConsoleKey.S or ConsoleKey.DownArrow:
                
                playerCanMakeMove = map.MapSquareMap[player.Position.X, player.Position.Y + 1].IsWalkable;
                
                //makes the move
                if (playerCanMakeMove)
                {
                    map.MapSquareMap[player.Position.X, player.Position.Y].HasPlayer = false;
                    map.MapSquareMap[player.Position.X, player.Position.Y].IsWalkable = true;
                    player.Position.Y++;
                    map.MapSquareMap[player.Position.X, player.Position.Y].HasPlayer = true;
                    map.MapSquareMap[player.Position.X, player.Position.Y].IsWalkable = false;
                }
                break;
            
            //Movement Left
            case ConsoleKey.A or ConsoleKey.LeftArrow:
                
                playerCanMakeMove = map.MapSquareMap[player.Position.X - 1, player.Position.Y].IsWalkable;
        
                //makes the move
                if (playerCanMakeMove)
                {
                    map.MapSquareMap[player.Position.X, player.Position.Y].HasPlayer = false;
                    map.MapSquareMap[player.Position.X, player.Position.Y].IsWalkable = true;
                    player.Position.X--;
                    map.MapSquareMap[player.Position.X, player.Position.Y].HasPlayer = true;
                    map.MapSquareMap[player.Position.X, player.Position.Y].IsWalkable = false;
                }
                IsLastMoveLeft = true;
                break;
            
            //Movement Right
            case ConsoleKey.D or ConsoleKey.RightArrow:
                
                playerCanMakeMove = map.MapSquareMap[player.Position.X + 1, player.Position.Y].IsWalkable;
        
                //makes the move
                if (playerCanMakeMove)
                {
                    map.MapSquareMap[player.Position.X, player.Position.Y].HasPlayer = false;
                    map.MapSquareMap[player.Position.X, player.Position.Y].IsWalkable = true;
                    player.Position.X++;
                    map.MapSquareMap[player.Position.X, player.Position.Y].HasPlayer = true;
                    map.MapSquareMap[player.Position.X, player.Position.Y].IsWalkable = false;
                }
                IsLastMoveLeft = false;
                break;
        }
    }
}


















