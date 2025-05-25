using System;
using UnityEngine;
using UnityEngine.UIElements;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    /// <summary>
    /// Used in <b>Searching Page - Main Page</b> to show the type of searching.
    /// </summary>
    public class SearchingSelectTypeUI : VisualElement
    {
        public delegate void OnSelectedEvent(bool a, Room.StayType b, Room.RoomType c);
        public static Action<bool, Room.StayType, Room.RoomType> OnSelected { get; set; }

        private const string _rootClass = "searching-select-type";
        private const string _iconClass = _rootClass + "__icon";
        private const string _labelClass = _rootClass + "__label";
        private const string _selected = "selected";

        private VisualElement _icon;
        private Label _label;

        private bool _isSelected;
        private bool _isStayType;
        private Room.StayType _stayType;
        private Room.RoomType _roomType;
        private Texture2D _lightIcon;
        private Texture2D _filledIcon;

        private OnSelectedEvent _selectedEvent;

        public SearchingSelectTypeUI(string label, OnSelectedEvent evt, bool isSelected)
        {
            _selectedEvent = evt;
            _isSelected = isSelected;
            _lightIcon = Main.Resources.Icons[label];
            _filledIcon = Main.Resources.Icons[label];

            this.AddStyle(Main.Resources.Styles["StyleVariableUI"]);
            this.AddStyle(Main.Resources.Styles["SearchingSelectTypeUI"]);
            this.AddClass(_rootClass);

            _icon = new();
            _icon.AddClass(_iconClass);
            _icon.style.backgroundImage = _lightIcon;
            this.AddElements(_icon);

            _label = new(label);
            _label.AddClass(_labelClass);
            this.AddElements(_label);

            UpdateUI();

            this.RegisterCallback<PointerUpEvent>(OnClicked_Button);

            OnSelected += RecheckUI;
        }
        ~SearchingSelectTypeUI()
        {
            OnSelected -= RecheckUI;
        }

        public SearchingSelectTypeUI SetStayType(Room.StayType stayType)
        {
            _isStayType = true;
            _stayType = stayType;
            return this;
        }

        public SearchingSelectTypeUI SetRoomType(Room.RoomType roomType)
        {
            _isStayType = false;
            _roomType = roomType;
            return this;
        }

        private void OnClicked_Button(PointerUpEvent evt)
        {
            if (_isSelected) return;

            OnSelected?.Invoke(_isStayType, _stayType, _roomType);
            _selectedEvent(_isStayType, _stayType, _roomType);

            _isSelected = true;

            UpdateUI();
        }

        private void RecheckUI(bool isStayType, Room.StayType stayType, Room.RoomType roomType)
        {
            if (_isStayType != isStayType) return;

            if (isStayType)
            {
                _isSelected = _stayType == stayType;
            }
            else
            {
                _isSelected = _roomType == roomType;
            }

            UpdateUI();
        }

        private void UpdateUI()
        {
            _icon.EnableInClassList(_selected, _isSelected);
            _icon.style.backgroundImage = _isSelected ? _filledIcon : _lightIcon;

            _label.EnableInClassList(_selected, _isSelected);
        }
    }
}