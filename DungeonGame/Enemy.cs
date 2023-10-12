namespace DungeonGame;

public class Enemy : Entity
{
    
    public Enemy(Location tileLocation, int tileType, EnemyType enemyType)
    {
        GenerateEnemy(tileLocation, tileType, enemyType);
    }

    private void GenerateEnemy(Location tileLocation, int tileType, EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Goblin:
                TileLocation = tileLocation;
                RelativeLocation = GenerateSpawnLocation(tileType);
                Speed = 1;
                Health = 10;
                Level = 1;
                Armor = new Armor();
                break;
        }
    }
    
    private static Location GenerateSpawnLocation(int tileType)
    {
        while (true)
        {
            var x = Map.Rng.Next(0, 5);
            var y = Map.Rng.Next(0, 5);
            if (Map.Tiles[tileType][y, x] == Map.VoidTile)
            {
                return new Location(x, y);
            }
        }
        
    }
    
    public enum EnemyType
    {
        Goblin
    }
}