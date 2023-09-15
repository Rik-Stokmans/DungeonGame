namespace DungeonGame;

class Programm
{
    public static int seed = DateTime.Now.ToString("MM/dd/yyyy").GetHashCode();
    
    public static void Main()
    {
        Map map = new Map(100, 100, 52);
        
        //map.printBoard();
    }
}