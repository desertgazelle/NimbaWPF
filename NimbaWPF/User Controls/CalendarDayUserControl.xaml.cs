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

using NimbaWPF.Data;

namespace NimbaWPF
{
    /// <summary>
    /// Logique d'interaction pour CalendarDayUserControl.xaml
    /// </summary>
    public partial class CalendarDayUserControl : UserControl
    {
        #region Enumeration

        public enum DayState
        {
            EmptyDisabled,
            Disabled,
            Enabled,
            Selected,
        }

        #endregion

        #region Constants

        Brush _defaultColor = Brushes.Beige;
        Brush _selectedColor = Brushes.BurlyWood;
        Brush _normalCycleColor = Brushes.LightGreen;
        Brush _pregnancyColor = Brushes.LightBlue;
        Brush _transitionColor = Brushes.Khaki;

        #endregion

        #region Private Members

        private double _defaultTemperature = 37.5;

        private bool _startPregnancyDay = false;

        private bool _endPregnancyDay = false;

        private bool _isInitialized = false;

        #endregion

        #region Properties

        public string Day
        {
            set
            {
                DayNumber.Text = value;
            }
            get
            {
                return DayNumber.Text;
            }
        }
        
        public double? Temperature
        {
            get
            {
                return _dayTemperature.Value;
            }
            set
            {
                SetTemperature(value);                    
            }
        }

        public bool PeriodDay
        {
            get
            {
                return DayPeriod.IsVisible;
            }
            set
            {
                SetPeriodDay(value);
            }
        }

        public bool NextPeriodDay
        {
            get
            {
                return DayNextPeriod.IsVisible;
            }
            set
            {
                SetNextPeriodDay(value);
            }
        }

        public bool OvulationDay
        {
            get
            {
                return DayOvulation.IsVisible;
            }
            set
            {
                SetOvulationDay(value);
            }
        }

        public bool NextOvulationDay
        {
            get
            {
                return DayNextOvulation.IsVisible;
            }
            set
            {
                SetNextOvulationDay(value);
            }
        }

        public bool LiquidMucusDay
        {
            get
            {
                return DayLiquidMucus.IsVisible;
            }
            set
            {
                SetLiquidMucusDay(value);
            }
        }

        public bool PregnancyDay
        {
            get
            {
                return (DayPregnant.IsVisible && !_startPregnancyDay && !_endPregnancyDay);
            }
            set
            {
                SetPregnantDay(value);
            }
        }

        public bool StartPregnancyDay
        {
            get
            {
                return _startPregnancyDay;
            }
            set
            {
                SetStartPregnancyDay(value);
            }
        }

        public bool EndPregnancyDay
        {
            get
            {
                return _endPregnancyDay;
            }
            set
            {
                SetEndPregnancyDay(value);
            }
        }

        public DayState State
        {
            get;
            set;
        }

        public int Index
        {
            get;
            set;
        }

        #endregion

        #region Delegates

        public delegate void DayProperties_ChangedDelegate(object sender, RoutedEventArgs eventArgs);

        #endregion

        #region Events

        public event DayProperties_ChangedDelegate DayProperties_Changed;

        #endregion

        #region Initializers

        public CalendarDayUserControl()
        {
            InitializeComponent();

            this.Index = -1;

            SetStateEmptyDisabled();
        }

        #endregion

        #region Private Methods

        private void EnableContextMenu(bool enable)
        {
            _buttonDayNumber.IsEnabled = enable;
            _buttonDayLiquidMucus.IsEnabled = enable;
            _buttonDayPregnant.IsEnabled = enable;
            _buttonDayPeriod.IsEnabled = enable;
            _buttonDayOvulation.IsEnabled = enable;
            _buttonTemperature.IsEnabled = enable;
            _buttonDayNextPeriod.IsEnabled = enable;
            _buttonDayNextOvulation.IsEnabled = enable;

            if (enable)
                _grid.ContextMenu = DayContextMenu;
            else
                _grid.ContextMenu = null;
        }

