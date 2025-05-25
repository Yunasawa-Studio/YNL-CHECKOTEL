namespace YNL.Checkotel
{
    public partial class Room
    {
        public enum StayType { Hourly, Overnight, Daily }

        public enum RoomType : byte { Standard, Family, Business }

        public enum ViewType
        {
            None,
            City,
            Garden,
            Ocean,
            Mountain,
            Lake,
            Street
        }
    }
}
