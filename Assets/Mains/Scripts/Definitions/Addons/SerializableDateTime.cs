using System;
using UnityEngine;

namespace YNL.Checkotel
{
    [Serializable]
    public class SerializableDateTime
    {
        [SerializeField] private string _dateTimeString;

        public DateTime DateTime
        {
            get => DateTime.Parse(_dateTimeString);
            set => _dateTimeString = value.ToString("o");
        }

        public SerializableDateTime(DateTime dateTime)
        {
            DateTime = dateTime;
        }
        public SerializableDateTime(string dateTime)
        {
            _dateTimeString = dateTime;
        }

        public override string ToString() => DateTime.ToString();

        public static implicit operator DateTime(SerializableDateTime serializableDateTime) => serializableDateTime.DateTime;

        public static bool operator >(SerializableDateTime left, DateTime right) => left.DateTime > right;
        public static bool operator <(SerializableDateTime left, DateTime right) => left.DateTime < right;
        public static bool operator >=(SerializableDateTime left, DateTime right) => left.DateTime >= right;
        public static bool operator <=(SerializableDateTime left, DateTime right) => left.DateTime <= right;
    }
}