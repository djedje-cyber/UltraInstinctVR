using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

using UnityEngine;

namespace InriaTools
{
    public class UnityTcpClient
    {
        #region Fields

        public object socketLock = new object();
        public bool DebugMode = false;
        private TcpClient tcpClient = new TcpClient();
        private int serverPort = 4455;
        private string serverHostName = "localhost";
        private bool tryingToConneectToServer;

        private Action<string> incomingDataCallback;

        #endregion

        #region Properties

        public bool IsConnectedToServer { get; private set; }

        #endregion

        #region Constructors

        public UnityTcpClient(string hostName, int port, Action<string> handleIncomingData)
        {
            serverPort = port;
            serverHostName = hostName;
            incomingDataCallback = handleIncomingData;
            LaunchConnectToServerThread();
            RegisterToManager();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Check if data is available
        /// </summary>
        /// <returns></returns>
        public bool HasDataAvailable()
        {
            lock (socketLock)
            {
                return HasDataAvailableNOTThreadSafe();
            }
        }

        public void TryReceiveDataThroughCallback()
        {
            incomingDataCallback?.Invoke(TryReceiveData());
        }

        /// <summary>
        /// Receive data
        /// </summary>
        /// <returns>string.Empty if no data was available</returns>
        public string TryReceiveData()
        {
            lock (socketLock)
            {
                if (HasDataAvailableNOTThreadSafe())
                {
                    byte[] bytesFrom = new byte[tcpClient.Client.ReceiveBufferSize];
                    tcpClient.Client.Receive(bytesFrom);
                    string dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Trim(new char[] { '\n', '\0', '\r', ' ' });
                    return dataFromClient;
                }
            }
            return string.Empty;
        }

        public void Close()
        {
            try
            {
                lock (socketLock)
                {
                    if (tcpClient != null && tcpClient.Connected)
                    {
                        Debug.LogFormat("Closing connection to server {0}:{1}...", serverHostName, serverPort);
                        SendMessage("Disconnecting");
                        tcpClient.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public void LaunchConnectToServerThread()
        {
            if (!tryingToConneectToServer)
            {
                tryingToConneectToServer = true;
                SeparateThread.Instance.ExecuteInThread(() =>
                {
                    TryToConnectToServer();
                }, () =>
                {
                    tryingToConneectToServer = false;
                });
            }
        }

        public bool SendMessage(string v)
        {
            try
            {
                lock (socketLock)
                {
                    if (tcpClient == null)
                        return false;
                    if (DebugMode)
                        Debug.Log("message : " + v);
                    if (!v.EndsWith("\n"))
                        v += "\n";

                    NetworkStream serverStream = tcpClient.GetStream();
                    byte[] outStream = System.Text.Encoding.ASCII.GetBytes(v);
                    serverStream.Write(outStream, 0, outStream.Length);
                    serverStream.Flush();
                }
            }
            catch (IOException)
            {
                Debug.LogWarningFormat("Connection to server {0}:{1} lost, trying to reconnect ...", serverHostName, serverPort);
                LaunchConnectToServerThread();
                return false;
            }
            return true;
        }

        private bool HasDataAvailableNOTThreadSafe()
        {
            return tcpClient != null && tcpClient.Client.Available > 0;
        }

        private void RegisterToManager()
        {
            UnityTcpClientManager.Instance.Register(this);
        }

        private void TryToConnectToServer()
        {
            Debug.LogFormat("Connecting to server {0} ({1}:{2}) ...", serverHostName, Dns.GetHostEntry(serverHostName).AddressList[0], serverPort);
            try
            {
                lock (socketLock)
                {
                    if (tcpClient != null)
                        tcpClient.Close();
                    tcpClient = new TcpClient();
                    tcpClient.Connect(serverHostName, serverPort);
                }
                Debug.LogFormat("Connected to server {0} ({1}:{2})", serverHostName, Dns.GetHostEntry(serverHostName).AddressList[0], serverPort);
                IsConnectedToServer = true;
            }
            catch (SocketException e)
            {
                Debug.LogWarningFormat("Connection to server {0} ({1}:{2}) failed with message : {3}.\nIt's ok if you are not launching this in Immersia ", serverHostName, Dns.GetHostEntry(serverHostName).AddressList[0], serverPort, e.Message);
            }
        }

        #endregion
    }
}
