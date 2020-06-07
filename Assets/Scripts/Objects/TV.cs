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
using UnityEngine.Video;

public class TV : Interactable, IMQTTComponent
{
    Renderer mat;
    int tvCounter = 0;
    private bool mqttTvOn = false;
    private bool mqttTvOff = false;
    private bool mqttTvVolDown = false;
    private bool mqttTvVolUp = false;
    private bool mqttTvChDown = false;
    private bool mqttTvChUp = false;
    private bool clientReady = false;
    private MqttClient client;

    public VideoClip[] tvShows;
    public VideoPlayer vp;
    public float volume = Mathf.Clamp(1.0f, 0.0f, 1.0f);
    public GameObject player;
    public float steps = 10f;
    public string m_mainTopic = "tv";
    public string m_subTopic;
    public string mainTopic { get; set; }
    public string subTopic { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>();
        // AudioSource audioSource = gameObject.AddComponent<AudioSource>();

        vp.clip = tvShows[0];
        mat.material.SetColor("_Color", Color.black);
        mat.material.SetColor("_EmissionColor", Color.black);
    }

    public void ClientMqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
    {
        string lastMessage = System.Text.Encoding.UTF8.GetString(e.Message);

        if(e.Topic.Equals(mainTopic + "/" + subTopic)) {
            switch(lastMessage) {
                case "on":
                    mqttTvOn = true;
                break;

                case "off":
                    mqttTvOff = true;
                break;

                case "volume_down":
                    mqttTvVolDown = true;
                break;

                case "volume_up":
                    mqttTvVolUp = true;
                break;

                case "channel_down":
                    mqttTvChDown = true;
                break;

                case "channel_up":
                    mqttTvChUp = true;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(clientReady) {
            if(triggerActive && Input.GetKeyDown(KeyCode.E) || (mqttTvOn && !vp.isPlaying) || (mqttTvOff && vp.isPlaying)) {
                mqttTvOn = false;
                mqttTvOff = false;

                if(vp.isPlaying) {
                    mat.material.SetColor("_Color", Color.black);
                    mat.material.SetColor("_EmissionColor", Color.black);
                    client.Publish(mainTopic + "/" + subTopic + "/turned", System.Text.Encoding.UTF8.GetBytes("off"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
                    vp.Stop();
                } else {
                    mat.material.SetColor("_Color", Color.white);
                    mat.material.SetColor("_EmissionColor", Color.white);
                    client.Publish(mainTopic + "/" + subTopic + "/turned", System.Text.Encoding.UTF8.GetBytes("on"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
                    vp.Play();
                }
            }

            if(triggerActive && Input.GetKeyDown(KeyCode.K) || mqttTvChUp) {
                mqttTvChUp = false;
                tvCounter = (tvCounter + 1) % 5;
                vp.Stop();
                vp.clip = tvShows[tvCounter];
                vp.Play();
                client.Publish(mainTopic + "/" + subTopic + "/channel", System.Text.Encoding.UTF8.GetBytes(tvCounter.ToString()), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            }

            if(triggerActive && Input.GetKeyDown(KeyCode.L) || mqttTvChDown) {
                mqttTvChDown = false;
                tvCounter = (tvCounter <= 0) ? tvShows.Length : tvCounter - 1;
                vp.Stop();
                vp.clip = tvShows[tvCounter];
                vp.Play();
                client.Publish(mainTopic + "/" + subTopic + "/channel", System.Text.Encoding.UTF8.GetBytes(tvCounter.ToString()), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            }

            if(triggerActive && Input.GetKeyDown(KeyCode.M) || mqttTvVolUp) {
                mqttTvVolUp = false;
                steps++;
                vp.SetDirectAudioVolume(0, steps / 10);
                client.Publish(mainTopic + "/" + subTopic + "/volume", System.Text.Encoding.UTF8.GetBytes(steps.ToString()), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            }

            if(triggerActive && Input.GetKeyDown(KeyCode.N) || mqttTvVolDown) {
                mqttTvVolDown = false;
                steps--;
                vp.SetDirectAudioVolume(0, steps / 10);
                client.Publish(mainTopic + "/" + subTopic + "/volume", System.Text.Encoding.UTF8.GetBytes(steps.ToString()), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
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
}
