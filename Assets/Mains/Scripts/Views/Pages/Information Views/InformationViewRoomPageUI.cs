using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public class InformationViewRoomPageUI : ViewPageUI
    {
        private VisualElement _labelField;
        private VisualElement _timeField;
        private VisualElement _timeIcon;
        private Label _timeText;
        private ListView _itemList;

        private List<UID> _rooms = new();
        private UID _hotelID;

        protected override void VirtualAwake()
        {
            Marker.OnHotelRoomsDisplayed += OnHotelRoomsDisplayed;
        }

        private void OnDestroy()
        {
            Marker.OnHotelRoomsDisplayed -= OnHotelRoomsDisplayed;
        }

        protected override void Collect()
        {
            _labelField = Root.Q("HeaderField").Q("LabelField");
            _labelField.RegisterCallback<PointerUpEvent>(OnClicked_LabelField);

            _timeField = Root.Q("HeaderField").Q("TimeField");
            _timeIcon = _timeField.Q("Icon");
            _timeText = _timeField.Q("Text") as Label;

            var scrollView = Root.Q("ItemScroll") as ScrollView;
            Root.Remove(scrollView);

            _itemList = Root.Q("ItemList") as ListView;
            _itemList.Q("unity-content-container").SetFlexGrow(1);
            _itemList.Q<ScrollView>().verticalScrollerVisibility = ScrollerVisibility.Hidden;
            _itemList.fixedItemHeight = 700;
            _itemList.itemsSource = _rooms;
            _itemList.makeItem = () => new RoomSelectItemUI();
            _itemList.bindItem = (element, index) =>
            {
                var item = element as RoomSelectItemUI;
                item.OnBooked = OnRoomBooked;
                if (index < _rooms.Count)
                {
                    item.Apply(_hotelID, _rooms[index]);
                }
            };
        }

        private void OnClicked_LabelField(PointerUpEvent evt)
        {
            Marker.OnViewPageSwitched?.Invoke(ViewType.InformationViewMainPage, true, false);
        }

        private void RebuildHistoryList()
        {
            _itemList.itemsSource = null;
            _itemList.itemsSource = _rooms;
            _itemList.Rebuild();
        }

        private void OnHotelRoomsDisplayed(UID hotelID)
        {
            var style = Main.Runtime.Data.StayType.GetInformationTimeFieldStyle();

            _timeField.SetBackgroundColor(style.Backbround);
            _timeField.SetBorderColor(style.Border);
            _timeIcon.SetBackgroundImage(style.Icon);

            var unit = Main.Database.Hotels[_hotelID = hotelID];
            _rooms = unit.Rooms.Where(i => Main.Database.Rooms[i].Description.Restriction.StayType == Main.Runtime.Data.StayType).ToList();

            RebuildHistoryList();
        }

        private void OnRoomBooked(UID roomID, bool isBook)
        {
            if (isBook)
            {
                Main.Runtime.Data.BookedRooms[_hotelID].Rooms.Remove(roomID);
            }
            else
            {
                Marker.OnViewPageSwitched?.Invoke(ViewType.InformationViewPaymentPage, true, false);
                Marker.OnPaymentRequested?.Invoke(_hotelID, roomID);
            }
        }
    }
}