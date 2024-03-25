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
using System.IO;
using Microsoft.Win32;

using NimbaWPF.Data;
using NimbaWPF.User_Controls;

namespace NimbaWPF
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Enumerations

        public enum ViewEnum
        {
            Home,
            Calendar,
            Statistics,
            CycleStatistics,
            CycleTemperatureStatistics,
        }

        #endregion

        #region Sub-Classes

        public class TabItemData
        {
            #region Properties

            public ViewEnum TabType
            {
                get;
                set;
            }

            public ClosableTabItem Item
            {
                get;
                set;
            }

            public int TabIndex
            {
                get;
                set;
            }

            #endregion

            #region Initializers

            public TabItemData(ViewEnum viewEnum, ClosableTabItem tabItem)
            {
                this.TabType = viewEnum;
                this.Item = tabItem;
                this.TabIndex = -1;
            }

            #endregion

            #region Public Methods

            #endregion
        }

        #endregion

        #region Private Members

        private NimbaDataManager _dataManager = null;

        private Dictionary<int,TabItemData> _tabItemList = null;

        private bool _confirmFilepath = false;

        private string _newFilePrefix = "Document";

        private string _fileExtension = "xml";

        #endregion

        #region Initializers

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                this.AddHandler(ClosableTabItem.CloseTabEvent, new RoutedEventHandler(this.RemoveTab));

                _tabItemList = new Dictionary<int, TabItemData>();

                _dataManager = new NimbaDataManager();

                EnableMenuItems(false);
            }
            catch (Exception exception)
            {
                string message = exception.Message;
            }
        }

        #endregion

        #region Private Methods

        private void CreateNewTab(ViewEnum viewEnum)
        {
            StackPanel stackPanel = new StackPanel();
            TextBlock textBlock = new TextBlock();
            Button button = new Button();
            ClosableTabItem tabItem = new ClosableTabItem();
            Image image = new Image();
            Image imageButton = new Image();
            CalendarViewControl calendarViewControl = null;
            GeneralStatisticsViewControl generalStatisticsViewControl = null;
            CycleStatisticsViewControl cycleStatisticsViewControl = null;
            TabItemData tabItemData = null;

            
            string tabTitle = null;
            string sourcePath = null;

            switch (viewEnum)
            {
                case ViewEnum.Calendar:
                {
                    tabTitle = "Calendar";
                    sourcePath = "pack://application:,,,/NimbaWPF;component/Resources/calendar.png";
                    calendarViewControl = new CalendarViewControl(_dataManager);
                    tabItem.Content = calendarViewControl;
                    break;
                }
                case ViewEnum.Statistics:
                {
                    tabTitle = "Statistics";
                    sourcePath = @"pack://application:,,,/NimbaWPF;component/Resources/calculator.png";
                    generalStatisticsViewControl = new GeneralStatisticsViewControl(_dataManager);
                    tabItem.Content = generalStatisticsViewControl;
                    break;
                }
                case ViewEnum.CycleStatistics:
                {
                    tabTitle = "Cycle Statistics";
                    sourcePath = @"pack://application:,,,/NimbaWPF;component/Resources/graphics.png";
                    cycleStatisticsViewControl = new CycleStatisticsViewControl();
                    tabItem.Content = cycleStatisticsViewControl;
                    break;
                }
                case ViewEnum.CycleTemperatureStatistics:
                {
                    tabTitle = "Cycle Temperature Statistics";
                    sourcePath = @"pack://application:,,,/NimbaWPF;component/Resources/chart_line.png";
                    break;
                }
            }

            image.Source = new BitmapImage(new Uri(sourcePath));
            image.Margin = new Thickness(2, 0, 2, 0);
            textBlock.Text = tabTitle;

            stackPanel.Orientation = Orientation.Horizontal;
            stackPanel.Children.Add(image);
            stackPanel.Children.Add(textBlock);
            //tabItem.IconSource = sourcePath;
            tabItem.Header = stackPanel;

            tabItemData = new TabItemData(viewEnum, tabItem);
            tabItemData.TabIndex = _tabControl.Items.Add(tabItem);

            _tabItemList.Add(tabItemData.TabIndex, tabItemData);

            tabItem.IsSelected = true;
        }

        private void DisplayTab(ViewEnum viewEnum)
        {
            foreach (TabItemData tabItemData in _tabItemList.Values)
            {
                if (tabItemData.TabType == viewEnum)
                {
                    tabItemData.Item.IsSelected = true;
                    return;
                }
            }
            
            CreateNewTab(viewEnum);
        }

        private void EnableMenuItems(bool enable)
        {
            _fileCloseMenuItem.IsEnabled = enable;
            _fileSaveMenuItem.IsEnabled = enable;
            _fileSaveAsMenuItem.IsEnabled = enable;

            _viewCalendarMenuItem.IsEnabled = enable;
            _viewStatisticsMenuItem.IsEnabled = enable;
            _viewCycleStatisticsMenuItem.IsEnabled = enable;
            _viewCycleTemperatureMenuItem.IsEnabled = enable;

            _toolsConfigurationMenuItem.IsEnabled = enable;
        }

        private void RemoveAllTabs()
        {
            _tabControl.Items.Clear();

            _tabItemList.Clear();
        }

        #region File Management

        private void CreateNewFile()
        {
            string filepath = null;

            //
            // Check if a file is already open and if it needs to be saved.
            //
            if (!ConfirmCloseWithoutSaving())
                return;

            //
            // Build the name of the new file.
            //
            filepath = BuildNewFileName();
            
            //
            // Load the file.
            //
            LoadFile(filepath);

        }

        private void OpenFile()
        {
            OpenFileDialog openFileDialog = null;
            bool? dialogResult = null;

            //
            // Check if a file is already open and if it needs to be saved.
            //
            if (!ConfirmCloseWithoutSaving())
                return;

            //
            // Get the filepath of the file to open.
            //
            openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = _fileExtension;
            dialogResult = openFileDialog.ShowDialog();

            if ((!dialogResult.HasValue) || ((dialogResult.HasValue) && (!dialogResult.Value)))
                return;

            //
            // Load the file.
            //
            LoadFile(openFileDialog.FileName);
        }

        private void LoadFile(string filepath)
        {
            //
            // Load the file.
            //
            _dataManager.Load(filepath);

            //
            // Enable Menu items.
            //
            EnableMenuItems(true);

            DisplayTab(ViewEnum.Calendar);

        }

        private string BuildNewFileName()
        {
            //
            // Ensure that the filepath and filename will be confirm before saving.
            //
            _confirmFilepath = true;

            return string.Format(@"{0}\{1}{2}.{3}", _dataManager.UserWorkingFolder, _newFilePrefix, FindNextNewFileIndex(), _fileExtension);
        }

        private bool ConfirmCloseWithoutSaving()
        {
            MessageBoxResult messageBoxResult = MessageBoxResult.Cancel;

            if (_dataManager == null || !_dataManager.DataModified)
                return true;

            messageBoxResult = MessageBox.Show(string.Format("Do you want to save changes to {0} ?", _dataManager.Filepath), "Nimba", MessageBoxButton.YesNoCancel);

            //
            // Cancel close operation.
            //
            if (messageBoxResult == MessageBoxResult.Cancel)
                return false;

            //
            // Close without saving.
            //
            if (messageBoxResult == MessageBoxResult.No)
                return true;

            //
            // Save operation.
            //
            Save(_confirmFilepath);

            return true;
        }

        private bool Save(bool showSaveDialog)
        {
            bool? result = null;
            SaveFileDialog saveFileDialog = null;
            
            if (showSaveDialog)
            {
                saveFileDialog = new SaveFileDialog();
                saveFileDialog.DefaultExt = _fileExtension;
                saveFileDialog.InitialDirectory = _dataManager.UserWorkingFolder;
                saveFileDialog.FileName = _dataManager.Filepath;
                result = saveFileDialog.ShowDialog();

                if (result.HasValue && !result.Value)
                    return false;

                _dataManager.Filepath = saveFileDialog.FileName;

                //
                // Filepath confirmed.
                //
                _confirmFilepath = false;
            }

            //
            // Save data and configuration.
            //
            _dataManager.Save();

            return true;
        }

        private int FindNextNewFileIndex()
        {
            string[] files = null;
            string fileIndex = null;
            int currentIndex = 0;
            List<int> indexes = null;

            files = Directory.GetFiles(_dataManager.UserWorkingFolder, string.Format("{0}*.{1}", _newFilePrefix, _fileExtension));

            if (files.Length == 0)
                return 1;

            indexes = new List<int>();

            foreach (string filename in files)
            {
                fileIndex = filename.Replace(string.Format(@"{0}\{1}", _dataManager.UserWorkingFolder,_newFilePrefix), "");
                fileIndex = fileIndex.Replace(string.Format(".{0}", _fileExtension), "");

                if (!int.TryParse(fileIndex, out currentIndex))
                    continue;

                indexes.Add(currentIndex);
            }

            for (int i = 1; i < 100; ++i)
            {
                if (!indexes.Contains(i))
                    return i;
            }

            return 1;
        }

        private bool CloseFile()
        {
            if (!ConfirmCloseWithoutSaving())
                return false;

            RemoveAllTabs();

            EnableMenuItems(false);

            _dataManager.Clear();

            return true;
        }

        #endregion

        #endregion

        #region Menu Event Handling

        private void FileNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CreateNewFile();
            }
            catch (Exception exception)
            {
                string message = exception.Message;
            }
        }

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFile();
            }
            catch (Exception exception)
            {
                string message = exception.Message;
            }
        }

        private void FileClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CloseFile();
            }
            catch (Exception exception)
            {
                string message = exception.Message;
            }
        }

        private void FileSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Save(_confirmFilepath);
            }
            catch (Exception exception)
            {
                string message = exception.Message;
            }
        }

        private void FileSaveAs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Save(true);
            }
            catch (Exception exception)
            {
                string message = exception.Message;
            }
        }

        private void FileQuit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CloseFile();

                this.Close();
            }
            catch (Exception exception)
            {
                string message = exception.Message;
            }
        }

        private void ViewCalendarView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DisplayTab(ViewEnum.Calendar);
            }
            catch (Exception exception)
            {
                string message = exception.Message;
            }
        }

        private void ViewStatisticsView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DisplayTab(ViewEnum.Statistics);
            }
            catch (Exception exception)
            {
                string message = exception.Message;
            }
        }

        private void ViewCycleStatisticsView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DisplayTab(ViewEnum.CycleStatistics);
            }
            catch (Exception exception)
            {
                string message = exception.Message;
            }
        }

        private void ViewCycleTemperature_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DisplayTab(ViewEnum.CycleTemperatureStatistics);
            }
            catch (Exception exception)
            {
                string message = exception.Message;
            }
        }

        private void ToolsConfiguration_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            }
            catch (Exception exception)
            {
                string message = exception.Message;
            }
        }

        private void HelpAbout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            }
            catch (Exception exception)
            {
                string message = exception.Message;
            }
        }

        private void HelpHelp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            }
            catch (Exception exception)
            {
                string message = exception.Message;
            }

        }

        private void Window_Closing(object sender, EventArgs e)
        {
            try
            {
                CloseFile();
            }
            catch (Exception exception)
            {
                string message = exception.Message;
            }
        }

        private void RemoveTab(object sender, RoutedEventArgs e)
        {
            try
            {
                TabItem tabItem = e.Source as TabItem;

                if (tabItem != null)
                {
                    TabControl tabControl = tabItem.Parent as TabControl;

                    if (_tabItemList.ContainsKey(tabControl.SelectedIndex))
                        _tabItemList.Remove(tabControl.SelectedIndex);

                    if (tabControl != null)
                    {
                        tabControl.Items.Remove(tabItem);
                    }
                }
            }
            catch (Exception exception)
            {
                string message = exception.Message;
            }
        }
        
        #endregion
    }
}
