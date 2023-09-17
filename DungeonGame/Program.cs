namespace DungeonGame;

class Programm
{
    public static readonly int Seed = CreatePseudoRandomSeed();
    
    public static void Main()
    {
        //create a new map
        var map = new Map(20, 20, 60);
        
        //print the map //TODO
        map.PrintMap();
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