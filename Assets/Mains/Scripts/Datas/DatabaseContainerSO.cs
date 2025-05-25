using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using YNL.Utilities.Addons;

namespace YNL.Checkotel
{
    [CreateAssetMenu(fileName = "DatabaseContainerSO", menuName = "YNL - Checkotel/DatabaseContainerSO")]
    public class DatabaseContainerSO : ScriptableObject
    {
        public SerializableDictionary<UID, Account> Accounts = new();
        public SerializableDictionary<UID, HotelUnit> Hotels = new();
        public SerializableDictionary<UID, RoomUnit> Rooms = new();
        public SerializableDictionary<UID, ReviewFeedback> Feedbacks = new();
        public List<string> Locations = new();
    }
}