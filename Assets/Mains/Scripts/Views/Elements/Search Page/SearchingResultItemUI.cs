using System;
using UnityEngine.UIElements;
using YNL.Utilities.Addons;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public partial class SearchingResultItemUI
    {
        public class HotTagUI : VisualElement
        {
            private const string _elementClass = _rootClass + "__hot-tag";
            private const string _iconClass = _elementClass + "-icon";
            private const string _textClass = _elementClass + "-text";

            private VisualElement _icon;
            private VisualElement _text;

            public HotTagUI()
            {
                this.AddClass(_elementClass);

                _icon = new VisualElement().AddClass(_iconClass);
                _text = new Label("Hot").AddClass(_textClass);
                this.AddElements(_icon, _text);
            }
        }
    
        public class NameFieldUI : VisualElement
        {
            private const string _elementClass = _rootClass + "__name-field";
            private const string _nameTextClass = _rootClass + "__name-text";
            private const string _ratingFieldClass = _rootClass + "__rating-field";
            private const string _ratingIconClass = _rootClass + "__rating-icon";
            private const string _ratingTextClass = _rootClass + "__rating-text";

            private Label _nameText;
            private VisualElement _ratingField;
            private VisualElement _ratingIcon;
            private Label _ratingText;

            public NameFieldUI()
            {
                this.AddClass(_elementClass);

                _nameText = new Label().AddClass(_nameTextClass);
                this.Add(_nameText);

                _ratingField = new VisualElement().AddClass(_ratingFieldClass);
                this.Add(_ratingField);

                _ratingIcon = new VisualElement().AddClass(_ratingIconClass);
                _ratingField.Add(_ratingIcon);

                _ratingText = new Label().AddClass(_ratingTextClass);
                _ratingField.Add(_ratingText);
            }

            public void Apply(string name, (float score, int amount) rating)
            {
                _nameText.SetText(name);

                string ratingScoreText = rating.score == -1 ? "-" : rating.score.ToString("0.0");

                _ratingText.SetText($"<b>{ratingScoreText}</b> ({rating.amount})");
            }
        }
    
        public class AddressFieldUI : VisualElement
        {
            private const string _elementClass = _rootClass + "__address-field";
            private const string _iconClass = _rootClass + "__address-icon";
            private const string _textClass = _rootClass + "__address-text";

            private VisualElement _icon;
            private Label _text;

            public AddressFieldUI()
            {
                this.AddClass(_elementClass);

                _icon = new VisualElement().AddClass(_iconClass);
                _text = new Label().AddClass(_textClass);
                this.AddElements(_icon, _text);
            }

            public void Apply(string address)
            {
                _text.SetText(address);
            }
        }

        public class PriceFieldUI : VisualElement
        {
            private const string _elementClass = _rootClass + "__price-field";
            private const string _originalFieldClass = _rootClass + "__original-field";
            private const string _originalPriceClass = _rootClass + "__original-price";
            private const string _discountFieldClass = _rootClass + "__discount-field";
            private const string _discountIconClass = _rootClass + "__discount-icon";
            private const string _discountTextClass = _rootClass + "__discount-text";
            private const string _lastPriceClass = _rootClass + "__last-price";

            private VisualElement _originalField;
            private Label _originalPrice;
            private VisualElement _discountField;
            private VisualElement _discountIcon;
            private Label _discountText;
            private Label _lastPrice;

            public PriceFieldUI()
            {
                this.AddClass(_elementClass);

                _originalField = new VisualElement().AddClass(_originalFieldClass);
                this.AddElements(_originalField);

                _originalPrice = new Label().AddClass(_originalPriceClass);
                _originalField.AddElements(_originalPrice);

                _discountField = new VisualElement().AddClass(_discountFieldClass);
                _originalField.AddElements(_discountField);

                _discountIcon = new VisualElement().AddClass(_discountIconClass);
                _discountField.AddElements(_discountIcon);

                _discountText = new Label().AddClass(_discountTextClass);
                _discountField.AddElements(_discountText);

                _lastPrice = new Label().AddClass(_lastPriceClass);
                this.AddElements(_lastPrice);
            }

            public void Apply(float price, Room.StayType type, int discount, byte duration, byte roomAmount)
            {
                _originalPrice.SetDisplay(discount > 0 ? DisplayStyle.Flex : DisplayStyle.None);
                _discountField.SetDisplay(discount > 0 ? DisplayStyle.Flex : DisplayStyle.None);

                _originalPrice.SetText($"Only <s>{price.ToString("0.00")}$</s>");
                _discountText.SetText($"Discount {discount}%");

                var lastPrice = price * (1 - discount / 100f);
                string priceText = $"<b><color=#FED1A7>{lastPrice.ToString("0.00")}$</color></b> <size=35>/ {duration} {type.GetStayTypeUnit(duration)} • <color=#75caff>Only {roomAmount} room left</color></size>";

                _lastPrice.SetText(priceText);
            }
        }
    }

    public partial class SearchingResultItemUI : VisualElement, IRefreshable
    {
        public static Action<UID> OnSelected { get; set; }

        protected const string _rootClass = "searching-result-item";
        protected const string _previewAreaClass = _rootClass + "__preview-area";
        protected const string _infoAreaClass = _rootClass + "__info-area";
        protected const string _spaceFieldClass = _rootClass + "__space-field";

        private VisualElement _previewArea;
        private HotTagUI _hotTag;
        private VisualElement _infoArea;
        private NameFieldUI _nameField;
        private AddressFieldUI _addressField;
        private VisualElement _spaceField;
        private PriceFieldUI _priceField;

        private UID _hotelID;

        public SearchingResultItemUI()
        {
            this.AddStyle(Main.Resources.Styles["StyleVariableUI"]);
            this.AddStyle(Main.Resources.Styles["SearchingResultItemUI"]);
            this.AddClass(_rootClass).SetMarginBottom(50);
            this.RegisterCallback<PointerUpEvent>(OnSelected_ResultItem);

            _previewArea = new VisualElement().AddClass(_previewAreaClass);
            _infoArea = new VisualElement().AddClass(_infoAreaClass);
            this.AddElements(_previewArea, _infoArea);

            _hotTag = new();
            //_previewArea.AddElements(_hotTag);

            _nameField = new();
            _infoArea.AddElements(_nameField);

            _addressField = new();
            _infoArea.AddElements(_addressField);

            _spaceField = new VisualElement().AddClass(_spaceFieldClass);
            _infoArea.AddElements(_spaceField);

            _priceField = new();
            _infoArea.AddElements(_priceField);
        }

        public void Refresh()
        {

        }

        public void Apply(UID id, Room.StayType type)
        {
            if (!Main.IsSystemStarted) return;

            _hotelID = id;

            if (!Main.Database.Hotels.TryGetValue(id, out var unit)) return;

            Extension.Function.ApplyCloudImageAsync(_previewArea, unit.Description.ImageURL);

            _nameField.Apply(unit.Description.Name, (unit.Review.AverageRating, unit.Review.Feedbacks.Count));
            _addressField.Apply(unit.Description.Address);

            var lowestPrice = id.GetLowestPrice(type);

            _priceField.Apply(lowestPrice, type, 0, 1, 5);
        }

        private void OnSelected_ResultItem(PointerUpEvent evt)
        {
            Marker.OnViewPageSwitched?.Invoke(ViewType.InformationViewMainPage, true, false);
            Marker.OnHotelInformationDisplayed?.Invoke(_hotelID, true);
        }
    }
}