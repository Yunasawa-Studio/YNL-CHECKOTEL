namespace YNL.Checkotel
{
    [System.Serializable]
    public struct TimeRange
    {
        public byte TimeIn;
        public byte TimeOut;

        public static TimeRange Zero => new TimeRange();

        public TimeRange(byte timeIn, byte timeOut)
        {
            TimeIn = timeIn;
            TimeOut = timeOut;
        }

        public static bool operator ==(TimeRange a, TimeRange b) => (a.TimeIn == b.TimeIn) && (a.TimeOut == b.TimeOut);
        public static bool operator !=(TimeRange a, TimeRange b) => (a.TimeIn != b.TimeIn) || (a.TimeOut != b.TimeOut);
    }
}