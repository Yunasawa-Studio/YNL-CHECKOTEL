using System;
using UnityEngine;
using UnityEngine.UIElements;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public class HomeNavigationButton : VisualElement
    {
        private static Action<ViewType> _onNavigated { get; set; }

        private const string _rootClass = "home-navigation-button";
        private const string _iconClass = _rootClass + "__icon";
        private const string _labelClass = _rootClass + "__label";
        private const string _selected = "selected";

        private VisualElement _icon;
        private Label _label;

        private bool _isSelected;
        private ViewType _type;

        public HomeNavigationButton(Texture2D icon, string label, bool isSelected, ViewType type)
        {
            _isSelected = isSelected;
            _type = type;

            this.AddStyle(Main.Resources.Styles["StyleVariableUI"]);
            this.AddStyle(Main.Resources.Styles["HomeNavigationButtonUI"]);
            this.AddClass(_rootClass);

            _icon = new();
            _icon.AddClass(_iconClass);
            _icon.style.backgroundImage = icon;
            this.AddElements(_icon);

            _label = new();
            _label.AddClass(_labelClass);
            _label.text = label;
            this.AddElements(_label);

            UpdateUI();

            this.RegisterCallback<PointerUpEvent>(OnClicked_Button);

            _onNavigated += RecheckUI;
            Marker.OnViewPageSwitched += OnViewPageSwitched;
        }
        ~HomeNavigationButton()
        {
            _onNavigated -= RecheckUI;
            Marker.OnViewPageSwitched -= OnViewPageSwitched;
        }

        private void UpdateUI()
        {
            this.EnableClass(_isSelected, _selected);
            _icon.EnableClass(_isSelected, _selected);
            _label.EnableClass(_isSelected, _selected);
        }

        private void OnClicked_Button(PointerUpEvent evt)
        {
            _isSelected = true;
            UpdateUI();

            _onNavigated?.Invoke(_type);
            Marker.OnViewPageSwitched?.Invoke(_type, true, true);
        }

        private void OnViewPageSwitched(ViewType type, bool a, bool b)
        {
            _isSelected = type == _type;
            UpdateUI();
        }

        private void RecheckUI(ViewType type)
        {
            if (_type == type) return;

            _isSelected = false;
            UpdateUI();
        }
    }
}