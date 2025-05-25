using System.Collections.Generic;
using UnityEngine.UIElements;
using YNL.Utilities.Addons;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public class FeedbackResultItemUI : VisualElement
    {
        private static SerializableDictionary<UID, LikedFeedback> _likedFeedbacks => Main.Runtime.Data.LikedFeedbacks;

        private const string _rootClass = "review-result-item";
        private const string _backgroundClass = _rootClass + "__background";
        private const string _infoFieldClass = _rootClass + "__info-field";
        private const string _accountPictureClass = _rootClass + "__account-picture";
        private const string _accountTextClass = _rootClass + "__account-text";
        private const string _starFieldClass = _rootClass + "__star-field";
        private const string _starIconClass = _rootClass + "__star-icon";
        private const string _commentTextClass = _rootClass + "__comment-text";
        private const string _toolFieldClass = _rootClass + "__tool-field";
        private const string _timeStampClass = _rootClass + "__time-stamp";
        private const string _likeFieldClass = _rootClass + "__like-field";
        private const string _likeAmountClass = _rootClass + "__like-amount";
        private const string _likeIconClass = _rootClass + "__like-icon";

        private VisualElement _accountPicture;
        private Label _accountText;
        private VisualElement _starField;
        private Label _commentText;
        private Label _timeStamp;
        private Label _likeAmount;
        private VisualElement _likeIcon;
        private List<VisualElement> _starIcons = new();

        private UID _hotelID;
        private UID _feedbackID;

        public FeedbackResultItemUI()
        {
            this.AddStyle(Main.Resources.Styles["StyleVariableUI"]);
            this.AddStyle(Main.Resources.Styles["FeedbackResultItemUI"]);
            this.AddClass(_rootClass);

            var background = new VisualElement().AddClass(_backgroundClass);
            this.AddElements(background);

            var infoField = new VisualElement().AddClass(_infoFieldClass);
            this.AddElements(infoField);

            _accountPicture = new VisualElement().AddClass(_accountPictureClass);
            infoField.AddElements(_accountPicture);

            _accountText = new Label().AddClass(_accountTextClass);
            infoField.AddElements(_accountText);

            _starField = new VisualElement().AddClass(_starFieldClass);
            infoField.AddElements(_starField);

            _commentText = new Label().AddClass(_commentTextClass);
            this.AddElements(_commentText);

            var toolField = new VisualElement().AddClass(_toolFieldClass);
            this.AddElements(toolField);

            _timeStamp = new Label().AddClass(_timeStampClass);
            toolField.AddElements(_timeStamp);

            var likeField = new VisualElement().AddClass(_likeFieldClass);
            likeField.RegisterCallback<PointerUpEvent>(OnClicked_LikeField);
            toolField.AddElements(likeField);

            _likeAmount = new Label().AddClass(_likeAmountClass);
            likeField.AddElements(_likeAmount);

            _likeIcon = new VisualElement().AddClass(_likeIconClass);
            likeField.AddElements(_likeIcon);

            Initialize();
        }

        public void Initialize()
        {
            for (byte i = 0; i < 5; i++)
            {
                var starIcon = new VisualElement().AddClass(_starIconClass);

                _starIcons.Add(starIcon);
                _starField.AddElements(starIcon);
            }
        }

        public void Apply(UID hotelID, UID feedbackID)
        {
            _hotelID = hotelID;
            _feedbackID = feedbackID;
            var feedback = Main.Database.Feedbacks[feedbackID];
            var status = Main.Database.Hotels[hotelID].Review.Feedbacks[_feedbackID];

            var account = Main.Database.Accounts[feedback.CustomerID];

            _accountText.SetText($"{account.Name}");//\r\n<size=35><color=#808080>Room 302</color></size>");

            _commentText.SetText(feedback.Comment);
            _timeStamp.SetText(string.Empty);// "Created on 12/05");


            bool isLiked = _likedFeedbacks.TryGetValue(hotelID, out var uids) && uids.Feedbacks.Contains(feedbackID);

            _likeAmount.SetText(status.Like.ToString());
            _likeIcon.SetBackgroundImage(Main.Resources.Icons[isLiked ? "Heart (Filled)" : "Heart"]);
            _likeIcon.SetBackgroundImageTintColor(isLiked ? "#FF4040" : "#FFFFFF");

            UpdateStarField(feedback.AverageRating);
        }

        private void OnClicked_LikeField(PointerUpEvent evt)
        {
            bool isLiked = false;
            var status = Main.Database.Hotels[_hotelID].Review.Feedbacks[_feedbackID];

            if (_likedFeedbacks.TryGetValue(_hotelID, out var uids))
            {
                if (uids.Feedbacks.Contains(_feedbackID))
                {
                    uids.Feedbacks.Remove(_feedbackID);
                    status.Like--;
                    isLiked = false;
                }
                else
                {
                    uids.Feedbacks.Add(_feedbackID);
                    status.Like++;
                    isLiked = true;
                }
            }
            else
            {
                _likedFeedbacks.Add(_hotelID, new() { Feedbacks = new() { _feedbackID } });
                status.Like++;
                isLiked = true;

                Marker.OnRuntimeSavingRequested?.Invoke();
            }

            _likeAmount.SetText(status.Like.ToString());
            _likeIcon.SetBackgroundImage(Main.Resources.Icons[isLiked ? "Heart (Filled)" : "Heart"]);
            _likeIcon.SetBackgroundImageTintColor(isLiked ? "#FF4040" : "#FFFFFF");
        }

        private void UpdateStarField(float rating)
        {
            int fullStarAmount = (int)rating;
            bool hasHalfStar = rating - fullStarAmount >= 0.5f;

            for (byte i = 0; i < _starIcons.Count; i++)
            {
                if (i < fullStarAmount)
                {
                    _starIcons[i].SetBackgroundImage(Main.Resources.Icons["Star (Filled)"]);
                }
                else if (i == fullStarAmount && hasHalfStar)
                {
                    _starIcons[i].SetBackgroundImage(Main.Resources.Icons["Star (Half)"]);
                }
                else
                {
                    _starIcons[i].SetBackgroundImage(Main.Resources.Icons["Star"]);
                }
            }
        }
    }
}