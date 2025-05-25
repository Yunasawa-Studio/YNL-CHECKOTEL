using System;

namespace YNL.Checkotel
{
    [System.Serializable]
    public class RoomUnit
    {
        public string Name = string.Empty;
        public RoomDescription Description = new();
        public RoomPrice Price = new();
        public byte RoomAmount = 0;
    }

    [System.Serializable]
    public class RoomPrice
    {
        public float BasePrice = 0; 
        public uint Discount = 0;   
        public float FinalPrice
        {
            get
            {
                return Math.Max(BasePrice - Discount, 0);
            }
        }
    }
}
