using System;
using UnityEngine.UIElements;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public class FilteringSelectionItemUI : VisualElement
    {
        public static Action<HotelFacility> OnSelected { get; set; }

        private const string _rootClass = "toggle-property-item";
        private const string _labelClass = _rootClass + "__label";
        private const string _toggleClass = _rootClass + "__toggle";
        private const string _selected = "selected";

        private Label _label;
        private VisualElement _toggle;

        private bool _isSelected = false;
        private SearchViewFilterPageUI _filterPage;
        private HotelFacility _hotelFacility;

        public FilteringSelectionItemUI(SearchViewFilterPageUI filterPage, HotelFacility type)
        {
            OnSelected += UpdateUI;

            _filterPage = filterPage;
            _hotelFacility = type;

            this.AddStyle(Main.Resources.Styles["StyleVariableUI"]);
            this.AddStyle(Main.Resources.Styles["TogglePropertyItemUI"]);
            this.AddClass(_rootClass);
            this.RegisterCallback<PointerUpEvent>(OnClicked__Toggle);

            _label = new(type.ToSentenceCase());
            _label.AddClass(_labelClass);
            this.AddElements(_label);

            _toggle = new();
            _toggle.AddClass(_toggleClass);
            this.AddElements(_toggle);
        }
        ~FilteringSelectionItemUI()
        {
            OnSelected -= UpdateUI;
        }

        public void SetAsLastItem()
        {
            this.style.borderBottomWidth = 0;
        }

        public void OnClicked__Toggle(PointerUpEvent evt = null)
        {
            _isSelected = !_isSelected;
            _toggle.EnableClass(_isSelected, _selected);

            if (_hotelFacility == HotelFacility.None && _isSelected)
            {
                _filterPage.HotelFacility = _hotelFacility;
            }
            else if (_isSelected)
            {
                _filterPage.HotelFacility |= _hotelFacility;
                _filterPage.HotelFacility &= ~HotelFacility.None;
            }
            else
            {
                _filterPage.HotelFacility &= ~_hotelFacility;    
                if (_filterPage.HotelFacility == 0)
                {
                    _filterPage.HotelFacility = HotelFacility.None;
                }
            }

            OnSelected?.Invoke(_hotelFacility);
        }

        private void UpdateUI(HotelFacility hotelFacility)
        {
            if (!(_isSelected && (_hotelFacility == HotelFacility.None) != (hotelFacility == HotelFacility.None))) return;

            _isSelected = false;
            _toggle.EnableClass(_isSelected, _selected);
        }
    }
}