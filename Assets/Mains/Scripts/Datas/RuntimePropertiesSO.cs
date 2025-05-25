using System;
using System.Collections.Generic;
using UnityEngine;
using YNL.Utilities.Addons;

namespace YNL.Checkotel
{
    [System.Serializable]
    public class LikedFeedback
    {
        public List<UID> Feedbacks = new();
    }

    [System.Serializable]
    public class BookedRoom
    {
        public List<UID> Rooms = new();
    }

    [Serializable]
    public class RuntimeData
    {
        public UID AccountID;
        public List<string> SearchingAddressHistory = new();
        public SerializableDictionary<UID, LikedFeedback> LikedFeedbacks = new();
        public List<UID> FavoriteHotels = new();

        public Room.StayType StayType = Room.StayType.Hourly;
        public DateTime CheckInTime;
        public byte Duration = 1;

        public SerializableDictionary<uint, BookedRoom> BookedRooms = new();

        public RuntimeData()
        {
            CheckInTime = CheckInTime.GetNextNearestTime();
        }
    }

    [CreateAssetMenu(fileName = "RuntimePropertiesSO", menuName = "YNL - Checkotel/RuntimePropertiesSO")]
    public class RuntimePropertiesSO : ScriptableObject
    {
        public RuntimeData Data = new();
        public bool IsSearchTimeApplied = false;

        public void Reset()
        {
            Data = new();
        }
    }
}