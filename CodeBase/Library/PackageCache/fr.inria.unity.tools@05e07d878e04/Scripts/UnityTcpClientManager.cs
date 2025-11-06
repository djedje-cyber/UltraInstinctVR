using System;
using System.Collections.Generic;

namespace InriaTools
{
    public class UnityTcpClientManager : UnitySingleton<UnityTcpClientManager>
    {
        #region Fields

        protected List<UnityTcpClient> unityTcpClients = new List<UnityTcpClient>();

        #endregion

        #region Methods

        public UnityTcpClient CreateClient(string hostName, int port, Action<string> handleIncomingData = null)
        {
            UnityTcpClient client = new UnityTcpClient(hostName, port, handleIncomingData);
            return client;
        }

        public override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            foreach (UnityTcpClient client in unityTcpClients)
            {
                client.Close();
            }
        }

        internal void Register(UnityTcpClient unityTcpClient)
        {
            if (!unityTcpClients.Contains(unityTcpClient))
                unityTcpClients.Add(unityTcpClient);
        }

        // Update is called once per frame
        private void Update()
        {
            foreach (UnityTcpClient client in unityTcpClients)
            {
                client.TryReceiveDataThroughCallback();
            }
        }

        #endregion
    }
}
