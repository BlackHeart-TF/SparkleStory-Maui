namespace SparkleServer
{
    public class ClientObject
    {
        public CustomizationOption ItemType { get; set; }
        public long ItemID { get; set; }
        public string? Name { get; set; }
        public string? Path { get; set; }
        public byte[]? Content { get; set; }
    }

    public enum CustomizationOption
    {
        SkinColor,
        Hair,
        ActualFace,
        Hat,
        Top,
        Bottom,
        Shoe,
        Cape,
        Glove,
        FaceAccessory,
        Eye,
        Ear,
        Weapon
    }
}
