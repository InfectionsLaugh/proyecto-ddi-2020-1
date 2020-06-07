using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Exceptions;

public class Alarm : Interactable
{
    public string m_mainTopic = "alarm";
    public string m_subTopic;
    public string alarmName;
    public bool alarmActive { get; set; }
    public string mainTopic { get; set; }
    public string subTopic { get; set; }

    private MqttClient client;
    private bool clientReady = false;

    private void Update()
    {
        if(clientReady) {
            if(triggerActive && alarmActive) {
                Debug.Log("Se ha activado la alarma");
                client.Publish(m_mainTopic + "/" + m_subTopic, System.Text.Encoding.UTF8.GetBytes("true"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
                alarmActive = false;
                SendNotification.Send("Se ha detectado movimiento en " + alarmName);
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
        
        return true;
    }
}