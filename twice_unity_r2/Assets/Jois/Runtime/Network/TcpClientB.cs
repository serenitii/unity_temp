using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Jois
{
    public class TcpClientB : MonoBehaviour
    {
        TcpClient _tcp;
        NetworkStream _stream;
        IPHostEntry _ipHostEntry;

        Action<bool> _connectionCallback;
        string _hostname;
        int _port;

        byte[] _readBuffer = new byte[kMaxReadSize];

        int _readOffset;
        int _leftSize;

        // buffer, size
        public Action<byte[], int> OnMessageRead;

        const int kLengthSize = 2;
        const int kHeaderSize = 12;
        const int kMaxReadSize = 1024 * 10 * 4;

        void Update()
        {
            if (_stream != null && _stream.CanRead && _stream.DataAvailable)
            {
                //Debug.AssertFormat(leftSize > 0, "should leftSize > 0, but {0}", leftSize);

                #if false
                int readSize = _stream.Read(readBuffer, readOffset, leftSize);
                readOffset += readSize;


                int packetLength = JLittleEndian.Read_u16(readBuffer);
                leftSize = packetLength - kHeaderSize;

                while (leftSize > 0)
                {
                    readSize = _stream.Read(readBuffer, readOffset, leftSize);
                    readOffset += readSize;
                    leftSize -= readSize;
                }

                // Complete
                if (leftSize == 0)
                {
                    OnReadMessage_(readBuffer, readOffset);

                    readOffset = 0;
                    leftSize = kHeaderSize;
                }
#else
                var readSize = _stream.Read(_readBuffer, 0, kMaxReadSize);
                OnMessageRead(_readBuffer, readSize);
#endif
            }
        }

        public void Connect(string hostname, int port, Action<bool> callback, float timeout)
        {
            _connectionCallback = callback;
            _hostname = hostname;
            _port = port;
            _readOffset = 0;
            _leftSize = kHeaderSize;

            Debug.Assert(_tcp == null);
            Debug.Assert(_readBuffer != null, "should be explicitly assigned a readbuffer ");
            Debug.LogFormat("Trying to connect to {0}:{1}\n", hostname, port);

            try
            {
                _ipHostEntry = Dns.GetHostEntry(hostname);

                _tcp = new TcpClient(AddressFamily.InterNetworkV6);
                // _tcp = new TcpClient
                // {
                //     Client = ,
                //     ExclusiveAddressUse = ,
                //     LingerState = ,
                //     NoDelay = true,
                //     ReceiveBufferSize = ,
                //     ReceiveTimeout = 1000,
                //     SendBufferSize = ,
                //     SendTimeout = 1000
                // };
                
                _tcp.Connect(_ipHostEntry.AddressList, port);

                StartCoroutine(CheckConnected(0.5f, timeout));
            }
            catch (ArgumentNullException e)
            {
                Debug.LogFormat("ArgumentNullException: {0} ", e);
                Disconnect();
            }
            catch (SocketException e)
            {
                Debug.LogFormat("SocketException-> {0} ", e);
                callback(false);
                Disconnect();
            }
        }

        public void Disconnect()
        {
            if (_tcp != null)
            {
                if (_tcp.Connected)
                    Debug.LogFormat("Disconnected ({0}, {1})\n", _ipHostEntry.HostName, _port);

                _ipHostEntry = null;
                _stream = null;
                _tcp.Close();
                _tcp = null;
            }

            gameObject.SetActive(false);
        }


        IEnumerator CheckConnected(float checkInterval, float timeout)
        {
            int count = (int) (timeout / checkInterval);
            float elapsedTime = 0f;

            for (int i = 0; i < count; ++i)
            {
                yield return new WaitForSeconds(checkInterval);

                elapsedTime += checkInterval;

                if (_tcp.Connected)
                {
                    Debug.LogFormat("u(-1) Successfully Connected to {0} {1} \n", _hostname, _port);

                    _stream = _tcp.GetStream();

                    if (_connectionCallback != null)
                    {
                        _connectionCallback(true);
                        _connectionCallback = null;
                    }

                    yield break;
                }
                else
                {
                    if (elapsedTime >= timeout)
                    {
                        if (_connectionCallback != null)
                        {
                            _connectionCallback(false);
                            _connectionCallback = null;
                        }

                        Disconnect();
                        yield break;
                    }
                }
            }
        }

        public void WriteMessage(byte[] buf, int size)
        {
            try
            {
                NetworkStream stream = _tcp.GetStream();
                if (stream.CanWrite)
                {
                    // Write byte array to socketConnection stream.
                    stream.Write(buf, 0, size);
                }
                else
                {
                    Debug.LogError("Could not TcpClientBase.WriteMessage \n");
                }
            }
            catch (SocketException socketException)
            {
                Debug.Log("Socket exception-> " + socketException);
            }
        }
    }
}
