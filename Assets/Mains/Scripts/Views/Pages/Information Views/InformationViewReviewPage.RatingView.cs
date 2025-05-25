using UnityEngine.UIElements;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public partial class InformationViewReviewPage
    {
        public class RatingView
        {
            private Label _ratingText;
            private Label _rankText;
            private Label _amountText;
            private (VisualElement Bar, Label Text) _cleanlinessScore;
            private (VisualElement Bar, Label Text) _facilitiesScore;
            private (VisualElement Bar, Label Text) _serviceScore;

            public RatingView(VisualElement view)
            {
                var ratingField = view.Q("RatingField");
                _ratingText = ratingField.Q("RatingText") as Label;
                _rankText = ratingField.Q("RankText") as Label;
                _amountText = ratingField.Q("AmountText") as Label;

                var cleanlinessScore = view.Q("ScoreView").Q("CleanlinessScore");
                var facilitiesScore = view.Q("ScoreView").Q("FacilitiesScore");
                var serviceScore = view.Q("ScoreView").Q("ServiceScore");

                _cleanlinessScore = (cleanlinessScore.Q("ScoreLine").Q("LineFill"), cleanlinessScore.Q("ScoreText") as Label);
                _facilitiesScore = (facilitiesScore.Q("ScoreLine").Q("LineFill"), facilitiesScore.Q("ScoreText") as Label);
                _serviceScore = (serviceScore.Q("ScoreLine").Q("LineFill"), serviceScore.Q("ScoreText") as Label);
            }

            public void Apply(HotelReview review)
            {
                _ratingText.SetText(review.AverageRating.ToString("0.0"));
                _rankText.SetText(review.AverageRating.ToRank());
                _amountText.SetText($"{review.FeebackAmount} reviews");

                var averageCleanliness = review.AverageCleanliness;
                _cleanlinessScore.Text.SetText(averageCleanliness.ToString("0.0"));
                _cleanlinessScore.Bar.SetWidth(averageCleanliness / 5 * 100, true);

                var averageFacilities = review.AverageFacilities;
                _facilitiesScore.Text.SetText(averageFacilities.ToString("0.0"));
                _facilitiesScore.Bar.SetWidth(averageFacilities / 5 * 100, true);

                var averageService = review.AverageService;
                _serviceScore.Text.SetText(averageService.ToString("0.0"));
                _serviceScore.Bar.SetWidth(averageService / 5 * 100, true);
            }
        }
    }
}