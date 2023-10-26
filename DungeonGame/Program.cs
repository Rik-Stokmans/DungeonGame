using System;
using System.Threading;
using System.Threading.Tasks;

namespace DungeonGame;

public static class Program
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
        
        //generate the player entity //todo make the player spawn in a random location
        Player = new Player(new Map.Coord(5, 5));
        
        //render the new map
        for (int i = 0; i < Dungeon.Map.MapSquareMap.GetLength(0); i++)
        {
            for (int j = 0; j < Dungeon.Map.MapSquareMap.GetLength(1); j++)
            {
                Console.Write(Dungeon.Map.MapSquareMap[i, j].IsWall ? '#' : ' ');
            }
            Console.WriteLine();
        }
        
        Thread gameLoop = new Thread(new ThreadStart(GameLoopWorker));
        gameLoop.Start();
        
        Thread musicPlayer = new Thread(new ThreadStart(MusicPlayerWorker));
        musicPlayer.Start();
        
        void GameLoopWorker() {
            while (true)
            {
                //await a player movement (returns true if the player loaded new chunks)
                KeyboardInputHandler.HandleInput(Player, Dungeon);
                
                //handle enemy movement
                
                
                //render the new map
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
    public static int CreatePseudoRandomSeed()
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