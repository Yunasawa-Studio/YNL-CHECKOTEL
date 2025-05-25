using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace YNL.Checkotel
{
    public class MainViewHomePageUI : ViewPageUI
    {
        private VisualElement _searchField;
        private VisualElement _notificationButton;
        private ScrollView _avalableFilterScroll;
        private ScrollView _typeFilterScroll;
        private ScrollView _pageScroll;

        private List<HotelPreviewListUI> _previewLists = new();

        protected override void Collect()
        {
            _searchField = Root.Q("TopBar").Q("SearchField");
            _searchField.RegisterCallback<PointerUpEvent>(OnClicked__SearchField);

            _notificationButton = Root.Q("TopBar").Q("NotificationButton");
            _notificationButton.RegisterCallback<PointerUpEvent>(OnClicked__NotificationButton);

            _pageScroll = Root.Q("ScrollView") as ScrollView;

            _avalableFilterScroll = _pageScroll.Q("BackgroundContainer").Q("AvalableScroll") as ScrollView;

            _typeFilterScroll = _pageScroll.Q("BackgroundContainer").Q("FilterScroll") as ScrollView;
        }

        protected override void Initialize()
        {
            _avalableFilterScroll.Clear();
            _avalableFilterScroll.Add(new SuggestFilterButtonUI(true, "Heart", "Romantic", SuggestFilterType.Romantic, true));
            _avalableFilterScroll.Add(new SuggestFilterButtonUI(true, "Pool", "With Pool", SuggestFilterType.WithPool));
            _avalableFilterScroll.Add(new SuggestFilterButtonUI(true, "Family", "Homestay", SuggestFilterType.Homestay));

            _typeFilterScroll.Clear();
            _typeFilterScroll.Add(new SuggestFilterButtonUI(false, "Location 1", "<b>Near You</b>\n<size=30>One step to the sky</size>", SuggestFilterType.NearYou, true));
            _typeFilterScroll.Add(new SuggestFilterButtonUI(false, "Hourly", "<b>Hourly</b>\n<size=30>Enjoy every seconds</size>", SuggestFilterType.Hourly));
            _typeFilterScroll.Add(new SuggestFilterButtonUI(false, "Overnight", "<b>Overnight</b>\n<size=30>Comfort like home</size>", SuggestFilterType.Overnight));
            _typeFilterScroll.Add(new SuggestFilterButtonUI(false, "Daily", "<b>Daily</b>\n<size=30>Every day is joyful</size>", SuggestFilterType.Daily));

            _previewLists.Add(new HotelPreviewListUI(PreviewListFilterType.MostPopular));
            _previewLists.Add(new HotelPreviewListUI(PreviewListFilterType.LuxuryStays, true));
            _previewLists.Add(new HotelPreviewListUI(PreviewListFilterType.ExceptionalChoices , true));
            _previewLists.Add(new HotelPreviewListUI(PreviewListFilterType.HighRated));
            _previewLists.Add(new HotelPreviewListUI(PreviewListFilterType.NewHotels, true).SetAsLastItem());
            foreach (var list in _previewLists) _pageScroll.Add(list);
        }

        protected override void Refresh()
        {
            foreach (var list in _previewLists) list.Refresh();
        }

        private void OnClicked__SearchField(PointerUpEvent evt)
        {
            Marker.OnViewPageSwitched?.Invoke(ViewType.SearchViewMainPage, true, false);
        }

        private void OnClicked__NotificationButton(PointerUpEvent evt)
        {
            Marker.OnNotificationViewOpened?.Invoke();
        }
    }
}