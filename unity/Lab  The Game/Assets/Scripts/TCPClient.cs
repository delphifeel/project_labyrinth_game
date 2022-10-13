using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPClient
{
    private TcpClient socketConnection;
    private Thread recvThread;
    private string serverAddr;
    private int serverPort;

    public Action<byte[]>   OnReceive   = null;
    public Action           OnReady     = null;

    public TCPClient(string serverAddr, int serverPort)
    {
        this.serverAddr = serverAddr;
        this.serverPort = serverPort;
    }

    public void Connect()
    {
        if (OnReady == null)
        {
            Debug.Log("OnReady is null");
            return;
        }

        if (OnReceive == null)
        {
            Debug.Log("OnReceive is null");
            return;
        }

        try
        {
            recvThread = new Thread(new ThreadStart(OnRecvThreadStart));
            recvThread.IsBackground = true;
            recvThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("recvThread start error: " + e);
        }
    }

    public void Send(byte[] buffer)
    {
        if (socketConnection == null)
        {
            Debug.Log("socket connection is null");
            return;
        }
        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite == false)
            {
                Debug.Log("socker connection is read only");
                return;
            }
  
            stream.WriteAsync(buffer, 0, buffer.Length);
        }
        catch (SocketException socketException)
        {
            Debug.Log("Send socket exception: " + socketException);
        }
    }

    private void OnRecvThreadStart()
    {
        try
        {
            socketConnection = new TcpClient(serverAddr, serverPort);
            OnReady();
            byte[] bytes = new byte[1024];
            while (true)
            {
                // Get a stream object for reading 				
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    // Read incomming stream into byte arrary. 					
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var buffer = new byte[length];
                        Array.Copy(bytes, 0, buffer, 0, length);
                        OnReceive(buffer);
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("OnRecvThreadStart socket exception: " + socketException);
        }
    }
}