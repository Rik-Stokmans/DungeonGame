using System;
using System.Threading;
using System.Threading.Tasks;

namespace DungeonGame;

static class Program
{
    public static readonly int Seed = CreatePseudoRandomSeed();
    public static Player Player;
    public static Dungeon Dungeon;
    public static readonly int RenderDistance = 2;

    private static readonly String MainDungeonMusicFile =
        "/Users/rikstokmans/RiderProjects/DungeonGame/DungeonGame/sounds/Ruins.wav";
    private static readonly String EnemyApproachingMusicFile =
        "/Users/rikstokmans/RiderProjects/DungeonGame/DungeonGame/sounds/EnemyApproaching.wav";
    private static readonly String DatingFightMusicFile =
        "/Users/rikstokmans/RiderProjects/DungeonGame/DungeonGame/sounds/DatingFight.wav";
    private static readonly String SaveTheWorldMusicFile =
        "/Users/rikstokmans/RiderProjects/DungeonGame/DungeonGame/sounds/SaveTheWorld.wav";
    private static readonly String BonetrousleMusicFile =
        "/Users/rikstokmans/RiderProjects/DungeonGame/DungeonGame/sounds/Bonetrousle.wav";

    public static void Main()
    {
        //generate the dungeon and the player
        Dungeon = new Dungeon(50, 50, 54);
        Player = new Player(new Location(Dungeon.Map.PlayerSpawnTile.TileX, Dungeon.Map.PlayerSpawnTile.TileY), new Location(2, 2));
        
        //render the new map
        Dungeon.Map.PrintTiles(new Map.Coord(Player.TileLocation.X, Player.TileLocation.Y), RenderDistance, RenderDistance);
        
        
        Thread gameLoop = new Thread(new ThreadStart(GameLoopWorker));
        gameLoop.Start();
        
        Thread musicPlayer = new Thread(new ThreadStart(MusicPlayerWorker));
        musicPlayer.Start();
        
        void GameLoopWorker() {
            while (true)
            {
                //await a player movement (returns true if the player loaded new chunks)
                List<Location> chunksToBeLoaded = KeyboardInputHandler.HandleInput(Player, Dungeon);
                if (chunksToBeLoaded.Count > 0)
                {
                    //handle chunk loading (eg. enemy spawns)
                    foreach (var loc in chunksToBeLoaded)
                    {
                        Dungeon.Map.LoadedChunks[loc.X, loc.Y] = true;
                        Dungeon.Map.SpawnEnemiesInChunk(loc);
                    }
                }
                
                //render the new map
                Dungeon.Map.PrintTiles(new Map.Coord(Player.TileLocation.X, Player.TileLocation.Y), RenderDistance, RenderDistance);
            }
        }
        

        async void MusicPlayerWorker()
        {
            while (true)
            {
                await Audio.Play(BonetrousleMusicFile, new PlaybackOptions { Rate = 0.5, Quality = 1, Time = 0});
                await Audio.Play(MainDungeonMusicFile, new PlaybackOptions { Rate = 0.5, Quality = 1, Time = 0});
                await Audio.Play(SaveTheWorldMusicFile, new PlaybackOptions { Rate = 0.5, Quality = 1, Time = 0});
                await Audio.Play(DatingFightMusicFile, new PlaybackOptions { Rate = 0.5, Quality = 1, Time = 0});
                await Audio.Play(EnemyApproachingMusicFile, new PlaybackOptions { Rate = 0.5, Quality = 1, Time = 0});
            }
        }
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