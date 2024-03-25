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
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:NimbaWPF.User_Controls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:NimbaWPF.User_Controls;assembly=NimbaWPF.User_Controls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:ClosableTabItem/>
    ///
    /// </summary>
    public class ClosableTabItem : TabItem
    {
        #region Initializers

        static ClosableTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ClosableTabItem), new FrameworkPropertyMetadata(typeof(ClosableTabItem)));
        }

        #endregion

        #region Events

        public static readonly RoutedEvent CloseTabEvent = EventManager.RegisterRoutedEvent("CloseTab", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ClosableTabItem));

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register("IconSource", typeof(ImageSource), typeof(ClosableTabItem), new FrameworkPropertyMetadata(new BitmapImage()));

        //public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register("IconSource", typeof(string), typeof(ClosableTabItem));

        #endregion

        #region Properties

        public string IconSource
        {
            //get
            //{
            //    return ((BitmapImage)GetValue(IconSourceProperty)).BaseUri.AbsolutePath;
            //}
            set
            {
                SetValue(IconSourceProperty, new BitmapImage(new Uri(value)));
            }
        }

        //public BitmapImage IconSource
        //{
        //    //get
        //    //{
        //    //    return ((BitmapImage)GetValue(IconSourceProperty)).BaseUri.AbsolutePath;
        //    //}
        //    set
        //    {
        //        SetValue(IconSourceProperty, value);
        //    }
        //}

        //public string IconSource
        //{
        //    //get
        //    //{
        //    //    return ((BitmapImage)GetValue(IconSourceProperty)).BaseUri.AbsolutePath;
        //    //}
        //    set
        //    {
        //        SetValue(IconSourceProperty, value);
        //    }
        //}

        #endregion

        #region Event Handler

        public event RoutedEventHandler CloseTab
        {
            add { AddHandler(CloseTabEvent, value); }
            remove { RemoveHandler(CloseTabEvent, value); }
        }

        #endregion

        #region Public Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Button closeButton = base.GetTemplateChild("PART_Close") as Button;
            if(closeButton != null)
                closeButton.Click += new RoutedEventHandler(closeButton_Click);
        }

        #endregion

        #region Event Handling

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs(CloseTabEvent, this));
        }

        #endregion
    }
}
