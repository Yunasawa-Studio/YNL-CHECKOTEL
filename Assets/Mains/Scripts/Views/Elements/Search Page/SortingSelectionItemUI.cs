using System;
using UnityEngine.UIElements;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public enum SortingSelectionType : byte
    {
        BestMatch, DistanceFromCloseToFar, RatingFromHighToLow, PriceIncreasing, PriceDecreasing,
    }

    public class SortingSelectionItemUI : VisualElement
    {
        public static Action<SortingSelectionType> OnSelected { get; set; }

        private const string _rootClass = "toggle-property-item";
        private const string _labelClass = _rootClass + "__label";
        private const string _toggleClass = _rootClass + "__toggle";
        private const string _selected = "selected";

        private Label _label;
        private VisualElement _toggle;

        private SortingSelectionType _type;
        private bool _isSelected = false;

        public SortingSelectionItemUI(SortingSelectionType type)
        {
            _type = type;

            this.AddStyle(Main.Resources.Styles["StyleVariableUI"]);
            this.AddStyle(Main.Resources.Styles["TogglePropertyItemUI"]);
            this.AddClass(_rootClass);
            this.RegisterCallback<PointerUpEvent>(OnClicked__Toggle);

            _label = new(_type.ToSentenceCase());
            _label.AddClass(_labelClass);
            this.AddElements(_label);

            _toggle = new();
            _toggle.AddClass(_toggleClass);
            this.AddElements(_toggle);

            OnSelected += UpdateUI;
        }
        ~SortingSelectionItemUI()
        {
            OnSelected -= UpdateUI;
        }

        public void SetAsLastItem()
        {
            this.style.borderBottomWidth = 0;
        }

        public void OnClicked__Toggle(PointerUpEvent evt = null)
        {
            if (_isSelected) return;

            OnSelected?.Invoke(_type);

            _isSelected = true;

            _toggle.EnableClass(true, _selected);
        }

        private void UpdateUI(SortingSelectionType type)
        {
            if (_type == type) return;

            _isSelected = false;

            _toggle.EnableClass(false, _selected);
        }
    }
}