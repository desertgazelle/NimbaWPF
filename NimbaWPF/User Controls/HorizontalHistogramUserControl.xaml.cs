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
    /// Interaction logic for HorizontalHistogramUserControl.xaml
    /// </summary>
    public partial class HorizontalHistogramUserControl : UserControl
    {
        #region Enumeration

        enum Index
        {
            Maximum = 0,
            Average = 1,
            Current = 2,
            Minimum = 3,
        }

        #endregion

        #region Private Members

        private double _maximum = 100.0;

        private double _average = 40.0;

        private double _current = 50.0;

        private double _minimum = -20.0;

        private string _maximumLabel = "Maximum";

        private string _averageLabel = "Average";

        private string _currentLabel = "Current";

        private string _minimumLabel = "Minimum";

        private string _degreeUnit = string.Empty;

        #endregion

        #region Properties

        public double Maximum
        {
            set
            {
                _maximum = value;
            }
        }

        public double Average
        {
            set
            {
                _average = value;
            }
        }

        public double Current
        {
            set
            {
                _current = value;
            }
        }

        public double Minimum
        {
            set
            {
                _minimum = value;
            }
        }

        public string MaximumLabel
        {
            set
            {
                _maximumLabel = value;
            }
        }

        public string AverageLabel
        {
            set
            {
                _averageLabel = value;
            }
        }

        public string CurrentLabel
        {
            set
            {
                _currentLabel = value;
            }
        }

        public string MinimumLabel
        {
            set
            {
                _minimumLabel = value;
            }
        }

        public string DegreeUnit
        {
            set
            {
                _degreeUnit = value;
            }
        }

        #endregion

        #region Initializers

        public HorizontalHistogramUserControl()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception exception)
            {
                string message = exception.Message;

                throw exception;
            }
        }

        #endregion

        #region Private Methods

        private void Update()
        {
            SetLabels();

            SetBars();
        }

        private void SetLabels()
        {
            _maximumLabelLeft.Text = _maximumLabel;
            _averageLabelLeft.Text = _averageLabel;
            _currentLabelLeft.Text = _currentLabel;
            _minimumLabelLeft.Text = _minimumLabel;

            _maximumLabelRight.Text = string.Format("{0:F}{1}", _maximum, _degreeUnit);
            _averageLabelRight.Text = string.Format("{0:F}{1}", _average, _degreeUnit);
            _currentLabelRight.Text = string.Format("{0:F}{1}", _current, _degreeUnit);
            _minimumLabelRight.Text = string.Format("{0:F}{1}", _minimum, _degreeUnit);
        }

        private void SetBars()
        {
            double[] values = null;
            double columnWidth = 0.0;

            values = new double[4];

            columnWidth = 0.90 * _barColumn.ActualWidth;

            values[(int)Index.Maximum] = columnWidth;
            values[(int)Index.Average] = columnWidth * _average / _maximum;
            values[(int)Index.Current] = columnWidth * _current / _maximum;
            values[(int)Index.Minimum] = columnWidth * _minimum / _maximum;

            for (int i = 0; i < 4; ++i)
            {
                SetOneBar(values[i], (Index)i);
            }
        }

        private void SetOneBar(double value, Index index)
        {
            Button button = null;

            switch (index)
            {
                case Index.Maximum:
                {
                    button = _maximumBar;
                    break;
                }
                case Index.Average:
                {
                    button = _averageBar;
                    break;
                }
                case Index.Current:
                {
                    button = _currentBar;
                    break;
                }
                case Index.Minimum:
                {
                    button = _minimumBar;
                    break;
                }
            }

            if (value > 0.0)
            {
                button.Width = value;
                button.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                Grid.SetColumn(button, 2);
            }
            else if (value < 0.0)
            {
                button.Width = -1.0 * value;
                button.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                Grid.SetColumn(button, 1);
            }
            else
            {
                button.Width = 0;
            }            
        }

        #endregion

        #region Event Handling

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Update();
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
