namespace DungeonGame;

public class Map
{
    private int[,] _map;
    private int _width;
    private int _height;
    private readonly int _density;   
    
    
    public Map(int width, int height, int density)
    {
        _width = width;
        _height = height;
        _density = density;
        _map = new int[height,width];
        GenerateMap();
        
        ProcessMap();
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
        Random rng = new Random(Program.Seed);

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

    void ProcessMap()
    {
        var roomRegions = GetRegions(0);
    }

    private List<List<Coord>> GetRegions(int tileType) 
    {
        List<List<Coord>> regions = new ();
        var mapFlags = new int[_height, _width];

        for (var x = 0; x < _height; x ++) 
        {
            for (var y = 0; y < _width; y ++) 
            {
                if (mapFlags[x, y] == 0 && _map[x, y] == tileType) 
                {
                    var newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);
                    
                    foreach (var tile in newRegion) 
                    {
                        mapFlags[tile.TileX, tile.TileY] = 1;
                    }
                }
            }
        }
        return regions;
    }

    private List<Coord> GetRegionTiles(int startX, int startY) {
        List<Coord> tiles = new List<Coord> ();
        var mapFlags = new int[_height, _width];
        var tileType = _map[startX, startY];

        var queue = new Queue<Coord> ();
        queue.Enqueue (new Coord (startX, startY));
        mapFlags [startX, startY] = 1;

        while (queue.Count > 0) 
        {
            var tile = queue.Dequeue();
            tiles.Add(tile);

            for (var x = tile.TileX - 1; x <= tile.TileX + 1; x++) 
            {
                for (var y = tile.TileY - 1; y <= tile.TileY + 1; y++) 
                {
                    if (IsInMapRange(x,y) && (y == tile.TileY || x == tile.TileX)) 
                    {
                        if (mapFlags[x,y] == 0 && _map[x,y] == tileType) 
                        {
                            mapFlags[x,y] = 1;
                            queue.Enqueue(new Coord(x,y));
                        }
                    }
                }
            }
        }
        return tiles;
    }

    private bool IsInMapRange(int x, int y) {
        return x >= 0 && x < _height && y >= 0 && y < _width;
    }

    public void PrintMap()
    {
        for(var i = 0; i < _height; i++)
        {
            for(var j = 0; j < _width; j++)
            {
                Console.Write(_map[i, j] == 1 ? "\u2588\u2588" : "  ");
            }
            Console.WriteLine("");
        }
    }
    
    private class Coord {
        public int TileX;
        public int TileY;

        public Coord() {
        }

        public Coord(int x, int y) {
            TileX = x;
            TileY = y;
        }
    }
}