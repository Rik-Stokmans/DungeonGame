namespace DungeonGame;

public class Player : Entity
{
    public Player(Location playerSpawn, Location relativeLocation)
    {
        TileLocation = playerSpawn;
        RelativeLocation = relativeLocation;
        Speed = 0;
        Health = 0;
        Level = 0;
        Armor = new Armor();
    }
}