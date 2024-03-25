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

using NimbaWPF.Data;

namespace NimbaWPF.User_Controls
{


    /// <summary>
    /// Interaction logic for GeneralStatisticsViewControl.xaml
    /// </summary>
    public partial class GeneralStatisticsViewControl : UserControl
    {
        #region Enumerations

        public enum StatisticElement
        {
            CycleDuration,
            PeriodDuration,
            NbDaysBeforeOvulation,
            NbDaysBeforeLiquidMucus,
            NbDaysBetweenLiquidMucusAndOvulation,
            TemperatureOvulation,
        }

        #endregion

        #region Private Members

        private NimbaDataManager _dataManager = null;

        private Dictionary<CycleType, Dictionary<StatisticElement, Dictionary<StatisticValue, TextBlock>>> _statistics = null;

        #endregion

        #region Initializers

        public GeneralStatisticsViewControl(NimbaDataManager dataManager)
        {
            try
            {
                InitializeComponent();

                _dataManager = dataManager;

                InitializeStatistics();

                SetStatistics();
            }
            catch (Exception exception)
            {
                string message = exception.Message;

                throw exception;
            }
        }

        private void InitializeStatistics()
        {
            _statistics = new Dictionary<CycleType, Dictionary<StatisticElement, Dictionary<StatisticValue, TextBlock>>>();

            foreach (CycleType cycleType in Enum.GetValues(typeof(CycleType)))
            {
                if (cycleType == CycleType.None)
                    continue;

                _statistics.Add(cycleType, new Dictionary<StatisticElement, Dictionary<StatisticValue, TextBlock>>());

                foreach (StatisticElement statisticElement in Enum.GetValues(typeof(StatisticElement)))
                {
                    _statistics[cycleType].Add(statisticElement, new Dictionary<StatisticValue, TextBlock>());

                    foreach (StatisticValue statisticValue in Enum.GetValues(typeof(StatisticValue)))
                        _statistics[cycleType][statisticElement].Add(statisticValue, null);
                }
            }

            _statistics[CycleType.Normal][StatisticElement.CycleDuration][StatisticValue.Minimum] = _normalCycleDurationMin;
            _statistics[CycleType.Normal][StatisticElement.CycleDuration][StatisticValue.Average] = _normalCycleDurationAvg;
            _statistics[CycleType.Normal][StatisticElement.CycleDuration][StatisticValue.Maximum] = _normalCycleDurationMax;

            _statistics[CycleType.Normal][StatisticElement.PeriodDuration][StatisticValue.Minimum] = _normalPeriodDurationMin;
            _statistics[CycleType.Normal][StatisticElement.PeriodDuration][StatisticValue.Average] = _normalPeriodDurationAvg;
            _statistics[CycleType.Normal][StatisticElement.PeriodDuration][StatisticValue.Maximum] = _normalPeriodDurationMax;

            _statistics[CycleType.Normal][StatisticElement.NbDaysBeforeOvulation][StatisticValue.Minimum] = _normalNbDaysBeforeOvulationMin;
            _statistics[CycleType.Normal][StatisticElement.NbDaysBeforeOvulation][StatisticValue.Average] = _normalNbDaysBeforeOvulationAvg;
            _statistics[CycleType.Normal][StatisticElement.NbDaysBeforeOvulation][StatisticValue.Maximum] = _normalNbDaysBeforeOvulationMax;

            _statistics[CycleType.Normal][StatisticElement.NbDaysBeforeLiquidMucus][StatisticValue.Minimum] = _normalNbDaysBeforeLiquidMucusMin;
            _statistics[CycleType.Normal][StatisticElement.NbDaysBeforeLiquidMucus][StatisticValue.Average] = _normalNbDaysBeforeLiquidMucusAvg;
            _statistics[CycleType.Normal][StatisticElement.NbDaysBeforeLiquidMucus][StatisticValue.Maximum] = _normalNbDaysBeforeLiquidMucusMax;

            _statistics[CycleType.Normal][StatisticElement.NbDaysBetweenLiquidMucusAndOvulation][StatisticValue.Minimum] = _normalNbDaysBtwLiquidMucusOvulationMin;
            _statistics[CycleType.Normal][StatisticElement.NbDaysBetweenLiquidMucusAndOvulation][StatisticValue.Average] = _normalNbDaysBtwLiquidMucusOvulationAvg;
            _statistics[CycleType.Normal][StatisticElement.NbDaysBetweenLiquidMucusAndOvulation][StatisticValue.Maximum] = _normalNbDaysBtwLiquidMucusOvulationMax;

            _statistics[CycleType.Normal][StatisticElement.TemperatureOvulation][StatisticValue.Minimum] = _normalTemperatureOvulationMin;
            _statistics[CycleType.Normal][StatisticElement.TemperatureOvulation][StatisticValue.Average] = _normalTemperatureOvulationAvg;
            _statistics[CycleType.Normal][StatisticElement.TemperatureOvulation][StatisticValue.Maximum] = _normalTemperatureOvulationMax;

            _statistics[CycleType.Pregnancy][StatisticElement.CycleDuration][StatisticValue.Minimum] = _pregnancyCycleDurationMin;
            _statistics[CycleType.Pregnancy][StatisticElement.CycleDuration][StatisticValue.Average] = _pregnancyCycleDurationAvg;
            _statistics[CycleType.Pregnancy][StatisticElement.CycleDuration][StatisticValue.Maximum] = _pregnancyCycleDurationMax;

            _statistics[CycleType.Pregnancy][StatisticElement.PeriodDuration][StatisticValue.Minimum] = _pregnancyPeriodDurationMin;
            _statistics[CycleType.Pregnancy][StatisticElement.PeriodDuration][StatisticValue.Average] = _pregnancyPeriodDurationAvg;
            _statistics[CycleType.Pregnancy][StatisticElement.PeriodDuration][StatisticValue.Maximum] = _pregnancyPeriodDurationMax;

            _statistics[CycleType.Transition][StatisticElement.CycleDuration][StatisticValue.Minimum] = _transitionCycleDurationMin;
            _statistics[CycleType.Transition][StatisticElement.CycleDuration][StatisticValue.Average] = _transitionCycleDurationAvg;
            _statistics[CycleType.Transition][StatisticElement.CycleDuration][StatisticValue.Maximum] = _transitionCycleDurationMax;

            _statistics[CycleType.Transition][StatisticElement.PeriodDuration][StatisticValue.Minimum] = _transitionPeriodDurationMin;
            _statistics[CycleType.Transition][StatisticElement.PeriodDuration][StatisticValue.Average] = _transitionPeriodDurationAvg;
            _statistics[CycleType.Transition][StatisticElement.PeriodDuration][StatisticValue.Maximum] = _transitionPeriodDurationMax;
        }

