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

public class Door :  Interactable, IMQTTComponent
{
    private Animator doorAnimator;
    private bool clientReady = false;

    public bool openDoor = true;
    public bool mqttOpenDoor = false;
    public bool mqttCloseDoor = false;
    public MqttClient client;
    public string m_mainTopic;
    public string m_subTopic;
    public string mainTopic { get; set; }
    public string subTopic { get; set; }

    private void Start() {
        doorAnimator = GetComponent<Animator>();
    }

    public void ClientMqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
    {
        string lastMessage = System.Text.Encoding.UTF8.GetString(e.Message);

        if(e.Topic.Equals(mainTopic + "/" + subTopic)) {
            if(lastMessage.Equals("open")) {
                mqttOpenDoor = true;
            } else if(lastMessage.Equals("close")) {
                mqttCloseDoor = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(clientReady) {
            if(triggerActive && Input.GetKeyDown(KeyCode.E) || (mqttOpenDoor && openDoor) || (mqttCloseDoor && !openDoor)) {
                mqttOpenDoor = false;
                mqttCloseDoor = false;
                ToggleDoor();
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

    private void ToggleDoor()
    {
        string message;
        if(openDoor) {
            message = "open";
        } else {
            message = "close";
        }
        client.Publish(mainTopic + "/" + subTopic, System.Text.Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        doorAnimator.SetBool("open", openDoor);
        openDoor = !openDoor;
    }
}