        private void SetStateEmptyDisabled()
        {
            //
            // No day number. No icon. ContextMenu disabled.
            //
            DayNumber.Text = string.Empty;
            DayLiquidMucus.Visibility = System.Windows.Visibility.Hidden;
            DayNextOvulation.Visibility = System.Windows.Visibility.Hidden;
            DayNextPeriod.Visibility = System.Windows.Visibility.Hidden;
            DayOvulation.Visibility = System.Windows.Visibility.Hidden;
            DayPeriod.Visibility = System.Windows.Visibility.Hidden;
            DayPregnant.Visibility = System.Windows.Visibility.Hidden;
            _buttonDayPregnant.Background = Brushes.Transparent;
            _dayTemperature.Visibility = System.Windows.Visibility.Hidden;
            _dayTemperature.Value = null;

            _calendarDayUserControl.BorderThickness = new Thickness(1.0);

            _calendarDayUserControl.Background = _defaultColor;
            _grid.Background = _defaultColor;

            EnableContextMenu(false);

            this.State = DayState.EmptyDisabled;

            DayContextMenu.Items.Clear();

            _isInitialized = false;
        }

        private void SetDisabled()
        {
            //
            // ContextMenu disabled. NextOvulation and NextPeriod may be enabled.
            //
            DayLiquidMucus.Visibility = System.Windows.Visibility.Hidden;
            DayOvulation.Visibility = System.Windows.Visibility.Hidden;
            DayPeriod.Visibility = System.Windows.Visibility.Hidden;
            DayPregnant.Visibility = System.Windows.Visibility.Hidden;
            _dayTemperature.Visibility = System.Windows.Visibility.Hidden;
            _dayTemperature.Value = null;

            _calendarDayUserControl.BorderThickness = new Thickness(1.0);

            _calendarDayUserControl.Background = _defaultColor;

            EnableContextMenu(false);

            this.State = DayState.Disabled;
        }

        private void SetEnabled()
        {
            //
            // ContextMenu enabled. All icon may be enabled.
            //
            _calendarDayUserControl.BorderThickness = new Thickness(1.0);

            _calendarDayUserControl.Background = _defaultColor;

            EnableContextMenu(true);

            this.State = DayState.Enabled;
        }

        private void SetSelected()
        {
            //
            // ContextMenu enabled. All icon may be enabled. Selected.
            //
            _calendarDayUserControl.BorderThickness = new Thickness(3.0);

            EnableContextMenu(true);

            this.State = DayState.Selected;
        }

        private void SetPeriodDay(bool visible)
        {
            if (visible)
                DayPeriod.Visibility = System.Windows.Visibility.Visible;
            else
                DayPeriod.Visibility = System.Windows.Visibility.Hidden;
        }

        private void SetNextPeriodDay(bool visible)
        {
            if (visible)
                DayNextPeriod.Visibility = System.Windows.Visibility.Visible;
            else
                DayNextPeriod.Visibility = System.Windows.Visibility.Hidden;
        }

        private void SetOvulationDay(bool visible)
        {
            if (visible)
                DayOvulation.Visibility = System.Windows.Visibility.Visible;
            else
                DayOvulation.Visibility = System.Windows.Visibility.Hidden;
        }

        private void SetNextOvulationDay(bool visible)
        {
            if (visible)
                DayNextOvulation.Visibility = System.Windows.Visibility.Visible;
            else
                DayNextOvulation.Visibility = System.Windows.Visibility.Hidden;
        }

        private void SetLiquidMucusDay(bool visible)
        {
            if (visible)
                DayLiquidMucus.Visibility = System.Windows.Visibility.Visible;
            else
                DayLiquidMucus.Visibility = System.Windows.Visibility.Hidden;
        }

        private void SetPregnantDay(bool visible)
        {
            if (visible)
                DayPregnant.Visibility = System.Windows.Visibility.Visible;
            else
                DayPregnant.Visibility = System.Windows.Visibility.Hidden;
        }

