using System.Collections;
using System.Collections.Generic;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using UnityEngine;
using System;

public class RealLamp : Interactable, IMQTTComponent
{
    private bool lightOn = true;
    private bool mqttLightOn = false;
    private bool mqttLightOff = false;
    private bool clientReady = false;
    private MqttClient client;
    public SerialCommunication sc;

    public string m_mainTopic = "lights";
    public string m_subTopic = "real_lamp";
    public string mainTopic { get; set; }
    public string subTopic { get; set; }
    void Start()
    {
        sc = GameObject.Find("SerialComm").GetComponent<SerialCommunication>();
    }

    void Update()
    {
        if(clientReady) {
            if((mqttLightOn && lightOn) || (mqttLightOff && !lightOn)) {
                mqttLightOn = false;
                mqttLightOff = false;
                ToggleLamp();
            }
        } else  {
            clientReady = SetClient();
        }

    }

    public void ClientMqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
    {
        string lastMessage = System.Text.Encoding.UTF8.GetString(e.Message);

        if(e.Topic.Equals(mainTopic + "/" + subTopic) || e.Topic.Equals(mainTopic)) {
            if(lastMessage.Equals("on")) {
                mqttLightOff = true;
            } else if(lastMessage.Equals("off")) {
                mqttLightOn = true;
            }
        }
    }

    void ToggleLamp()
    {
        if(lightOn)
            sc.SendCommData("1");
        else
            sc.SendCommData("0");
        lightOn = !lightOn;
    }

    bool SetClient()
    {
        client = GameObject.Find("MQTT").GetComponent<MQTTtest>().client;

        if(client == null) {
            return false;
        }

        mainTopic = m_mainTopic;
        subTopic = m_subTopic;

        client.MqttMsgPublishReceived += ClientMqttMsgPublishReceived;

        client.Subscribe(new string[] { mainTopic + "/" + subTopic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

        return true;
    }
}