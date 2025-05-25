using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.UIElements;
using YNL.Utilities.Addons;
using YNL.Utilities.Extensions;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    using static YNL.Checkotel.Room;
    using HotelList = SerializableDictionary<UID, HotelUnit>;

    public class SearchViewResultPageUI : ViewPageUI
    {
        private static HotelList _hotels => Main.Database.Hotels;

        [SerializeField] private SearchViewSortPageUI _sortPage;
        [SerializeField] private SearchViewFilterPageUI _filterPage;

        private VisualElement _searchBar;
        private Label _searchText;
        private VisualElement _resultScroll;
        private ListView _resultList;
        private VisualElement _sortButton;
        private VisualElement _filterButton;
        private Label _emptyResultLabel;

        private List<UID> _resultIDs = new();
        private List<UID> _cachedResultIDs = new();

        protected override void VirtualAwake()
        {
            Marker.OnSearchingResultRequested += OnSearchingResultRequested;
            Marker.OnFilterResultRequested += OnFilterResultRequested;
            Marker.OnSearchResultFiltered += OnSearchResultFiltered;
            Marker.OnSearchResultSorted += OnSearchResultSorted;
        }

        private void OnDestroy()
        {
            Marker.OnSearchingResultRequested -= OnSearchingResultRequested;
            Marker.OnFilterResultRequested -= OnFilterResultRequested;
            Marker.OnSearchResultFiltered -= OnSearchResultFiltered;
            Marker.OnSearchResultSorted -= OnSearchResultSorted;
        }

        protected override void Collect()
        {
            var resultPage = Root.Q("SearchingResultPage");

            _searchBar = resultPage.Q("SearchBar");
            _searchBar.RegisterCallback<PointerUpEvent>(OnClicked_SearchBar);

            _searchText = _searchBar.Q("SearchField").Q("SearchText") as Label;

            _resultScroll = Root.Q("ResultScroll");
            _resultScroll.SetDisplay(DisplayStyle.None);

            _resultList = new ListView().SetFlexGrow(1).SetMarginTop(50);
            _resultList.Q("unity-content-container").SetFlexGrow(1);
            _resultList.Q<ScrollView>().verticalScrollerVisibility = ScrollerVisibility.Hidden;
            _resultList.fixedItemHeight = 700;
            _resultList.itemsSource = _resultIDs;
            _resultList.makeItem = () => new SearchingResultItemUI();
            _resultList.bindItem = (element, index) =>
            {
                var item = element as SearchingResultItemUI;
                if (index < _resultIDs.Count)
                {
                    item.Apply(_resultIDs[index], Main.Runtime.Data.StayType);
                }
            };
            resultPage.Add(_resultList);

            _sortButton = resultPage.Q("ResultField").Q("SortingButton");
            _sortButton.RegisterCallback<PointerUpEvent>(OnClicked_SortButton);

            _filterButton = resultPage.Q("ResultField").Q("FilteringButton");
            _filterButton.RegisterCallback<PointerUpEvent>(OnClicked_FilterButton);

            _emptyResultLabel = resultPage.Q("EmptyResultLabel") as Label;
        }

        protected override void Refresh()
        {
            RebuildHistoryList();
        }

        private void OnClicked_SearchBar(PointerUpEvent evt)
        {
            Marker.OnViewPageSwitched?.Invoke(ViewType.SearchViewMainPage, true, false);
        }

        private void OnClicked_SortButton(PointerUpEvent evt)
        {
            _sortPage.OnPageOpened(true, true);
        }

        private void OnClicked_FilterButton(PointerUpEvent evt)
        {
            _filterPage.OnPageOpened(true, true);
        }

        private void RebuildHistoryList()
        {
            _resultList.itemsSource = null;
            _resultList.itemsSource = _resultIDs;
            _resultList.Rebuild();
        }

        private void OnSearchingResultRequested(string address, Room.RoomType roomType)
        {
            var timeRangeText = Main.Runtime.Data.CheckInTime.GetCheckingTimeText(Main.Runtime.Data.Duration);
            var addressText = string.IsNullOrEmpty(address) ? "Anywhere" : address;
            _searchText.SetText($"<b>{addressText}</b>\r\n<size=35>{Main.Runtime.Data.StayType} • {timeRangeText.In} - {timeRangeText.Out}</size>");

            _resultIDs.Clear();
            _cachedResultIDs.Clear();

            foreach (var pair in _hotels)
            {
                var unit = pair.Value;
                var matchingArray = new bool[]
                {
                    unit.Description.Address.FuzzyContains(address) || unit.Description.Name.FuzzyContains(address),
                    unit.Rooms.Any(i => Main.Database.Rooms[i].Description.Restriction.StayType == Main.Runtime.Data.StayType),
                    unit.Rooms.Any(i => Main.Database.Rooms[i].Description.Restriction.RoomType == roomType),
                    pair.Key.IsValidTimeRange(Main.Runtime.Data.StayType, Main.Runtime.Data.CheckInTime, Main.Runtime.Data.Duration)
                };

                if (matchingArray.All(i => i == true))
                {
                    _resultIDs.Add(pair.Key);
                    _cachedResultIDs.Add(pair.Key);
                }
            }

            bool emptyResult = _resultIDs.IsEmpty();

            _emptyResultLabel.SetDisplay(emptyResult ? DisplayStyle.Flex : DisplayStyle.None);
            _resultList.SetDisplay(emptyResult ? DisplayStyle.None : DisplayStyle.Flex);

            if (!emptyResult) RebuildHistoryList();
        }

        private void OnFilterResultRequested(Room.StayType stayType)
        {
            _resultIDs.Clear();
            _cachedResultIDs.Clear();

            foreach (var pair in _hotels)
            {
                var unit = pair.Value;

                if (unit.Rooms.Any(unit => Main.Database.Rooms[unit].Description.Restriction.StayType == stayType))
                {
                    _resultIDs.Add(pair.Key);
                    _cachedResultIDs.Add(pair.Key);
                }
            }

            bool emptyResult = _resultIDs.IsEmpty();

            _emptyResultLabel.SetDisplay(emptyResult ? DisplayStyle.Flex : DisplayStyle.None);
            _resultList.SetDisplay(emptyResult ? DisplayStyle.None : DisplayStyle.Flex);

            if (!emptyResult) RebuildHistoryList();
        }

        private void OnSearchResultFiltered(HotelFacility facilities, MRange priceRange, SerializableDictionary<FilterSelectionType, FilterPropertyType> filters)
        {
            _resultIDs.Clear();
            _cachedResultIDs.Clear();

            foreach (var pair in _hotels)
            {
                var unit = pair.Value;
                var matchingArray = new bool[]
                {
                    ValidateFacilities(unit),
                    ValidatePriceRange(unit),
                    ValidateFilter(unit)
                };

                if (matchingArray.All(i => i == true))
                {
                    _resultIDs.Add(pair.Key);
                    _cachedResultIDs.Add(pair.Key);
                }
            }

            bool emptyResult = _resultIDs.IsEmpty();

            _emptyResultLabel.SetDisplay(emptyResult ? DisplayStyle.Flex : DisplayStyle.None);
            _resultList.SetDisplay(emptyResult ? DisplayStyle.None : DisplayStyle.Flex);

            if (!emptyResult) RebuildHistoryList();

            bool ValidateFacilities(HotelUnit unit)
            {
                if (facilities == HotelFacility.None) return true;

                return (unit.Description.Facilities & facilities) == facilities;
            }

            bool ValidatePriceRange(HotelUnit unit)
            {
                bool validLowestPrice = unit.LowestPrices.Any(i => i.Value >= priceRange.Min);
                bool validHighestPrice = unit.HighestPrices.Any(i => i.Value <= priceRange.Max);

                return validLowestPrice && validHighestPrice;
            }

            bool ValidateFilter(HotelUnit unit)
            {
                bool reviewScore = unit.IsValidFilter(FilterSelectionType.ReviewScore, filters[FilterSelectionType.ReviewScore]);
                bool cleanliness = unit.IsValidFilter(FilterSelectionType.Cleanliness, filters[FilterSelectionType.Cleanliness]);

                return reviewScore && cleanliness;
            }
        }

        private void OnSearchResultSorted(SortingSelectionType type)
        {
            if (type == SortingSelectionType.BestMatch)
            {
                _resultIDs = _cachedResultIDs;
            }
            else if (type == SortingSelectionType.DistanceFromCloseToFar)
            {
                _resultIDs = _cachedResultIDs;
            }
            else if (type == SortingSelectionType.RatingFromHighToLow)
            {
                _resultIDs = _cachedResultIDs.OrderByDescending(i => _hotels[i].Review.AverageRating).ToList();
            }
            else if (type == SortingSelectionType.PriceIncreasing)
            {
                _resultIDs = _cachedResultIDs.OrderBy(i => _hotels[i].LowestPrices.Min(i => i.Value)).ToList();
            }
            else if (type == SortingSelectionType.PriceDecreasing)
            {
                _resultIDs = _cachedResultIDs.OrderByDescending(i => _hotels[i].LowestPrices.Min(i => i.Value)).ToList();
            }

            RebuildHistoryList();
        }
    }
}