        private void SetStartPregnancyDay(bool visible)
        {
            if (visible)
            {
                _startPregnancyDay = true;
                DayPregnant.Visibility = System.Windows.Visibility.Visible;
                _buttonDayPregnant.Background = Brushes.Violet;
            }
            else
            {
                _startPregnancyDay = false;
                DayPregnant.Visibility = System.Windows.Visibility.Hidden;
                _buttonDayPregnant.Background = Brushes.Transparent;
            }
        }

        private void SetEndPregnancyDay(bool visible)
        {
            if (visible)
            {
                _endPregnancyDay = true;
                DayPregnant.Visibility = System.Windows.Visibility.Visible;
                _buttonDayPregnant.Background = Brushes.SeaGreen;
            }
            else
            {
                _endPregnancyDay = false;
                DayPregnant.Visibility = System.Windows.Visibility.Hidden;
                _buttonDayPregnant.Background = Brushes.Transparent;
            }
        }

        private void SetTemperature(double? temperature)
        {
            if (temperature.HasValue)
                _dayTemperature.Visibility = System.Windows.Visibility.Visible;
            else
                _dayTemperature.Visibility = System.Windows.Visibility.Hidden;

            _dayTemperature.Value = temperature;
        }

        private void FireDayPropertiesChangedEvent()
        {
            if (DayProperties_Changed == null)
                return;

            DayProperties_Changed(this, new RoutedEventArgs());
        }

        private void AddMenuItem(string name, string header, string imageSource, bool isChecked, RoutedEventHandler eventHandler)
        {
            MenuItem menuItem = new MenuItem();
            Image image = new Image();

            image.Source = new BitmapImage(new Uri(imageSource));
            image.Width = 32;
            image.Height = 32;

            menuItem.Name = name;
            menuItem.Header = header;
            menuItem.IsCheckable = true;
            menuItem.IsChecked = isChecked;
            menuItem.Icon = image;
            menuItem.Click += eventHandler;

            DayContextMenu.Items.Add(menuItem);
        }

        private void BuildContextMenu(CycleType cycleType, NimbaDatetime nimbaDatetime)
        {
            switch (cycleType)
            {
                case CycleType.Normal:
                {
                    AddMenuItem("DayContextMenuPeriod", "_Period", "pack://application:,,,/NimbaWPF;component/Resources/Fairytale_no.png", nimbaDatetime.IsPeriod, ContextMenuPeriod_Click);
                    AddMenuItem("DayContextMenuOvulation", "_Ovulation", "pack://application:,,,/NimbaWPF;component/Resources/Fairytale_colors.png", nimbaDatetime.IsOvulation, ContextMenuOvulation_Click);
                    AddMenuItem("DayContextMenuLiquidMucus", "_Liquid Mucus", "pack://application:,,,/NimbaWPF;component/Resources/Fairytale_firewire.png", nimbaDatetime.IsLiquidMucus, ContextMenuLiquidMucus_Click);
                    AddMenuItem("DayContextMenuTemperature", "_Temperature", "pack://application:,,,/NimbaWPF;component/Resources/Temperature_icon.png", nimbaDatetime.Temperature.HasValue, ContextMenuTemperature_Click);
                    AddMenuItem("DayContextMenuStartPregnancy", "_Start Pregnancy", "pack://application:,,,/NimbaWPF;component/Resources/pregnant.png", nimbaDatetime.StartPregnancy, ContextMenuStartPregnancy_Click);
                    break;
                }
                case CycleType.Pregnancy:
                {
                    AddMenuItem("DayContextMenuPeriod", "_Period", "pack://application:,,,/NimbaWPF;component/Resources/Fairytale_no.png", nimbaDatetime.IsPeriod, ContextMenuPeriod_Click);
                    AddMenuItem("DayContextMenuLiquidMucus", "_Liquid Mucus", "pack://application:,,,/NimbaWPF;component/Resources/Fairytale_firewire.png", nimbaDatetime.IsLiquidMucus, ContextMenuLiquidMucus_Click);
                    AddMenuItem("DayContextMenuTemperature", "_Temperature", "pack://application:,,,/NimbaWPF;component/Resources/Temperature_icon.png", nimbaDatetime.Temperature.HasValue, ContextMenuTemperature_Click);
                    if (nimbaDatetime.StartPregnancy)
                        AddMenuItem("DayContextMenuStartPregnancy", "_Start Pregnancy", "pack://application:,,,/NimbaWPF;component/Resources/pregnant.png", nimbaDatetime.StartPregnancy, ContextMenuStartPregnancy_Click);

                    if (!nimbaDatetime.StartPregnancy)
                        AddMenuItem("DayContextMenuEndPregnancy", "_End Pregnancy", "pack://application:,,,/NimbaWPF;component/Resources/pregnant.png", nimbaDatetime.EndPregnancy, ContextMenuEndPregnancy_Click);
                    break;
                }
                default:
                {
                    AddMenuItem("DayContextMenuPeriod", "_Period", "pack://application:,,,/NimbaWPF;component/Resources/Fairytale_no.png", nimbaDatetime.IsPeriod, ContextMenuPeriod_Click);
                    AddMenuItem("DayContextMenuOvulation", "_Ovulation", "pack://application:,,,/NimbaWPF;component/Resources/Fairytale_colors.png", nimbaDatetime.IsOvulation, ContextMenuOvulation_Click);
                    AddMenuItem("DayContextMenuLiquidMucus", "_Liquid Mucus", "pack://application:,,,/NimbaWPF;component/Resources/Fairytale_firewire.png", nimbaDatetime.IsLiquidMucus, ContextMenuLiquidMucus_Click);
                    AddMenuItem("DayContextMenuTemperature", "_Temperature", "pack://application:,,,/NimbaWPF;component/Resources/Temperature_icon.png", nimbaDatetime.Temperature.HasValue, ContextMenuTemperature_Click);
                    AddMenuItem("DayContextMenuStartPregnancy", "_Start Pregnancy", "pack://application:,,,/NimbaWPF;component/Resources/pregnant.png", nimbaDatetime.StartPregnancy, ContextMenuStartPregnancy_Click);
                    break;
                }
            }
        }

