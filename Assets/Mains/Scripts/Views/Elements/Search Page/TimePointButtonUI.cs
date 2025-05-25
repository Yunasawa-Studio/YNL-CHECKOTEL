using UnityEngine.UIElements;
using System;
using YNL.Utilities.UIToolkits;
namespace YNL.Checkotel
{


    public class TimePointButtonUI : VisualElement
    {
        public static Action<bool, string> OnSelected { get; set; }

        private const string _rootClass = "time-point-button";
        private const string _labelClass = _rootClass + "__label";
        private const string _selected = "selected";

        private Label _label;

        private string _value;
        private bool _isSelected = false;
        private bool _isCheckInTime;

        public TimePointButtonUI(string value, bool isSelected = false, bool isCheckInTime = true)
        {
            _value = value;
            _isSelected = isSelected;
            _isCheckInTime = isCheckInTime;

            this.AddStyle(Main.Resources.Styles["StyleVariableUI"]);
            this.AddStyle(Main.Resources.Styles["TimePointButtonUI"]);
            this.AddClass(_rootClass);
            this.SetWidth(isCheckInTime ? 175 : 225);
            this.RegisterCallback<PointerUpEvent>(OnClicked_Button);

            _label = new(value);
            _label.AddClass(_labelClass);
            this.AddElements(_label);

            UpdateUI();

            OnSelected += RecheckUI;
        }
        ~TimePointButtonUI()
        {
            OnSelected -= RecheckUI;
        }

        public void OnClicked_Button(PointerUpEvent evt)
        {
            OnSelected?.Invoke(_isCheckInTime, _value);

            _isSelected = true;
            UpdateUI();
        }

        private void UpdateUI()
        {
            this.EnableClass(_isSelected, _selected);
            _label.EnableClass(_isSelected, _selected);
        }

        private void RecheckUI(bool isCheckInTime, string value)
        {
            if (_isCheckInTime != isCheckInTime) return;
            if (_value == value) return;

            _isSelected = false;
            UpdateUI();
        }
    }
}