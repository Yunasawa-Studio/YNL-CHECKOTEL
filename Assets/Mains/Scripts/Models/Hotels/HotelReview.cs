using System;
using System.Globalization;
using System.Linq;
using UnityEngine;
using YNL.Utilities.Addons;

namespace YNL.Checkotel
{
    [System.Serializable]
    public class FeedbackStatus
    {
        public UID FeedbackID;
        public uint Like;
    }

    [System.Serializable]
    public class HotelReview
    {
        public SerializableDictionary<UID, FeedbackStatus> Feedbacks = new();

        public ReviewRating AverageRating => Feedbacks.Count > 0 ? Feedbacks.Sum(f => Main.Database.Feedbacks[f.Key].AverageRating) / Feedbacks.Count : -1;
        public ReviewRating AverageCleanliness => Feedbacks.Count > 0 ? Feedbacks.Sum(f => Main.Database.Feedbacks[f.Key].Cleanliness) / Feedbacks.Count : -1;
        public ReviewRating AverageFacilities => Feedbacks.Count > 0 ? Feedbacks.Sum(f => Main.Database.Feedbacks[f.Key].Facilities) / Feedbacks.Count : -1;
        public ReviewRating AverageService => Feedbacks.Count > 0 ? Feedbacks.Sum(f => Main.Database.Feedbacks[f.Key].Service) / Feedbacks.Count : -1;

        public int FeebackAmount => Feedbacks.Count;
    }

    [System.Serializable]
    public struct ReviewRating : IComparable<ReviewRating>
    {
        [SerializeField] private float _rating;

        public static implicit operator float(ReviewRating rating) => rating._rating;
        public static implicit operator ReviewRating(float rating) => new() { _rating = rating };

        public static ReviewRating Parse(string rating)
        {
            if (float.TryParse(rating, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
            {
                return new ReviewRating { _rating = value };
            }
            throw new FormatException($"Invalid ReviewRating format: {rating}");
        }

        public int CompareTo(ReviewRating other)
        {
            return _rating.CompareTo(other._rating);
        }
        public string ToString(string format = "0.0") => _rating == -1 ? "-" : _rating.ToString(format);
    }

    [System.Serializable]
    public class ReviewFeedback
    {
        public UID CustomerID;
        public ReviewRating Cleanliness = 0;
        public ReviewRating Facilities = 0;
        public ReviewRating Service = 0;
        public string Comment = string.Empty;

        public ReviewRating AverageRating => (Cleanliness + Facilities + Service) / 3;
    }
}