        private void SetTemperatureConfiguration(NimbaDataManager dataManager)
        {
            switch (dataManager.UserDegreeUnit)
            {
                case DegreeUnit.Celsius:
                    {
                        _dayTemperature.Unit = "°C";
                        _dayTemperature.DisplayFormat = "{0:F1}{1}";
                        _dayTemperature.Minimum = 35;
                        _dayTemperature.Increment = 0.1;
                        _dayTemperature.Maximum = 41;
                        _defaultTemperature = 37.5;
                        break;
                    }
                case DegreeUnit.Farenheit:
                    {
                        _dayTemperature.Unit = "°F";
                        _dayTemperature.DisplayFormat = "{0:F2}{1}";
                        _dayTemperature.Increment = 0.18;
                        _dayTemperature.Minimum = 95.0;
                        _defaultTemperature = 97.70;
                        _dayTemperature.Maximum = 105.80;
                        break;
                    }
            }
        }

        #endregion

        #region Public Methods

        public void setState(DayState dayState)
        {
            try
            {
                switch (dayState)
                {
                    case DayState.EmptyDisabled:
                        {
                            SetStateEmptyDisabled();
                            break;
                        }
                    case DayState.Disabled:
                        {
                            SetDisabled();
                            break;
                        }
                    case DayState.Enabled:
                        {
                            SetEnabled();
                            break;
                        }
                    case DayState.Selected:
                        {
                            SetSelected();
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

        public void SetDay(DateTime datetime, NimbaDataManager dataManager)
        {
            NimbaCycle nimbaCycle = null;
            NimbaDatetime nimbaDatetime = null;

            try
            {
                if (!dataManager.FindCycle(datetime, out nimbaCycle))
                {
                    nimbaCycle = new NimbaCycle();
                    nimbaCycle.Type = CycleType.None;
                }

                if (!dataManager.FindDate(datetime, false, out nimbaDatetime))
                    nimbaDatetime = new NimbaDatetime();

                switch (nimbaCycle.Type)
                {
                    case CycleType.Normal:
                        {
                            _grid.Background = _normalCycleColor;
                            break;
                        }
                    case CycleType.Pregnancy:
                        {
                            _grid.Background = _pregnancyColor;
                            break;
                        }
                    case CycleType.Transition:
                        {
                            _grid.Background = _transitionColor;
                            break;
                        }
                    default:
                        {
                            _grid.Background = _defaultColor;
                            break;
                        }
                }

                BuildContextMenu(nimbaCycle.Type, nimbaDatetime);

                PeriodDay = nimbaDatetime.IsPeriod;
                OvulationDay = nimbaDatetime.IsOvulation;
                LiquidMucusDay = nimbaDatetime.IsLiquidMucus;
                NextPeriodDay = nimbaDatetime.IsNextPeriod;
                NextOvulationDay = nimbaDatetime.IsNextOvulation;
                if (nimbaDatetime.StartPregnancy)
                    StartPregnancyDay = nimbaDatetime.StartPregnancy;
                else if (nimbaDatetime.EndPregnancy)
                    EndPregnancyDay = nimbaDatetime.EndPregnancy;
                else if (nimbaDatetime.IsPregnant)
                    PregnancyDay = nimbaDatetime.IsPregnant;
                if (nimbaDatetime.Temperature.HasValue)
                    Temperature = nimbaDatetime.Temperature.Value;

                SetTemperatureConfiguration(dataManager);

                _isInitialized = true;
            }
            catch (Exception exception)
            {
                string message = exception.Message;

                throw exception;
            }
        }

        #endregion

        #region Event Handling

        private void ContextMenuPeriod_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.State == DayState.Disabled || this.State == DayState.EmptyDisabled)
                    return;

                SetPeriodDay(!DayPeriod.IsVisible);

                FireDayPropertiesChangedEvent();
            }
            catch (Exception exception)
            {
                string message = exception.Message;

                throw exception;
            }
        }

        private void ContextMenuOvulation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.State == DayState.Disabled || this.State == DayState.EmptyDisabled)
                    return;

