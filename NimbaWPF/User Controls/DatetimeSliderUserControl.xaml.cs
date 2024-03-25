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

namespace NimbaWPF
{
    /// <summary>
    /// Interaction logic for DatetimeSliderUserControl.xaml
    /// </summary>
    public partial class DatetimeSliderUserControl : UserControl
    {
        #region Delegate

        public delegate void Month_ChangedDelegate(object sender, EventArgs e);

        #endregion

        #region Events

        public event Month_ChangedDelegate MonthChanged;

        #endregion

        #region Properties

        public DateTime Date
        {
            get;
            set;
        }

        #endregion

        #region Initializers

        public DatetimeSliderUserControl()
        {
            try
            {
                this.Date = DateTime.Now;
                InitializeComponent();
                SetMonthPickerText();
            }
            catch (Exception exception)
            {
                string message = exception.Message;

                throw exception;
            }
        }

        #endregion

        #region Private Methods

        private void SetMonthPickerText()
        {
            CultureInfo cultureInfo = new CultureInfo("en-US");
            datetimeText.Text = string.Format("{0} {1}", this.Date.Year, cultureInfo.DateTimeFormat.MonthNames[this.Date.Month - 1]);
        }

        private void FireMonthChangedEvent()
        {
            if (MonthChanged != null)
                MonthChanged(this, new EventArgs());
        }
        
        #endregion

        #region Event Handling

        private void next_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Date = this.Date.AddMonths(1);
                SetMonthPickerText();
                FireMonthChangedEvent();
            }
            catch (Exception exception)
            {
                string message = exception.Message;

                throw exception;
            }
        }

        private void previous_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Date = this.Date.AddMonths(-1);
                SetMonthPickerText();
                FireMonthChangedEvent();
            }
            catch (Exception exception)
            {
                string message = exception.Message;

                throw exception;
            }
        }        

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SetMonthPickerText();
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
