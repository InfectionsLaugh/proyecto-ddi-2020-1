using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;

interface IMQTTComponent
{
    string mainTopic { get; set; }
    string subTopic { get; set; }

    void ClientMqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e);
}