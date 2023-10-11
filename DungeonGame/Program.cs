using System;
using System.Threading;
using System.Threading.Tasks;

namespace DungeonGame;

static class Program
{
    public static readonly int Seed = CreatePseudoRandomSeed();
    public static Player Player;
    public static Dungeon _dungeon;

    private static readonly String MainDungeonMusicFile =
        "/Users/rikstokmans/RiderProjects/DungeonGame/DungeonGame/sounds/Ruins.wav";
    private static readonly String EnemyApproachingMusicFile =
        "/Users/rikstokmans/RiderProjects/DungeonGame/DungeonGame/sounds/EnemyApproaching.wav";
    private static readonly String DatingFightMusicFile =
        "/Users/rikstokmans/RiderProjects/DungeonGame/DungeonGame/sounds/DatingFight.wav";
    private static readonly String SaveTheWorldMusicFile =
        "/Users/rikstokmans/RiderProjects/DungeonGame/DungeonGame/sounds/SaveTheWorld.wav";

    public static void Main()
    {
        _dungeon = new Dungeon(50, 50, 54);
        
        Player = new Player(new Location(_dungeon.Map.PlayerSpawnTile.TileX, _dungeon.Map.PlayerSpawnTile.TileY), new Location(2, 2));
        
        //prints the entire map
        _dungeon.Map.PrintTiles(new Map.Coord(0, 0), 24, 24);
        
        
        Thread updateLoop = new Thread(new ThreadStart(UpdateLoopWorker));
        updateLoop.Start();
        
        Thread renderLoop = new Thread(new ThreadStart(RendererWorker));
        renderLoop.Start();
        
        Thread musicPlayer = new Thread(new ThreadStart(MusicPlayerWorker));
        musicPlayer.Start();
        
        void RendererWorker() {
            while (true)
            {
                _dungeon.Map.PrintTiles(new Map.Coord(Player.TileLocation.X, Player.TileLocation.Y), 3, 3);
                Thread.Sleep(50);
            }
        }
        
        void UpdateLoopWorker() {
            while (true)
            {
                KeyboardInputHandler.HandleInput(Player, _dungeon);
                Thread.Sleep(50);
            }
        }

        async void MusicPlayerWorker()
        {
            while (true)
            {
                //await Audio.Play(SaveTheWorldMusicFile, new PlaybackOptions { Rate = 0.5, Quality = 1, Time = 0});
                await Audio.Play(DatingFightMusicFile, new PlaybackOptions { Rate = 0.5, Quality = 1, Time = 0});
                await Audio.Play(EnemyApproachingMusicFile, new PlaybackOptions { Rate = 0.5, Quality = 1, Time = 0});
                await Audio.Play(MainDungeonMusicFile, new PlaybackOptions { Rate = 0.5, Quality = 1, Time = 0});
            }
        }
        
        
        
        _dungeon.Map.PrintTiles(new Map.Coord(Player.TileLocation.X, Player.TileLocation.Y), 5, 5);
        ////update loop //TODO: make thread for this && add render loop
        
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