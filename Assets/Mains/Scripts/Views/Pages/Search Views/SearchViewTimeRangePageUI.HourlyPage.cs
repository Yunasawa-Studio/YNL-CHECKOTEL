using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public partial class SearchViewTimeRangePageUI
    {
        public class HourlyPage : ICollectible, IRefreshable
        {
            public int MaxDuration = 15;

            public DateTime SelectedDate = DateTime.Now.AddHours(1);
            public byte CheckInTime;
            public byte Duration;

            public Action<DateTime, byte> OnTimeRangeChanged { get; set; }

            public DateTime CheckInDate => SelectedDate.AddHours(CheckInTime);

            private VisualElement _root;
            private VisualElement _hourlyPage;
            private ScrollView _checkInTimeScroll;
            private ScrollView _durationScroll;
            private VisualElement _calendarView;

            private MonthCalendarViewUI _monthCalendarView;

            private List<TimePointButtonUI> _checkInTimeButtons = new();
            private List<TimePointButtonUI> _durationButtons = new();

            public HourlyPage(VisualElement root)
            {
                MonthCalendarViewUI.OnSelectedTimeChanged += OnSelected_CalendarDate;
                TimePointButtonUI.OnSelected += OnSelected_TimePointButton;

                _root = root;

                Collect();
            }
            ~HourlyPage()
            {
                MonthCalendarViewUI.OnSelectedTimeChanged -= OnSelected_CalendarDate;
                TimePointButtonUI.OnSelected -= OnSelected_TimePointButton;
            }

            public void Collect()
            {
                _hourlyPage = _root.Q("ContentPage").Q("HourlyPage");

                _checkInTimeScroll = _hourlyPage.Q("CheckInTimeView").Q("TimeScroll") as ScrollView;
                _checkInTimeScroll.Clear();

                for (int i = 0; i < 24; i++)
                {
                    var timeButton = new TimePointButtonUI($"{i.ToString("00")}:00", false, true);
                    _checkInTimeScroll.Add(timeButton);
                    _checkInTimeButtons.Add(timeButton);
                }

                _durationScroll = _hourlyPage.Q("DurationView").Q("TimeScroll") as ScrollView;
                _durationScroll.Clear();

                for (int i = 1; i <= 24; i++)
                {
                    string durationText = i == 1 ? "1 hour" : $"{i} hours";
                    var timeButton = new TimePointButtonUI($"{durationText}", false, false);
                    _durationScroll.Add(timeButton);
                    _durationButtons.Add(timeButton);
                }

                _calendarView = _hourlyPage.Q("CalendarView");
                _hourlyPage.Remove(_calendarView);

                _monthCalendarView = new MonthCalendarViewUI(this);
                _hourlyPage.Insert(0, _monthCalendarView);

                Refresh();
            }

            public void Refresh()
            {
                foreach (var button in _checkInTimeButtons)
                {
                    if (button.GetDisplay() == DisplayStyle.Flex)
                    {
                        button.OnClicked_Button(null);
                        break;
                    }
                }

                bool durationSelected = false;

                for (byte i = 0; i < _durationButtons.Count; i++)
                {
                    bool validDuration = i < MaxDuration;

                    _durationButtons[i].SetDisplay(validDuration ? DisplayStyle.Flex : DisplayStyle.None);

                    if (validDuration && !durationSelected)
                    {
                        _durationButtons[i].OnClicked_Button(null);
                        durationSelected = true;
                    }
                }
            }

            private void OnSelected_CalendarDate(DateTime date)
            {
                bool isToday = date.Date == DateTime.Today.Date;

                for (byte i = 0; i < _checkInTimeButtons.Count; i++)
                {
                    bool validButton = !isToday || i > DateTime.Now.Hour;
                    _checkInTimeButtons[i].SetDisplay(validButton ? DisplayStyle.Flex : DisplayStyle.None);
                }

                OnTimeRangeChanged?.Invoke(CheckInDate, Duration);
            }

            private void OnSelected_TimePointButton(bool isCheckInTime, string value)
            {
                if (isCheckInTime)
                {
                    CheckInTime = byte.Parse(value.Split(':')[0]);
                }
                else
                {
                    Duration = byte.Parse(value.Split(' ')[0]);
                }

                OnTimeRangeChanged?.Invoke(CheckInDate, Duration);
            }
        }
    }
}