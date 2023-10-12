namespace DungeonGame;

public class Map
{
    private int _density;
    public int Height;
    public int Width;
    private int _tileMapHeight;
    private int _tileMapWidth;
    public Coord PlayerSpawnTile;
    public int[,] BitMap = null!;
    public int[,] TileMap = null!;
    public bool[,] LoadedChunks = null!;
    public static List<String[,]> Tiles = null!;
    public List<Enemy> enemies = new();
    private const int PathSize = 1;
    private List<List<Coord>> _regions = null!;
    private const int RoomDensity = 200;
    public static readonly String VoidTile = "  ";
    public static readonly String WallTile = "\u2588\u2588";
    public static Random Rng = new(Program.Seed);
    
    public Map(int width, int height, int density)
    { 
        Tiles = GenerateTiles();
        _density = density;
        Width = width;
        Height = height;
        _tileMapWidth = width - 1;
        _tileMapHeight = height - 1;

        var foundSuitableMap = false;
        while (!foundSuitableMap)
        {
            BitMap = new int[width, height];
            TileMap = new int[width - 1, height - 1];
            RandomFillMap();

            for (var i = 0; i < 2; i++)
            {
                SmoothMap();
            }

            _regions = GetRegions(0);

            if (_regions.Count >= Math.Ceiling((double)(Width * Height) / RoomDensity))
            {
                foundSuitableMap = true;
                ProcessMap();
                GenerateTileMap(BitMap);

                PlayerSpawnTile = GeneratePlayerSpawnTile();
            }

            LoadedChunks = new bool[Width - 1, Height - 1];
            for (var i = 0; i < Width - 1; i++)
            {
                for (var j = 0; j < Height - 1; j++)
                {
                    if (TileMap[i, j] == 15)
                    {
                        LoadedChunks[i, j] = true;
                    } else {
                        LoadedChunks[i, j] = false;
                    }
                }
            }
        }
    }
    
    void RandomFillMap()
    {
        for (var i = 0; i < Width; i++)
        {
            for (var j = 0; j < Height; j++)
            {
                if (j == 0 || j == Height - 1 || i == 0 || i == Width - 1)
                {
                    BitMap[i, j] = 1;
                }
                else
                {
                    if (Rng.NextDouble() * 100 <= _density) BitMap[i, j] = 1;
                }
            }
        }
    }
    
