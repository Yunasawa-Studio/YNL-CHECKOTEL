using Newtonsoft.Json;
using System;

namespace YNL.Checkotel
{
    public enum UIDType : byte
    { 
        Account, Hotel
    }

    [System.Serializable]
    public struct UID
    {
        public static uint SUID;

        public uint _id;

        [JsonConstructor]
        public UID(uint id)
        {
            _id = id;
        }
        public UID(string id)
        {
            _id = Parse(id);
        }

        public static implicit operator uint(UID id) => id._id;
        public static implicit operator UID(uint id) => new(id);
        public static implicit operator UID(int id) => (uint)id;

        public static UID Parse(string id) => new(uint.Parse(id));
        public static bool TryParse(string id, out UID result)
        {
            if (uint.TryParse(id, out uint value))
            {
                result = value;
                return true;
            }

            throw new FormatException($"Invalid UID format: {id}");
        }
        public static UID Create(UIDType type) => new((uint)type * 10000000 + SUID++);

        public override string ToString() => $"{_id.ToString("D5")}";
        public override bool Equals(object obj)
        {
            if (obj is UID other)
            {
                return _id == other._id;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
    }
}
