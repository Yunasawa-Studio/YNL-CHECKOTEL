using System;
using UnityEngine.UIElements;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public enum FilterSelectionType : byte
    {
        ReviewScore, Cleanliness, HotelType
    }

    public enum FilterPropertyType : byte
    {
        ScoreG45, ScoreG40, ScoreG35, CleanE45, CleanG40, CleanG35, FlashSale, Hot, New, Stamp, Discount, Coupon
    }

    public class FilterPropertyButtonUI : VisualElement
    {
        public static Action<FilterSelectionType, FilterPropertyType> OnSelected { get; set; }

        private const string _rootClass = "filter-property-button";
        private const string _labelClass = _rootClass + "__label";
        private const string _iconClass = _rootClass + "__icon";
        private const string _selectedClass = "selected";

        private Label _label;
        private VisualElement _icon;

        private FilterSelectionType _selectionType;
        private FilterPropertyType _propertyType;
        private bool _isSelected = false;

        private Action<FilterSelectionType, FilterPropertyType> _onSelected;

        public FilterPropertyButtonUI(string label, FilterSelectionType selection, FilterPropertyType property, Action<FilterSelectionType, FilterPropertyType> onSelected)
        {
            _selectionType = selection;
            _propertyType = property;
            _onSelected = onSelected;

            this.AddStyle(Main.Resources.Styles["StyleVariableUI"]);
            this.AddStyle(Main.Resources.Styles["FilterPropertyButtonUI"]);
            this.AddClass(_rootClass);
            this.RegisterCallback<PointerUpEvent>(OnClicked__Button);

            _label = new(label);
            _label.AddClass(_labelClass);
            this.AddElements(_label);

            _icon = new();
            _icon.AddClass(_iconClass);
            if (_selectionType != FilterSelectionType.HotelType)
            {
                this.AddElements(_icon);
            }

            OnSelected += UpdateUI;
        }
        ~FilterPropertyButtonUI()
        {
            OnSelected -= UpdateUI;
        }

        public void OnClicked__Button(PointerUpEvent evt = null)
        {
            if (_isSelected) return;

            OnSelected?.Invoke(_selectionType, _propertyType);
            _onSelected?.Invoke(_selectionType, _propertyType);

            _isSelected = true;

            this.EnableClass(true, _selectedClass);
            _label.EnableClass(true, _selectedClass);
        }

        private void UpdateUI(FilterSelectionType select, FilterPropertyType property)
        {
            if (_selectionType != select) return;
            if (_propertyType == property) return;

            _isSelected = false;

            this.EnableClass(false, _selectedClass);
            _label.EnableClass(false, _selectedClass);
        }
    }
}