using Cysharp.Threading.Tasks;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using YNL.Utilities.Extensions;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public partial class HotelPreviewItemUI : VisualElement, IRefreshable
    {
        private const string _rootClass = "hotel-preview-item";
        private const string _imageClass = _rootClass + "__image";
        private const string _infoClass = _rootClass + "__info";
        private const string _infoNameClass = _rootClass + "__info-name";
        private const string _infoLocationClass = _rootClass + "__info-location";
        private const string _infoLocationIconClass = _rootClass + "__info-location-icon";
        private const string _infoLocationTextClass = _rootClass + "__info-location-text";
        private const string _infoPriceClass = _rootClass + "__info-price";
        private const string _infoPriceTextClass = _rootClass + "__info-price-text";
        private const string _infoRatingClass = _rootClass + "__info-rating";
        private const string _infoRatingIconClass = _rootClass + "__info-rating-icon";
        private const string _infoRatingTextClass = _rootClass + "__info-rating-text";
        private const string _infoDiscountClass = _rootClass + "__info-discount";
        private const string _infoDiscountFrameClass = _rootClass + "__info-discount-frame";
        private const string _infoDiscountAmountClass = _rootClass + "__info-discount-amount";
        private const string _infoDiscountTextClass = _rootClass + "__info-discount-text";
        private const string _spaceClass = _rootClass + "__space";
        private const string _miniClass = "mini";

        private VisualElement _previewImage;
        private VisualElement _previewInfo;
        private Label _nameLabel;
        private VisualElement _locationField;
        private VisualElement _locationIcon;
        private Label _locationText;
        private VisualElement _priceField;
        private Label _priceText;
        private VisualElement _ratingField;
        private VisualElement _ratingIcon;
        private Label _ratingText;
        private VisualElement _discountField;
        private VisualElement _discountFrame;
        private Label _discountAmount;
        private Label _discountText;
        private VisualElement _space;

        private UID _id;

        public HotelPreviewItemUI(UID hotelID, bool isMini = false)
        {
            _id = hotelID;

            Initialize(isMini);
            Apply(hotelID);
        }

        private void Initialize(bool isMini)
        {
            this.AddStyle(Main.Resources.Styles["StyleVariableUI"]);
            this.AddStyle(Main.Resources.Styles["HotelPreviewItemUI"]);
            this.AddClass(_rootClass).EnableClass(isMini, _miniClass);
            this.RegisterCallback<PointerUpEvent>(OnClicked_PreviewItem);

            _previewImage = new VisualElement().AddClass(_imageClass);
            this.AddElements(_previewImage);

            _previewInfo = new VisualElement().AddClass(_infoClass);
            this.AddElements(_previewInfo);

            _nameLabel = new Label().AddClass(_infoNameClass);
            _previewInfo.AddElements(_nameLabel);

            _locationField = new VisualElement().AddClass(_infoLocationClass);
            if (!isMini) _previewInfo.AddElements(_locationField);

            _locationIcon = new VisualElement().AddClass(_infoLocationIconClass);
            _locationField.AddElements(_locationIcon);

            _locationText = new Label().AddClass(_infoLocationTextClass);
            _locationField.AddElements(_locationText);

            _space = new VisualElement().AddClass(_spaceClass);
            _previewInfo.AddElements(_space);

            _priceField = new VisualElement().AddClass(_infoPriceClass);
            _previewInfo.AddElements(_priceField);

            _priceText = new Label().AddClass(_infoPriceTextClass);
            _priceField.AddElements(_priceText);

            _ratingField = new VisualElement().AddClass(_infoRatingClass);
            if (isMini) _previewInfo.Insert(1, _ratingField);
            else _priceField.AddElements(_ratingField);

            _ratingIcon = new VisualElement().AddClass(_infoRatingIconClass);
            _ratingField.AddElements(_ratingIcon);

            _ratingText = new Label().AddClass(_infoRatingTextClass);
            _ratingField.AddElements(_ratingText);

            _discountField = new VisualElement().AddClass(_infoDiscountClass);
            _previewInfo.AddElements(_discountField);

            _discountFrame = new VisualElement().AddClass(_infoDiscountFrameClass);
            _discountField.AddElements(_discountFrame);

            _discountAmount = new Label().AddClass(_infoDiscountAmountClass);
            _discountFrame.AddElements(_discountAmount);

            _discountText = new Label().AddClass(_infoDiscountTextClass);
            _discountField.AddElements(_discountText);
        }

        private void Apply(UID id)
        {
            var unit = Main.Database.Hotels[id];

            int discountPercentage = 45;

            Extension.Function.ApplyCloudImageAsync(_previewImage, unit.Description.ImageURL);

            _nameLabel.text = unit.Description.Name;
            _locationText.text = unit.Description.Address;
            var price = unit.Rooms.IsEmpty() ? 0 : Main.Database.Rooms[unit.Rooms[0]].Price.BasePrice;
            _priceText.text = $"<b><color=#FED1A7>${price * (1 - (discountPercentage / 100f))}</color></b> <s>${price}</s> <b><size=30>/ 2 days</size></b>";
            _ratingText.text = $"<b>{Random.Range(3.0f, 5.0f).ToString("F1")}</b> ({Random.Range(100, 100)})";

            if (discountPercentage > 0)
            {
                _discountAmount.text = $"-{discountPercentage}%";
                _discountText.text = $"Only {Random.Range(1, 10)} rooms left";
                _discountField.style.display = DisplayStyle.Flex;
            }
            else
            {
                _discountField.style.display = DisplayStyle.None;
            }
        }

        public void Refresh()
        {
            Apply(_id);
        }

        public HotelPreviewItemUI SetAsLastItem(bool set = true)
        {
            if (!set) return this;

            this.SetMarginRight(50);

            return this;
        }

        private void OnClicked_PreviewItem(PointerUpEvent evt)
        {
            Marker.OnViewPageSwitched?.Invoke(ViewType.InformationViewMainPage, true, false);
            Marker.OnHotelInformationDisplayed?.Invoke(_id, false);
            Marker.OnHotelFacilitiesDisplayed?.Invoke(_id);
        }
    }
}