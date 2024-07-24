using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using UnityEngine;


namespace Jois
{
    /// <summary>
    /// Client class shows how to implement and use TcpClient in Unity.
    /// </summary>
    public class TcpClientA : MonoBehaviour
    {
        #region Public Variables

        [Header("Network")]
        // public string ipAddress = "127.0.0.1";
        // public int port = 54010;
        public float waitingMessagesFrequency = 2;

        #endregion

        #region Private m_Variables

        private TcpClient m_Client;
        private NetworkStream m_NetStream = null;
        private byte[] m_Buffer = new byte[1024 * 48];
        private int m_BytesReceived = 0;
        private string m_ReceivedMessage = "";
        private IEnumerator m_ListenServerMsgsCoroutine = null;

        #endregion

        #region Delegate Variables

        protected Action OnClientStarted = null; //Delegate triggered when client start
        protected Action OnClientClosed = null; //Delegate triggered when client close

        #endregion

        //Start client and establish connection with server
        public void Connect(string ipAddress, int port, Action<bool> onFinish)
        {
            //Early out
            if (m_Client != null)
            {
                Debug.LogWarning("There is already a running client \n");
                return;
            }

            try
            {
                //Create new client
                m_Client = new TcpClient();
                //Set and enable client
                m_Client.Connect(ipAddress, port);
                Debug.LogFormat("Client Started ip({0}) port({1})\n", ipAddress, port);
                OnClientStarted?.Invoke();

                onFinish(true);
                
                //Start Listening Server Messages coroutine
                m_ListenServerMsgsCoroutine = ListenServerMessages();
                StartCoroutine(m_ListenServerMsgsCoroutine);
            }
            catch (SocketException ex)
            {
                Debug.LogWarningFormat("Socket Exception: Start Server first ({0})\n", ex.Message);
                Disconnect();
                onFinish(false);
            }
        }
        public void Disconnect()
        {
            //Reset everything to defaults   
            if (m_Client != null)
            {
                Debug.LogFormat("TcpClientA: Client Closed");
                if (m_Client.Connected)
                    m_Client.Close();
                m_Client = null;
                m_NetStream = null;
            }

            OnClientClosed?.Invoke();
        }
        

        #region Communication Client<->Server

        //Coroutine waiting server messages
        private IEnumerator ListenServerMessages()
        {
            //early out if there is nothing connected       
            if (!m_Client.Connected)
                yield break;

            //Stablish Client NetworkStream information
            m_NetStream = m_Client.GetStream();

            //Start Async Reading from Server and manage the response on MessageReceived function
            do
            {
                if (m_Client == null)
                    break;
                
                // Debug.Log("Client is listening server msg... \n");
                
                //Start Async Reading from Server and manage the response on MessageReceived function
                m_NetStream.BeginRead(m_Buffer, 0, m_Buffer.Length, MessageReceived, null);

                if (m_BytesReceived > 0)
                {
                    OnMessageReceived(m_ReceivedMessage);
                    m_BytesReceived = 0;
                }

                yield return new WaitForSeconds(waitingMessagesFrequency);
                
            } while (m_BytesReceived >= 0 && m_NetStream != null);

            Debug.Log("ListenServerMessages Loop has Done!\n");
            //The communication is over
            //CloseClient();
        }

        //What to do with the received message on client
        protected virtual void OnMessageReceived(string receivedMessage)
        {
            Debug.LogFormat("Msg received on Client: <b>{0}</b>", receivedMessage);
            switch (m_ReceivedMessage)
            {
                case "Close":
                    Disconnect();
                    break;
                default:
                    Debug.LogFormat("Received message: {0}, has no special behaviour \n", receivedMessage);
                    break;
            }
        }

        //Send custom string msg to server
        public void SendStringMsg(string sendMsg)
        {
            //early out if there is nothing connected       
            if (m_Client == null || m_NetStream == null | !m_Client.Connected)
            {
                Debug.LogWarningFormat("Socket Error: Establish Server connection first");
                return;
            }

            //Build message to server
            byte[] msg = Encoding.ASCII.GetBytes(sendMsg); //Encode message as bytes
            
            //Start Sync Writing
            m_NetStream.Write(msg, 0, msg.Length);
            Debug.LogFormat("Sent msg: {0}\n", sendMsg);
        }

        //AsyncCallback called when "BeginRead" is ended, waiting the message response from server
        private void MessageReceived(IAsyncResult result)
        {
            if (m_Client != null && m_NetStream != null && result.IsCompleted && m_Client.Connected)
            {
                //build message received from server
                m_BytesReceived = m_NetStream.EndRead(result);
                m_ReceivedMessage = Encoding.ASCII.GetString(m_Buffer, 0, m_BytesReceived);
            }
        }

        #endregion
    }
}