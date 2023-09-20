namespace DungeonGame;

public class Entity
{
    private Location _location = new Location();
    private double _speed = 0;
    private int _health = 0;
    private int _level = 0;
    private Armor _armor = new Armor();
    
    public Entity()
    {
         
    }
    
}

internal class Location
{
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
}

internal class Armor
{
    public int _armorValue { get; set; } = 0;


}