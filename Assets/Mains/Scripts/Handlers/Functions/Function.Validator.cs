using System;
using System.Linq;
using UnityEngine;
using YNL.Utilities.Addons;
using YNL.Utilities.Extensions;

namespace YNL.Checkotel
{
    public static partial class Function
    {
        private static SerializableDictionary<UID, HotelUnit> _hotels => Main.Database.Hotels;

        public static bool FuzzyContains(this string source, string target, int maxDistance = 1)
        {
            if (string.IsNullOrEmpty(target)) return true;

            int targetLength = target.Length;

            for (int i = 0; i <= source.Length - targetLength; i++)
            {
                string substring = source.Substring(i, targetLength);

                if (LevenshteinDistance(substring, target) <= maxDistance)
                {
                    return true;
                }
            }

            return false;

            int LevenshteinDistance(string s, string t)
            {
                if (s == t) return 0;
                if (s.Length == 0) return t.Length;
                if (t.Length == 0) return s.Length;

                int[,] distance = new int[s.Length + 1, t.Length + 1];

                for (int i = 0; i <= s.Length; i++) distance[i, 0] = i;
                for (int j = 0; j <= t.Length; j++) distance[0, j] = j;

                for (int i = 1; i <= s.Length; i++)
                {
                    for (int j = 1; j <= t.Length; j++)
                    {
                        int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                        distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
                    }
                }

                return distance[s.Length, t.Length];
            }
        }

        public static bool IsValidTimeRange(this UID id, Room.StayType type, DateTime checkInTime, byte duration)
        {
            if (checkInTime == DateTime.MinValue || Main.Runtime.IsSearchTimeApplied == false) return true;

            var unit = _hotels[id];

            foreach (var roomID in unit.Rooms)
            {
                var restriction = Main.Database.Rooms[roomID].Description.Restriction;

                if (restriction.StayType != type) continue;

                var (validTime, validStay) = (restriction.ValidTime, restriction.ValidStay);

                bool isValid = type switch
                {
                    Room.StayType.Hourly => checkInTime.Hour >= validTime.TimeIn && checkInTime.Hour < validTime.TimeOut
                                      && duration >= validStay.TimeIn && duration <= validStay.TimeOut,

                    Room.StayType.Overnight => checkInTime.Hour >= validTime.TimeIn
                                          && checkInTime.AddHours(duration).Hour <= validTime.TimeOut
                                          && validStay.TimeIn == 0 && validStay.TimeOut == 0,

                    Room.StayType.Daily => checkInTime.Day >= validTime.TimeIn && checkInTime.Day < validTime.TimeOut
                                      && duration >= validStay.TimeIn && duration <= validStay.TimeOut,

                    _ => false
                };

                if (isValid) return true;
            }

            return false;
        }
        
        public static bool IsValidFilter(this HotelUnit unit, FilterSelectionType selection, FilterPropertyType property)
        {
            if (selection == FilterSelectionType.ReviewScore)
            {
                return property switch
                {
                    FilterPropertyType.ScoreG45 => unit.Review.AverageRating >= 4.5f,
                    FilterPropertyType.ScoreG40 => unit.Review.AverageRating >= 4.0f,
                    FilterPropertyType.ScoreG35 => unit.Review.AverageRating >= 3.5f,
                    _ => false
                };
            }
            else if (selection == FilterSelectionType.Cleanliness)
            {
                return property switch
                {
                    FilterPropertyType.CleanE45 => unit.Review.AverageCleanliness >= 4.5f,
                    FilterPropertyType.CleanG40 => unit.Review.AverageCleanliness >= 4.0f,
                    FilterPropertyType.CleanG35 => unit.Review.AverageCleanliness >= 3.5f,
                    _ => false
                };
            }
            else if (selection == FilterSelectionType.HotelType)
            {
                return true;
            }

            return false;
        }
    }
}
