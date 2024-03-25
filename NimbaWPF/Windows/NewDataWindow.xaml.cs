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
using System.Windows.Shapes;

namespace NimbaWPF
{
    /// <summary>
    /// Interaction logic for NewDataWindow.xaml
    /// </summary>
    public partial class NewDataWindow : Window
    {
        #region Properties

        public string TitleLabel
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string FirstPeriodDayLabel
        {
            get;
            set;
        }

        public string OK
        {
            get;
            set;
        }

        public string Cancel
        {
            get;
            set;
        }

        public DateTime? FirstPeriodDay
        {
            get;
            set;
        }
        
        #endregion

        #region Initializers

        public NewDataWindow()
        {
            this.TitleLabel = "Start a new cycle";
            this.Description = "Specify the first day of your last period to start a new cycle:";
            this.FirstPeriodDayLabel = "The first day of your last period:";
            this.FirstPeriodDay = DateTime.Now;
            this.OK = "OK";
            this.Cancel = "Cancel";

            InitializeComponent();

            SetControls();
        }

        #endregion

        #region Private Methods

        private void SetControls()
        {
            _window.TitleLabel = this.TitleLabel;
            _descriptionLabel.Text = this.Description;
            _datetimeLabel.Text = this.FirstPeriodDayLabel;
            _OK.Content = this.OK;
            _cancel.Content = this.Cancel;
            _datePicker.DisplayDate = this.FirstPeriodDay.Value;
            _datePicker.SelectedDate = this.FirstPeriodDay;
        }

        #endregion

        #region Event Handling

        private void _window_Loaded(object sender, RoutedEventArgs e)
        {
            SetControls();
        }

        private void _OK_Click(object sender, RoutedEventArgs e)
        {
            this.FirstPeriodDay = _datePicker.SelectedDate;
            this.Close();
        }

        private void _cancel_Click(object sender, RoutedEventArgs e)
        {
            this.FirstPeriodDay = null;
            this.Close();
        }

        #endregion

    }
}
