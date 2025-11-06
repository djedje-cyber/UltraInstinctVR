using System;
using System.Linq;
using UnityEngine;

namespace InriaTools.GraphDisplay
{
    public class Graph
    {
        private static Shader shaderFull;

        public static Shader ShaderFull
        {
            get
            {
                if (shaderFull == null)
                    shaderFull = Shader.Find("Tools/Graph Standard");
                return shaderFull;
            }
        }

        private float limit = 1 / 5f;

        public float Limit
        {
            get
            {
                return limit;
            }
            set
            {
                if (value != limit)
                {
                    limit = value;
                    graphShader.GoodThreshold = limit / max;
                    graphShader.CautionThreshold = max;
                    graphShader.UpdateThresholds();
                }
            }
        }

        private float max = 1;

        public float Max
        {
            get
            {
                return max;
            }
            set
            {
                if (value != max)
                {
                    max = value;
                    graphShader.GoodThreshold = limit / max;
                    graphShader.CautionThreshold = max;
                    graphShader.UpdateThresholds();
                }
            }
        }

        public bool ShowAverage { get; set; }

        private int nextDataId;
        private readonly int m_graphDataSize;
        private readonly float[] dataArray;
        private readonly float[] queueDataArray;

        private readonly GraphShader graphShader = new GraphShader();

        private int dataNb = 0;

        public Graph(UnityEngine.UI.Image image, int graphDataSize = 50)
        {
            graphShader.Image = image;
            if (graphDataSize > 1023)
                graphDataSize = 1023;
            m_graphDataSize = graphDataSize;
            graphShader.ArrayMaxSize = m_graphDataSize;
            dataArray = new float[m_graphDataSize];
            queueDataArray = new float[m_graphDataSize];
            for (int i = 0; i < graphDataSize; i++)
            {
                dataArray[i] = 0;
                queueDataArray[i] = 0;
            }
            graphShader.Image.material = new Material(ShaderFull);
            graphShader.InitializeShader();
            graphShader.Array = dataArray;
            graphShader.UpdateArray();
            graphShader.GoodColor = Color.green;
            graphShader.CautionColor = Color.red;
            graphShader.CriticalColor = Color.red - new Color(0.2f, 0.2f, 0.2f);
            graphShader.UpdateColors();
            graphShader.GoodThreshold = limit / max;
            graphShader.CautionThreshold = max;
            graphShader.UpdateThresholds();
        }

        public void AddData(long data)
        {
            dataArray[nextDataId] = data / Max;
            nextDataId = (nextDataId + 1) % m_graphDataSize;
            if (dataNb < m_graphDataSize)
                dataNb++;
        }

        public void AddData(float data)
        {
            dataArray[nextDataId] = data / Max;
            nextDataId = (nextDataId + 1) % m_graphDataSize;
            if (dataNb < m_graphDataSize)
                dataNb++;
        }

        public void PushData(long data)
        {
            queueDataArray[nextDataId] = data / Max;
            // precalculate next ID
            int newDataId = (nextDataId + 1) % m_graphDataSize;
            // how many data is there after the current ID ?
            int remaining = queueDataArray.Length - newDataId;
            // copy from the next id to the end
            Array.Copy(queueDataArray, newDataId, dataArray, 0, remaining);
            // copy from the begginin to the current id
            Array.Copy(queueDataArray, 0, dataArray, remaining, newDataId);
            nextDataId = newDataId;
            if (dataNb < m_graphDataSize)
                dataNb++;
        }

        public void PushData(float data)
        {
            queueDataArray[nextDataId] = data / Max;
            // precalculate next ID
            int newDataId = (nextDataId + 1) % m_graphDataSize;
            // how many data is there after the current ID ?
            int remaining = queueDataArray.Length - newDataId;
            // copy from the next id to the end
            Array.Copy(queueDataArray, newDataId, dataArray, 0, remaining);
            // copy from the begginin to the current id
            Array.Copy(queueDataArray, 0, dataArray, remaining, newDataId);
            nextDataId = newDataId;
            if (dataNb < m_graphDataSize)
                dataNb++;
        }

        public void UpdateGraph()
        {
            graphShader.Array = dataArray;
            graphShader.UpdatePoints();
            if (ShowAverage && dataNb > 0)
            {
                graphShader.Average = dataArray.Sum() / dataNb;
                graphShader.UpdateAverage();
            }
        }
    }
}
