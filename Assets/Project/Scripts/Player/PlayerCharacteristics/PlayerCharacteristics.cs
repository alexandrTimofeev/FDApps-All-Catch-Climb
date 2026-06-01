public class PlayerCharacteristics
{
    private PlayerFishingCharacteristics fishingCharacteristics;
    public PlayerCharacteristics()
    {
        fishingCharacteristics = new PlayerFishingCharacteristics();
    }
    public PlayerFishingCharacteristics GetFishingCharacteristics() => fishingCharacteristics;
}
