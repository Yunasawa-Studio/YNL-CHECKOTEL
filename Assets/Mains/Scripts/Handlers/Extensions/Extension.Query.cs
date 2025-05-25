using System.Linq;
using System;
using YNL.Utilities.Addons;
using System.Collections.Generic;

namespace YNL.Checkotel
{
    public static partial class Extension
    {
        public static class Query
        {
            private static SerializableDictionary<UID, HotelUnit> _hotels => Main.Database.Hotels;

            public static UID[] GetNewHotelsList()
            {
                return _hotels.Where(i => i.Value.Status.ParticipationDate >= DateTime.Now.AddDays(-10)).Select(p => p.Key).ToArray();
            }
            public static UID[] GetMostPopularList()
            {
                return _hotels.OrderByDescending(u => u.Value.Status.ReservationAmount).Take(10).Select(p => p.Key).ToArray();
            }
            public static UID[] GetHighRatedList()
            {
                return _hotels.OrderByDescending(u => u.Value.Review.AverageRating).Take(10).Select(p => p.Key).ToArray();
            }
            public static UID[] GetLuxuryStaysList()
            {
                return _hotels.OrderByDescending(u => u.Key.GetHighestPriceAllType()).Take(10).Select(p => p.Key).ToArray();
            }
            public static UID[] GetExceptionalChoicesList()
            {
                return _hotels.OrderByDescending(u => u.Value.Review.AverageRating).Take(10).Select(p => p.Key).ToArray();
            }
        
            public static UID[] SearchForHotels(string location)
            {
                Func<KeyValuePair<UID, HotelUnit>, bool> validAddress = u => u.Value.Description.Address.FuzzyContains(location);

                return _hotels.Where(validAddress).Select(p => p.Key).ToArray();
            }
        }
    }
}