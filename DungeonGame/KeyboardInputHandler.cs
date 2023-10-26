namespace DungeonGame;

public static class KeyboardInputHandler
{
    public static char LastMoveLeftRight = 'd';
    //returns true if the player loaded new chunks
    public static void HandleInput(Player player, Dungeon dungeon)
    {
        var map = dungeon.Map;
        var key = Console.ReadKey().Key;

        bool playerCanMakeMove;
        switch (key)
        {
            //Movement Up
            case ConsoleKey.W or ConsoleKey.UpArrow:
                
                playerCanMakeMove = true;

                //makes the move
                if (playerCanMakeMove) player.Position.Y--;
                break;
            
            //Movement Down
            case ConsoleKey.S or ConsoleKey.DownArrow:
                
                playerCanMakeMove = true;
                
                //makes the move
                if (playerCanMakeMove) player.Position.Y++;
                break;
            
            //Movement Left
            case ConsoleKey.A or ConsoleKey.LeftArrow:
                
                playerCanMakeMove = true;
        
                //makes the move
                if (playerCanMakeMove) player.Position.X--;
                LastMoveLeftRight = 'a';
                break;
            
            //Movement Right
            case ConsoleKey.D or ConsoleKey.RightArrow:
                
                playerCanMakeMove = true;
        
                //makes the move
                if (playerCanMakeMove) player.Position.X++;
                LastMoveLeftRight = 'd';
                break;
        }
    }
}


















