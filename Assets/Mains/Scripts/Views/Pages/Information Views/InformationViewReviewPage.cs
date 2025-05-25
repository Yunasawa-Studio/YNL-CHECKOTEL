using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public partial class InformationViewReviewPage : ViewPageUI
    {
        private VisualElement _backButton;
        private RatingView _ratingView;
        private ScrollView _reviewScroll;
        private ListView _reviewList;

        private UID _hotelID;
        private List<UID> _feedbackIDs = new();

        protected override void VirtualAwake()
        {
            Marker.OnHotelInformationDisplayed += OnHotelInformationDisplayed;
        }

        private void OnDestroy()
        {
            Marker.OnHotelInformationDisplayed -= OnHotelInformationDisplayed;
        }

        protected override void Collect()
        {
            _backButton = Root.Q("TopBar").Q("LabelField");
            _backButton.RegisterCallback<PointerUpEvent>(OnClicked_BackButton);

            _ratingView = new(Root.Q("TopBar").Q("RatingView"));

            _reviewScroll = Root.Q("ContentScroll") as ScrollView;
            _reviewScroll.SetDisplay(DisplayStyle.None);

            _reviewList = Root.Q("ContentList") as ListView;
            _reviewList.Q("unity-content-container").SetFlexGrow(1);
            _reviewList.Q<ScrollView>().verticalScrollerVisibility = ScrollerVisibility.Hidden;
            _reviewList.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            _reviewList.itemsSource = _feedbackIDs;
            _reviewList.makeItem = () => new FeedbackResultItemUI();
            _reviewList.bindItem = (element, index) =>
            {
                var item = element as FeedbackResultItemUI;
                if (index < _feedbackIDs.Count)
                {
                    item.Apply(_hotelID, _feedbackIDs[index]);
                }
            };
        }

        private void OnClicked_BackButton(PointerUpEvent evt)
        {
            Marker.OnViewPageSwitched?.Invoke(ViewType.InformationViewMainPage, true, false);
        }

        private void RebuildHistoryList()
        {
            _reviewList.itemsSource = null;
            _reviewList.itemsSource = _feedbackIDs;
            _reviewList.Rebuild();
        }

        private void OnHotelInformationDisplayed(UID id, bool isSearchResult)
        {
            _hotelID = id;
            var unit = Main.Database.Hotels[id];

            _ratingView.Apply(unit.Review);

            _feedbackIDs = unit.Review.Feedbacks.Keys.ToList();

            bool emptyFeedback = unit.Review.FeebackAmount == 0;

            if (!emptyFeedback) RebuildHistoryList();
        }
    }
}