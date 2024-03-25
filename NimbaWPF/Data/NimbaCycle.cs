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


    public enum StatisticValue
    {
        Minimum,
        Average,
        Maximum,
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
            this.Temperature = 0.0;
        }

        #endregion
    }

    public class NimbaNormalCycleStatisticData : NimbaCycleStatisticData
    {
        #region Properties

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

        #endregion

        #region Initializers

        public NimbaNormalCycleStatisticData()
        {
            this.NbDaysBeforeOvulation = 0.0;
            this.NbDaysBeforeLiquidMucus = 0.0;
            this.NbDaysBtwLiquidMucusOvulation = 0.0;
            this.TemperatureOvulation = 0.0;
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

        #endregion

        #region Properties

        public Dictionary<CycleType, Dictionary<StatisticValue, NimbaCycleStatisticData>> Statistics
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
            this.Statistics = new Dictionary<CycleType, Dictionary<StatisticValue, NimbaCycleStatisticData>>();

            foreach (CycleType cycleType in Enum.GetValues(typeof(CycleType)))
            {
                if (cycleType == CycleType.None)
                    continue;

                this.Statistics.Add(cycleType, new Dictionary<StatisticValue, NimbaCycleStatisticData>());

                foreach (StatisticValue statisticValue in Enum.GetValues(typeof(StatisticValue)))
                {
                    switch (cycleType)
                    {
                        case CycleType.Normal:
                        {
                            this.Statistics[cycleType].Add(statisticValue, new NimbaNormalCycleStatisticData());
                            break;
                        }
                        default:
                        {
                            this.Statistics[cycleType].Add(statisticValue, new NimbaCycleStatisticData());
                            break;
                        }
                    }
                     
                }
            }

            this.AbsoluteMaxCycleDuration = 0;
            this.AbsoluteMinTemperature = 0.0;
            this.AbsoluteMaxTemperature = 0.0;

            _eachCycleDayTemperatures = new Dictionary<int, List<double>>();
        }

        #endregion

        #region Private Methods

        #region Private Load Methods

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
                        case StateMachineEvent.PeriodDay:
                        {
                            nextAction = StateMachineAction.OpenNewCycleAndAddToNewCycle;
                            nextState = StateMachineState.NormalCycleCloseNotAllowed;
                            break;
                        }
                        case StateMachineEvent.MaxNormalCycleDaysReached:
                        {
                            nextAction = StateMachineAction.CloseNormalCycleMaxReached;
                            nextState = StateMachineState.TransitionCycleEmpty;
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
                        case StateMachineEvent.MaxNormalCycleDaysReached:
                        {
                            nextAction = StateMachineAction.CloseNormalCycleMaxReached;
                            nextState = StateMachineState.TransitionCycleEmpty;
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
                            nextState = StateMachineState.TransitionCycleEmpty;
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

            if (datetime.IsOvulation && cycle.Type == CycleType.Pregnancy)
                datetime.IsOvulation = false;
            else if (datetime.IsOvulation)
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
            if ((datetime.IsLiquidMucus) && (cycle.FirstLiquidMucusDay == null))
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
            // End Pregnancy
            //
            if (datetime.EndPregnancy && cycle.Type != CycleType.Pregnancy)
                datetime.EndPregnancy = false;

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

                if (cycle.Days.Count > 0)
                    this.Add(cycle);
            }
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
            if ((currentState != StateMachineState.TransitionCycleEmpty) || ((cycle != null) && (cycle.Days.Count > 0)))
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

        private void CloseNormalCycleMaxReached(NimbaDatetime date, int userMaxNormalCycleDuration, StateMachineState nextState, NimbaDatetimeSortedSet dates, List<NimbaDatetime> toAdd, ref NimbaDatetime dayAfterEndPregnancy, ref NimbaCycle cycle)
        {
            NimbaDatetime nimbaDatetime = null;

            //
            // Add last day of current cycle.
            //
            if (!dates.Find(cycle.FirstDay.Date.AddDays(userMaxNormalCycleDuration), out nimbaDatetime))
            {
                nimbaDatetime = new NimbaDatetime(cycle.FirstDay.Date.AddDays(userMaxNormalCycleDuration));

                AddLastCycleDay(nimbaDatetime, cycle, dates, toAdd);
            }
            else
                AddLastCycleDay(nimbaDatetime, cycle, null, null);
            
            //
            // Open new cycle.
            //
            cycle = OpenNewCycle(nextState);

            dayAfterEndPregnancy = new NimbaDatetime(nimbaDatetime.Date.AddDays(1));

            //
            // Avoid adding twice the last day when the current day corresponds to the max cycle day.
            //
            if (dayAfterEndPregnancy.Date < date.Date)
            {
                AddToCurrentCycle(dayAfterEndPregnancy, cycle);
                dayAfterEndPregnancy = null;
            }          

            //
            // Add to current cycle.
            //
            if (date.Date > nimbaDatetime.Date)
                AddToCurrentCycle(date, cycle);
        }

        private void ClosePregnancyCycleMaxReached(NimbaDatetime date, int userMaxPregnancyCycleDuration, StateMachineState nextState, NimbaDatetimeSortedSet dates, List<NimbaDatetime> toAdd, ref NimbaDatetime dayAfterEndPregnancy, ref NimbaCycle cycle)
        {
            NimbaDatetime nimbaDatetime = null;

            //
            // Add last day of current cycle.
            //
            if (!dates.Find(cycle.FirstDay.Date.AddDays(userMaxPregnancyCycleDuration), out nimbaDatetime))
            {
                nimbaDatetime = new NimbaDatetime(cycle.FirstDay.Date.AddDays(userMaxPregnancyCycleDuration));

                nimbaDatetime.EndPregnancy = true;

                AddLastCycleDay(nimbaDatetime, cycle, dates, toAdd);
            }
            else
            {
                nimbaDatetime.EndPregnancy = true;

                AddLastCycleDay(nimbaDatetime, cycle, null, null);
            }

            //
            // Open new cycle.
            //
            cycle = OpenNewCycle(nextState);

            dayAfterEndPregnancy = new NimbaDatetime(nimbaDatetime.Date.AddDays(1));

            if (dayAfterEndPregnancy.Date < date.Date)
            {
                AddToCurrentCycle(dayAfterEndPregnancy, cycle);
                dayAfterEndPregnancy = null;
            }

            //
            // Add to current cycle.
            //
            if (date.Date > nimbaDatetime.Date)
                AddToCurrentCycle(date, cycle);
        }

        private void AddTransitionCycleFirstDay(NimbaDatetime date, StateMachineState currentState, ref NimbaDatetime dayAfterEndPregnancy, ref NimbaCycle cycle)
        {
            if (currentState == StateMachineState.TransitionCycleEmpty && dayAfterEndPregnancy != null && date.Date != dayAfterEndPregnancy.Date)
            {
                AddToCurrentCycle(dayAfterEndPregnancy, cycle);
                dayAfterEndPregnancy = null;
            }
            else
                dayAfterEndPregnancy = null;
        }

        #endregion

        #region Private Statistics Methods

        private void SetStatisticsToDefault(NimbaConfiguration configuration)
        {
            SetTransitionCycleStatisticsToDefault(configuration);

            SetPregnancyCycleStatisticsToDefault(configuration);

            SetNormalCycleStatisticsToDefault(configuration);
        }

        private void SetTransitionCycleStatisticsToDefault(NimbaConfiguration configuration)
        {
            //
            // Cycle Duration.
            //
            this.Statistics[CycleType.Transition][StatisticValue.Minimum].CycleDuration = configuration.UserCycleDuration;
            this.Statistics[CycleType.Transition][StatisticValue.Average].CycleDuration = configuration.UserCycleDuration;
            this.Statistics[CycleType.Transition][StatisticValue.Maximum].CycleDuration = configuration.UserCycleDuration;

            //
            // Period Duration.
            //
            this.Statistics[CycleType.Transition][StatisticValue.Minimum].PeriodDuration = configuration.UserPeriodDuration;
            this.Statistics[CycleType.Transition][StatisticValue.Average].PeriodDuration = configuration.UserPeriodDuration;
            this.Statistics[CycleType.Transition][StatisticValue.Maximum].PeriodDuration = configuration.UserPeriodDuration;

            //
            // Temperature.
            //
            this.Statistics[CycleType.Transition][StatisticValue.Minimum].Temperature = 0.0;
            this.Statistics[CycleType.Transition][StatisticValue.Average].Temperature = 0.0;
            this.Statistics[CycleType.Transition][StatisticValue.Maximum].Temperature = 0.0;
        }

        private void SetPregnancyCycleStatisticsToDefault(NimbaConfiguration configuration)
        {
            //
            // Cycle Duration.
            //
            this.Statistics[CycleType.Pregnancy][StatisticValue.Minimum].CycleDuration = configuration.UserCycleDuration;
            this.Statistics[CycleType.Pregnancy][StatisticValue.Average].CycleDuration = configuration.UserCycleDuration;
            this.Statistics[CycleType.Pregnancy][StatisticValue.Maximum].CycleDuration = configuration.UserCycleDuration;

            //
            // Period Duration.
            //
            this.Statistics[CycleType.Pregnancy][StatisticValue.Minimum].PeriodDuration = configuration.UserPeriodDuration;
            this.Statistics[CycleType.Pregnancy][StatisticValue.Average].PeriodDuration = configuration.UserPeriodDuration;
            this.Statistics[CycleType.Pregnancy][StatisticValue.Maximum].PeriodDuration = configuration.UserPeriodDuration;

            //
            // Temperature.
            //
            this.Statistics[CycleType.Pregnancy][StatisticValue.Minimum].Temperature = 0.0;
            this.Statistics[CycleType.Pregnancy][StatisticValue.Average].Temperature = 0.0;
            this.Statistics[CycleType.Pregnancy][StatisticValue.Maximum].Temperature = 0.0;
        }

        private void SetNormalCycleStatisticsToDefault(NimbaConfiguration configuration)
        {
            //
            // Cycle Duration.
            //
            this.Statistics[CycleType.Pregnancy][StatisticValue.Minimum].CycleDuration = configuration.UserCycleDuration;
            this.Statistics[CycleType.Pregnancy][StatisticValue.Average].CycleDuration = configuration.UserCycleDuration;
            this.Statistics[CycleType.Pregnancy][StatisticValue.Maximum].CycleDuration = configuration.UserCycleDuration;

            //
            // Period Duration.
            //
            this.Statistics[CycleType.Pregnancy][StatisticValue.Minimum].PeriodDuration = configuration.UserPeriodDuration;
            this.Statistics[CycleType.Pregnancy][StatisticValue.Average].PeriodDuration = configuration.UserPeriodDuration;
            this.Statistics[CycleType.Pregnancy][StatisticValue.Maximum].PeriodDuration = configuration.UserPeriodDuration;

            //
            // NbDaysBeforeOvulation.
            //
            ((NimbaNormalCycleStatisticData)this.Statistics[CycleType.Normal][StatisticValue.Minimum]).NbDaysBeforeOvulation = configuration.UserNbDaysBeforeOvulation;
            ((NimbaNormalCycleStatisticData)this.Statistics[CycleType.Normal][StatisticValue.Average]).NbDaysBeforeOvulation = configuration.UserNbDaysBeforeOvulation;
            ((NimbaNormalCycleStatisticData)this.Statistics[CycleType.Normal][StatisticValue.Maximum]).NbDaysBeforeOvulation = configuration.UserNbDaysBeforeOvulation;

            //
            // NbDaysBeforeLiquidMucus.
            //
            ((NimbaNormalCycleStatisticData)this.Statistics[CycleType.Normal][StatisticValue.Minimum]).NbDaysBeforeLiquidMucus = configuration.UserNbDaysBeforeLiquidMucus;
            ((NimbaNormalCycleStatisticData)this.Statistics[CycleType.Normal][StatisticValue.Average]).NbDaysBeforeLiquidMucus = configuration.UserNbDaysBeforeLiquidMucus;
            ((NimbaNormalCycleStatisticData)this.Statistics[CycleType.Normal][StatisticValue.Maximum]).NbDaysBeforeLiquidMucus = configuration.UserNbDaysBeforeLiquidMucus;

            //
            // NbDaysBtwLiquidMucusOvulation.
            //
            ((NimbaNormalCycleStatisticData)this.Statistics[CycleType.Normal][StatisticValue.Minimum]).NbDaysBtwLiquidMucusOvulation = configuration.UserNbDaysBetweenLiquidMucusAndOvulation;
            ((NimbaNormalCycleStatisticData)this.Statistics[CycleType.Normal][StatisticValue.Average]).NbDaysBtwLiquidMucusOvulation = configuration.UserNbDaysBetweenLiquidMucusAndOvulation;
            ((NimbaNormalCycleStatisticData)this.Statistics[CycleType.Normal][StatisticValue.Maximum]).NbDaysBtwLiquidMucusOvulation = configuration.UserNbDaysBetweenLiquidMucusAndOvulation;

            //
            // TemperatureOvulation.
            //
            ((NimbaNormalCycleStatisticData)this.Statistics[CycleType.Normal][StatisticValue.Minimum]).TemperatureOvulation = 0.0;
            ((NimbaNormalCycleStatisticData)this.Statistics[CycleType.Normal][StatisticValue.Average]).TemperatureOvulation = 0.0;
            ((NimbaNormalCycleStatisticData)this.Statistics[CycleType.Normal][StatisticValue.Maximum]).TemperatureOvulation = 0.0;

            //
            // Temperature.
            //
            this.Statistics[CycleType.Pregnancy][StatisticValue.Minimum].Temperature = 0.0;
            this.Statistics[CycleType.Pregnancy][StatisticValue.Average].Temperature = 0.0;
            this.Statistics[CycleType.Pregnancy][StatisticValue.Maximum].Temperature = 0.0;
        }
        
        private void ComputeCyclesStatistics(NimbaConfiguration configuration)
        {
            Dictionary<CycleType, List<double>> cycleDuration = new Dictionary<CycleType, List<double>>();
            Dictionary<CycleType, List<double>> periodDuration = new Dictionary<CycleType, List<double>>();
            List<double> nbDaysBeforeOvulation = new List<double>();
            List<double> nbDaysBeforeLiquidMucus = new List<double>();
            List<double> nbDaysBtwLiquidMucusOvulation = new List<double>();
            List<double> temperatureOvulation = new List<double>();
            Dictionary<CycleType, List<double>> temperature = new Dictionary<CycleType, List<double>>();

            if (this.Count < 2)
            {
                SetStatisticsToDefault(configuration);
                return;
            }

            foreach (NimbaCycle cycle in this)
            {
                //
                // Skip the last cycle.
                //
                if (cycle.Equals(this.Max))
                    continue;

                if (!cycleDuration.ContainsKey(cycle.Type))
                    cycleDuration.Add(cycle.Type, new List<double>());

                if (!periodDuration.ContainsKey(cycle.Type))
                    periodDuration.Add(cycle.Type, new List<double>());

                if (!temperature.ContainsKey(cycle.Type))
                    temperature.Add(cycle.Type, new List<double>());

                cycleDuration[cycle.Type].Add(cycle.NbDays);

                if (cycle.NbPeriodDays > 0)
                    periodDuration[cycle.Type].Add(cycle.NbPeriodDays);

                switch (cycle.Type)
                {
                    case CycleType.Normal:
                    {
                        if (cycle.NbDaysBeforeOvulation.HasValue)
                            nbDaysBeforeOvulation.Add(cycle.NbDaysBeforeOvulation.Value);

                        if (cycle.NbDaysBeforeLiquidMucus.HasValue)
                            nbDaysBeforeLiquidMucus.Add(cycle.NbDaysBeforeLiquidMucus.Value);

                        if (cycle.NbDaysBtwLiquidMucusOvulation.HasValue)
                            nbDaysBtwLiquidMucusOvulation.Add(cycle.NbDaysBtwLiquidMucusOvulation.Value);

                        if (cycle.TemperatureOvulation.HasValue)
                            temperatureOvulation.Add(cycle.TemperatureOvulation.Value);
                        break;
                    }
                }

                temperature[cycle.Type].AddRange(cycle.Temperatures.Values);
            }

            foreach (CycleType cycleType in Enum.GetValues(typeof(CycleType)))
            {

                //
                // Cycle Duration.
                //
                if (cycleDuration.ContainsKey(cycleType) && cycleDuration[cycleType].Count > 0)
                {
                    this.Statistics[cycleType][StatisticValue.Minimum].CycleDuration = cycleDuration[cycleType].Min();
                    this.Statistics[cycleType][StatisticValue.Average].CycleDuration = cycleDuration[cycleType].Average();
                    this.Statistics[cycleType][StatisticValue.Maximum].CycleDuration = cycleDuration[cycleType].Max();
                }

                //
                // Period Duration.
                //
                if (periodDuration.ContainsKey(cycleType) && periodDuration[cycleType].Count > 0)
                {
                    this.Statistics[cycleType][StatisticValue.Minimum].PeriodDuration = periodDuration[cycleType].Min();
                    this.Statistics[cycleType][StatisticValue.Average].PeriodDuration = periodDuration[cycleType].Average();
                    this.Statistics[cycleType][StatisticValue.Maximum].PeriodDuration = periodDuration[cycleType].Max();
                }

                switch (cycleType)
                {
                    case CycleType.Normal:
                    {

                        //
                        // NbDaysBeforeOvulation.
                        //
                        if (nbDaysBeforeOvulation.Count > 0)
                        {
                            ((NimbaNormalCycleStatisticData)this.Statistics[cycleType][StatisticValue.Minimum]).NbDaysBeforeOvulation = nbDaysBeforeOvulation.Min();
                            ((NimbaNormalCycleStatisticData)this.Statistics[cycleType][StatisticValue.Average]).NbDaysBeforeOvulation = nbDaysBeforeOvulation.Average();
                            ((NimbaNormalCycleStatisticData)this.Statistics[cycleType][StatisticValue.Maximum]).NbDaysBeforeOvulation = nbDaysBeforeOvulation.Max();
                        }
                        //
                        // NbDaysBeforeLiquidMucus.
                        //
                        if (nbDaysBeforeLiquidMucus.Count > 0)
                        {
                            ((NimbaNormalCycleStatisticData)this.Statistics[cycleType][StatisticValue.Minimum]).NbDaysBeforeLiquidMucus = nbDaysBeforeLiquidMucus.Min();
                            ((NimbaNormalCycleStatisticData)this.Statistics[cycleType][StatisticValue.Average]).NbDaysBeforeLiquidMucus = nbDaysBeforeLiquidMucus.Average();
                            ((NimbaNormalCycleStatisticData)this.Statistics[cycleType][StatisticValue.Maximum]).NbDaysBeforeLiquidMucus = nbDaysBeforeLiquidMucus.Max();
                        }

                        //
                        // NbDaysBtwLiquidMucusOvulation.
                        //
                        if (nbDaysBtwLiquidMucusOvulation.Count > 0)
                        {
                            ((NimbaNormalCycleStatisticData)this.Statistics[cycleType][StatisticValue.Minimum]).NbDaysBtwLiquidMucusOvulation = nbDaysBtwLiquidMucusOvulation.Min();
                            ((NimbaNormalCycleStatisticData)this.Statistics[cycleType][StatisticValue.Average]).NbDaysBtwLiquidMucusOvulation = nbDaysBtwLiquidMucusOvulation.Average();
                            ((NimbaNormalCycleStatisticData)this.Statistics[cycleType][StatisticValue.Maximum]).NbDaysBtwLiquidMucusOvulation = nbDaysBtwLiquidMucusOvulation.Max();
                        }

                        //
                        // TemperatureOvulation.
                        //
                        if (temperatureOvulation.Count > 0)
                        {
                            ((NimbaNormalCycleStatisticData)this.Statistics[cycleType][StatisticValue.Minimum]).TemperatureOvulation = temperatureOvulation.Min();
                            ((NimbaNormalCycleStatisticData)this.Statistics[cycleType][StatisticValue.Average]).TemperatureOvulation = temperatureOvulation.Average();
                            ((NimbaNormalCycleStatisticData)this.Statistics[cycleType][StatisticValue.Maximum]).TemperatureOvulation = temperatureOvulation.Max();
                        }
                        break;
                    }

                }
            }
        }

        #endregion

        #endregion

        #region Public Methods

        public void Load(NimbaDatetimeSortedSet dates, NimbaConfiguration configuration)
        {
            StateMachineAction nextAction = StateMachineAction.OpenNewCycleAndAddToNewCycle;
            StateMachineState currentState = StateMachineState.TransitionCycleEmpty;
            StateMachineState nextState = StateMachineState.TransitionCycle;
            StateMachineEvent currentEvent = StateMachineEvent.Nothing;
            NimbaCycle currentCycle = null;
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
                    AddTransitionCycleFirstDay(date, currentState, ref dayAfterEndPregnancy, ref currentCycle);

                    //
                    // Determine the current event.
                    //
                    DetermineCurrentEvent(currentState, date, currentCycle, configuration, out currentEvent);

                    //
                    // Determine which action to take.
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

                this.ComputeStatistics(configuration);
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