        private void SetStatistics()
        {
            SetNormalCycleStatistics();

            SetPregnancyCycleStatistics();

            SetTransitionCycleStatistics();
        }

        private void SetStatisticElement(CycleType cycleType, StatisticElement statisticElement)
        {
            switch (cycleType)
            {
            }
        }

        private void SetNormalCycleStatistics()
        {
            NimbaNormalCycleStatisticData minStatistics = null;
            NimbaNormalCycleStatisticData avgStatistics = null;
            NimbaNormalCycleStatisticData maxStatistics = null;

            if (_dataManager == null)
                return;

            minStatistics = (NimbaNormalCycleStatisticData)_dataManager.Statistics[CycleType.Normal][StatisticValue.Minimum];
            avgStatistics = (NimbaNormalCycleStatisticData)_dataManager.Statistics[CycleType.Normal][StatisticValue.Average];
            maxStatistics = (NimbaNormalCycleStatisticData)_dataManager.Statistics[CycleType.Normal][StatisticValue.Maximum];

            _normalCycleDurationMin.Text = minStatistics.CycleDuration.ToString();
            _normalCycleDurationAvg.Text = avgStatistics.CycleDuration.ToString();
            _normalCycleDurationMax.Text = maxStatistics.CycleDuration.ToString();

            _normalPeriodDurationMin.Text = minStatistics.PeriodDuration.ToString();
            _normalPeriodDurationAvg.Text = avgStatistics.PeriodDuration.ToString();
            _normalPeriodDurationMax.Text = maxStatistics.PeriodDuration.ToString();

            _normalNbDaysBeforeOvulationMin.Text = minStatistics.NbDaysBeforeOvulation.ToString();
            _normalNbDaysBeforeOvulationAvg.Text = avgStatistics.NbDaysBeforeOvulation.ToString();
            _normalNbDaysBeforeOvulationMax.Text = maxStatistics.NbDaysBeforeOvulation.ToString();

            _normalNbDaysBeforeLiquidMucusMin.Text = minStatistics.NbDaysBeforeLiquidMucus.ToString();
            _normalNbDaysBeforeLiquidMucusAvg.Text = avgStatistics.NbDaysBeforeLiquidMucus.ToString();
            _normalNbDaysBeforeLiquidMucusMax.Text = maxStatistics.NbDaysBeforeLiquidMucus.ToString();

            _normalNbDaysBtwLiquidMucusOvulationMin.Text = minStatistics.NbDaysBtwLiquidMucusOvulation.ToString();
            _normalNbDaysBtwLiquidMucusOvulationAvg.Text = avgStatistics.NbDaysBtwLiquidMucusOvulation.ToString();
            _normalNbDaysBtwLiquidMucusOvulationMax.Text = maxStatistics.NbDaysBtwLiquidMucusOvulation.ToString();

            _normalTemperatureOvulationMin.Text = minStatistics.TemperatureOvulation.ToString();
            _normalTemperatureOvulationAvg.Text = avgStatistics.TemperatureOvulation.ToString();
            _normalTemperatureOvulationMax.Text = maxStatistics.TemperatureOvulation.ToString();
        }

