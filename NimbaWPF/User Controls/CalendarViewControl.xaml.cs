using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

using NimbaWPF.Data;

namespace NimbaWPF.User_Controls
{
    /// <summary>
    /// Interaction logic for CalendarViewControl.xaml
    /// </summary>
    public partial class CalendarViewControl
    {
        #region Private Members

        private CalendarDayUserControl[] _calendar = null;

        private TextBlock[] _weekTitle = null;

        private Dictionary<DayOfWeek, int> _weekDayOfWeekIndexes = null;

        private int? _selectedCell = null;

        #endregion

        #region Properties

        public NimbaDataManager DataManager
        {
            get;
            set;
        }

        #endregion

        #region Initializers

        public CalendarViewControl(NimbaDataManager dataManager)
        {
            try
            {
                InitializeComponent();

                InitializeCalendar();

                InitializeWeekTitle();

                InitializeWeekDayOfWeekIndexes();

                this.DataManager = dataManager;
            }
            catch (Exception exception)
            {
                string message = exception.Message;

                throw exception;
            }
        }

        private void InitializeCalendar()
        {
            _calendar = new CalendarDayUserControl[42];

            _calendar[0] = Day0;
            _calendar[1] = Day1;
            _calendar[2] = Day2;
            _calendar[3] = Day3;
            _calendar[4] = Day4;
            _calendar[5] = Day5;
            _calendar[6] = Day6;
            _calendar[7] = Day7;
            _calendar[8] = Day8;
            _calendar[9] = Day9;

            _calendar[10] = Day10;
            _calendar[11] = Day11;
            _calendar[12] = Day12;
            _calendar[13] = Day13;
            _calendar[14] = Day14;
            _calendar[15] = Day15;
            _calendar[16] = Day16;
            _calendar[17] = Day17;
            _calendar[18] = Day18;
            _calendar[19] = Day19;

            _calendar[20] = Day20;
            _calendar[21] = Day21;
            _calendar[22] = Day22;
            _calendar[23] = Day23;
            _calendar[24] = Day24;
            _calendar[25] = Day25;
            _calendar[26] = Day26;
            _calendar[27] = Day27;
            _calendar[28] = Day28;
            _calendar[29] = Day29;

            _calendar[30] = Day30;
            _calendar[31] = Day31;
            _calendar[32] = Day32;
            _calendar[33] = Day33;
            _calendar[34] = Day34;
            _calendar[35] = Day35;
            _calendar[36] = Day36;
            _calendar[37] = Day37;
            _calendar[38] = Day38;
            _calendar[39] = Day39;

            _calendar[40] = Day40;
            _calendar[41] = Day41;

            foreach (CalendarDayUserControl calendarDayUserControl in _calendar)
                calendarDayUserControl.DayProperties_Changed += new CalendarDayUserControl.DayProperties_ChangedDelegate(CalendarDayUserControl_DayProperties_Changed);
        }

        private void InitializeWeekTitle()
        {
            _weekTitle = new TextBlock[7];

            _weekTitle[0] = TitleDay1;
            _weekTitle[1] = TitleDay2;
            _weekTitle[2] = TitleDay3;
            _weekTitle[3] = TitleDay4;
            _weekTitle[4] = TitleDay5;
            _weekTitle[5] = TitleDay6;
            _weekTitle[6] = TitleDay7;
        }

        private void InitializeWeekDayOfWeekIndexes()
        {
            _weekDayOfWeekIndexes = new Dictionary<DayOfWeek, int>();

            foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
            {
                _weekDayOfWeekIndexes.Add(dayOfWeek, (int)dayOfWeek);
            }
        }

        #endregion

        #region Private Methods

        private void CleanCalendar()
        {
            for (int i = 0; i < _calendar.Length; ++i)
            {
                _calendar[i].setState(CalendarDayUserControl.DayState.EmptyDisabled);
            }

            _selectedCell = null;
        }

        private void UpdateFirstDayOfWeek(DayOfWeek firstDayOfWeek)
        {
            int delta = 0;
            string culture = null;
            CultureInfo cultureInfo = null;

            switch (this.DataManager.UserLanguage)
            {
                case Data.Language.English:
                {
                    culture = "en-US";
                    break;
                }
                case Data.Language.French:
                {
                    culture = "fr-CA";
                    break;
                }
            }

            cultureInfo = new CultureInfo(culture);

            for (int dayOfWeek = 0; dayOfWeek < _weekDayOfWeekIndexes.Count; ++dayOfWeek)
            {
                delta = (int)firstDayOfWeek - dayOfWeek;

                if (delta <= 0)
                    _weekDayOfWeekIndexes[(DayOfWeek)dayOfWeek] = 0 - delta;
                else
                    _weekDayOfWeekIndexes[(DayOfWeek)dayOfWeek] = 7 - delta;

                _weekTitle[_weekDayOfWeekIndexes[(DayOfWeek)dayOfWeek]].Text = cultureInfo.DateTimeFormat.DayNames[dayOfWeek];

            }
        }

