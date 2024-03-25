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

namespace NimbaWPF.User_Controls
{
    /// <summary>
    /// Interaction logic for NumericUpDownControl.xaml
    /// </summary>
    public partial class NumericUpDownControl : UserControl
    {
        #region Private Members

        private double? _numericValue = 0.0;

        #endregion

        #region Properties

        public double Minimum
        {
            get;
            set;
        }

        public double? Value
        {
            get
            {
                return _numericValue;
            }
            set
            {
                _numericValue = value;

                Refresh();
            }
        }

        public double Maximum
        {
            get;
            set;
        }

        public double Increment
        {
            get;
            set;
        }

        public string Unit
        {
            get;
            set;
        }

        public string DisplayFormat
        {
            get;
            set;
        }

        #endregion

        #region Delegates

        public delegate void Value_ChangedDelegate(object sender, RoutedEventArgs eventArgs);

        #endregion

        #region Events

        public event Value_ChangedDelegate Value_Changed;

        #endregion

        #region Initializers

        public NumericUpDownControl()
        {
            try
            {
                InitializeComponent();

                this.Minimum = Double.MinValue;
                this.Maximum = Double.MaxValue;
                this.Increment = 1.0;
                this.Unit = "°C";
                this.DisplayFormat = "{0:F1}{1}";
            }
            catch (Exception exception)
            {
                string message = exception.Message;

                throw exception;
            }
        }

        #endregion

        #region Private Methods

        private void Refresh()
        {
            if (this.Value.HasValue)
                _value.Text = string.Format(this.DisplayFormat, this.Value, this.Unit);
            else
                _value.Text = string.Empty;
        }

        private void FireValueChangedEvent()
        {
            if (Value_Changed == null)
                return;

            Value_Changed(this, new RoutedEventArgs());
        }

        #endregion

        #region Event Handling

        private void _up_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((!_numericValue.HasValue) || (_numericValue >= this.Maximum))
                    return;

                _numericValue += this.Increment;

                FireValueChangedEvent();

                Refresh();
            }
            catch (Exception exception)
            {
                string message = exception.Message;

                throw exception;
            }
        }

        private void _down_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((!_numericValue.HasValue) || (_numericValue <= this.Minimum))
                    return;

                _numericValue -= this.Increment;

                FireValueChangedEvent();

                Refresh();
            }
            catch (Exception exception)
            {
                string message = exception.Message;

                throw exception;
            }
        }

        private void _value_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = null;
            double value = 0.0;

            try
            {
                if (string.IsNullOrEmpty(this.Unit))
                    return;

                text = _value.Text.Replace(this.Unit, "");

                if (!double.TryParse(text, out value))
                    return;

                if ((value < this.Minimum) || (value > this.Maximum))
                    return;

                _numericValue = value;

                FireValueChangedEvent();

                Refresh();
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