    private void SmoothMap()
    {
        int[,] smoothedMap = new int[Width, Height];

        for (var i = 0; i < Width; i++)
        {
            for (var j = 0; j < Height; j++)
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

        BitMap = smoothedMap;
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (IsInMapRange(neighbourX, neighbourY))
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += BitMap[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }
    
    private Coord GeneratePlayerSpawnTile()
    {
        var playerSpawn = new Coord(0, 0);
        
        for (var j = 0; j < Width - 1; j++)
        {
            for (var i = 0; i < Height - 1; i++)
            {
                switch (TileMap[i, j])
                {
                    case 1:
                    case 2:
                    case 4:
                    case 8:
                        playerSpawn.TileX = i;
                        playerSpawn.TileY = j;
                        return playerSpawn;
                    case 3:
                    case 5:
                    case 10:
                    case 12:
                        playerSpawn.TileX = i;
                        playerSpawn.TileY = j;
                        return playerSpawn;
                    case 6:
                    case 9:
                        playerSpawn.TileX = i;
                        playerSpawn.TileY = j;
                        return playerSpawn;
                }
            }
        }
        return playerSpawn;
    }
    
    void ProcessMap()
    {
        Console.WriteLine(_regions.Count);
        int roomThresholdSize = 4;
        List<Room> survivingRooms = new List<Room>();

        foreach (List<Coord> roomRegion in _regions)
        {
            if (roomRegion.Count < roomThresholdSize)
            {
                foreach (Coord tile in roomRegion)
                {
                    BitMap[tile.TileX, tile.TileY] = 1;
                }
            }
            else
            {
                survivingRooms.Add(new Room(roomRegion, BitMap));
            }
        }

        if (survivingRooms.Count < 0)
        {
            return;
        }
        
        survivingRooms.Sort();
        survivingRooms[0].IsAccessibleFromMainRoom = true;

        ConnectClosestRooms(survivingRooms);
    }
    
    List<List<Coord>> GetRegions(int tileType)
    {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[Width, Height];

        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                if (mapFlags[x, y] == 0 && BitMap[x, y] == tileType)
                {
                    List<Coord> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);

                    foreach (Coord tile in newRegion)
                    {
                        mapFlags[tile.TileX, tile.TileY] = 1;
                    }
                }
            }
        }
        return regions;
    }
    
    List<Coord> GetRegionTiles(int startX, int startY)
    {
        List<Coord> tiles = new List<Coord>();
        var mapFlags = new int[Width, Height];
        var tileType = BitMap[startX, startY];

        var queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        mapFlags[startX, startY] = 1;
        
        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            for (var x = tile.TileX - 1; x <= tile.TileX + 1; x++)
            {
                for (var y = tile.TileY - 1; y <= tile.TileY + 1; y++)
                {
                    if (IsInMapRange(x, y) && (y == tile.TileY || x == tile.TileX))
                    {
                        if (mapFlags[x, y] == 0 && BitMap[x, y] == tileType)
                        {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }
        return tiles;
    }
    
    bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    void ConnectClosestRooms(List<Room> allRooms, bool forceAccessibilityFromMainRoom = false)
    {
        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();

        if (forceAccessibilityFromMainRoom)
        {
            foreach (Room room in allRooms)
            {
                if (room.IsAccessibleFromMainRoom)
                {
                    roomListB.Add(room);
                }
                else
                {
                    roomListA.Add(room);
                }
            }
        }
        else
        {
            roomListA = allRooms;
            roomListB = allRooms;
        }

        int bestDistance = 0;
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();
        bool possibleConnectionFound = false;

        foreach (Room roomA in roomListA)
        {
            if (!forceAccessibilityFromMainRoom)
            {
                possibleConnectionFound = false;
                if (roomA.ConnectedRooms.Count > 0)
                {
                    continue;
                }
            }

            foreach (Room roomB in roomListB)
            {
                if (roomA == roomB || roomA.IsConnected(roomB))
                {
                    continue;
                }

                for (int tileIndexA = 0; tileIndexA < roomA.EdgeTiles.Count; tileIndexA++)
                {
                    for (int tileIndexB = 0; tileIndexB < roomB.EdgeTiles.Count; tileIndexB++)
                    {
                        Coord tileA = roomA.EdgeTiles[tileIndexA];
                        Coord tileB = roomB.EdgeTiles[tileIndexB];
                        int distanceBetweenRooms = (int)(Math.Pow(tileA.TileX - tileB.TileX, 2) +
                                                         Math.Pow(tileA.TileY - tileB.TileY, 2));

                        if (distanceBetweenRooms < bestDistance || !possibleConnectionFound)
                        {
                            bestDistance = distanceBetweenRooms;
                            possibleConnectionFound = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
                }
            }

            if (possibleConnectionFound && !forceAccessibilityFromMainRoom)
            {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            }
        }

        if (possibleConnectionFound && forceAccessibilityFromMainRoom)
        {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(allRooms, true);
        }

        if (!forceAccessibilityFromMainRoom)
        {
            ConnectClosestRooms(allRooms, true);
        }
    }

    void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB)
    {
        Room.ConnectRooms(roomA, roomB);

        List<Coord> line = GetLine(tileA, tileB);
        foreach (Coord c in line)
        {
            DrawCircle(c, PathSize);
        }
    }

    void DrawCircle(Coord c, int r)
    {
        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if (x * x + y * y <= r * r)
                {
                    int drawX = c.TileX + x;
                    int drawY = c.TileY + y;
                    if (IsInMapRange(drawX, drawY))
                    {
                        BitMap[drawX, drawY] = 0;
                    }
                }
            }
        }
    }

    List<Coord> GetLine(Coord from, Coord to)
    {
        List<Coord> line = new List<Coord>();

        int x = from.TileX;
        int y = from.TileY;

        int dx = to.TileX - from.TileX;
        int dy = to.TileY - from.TileY;

        bool inverted = false;
        int step = Math.Sign(dx);
        int gradientStep = Math.Sign(dy);

        int longest = Math.Abs(dx);
        int shortest = Math.Abs(dy);

        if (longest < shortest)
        {
            inverted = true;
            longest = Math.Abs(dy);
            shortest = Math.Abs(dx);

            step = Math.Sign(dy);
            gradientStep = Math.Sign(dx);
        }

        int gradientAccumulation = longest / 2;
        for (int i = 0; i < longest; i++)
        {
            line.Add(new Coord(x, y));

            if (inverted)
            {
                y += step;
            }
            else
            {
                x += step;
            }

            gradientAccumulation += shortest;
            if (gradientAccumulation >= longest)
            {
                if (inverted)
                {
                    x += gradientStep;
                }
                else
                {
                    y += gradientStep;
                }

                gradientAccumulation -= longest;
            }
        }

        return line;
    }
    
    private void GenerateTileMap(int[,] map)
    {
        for (var i = 0; i < Height - 1; i++)
        {
            for (var j = 0; j < Width - 1; j++)
            {
                TileMap[i, j] = map[i, j] + map[i+1, j]*2 + map[i, j+1]*4 + map[i+1, j+1]*8;
            }
        }
    }

    
    public void PrintTiles(Coord centerTile, int sizeX, int sizeY)
    {   
        var printableMapSizeX = _tileMapWidth - (sizeX + 1);
        var printableMapSizeY = _tileMapHeight - (sizeY + 1);

        if (centerTile.TileX >= printableMapSizeX) centerTile.TileX = printableMapSizeX;
        if (centerTile.TileY >= printableMapSizeY) centerTile.TileY = printableMapSizeY;
        if (centerTile.TileX - sizeX < 0) centerTile.TileX = sizeX;
        if (centerTile.TileY - sizeY < 0) centerTile.TileY = sizeY;

        Console.Clear();
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.BackgroundColor = ConsoleColor.White;
        
        //prints the tiles that are in the bounds
        List<Enemy> tempEnemyList = new List<Enemy>(enemies);
        for (var j = 0 - sizeY; j <= sizeY; j++)
        {
            for (var y = 0; y < 5; y++)
            {
                for (var i = 0 - sizeX; i <= sizeX; i++)
                {
                    for (var x = 0; x < 5; x++)
                    {
                        var emptyTile = true;
                        foreach (var enemy in tempEnemyList)
                        {
                            if (centerTile.TileX + i == enemy.TileLocation.X &&
                                centerTile.TileY + j == enemy.TileLocation.Y &&
                                x == enemy.RelativeLocation.X &&
                                y == enemy.RelativeLocation.Y)
                            {
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.Write("O/");
                                Console.ForegroundColor = ConsoleColor.DarkBlue;
                                Console.BackgroundColor = ConsoleColor.White;
                                emptyTile = false;
                                tempEnemyList.Remove(enemy);
                                break;
                            }
                        }
                        
                        if (centerTile.TileX + i == Program.Player.TileLocation.X &&
                            centerTile.TileY + j == Program.Player.TileLocation.Y &&
                            x == Program.Player.RelativeLocation.X &&
                            y == Program.Player.RelativeLocation.Y)
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                            if (KeyboardInputHandler.LastMoveLeftRight == 'a') Console.Write("\\O");
                            else if (KeyboardInputHandler.LastMoveLeftRight == 'd') Console.Write("O/");
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            Console.BackgroundColor = ConsoleColor.White;
                            emptyTile = false;
                        }
                        if (emptyTile)
                        {
                            Console.Write(Tiles[TileMap[centerTile.TileX + i, centerTile.TileY + j]][y, x]);
                        }
                    }
                }
                Console.WriteLine("");
            }
        }
    }

    public void SpawnEnemiesInChunk(Location chunk)
    {
        if (Rng.NextDouble()*2 <= 1) enemies.Add(new Enemy(chunk, TileMap[chunk.X,chunk.Y], Enemy.EnemyType.Goblin));
    }
    
    public struct Coord
    {
        public int TileX;
        public int TileY;

        public Coord(int x, int y)
        {
            TileX = x;
            TileY = y;
        }
    }

    public static List<String[,]> GenerateTiles()
    {
        var tiles = new List<String[,]>();
        
        tiles.Add(new[,]
        {
            {VoidTile,VoidTile,VoidTile,VoidTile,VoidTile},
            {VoidTile,VoidTile,VoidTile,VoidTile,VoidTile},
            {VoidTile,VoidTile,VoidTile,VoidTile,VoidTile},
            {VoidTile,VoidTile,VoidTile,VoidTile,VoidTile},
            {VoidTile,VoidTile,VoidTile,VoidTile,VoidTile}
        });
        tiles.Add(new[,]
        {
            {WallTile,WallTile,VoidTile,VoidTile,VoidTile},
            {WallTile,VoidTile,VoidTile,VoidTile,VoidTile},
            {VoidTile,VoidTile,VoidTile,VoidTile,VoidTile},
            {VoidTile,VoidTile,VoidTile,VoidTile,VoidTile},
            {VoidTile,VoidTile,VoidTile,VoidTile,VoidTile}
        });
        tiles.Add(new[,]
        {
            {VoidTile,VoidTile,VoidTile,WallTile,WallTile},
            {VoidTile,VoidTile,VoidTile,VoidTile,WallTile},
            {VoidTile,VoidTile,VoidTile,VoidTile,VoidTile},
            {VoidTile,VoidTile,VoidTile,VoidTile,VoidTile},
            {VoidTile,VoidTile,VoidTile,VoidTile,VoidTile}
        });
        tiles.Add(new[,]
        {
            {WallTile,WallTile,WallTile,WallTile,WallTile},
            {WallTile,WallTile,WallTile,WallTile,WallTile},
            {VoidTile,VoidTile,VoidTile,VoidTile,VoidTile},
            {VoidTile,VoidTile,VoidTile,VoidTile,VoidTile},
            {VoidTile,VoidTile,VoidTile,VoidTile,VoidTile}
        });
        tiles.Add(new[,]
        {
            {VoidTile,VoidTile,VoidTile,VoidTile,VoidTile},
            {VoidTile,VoidTile,VoidTile,VoidTile,VoidTile},
            {VoidTile,VoidTile,VoidTile,VoidTile,VoidTile},
            {WallTile,VoidTile,VoidTile,VoidTile,VoidTile},
            {WallTile,WallTile,VoidTile,VoidTile,VoidTile}
        });
        tiles.Add(new[,]
        {
            {WallTile,WallTile,VoidTile,VoidTile,VoidTile},
            {WallTile,WallTile,VoidTile,VoidTile,VoidTile},
            {WallTile,WallTile,VoidTile,VoidTile,VoidTile},
            {WallTile,WallTile,VoidTile,VoidTile,VoidTile},
            {WallTile,WallTile,VoidTile,VoidTile,VoidTile}
        });
        tiles.Add(new[,]
        {
            {VoidTile,VoidTile,VoidTile,WallTile,WallTile},
            {VoidTile,VoidTile,VoidTile,VoidTile,WallTile},
            {VoidTile,VoidTile,VoidTile,VoidTile,VoidTile},
            {WallTile,VoidTile,VoidTile,VoidTile,VoidTile},
            {WallTile,WallTile,VoidTile,VoidTile,VoidTile}
        });
        tiles.Add(new[,]
        {
            {WallTile,WallTile,WallTile,WallTile,WallTile},
            {WallTile,WallTile,WallTile,WallTile,WallTile},
            {WallTile,WallTile,WallTile,VoidTile,VoidTile},
            {WallTile,WallTile,VoidTile,VoidTile,VoidTile},
            {WallTile,WallTile,VoidTile,VoidTile,VoidTile}
        });
        tiles.Add(new[,]
        {
            {VoidTile,VoidTile,VoidTile,VoidTile,VoidTile},
            {VoidTile,VoidTile,VoidTile,VoidTile,VoidTile},
            {VoidTile,VoidTile,VoidTile,VoidTile,VoidTile},
            {VoidTile,VoidTile,VoidTile,VoidTile,WallTile},
            {VoidTile,VoidTile,VoidTile,WallTile,WallTile}
        });
        tiles.Add(new[,]
        {
            {WallTile,WallTile,VoidTile,VoidTile,VoidTile},
            {WallTile,VoidTile,VoidTile,VoidTile,VoidTile},
            {VoidTile,VoidTile,VoidTile,VoidTile,VoidTile},
            {VoidTile,VoidTile,VoidTile,VoidTile,WallTile},
            {VoidTile,VoidTile,VoidTile,WallTile,WallTile}
        });
        tiles.Add(new[,]
        {
            {VoidTile,VoidTile,VoidTile,WallTile,WallTile},
            {VoidTile,VoidTile,VoidTile,WallTile,WallTile},
            {VoidTile,VoidTile,VoidTile,WallTile,WallTile},
            {VoidTile,VoidTile,VoidTile,WallTile,WallTile},
            {VoidTile,VoidTile,VoidTile,WallTile,WallTile}
        });
        tiles.Add(new[,]
        {
            {WallTile,WallTile,WallTile,WallTile,WallTile},
            {WallTile,WallTile,WallTile,WallTile,WallTile},
            {VoidTile,VoidTile,WallTile,WallTile,WallTile},
            {VoidTile,VoidTile,VoidTile,WallTile,WallTile},
            {VoidTile,VoidTile,VoidTile,WallTile,WallTile}
        });
        tiles.Add(new[,]
        {
            {VoidTile,VoidTile,VoidTile,VoidTile,VoidTile},
            {VoidTile,VoidTile,VoidTile,VoidTile,VoidTile},
            {VoidTile,VoidTile,VoidTile,VoidTile,VoidTile},
            {WallTile,WallTile,WallTile,WallTile,WallTile},
            {WallTile,WallTile,WallTile,WallTile,WallTile}
        });
        tiles.Add(new[,]
        {
            {WallTile,WallTile,VoidTile,VoidTile,VoidTile},
            {WallTile,WallTile,VoidTile,VoidTile,VoidTile},
            {WallTile,WallTile,WallTile,VoidTile,VoidTile},
            {WallTile,WallTile,WallTile,WallTile,WallTile},
            {WallTile,WallTile,WallTile,WallTile,WallTile}
        });
        tiles.Add(new[,]
        {
            {VoidTile,VoidTile,VoidTile,WallTile,WallTile},
            {VoidTile,VoidTile,VoidTile,WallTile,WallTile},
            {VoidTile,VoidTile,WallTile,WallTile,WallTile},
            {WallTile,WallTile,WallTile,WallTile,WallTile},
            {WallTile,WallTile,WallTile,WallTile,WallTile}
        });
        tiles.Add(new[,]
        {
            {WallTile,WallTile,WallTile,WallTile,WallTile},
            {WallTile,WallTile,WallTile,WallTile,WallTile},
            {WallTile,WallTile,WallTile,WallTile,WallTile},
            {WallTile,WallTile,WallTile,WallTile,WallTile},
            {WallTile,WallTile,WallTile,WallTile,WallTile}
        });
        return tiles;
    }

    private class Room : IComparable<Room>
    {
        public List<Room> ConnectedRooms = null!;
        public List<Coord> EdgeTiles = null!;
        public bool IsAccessibleFromMainRoom;
        public int RoomSize;
        public List<Coord> Tiles = null!;

        public Room() 
        {
        }

        public Room(List<Coord> roomTiles, int[,] map)
        {
            Tiles = roomTiles;
            RoomSize = Tiles.Count;
            ConnectedRooms = new List<Room>();

            EdgeTiles = new List<Coord>();
            foreach (Coord tile in Tiles)
            {
                for (int x = tile.TileX - 1; x <= tile.TileX + 1; x++)
                {
                    for (int y = tile.TileY - 1; y <= tile.TileY + 1; y++)
                    {
                        if (x == tile.TileX || y == tile.TileY)
                        {
                            if (map[x, y] == 1)
                            {
                                EdgeTiles.Add(tile);
                            }
                        }
                    }
                }
            }
        }

        public int CompareTo(Room? otherRoom)
        {
            return otherRoom!.RoomSize.CompareTo(RoomSize);
        }

        public void SetAccessibleFromMainRoom()
        {
            if (!IsAccessibleFromMainRoom)
            {
                IsAccessibleFromMainRoom = true;
                foreach (Room connectedRoom in ConnectedRooms)
                {
                    connectedRoom.SetAccessibleFromMainRoom();
                }
            }
        }

        public static void ConnectRooms(Room roomA, Room roomB)
        {
            if (roomA.IsAccessibleFromMainRoom)
            {
                roomB.SetAccessibleFromMainRoom();
            }
            else if (roomB.IsAccessibleFromMainRoom)
            {
                roomA.SetAccessibleFromMainRoom();
            }

            roomA.ConnectedRooms.Add(roomB);
            roomB.ConnectedRooms.Add(roomA);
        }

        public bool IsConnected(Room otherRoom)
        {
            return ConnectedRooms.Contains(otherRoom);
        }
    }
}