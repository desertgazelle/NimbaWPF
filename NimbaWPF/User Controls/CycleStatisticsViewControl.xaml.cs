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
    /// Interaction logic for CycleStatisticsViewControl.xaml
    /// </summary>
    public partial class CycleStatisticsViewControl : UserControl
    {
        #region Initializers

        public CycleStatisticsViewControl()
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

        #region Event Handling

        private void next_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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
