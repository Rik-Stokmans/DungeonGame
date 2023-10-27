namespace DungeonGame;

public class Dungeon
{
    public static Map Map = null!;
    
    public Dungeon(int width, int height, int density)
    {
        //generates the map
        Map = new Map(width, height, density);
    }
    
    
}