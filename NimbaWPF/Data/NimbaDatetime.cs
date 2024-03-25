using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Globalization;

namespace NimbaWPF.Data
{
    public class NimbaDatetime:IComparable
    {
        #region Private Members

        private bool _isPeriod = false;

        private bool _isOvulation = false;

        private bool _isLiquidMucus = false;

        private bool _startPregnancy = false;

        private bool _endPregnancy = false;

        private double? _temperature = null;

        #endregion

        #region Properties

        public DateTime Date
        {
            set;
            get;
        }

        public bool IsPeriod
        {
            set
            {
                _isPeriod = value;
                this.Modified = true;
            }
            get
            {
                return _isPeriod;
            }
        }

        public bool IsOvulation
        {
            set
            {
                _isOvulation = value;
                this.Modified = true;
            }
            get
            {
                return _isOvulation;
            }
        }

        public bool IsNextPeriod
        {
            set;
            get;
        }

        public bool IsNextOvulation
        {
            set;
            get;
        }
        
        public bool IsLiquidMucus
        {
            set
            {
                _isLiquidMucus = value;
                this.Modified = true;
            }
            get
            {
                return _isLiquidMucus;
            }
        }

        public bool IsPregnant
        {
            set;
            get;
        }

        public bool StartPregnancy
        {
            set
            {
                _startPregnancy = value;
                this.Modified = true;
            }
            get
            {
                return _startPregnancy;
            }
        }

        public bool EndPregnancy
        {
            set
            {
                _endPregnancy = value;
                this.Modified = true;
            }
            get
            {
                return _endPregnancy;                
            }
        }
        
        public double? Temperature
        {
            set
            {
                _temperature = value;
                this.Modified = true;
            }
            get
            {
                return _temperature;
            }
        }

        public bool Modified
        {
            set;
            get;
        }

        #endregion

        #region Initializers

        public NimbaDatetime()
        {
            this.Date = DateTime.Now;
            this.IsNextPeriod = false;
            this.IsNextOvulation = false;
        }

        public NimbaDatetime(DateTime datetime)
        {
            this.Date = datetime;
            this.IsNextPeriod = false;
            this.IsNextOvulation = false;
        }
        
        #endregion

        #region Public Methods

        public int CompareTo(object obj)
        {
            return this.Date.CompareTo(((NimbaDatetime)obj).Date);
        }

        public bool IsSavable()
        {
            return (IsOvulation || IsPeriod || IsLiquidMucus || StartPregnancy || EndPregnancy || (Temperature.HasValue));
        }

        #endregion
    }

    public class NimbaDatetimeSortedSet : SortedSet<NimbaDatetime>
    {
        #region Constants

        private const string _nimbaDatetimeCollection = "NimbaDatetimeCollection";

        private const string _nimbaDatetime = "NimbaDatetime";

        private const string _date = "Date";

        private const string _isPeriod = "IsPeriod";

        private const string _isOvulation = "IsOvulation";

        private const string _isLiquidMucus = "IsLiquidMucus";

        private const string _startPregnancy = "StartPregnancy";

        private const string _endPregnancy = "EndPregnancy";

        private const string _temperature = "Temperature";

        #endregion

        #region Initializers

        #endregion

        #region Private Methods

        private XmlDocument CreateFile()
        {
            XmlDocument document = null;
            XmlNode configurationNode = null;

            document = new XmlDocument();

            document.CreateXmlDeclaration("1.0", "utf-8", null);

            configurationNode = document.CreateElement(_nimbaDatetimeCollection);
            document.AppendChild(configurationNode);

            return document;
        }

        private void InsertAttribute(XmlDocument document, XmlNode node, string attributeName, string attributeValue)
        {
            XmlAttribute attribute = null;

            attribute = document.CreateAttribute(attributeName);
            attribute.Value = attributeValue;
            node.Attributes.Append(attribute);
        }

        private bool HasAttribute(XmlNode node, string attributeName)
        {
            XmlNode attribute = null;

            attribute = node.SelectSingleNode(string.Format("@{0}", attributeName));

            if (attribute == null)
                return false;

            return !string.IsNullOrEmpty(attribute.Value);
        }

        private void ResetModified()
        {
            foreach (NimbaDatetime nimbaDatetime in this)
                nimbaDatetime.Modified = false;
        }

        #endregion

        #region Public Methods

