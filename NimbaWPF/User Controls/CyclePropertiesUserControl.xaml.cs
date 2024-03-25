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

namespace NimbaWPF
{
    /// <summary>
    /// Interaction logic for CyclePropertiesUserControl.xaml
    /// </summary>
    public partial class CyclePropertiesUserControl : UserControl
    {
        #region Properties

        public string firstCycleDayLabel
        {
            get;
            set;
        }

        public string lastCycleDayLabel
        {
            get;
            set;
        }
        
        public string ovulationDayLabel
        {
            get;
            set;
        }

        public string numberDaysCycleLabel
        {
            get;
            set;
        }

        public string numberPeriodDaysCycleLabel
        {
            get;
            set;
        }

        public DateTime firstCycleDay
        {
            get;
            set;
        }

        public DateTime lastCycleDay
        {
            get;
            set;
        }

        public DateTime ovulationDay
        {
            get;
            set;
        }

        public int numberDaysCycle
        {
            get;
            set;
        }

        public int numberPeriodDaysCycle
        {
            get;
            set;
        }
        
        #endregion

        #region Initializers

        public CyclePropertiesUserControl()
        {
            try
            {
                this.firstCycleDayLabel = "First day of the cycle:";
                this.lastCycleDayLabel = "Last day of the cycle:";
                this.ovulationDayLabel = "Ovulation day:";
                this.numberDaysCycleLabel = "Cycle number of days:";
                this.numberPeriodDaysCycleLabel = "Number of period days:";
                this.firstCycleDay = DateTime.Now;
                this.lastCycleDay = DateTime.Now;
                this.numberDaysCycle = 26;
                this.numberPeriodDaysCycle = 5;

                InitializeComponent();

                SetControls();
            }
            catch (Exception exception)
            {
                string message = exception.Message;

                throw exception;
            }
        }

        #endregion

        #region Private Methods

        private void SetControls()
        {
            _firstCycleDayLabel.Text = this.firstCycleDayLabel;
            _lastCycleDayLabel.Text = this.lastCycleDayLabel;
            _ovulationCycleDayLabel.Text = this.ovulationDayLabel;
            _nbDaysInCycleLabel.Text = this.numberDaysCycleLabel;
            _nbPeriodDaysInCycleLabel.Text = this.numberPeriodDaysCycleLabel;

            _firstCycleDay.Text = this.firstCycleDay.ToLongDateString();
            _lastCycleDay.Text = this.lastCycleDay.ToLongDateString();
            _ovulationCycleDay.Text = this.ovulationDay.ToLongDateString();
            _nbDaysInCycle.Text = this.numberDaysCycle.ToString();
            _nbPeriodDaysInCycle.Text = this.numberPeriodDaysCycle.ToString();
        }
        
        #endregion

        #region Event Handling

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SetControls();
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
