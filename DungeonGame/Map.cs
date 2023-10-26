namespace DungeonGame;

public class Map
{
    private static readonly List<bool[,]> Tiles = GenerateTiles();
    private static readonly Random Rng = new(Program.Seed);
    private readonly int _height;
    private readonly int _width;
    private const int RoomDensity = 200;
    private const int RoomMinSize = 4;
    private const int PathSize = 1;
    
    public MapSquare[,] MapSquareMap = null!;
    

    public Map(int width, int height, int density = 54)
    { 
        _width = width;
        _height = height;

        bool foundValidMap = false;
        while (!foundValidMap) //loops until a valid map is found and then stops
        {
            bool[,] bitMap = RandomFillMap(density);

            for (var i = 0; i < 2; i++)
            {
                SmoothMap(ref bitMap);
            }

            List<List<Coord>> regions = GetRegions(false, bitMap);

            if (regions.Count >= Math.Ceiling((double)(_width * _height) / RoomDensity))
            {
                ProcessMap(regions, ref bitMap);
                int[,] tileMap = GenerateTileMap(bitMap);
                
                MapSquareMap = GenerateMapSquareMap(tileMap);

                foundValidMap = true;
            }
        }
    }
    
    private bool[,] RandomFillMap(int density)
    {
        bool[,] bitMap = new bool[_width, _height];
        for (var i = 0; i < _width; i++)
        {
            for (var j = 0; j < _height; j++)
            {
                if (j == 0 || j == _height - 1 || i == 0 || i == _width - 1)
                {
                    bitMap[i, j] = true;
                }
                else
                {
                    if (Rng.NextDouble() * 100 <= density) bitMap[i, j] = true;
                }
            }
        }
        return bitMap;
    }
    
    private void SmoothMap(ref bool[,] bitMap)
    {
        bool[,] smoothedMap = new bool[_width, _height];

        for (var i = 0; i < _width; i++)
        {
            for (var j = 0; j < _height; j++)
            {
                var neighbourWallTiles = GetSurroundingWallCount(i, j, bitMap);

                smoothedMap[i, j] = neighbourWallTiles switch
                {
                    > 4 => true,
                    < 4 => false,
                    _ => smoothedMap[i, j]
                };
            }
        }

        bitMap = smoothedMap;
    }

    int GetSurroundingWallCount(int gridX, int gridY, bool[,] bitMap)
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
                        if (bitMap[neighbourX, neighbourY]) wallCount++;
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
    
    void ProcessMap(List<List<Coord>> regions, ref bool[,] bitMap)
    {
        Console.WriteLine(regions.Count);
        List<Room> survivingRooms = new List<Room>();

        foreach (List<Coord> roomRegion in regions)
        {
            if (roomRegion.Count < RoomMinSize)
            {
                foreach (Coord tile in roomRegion)
                {
                    bitMap[tile.X, tile.Y] = true;
                }
            }
            else
            {
                survivingRooms.Add(new Room(roomRegion, bitMap));
            }
        }

        if (survivingRooms.Count < 0)
        {
            return;
        }
        
        survivingRooms.Sort();
        survivingRooms[0].IsAccessibleFromMainRoom = true;

        ConnectClosestRooms(survivingRooms, ref bitMap);
    }

    List<List<Coord>> GetRegions(bool tileType, bool[,] bitMap)
    {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[_width, _height];

        for (var x = 0; x < _width; x++)
        {
            for (var y = 0; y < _height; y++)
            {
                if (mapFlags[x, y] == 0 && bitMap[x, y] == tileType)
                {
                    List<Coord> newRegion = GetRegionTiles(x, y, bitMap);
                    regions.Add(newRegion);

                    foreach (Coord tile in newRegion)
                    {
                        mapFlags[tile.X, tile.Y] = 1;
                    }
                }
            }
        }
        return regions;
    }
    
