namespace DungeonGame;

public class Player : Entity
{
    
    public Player()
    {
        Speed = 0;
        Health = 10;
        Level = 0;
        Armor = new Armor();
        Position = GenerateSpawnLocation();
        Dungeon.Map.DisableEnemySpawningAtPoint(Position, 10);
        Dungeon.Map.MapSquareMap[Position.X, Position.Y].HasPlayer = true;
        Dungeon.Map.MapSquareMap[Position.X, Position.Y].IsWalkable = false;
    }

    private Map.Coord GenerateSpawnLocation()
    {
        Map.MapSquare[,] mapSquareMap = Dungeon.Map.MapSquareMap;
        //find a desirable spawn location
        for (int j = 0; j < mapSquareMap.GetLength(1); j++)
        {
            for (int i = 0; i < mapSquareMap.GetLength(0); i++)
            {
                if (isSuitibleSpawnLocation(i, j, mapSquareMap)) return new Map.Coord(i, j);
            }
        }
        //if there was no good spot we look for a less desirable spot
        for (int j = 0; j < mapSquareMap.GetLength(1); j++)
        {
            for (int i = 0; i < mapSquareMap.GetLength(0); i++)
            {
                if (mapSquareMap[i, j].IsSpawnable) return new Map.Coord(i, j);
            }
        }
        //if there was no spot we return 0,0
        return new Map.Coord(0, 0);
    }

    private bool isSuitibleSpawnLocation(int x, int y, Map.MapSquare[,] mapSquareMap)
    {
        if (mapSquareMap[x, y].IsSpawnable)
        {
            int mapSizeX = mapSquareMap.GetLength(0);
            int mapSizeY = mapSquareMap.GetLength(1);
            if (x - 2 <= 0 || x + 2 >= mapSizeX || y - 2 <= 0 || y + 2 >= mapSizeY) return false;

            for (int i = x - 2; i <= x + 2; i++)
            {
                if (mapSquareMap[i, y].IsWall) return false;
            }

            for (int i = y - 2; i <= y + 2; i++)
            {
                if (mapSquareMap[x, i].IsWall) return false;
            }
        }
        else return false;
        
        return true;
    }


}