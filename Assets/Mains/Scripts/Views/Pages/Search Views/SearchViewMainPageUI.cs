using System;
using UnityEngine;
using UnityEngine.UIElements;
using YNL.Utilities.Extensions;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public class SearchViewMainPageUI : ViewPageUI
    {
        [SerializeField] private SearchViewAddressPageUI _addressPageUI;
        [SerializeField] private SearchViewTimeRangePageUI _timeRangePageUI;

        private VisualElement _closeButton;
        private VisualElement _addressButton;
        private Label _addressText;
        private VisualElement _stayTypeField;
        private VisualElement _roomTypeField;
        private VisualElement _timeRangeButton;
        private Label _checkInTime;
        private Label _checkOutTime;
        private VisualElement _searchButton;

        private string _addressValue;
        private (Room.StayType stayType, Room.RoomType roomType) _searchingType = (Room.StayType.Hourly, Room.RoomType.Standard);

        protected override void VirtualAwake()
        {
            Marker.OnAddressSearchSubmitted += OnAddressSearchSubmitted;
        }

        private void OnDestroy()
        {
            Marker.OnAddressSearchSubmitted -= OnAddressSearchSubmitted;
        }

        protected override void Collect()
        {
            _closeButton = Root.Q("LabelField");
            _closeButton.RegisterCallback<PointerUpEvent>(OnClicked_CloseButton);

            _addressButton = Root.Q("SearchField");
            _addressButton.RegisterCallback<PointerUpEvent>(OnClicked_AddressButton);

            _addressText = _addressButton.Q("Label") as Label;

            _stayTypeField = Root.Q("StayTypeField").Q("TimeField");

            _roomTypeField = Root.Q("RoomTypeField").Q("TypeField");

            var timeRangeField = Root.Q("TimeRangeField");

            _timeRangeButton = timeRangeField.Q("TimeField");
            _timeRangeButton.RegisterCallback<PointerUpEvent>(OnClicked_TimeRangeButton);

            _checkInTime = _timeRangeButton.Q("CheckInField").Q("Time") as Label;
            _checkOutTime = _timeRangeButton.Q("CheckOutField").Q("Time") as Label;

            _searchButton = Root.Q("SearchButton");
            _searchButton.RegisterCallback<PointerUpEvent>(OnClicked_SearchButton);

            _timeRangePageUI.OnTimeRangeSubmitted = OnTimeRangeSubmitted;
        }

        protected override void Initialize()
        {
            _stayTypeField.Clear();
            _stayTypeField.Add(new SearchingSelectTypeUI("Hourly", OnSearchingTypeSelected, true).SetStayType(Room.StayType.Hourly));
            _stayTypeField.Add(new SearchingSelectTypeUI("Overnight", OnSearchingTypeSelected, false).SetStayType(Room.StayType.Overnight));
            _stayTypeField.Add(new SearchingSelectTypeUI("Daily", OnSearchingTypeSelected, false).SetStayType(Room.StayType.Daily));

            _roomTypeField.Clear();
            _roomTypeField.Add(new SearchingSelectTypeUI("Standard", OnSearchingTypeSelected, true).SetRoomType(Room.RoomType.Standard));
            _roomTypeField.Add(new SearchingSelectTypeUI("Family", OnSearchingTypeSelected, false).SetRoomType(Room.RoomType.Family));
            _roomTypeField.Add(new SearchingSelectTypeUI("Business", OnSearchingTypeSelected, false).SetRoomType(Room.RoomType.Business));
        }

        private void OnClicked_CloseButton(PointerUpEvent evt)
        {
            Marker.OnViewPageSwitched?.Invoke(ViewType.MainViewHomePage, true, true);
        }

        private void OnClicked_AddressButton(PointerUpEvent evt)
        {
            _addressPageUI.OnPageOpened(true, false);
        }

        private void OnClicked_TimeRangeButton(PointerUpEvent evt)
        {
            _timeRangePageUI.OnPageOpened(true, false);
        }

        private void OnClicked_SearchButton(PointerUpEvent evt)
        {
            Marker.OnViewPageSwitched?.Invoke(ViewType.SearchViewResultPage, true, true);

            Marker.OnSearchingResultRequested?.Invoke(_addressValue, _searchingType.roomType);
        }

        private void OnSearchingTypeSelected(bool isStayType, Room.StayType stayType, Room.RoomType roomType)
        {
            if (isStayType)
            {
                _searchingType.stayType = stayType;
                Main.Runtime.Data.StayType = stayType;
            }
            else
            {
                _searchingType.roomType = roomType;
            }
        }

        private void OnAddressSearchSubmitted(string value)
        {
            _addressValue = value;
            _addressText.SetText(value);
        }

        private void OnTimeRangeSubmitted(DateTime checkInTime, byte duration)
        {
            _checkInTime.text = checkInTime.ToString("dd/MM, HH:mm");
            _checkOutTime.text = checkInTime.AddHours(duration).ToString("dd/MM, HH:mm");;
        }
    }
}