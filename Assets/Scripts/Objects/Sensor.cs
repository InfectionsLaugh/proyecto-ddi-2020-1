using System;
using System.Net;
using UnityEngine;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;

public class Sensor : MonoBehaviour
{
    public string m_mainTopic = "sensors";
    public string m_subTopic;
    public string sensorType;
    public int threadDelay;

    private SerialCommunication sc;
    private string[] sensorValues;
    private bool isSerialReady = false;
    private bool isClientReady = false;
    private bool stopThread = false;
    private MqttClient client;

    private Thread readSensor;

    private void Awake() {
        readSensor = new Thread(new ThreadStart(ReadSensor));
        readSensor.Start();
    }

    private void Update()
    {
        if(!isSerialReady) {
            isSerialReady = SetSerialComm();
        }

        if(!isClientReady) {
            isClientReady = SetClient();
        }
    }

    void ReadSensor()
    {
        while(!stopThread) {
            if(isClientReady && isSerialReady) {
                sensorValues = sc.SplittedData();
                if(sensorValues[0] == sensorType) {
                    Debug.Log("Valor de sensor " + sensorType + ": " + sensorValues[1]);
                    client.Publish(m_mainTopic + "/" + m_subTopic, System.Text.Encoding.UTF8.GetBytes(sensorValues[1]), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
                }
            }

            Thread.Sleep(threadDelay);
        }
    }

    bool SetClient()
    {
        client = GameObject.Find("MQTT").GetComponent<MQTTtest>().client;

        if(client == null) {
            return false;
        }

        return true;
    }

    bool SetSerialComm()
    {
        sc = GameObject.Find("SerialComm").GetComponent<SerialCommunication>();

        if(!sc) {
            return false;
        }

        return true;
    }

    private void OnApplicationQuit() {
        stopThread = true;
        readSensor.Join();
    }
}