        public void Load(string filepath)
        {
            XmlDocument document = null;
            XmlNodeList dates = null;
            NimbaDatetime date = null;
            DateTimeFormatInfo dateFormatInfo = null;
            NumberFormatInfo numberFormatInfo = null;

            try
            {
                document = new XmlDocument();

                if (!File.Exists(filepath))
                    document = CreateFile();
                else
                    document.Load(filepath);

                this.Clear();

                dates = document.GetElementsByTagName(_nimbaDatetime);

                foreach (XmlNode nodeDate in dates)
                {
                    date = new NimbaDatetime();

                    if (HasAttribute(nodeDate, _date))
                    {
                        dateFormatInfo = new DateTimeFormatInfo();
                        dateFormatInfo.ShortDatePattern = "dd/MM/yyyy";
                        date.Date = Convert.ToDateTime(nodeDate.Attributes[_date].Value, dateFormatInfo);
                    }

                    if (HasAttribute(nodeDate, _isPeriod))
                        date.IsPeriod = true;

                    if (HasAttribute(nodeDate, _isOvulation))
                        date.IsOvulation = true;

                    if (HasAttribute(nodeDate, _isLiquidMucus))
                        date.IsLiquidMucus = true;

                    if (HasAttribute(nodeDate, _startPregnancy))
                        date.StartPregnancy = true;

                    if (HasAttribute(nodeDate, _endPregnancy))
                        date.EndPregnancy = true;

                    if (HasAttribute(nodeDate, _temperature))
                    {
                        numberFormatInfo = new NumberFormatInfo();
                        numberFormatInfo.NumberDecimalSeparator = ".";
                        date.Temperature = Convert.ToDouble(nodeDate.Attributes[_temperature].Value, numberFormatInfo);
                    }

                    this.Add(date);
                }

                ResetModified();
            }
            catch (Exception e)
            {
                string message = e.Message;
            }
        }

        public void Save(string filepath)
        {
            XmlDocument document = null;
            XmlNode rootNode = null;
            XmlElement elementDate = null;
            DateTimeFormatInfo formatInfo = null;
            NumberFormatInfo numberFormatInfo = null;

            try
            {
                document = new XmlDocument();

                if (!File.Exists(filepath))
                    document = CreateFile();
                else
                    document.Load(filepath);

                rootNode = document.GetElementsByTagName(_nimbaDatetimeCollection)[0];

                rootNode.RemoveAll();

                foreach (NimbaDatetime date in this)
                {
                    if (!date.IsSavable())
                        continue;

                    //
                    // NimbaDatetime node.
                    //
                    elementDate = document.CreateElement(_nimbaDatetime);
                    rootNode.AppendChild(elementDate);

                    //
                    // Date attribute.
                    //
                    formatInfo = new DateTimeFormatInfo();
                    formatInfo.ShortDatePattern = "dd/MM/yyyy";
                    InsertAttribute(document, elementDate, _date, date.Date.ToString(formatInfo));
                    
                    //
                    // IsPeriod attribute.
                    //
                    if (date.IsPeriod)
                        InsertAttribute(document, elementDate, _isPeriod, true.ToString());

                    //
                    // IsOvulation attribute.
                    //
                    if (date.IsOvulation)
                        InsertAttribute(document, elementDate, _isOvulation, true.ToString());

                    //
                    // IsLiquidMucus attribute.
                    //
                    if (date.IsLiquidMucus)
                        InsertAttribute(document, elementDate, _isLiquidMucus, true.ToString());

                    //
                    // StartPregnancy attribute.
                    //
                    if (date.StartPregnancy)
                        InsertAttribute(document, elementDate, _startPregnancy, true.ToString());

                    //
                    // StartPregnancy attribute.
                    //
                    if (date.EndPregnancy)
                        InsertAttribute(document, elementDate, _endPregnancy, true.ToString());

                    //
                    // Temperature attribute.
                    //
                    if (date.Temperature.HasValue)
                    {
                        numberFormatInfo = new NumberFormatInfo();
                        numberFormatInfo.NumberDecimalSeparator = ".";
                        InsertAttribute(document, elementDate, _temperature, date.Temperature.Value.ToString(numberFormatInfo));
                    }
                }

                document.Save(filepath);

                ResetModified();
            }
            catch (Exception e)
            {
                string message = e.Message;
            }
        }

        public bool Find(DateTime datetime, out NimbaDatetime nimbaDatetime)
        {
            //
            // Initialize out parameters.
            //
            nimbaDatetime = null;

            try
            {
                foreach (NimbaDatetime date in this)
                {
                    if (date.Date.Equals(datetime))
                    {
                        nimbaDatetime = date;
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return false;
        }

        public bool IsModified()
        {
            try
            {
                foreach (NimbaDatetime nimbaDatetime in this)
                {
                    if (nimbaDatetime.Modified)
                        return true;
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return false;
        }

        #endregion
    }
}
