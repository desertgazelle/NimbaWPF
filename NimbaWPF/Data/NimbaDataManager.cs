using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NimbaWPF.Data
{
    public class NimbaDataManager
    {
        #region Properties

        private NimbaConfiguration Configuration
        {
            get;
            set;
        }

        private NimbaDatetimeSortedSet Dates
        {
            get;
            set;
        }

        private NimbaCycleSortedSet Cycles
        {
            get;
            set;
        }

        public NimbaCycle CurrentCycle
        {
            get;
            set;
        }

        public Dictionary<CycleType, Dictionary<StatisticValue, NimbaCycleStatisticData>> Statistics
        {
            get
            {
                if (Cycles != null)
                    return Cycles.Statistics;
                else
                    return null;
            }
        }

        public Language UserLanguage
        {
            get
            {
                return this.Configuration.UserLanguage;
            }
        }

        public string UserWorkingFolder
        {
            get
            {
                return this.Configuration.UserWorkingFolder;
            }
        }

        public DayOfWeek UserFirstDayOfWeek
        {
            get
            {
                return this.Configuration.UserFirstDayOfWeek;
            }
        }

        public DegreeUnit UserDegreeUnit
        {
            get
            {
                return this.Configuration.UserDegreeUnit;
            }
        }

        public bool DataModified
        {
            get
            {
                return this.Dates.IsModified();
            }
        }

        public string Filepath
        {
            get;
            set;
        }

        #endregion

        #region Initializers

        public NimbaDataManager()
        {
            NimbaConfiguration.LoadDefaultSettings();

            Configuration = new NimbaConfiguration();

            Cycles = new NimbaCycleSortedSet();

            CurrentCycle = null;

            Dates = new NimbaDatetimeSortedSet();
        }

        #endregion

        #region Public Methods

        public void Load(string filepath)
        {
            try
            {
                this.Dates.Load(filepath);
                this.Cycles.Load(this.Dates, this.Configuration);

                this.Filepath = filepath;
            }
            catch (Exception e)
            {
                string message = e.Message;
            }
        }

        public void Save()
        {
            try
            {
                //
                // Save data.
                //
                Dates.Save(this.Filepath);

                //
                // Save configuration.
                //
                Configuration.Save();
            }
            catch (Exception e)
            {
                string message = e.Message;
            }
        }

        public void Clear()
        {
            try
            {
                Dates.Clear();

                Cycles.Clear();
            }
            catch (Exception e)
            {
                string message = e.Message;
            }
        }

        public void AddDate(DateTime datetime, bool rebuild)
        {
            NimbaDatetime nimbaDatetime = null;

            try
            {
                if (this.Dates.Find(datetime, out nimbaDatetime))
                    return;

                this.Dates.Add(new NimbaDatetime(datetime));

                if (rebuild)
                    this.Cycles.Load(this.Dates, this.Configuration);
            }
            catch (Exception e)
            {
                string message = e.Message;
            }
        }

        public bool FindDate(DateTime datetime, bool addIfAbsent, out NimbaDatetime nimbaDatetime)
        {
            //
            // Initialize out parameters.
            //
            nimbaDatetime = null;

            try
            {
                if (this.Dates.Find(datetime, out nimbaDatetime))
                    return true;
                else if (addIfAbsent)
                {
                    nimbaDatetime = new NimbaDatetime(datetime);
                    this.Dates.Add(nimbaDatetime);

                    return true;
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return false;
        }

        public bool FindCycle(DateTime datetime, out NimbaCycle nimbaCycle)
        {
            //
            // Initialize out parameters.
            //
            nimbaCycle = null;

            try
            {
                return this.Cycles.Find(datetime, out nimbaCycle);
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return false;
        }

        public void LoadCycle()
        {
            try
            {
                this.Cycles.Load(this.Dates, this.Configuration);
            }
            catch (Exception e)
            {
                string message = e.Message;
            }
        }

        public static double CelsiusToFarenheit(double pCelsius)
        {
            return (pCelsius * 9 / 5) + 32.0;
        }

        public static double FarenheitToCelsius(double pFarenheit)
        {
            return (pFarenheit - 32) * 5 / 9;
        }
        
        #endregion
    }
}
