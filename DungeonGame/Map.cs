namespace DungeonGame;

public class Map
{
    private int[,] _map;
    private readonly int _width;
    private readonly int _height;
    private readonly int _density;   
    
    
    public Map(int width, int height, int density)
    {
        _width = width;
        _height = height;
        _density = density;
        _map = new int[height,width];
        GenerateMap();
    }

    private void GenerateMap()
    {
        
        RandomFillMap();

        for (int i = 0; i < 2; i++) {
            SmoothMap();
        }
        
    }

    private void SmoothMap()
    {
        int[,] smoothedMap = new int[_height, _width];

        for (var i = 0; i < _height; i++)
        {
            for (var j = 0; j < _width; j++)
            {
                var neighbourWallTiles = GetSurroundingWallCount(i, j);

                smoothedMap[i, j] = neighbourWallTiles switch
                {
                    > 4 => 1,
                    < 4 => 0,
                    _ => smoothedMap[i, j]
                };
            }
        }
        _map = smoothedMap;
    }

    int GetSurroundingWallCount(int x, int y)
    {
        var wallCount = 0;

        for (int neighbourX = x - 1; neighbourX <= x + 1; neighbourX++) {
            for (int neighbourY = y - 1; neighbourY <= y + 1; neighbourY++) {
                if (neighbourX >= 0 && neighbourX < _height && neighbourY >= 0 && neighbourY < _width) {
                    if (neighbourX != x || neighbourY != y) {
                        wallCount += _map[neighbourX,neighbourY];
                    }
                } else {
                    wallCount++;
                }
            }
        }
        return wallCount;
    }

    void RandomFillMap()
    {
        Random rng = new Random(Programm.Seed);

        for(var i = 0; i < _height; i++)
        {
            for(var j = 0; j < _width; j++)
            {
                if (j == 0 || j == _width -1 || i == 0 || i == _height -1) {
                    _map[i,j] = 1;
                } else
                {
                    if (rng.NextDouble() * 100 <= _density) _map[i, j] = 1;
                }
            }
        }
    }

    public void PrintMap()
    {
        for(var i = 0; i < _height; i++)
        {
            for(var j = 0; j < _width; j++)
            {
                Console.Write(_map[i, j] == 1 ? "\u2588" : " ");
            }
            Console.WriteLine("");
        }
    }
}