        private void SetPregnancyCycleStatistics()
        {
            if (_dataManager == null)
                return;

            _pregnancyCycleDurationMin.Text = _dataManager.Statistics[CycleType.Pregnancy][StatisticValue.Minimum].CycleDuration.ToString();
            _pregnancyCycleDurationAvg.Text = _dataManager.Statistics[CycleType.Pregnancy][StatisticValue.Average].CycleDuration.ToString();
            _pregnancyCycleDurationMax.Text = _dataManager.Statistics[CycleType.Pregnancy][StatisticValue.Maximum].CycleDuration.ToString();

            _pregnancyPeriodDurationMin.Text = _dataManager.Statistics[CycleType.Pregnancy][StatisticValue.Minimum].PeriodDuration.ToString();
            _pregnancyPeriodDurationAvg.Text = _dataManager.Statistics[CycleType.Pregnancy][StatisticValue.Average].PeriodDuration.ToString();
            _pregnancyPeriodDurationMax.Text = _dataManager.Statistics[CycleType.Pregnancy][StatisticValue.Maximum].PeriodDuration.ToString();
        }

        private void SetTransitionCycleStatistics()
        {
            if (_dataManager == null)
                return;

            _transitionCycleDurationMin.Text = _dataManager.Statistics[CycleType.Transition][StatisticValue.Minimum].CycleDuration.ToString();
            _transitionCycleDurationAvg.Text = _dataManager.Statistics[CycleType.Transition][StatisticValue.Average].CycleDuration.ToString();
            _transitionCycleDurationMax.Text = _dataManager.Statistics[CycleType.Transition][StatisticValue.Maximum].CycleDuration.ToString();

            _transitionPeriodDurationMin.Text = _dataManager.Statistics[CycleType.Transition][StatisticValue.Minimum].PeriodDuration.ToString();
            _transitionPeriodDurationAvg.Text = _dataManager.Statistics[CycleType.Transition][StatisticValue.Average].PeriodDuration.ToString();
            _transitionPeriodDurationMax.Text = _dataManager.Statistics[CycleType.Transition][StatisticValue.Maximum].PeriodDuration.ToString();
        }

        #endregion
    }

    

}
