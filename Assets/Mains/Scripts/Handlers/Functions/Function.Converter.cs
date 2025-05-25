using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace YNL.Checkotel
{
    public static partial class Function
    {
        public static string ToRank(this ReviewRating rating)
        {
            if (rating >= 0 && rating < 1) return "Disappointing";
            else if (rating >= 1 && rating < 2) return "Moderate";
            else if (rating >= 2 && rating < 3) return "Adequate";
            else if (rating >= 3 && rating < 4) return "Impressive";
            else if (rating >= 4 && rating < 5) return "Exceptional";
            return "";
        }

        public static string ToSentenceCase<T>(this T input)
        {
            string spaced = Regex.Replace(input.ToString(), "(?<!^)([A-Z](?=[a-z])|(?<=[a-z])[A-Z])", " $1");

            string[] words = spaced.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (!Regex.IsMatch(words[i], @"^[A-Z]+$"))
                {
                    words[i] = words[i].ToLower();
                }
            }

            TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
            words[0] = textInfo.ToTitleCase(words[0]);

            return string.Join(" ", words);
        }

        public static string ToDateFormat(this byte number)
        {
            if (number <= 0) return number.ToString();

            int lastDigit = number % 10;
            int lastTwoDigits = number % 100;

            string suffix = (lastTwoDigits >= 11 && lastTwoDigits <= 13) ? "th"
                          : (lastDigit == 1) ? "st"
                          : (lastDigit == 2) ? "nd"
                          : (lastDigit == 3) ? "rd"
                          : "th";

            return number + suffix;
        }

        public static HotelFacility ToHotelFacilities(this string input)
        {
            HotelFacility result = HotelFacility.None;

            foreach (string part in input.Split(';'))
            {
                string trimmed = part.Trim();
                if (Enum.TryParse<HotelFacility>(trimmed, ignoreCase: true, out var facility))
                {
                    result |= facility;
                }
                else
                {
                    Console.WriteLine($"Warning: Unknown facility '{trimmed}'");
                }
            }

            return result;
        }
        
        public static HotelFacility[] ToArray(this HotelFacility facilities)
        {
            List<HotelFacility> facilityArray = new List<HotelFacility>();

            foreach (HotelFacility facility in Enum.GetValues(typeof(HotelFacility)))
            {
                if (facility != HotelFacility.None && facilities.HasFlag(facility))
                {
                    facilityArray.Add(facility);
                }
            }

            return facilityArray.ToArray();
        }
    
        public static List<UID> ToRooms(this string input)
        {
            List<UID> results = new();

            foreach (string part in input.Split(';'))
            {
                string trimmed = part.Trim();

                if (UID.TryParse(trimmed, out var uid))
                {
                    results.Add(30000000 + uid);
                }
                else
                {
                    Console.WriteLine($"Warning: Unknown room '{trimmed}'");
                }
            }

            return results;
        }

        public static Room.BedType ToBedType(this string input)
        {
            string[] parts = input.Split(';');

            var bedType = new Room.BedType();

            Enum.TryParse(parts[0].Trim(), out Room.BedSize size);
            bedType.Size = size == 0 ? 0 : size;

            Enum.TryParse(parts[1].Trim(), out Room.BedDesign design);
            bedType.Design = design == 0 ? 0 : design;

            Enum.TryParse(parts[2].Trim(), out Room.BedStyle style);
            bedType.Style = style == 0 ? 0 : style;

            Enum.TryParse(parts[3].Trim(), out Room.BedKid kidType);
            bedType.KidType = kidType == 0 ? 0 : kidType;

            Enum.TryParse(parts[4].Trim(), out Room.BedFrame frame);
            bedType.Frame = frame == 0 ? 0 : frame;

            return bedType;
        }

        public static RoomFacility ToRoomFacilities(this string input)
        {
            RoomFacility result = RoomFacility.None;

            foreach (string part in input.Split(';'))
            {
                string trimmed = part.Trim();
                if (Enum.TryParse<RoomFacility>(trimmed, ignoreCase: true, out var facility))
                {
                    result |= facility;
                }
                else
                {
                    Console.WriteLine($"Warning: Unknown facility '{trimmed}'");
                }
            }

            return result;
        }

        public static TimeRange ToTimeRange(this string input)
        {
            var parts = input.Split(';');

            return new(byte.Parse(parts[0].Trim()), byte.Parse(parts[1].Trim()));
        }
    }
}