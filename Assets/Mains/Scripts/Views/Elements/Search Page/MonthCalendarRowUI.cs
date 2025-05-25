using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public class MonthCalendarRowUI : VisualElement
    {        
        public bool IsRowItemSelected = false;

        private const string _rootClass = "month-calendar-row";

        private List<MonthCalendarDayUI> _cachedDays = new();

        public MonthCalendarRowUI()
        {
            MonthCalendarDayUI.OnSelected += UpdateUI;

            this.AddStyle(Main.Resources.Styles["StyleVariableUI"]);
            this.AddStyle(Main.Resources.Styles["MonthCalendarRowUI"]);
            this.AddClass(_rootClass);

            InitializeUI();
        }
        ~MonthCalendarRowUI()
        {
            MonthCalendarDayUI.OnSelected -= UpdateUI;
        }

        public void UpdateRowItems((string day, bool isInMonth)[] days, int year, int month, bool isReseted, DateTime selectedDate)
        {
            DateTime now = DateTime.Now;

            for (int i = 0; i < 7; i++)
            {
                _cachedDays[i].UpdateDay(days[i], year, month, isReseted, now, selectedDate);
            }
        }

        public MonthCalendarRowUI SetAsWeekday()
        {
            UpdateRowItems(new (string, bool)[]
            {
                ("SUN", true), ("MON", true), ("TUE", true),
                ("WED", true), ("THU", true), ("FRI", true), ("SAT", true)
            }, 0, 0, false, DateTime.Today);
            this.SetMarginBottom(50).SetFontStyle(FontStyle.Bold);

            return this;
        }

        public void ResetUI()
        {
            UpdateUI(DateTime.Today);
        }

        private void InitializeUI()
        {
            for (int i = 0; i < 7; i++)
            {
                int index = i;

                var dayItem = new MonthCalendarDayUI(this);
                _cachedDays.Add(dayItem);

                this.AddElements(dayItem);
            }
        }

        private void UpdateUI(DateTime date)
        {
            if (!IsRowItemSelected) return;

            for (int i = 0; i < 7; i++)
            {
                _cachedDays[i].Deselect();
            }

            IsRowItemSelected = false;
        }
    }

    public class MonthCalendarDayUI : VisualElement
    {
        public static Action<DateTime> OnSelected { get; set; }
        public DateTime Date;

        private const string _rootClass = "month-calendar-day";
        private const string _textClass = _rootClass + "__text";
        private const string _itemTextNotInMonthClass = "not-in-month";
        private const string _itemTextPassedDayClass = "passed-day";
        private const string _selectedClass = "selected";

        private MonthCalendarRowUI _rowUI;
        private Label _text;
        private bool _isValidDay;

        public MonthCalendarDayUI(MonthCalendarRowUI rowUI)
        {
            _rowUI = rowUI;

            this.AddStyle(Main.Resources.Styles["StyleVariableUI"]);
            this.AddStyle(Main.Resources.Styles["MonthCalendarRowUI"]);
            this.AddClass(_rootClass);

            _text = new Label("").AddClass(_textClass);
            this.AddElements(_text);

            this.RegisterCallback<PointerUpEvent>(OnClicked_DayItem);
        }

        public void UpdateDay((string day, bool isInMonth) day, int year, int month, bool isReseted, DateTime now, DateTime selectedDate)
        {
            _text.text = day.day;
            _text.EnableClass(!day.isInMonth, _itemTextNotInMonthClass);

            now = now.AddHours(1);

            if (int.TryParse(day.day, out int dayValue))
            {
                try
                {
                    Date = new DateTime(year, month, dayValue);
                }
                catch
                {
                    
                }

                if (day.isInMonth)
                {
                    bool isPassedDay = Date.Date < now.Date && year != 0 && month != 0;

                    _text.EnableClass(isPassedDay, _itemTextPassedDayClass);

                    _isValidDay = !isPassedDay;
                }
                else
                {
                    _isValidDay = false;
                }

                if ((isReseted && Date.Date == now.Date) || Date.Date == selectedDate.Date)
                {
                    OnClicked_DayItem(null);
                }
            }
        }

        public void Deselect()
        {
            _text.EnableClass(false, _selectedClass);
        }

        private void OnClicked_DayItem(PointerUpEvent evt)
        {
            if (!_isValidDay) return;

            OnSelected?.Invoke(Date);

            _rowUI.IsRowItemSelected = true;

            _text.EnableClass(true, _selectedClass);
        }
    }
}