namespace DungeonGame;

public class Dungeon
{
    public Map Map;
    
    public Dungeon(int width, int height, int density)
    {
        //generates the map
        Map = new Map(width, height, density);
    }
    
}