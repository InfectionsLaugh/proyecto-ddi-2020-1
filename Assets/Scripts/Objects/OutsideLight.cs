using System.Collections;
using System.Collections.Generic;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using UnityEngine;
using System;

public class OutsideLight : Interactable, IMQTTComponent
{
    private MqttClient client;
    private bool lightOn = true;
    private bool mqttLightOn = false;
    private bool mqttLightOff = false;

    public Light outsideLight;
    public string m_mainTopic = "outside";
    public string m_subTopic;
    public string mainTopic { get; set; }
    public string subTopic { get; set; }

    void Start()
    {
        client = GameObject.Find("MQTT").GetComponent<MQTTtest>().client;
        mainTopic = m_mainTopic;   
        subTopic = m_subTopic;

        client.MqttMsgPublishReceived += ClientMqttMsgPublishReceived;
        outsideLight = transform.Find("Point Light").GetComponent<Light>();
        outsideLight.enabled = false;
        // client.Subscribe(new string[] { mainTopic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        client.Subscribe(new string[] { mainTopic + "/" + subTopic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
    }

    public void ClientMqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
    {
        string lastMessage = System.Text.Encoding.UTF8.GetString(e.Message);

        if(e.Topic.Equals(mainTopic + "/" + subTopic) || e.Topic.Equals(mainTopic)) {
            if(lastMessage.Equals("on")) {
                mqttLightOn = true;
            } else if(lastMessage.Equals("off")) {
                mqttLightOff = true;
            }
        }
    }

    void Update()
    {
        if ((mqttLightOn && lightOn) || (mqttLightOff && !lightOn))
        {
            mqttLightOn = false;
            mqttLightOff = false;
            ToggleLights();
        }
    }

    public void ToggleLights()
    {
        outsideLight.enabled = lightOn;
        lightOn = !lightOn;
    }
}