using InriaTools.Timer;
using UnityEngine;
using UnityEngine.UI;

namespace InriaTools.GraphDisplay
{
    /// <summary>
    /// Delay : shows how precise the timer is
    /// EventExecutionTime : show how long the execution of the loop is
    /// </summary>
    public class TimerGraphDisplay : MonoBehaviour
    {

        public enum EDataType
        {
            Delay = 0,
            EventExecutionTime = 1
        };

        protected ITimer timer;

        protected Graph graph;

        protected long lastValue;


        public Image Image { get; private set; }

        public bool PushMode { get; set; } = false;

        public EDataType DataType { get; set; } = EDataType.Delay;

        public void ShowTimer(ITimer timer)
        {
            if (this.timer == null)
            {
                this.timer = timer;
                graph = new Graph(Image, (int)(5 * 1 * 1000 * 1000 / timer.Interval));
                timer.Elapsed += UpdateGraphData;
                graph.Limit = timer.Interval;
                graph.Max = timer.Interval * 5;
            }
        }

        // Update is called once per frame
        protected void Update()
        {
            if (graph != null)
                graph.UpdateGraph();
        }

        private void UpdateGraphData(object sender, TimerElapsedEventArgs e)
        {
            if (graph != null)
            {
                switch (DataType)
                {
                    case EDataType.EventExecutionTime:
                        lastValue = e.PreviousElapsedEventExecutionTime;
                        break;
                    case EDataType.Delay:
                        lastValue = e.Delay;
                        break;
                    default:
                        lastValue = e.Delay;
                        break;
                }
                if (!PushMode)
                    graph.AddData(lastValue);
                else
                    graph.PushData(lastValue);
            }
        }
    }
}