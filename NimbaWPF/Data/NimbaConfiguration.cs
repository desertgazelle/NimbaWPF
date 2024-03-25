using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace NimbaWPF.Data
{
    public enum OvulationComputationMode
    {
        Default, 
        LiquidMucus, 
        Temperature
    }

    public enum ParametersSelection
    {
        UseDefaultParameters, 
        UseStatistics
    }

    public enum DegreeUnit
    {
        Celsius, 
        Farenheit
    }

    public enum Language
    {
        English, 
        French
    }

    public class NimbaConfiguration
    {
        #region Constants

        private static string _nimba = "Nimba";

        private static string _executable = "NimbaConfiguration.xml";

        private static string _configuration = "configuration";

        private static string _application = "Application";

        private static string _nimbaConfiguration = "NimbaConfiguration";

        private static string _workingFolder = "WorkingFolder";

        private static string _degreeUnit = "DegreeUnit";

        private static string _firstDayOfWeek = "FirstDayOfWeek";

        private static string _language = "Language";

        private static string _cycleDuration = "CycleDuration";

        private static string _periodDuration = "PeriodDuration";

        private static string _nbDaysBeforeOvulation = "NbDaysBeforeOvulation";

        private static string _nbDaysBeforeLiquidMucus = "NbDaysBeforeLiquidMucus";

        private static string _nbDaysBetweenLiquidMucusAndOvulation = "NbDaysBetweenLiquidMucusAndOvulation";

        private static string _maxNormalCycleDuration = "MaxNormalCycleDuration";

        private static string _maxPregnancyCycleDuration = "MaxPregnancyCycleDuration";

        private static string _ovulationComputationMode = "OvulationComputationMode";

        private static string _parametersSelection = "ParametersSelection";

        private static string _user = "User";

        private static string _default = "Default";

        private static DegreeUnit _defaultDegreeUnit = DegreeUnit.Celsius;

        private static DayOfWeek _defaultFirstDayOfWeek = DayOfWeek.Sunday;

        private static Language _defaultLanguage = Language.English;

        private static int _defaultCycleDuration = 28;

        private static int _defaultPeriodDuration = 5;

        private static int _defaultNbDaysBeforeOvulation = 14;

        private static int _defaultNbDaysBeforeLiquidMucus = 16;

        private static int _defaultNbDaysBetweenLiquidMucusAndOvulation = 2;

        private static int _defaultMaxNormalCycleDuration = 36;

        private static int _defaultMaxPregnancyCycleDuration = 300;

        private static OvulationComputationMode _defaultOvulationComputationMode = OvulationComputationMode.Default;

        private static ParametersSelection _defaultParametersSelection = ParametersSelection.UseStatistics;

        #endregion

        #region Properties

        public static string DefaultWorkingFolder
        {
            get;
            set;
        }

        public static string DefaultIconsFolder
        {
            get;
            set;
        }

        public static DegreeUnit DefaultDegreeUnit
        {
            get;
            set;
        }

        public static DayOfWeek DefaultFirstDayOfWeek
        {
            get;
            set;
        }

        public static Language DefaultLanguage
        {
            get;
            set;
        }

        public static int DefaultCycleDuration
        {
            get;
            set;
        }

        public static int DefaultMaxNormalCycleDuration
        {
            get;
            set;
        }

        public static int DefaultMaxPregnancyCycleDuration
        {
            get;
            set;
        }

        public static int DefaultPeriodDuration
        {
            get;
            set;
        }

        public static int DefaultNbDaysBeforeOvulation
        {
            get;
            set;
        }

        public static int DefaultNbDaysBeforeLiquidMucus
        {
            get;
            set;
        }

        public static int DefaultNbDaysBetweenLiquidMucusAndOvulation
        {
            get;
            set;
        }

        public static ParametersSelection DefaultParametersSelection
        {
            get;
            set;
        }

        public static OvulationComputationMode DefaultOvulationComputationMode
        {
            get;
            set;
        }

        public string UserWorkingFolder
        {
            get;
            set;
        }

        public string UserIconsFolder
        {
            get;
            set;
        }

        public DegreeUnit UserDegreeUnit
        {
            get;
            set;
        }

        public DayOfWeek UserFirstDayOfWeek
        {
            get;
            set;
        }

        public Language UserLanguage
        {
            get;
            set;
        }

        public int UserCycleDuration
        {
            get;
            set;
        }

        public int UserMaxNormalCycleDuration
        {
            get;
            set;
        }

        public int UserMaxPregnancyCycleDuration
        {
            get;
            set;
        }

        public int UserPeriodDuration
        {
            get;
            set;
        }

        public int UserNbDaysBeforeOvulation
        {
            get;
            set;
        }

        public int UserNbDaysBeforeLiquidMucus
        {
            get;
            set;
        }

        public int UserNbDaysBetweenLiquidMucusAndOvulation
        {
            get;
            set;
        }

        public ParametersSelection UserParametersSelection
        {
            get;
            set;
        }

        public OvulationComputationMode UserOvulationComputationMode
        {
            get;
            set;
        }

        #endregion

        #region Initializers

        public NimbaConfiguration()
        {
            LoadUserSettings();
        }

        #endregion

        #region Private Methods

        private static XmlDocument CreateConfigurationFile()
        {
            XmlDocument document = null;
            XmlNode configurationNode = null;

            document = new XmlDocument();

            document.CreateXmlDeclaration("1.0", "utf-8", null);

            configurationNode = document.CreateElement(_configuration);
            document.AppendChild(configurationNode);

            return document;
        }
        
        private static bool HasAttribute(XmlNode node, string attributeName)
        {
            XmlNode attribute = null;

            attribute = node.SelectSingleNode(string.Format("@{0}", attributeName));

            if (attribute == null)
                return false;

            return !string.IsNullOrEmpty(attribute.Value);
        }

        private static void InsertAttribute(XmlDocument document, XmlNode parentNode, string attributePrefixName, string attributeName, string attributeValue)
        {
            XmlAttribute attribute = null;

            attribute = document.CreateAttribute(string.Format("{0}{1}", attributePrefixName, attributeName));
            attribute.Value = attributeValue;
            parentNode.Attributes.Append(attribute);
        }

        private static string GetAttribute(XmlDocument document, XmlNode parentNode, string attributePrefixName, string attributeName, string defaultValue)
        {
            string attributeValue = string.Empty;

            if (HasAttribute(parentNode, string.Format("{0}{1}", attributePrefixName, attributeName)))
                attributeValue = parentNode.Attributes[string.Format("{0}{1}", attributePrefixName, attributeName)].Value;
            else
            {
                attributeValue = defaultValue;

                InsertAttribute(document, parentNode, attributePrefixName, attributeName, attributeValue.ToString());
            }                

            return attributeValue;
        }

        private static int GetAttribute(XmlDocument document, XmlNode parentNode, string attributePrefixName, string attributeName, int defaultValue)
        {
            int attributeValue = 0;

            if (HasAttribute(parentNode, string.Format("{0}{1}", attributePrefixName, attributeName)))
                attributeValue = Convert.ToInt32(parentNode.Attributes[string.Format("{0}{1}", _default, attributeName)].Value);
            else
            {
                attributeValue = defaultValue;

                InsertAttribute(document, parentNode, _default, attributeName, attributeValue.ToString());
            }                

            return attributeValue;
        }

        private static T GetAttribute<T>(XmlDocument document, XmlNode parentNode, string attributePrefixName, string attributeName, T defaultValue) where T : struct
        {
            T attributeValue = defaultValue;

            if (HasAttribute(parentNode, string.Format("{0}{1}", attributePrefixName, attributeName)))
            {
                if (!Enum.TryParse<T>(parentNode.Attributes[string.Format("{0}{1}", attributePrefixName, attributeName)].Value, out attributeValue))
                    attributeValue = defaultValue;
            }
            else
            {
                attributeValue = defaultValue;

                InsertAttribute(document, parentNode, attributePrefixName, attributeName, attributeValue.ToString());
            }

            return attributeValue;
        }

        private void LoadUserSettings()
        {
            XmlDocument document = null;
            XmlNode nodeUserApplication = null;
            XmlNode nodeUserNimbaConfiguration = null;

            string configurationFile = null;

            try
            {
                configurationFile = string.Format(@"{0}\{1}", Environment.CurrentDirectory, NimbaConfiguration._executable);
               
                if (File.Exists(configurationFile))
                {
                    document = new XmlDocument();
                    document.Load(configurationFile);
                }
                else
                    document = CreateConfigurationFile();                                

                //
                // User Application.
                //
                nodeUserApplication = document.GetElementsByTagName(string.Format("{0}{1}", _user, _application))[0];

                if (nodeUserApplication == null)
                {
                    nodeUserApplication = document.CreateElement(string.Format("{0}{1}", _user, _application));
                    document.GetElementsByTagName(_configuration)[0].AppendChild(nodeUserApplication);
                }

                //
                // User Working Folder.
                //
                this.UserWorkingFolder = GetAttribute(document, nodeUserApplication, _user, _workingFolder, NimbaConfiguration.DefaultWorkingFolder);

                //
                // User Language.
                //
                this.UserLanguage = GetAttribute<Language>(document, nodeUserApplication, _user, _language, NimbaConfiguration.DefaultLanguage);

                //
                // User Degree Unit.
                //
                this.UserDegreeUnit = GetAttribute<DegreeUnit>(document, nodeUserApplication, _user, _degreeUnit, NimbaConfiguration.DefaultDegreeUnit);

                //
                // User First Day Of Week.
                //
                this.UserFirstDayOfWeek = GetAttribute<DayOfWeek>(document, nodeUserApplication, _user, _firstDayOfWeek, NimbaConfiguration.DefaultFirstDayOfWeek);

                //
                // User NimbaConfigurationnode.
                //
                nodeUserNimbaConfiguration = document.GetElementsByTagName(string.Format("{0}{1}", _user, _nimbaConfiguration))[0];

                if (nodeUserNimbaConfiguration == null)
                {
                    nodeUserNimbaConfiguration = document.CreateElement(string.Format("{0}{1}", _user, _nimbaConfiguration));
                    document.GetElementsByTagName(_configuration)[0].AppendChild(nodeUserNimbaConfiguration);
                }

                //
                // User Cycle Duration.
                //
                this.UserCycleDuration = GetAttribute(document, nodeUserNimbaConfiguration, _user, _cycleDuration, NimbaConfiguration.DefaultCycleDuration);

                //
                // User Period Duration.
                //
                this.UserPeriodDuration = GetAttribute(document, nodeUserNimbaConfiguration, _user, _periodDuration, NimbaConfiguration.DefaultPeriodDuration);

                //
                // User Number of days before Ovulation.
                //
                this.UserNbDaysBeforeOvulation = GetAttribute(document, nodeUserNimbaConfiguration, _user, _nbDaysBeforeOvulation, NimbaConfiguration.DefaultNbDaysBeforeOvulation);

                //
                // User Number of days before Liquid Mucus.
                //
                this.UserNbDaysBeforeLiquidMucus = GetAttribute(document, nodeUserNimbaConfiguration, _user, _nbDaysBeforeLiquidMucus, NimbaConfiguration.DefaultNbDaysBeforeLiquidMucus);

                //
                // User Number of days between Liquid Mucus and Ovulation.
                //
                this.UserNbDaysBetweenLiquidMucusAndOvulation = GetAttribute(document, nodeUserNimbaConfiguration, _user, _nbDaysBetweenLiquidMucusAndOvulation, NimbaConfiguration.DefaultNbDaysBetweenLiquidMucusAndOvulation);

                //
                // User Max Normal Cycle Duration.
                //
                this.UserMaxNormalCycleDuration = GetAttribute(document, nodeUserNimbaConfiguration, _user, _maxNormalCycleDuration, NimbaConfiguration.DefaultMaxNormalCycleDuration);

                //
                // User Max Pregnancy Cycle Duration.
                //
                this.UserMaxPregnancyCycleDuration = GetAttribute(document, nodeUserNimbaConfiguration, _user, _maxPregnancyCycleDuration, NimbaConfiguration.DefaultMaxPregnancyCycleDuration);

                //
                // User Ovulation Computation Mode.
                //
                this.UserOvulationComputationMode = GetAttribute<OvulationComputationMode>(document, nodeUserNimbaConfiguration, _user, _ovulationComputationMode, NimbaConfiguration.DefaultOvulationComputationMode);

                //
                // User Parameter Selection.
                //
                this.UserParametersSelection = GetAttribute<ParametersSelection>(document, nodeUserNimbaConfiguration, _user, _parametersSelection, NimbaConfiguration.DefaultParametersSelection);

                document.Save(configurationFile);
            }
            catch (Exception e)
            {
                string message = e.Message;
            }
        }

        #endregion

        #region Public Methods

        public static void LoadDefaultSettings()
        {
            XmlDocument document = null;
            XmlNode nodeDefaultApplication = null;
            XmlNode nodeDefaultNimbaConfiguration = null;

            string configurationFile = null;

            try
            {
                configurationFile = string.Format(@"{0}\{1}", Environment.CurrentDirectory, NimbaConfiguration._executable);

                document = new XmlDocument();

                if (File.Exists(configurationFile))
                {
                    document = new XmlDocument();
                    document.Load(configurationFile);
                }
                else
                    document = CreateConfigurationFile();   

                //
                // Default Application.
                //
                nodeDefaultApplication = document.GetElementsByTagName(string.Format("{0}{1}", _default, _application))[0];

                if (nodeDefaultApplication == null)
                {
                    nodeDefaultApplication = document.CreateElement(string.Format("{0}{1}", _default, _application));
                    document.GetElementsByTagName(_configuration)[0].AppendChild(nodeDefaultApplication);
                }

                //
                // Default Working Folder.
                //
                NimbaConfiguration.DefaultWorkingFolder = GetAttribute(document, nodeDefaultApplication, _default, _workingFolder, System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), _nimba));

                //
                // Default Language.
                //
                NimbaConfiguration.DefaultLanguage = GetAttribute<Language>(document, nodeDefaultApplication, _default, _language, _defaultLanguage);

                //
                // Default Degree Unit.
                //
                NimbaConfiguration.DefaultDegreeUnit = GetAttribute<DegreeUnit>(document, nodeDefaultApplication, _default, _degreeUnit, _defaultDegreeUnit);

                //
                // Default First Day Of Week.
                //
                NimbaConfiguration.DefaultFirstDayOfWeek = GetAttribute<DayOfWeek>(document, nodeDefaultApplication, _default, _firstDayOfWeek, _defaultFirstDayOfWeek);

                //
                // Default NimbaConfigurationnode.
                //
                nodeDefaultNimbaConfiguration = document.GetElementsByTagName(string.Format("{0}{1}", _default, _nimbaConfiguration))[0];

                if (nodeDefaultNimbaConfiguration == null)
                {
                    nodeDefaultNimbaConfiguration = document.CreateElement(string.Format("{0}{1}", _default, _nimbaConfiguration));
                    document.GetElementsByTagName(_configuration)[0].AppendChild(nodeDefaultNimbaConfiguration);
                }

                //
                // Default Cycle Duration.
                //
                NimbaConfiguration.DefaultCycleDuration = GetAttribute(document, nodeDefaultNimbaConfiguration, _default, _cycleDuration, _defaultCycleDuration);

                //
                // Default Period Duration.
                //
                NimbaConfiguration.DefaultPeriodDuration = GetAttribute(document, nodeDefaultNimbaConfiguration, _default, _periodDuration, _defaultPeriodDuration);

                //
                // Default Number of days before Ovulation.
                //
                NimbaConfiguration.DefaultNbDaysBeforeOvulation = GetAttribute(document, nodeDefaultNimbaConfiguration, _default, _nbDaysBeforeOvulation, _defaultNbDaysBeforeOvulation);

                //
                // Default Number of days before Liquid Mucus.
                //
                NimbaConfiguration.DefaultNbDaysBeforeLiquidMucus = GetAttribute(document, nodeDefaultNimbaConfiguration, _default, _nbDaysBeforeLiquidMucus, _defaultNbDaysBeforeLiquidMucus);

                //
                // Default Number of days between Liquid Mucus and Ovulation.
                //
                NimbaConfiguration.DefaultNbDaysBetweenLiquidMucusAndOvulation = GetAttribute(document, nodeDefaultNimbaConfiguration, _default, _nbDaysBetweenLiquidMucusAndOvulation, _defaultNbDaysBetweenLiquidMucusAndOvulation);

                //
                // Default Max Normal Cycle Duration.
                //
                NimbaConfiguration.DefaultMaxNormalCycleDuration = GetAttribute(document, nodeDefaultNimbaConfiguration, _default, _maxNormalCycleDuration, _defaultMaxNormalCycleDuration);

                //
                // Default Max Pregnancy Cycle Duration.
                //
                NimbaConfiguration.DefaultMaxPregnancyCycleDuration = GetAttribute(document, nodeDefaultNimbaConfiguration, _default, _maxPregnancyCycleDuration, _defaultMaxPregnancyCycleDuration);

                //
                // Default Ovulation Computation Mode.
                //
                NimbaConfiguration.DefaultOvulationComputationMode = GetAttribute<OvulationComputationMode>(document, nodeDefaultNimbaConfiguration, _default, _ovulationComputationMode, _defaultOvulationComputationMode);

                //
                // Default Parameter Selection.
                //
                NimbaConfiguration.DefaultParametersSelection = GetAttribute<ParametersSelection>(document, nodeDefaultNimbaConfiguration, _default, _parametersSelection, _defaultParametersSelection);

                document.Save(configurationFile);
            }
            catch (Exception e)
            {
                string message = e.Message;
            }
        }

        public void Save()
        {
            XmlDocument document = null;
            XmlNode nodeApplication = null;
            XmlNode nodeNimbaConfiguration = null;

            try
            {
                document = new XmlDocument();
                document.Load(string.Format(@"{0}\{1}", Environment.CurrentDirectory, NimbaConfiguration._executable));

                //
                // UserApplication node.
                //
                nodeApplication = document.CreateElement(string.Format("{0}{1}",_user, _application));
                document.GetElementsByTagName(_configuration)[0].AppendChild(nodeApplication);

                //
                // User Working Folder.
                //
                InsertAttribute(document, nodeApplication, _user, _workingFolder, this.UserWorkingFolder);

                //
                // User Language.
                //
                InsertAttribute(document, nodeApplication, _user, _language, this.UserLanguage.ToString());

                //
                // User Degree Unit.
                //
                InsertAttribute(document, nodeApplication, _user, _degreeUnit, this.UserDegreeUnit.ToString());

                //
                // User First Day of Week.
                //
                InsertAttribute(document, nodeApplication, _user, _firstDayOfWeek, this.UserFirstDayOfWeek.ToString());

                //
                // UserNimbaConfiguration node.
                //
                nodeNimbaConfiguration = document.CreateElement(string.Format("{0}{1}", _user, _nimbaConfiguration));
                document.GetElementsByTagName(_configuration)[0].AppendChild(nodeNimbaConfiguration);

                //
                // User Cycle Duration.
                //
                InsertAttribute(document, nodeNimbaConfiguration, _user, _cycleDuration, this.UserCycleDuration.ToString());

                //
                // User Period Duration.
                //
                InsertAttribute(document, nodeNimbaConfiguration, _user, _periodDuration, this.UserPeriodDuration.ToString());

                //
                // User Number of days before ovulation.
                //
                InsertAttribute(document, nodeNimbaConfiguration, _user, _nbDaysBeforeOvulation, this.UserNbDaysBeforeOvulation.ToString());

                //
                // User Number of days before liquid mucus.
                //
                InsertAttribute(document, nodeNimbaConfiguration, _user, _nbDaysBeforeLiquidMucus, this.UserNbDaysBeforeLiquidMucus.ToString());

                //
                // User Number of days between liquid mucus and ovulation.
                //
                InsertAttribute(document, nodeNimbaConfiguration, _user, _nbDaysBetweenLiquidMucusAndOvulation, this.UserNbDaysBetweenLiquidMucusAndOvulation.ToString());

                //
                // User Ovulation Computation mode.
                //
                InsertAttribute(document, nodeNimbaConfiguration, _user, _ovulationComputationMode, this.UserOvulationComputationMode.ToString());

                //
                // User Parameter Selection.
                //
                InsertAttribute(document, nodeNimbaConfiguration, _user, _parametersSelection, this.UserParametersSelection.ToString());

                document.Save(string.Format(@"{0}\{1}", Environment.CurrentDirectory, NimbaConfiguration._executable));
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

        }

        #endregion
    }
}
