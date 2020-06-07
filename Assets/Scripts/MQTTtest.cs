using System.Collections;
using System.Collections.Generic;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MQTTtest : MonoBehaviour
{
    public string clientIP = "192.168.1.71"; // Dirección IP del broker
    public MqttClient client;                // Objeto del cliente MQTT que tendremos en nuestro juego
    public int brokerPort = 1883;            // Puerto del broker

    void Start()
    {
        client = new MqttClient(IPAddress.Parse(clientIP), brokerPort, false, null);
        string clientID = Guid.NewGuid().ToString();

        client.Connect(clientID);
    }

	void OnApplicationQuit()
	{
		client.Disconnect();
	}
}
