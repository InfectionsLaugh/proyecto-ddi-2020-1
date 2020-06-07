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

public class Lightswitch : Interactable, IMQTTComponent
{
    public Light[] lights;
    private bool lightOn = true;
    private bool mqttLightOn = false;
    private bool mqttLightOff = false;
    private bool clientReady = false;
    private MqttClient client;

    public GameObject[] lamps;
    public string m_mainTopic = "lights";
    public string m_subTopic;
    public string mainTopic { get; set; }
    public string subTopic { get; set; }

    private void Start()
    {
        lights = new Light[lamps.Length];

        for(int i = 0; i < lamps.Length; i++) {
            lights[i] = lamps[i].transform.Find("Point Light").GetComponent<Light>();
            lights[i].enabled = false;
        }
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

    private void Update()
    {
        if(clientReady) {
            if (triggerActive && Input.GetKeyDown(KeyCode.E) || (mqttLightOn && lightOn) || (mqttLightOff && !lightOn))
            {
                mqttLightOn = false;
                mqttLightOff = false;
                ToggleLights();
            }
        } else {
            clientReady = SetClient();
        }
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

    public void ToggleLights()
    {
        string message;
        if(lightOn) {
            message = "on";
        } else {
            message = "off";
        }
        client.Publish(mainTopic + "/" + subTopic, System.Text.Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        foreach(Light light in lights) {
            light.enabled = lightOn;
        }
        lightOn = !lightOn;
    }
}
