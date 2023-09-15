namespace DungeonGame;

public class Map
{
    private int[,] map;
    private int width;
    private int height;
    private int density;
    
    
    public Map(int width, int height, int density)
    {
        this.width = width;
        this.height = height;
        this.density = density;
        map = new int[height,width];
        generateBoard();
    }

    private void generateBoard()
    {
        
        RandomFillMap();

        for (int i = 0; i < 4; i++) {
            SmoothMap();
        }
        
        printBoard();

        //ProcessMap();
        
    }

    void SmoothMap()
    {
        int[,] smoothedMap = new int[height, width];

        for (var i = 0; i < height; i++)
        {
            for (var j = 0; j < width; j++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(i, j);

                smoothedMap[i, j] = neighbourWallTiles switch
                {
                    > 4 => 1,
                    < 4 => 0,
                    _ => smoothedMap[i, j]
                };
            }
        }
        map = smoothedMap;
    }

    int GetSurroundingWallCount(int x, int y)
    {
        int WallCount = 0;

        for (int neighbourX = x - 1; neighbourX <= x + 1; neighbourX++) {
            for (int neighbourY = y - 1; neighbourY <= y + 1; neighbourY++) {
                if (neighbourX >= 0 && neighbourX < height && neighbourY >= 0 && neighbourY < width) {
                    if (neighbourX != x || neighbourY != y) {
                        WallCount += map[neighbourX,neighbourY];
                    }
                } else {
                    WallCount++;
                }
            }
        }
        return WallCount;
    }

    void RandomFillMap()
    {
        Random rng = new Random(Programm.seed);

        for(var i = 0; i < height; i++)
        {
            for(var j = 0; j < width; j++)
            {
                if (j == 0 || j == width -1 || i == 0 || i == height -1) {
                    map[i,j] = 1;
                } else
                {
                    if (rng.NextDouble() * 100 <= density) map[i, j] = 1;
                }
            }
        }
    }

    public void printBoard()
    {
        for(var i = 0; i < height; i++)
        {
            for(var j = 0; j < width; j++)
            {
                if (map[i, j] == 1) 
                {
                    Console.Write("\u2588");
                }
                else
                {
                    Console.Write(" ");
                }
            }
            Console.WriteLine("");
        }
    }
}