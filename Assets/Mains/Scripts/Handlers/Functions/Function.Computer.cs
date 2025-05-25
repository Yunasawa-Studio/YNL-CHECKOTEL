using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YNL.Utilities.Extensions;

namespace YNL.Checkotel
{
    public static partial class Function
    {
        public static (string In, string Out) GetCheckingTimeText(this DateTime checkInTime, byte duration)
        {
            string checkInText = checkInTime == DateTime.MinValue ? "Anytime" : $"{checkInTime:HH:mm}, {checkInTime:dd/MM}";

            DateTime checkOutTime = checkInTime.AddHours(duration);
            string checkOutText = checkInTime == DateTime.MinValue ? "Anytime" : $"{checkOutTime:HH:mm}, {checkOutTime:dd/MM}";

            return (checkInText, checkOutText);
        }

        public static (string In, string Out, string Duration) GetTimeRangeText(this Room.StayType type, DateTime checkInTime, byte duration, string format = null)
        {
            string durationText = type switch
            {
                Room.StayType.Hourly => $"{duration} {(duration > 1 ? "hours" : "hour")}",
                Room.StayType.Overnight => $"{duration} night",
                Room.StayType.Daily => $"{duration} {(duration > 1 ? "days" : "day")}",
                _ => ""
            };

            format = format == null ? "dd/MM, HH:mm" : format;

            string checkInText = checkInTime.ToString(format);
            string checkOutText = checkInTime.AddHours(duration).ToString(format);

            return (checkInText, checkOutText, durationText);
        }

        public static List<HotelFacility> GetHotelFacilities(this HotelUnit unit, int max = 5)
        {
            var selectedFacilities = new List<HotelFacility>();
            var facilities = unit.Description.Facilities;

            foreach (HotelFacility facility in Enum.GetValues(typeof(HotelFacility)))
            {
                if (facility == HotelFacility.None) continue;

                if (facilities.HasFlag(facility))
                {
                    selectedFacilities.Add(facility);
                    if (selectedFacilities.Count >= max) break;
                }
            }

            return selectedFacilities;
        }

        public static float GetHighestPrice(this UID id, Room.StayType type)
        {
            var unit = _hotels[id];

            if (unit.HighestPrices.TryGetValue(type, out float price)) return price;

            var validRooms = unit.Rooms.Where(i => Main.Database.Rooms[i].Description.Restriction.StayType == type).ToArray();

            var orderedRooms = validRooms.OrderByDescending(r => Main.Database.Rooms[r].Price.BasePrice).ToArray();
            var highestPrice = orderedRooms.IsEmpty() ? 0 : Main.Database.Rooms[orderedRooms[0]].Price.BasePrice;

            unit.HighestPrices[type] = highestPrice;

            return highestPrice;
        }
        public static float GetHighestPriceAllType(this UID id)
        {
            var unit = _hotels[id];

            if (unit.HighestPrices.Count < 3)
            {
                id.GetHighestPrice(Room.StayType.Hourly);
                id.GetHighestPrice(Room.StayType.Overnight);
                id.GetHighestPrice(Room.StayType.Daily);
            }

            return unit.HighestPrices.Max(i => i.Value);
        }
        public static float GetLowestPrice(this UID id, Room.StayType type)
        {
            var unit = _hotels[id];

            if (unit.LowestPrices.TryGetValue(type, out float price)) return price;

            var validRooms = unit.Rooms.Where(i => Main.Database.Rooms[i].Description.Restriction.StayType == type).ToArray();

            var orderedRooms = validRooms.OrderBy(r => Main.Database.Rooms[r].Price.BasePrice).ToArray();
            var lowestPrice = orderedRooms.IsEmpty() ? 0 : Main.Database.Rooms[orderedRooms[0]].Price.BasePrice;

            unit.LowestPrices[type] = lowestPrice;

            return lowestPrice;
        }
        public static float GetLowestPriceAllType(this UID id)
        {
            var unit = _hotels[id];

            if (unit.LowestPrices.Count < 3)
            {
                id.GetLowestPrice(Room.StayType.Hourly);
                id.GetLowestPrice(Room.StayType.Overnight);
                id.GetLowestPrice(Room.StayType.Daily);
            }

            return unit.LowestPrices.Min(i => i.Value);
        }

        public static string GetStayTypeUnit(this Room.StayType type, int duration, bool isCapital = false)
        {
            string unit = type switch
            {
                Room.StayType.Hourly => isCapital ? "Hour" : "hour",
                Room.StayType.Overnight => isCapital ? "Night" : "night",
                Room.StayType.Daily => isCapital ? "Day" : "day",
                _ => ""
            };

            return duration <= 1 ? unit : $"{unit}s";
        }
        public static (string Name, Texture2D Icon) GetHotelFacilitiesField(this HotelFacility facility)
        {
            var name = facility.ToString().RemoveWord("Has").RemoveWord("Is").ToSentenceCase();
            var icon = Main.Resources.Icons["Settings"];

            if (Main.Resources.Icons.TryGetValue(name, out Texture2D validIcon))
            {
                icon = validIcon;
            }

            return (name, icon);
        }

        public static (TimeRange Hourly, TimeRange Overnight, TimeRange Daily) GetFirstTimeRange(this HotelUnit unit)
        {
            var hourlyTime = new TimeRange();
            var overnightTime = new TimeRange();
            var dailyTime = new TimeRange();

            foreach (var id in unit.Rooms)
            {
                var room = Main.Database.Rooms[id];
                if (room.Description.Restriction.StayType == Room.StayType.Hourly)
                {
                    if (hourlyTime != TimeRange.Zero) continue;
                    hourlyTime = room.Description.Restriction.ValidTime;
                }
                else if (room.Description.Restriction.StayType == Room.StayType.Overnight)
                {
                    if (overnightTime != TimeRange.Zero) continue;
                    overnightTime = room.Description.Restriction.ValidTime;
                }
                else if (room.Description.Restriction.StayType == Room.StayType.Daily)
                {
                    if (dailyTime != TimeRange.Zero) continue;
                    dailyTime = room.Description.Restriction.ValidTime;
                }
            }

            return (hourlyTime, overnightTime, dailyTime);
        }

        public static (Color Backbround, Color Border, Texture2D Icon) GetInformationTimeFieldStyle(this Room.StayType type)
        {
            return type switch
            {
                Room.StayType.Hourly => ("#37322E".ToColor(), "#FED1A7".ToColor(), Main.Resources.Icons["Hourly"]),
                Room.StayType.Overnight => ("#372E37".ToColor(), "#FCA7FE".ToColor(), Main.Resources.Icons["Overnight"]),
                Room.StayType.Daily => ("#2E3735".ToColor(), "#A7FEE9".ToColor(), Main.Resources.Icons["Daily"]),
                _ => (Color.white, Color.white, null)
            };
        }

        public static DateTime GetNextNearestTime(this DateTime now)
        {
            int remainMinutes = 30 - (now.Minute % 30);
            return now.AddMinutes(remainMinutes).Date.AddHours(now.Hour).AddMinutes(remainMinutes);
        }
    }
}