        private int GetFirstMonthFirstDayIndex()
        {
            DateTime firstDayOfMonth = new DateTime(DatetimeSlider.Date.Year, DatetimeSlider.Date.Month, 1);

            return _weekDayOfWeekIndexes[firstDayOfMonth.DayOfWeek];
        }

        #endregion

        #region Public Methods

        public void LoadMonth()
        {
            int indexFirstDayOfMonth = -1;
            int numberOfDaysInMonth = -1;
            int day = 1;
            int lastEnableDay = 0;

            try
            {
                UpdateFirstDayOfWeek(this.DataManager.UserFirstDayOfWeek);

                indexFirstDayOfMonth = GetFirstMonthFirstDayIndex();
                numberOfDaysInMonth = DateTime.DaysInMonth(DatetimeSlider.Date.Year, DatetimeSlider.Date.Month);

                CleanCalendar();

                //
                // Determine the number of days to enable in the month.
                //
                if ((DatetimeSlider.Date.Year < DateTime.Now.Year) ||
                    ((DatetimeSlider.Date.Year == DateTime.Now.Year) && (DatetimeSlider.Date.Month < DateTime.Now.Month)))
                {
                    lastEnableDay = numberOfDaysInMonth;
                }
                else if ((DatetimeSlider.Date.Year == DateTime.Now.Year) && (DatetimeSlider.Date.Month == DateTime.Now.Month))
                    lastEnableDay = DateTime.Now.Day;
                else
                    lastEnableDay = 0;

                for (int i = indexFirstDayOfMonth; i < indexFirstDayOfMonth + numberOfDaysInMonth; ++i)
                {
                    if (i < indexFirstDayOfMonth + lastEnableDay)
                        _calendar[i].setState(CalendarDayUserControl.DayState.Enabled);
                    else
                        _calendar[i].setState(CalendarDayUserControl.DayState.Disabled);

                    _calendar[i].Day = day.ToString();
                    ++day;

                    if (this.DataManager == null)
                        continue;

                    _calendar[i].SetDay(new DateTime(DatetimeSlider.Date.Year, DatetimeSlider.Date.Month, day - 1), this.DataManager);
                }
            }
            catch (Exception exception)
            {
                string message = exception.Message;

                throw exception;
            }
        }

        #endregion

        #region Event Handling

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadMonth();
            }
            catch (Exception exception)
            {
                string message = exception.Message;

                throw exception;
            }
        }

        private void DatetimeSlider_MonthChanged(object sender, EventArgs e)
        {
            try
            {
                LoadMonth();
            }
            catch (Exception exception)
            {
                string message = exception.Message;

                throw exception;
            }
        }

        private void Day_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                CalendarDayUserControl day = (CalendarDayUserControl)sender;

                switch (day.State)
                {
                    case CalendarDayUserControl.DayState.Enabled:
                    {
                        if (_selectedCell.HasValue)
                            _calendar[_selectedCell.Value].setState(CalendarDayUserControl.DayState.Enabled);

                        day.setState(CalendarDayUserControl.DayState.Selected);

                        _selectedCell = day.Index;

                        this.DataManager.AddDate(new DateTime(DatetimeSlider.Date.Year, DatetimeSlider.Date.Month, Convert.ToInt32(day.Day)), true);

                        break;
                    }
                }
            }
            catch (Exception exception)
            {
                string message = exception.Message;

                throw exception;
            }
        }

        private void CalendarDayUserControl_DayProperties_Changed(object sender, RoutedEventArgs eventArgs)
        {
            try
            {
                CalendarDayUserControl day = (CalendarDayUserControl)sender;
                NimbaDatetime nimbaDatetime = null;

                if (!this.DataManager.FindDate(new DateTime(DatetimeSlider.Date.Year, DatetimeSlider.Date.Month, Convert.ToInt32(day.Day)), true, out nimbaDatetime))
                    return;

                nimbaDatetime.IsPeriod = day.PeriodDay;
                nimbaDatetime.IsOvulation = day.OvulationDay;
                nimbaDatetime.IsLiquidMucus = day.LiquidMucusDay;
                nimbaDatetime.Temperature = day.Temperature;
                nimbaDatetime.StartPregnancy = day.StartPregnancyDay;
                nimbaDatetime.EndPregnancy = day.EndPregnancyDay;

                this.DataManager.LoadCycle();

                this.LoadMonth();
            }
            catch (Exception exception)
            {
                string message = exception.Message;

                throw exception;
            }
        }

        #endregion
    }
}
