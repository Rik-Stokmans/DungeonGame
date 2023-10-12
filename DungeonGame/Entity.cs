namespace DungeonGame;

public class Entity
{
    public Location TileLocation;
    public Location RelativeLocation;
    public double Speed = 0;
    public int Health = 0;
    public int Level = 0;
    public Armor Armor;

    public Entity(Location tileLocation, Location relativeLocation, double speed, int health, int level, Armor armor)
    {
        TileLocation = tileLocation;
        RelativeLocation = relativeLocation;
        Speed = speed;
        Health = health;
        Level = level;
        Armor = armor;
    }

    public Entity()
    {
        TileLocation = new Location();
        RelativeLocation = new Location();
        Speed = 0;
        Health = 0;
        Level = 0;
        Armor = new Armor();
    }
}


public class Location
{
    
    public int X { get; set; }
    public int Y { get; set; }
    
    public Location(int x, int y)
    {
        X = x;
        Y = y;
    }
    public Location()
    {
        X = 0;
        Y = 0;
    }
    
    public bool isSameLocation(Location location)
    {
        return X == location.X && Y == location.Y;
    }
    
}

public class Armor
{
    public int ArmorValue { get; set; } = 0;
}