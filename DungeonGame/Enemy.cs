namespace DungeonGame;

public class Enemy : Entity
{
    
    public Enemy(EnemyType enemyType)
    {
        GenerateEnemy(enemyType);
    }

    private void GenerateEnemy(EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Goblin:
                Speed = 1;
                Health = 10;
                Level = 1;
                Armor = new Armor();
                break;
        }
    }
    
    public enum EnemyType
    {
        Goblin
    }
}