using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.UIElements;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    using HomePage = SearchViewTimeRangePageUI.HourlyPage;

    public class MonthCalendarViewUI : VisualElement, IInitializable
    {
        public static Action<DateTime> OnSelectedTimeChanged { get; set; }

        private const string _rootClass = "month-calendar-view";
        private const string _monthFieldClass = _rootClass + "__month-field";
        private const string _monthTextClass = _rootClass + "__month-text";
        private const string _previousMonthButtonClass = _rootClass + "__previous-month-button";
        private const string _nextMonthButtonClass = _rootClass + "__next-month-button";
        private const string _isCurrentMonthClass = "is-current-month";

        private VisualElement _monthField;
        private Label _monthText;
        private VisualElement _previousMonthButton;
        private VisualElement _nextMonthButton;

        private List<MonthCalendarRowUI> _cachedRows = new();
        private HomePage _homePage;
        private int _year, _month;
        private bool _isCurrentMonth = true;

        public MonthCalendarViewUI(HomePage homePage)
        {
            MonthCalendarDayUI.OnSelected += UpdateSelectedDate;

            _homePage = homePage;

            Initialize();

            InitializeCalendar();

            UpdateCalendar(_year, _month, true);
        }
        ~MonthCalendarViewUI()
        {
            MonthCalendarDayUI.OnSelected -= UpdateSelectedDate;
        }

        public void Initialize()
        {
            var dateTime = DateTime.Today;
            _year = dateTime.Year;
            _month = dateTime.Month;

            this.AddStyle(Main.Resources.Styles["StyleVariableUI"]);
            this.AddStyle(Main.Resources.Styles["MonthCalendarViewUI"]);
            this.AddClass(_rootClass);

            _monthField = new VisualElement().AddClass(_monthFieldClass);

            _monthText = new Label("October, 2023").AddClass(_monthTextClass);
            _monthField.AddElements(_monthText);

            _previousMonthButton = new VisualElement().AddClass(_previousMonthButtonClass);
            _previousMonthButton.EnableClass(_isCurrentMonth, _isCurrentMonthClass);
            _previousMonthButton.RegisterCallback<PointerUpEvent>(OnClicked_PreviousMonthButton);
            _monthField.AddElements(_previousMonthButton);

            _nextMonthButton = new VisualElement().AddClass(_nextMonthButtonClass);
            _nextMonthButton.RegisterCallback<PointerUpEvent>(OnClicked_NextMonthButton);
            _monthField.AddElements(_nextMonthButton);

            this.AddElements(_monthField);

            this.AddElements(new MonthCalendarRowUI().SetAsWeekday());
        }

        public void MoveCalendarMonth(bool isMoveNext)
        {
            _month += isMoveNext ? 1 : -1;

            if (_month > 12)
            {
                _month = 1;
                _year++;
            }
            else if (_month < 1)
            {
                _month = 12;
                _year--;
            }

            DateTime today = DateTime.Today;
            _isCurrentMonth = (_year == today.Year && _month == today.Month);

            _previousMonthButton.EnableClass(_isCurrentMonth, _isCurrentMonthClass);

            UpdateCalendar(_year, _month);
        }

        private void OnClicked_PreviousMonthButton(PointerUpEvent evt)
        {
            if (_isCurrentMonth) return;

            MoveCalendarMonth(false);
        }

        private void OnClicked_NextMonthButton(PointerUpEvent evt)
        {
            MoveCalendarMonth(true);
        }

        private void UpdateSelectedDate(DateTime date)
        {
            _homePage.SelectedDate = date;

            OnSelectedTimeChanged?.Invoke(date);
        }

        private void InitializeCalendar()
        {
            for (int i = 0; i < 6; i++)
            {
                var row = new MonthCalendarRowUI();
                _cachedRows.Add(row);
                this.AddElements(row);
            }
        }

        private List<(string, bool)> GenerateMonthDays(int year, int month)
        {
            List<(string, bool)> days = new List<(string, bool)>();

            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            int startDayOfWeek = (int)firstDayOfMonth.DayOfWeek;

            int previousMonth = month == 1 ? 12 : month - 1;
            int previousMonthYear = month == 1 ? year - 1 : year;
            int daysInPreviousMonth = DateTime.DaysInMonth(previousMonthYear, previousMonth);

            for (int i = startDayOfWeek; i > 0; i--)
            {
                days.Add(((daysInPreviousMonth - i + 1).ToString(), false));
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                days.Add((day.ToString(), true));
            }

            while (days.Count % 7 != 0)
            {
                days.Add(((days.Count - daysInMonth - startDayOfWeek + 1).ToString(), false));
            }

            return days;
        }

        public void UpdateCalendar(int year, int month, bool isReseted = false)
        {
            _monthText.text = $"{new DateTime(year, month, 1).ToString("MMMM, yyyy", new CultureInfo("en-US"))}";

            var days = GenerateMonthDays(year, month);

            for (byte i = 0; i < _cachedRows.Count; i++)
            {
                var rowItems = new (string, bool)[7];
                for (int j = 0; j < 7; j++)
                {
                    int index = i * 7 + j;
                    rowItems[j] = index < days.Count ? days[index] : ("", false);
                }

                _cachedRows[i].ResetUI();
                _cachedRows[i].UpdateRowItems(rowItems, _year, _month, isReseted, _homePage.SelectedDate);
            }
        }
    }
}