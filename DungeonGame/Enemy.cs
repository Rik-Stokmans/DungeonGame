namespace DungeonGame;

public class Enemy : Entity
{
    
    public Enemy(EnemyType enemyType, Map.Coord position, int id)
    {
        Position = position;
        ID = id;
        GenerateEnemy(enemyType);
    }
    
    private void GenerateEnemy(EnemyType enemyType)
    {
        //sets the enemy on the map
        Dungeon.Map.MapSquareMap[Position.X, Position.Y].HasEnemy = true;
        Dungeon.Map.MapSquareMap[Position.X, Position.Y].IsWalkable = false;
        Dungeon.Map.MapSquareMap[Position.X, Position.Y].EnemyId = ID;
        
        //generates the enemy
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

    public void Move()
    {
        Map.Coord playerLocation = Program.Player.Position;
        Map.Coord bestMove = Position;
        double distanceToPlayerAfterBestMove = Map.Coord.GetDistance(Position, playerLocation);

        for (int i = Position.X - 1; i <= Position.X + 1; i++)
        {
            if (!Dungeon.Map.MapSquareMap[i, Position.Y].IsWalkable) continue;
            if (Position.X == i) continue;

            double distanceToPlayerAfterMove = Map.Coord.GetDistance(new Map.Coord(i, Position.Y), playerLocation);
            
            if (distanceToPlayerAfterMove < distanceToPlayerAfterBestMove)
            {
                bestMove = new Map.Coord(i, Position.Y);
                distanceToPlayerAfterBestMove = distanceToPlayerAfterMove;
            }
        }

        for (int j = Position.Y - 1; j <= Position.Y + 1; j++)
        {
            if (!Dungeon.Map.MapSquareMap[Position.X, j].IsWalkable) continue;
            if (Position.Y == j) continue;

            double distanceToPlayerAfterMove = Map.Coord.GetDistance(new Map.Coord(Position.X, j), playerLocation);

            if (distanceToPlayerAfterMove < distanceToPlayerAfterBestMove)
            {
                bestMove = new Map.Coord(Position.X, j);
                distanceToPlayerAfterBestMove = distanceToPlayerAfterMove;
            }
        }
        
        
        //makes the best move
        int enemyId = Dungeon.Map.MapSquareMap[Position.X, Position.Y].EnemyId;
        
        Dungeon.Map.MapSquareMap[Position.X, Position.Y].HasEnemy = false;
        Dungeon.Map.MapSquareMap[Position.X, Position.Y].EnemyId = -1;
        Dungeon.Map.MapSquareMap[Position.X, Position.Y].IsWalkable = true;
        Position = bestMove;
        Dungeon.Map.MapSquareMap[Position.X, Position.Y].HasEnemy = true;
        Dungeon.Map.MapSquareMap[Position.X, Position.Y].EnemyId = enemyId;
        Dungeon.Map.MapSquareMap[Position.X, Position.Y].IsWalkable = false;
    }
}