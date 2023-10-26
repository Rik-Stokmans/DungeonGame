namespace DungeonGame;

public class Player : Entity
{
    
    public Player(Map.Coord position)
    {
        Speed = 0;
        Health = 10;
        Level = 0;
        Armor = new Armor();
        Position = position;
    }
}