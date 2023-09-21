using System.Diagnostics;

namespace DungeonGame;

static class Program
{
    public static readonly int Seed = CreatePseudoRandomSeed();
    public static Player Player;
    private static Dungeon? _dungeon;

    public static void Main()
    {
        _dungeon = new Dungeon(52, 52, 54);
        
        Player = new Player(new Location(_dungeon.Map.PlayerSpawnTile.TileX, _dungeon.Map.PlayerSpawnTile.TileY), new Location(2, 2));
        
        
        
        //prints the entire map
        _dungeon.Map.PrintTiles(new Map.Coord(0, 0), 25, 25);

        _dungeon.Map.PrintTiles(new Map.Coord(Player.TileLocation.X, Player.TileLocation.Y), 5, 5);
        /* testing
        bool flipSwitch = false;
        for (int i = 0; i < 10000; i++)
        {
            if (!flipSwitch)
            {
                Player.TileLocation.X++;
                Player.TileLocation.Y++;
            }
            else
            {
                Player.TileLocation.X--;
                Player.TileLocation.Y--;
            }

            flipSwitch = Player.TileLocation.X switch
            {
                49 => true,
                0 => false,
                _ => flipSwitch
            };
            _dungeon.Map.PrintTiles(new Map.Coord(Player.TileLocation.X, Player.TileLocation.Y), 5, 5);
            Thread.Sleep(100);
        }
        */
    }
    
    //function to generate the seed
    private static int CreatePseudoRandomSeed()
    {
        var randomSeed = 0;
        var iterations = 0;
        var date = DateTime.Now.ToString("F").ToCharArray();
        foreach (var item in date)
        {
            //generates random seed based on the date
            randomSeed += item * iterations + iterations;
            iterations++;
        }
        return randomSeed;
    }
}