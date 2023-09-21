namespace DungeonGame;

public class Dungeon
{
    public Map Map;
    public List<Enemy> Enemies = new ();
    
    public Dungeon(int width, int height, int density)
    {
        //generates the map
        Map = new Map(width, height, density);
        
        //spawns the enemies
        GenerateEnemies();
    }

    private void GenerateEnemies()
    {
        Enemies.Add(new Enemy());
    }
    
}