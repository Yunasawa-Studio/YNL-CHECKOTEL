using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public partial class RoomSelectItemUI
    {
        public class PriceFieldUI : VisualElement
        {
            public Action<bool> OnBooked { get; set; }

            private const string _elementClass = _rootClass + "__price-field";
            private const string _priceAreaClass = _rootClass + "__price-area";
            private const string _bookButtonClass = _rootClass + "__book-button";
            private const string _originalFieldClass = _rootClass + "__original-field";
            private const string _originalPriceClass = _rootClass + "__original-price";
            private const string _discountFieldClass = _rootClass + "__discount-field";
            private const string _discountIconClass = _rootClass + "__discount-icon";
            private const string _discountTextClass = _rootClass + "__discount-text";
            private const string _lastPriceClass = _rootClass + "__last-price";

            private VisualElement _priceArea;
            private VisualElement _originalField;
            private Label _originalPrice;
            private VisualElement _discountField;
            private VisualElement _discountIcon;
            private Label _discountText;
            private Label _lastPrice;
            private Button _bookButton;

            private bool _isBooked;

            public PriceFieldUI()
            {
                this.AddClass(_elementClass);

                _priceArea = new VisualElement().AddClass(_priceAreaClass);
                this.AddElements(_priceArea);

                _originalField = new VisualElement().AddClass(_originalFieldClass);
                _priceArea.AddElements(_originalField);

                _originalPrice = new Label().AddClass(_originalPriceClass);
                _originalField.AddElements(_originalPrice);

                _discountField = new VisualElement().AddClass(_discountFieldClass);
                _originalField.AddElements(_discountField);

                _discountIcon = new VisualElement().AddClass(_discountIconClass);
                _discountField.AddElements(_discountIcon);

                _discountText = new Label().AddClass(_discountTextClass);
                _discountField.AddElements(_discountText);

                _lastPrice = new Label().AddClass(_lastPriceClass);
                _priceArea.AddElements(_lastPrice);

                _bookButton = new Button().SetText("Book").AddClass(_bookButtonClass);
                _bookButton.RegisterCallback<PointerUpEvent>(OnClicked_BookButton);
                this.AddElements(_bookButton);
            }

            public void Apply(float price, Room.StayType type, int discount, byte duration, byte roomAmount, bool isBooked)
            {
                _isBooked = isBooked;

                _originalPrice.SetDisplay(discount > 0 ? DisplayStyle.Flex : DisplayStyle.None);
                _discountField.SetDisplay(discount > 0 ? DisplayStyle.Flex : DisplayStyle.None);

                _originalPrice.SetText($"Only <s>{price.ToString("0.00")}$</s>");
                _discountText.SetText($"Discount {discount}%");

                var lastPrice = price * (1 - discount / 100f);
                string priceText = $"<b><color=#FED1A7>{lastPrice.ToString("0.00")}$</color></b> <size=35>/ {duration} {type.GetStayTypeUnit(duration)} • <color=#75caff>Only {roomAmount} room left</color></size>";

                _lastPrice.SetText(priceText);
                _bookButton.SetText(isBooked ? "Cancel" : "Book");
                _bookButton.SetBackgroundColor(isBooked ? "#FF6462" : "#FED1A7");
            }

            private void OnClicked_BookButton(PointerUpEvent evt)
            {
                OnBooked?.Invoke(_isBooked);
            }
        }
    }

    public partial class RoomSelectItemUI : VisualElement
    {
        public Action<UID, bool> OnBooked { get; set; }

        protected const string _rootClass = "room-select-item";
        protected const string _previewAreaClass = _rootClass + "__preview-area";
        protected const string _nameTextClass = _rootClass + "__name-text";
        protected const string _infoAreaClass = _rootClass + "__info-area";
        protected const string _spaceFieldClass = _rootClass + "__space-field";

        private VisualElement _previewArea;
        private VisualElement _infoArea;
        private Label _nameText;
        private VisualElement _spaceField;
        private PriceFieldUI _priceField;

        private UID _roomID;

        public RoomSelectItemUI()
        {
            this.AddStyle(Main.Resources.Styles["StyleVariableUI"]);
            this.AddStyle(Main.Resources.Styles["RoomSelectItemUI"]);
            this.AddClass(_rootClass);

            _previewArea = new VisualElement().AddClass(_previewAreaClass);
            _infoArea = new VisualElement().AddClass(_infoAreaClass);
            this.AddElements(_previewArea, _infoArea);

            _nameText = new Label().AddClass(_nameTextClass);
            _infoArea.AddElements(_nameText);

            _spaceField = new VisualElement().AddClass(_spaceFieldClass);
            _infoArea.AddElements(_spaceField);

            _priceField = new();
            _priceField.OnBooked = OnRoomBooked;
            _infoArea.AddElements(_priceField);
        }

        public void Apply(UID hotelID, UID roomID)
        {
            var unit = Main.Database.Rooms[_roomID = roomID];

            Extension.Function.ApplyCloudImageAsync(_previewArea, unit.Description.ImageURL);

            _nameText.SetText(unit.Name);

            bool isBooked = false;

            if (Main.Runtime.Data.BookedRooms.TryGetValue(hotelID, out var rooms))
            {
                if (rooms.Rooms.Contains(roomID)) isBooked = true;
            }

            _priceField.Apply(unit.Price.BasePrice, unit.Description.Restriction.StayType, 0, 1, unit.RoomAmount, isBooked);
        }

        private void OnRoomBooked(bool isBook)
        {
            OnBooked?.Invoke(_roomID, isBook);
        }
    }
}