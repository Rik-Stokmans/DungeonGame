namespace DungeonGame;

public class Entity
{
    public bool isAlive = true;
    public int ID;
    public double Speed;
    public int Health;
    public int Level;
    public Armor Armor;
    public Map.Coord Position;
}

public class Armor
{
    public int ArmorValue { get; set; } = 0;
}