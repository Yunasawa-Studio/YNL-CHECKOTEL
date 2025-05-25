using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public partial class InformationViewTimeRangePageUI : ViewPageUI
    {
        public Action OnTimeRangeSubmitted { get; set; }

        private VisualElement _background;
        private VisualElement _page;
        private Label _checkInTime;
        private Label _checkOutTime;
        private Label _durationTime;
        private VisualElement _applyButton;

        private Dictionary<Room.StayType, Label> _stayTypeButtons = new();
        private SearchViewTimeRangePageUI.HourlyPage _hourlyPage;

        protected override void Collect()
        {
            _background = Root.Q("ScreenBackground");
            _page = Root.Q("TimePickingPage");

            var resultField = Root.Q("ResultField");

            _checkInTime = resultField.Q("CheckInField").Q("Time") as Label;
            _checkOutTime = resultField.Q("CheckOutField").Q("Time") as Label;
            _durationTime = resultField.Q("DurationField").Q("Time") as Label;

            _applyButton = Root.Q("ToolBar").Q("ApplyButton");
            _applyButton.RegisterCallback<PointerUpEvent>(OnClicked_ApplyButton);

            _hourlyPage = new SearchViewTimeRangePageUI.HourlyPage(Root);
            _hourlyPage.OnTimeRangeChanged = OnTimeRangeChanged;

            foreach (Room.StayType type in Enum.GetValues(typeof(Room.StayType)))
            {
                var button = _page.Q("StayTypeField").Q(type.ToString()) as Label;
                button.RegisterCallback<PointerUpEvent>(evt => OnSelected_StayTypeButton(type));
                _stayTypeButtons.Add(type, button);
            } 
        }

        public override void OnPageOpened(bool isOpen, bool needRefresh = true)
        {
            if (isOpen)
            {
                _background.SetPickingMode(PickingMode.Position);
                _background.SetBackgroundColor(new Color(0.0865f, 0.0865f, 0.0865f, 0.725f));
                _page.SetTranslate(0, 0, true);
            }
            else
            {
                _background.SetBackgroundColor(Color.clear);
                _background.SetPickingMode(PickingMode.Ignore);
                _page.SetTranslate(0, 100, true);
            }

            if (isOpen && needRefresh) Refresh();
        }

        private void OnClicked_ApplyButton(PointerUpEvent evt)
        {
            OnPageOpened(false);
            OnTimeRangeSubmitted?.Invoke();
        }

        private void OnSelected_StayTypeButton(Room.StayType type)
        {
            Main.Runtime.Data.StayType = type;

            foreach (var pair in _stayTypeButtons)
            {
                if (pair.Key == Main.Runtime.Data.StayType)
                {
                    pair.Value.SetColor("#FED1A7");
                    pair.Value.SetBorderColor("#FED1A7");
                }
                else
                {
                    pair.Value.SetColor("#FFFFFF");
                    pair.Value.SetBorderColor(Color.clear);
                }
            }
        }

        private void OnTimeRangeChanged(DateTime checkInTime, byte duration)
        {
            var timeRangeText = Main.Runtime.Data.StayType.GetTimeRangeText(checkInTime, duration);

            _checkInTime.SetText(timeRangeText.In);
            _checkOutTime.SetText(timeRangeText.Out);
            _durationTime.SetText(timeRangeText.Duration);

            Main.Runtime.Data.CheckInTime = checkInTime;
            Main.Runtime.Data.Duration = duration;
        }
    }
}