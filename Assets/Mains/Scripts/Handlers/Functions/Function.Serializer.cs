using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using YNL.Utilities.Extensions;

namespace YNL.Checkotel
{
    public static partial class Function
    {
        public static string[] SplitCSV(this string csvData)
        {
            var pattern = @"(?:^|,)(?:""([^""]*)""|([^,]*))";
            var matches = Regex.Matches(csvData, pattern);

            List<string> fields = new List<string>();

            foreach (Match match in matches)
            {
                string value = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
                fields.Add(value);
            }

            return fields.ToArray();
        }

        public static void SerializeHotelDatabase(this DatabaseContainerSO _database, string[] fields)
        {
            var uid = UID.Parse(fields[0]);
            var unit = new HotelUnit();

            _database.Hotels[uid] = unit;

            unit.Description.Name = fields[1];
            unit.Description.Address = fields[2];
            unit.Description.Phone = fields[3];
            unit.Description.Email = fields[4];
            unit.Description.Description = fields[5];
            unit.Status.ParticipationDate = new(fields[6]);
            unit.Description.Facilities = fields[7].ToHotelFacilities();
            unit.Description.Policy = fields[8];
            unit.Description.Cancellation = fields[9];

            var reviews = fields[10].Split(';');
            foreach (var review in reviews)
            {
                var feedbackID = 20000000 + uint.Parse(review);
                unit.Review.Feedbacks.Add(feedbackID, new() { FeedbackID = feedbackID , Like = (uint)UnityEngine.Random.Range(200, 1000)});
            }

            if (uint.TryParse(fields[11], out uint result))
            {
                unit.Status.ReservationAmount = result;
            }
            else
            {
                unit.Status.ReservationAmount = 0;
            }
            unit.Rooms = fields[12].ToRooms();
            unit.Description.ImageURL = DataManager.HotelImageURL.Replace("@", fields[0]);
        }

        public static void SerializeRoomDatabase(this DatabaseContainerSO database, string[] fields)
        {
            var uid = UID.Parse(fields[0]);
            var unit = new RoomUnit();

            database.Rooms[uid] = unit;

            unit.Name = fields[1];
            unit.Description.Description = fields[2];
            unit.Description.Property.MaxOccupancy = ushort.Parse(fields[3]);
            unit.Description.Property.NumberOfBeds = ushort.Parse(fields[4]);
            unit.Description.Property.BedType = fields[5].ToBedType();
            unit.Description.Property.ViewType = Enum.Parse<Room.ViewType>(fields[6]);
            unit.Description.Facility = fields[7].ToRoomFacilities();
            unit.Description.Restriction.StayType = Enum.Parse<Room.StayType>(fields[8]);
            unit.Description.Restriction.ValidTime = fields[9].ToTimeRange();
            unit.Description.Restriction.ValidStay = fields[10].ToTimeRange();
            unit.Price.BasePrice = float.Parse(fields[11]);
            unit.RoomAmount = byte.Parse(fields[12]);
            unit.Description.ImageURL = DataManager.RoomImageURL.Replace("@", fields[0]);
        }

        public static void SerializeFeedbackDatabase(this DatabaseContainerSO _database, string[] fields)
        {
            var uid = UID.Parse(fields[0]);

            var feedback = new ReviewFeedback();

            _database.Feedbacks[uid] = feedback;

            feedback.CustomerID = UID.Parse(fields[1]);
            feedback.Cleanliness = ReviewRating.Parse(fields[2]);
            feedback.Facilities = ReviewRating.Parse(fields[3]);
            feedback.Service = ReviewRating.Parse(fields[4]);
            feedback.Comment = fields[5];
        }
    }
}