using System;
using UnityEngine;
using UnityEngine.UIElements;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public partial class SearchViewTimeRangePageUI : ViewPageUI
    {
        public Action<DateTime, byte> OnTimeRangeSubmitted { get; set; }

        private VisualElement _background;
        private VisualElement _page;
        private VisualElement _closeButton;
        private Label _checkInTime;
        private Label _checkOutTime;
        private Label _durationTime;
        private VisualElement _cancelButton;
        private VisualElement _applyButton;

        private HourlyPage _hourlyPage;

        protected override void Collect()
        {
            _background = Root.Q("ScreenBackground");
            _background.RegisterCallback<PointerUpEvent>(OnClicked_CloseButton);
            _page = Root.Q("TimePickingPage");

            _closeButton = Root.Q("LabelField");
            _closeButton.RegisterCallback<PointerUpEvent>(OnClicked_CloseButton);

            var resultField = Root.Q("ResultField");

            _checkInTime = resultField.Q("CheckInField").Q("Time") as Label;
            _checkOutTime = resultField.Q("CheckOutField").Q("Time") as Label;
            _durationTime = resultField.Q("DurationField").Q("Time") as Label;

            _cancelButton = Root.Q("ToolBar").Q("CancelButton");
            _cancelButton.RegisterCallback<PointerUpEvent>(OnClicked_CancelButton);

            _applyButton = Root.Q("ToolBar").Q("ApplyButton");
            _applyButton.RegisterCallback<PointerUpEvent>(OnClicked_ApplyButton);

            _hourlyPage = new HourlyPage(Root);
            _hourlyPage.OnTimeRangeChanged = OnTimeRangeChanged;
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

        private void OnClicked_CloseButton(PointerUpEvent evt)
        {
            OnPageOpened(false);
        }

        private void OnClicked_CancelButton(PointerUpEvent evt)
        {
            OnPageOpened(false);
        }

        private void OnClicked_ApplyButton(PointerUpEvent evt)
        {
            OnTimeRangeSubmitted?.Invoke(Main.Runtime.Data.CheckInTime, Main.Runtime.Data.Duration);

            OnPageOpened(false);
        }

        private void OnTimeRangeChanged(DateTime checkInTime, byte duration)
        {
            var timeRangeText = Main.Runtime.Data.StayType.GetTimeRangeText(checkInTime, duration);

            _checkInTime.SetText(timeRangeText.In);
            _checkOutTime.SetText(timeRangeText.Out);
            _durationTime.SetText(timeRangeText.Duration);

            Main.Runtime.Data.CheckInTime = checkInTime;
            Main.Runtime.Data.Duration = duration;

            Main.Runtime.IsSearchTimeApplied = true;
        }
    }
}