                SetOvulationDay(!DayOvulation.IsVisible);

                FireDayPropertiesChangedEvent();
            }
            catch (Exception exception)
            {
                string message = exception.Message;

                throw exception;
            }
        }

        private void ContextMenuLiquidMucus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.State == DayState.Disabled || this.State == DayState.EmptyDisabled)
                    return;

                SetLiquidMucusDay(!DayLiquidMucus.IsVisible);

                FireDayPropertiesChangedEvent();
            }
            catch (Exception exception)
            {
                string message = exception.Message;

                throw exception;
            }
        }

        private void ContextMenuTemperature_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.State == DayState.Disabled || this.State == DayState.EmptyDisabled)
                    return;

                if (_dayTemperature.IsVisible)
                    SetTemperature(null);
                else
                {
                    SetTemperature(_defaultTemperature);
                }

                FireDayPropertiesChangedEvent();
            }
            catch (Exception exception)
            {
                string message = exception.Message;

                throw exception;
            }
        }

        private void DayTemperature_ValueChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.State == DayState.Disabled || this.State == DayState.EmptyDisabled)
                    return;

                if (_isInitialized)
                    FireDayPropertiesChangedEvent();
            }
            catch (Exception exception)
            {
                string message = exception.Message;

                throw exception;
            }
        }

        private void ContextMenuStartPregnancy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.State == DayState.Disabled || this.State == DayState.EmptyDisabled)
                    return;

                SetStartPregnancyDay(!DayPregnant.IsVisible);

                FireDayPropertiesChangedEvent();
            }
            catch (Exception exception)
            {
                string message = exception.Message;

                throw exception;
            }
        }

        private void ContextMenuEndPregnancy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.State == DayState.Disabled || this.State == DayState.EmptyDisabled)
                    return;

                SetEndPregnancyDay(!DayPregnant.IsVisible);

                FireDayPropertiesChangedEvent();
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
