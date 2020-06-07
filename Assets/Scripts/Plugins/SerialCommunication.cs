using System;
using UnityEngine;
using System.IO.Ports;
using System.Threading;

public class SerialCommunication : MonoBehaviour
{
    public string port = "COM6"; //When using MacOS -> "/dev/cu.usbmodem1411"
    public int baudRate = 115200;
    SerialPort m_stream;
    Thread receiveDataThread;

    string m_returnData;
    string m_sentData;
    public string ReturnData { get { return m_returnData; } }

    private void Awake() {
        m_stream = new SerialPort(port, baudRate);
        m_stream.Open();
        receiveDataThread = new Thread(new ThreadStart(ReceiveData));
        receiveDataThread.Start();
    }

    private void Update() {
        Thread.Sleep(0);
    }

    void ReceiveData()
    {
        while(true) {
            if(m_stream.IsOpen) {
                m_returnData = m_stream.ReadLine();
                m_stream.BaseStream.Flush();

                if(m_returnData != null) {
                    m_stream.BaseStream.Flush();    
                }
            }

            Thread.Sleep(0);
        }
    }

    public string[] SplittedData()
    {
        string[] tokens = m_returnData.Split(',');
        return tokens;
    }

    public void SendCommData(string data)
    {
        Debug.Log("Se va a enviar");

        if(!m_stream.IsOpen)
            return;

        if(data != null) {
            m_stream.WriteLine(data);
            m_stream.BaseStream.Flush();
        }
    }

    private void OnApplicationQuit() {
        Debug.Log("Se ha cerrado el puerto");
        m_stream.Close();
    }
}
