namespace DungeonGame;

public class Dungeon
{
    private Map _map;
    private List<Enemy> _enemies = new ();
    
    public Dungeon()
    {
        //generates the map
        _map = new Map(50, 50, 54);
        
        //spawns the enemies
        GenerateEnemies();
    }

    private void GenerateEnemies()
    {
        _enemies.Add(new Enemy());
    }
    
}