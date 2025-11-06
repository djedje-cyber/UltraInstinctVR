using InriaTools;

using log4net;

using System.Collections.Generic;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Time.Unity
{
    [Renamed("Xareus.Scenarios.Time.Unity.UnityTimeSensor", "Xareus.Unity.Librairies")]
    [FunctionDescription("This time sensor precision depends on Unity's chosen time type : \n" +
        "- Update and Unscaled are checked in Update\n" +
        "- Physics and Unscaled_Physics are checked in Fixed Update\n" +
        "- Realtime is checked in Update and Fixed Update and uses realtimeSinceStartup\n" +
        "This sensor forces the scenario to update when the time is elapsed, resulting in a more precise timing than with Xareus default time sensor",
        "Time", "Unity", "Unity/Time")]
    public class UnityTimeSensor : AUnitySensor
    {
        #region Enums

        public enum TimeType
        {
            Update,
            Unscaled,
            Physics,
            Unscaled_Physics,
            Realtime
        }

        #endregion

        #region Fields

        /// <summary>
        /// The log4net logger
        /// </summary>
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(UnityTimeSensor));

        #endregion



        #region Fields

        [EventContextEntry]
        public static readonly string KEY_TIME = "time";

        /// <summary>
        /// minimum time before triggering (in units).
        /// </summary>
        [ConfigurationParameterAttribute("minimum", "minimum time before triggering. The time is a lower bound. Real time depends on the scenario update frequency.", Necessity.Required)]
        protected double time;

        /// <summary>
        /// time unit used (eg: second, minute)
        /// </summary>
        [ConfigurationParameterAttribute("unit", "time unit used (eg: second, minute, hour). default : second", Necessity.Optional)]
        protected TimeUnit Unit = TimeUnit.SECOND;

        /// <summary>
        /// time unit used (eg: second, minute)
        /// </summary>
        [ConfigurationParameterAttribute("Time type", "Use Unscaled Time", Necessity.Required)]
        protected TimeType timeType;

        /// <summary>
        /// When the sensor should trigger
        /// </summary>
        protected double expectedDateTime;

        protected bool triggered;

        protected SimpleDictionary eventContext = new();

        #endregion

        #region Properties

        /// <summary>
        /// When the sensor started
        /// </summary>
        public double SensorInitializationTime
        {
            get; protected set;
        }

        #endregion

        #region Constructors

        public UnityTimeSensor(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
           : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeReset()
        {
            triggered = false;
            SensorInitializationTime = timeType switch
            {
                TimeType.Unscaled => UnityEngine.Time.unscaledTimeAsDouble,
                TimeType.Physics => UnityEngine.Time.fixedTimeAsDouble,
                TimeType.Unscaled_Physics => UnityEngine.Time.fixedUnscaledTimeAsDouble,
                TimeType.Realtime => UnityEngine.Time.realtimeSinceStartupAsDouble,
                _ =>
#if MIDDLEVR_2
                MiddleVR.MVR.Kernel != null ? MiddleVR.MVR.Kernel.GetTime() / 1000f :
#endif
                UnityEngine.Time.timeAsDouble
            };

            expectedDateTime = SensorInitializationTime + (TimeUtils.ConvertToMillisecond(time, Unit) / 1000f);

            if (LOGGER.IsDebugEnabled)
                LOGGER.Debug("Unity Time Sensor trigger time is " + expectedDateTime);

            if (timeType is TimeType.Update or TimeType.Unscaled or TimeType.Realtime)
                UnityThreadExecute.RegisterActionForExecutionSteps(CheckTime, UnityThreadExecute.UnityExecutionStep.Update);
            if (timeType is TimeType.Physics or TimeType.Unscaled_Physics or TimeType.Realtime)
                UnityThreadExecute.RegisterActionForExecutionSteps(CheckTime, UnityThreadExecute.UnityExecutionStep.FixedUpdate);
        }

        public override void StopCheck()
        {
            base.StopCheck();
            UnityThreadExecute.UnRegisterActionForExecutionSteps(CheckTime, UnityThreadExecute.UnityExecutionStep.Any);
        }

        public override Result SafeSensorCheck()
        {
            return new Result(triggered, eventContext);
        }

        private void CheckTime()
        {
            double currentTime = timeType switch
            {
                TimeType.Unscaled => UnityEngine.Time.unscaledTimeAsDouble,
                TimeType.Physics => UnityEngine.Time.fixedTimeAsDouble,
                TimeType.Unscaled_Physics => UnityEngine.Time.fixedUnscaledTimeAsDouble,
                TimeType.Realtime => UnityEngine.Time.realtimeSinceStartupAsDouble,
                _ =>
#if MIDDLEVR_2
                MiddleVR.MVR.Kernel != null ? MiddleVR.MVR.Kernel.GetTime() / 1000f :
#endif
                UnityEngine.Time.timeAsDouble
            };
            if (expectedDateTime <= currentTime)
            {
                triggered = true;
                eventContext = new SimpleDictionary(KEY_TIME, currentTime - SensorInitializationTime);

                if (LOGGER.IsDebugEnabled)
                    LOGGER.Debug("Unity Time Sensor triggered at " + currentTime);

                UnityThreadExecute.UnRegisterActionForExecutionSteps(CheckTime, UnityThreadExecute.UnityExecutionStep.Any);

                UpdateScenario();
            }
        }

        #endregion
    }
}
