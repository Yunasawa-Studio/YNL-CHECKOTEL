using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public partial class InformationViewMainPage
    {
        public class PriceField
        {
            public Action OnOpenTimeRangePage;

            private VisualElement _timeField;
            private VisualElement _timeIcon;
            private Label _timeText;
            private Label _originalPrice;
            private Label _lastPrice;
            private Button _chooseButton;

            private UID _hotelID;

            public PriceField(VisualElement root)
            {
                var bottomBar = root.Q("BottomBar");

                _timeField = bottomBar.Q("TimeField");
                _timeField.RegisterCallback<PointerUpEvent>(OnClicked_TimeField);

                _timeIcon = _timeField.Q("Icon");

                _timeText = _timeField.Q("Text") as Label;

                var priceArea = bottomBar.Q("PriceArea");

                _originalPrice = priceArea.Q("PriceField").Q("OriginalPrice") as Label;

                _lastPrice = priceArea.Q("PriceField").Q("LastPrice") as Label;

                _chooseButton = priceArea.Q("ChooseButton") as Button;
                _chooseButton.RegisterCallback<PointerUpEvent>(OnClicked_ChooseButton);
            }

            public void Apply(UID hotelID)
            {
                _hotelID = hotelID;

                var style = Main.Runtime.Data.StayType.GetInformationTimeFieldStyle();

                _timeField.SetBackgroundColor(style.Backbround);
                _timeField.SetBorderColor(style.Border);
                _timeIcon.SetBackgroundImage(style.Icon);
                _lastPrice.SetColor(style.Border);
                _chooseButton.SetBackgroundColor(style.Border);

                var timeText = Main.Runtime.Data.StayType.GetTimeRangeText(Main.Runtime.Data.CheckInTime, Main.Runtime.Data.Duration);

                _timeText.SetText($"{timeText.Duration} | {timeText.In} → {timeText.Out}");
            }

            private void OnClicked_TimeField(PointerUpEvent evt)
            {
                OnOpenTimeRangePage?.Invoke();
            }

            private void OnClicked_ChooseButton(PointerUpEvent evt)
            {
                Marker.OnViewPageSwitched?.Invoke(ViewType.InformationViewRoomPage, true, false);
                Marker.OnHotelRoomsDisplayed?.Invoke(_hotelID);
            }
        }

        public class ImageView
        {
            private List<VisualElement> _roomImages = new();
            private VisualElement _fadeItem;
            private Label _amountText;

            public ImageView(VisualElement imageView)
            {
                for (byte i = 0; i < 4; i++) _roomImages.Add(imageView.Q($"Image{i}"));

                _fadeItem = imageView.Q("Image3").Q("Fade");
                _amountText = imageView.Q("Image3").Q("Text") as Label;
            }

            public async UniTaskVoid Apply(HotelUnit unit)
            {
                var images = await unit.GetRoomImageAsync();

                for (byte i = 0; i < 4; i++)
                {
                    if (i < images.Length)
                    {
                        _roomImages[i].SetDisplay(DisplayStyle.Flex);
                        _roomImages[i].SetBackgroundImage(images[i]);

                        if (images.Length > 4)
                        {
                            _fadeItem.SetDisplay(DisplayStyle.Flex);
                            _amountText.SetDisplay(DisplayStyle.Flex);
                            _amountText.SetText($"+{images.Length - 4}");
                        }
                        else
                        {
                            _fadeItem.SetDisplay(DisplayStyle.None);
                            _amountText.SetDisplay(DisplayStyle.None);
                        }
                    }
                    else
                    {
                        _roomImages[i].SetDisplay(DisplayStyle.None);
                    }
                }
            }
        }

        public class NameView
        {
            private VisualElement _badgeField;
            private Label _nameText;
            private Label _addressText;

            public NameView(VisualElement contentContainer)
            {
                _badgeField = contentContainer.Q("BadgeField");
                _nameText = contentContainer.Q("NameText") as Label;
                _addressText = contentContainer.Q("AddressText") as Label;
            }

            public void Apply(string name, string address)
            {
                _nameText.SetText(name);
                _addressText.SetText(address);
            }
        }

        public class ReviewView
        {
            private Label _ratingText;
            private Label _reviewAmount;
            private Label _seeMoreButton;
            private ScrollView _reviewScroll;
            private Label _emptyLabel;
            private VisualElement _ellipsis;

            private List<FeedbackPreviewItemUI> _feedbackItems = new();

            public ReviewView(VisualElement contentContainer)
            {
                var reviewView = contentContainer.Q("ReviewView").AddStyle(Main.Resources.Styles["GlobalStyleUI"]);
                var scoreField = reviewView.Q("ScoreField");

                _ratingText = scoreField.Q("Rating") as Label;
                _reviewAmount = scoreField.Q("Amount") as Label;
                _seeMoreButton = scoreField.Q("SeeMore") as Label;
                _seeMoreButton.RegisterCallback<PointerUpEvent>(OnClicked_SeeMoreButton);

                _emptyLabel = reviewView.Q("EmptyLabel") as Label;

                _reviewScroll = reviewView.Q("ReviewScroll") as ScrollView;
                _reviewScroll.Clear();

                for (byte i = 0; i < 5; i++)
                {
                    var feedbackItem = new FeedbackPreviewItemUI();
                    _feedbackItems.Add(feedbackItem);
                    _reviewScroll.AddElements(feedbackItem);
                }

                _ellipsis = new VisualElement().AddClass("review-view-ellipsis").SetBackgroundImage(Main.Resources.Icons["Ellipsis"]);
                _reviewScroll.AddElements(_ellipsis);
            }

            public void Apply(UID id)
            {
                var unit = Main.Database.Hotels[id];
                var review = unit.Review;
                var feedbacks = unit.Review.Feedbacks.Keys.ToArray();

                _ratingText.SetText(review.AverageRating.ToString("0.0"));
                _reviewAmount.SetText($"({review.FeebackAmount} reviews)");

                bool emptyFeedback = review.FeebackAmount == 0;

                _emptyLabel.SetDisplay(emptyFeedback ? DisplayStyle.Flex : DisplayStyle.None);
                _reviewScroll.SetDisplay(emptyFeedback ? DisplayStyle.None : DisplayStyle.Flex);

                if (!emptyFeedback)
                {
                    for (byte i = 0; i < 5; i++)
                    {
                        if (i < feedbacks.Length)
                        {
                            _feedbackItems[i].SetDisplay(DisplayStyle.Flex);
                            _feedbackItems[i].Apply(id, feedbacks[i]);
                        }
                        else
                        {
                            _feedbackItems[i].SetDisplay(DisplayStyle.None);
                        }
                    }
                }
            }

            private void OnClicked_SeeMoreButton(PointerUpEvent evt)
            {
                Marker.OnViewPageSwitched?.Invoke(ViewType.InformationViewReviewPage, true, false);
            }
        }

        public class FacilityView
        {
            public class FacilityItem : VisualElement
            {
                private VisualElement _icon;
                private Label _text;

                public FacilityItem(VisualElement item)
                {
                    _icon = item.Q("Icon");
                    _text = item.Q("Text") as Label;
                }

                public void Apply(HotelFacility facility)
                {
                    var field = facility.GetHotelFacilitiesField();

                    _icon.SetBackgroundImage(field.Icon);
                    _text.SetText(field.Name);
                }
            }

            private Label _seeMoreButton;
            private VisualElement _facilityField;
            private FacilityItem[] _facilityItems = new FacilityItem[4];

            public FacilityView(VisualElement contentContainer)
            {
                var facilityView = contentContainer.Q("FacilityView");

                _seeMoreButton = facilityView.Q("LabelField").Q("SeeMore") as Label;
                _seeMoreButton.RegisterCallback<PointerUpEvent>(OnClicked_SeeMoreButton);

                _facilityField = facilityView.Q("FacilityField");

                for (byte i = 0; i < _facilityItems.Length; i++)
                {
                    _facilityItems[i] = new(_facilityField.Q($"FacilityItem{i}"));
                }
            }

            public void Apply(HotelUnit unit)
            {
                var facilities = unit.GetHotelFacilities();

                for (byte i = 0; i < _facilityItems.Length; i++)
                {
                    if (i < facilities.Count)
                    {
                        _facilityItems[i].SetDisplay(DisplayStyle.Flex);
                        _facilityItems[i].Apply(facilities[i]);
                    }
                    else
                    {
                        _facilityItems[i].SetDisplay(DisplayStyle.None);
                    }
                }
            }

            private void OnClicked_SeeMoreButton(PointerUpEvent evt)
            {
                Marker.OnViewPageSwitched?.Invoke(ViewType.InformationViewFacilitiesPage, true, false);
            }
        }

        public class DescriptionField
        {
            private Label _descriptionText;

            private bool _isExpanded = false;

            public DescriptionField(VisualElement contentContainer)
            {
                _descriptionText = contentContainer.Q("DescriptionField").Q("DescriptionText") as Label;
                _descriptionText.RegisterCallback<PointerUpEvent>(OnClicked_DescriptionText);
            }

            public void Apply(string description)
            {
                _descriptionText.SetText(description == string.Empty ? "<color=#808080>No description!</color>" : description);
            }

            private void OnClicked_DescriptionText(PointerUpEvent evt)
            {
                _isExpanded = !_isExpanded;

                if (_isExpanded)
                {
                    _descriptionText.SetMaxHeight(StyleKeyword.Auto);
                }
                else
                {
                    _descriptionText.SetMaxHeight(175);
                }
            }
        }

        public class TimeField
        {
            private Label _hourlyTime;
            private Label _overnightTime;
            private Label _dailyTime;

            public TimeField(VisualElement field)
            {
                var tableView = field.Q("TableView");

                _hourlyTime = tableView.Q("HourlyTime").Q("Time") as Label;
                _overnightTime = tableView.Q("OvernightTime").Q("Time") as Label;
                _dailyTime = tableView.Q("DailyTime").Q("Time") as Label;
            }

            public void Apply(HotelUnit unit)
            {
                var timeRange = unit.GetFirstTimeRange();
                var hourly = timeRange.Hourly;
                var overnight = timeRange.Overnight;
                var daily = timeRange.Daily;

                var hourlyText = hourly == TimeRange.Zero ? "-" : $"From {StyleText($"{hourly.TimeIn.ToString("D2")}:00")} to {StyleText($"{hourly.TimeOut.ToString("D2")}:00")} everyday";
                var overnightText = overnight == TimeRange.Zero ? "-" : $"From {StyleText($"{overnight.TimeIn.ToString("D2")}:00")} to {StyleText($"{overnight.TimeIn.ToString("D2")}:00")} the day after";
                var dailyText = daily == TimeRange.Zero ? "-" : $"From day {StyleText(daily.TimeIn.ToDateFormat())} to day {StyleText(daily.TimeOut.ToDateFormat())}";

                _hourlyTime.SetText(hourlyText);
                _overnightTime.SetText(overnightText);
                _dailyTime.SetText(dailyText);

                string StyleText(string input) => $"<color=#FED1A7>{input}</color>";
                //string StyleText(string input) => $"<color=#FED1A7><b>{input}</b></color>";
            }
        }
    
        public class PolicyField
        {
            private Label _policyText;

            private bool _isExpanded = false;

            public PolicyField(VisualElement field)
            {
                _policyText = field.Q("Text") as Label;
                _policyText.RegisterCallback<PointerUpEvent>(OnClicked_DescriptionText);
            }

            public void Apply(HotelUnit unit)
            {
                string policy = unit.Description.Policy;
                _policyText.SetText(policy == string.Empty ? "<color=#808080>No hotel policy!</color>" : policy);
            }

            private void OnClicked_DescriptionText(PointerUpEvent evt)
            {
                _isExpanded = !_isExpanded;

                if (_isExpanded)
                {
                    _policyText.SetMaxHeight(StyleKeyword.Auto);
                }
                else
                {
                    _policyText.SetMaxHeight(175);
                }
            }
        }

        public class CancellationField
        {
            private Label _cancellationText;

            private bool _isExpanded = false;

            public CancellationField(VisualElement field)
            {
                _cancellationText = field.Q("Text") as Label;
                _cancellationText.RegisterCallback<PointerUpEvent>(OnClicked_DescriptionText);
            }

            public void Apply(HotelUnit unit)
            {
                string cancellation = unit.Description.Cancellation;
                _cancellationText.SetText(cancellation == string.Empty ? "<color=#808080>No cancellation policy!</color>" : cancellation);
            }

            private void OnClicked_DescriptionText(PointerUpEvent evt)
            {
                _isExpanded = !_isExpanded;

                if (_isExpanded)
                {
                    _cancellationText.SetMaxHeight(StyleKeyword.Auto);
                }
                else
                {
                    _cancellationText.SetMaxHeight(175);
                }
            }
        }
    }
}