using System.Diagnostics;

namespace DungeonGame;

class Program
{
    public static readonly int Seed = CreatePseudoRandomSeed();
    public static Player Player;
    private static Dungeon _dungeon;

    public static void Main()
    {
        Player = new Player();
        
        _dungeon = new Dungeon();
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