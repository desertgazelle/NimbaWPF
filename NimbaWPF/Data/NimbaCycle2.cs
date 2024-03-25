using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NimbaWPF.Data
{
    public enum CycleType
    {
        None,
        Normal,
        Pregnancy,
        Transition,
    }

    public class NimbaCycleStatisticData
    {
        #region Properties

        public double CycleDuration
        {
            get;
            set;
        }

        public double PeriodDuration
        {
            get;
            set;
        }

        public double NbDaysBeforeOvulation
        {
            get;
            set;
        }

        public double NbDaysBeforeLiquidMucus
        {
            get;
            set;
        }

        public double NbDaysBtwLiquidMucusOvulation
        {
            get;
            set;
        }

        public double TemperatureOvulation
        {
            get;
            set;
        }

        public double Temperature
        {
            get;
            set;
        }

        #endregion

        #region Initializers

        public NimbaCycleStatisticData()
        {
            this.CycleDuration = 0.0;
            this.PeriodDuration = 0.0;
            this.NbDaysBeforeOvulation = 0.0;
            this.NbDaysBeforeLiquidMucus = 0.0;
            this.NbDaysBtwLiquidMucusOvulation = 0.0;
            this.TemperatureOvulation = 0.0;
            this.Temperature = 0.0;
        }

        #endregion
    }

    public class NimbaCycle:IComparable
    {
        #region Properties

        public CycleType Type
        {
            get;
            set;
        }

        public NimbaDatetime FirstDay
        {
            get;
            set;
        }

        public NimbaDatetime LastDay
        {
            get;
            set;
        }

        public NimbaDatetime OvulationDay
        {
            get;
            set;
        }

        public NimbaDatetime FirstLiquidMucusDay
        {
            get;
            set;
        }
        
        public int NbDays
        {
            get;
            set;
        }

        public int NbPeriodDays
        {
            get;
            set;
        }

        public int? NbDaysBeforeOvulation
        {
            get;
            set;
        }

        public int? NbDaysBeforeLiquidMucus
        {
            get;
            set;
        }

        public int? NbDaysBtwLiquidMucusOvulation
        {
            get;
            set;
        }

        public double? TemperatureOvulation
        {
            get;
            set;
        }

        public Dictionary<int,double> Temperatures
        {
            get;
            set;
        }

        public SortedSet<NimbaDatetime> Days
        {
            get;
            set;
        }

        #endregion

        #region Initializers

        public NimbaCycle()
        {
            this.Type = CycleType.Normal;
            this.FirstDay = null;
            this.LastDay = null;
            this.OvulationDay = null;
            this.FirstLiquidMucusDay = null;
            this.NbDays = 0;
            this.NbPeriodDays = 0;
            this.NbDaysBeforeOvulation = null;
            this.NbDaysBeforeLiquidMucus = null;
            this.NbDaysBtwLiquidMucusOvulation = null;
            this.TemperatureOvulation = null;
            this.Temperatures = new Dictionary<int, double>();           
            this.Days = new SortedSet<NimbaDatetime>();
        }

        #endregion

        #region Public Methods

        public bool IsInCycle(DateTime datetime)
        {
            if ((datetime >= this.Days.Min.Date) && (datetime <= this.Days.Max.Date))
                return true;

            return false;
        }

        public int CompareTo(object obj)
        {
            return this.FirstDay.CompareTo(((NimbaCycle)obj).FirstDay);
        }

        #endregion
    }

    public class NimbaCycleSortedSet : SortedSet<NimbaCycle>
    {
        #region Private Members

        private Dictionary<int, List<double>> _eachCycleDayTemperatures = null;

        #endregion

        #region Enumerations

        private enum StateMachineAction
        {
            OpenNewCycleAndAddToCurrentCycle,
            OpenNewCycleAndAddToNewCycle,
            ClosePregnancyOpenNewCycleAndAddToNewCycle,
            ClearEndPregnancyAndAddToCurrentCycle,
            CloseNormalCycleMaxReached,
            ClosePregnancyCycleMaxReached,
            AddToCurrentCycle,
        }

        private enum StateMachineState
        {
            NormalCycleCloseNotAllowed,
            NormalCycleCloseAllowed,
            PregnancyCycle,
            TransitionCycleEmpty,
            TransitionCycle,
        }

        private enum StateMachineEvent
        {
            PeriodDay,
            OvulationDay,
            LiquidMucusDay,
            StartPregnancy,
            EndPregnancy,
            Nothing,
            OvulationReached,
            MaxNormalCycleDaysReached,
            MaxPregnancyCycleDaysReached,
        }

        public enum StatisticValue
        {
            Minimum,
            Average,
            Maximum,
        }

        #endregion

        #region Properties

        public Dictionary<StatisticValue, NimbaCycleStatisticData> Statistics
        {
            set;
            get;
        }

        public int AbsoluteMaxCycleDuration
        {
            get;
            set;
        }

        public double AbsoluteMinTemperature
        {
            get;
            set;
        }

        public double AbsoluteMaxTemperature
        {
            get;
            set;
        }

        #endregion

        #region Initializers

        public NimbaCycleSortedSet()
        {
            InitializeStatistics();
        }

        private void InitializeStatistics()
        {
            this.Statistics = new Dictionary<StatisticValue, NimbaCycleStatisticData>();
            this.Statistics.Add(StatisticValue.Minimum, new NimbaCycleStatisticData());
            this.Statistics.Add(StatisticValue.Average, new NimbaCycleStatisticData());
            this.Statistics.Add(StatisticValue.Maximum, new NimbaCycleStatisticData());

            this.AbsoluteMaxCycleDuration = 0;
            this.AbsoluteMinTemperature = 0.0;
            this.AbsoluteMaxTemperature = 0.0;

            _eachCycleDayTemperatures = new Dictionary<int, List<double>>();
        }

        #endregion

        #region Private Methods

        private void DetermineNextAction(StateMachineState state, StateMachineEvent stateMachineEvent, out StateMachineAction nextAction, out StateMachineState nextState)
        {
            //
            // Initialize out parameters.
            //
            nextAction = StateMachineAction.OpenNewCycleAndAddToNewCycle;
            nextState = StateMachineState.NormalCycleCloseAllowed;

            switch (state)
            {
                case StateMachineState.TransitionCycleEmpty:
                case StateMachineState.TransitionCycle:
                {
                    switch (stateMachineEvent)
                    {
                        case StateMachineEvent.StartPregnancy:
                        {
                            nextAction = StateMachineAction.OpenNewCycleAndAddToNewCycle;
                            nextState = StateMachineState.PregnancyCycle;
                            break;
                        }
                        case StateMachineEvent.EndPregnancy:
                        {
                            nextAction = StateMachineAction.ClearEndPregnancyAndAddToCurrentCycle;
                            nextState = StateMachineState.TransitionCycle;
                            break;
                        }
                        case StateMachineEvent.PeriodDay:
                        {
                            nextAction = StateMachineAction.OpenNewCycleAndAddToNewCycle;
                            nextState = StateMachineState.NormalCycleCloseNotAllowed;
                            break;
                        }
                        default:
                        {
                            nextAction = StateMachineAction.AddToCurrentCycle;
                            nextState = StateMachineState.TransitionCycle;
                            break;
                        }
                    }
                    break;
                }
                case StateMachineState.NormalCycleCloseAllowed:
                {
                    switch (stateMachineEvent)
                    {
                        case StateMachineEvent.StartPregnancy:
                        {
                            nextAction = StateMachineAction.OpenNewCycleAndAddToNewCycle;
                            nextState = StateMachineState.PregnancyCycle;
                            break;
                        }
                        case StateMachineEvent.EndPregnancy:
                        {
                            nextAction = StateMachineAction.ClearEndPregnancyAndAddToCurrentCycle;
                            nextState = StateMachineState.NormalCycleCloseAllowed;
                            break;
                        }
                        case StateMachineEvent.PeriodDay:
                        {
                            nextAction = StateMachineAction.OpenNewCycleAndAddToNewCycle;
                            nextState = StateMachineState.NormalCycleCloseNotAllowed;
                            break;
                        }
                        case StateMachineEvent.MaxNormalCycleDaysReached:
                        {
                            nextAction = StateMachineAction.CloseNormalCycleMaxReached;
                            nextState = StateMachineState.TransitionCycle;
                            break;
                        }
                        default:
                        {
                            nextAction = StateMachineAction.AddToCurrentCycle;
                            nextState = StateMachineState.NormalCycleCloseAllowed;
                            break;
                        }
                    }
                    break;
                }
                case StateMachineState.NormalCycleCloseNotAllowed:
                {
                    switch (stateMachineEvent)
                    {
                        case StateMachineEvent.StartPregnancy:
                        {
                            nextAction = StateMachineAction.OpenNewCycleAndAddToNewCycle;
                            nextState = StateMachineState.PregnancyCycle;
                            break;
                        }
                        case StateMachineEvent.EndPregnancy:
                        {
                            nextAction = StateMachineAction.ClearEndPregnancyAndAddToCurrentCycle;
                            nextState = StateMachineState.NormalCycleCloseNotAllowed;
                            break;
                        }
                        case StateMachineEvent.MaxNormalCycleDaysReached:
                        {
                            nextAction = StateMachineAction.CloseNormalCycleMaxReached;
                            nextState = StateMachineState.TransitionCycle;
                            break;
                        }
                        case StateMachineEvent.OvulationDay:
                        case StateMachineEvent.OvulationReached:
                        {
                            nextAction = StateMachineAction.AddToCurrentCycle;
                            nextState = StateMachineState.NormalCycleCloseAllowed;
                            break;
                        }
                        default:
                        {
                            nextAction = StateMachineAction.AddToCurrentCycle;
                            nextState = StateMachineState.NormalCycleCloseNotAllowed;
                            break;
                        }
                    }
                    break;
                }
                case StateMachineState.PregnancyCycle:
                {
                    switch (stateMachineEvent)
                    {
                        case StateMachineEvent.StartPregnancy:
                        {
                            nextAction = StateMachineAction.ClosePregnancyOpenNewCycleAndAddToNewCycle;
                            nextState = StateMachineState.PregnancyCycle;
                            break;
                        }
                        case StateMachineEvent.EndPregnancy:
                        {
                            nextAction = StateMachineAction.OpenNewCycleAndAddToCurrentCycle;
                            nextState = StateMachineState.TransitionCycleEmpty;
                            break;
                        }
                        case StateMachineEvent.MaxPregnancyCycleDaysReached:
                        {
                            nextAction = StateMachineAction.ClosePregnancyCycleMaxReached;
                            nextState = StateMachineState.TransitionCycle;
                            break;
                        }
                        default:
                        {
                            nextAction = StateMachineAction.AddToCurrentCycle;
                            nextState = StateMachineState.PregnancyCycle;
                            break;
                        }
                    }
                    break;
                }
            }
        }

        private void DetermineCurrentEvent(StateMachineState state, NimbaDatetime datetime, NimbaCycle currentCycle, NimbaConfiguration configuration, out StateMachineEvent currentEvent)
        {
            TimeSpan elapsedTime = TimeSpan.MinValue;

            //
            // Initialize out parameters.
            //
            currentEvent = StateMachineEvent.Nothing;

            if ((currentCycle != null) && (currentCycle.Days.Count > 0))
                elapsedTime = datetime.Date - currentCycle.FirstDay.Date;

            if (datetime.StartPregnancy)
                currentEvent = StateMachineEvent.StartPregnancy;
            else if (datetime.EndPregnancy)
                currentEvent = StateMachineEvent.EndPregnancy;
            else if (datetime.IsPeriod)
                currentEvent = StateMachineEvent.PeriodDay;
            else if (datetime.IsOvulation)
                currentEvent = StateMachineEvent.OvulationDay;
            else if ((state == StateMachineState.PregnancyCycle) &&
                (currentCycle != null) &&
                (elapsedTime.Days >= configuration.UserMaxPregnancyCycleDuration))
                currentEvent = StateMachineEvent.MaxPregnancyCycleDaysReached;
            else if (((state == StateMachineState.NormalCycleCloseAllowed) || (state == StateMachineState.NormalCycleCloseNotAllowed)) &&
                (currentCycle != null) &&
                (elapsedTime.Days >= configuration.UserMaxNormalCycleDuration))
                currentEvent = StateMachineEvent.MaxNormalCycleDaysReached;
            else if ((state == StateMachineState.NormalCycleCloseNotAllowed) &&
                (currentCycle != null) &&
                (elapsedTime.Days >= configuration.UserNbDaysBeforeOvulation))
                currentEvent = StateMachineEvent.OvulationReached;
            else if (datetime.IsLiquidMucus)
                currentEvent = StateMachineEvent.LiquidMucusDay;
            else
                currentEvent = StateMachineEvent.Nothing;
        }

        private NimbaCycle OpenNewCycle(StateMachineState nextState)
        {
            NimbaCycle nimbaCycle = null;

            nimbaCycle = new NimbaCycle();

            switch (nextState)
            {
                case StateMachineState.NormalCycleCloseNotAllowed:
                {
                    nimbaCycle.Type = CycleType.Normal;
                    break;
                }
                case StateMachineState.PregnancyCycle:
                {
                    nimbaCycle.Type = CycleType.Pregnancy;
                    break;
                }
                case StateMachineState.TransitionCycleEmpty:
                case StateMachineState.TransitionCycle:
                {
                    nimbaCycle.Type = CycleType.Transition;
                    break;
                }
                default:
                {
                    nimbaCycle.Type = CycleType.None;
                    break;
                }
            }

            return nimbaCycle;
        }

        private void AddToCurrentCycle(NimbaDatetime datetime, NimbaCycle cycle)
        {
            TimeSpan duration = TimeSpan.MinValue;

            //
            // First Day.
            //
            if (cycle.Days.Count == 0)
                cycle.FirstDay = datetime;

            //
            // Last Day.
            //
            cycle.LastDay = datetime;

            if (datetime.IsOvulation)
            {
                //
                // Ovulation Day.
                //
                foreach (NimbaDatetime date in cycle.Days)
                    date.IsOvulation = false;
                cycle.OvulationDay = datetime;

                //
                // TemperatureOvulation.
                //
                if (datetime.Temperature.HasValue)
                    cycle.TemperatureOvulation = datetime.Temperature.Value;
                else
                    cycle.TemperatureOvulation = null;

                //
                // NbDaysBeforeOvulation.
                //
                duration = datetime.Date - cycle.FirstDay.Date;
                cycle.NbDaysBeforeOvulation = duration.Days;

                //
                // NbDaysBtwLiquidMucusOvulation.
                //
                if (cycle.FirstLiquidMucusDay != null)
                {
                    duration = cycle.FirstLiquidMucusDay.Date - cycle.OvulationDay.Date;
                    cycle.NbDaysBtwLiquidMucusOvulation = duration.Days;
                }
            }

            //
            // First Liquid Mucus Day
            //
            if ((datetime.IsLiquidMucus) && (cycle.FirstLiquidMucusDay != null))
            {
                cycle.FirstLiquidMucusDay = datetime;

                //
                // NbDaysBeforeOvulation.
                //
                duration = datetime.Date - cycle.FirstDay.Date;
                cycle.NbDaysBeforeLiquidMucus = duration.Days;

                //
                // NbDaysBtwLiquidMucusOvulation.
                //
                if (cycle.OvulationDay != null)
                {
                    duration = cycle.FirstLiquidMucusDay.Date - cycle.OvulationDay.Date;
                    cycle.NbDaysBtwLiquidMucusOvulation = duration.Days;
                }
            }

            //
            // NbDays.
            //
            duration = datetime.Date - cycle.FirstDay.Date;
            cycle.NbDays = duration.Days;

            if (cycle.NbDays > this.AbsoluteMaxCycleDuration)
                this.AbsoluteMaxCycleDuration = cycle.NbDays;

            //
            // NbPeriodDays.
            //
            if (datetime.IsPeriod)
                ++cycle.NbPeriodDays;

            //
            // Temperatures.
            //
            if (datetime.Temperature.HasValue)
            {
                cycle.Temperatures.Add(cycle.NbDays, datetime.Temperature.Value);

                if (!this._eachCycleDayTemperatures.ContainsKey(cycle.NbDays))
                    this._eachCycleDayTemperatures.Add(cycle.NbDays, new List<double>());

                this._eachCycleDayTemperatures[cycle.NbDays].Add(datetime.Temperature.Value);

                this.AbsoluteMinTemperature = cycle.Temperatures.Values.Min();
                this.AbsoluteMaxTemperature = cycle.Temperatures.Values.Max();
            }

            //
            // Days.
            //
            cycle.Days.Add(datetime);                
        }

        private void AddLastCycleDay(NimbaDatetime datetime, NimbaCycle cycle, NimbaDatetimeSortedSet Dates, List<NimbaDatetime> toAdd)
        {
            if (cycle != null)
            {
                if ((Dates == null) && (!cycle.Days.Contains(datetime)))
                    AddToCurrentCycle(datetime, cycle);
                else if (!Dates.Contains(datetime))
                {
                    AddToCurrentCycle(datetime, cycle);
                    toAdd.Add(datetime);
                }

                this.Add(cycle);
            }
        }

        private void SetStatisticsToDefault(NimbaConfiguration configuration)
        {
            //
            // Cycle Duration.
            //
            this.Statistics[StatisticValue.Minimum].CycleDuration = configuration.UserCycleDuration;
            this.Statistics[StatisticValue.Average].CycleDuration = configuration.UserCycleDuration;
            this.Statistics[StatisticValue.Maximum].CycleDuration = configuration.UserCycleDuration;

            //
            // Period Duration.
            //
            this.Statistics[StatisticValue.Minimum].PeriodDuration = configuration.UserPeriodDuration;
            this.Statistics[StatisticValue.Average].PeriodDuration = configuration.UserPeriodDuration;
            this.Statistics[StatisticValue.Maximum].PeriodDuration = configuration.UserPeriodDuration;

            //
            // NbDaysBeforeOvulation.
            //
            this.Statistics[StatisticValue.Minimum].NbDaysBeforeOvulation = configuration.UserNbDaysBeforeOvulation;
            this.Statistics[StatisticValue.Average].NbDaysBeforeOvulation = configuration.UserNbDaysBeforeOvulation;
            this.Statistics[StatisticValue.Maximum].NbDaysBeforeOvulation = configuration.UserNbDaysBeforeOvulation;

            //
            // NbDaysBeforeLiquidMucus.
            //
            this.Statistics[StatisticValue.Minimum].NbDaysBeforeLiquidMucus = configuration.UserNbDaysBeforeLiquidMucus;
            this.Statistics[StatisticValue.Average].NbDaysBeforeLiquidMucus = configuration.UserNbDaysBeforeLiquidMucus;
            this.Statistics[StatisticValue.Maximum].NbDaysBeforeLiquidMucus = configuration.UserNbDaysBeforeLiquidMucus;

            //
            // NbDaysBtwLiquidMucusOvulation.
            //
            this.Statistics[StatisticValue.Minimum].NbDaysBtwLiquidMucusOvulation = configuration.UserNbDaysBetweenLiquidMucusAndOvulation;
            this.Statistics[StatisticValue.Average].NbDaysBtwLiquidMucusOvulation = configuration.UserNbDaysBetweenLiquidMucusAndOvulation;
            this.Statistics[StatisticValue.Maximum].NbDaysBtwLiquidMucusOvulation = configuration.UserNbDaysBetweenLiquidMucusAndOvulation;

            //
            // TemperatureOvulation.
            //
            this.Statistics[StatisticValue.Minimum].TemperatureOvulation = 0.0;
            this.Statistics[StatisticValue.Average].TemperatureOvulation = 0.0;
            this.Statistics[StatisticValue.Maximum].TemperatureOvulation = 0.0;

            //
            // Temperature.
            //
            this.Statistics[StatisticValue.Minimum].Temperature = 0.0;
            this.Statistics[StatisticValue.Average].Temperature = 0.0;
            this.Statistics[StatisticValue.Maximum].Temperature = 0.0;
        }

        private void ComputeCyclesStatistics(NimbaConfiguration configuration)
        {
            List<double> cycleDuration = new List<double>();
            List<double> periodDuration = new List<double>();
            List<double> nbDaysBeforeOvulation = new List<double>();
            List<double> nbDaysBeforeLiquidMucus = new List<double>();
            List<double> nbDaysBtwLiquidMucusOvulation = new List<double>();
            List<double> temperatureOvulation = new List<double>();
            List<double> temperature = new List<double>();

            if (this.Count < 2)
            {
                SetStatisticsToDefault(configuration);
                return;
            }

            foreach (NimbaCycle cycle in this)
            {
                if (cycle.Equals(this.Max))
                    continue;

                cycleDuration.Add(cycle.NbDays);

                if ((cycle.Type == CycleType.Normal) && (cycle.NbPeriodDays > 0))
                    periodDuration.Add(cycle.NbPeriodDays);

                if (cycle.NbDaysBeforeOvulation.HasValue)
                    nbDaysBeforeOvulation.Add(cycle.NbDaysBeforeOvulation.Value);

                if (cycle.NbDaysBeforeLiquidMucus.HasValue)
                    nbDaysBeforeLiquidMucus.Add(cycle.NbDaysBeforeLiquidMucus.Value);

                if (cycle.NbDaysBtwLiquidMucusOvulation.HasValue)
                    nbDaysBtwLiquidMucusOvulation.Add(cycle.NbDaysBtwLiquidMucusOvulation.Value);

                if (cycle.TemperatureOvulation.HasValue)
                    temperatureOvulation.Add(cycle.TemperatureOvulation.Value);

                temperature.AddRange(cycle.Temperatures.Values);
            }

            //
            // Cycle Duration.
            //
            this.Statistics[StatisticValue.Minimum].CycleDuration = cycleDuration.Min();
            this.Statistics[StatisticValue.Average].CycleDuration = cycleDuration.Average();
            this.Statistics[StatisticValue.Maximum].CycleDuration = cycleDuration.Max();

            //
            // Period Duration.
            //
            this.Statistics[StatisticValue.Minimum].PeriodDuration = periodDuration.Min();
            this.Statistics[StatisticValue.Average].PeriodDuration = periodDuration.Average();
            this.Statistics[StatisticValue.Maximum].PeriodDuration = periodDuration.Max();

            //
            // NbDaysBeforeOvulation.
            //
            this.Statistics[StatisticValue.Minimum].NbDaysBeforeOvulation = nbDaysBeforeOvulation.Min();
            this.Statistics[StatisticValue.Average].NbDaysBeforeOvulation = nbDaysBeforeOvulation.Average();
            this.Statistics[StatisticValue.Maximum].NbDaysBeforeOvulation = nbDaysBeforeOvulation.Max();

            //
            // NbDaysBeforeLiquidMucus.
            //
            this.Statistics[StatisticValue.Minimum].NbDaysBeforeLiquidMucus = nbDaysBeforeLiquidMucus.Min();
            this.Statistics[StatisticValue.Average].NbDaysBeforeLiquidMucus = nbDaysBeforeLiquidMucus.Average();
            this.Statistics[StatisticValue.Maximum].NbDaysBeforeLiquidMucus = nbDaysBeforeLiquidMucus.Max();

            //
            // NbDaysBtwLiquidMucusOvulation.
            //
            this.Statistics[StatisticValue.Minimum].NbDaysBtwLiquidMucusOvulation = nbDaysBtwLiquidMucusOvulation.Min();
            this.Statistics[StatisticValue.Average].NbDaysBtwLiquidMucusOvulation = nbDaysBtwLiquidMucusOvulation.Average();
            this.Statistics[StatisticValue.Maximum].NbDaysBtwLiquidMucusOvulation = nbDaysBtwLiquidMucusOvulation.Max();

            //
            // TemperatureOvulation.
            //
            this.Statistics[StatisticValue.Minimum].TemperatureOvulation = temperatureOvulation.Min();
            this.Statistics[StatisticValue.Average].TemperatureOvulation = temperatureOvulation.Average();
            this.Statistics[StatisticValue.Maximum].TemperatureOvulation = temperatureOvulation.Max();

            //
            // Temperature.
            //
            this.Statistics[StatisticValue.Minimum].Temperature = temperature.Min();
            this.Statistics[StatisticValue.Average].Temperature = temperature.Average();
            this.Statistics[StatisticValue.Maximum].Temperature = temperature.Max();
        }

        private void OpenNewCycleAndAddToCurrentCycle(NimbaDatetime date, StateMachineState nextState, ref NimbaDatetime dayAfterEndPregnancy, ref NimbaCycle cycle)
        {
            //
            // Add to current cycle.
            //
            AddLastCycleDay(date, cycle, null, null);

            //
            // Open new cycle.
            //
            cycle = OpenNewCycle(nextState);

            //
            // Save the day after end pregnancy.
            //
            dayAfterEndPregnancy = new NimbaDatetime(date.Date.AddDays(1));
        }
       
        private void OpenNewCycleAndAddToNewCycle(NimbaDatetime date, StateMachineState currentState, StateMachineState nextState, NimbaDatetimeSortedSet dates, List<NimbaDatetime> toAdd, ref NimbaCycle cycle)
        {
            if (currentState != StateMachineState.TransitionCycleEmpty)
            {
                //
                // Add last day of current cycle.
                //
                AddLastCycleDay(new NimbaDatetime(date.Date.AddDays(-1)), cycle, dates, toAdd);
            }

            //
            // Open new cycle.
            //
            cycle = OpenNewCycle(nextState);

            //
            // Add to current cycle.
            //
            AddToCurrentCycle(date, cycle);
        }

        private void ClosePregnancyOpenNewCycleAndAddToNewCycle(NimbaDatetime date, StateMachineState nextState, NimbaDatetimeSortedSet dates, List<NimbaDatetime> toAdd, ref NimbaCycle cycle)
        {
            NimbaDatetime nimbaDatetime = null;

            if (!dates.Find(date.Date.AddDays(-1), out nimbaDatetime))
                nimbaDatetime = new NimbaDatetime(date.Date.AddDays(-1));

            nimbaDatetime.EndPregnancy = true;

            //
            // Add last day of current cycle.
            //
            AddLastCycleDay(nimbaDatetime, cycle, dates, toAdd);

            //
            // Open new cycle.
            //
            cycle = OpenNewCycle(nextState);

            //
            // Add to current cycle.
            //
            AddToCurrentCycle(date, cycle);
        }

        private void ClearEndPregnancyAndAddToCurrentCycle(NimbaDatetime date, NimbaCycle cycle)
        {
            date.EndPregnancy = false;

            //
            // Add to current cycle.
            //
            AddToCurrentCycle(date, cycle);
        }

        private void CloseNormalCycleMaxReached(NimbaDatetime date, int userMaxNormalCycleDuration, StateMachineState nextState, NimbaDatetimeSortedSet dates, List<NimbaDatetime> toAdd, ref NimbaDatetime dayAfterEndPregnancy, ref NimbaCycle cycle)
        {
            NimbaDatetime nimbaDatetime = null;

            if (!dates.Find(cycle.FirstDay.Date.AddDays(userMaxNormalCycleDuration), out nimbaDatetime))
                nimbaDatetime = new NimbaDatetime(cycle.FirstDay.Date.AddDays(userMaxNormalCycleDuration));

            //
            // Add last day of current cycle.
            //
            AddLastCycleDay(nimbaDatetime, cycle, dates, toAdd);

            //
            // Open new cycle.
            //
            cycle = OpenNewCycle(nextState);

            dayAfterEndPregnancy = new NimbaDatetime(nimbaDatetime.Date.AddDays(1));

            if (date.Date != dayAfterEndPregnancy.Date)
            {
                AddToCurrentCycle(dayAfterEndPregnancy, cycle);
                dayAfterEndPregnancy = null;
            }

            //
            // Add to current cycle.
            //
            AddToCurrentCycle(date, cycle);
        }

        private void ClosePregnancyCycleMaxReached(NimbaDatetime date, int userMaxPregnancyCycleDuration, StateMachineState nextState, NimbaDatetimeSortedSet dates, List<NimbaDatetime> toAdd, ref NimbaDatetime dayAfterEndPregnancy, ref NimbaCycle cycle)
        {
            NimbaDatetime nimbaDatetime = null;

            if (!dates.Find(cycle.FirstDay.Date.AddDays(userMaxPregnancyCycleDuration), out nimbaDatetime))
                nimbaDatetime = new NimbaDatetime(cycle.FirstDay.Date.AddDays(userMaxPregnancyCycleDuration));

            nimbaDatetime.EndPregnancy = true;

            //
            // Add last day of current cycle.
            //
            AddLastCycleDay(nimbaDatetime, cycle, dates, toAdd);

            //
            // Open new cycle.
            //
            cycle = OpenNewCycle(nextState);

            dayAfterEndPregnancy = new NimbaDatetime(nimbaDatetime.Date.AddDays(1));

            if (date.Date != dayAfterEndPregnancy.Date)
            {
                AddToCurrentCycle(dayAfterEndPregnancy, cycle);
                dayAfterEndPregnancy = null;
            }

            //
            // Add to current cycle.
            //
            AddToCurrentCycle(date, cycle);
        }

        #endregion

        #region Public Methods

        public void Load(NimbaDatetimeSortedSet dates, NimbaConfiguration configuration)
        {
            StateMachineAction nextAction = StateMachineAction.OpenNewCycleAndAddToNewCycle;
            StateMachineState currentState = StateMachineState.TransitionCycleEmpty;
            StateMachineState nextState = StateMachineState.TransitionCycle;
            StateMachineEvent currentEvent = StateMachineEvent.Nothing;
            NimbaCycle currentCycle = null;
            NimbaDatetime nimbaDatetime = null;
            NimbaDatetime dayAfterEndPregnancy = null;
            List<NimbaDatetime> toAdd = null;

            try
            {
                this.Clear();

                toAdd = new List<NimbaDatetime>();

                //
                // Determine why the cycle starts :
                // 1. Period Day
                // 2. Start Pregnancy
                // 3. End Pregnancy or Max Nb Cycle Days reached (--> Liquid Mucus or Nothing)
                //
                currentState = StateMachineState.TransitionCycle;

                if (dates.Count == 0)
                    return;

                //
                // Add Datetime.Now to set of dates.
                //
                dates.Add(new NimbaDatetime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)));

                if (!dates.Min.StartPregnancy && !dates.Min.EndPregnancy && !dates.Min.IsPeriod)
                    currentCycle = OpenNewCycle(currentState);

                foreach (NimbaDatetime date in dates)
                {
                    //
                    // Add the first day of the transition cycle after the pregnancy if needed.
                    //
                    if (currentState == StateMachineState.TransitionCycleEmpty && dayAfterEndPregnancy != null && date.Date != dayAfterEndPregnancy.Date)
                    {
                        AddToCurrentCycle(dayAfterEndPregnancy, currentCycle);
                        dayAfterEndPregnancy = null;
                    }
                    else
                        dayAfterEndPregnancy = null;

                    //
                    // Determine the current event.
                    //
                    DetermineCurrentEvent(currentState, date, currentCycle, configuration, out currentEvent);

                    //
                    // Determine which action to take :
                    // 1. Open new cycle (and close current cycle if needed)
                    // 2. Add to current cycle.
                    //
                    // From possible states :
                    // 1. Normal cycle Close Cycle not allowed.
                    // 2. Normal cycle Close Cycle allowed.
                    // 3. Normal cycle Close cycle allowed Max Cycle Days not reached.
                    // 4. Pregnancy cycle.
                    // 5. Empty cycle.
                    //
                    // And possible events :
                    // 1. Period Day
                    // 2. Ovulation Day
                    // 3. Liquid Mucus Day
                    // 4. Start Pregnancy
                    // 5. End Pregnancy
                    // 6. Nothing
                    // 7. Default/Average number of days before ovulation reached
                    // 8. Max Cycle Days reached
                    //
                    DetermineNextAction(currentState, currentEvent, out nextAction, out nextState);

                    switch (nextAction)
                    {
                        case StateMachineAction.OpenNewCycleAndAddToCurrentCycle:
                        {
                            OpenNewCycleAndAddToCurrentCycle(date, nextState, ref dayAfterEndPregnancy, ref currentCycle);

                            break;
                        }
                        case StateMachineAction.OpenNewCycleAndAddToNewCycle:
                        {
                            OpenNewCycleAndAddToNewCycle(date, currentState, nextState, dates, toAdd, ref currentCycle);

                            break;
                        }
                        case StateMachineAction.ClosePregnancyOpenNewCycleAndAddToNewCycle:
                        {
                            ClosePregnancyOpenNewCycleAndAddToNewCycle(date, nextState, dates, toAdd, ref currentCycle);

                            break;
                        }
                        case StateMachineAction.ClearEndPregnancyAndAddToCurrentCycle:
                        {
                            ClearEndPregnancyAndAddToCurrentCycle(date, currentCycle);

                            break;
                        }
                        case StateMachineAction.CloseNormalCycleMaxReached:
                        {
                            CloseNormalCycleMaxReached(date, configuration.UserMaxNormalCycleDuration, nextState, dates, toAdd, ref dayAfterEndPregnancy, ref currentCycle);

                            break;
                        }
                        case StateMachineAction.ClosePregnancyCycleMaxReached:
                        {
                            ClosePregnancyCycleMaxReached(date, configuration.UserMaxPregnancyCycleDuration, nextState, dates, toAdd, ref dayAfterEndPregnancy, ref currentCycle);

                            break;
                        }
                        case StateMachineAction.AddToCurrentCycle:
                        {
                            AddToCurrentCycle(date, currentCycle);

                            break;
                        }
                    }

                    //
                    // Update state.
                    //
                    currentState = nextState;
                }

                //
                // Add Datetime.Now as last day of current cycle.
                //
                AddLastCycleDay(new NimbaDatetime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)), currentCycle, dates, toAdd);

                foreach (NimbaDatetime datetime in toAdd)
                    dates.Add(datetime);
            }
            catch (Exception e)
            {
                string message = e.Message;
            }
        }

        public void ComputeStatistics(NimbaConfiguration configuration)
        {
            try
            {
                if (configuration == null)
                {
                    SetStatisticsToDefault(configuration);
                    return;
                }

                switch (configuration.UserParametersSelection)
                {
                    case ParametersSelection.UseDefaultParameters:
                    {
                        SetStatisticsToDefault(configuration);
                        break;
                    }
                    case ParametersSelection.UseStatistics:
                    {
                        ComputeCyclesStatistics(configuration);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }
        }

        public bool Find(DateTime datetime, out NimbaCycle nimbaCycle)
        {
            //
            // Initialize out parameters.
            //
            nimbaCycle = null;

            try
            {
                foreach (NimbaCycle cycle in this)
                {
                    if (cycle.IsInCycle(datetime))
                    {
                        nimbaCycle = cycle;
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

        public override void Clear()
        {
            try
            {
                base.Clear();

                InitializeStatistics();
            }
            catch (Exception e)
            {
                string message = e.Message;
            }
        }

        public Dictionary<StatisticValue, Dictionary<int, double>> GetEachCycleDayStatisticsTemperature()
        {
            Dictionary<StatisticValue, Dictionary<int, double>> statisticsTemperature = null;

            try
            {
                statisticsTemperature = new Dictionary<StatisticValue, Dictionary<int, double>>();

                statisticsTemperature.Add(StatisticValue.Minimum, new Dictionary<int, double>());
                statisticsTemperature.Add(StatisticValue.Average, new Dictionary<int, double>());
                statisticsTemperature.Add(StatisticValue.Maximum, new Dictionary<int, double>());

                foreach (int cycleDay in _eachCycleDayTemperatures.Keys)
                {
                    statisticsTemperature[StatisticValue.Minimum].Add(cycleDay, _eachCycleDayTemperatures[cycleDay].Min());
                    statisticsTemperature[StatisticValue.Average].Add(cycleDay, _eachCycleDayTemperatures[cycleDay].Average());
                    statisticsTemperature[StatisticValue.Maximum].Add(cycleDay, _eachCycleDayTemperatures[cycleDay].Max());
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return statisticsTemperature;
        }

        #endregion
    }
}
