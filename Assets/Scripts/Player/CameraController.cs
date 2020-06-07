using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CameraController : MonoBehaviour
{
    public float cameraSensitivity = 100f;
    public Transform player;
    public float rate = 20f;
    public float lowerLimit = 0.25f;
    public float upperLimit = 1.5f;
    public float dec = -0.1f;

    float xRotation = 0;
    private MqttClient client;
    private bool clientReady = false;
    private bool lightsOn = false;
    private bool lightsOff = false;
    private bool updateLights = false;
    private Thread sendThread;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void UpdateLights()
    {
        Debug.Log("Cambiando luces");
        while(updateLights) {
            for(int i = 0; i < 14; i++) {
                if(lightsOn) {
                    lightsOff = false;
                    client.Publish("outside/light" + i, System.Text.Encoding.UTF8.GetBytes("on"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
                }

                if(lightsOff) {
                    lightsOn = false;
                    client.Publish("outside/light" + i, System.Text.Encoding.UTF8.GetBytes("off"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
                }
            }

            updateLights = false;
        }
        Thread.Sleep(0);
    }

    // Update is called once per frame
    void Update()
    {
        if(!clientReady) {
            clientReady = SetClient();
        }

        RenderSettings.ambientIntensity += dec * Time.deltaTime * rate;
        RenderSettings.skybox.SetFloat("_Rotation", rate * Time.time);

        if(RenderSettings.ambientIntensity <= lowerLimit || RenderSettings.ambientIntensity >= upperLimit) {
            dec *= -1;
        }   

        if(RenderSettings.ambientIntensity < (upperLimit / 2) && clientReady && !lightsOn) {
            Debug.Log("Prendiendo");
            updateLights = true;
            lightsOn = true;
            lightsOff = false;
            sendThread = new Thread(new ThreadStart(UpdateLights));
            sendThread.Start();
            sendThread.Join();
        } else if(RenderSettings.ambientIntensity > (upperLimit / 2) && clientReady && !lightsOff) {
            Debug.Log("Apagando");
            updateLights = true;
            lightsOn = false;
            lightsOff = true;
            sendThread = new Thread(new ThreadStart(UpdateLights));
            sendThread.Start();
            sendThread.Join();
        }


        float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        player.Rotate(Vector3.up * mouseX);
    }

    bool SetClient()
    {
        client = GameObject.Find("MQTT").GetComponent<MQTTtest>().client;

        if(client == null) {
            return false;
        }

        return true;
    }
}