    List<Coord> GetRegionTiles(int startX, int startY, bool[,] bitMap)
    {
        List<Coord> tiles = new List<Coord>();
        var mapFlags = new int[_width, _height];
        var tileType = bitMap[startX, startY];

        var queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        mapFlags[startX, startY] = 1;
        
        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            for (var x = tile.X - 1; x <= tile.X + 1; x++)
            {
                for (var y = tile.Y - 1; y <= tile.Y + 1; y++)
                {
                    if (IsInMapRange(x, y) && (y == tile.Y || x == tile.X))
                    {
                        if (mapFlags[x, y] == 0 && bitMap[x, y] == tileType)
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
        return x >= 0 && x < _width && y >= 0 && y < _height;
    }

    void ConnectClosestRooms(List<Room> allRooms, ref bool[,] bitMap, bool forceAccessibilityFromMainRoom = false)
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
                        int distanceBetweenRooms = (int)(Math.Pow(tileA.X - tileB.X, 2) +
                                                         Math.Pow(tileA.Y - tileB.Y, 2));

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
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB, ref bitMap);
            }
        }

        if (possibleConnectionFound && forceAccessibilityFromMainRoom)
        {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB, ref bitMap);
            ConnectClosestRooms(allRooms, ref bitMap, true);
        }

        if (!forceAccessibilityFromMainRoom)
        {
            ConnectClosestRooms(allRooms, ref bitMap, true);
        }
    }

    void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB, ref bool[,] bitMap)
    {
        Room.ConnectRooms(roomA, roomB);

        List<Coord> line = GetLine(tileA, tileB);
        foreach (Coord c in line)
        {
            DrawCircle(c, PathSize, ref bitMap);
        }
    }

    void DrawCircle(Coord c, int r, ref bool[,] bitMap)
    {
        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if (x * x + y * y <= r * r)
                {
                    int drawX = c.X + x;
                    int drawY = c.Y + y;
                    if (IsInMapRange(drawX, drawY))
                    {
                        bitMap[drawX, drawY] = false;
                    }
                }
            }
        }
    }

    List<Coord> GetLine(Coord from, Coord to)
    {
        List<Coord> line = new List<Coord>();

        int x = from.X;
        int y = from.Y;

        int dx = to.X - from.X;
        int dy = to.Y - from.Y;

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
    
    private int[,] GenerateTileMap(bool[,] bitMap)
    {
        int[,] tileMap = new int[_width - 1, _height - 1];
        for (var i = 0; i < _height - 1; i++)
        {
            for (var j = 0; j < _width - 1; j++)
            {
                tileMap[i, j] = 0;
                if (bitMap[i, j]) tileMap[i, j] += 1;
                if (bitMap[i+1, j]) tileMap[i, j] += 2;
                if (bitMap[i, j+1]) tileMap[i, j] += 4;
                if (bitMap[i+1, j+1]) tileMap[i, j] += 8;
            }
        }
        return tileMap;
    }

    private MapSquare[,] GenerateMapSquareMap(int[,] tileMap)
    {
        MapSquare[,] mapSquareMap = new MapSquare[(_width - 1) * 5, (_height - 1) * 5];
        
        for (var j = 0; j < _height - 1; j++)
        {
            for (var y = 0; y < 5; y++)
            {
                for (var i = 0; i < _width - 1; i++)
                {
                    for (var x = 0; x < 5; x++)
                    {
                        mapSquareMap[i * 5 + x, j * 5 + y] = new MapSquare
                        {
                            IsWall = Tiles[tileMap[i, j]][y, x],
                            HasEnemy = false,
                            EnemyID = -1,
                            HasPlayer = false,
                            IsWalkable = !Tiles[tileMap[i, j]][y, x]
                        };
                    }
                }
            }
        }
        return mapSquareMap;
    }
    
    public struct Coord
    {
        public int X;
        public int Y;

        public Coord(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public static List<bool[,]> GenerateTiles()
    {
        var tiles = new List<bool[,]>
        {
            new[,]
            {
                {false,false,false,false,false},
                {false,false,false,false,false},
                {false,false,false,false,false},
                {false,false,false,false,false},
                {false,false,false,false,false}
            },
            new[,]
            {
                {true,true,false,false,false},
                {true,false,false,false,false},
                {false,false,false,false,false},
                {false,false,false,false,false},
                {false,false,false,false,false}
            },
            new[,]
            {
                {false,false,false,true,true},
                {false,false,false,false,true},
                {false,false,false,false,false},
                {false,false,false,false,false},
                {false,false,false,false,false}
            },
            new[,]
            {
                {true,true,true,true,true},
                {true,true,true,true,true},
                {false,false,false,false,false},
                {false,false,false,false,false},
                {false,false,false,false,false}
            },
            new[,]
            {
                {false,false,false,false,false},
                {false,false,false,false,false},
                {false,false,false,false,false},
                {true,false,false,false,false},
                {true,true,false,false,false}
            },
            new[,]
            {
                {true,true,false,false,false},
                {true,true,false,false,false},
                {true,true,false,false,false},
                {true,true,false,false,false},
                {true,true,false,false,false}
            },
            new[,]
            {
                {false,false,false,true,true},
                {false,false,false,false,true},
                {false,false,false,false,false},
                {true,false,false,false,false},
                {true,true,false,false,false}
            },
            new[,]
            {
                {true,true,true,true,true},
                {true,true,true,true,true},
                {true,true,true,false,false},
                {true,true,false,false,false},
                {true,true,false,false,false}
            },
            new[,]
            {
                {false,false,false,false,false},
                {false,false,false,false,false},
                {false,false,false,false,false},
                {false,false,false,false,true},
                {false,false,false,true,true}
            },
            new[,]
            {
                {true,true,false,false,false},
                {true,false,false,false,false},
                {false,false,false,false,false},
                {false,false,false,false,true},
                {false,false,false,true,true}
            },
            new[,]
            {
                {false,false,false,true,true},
                {false,false,false,true,true},
                {false,false,false,true,true},
                {false,false,false,true,true},
                {false,false,false,true,true}
            },
            new[,]
            {
                {true,true,true,true,true},
                {true,true,true,true,true},
                {false,false,true,true,true},
                {false,false,false,true,true},
                {false,false,false,true,true}
            },
            new[,]
            {
                {false,false,false,false,false},
                {false,false,false,false,false},
                {false,false,false,false,false},
                {true,true,true,true,true},
                {true,true,true,true,true}
            },
            new[,]
            {
                {true,true,false,false,false},
                {true,true,false,false,false},
                {true,true,true,false,false},
                {true,true,true,true,true},
                {true,true,true,true,true}
            },
            new[,]
            {
                {false,false,false,true,true},
                {false,false,false,true,true},
                {false,false,true,true,true},
                {true,true,true,true,true},
                {true,true,true,true,true}
            },
            new[,]
            {
                {true,true,true,true,true},
                {true,true,true,true,true},
                {true,true,true,true,true},
                {true,true,true,true,true},
                {true,true,true,true,true}
            }
        };

        return tiles;
    }
    
    public struct MapSquare
    {
        public bool IsWall;
        public bool HasEnemy;
        public int EnemyID;
        public bool HasPlayer;
        public bool IsWalkable;
            
        public MapSquare(bool isWall, bool hasEnemy, int enemyID, bool hasPlayer, bool isWalkable)
        {
            IsWall = isWall;
            HasEnemy = hasEnemy;
            EnemyID = enemyID;
            HasPlayer = hasPlayer;
            IsWalkable = isWalkable;
        }
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

        public Room(List<Coord> roomTiles, bool[,] map)
        {
            Tiles = roomTiles;
            RoomSize = Tiles.Count;
            ConnectedRooms = new List<Room>();

            EdgeTiles = new List<Coord>();
            foreach (Coord tile in Tiles)
            {
                for (int x = tile.X - 1; x <= tile.X + 1; x++)
                {
                    for (int y = tile.Y - 1; y <= tile.Y + 1; y++)
                    {
                        if (x == tile.X || y == tile.Y)
                        {
                            if (map[x, y])
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