using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using System.Threading;

public class SendNotification : MonoBehaviour {

    private static Thread sendThread;
    private static byte[] byteArray;

    public static void Send(string message)
    {
        byteArray = Encoding.UTF8.GetBytes("{"
                                                + "\"app_id\": \"1ff9335a-4f61-49a7-b003-5e837241d3cc\","
                                                + "\"contents\": {\"en\": \"" + message + "\"},"
                                                + "\"included_segments\": [\"All\"]}");

        sendThread = new Thread(new ThreadStart(SendThread));
        sendThread.Start();
    }

    static void SendThread() {
        var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

        request.KeepAlive = true;
        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";

        request.Headers.Add("authorization", "Basic MjVmMmFlMTQtOTEwYy00ZjcyLTg2ODktNDUwM2Q2ZTYxMmVi");

        string responseContent = null;

        try {
            using (var writer = request.GetRequestStream()) {
                writer.Write(byteArray, 0, byteArray.Length);
            }

            using (var response = request.GetResponse() as HttpWebResponse) {
                using (var reader = new StreamReader(response.GetResponseStream())) {
                    responseContent = reader.ReadToEnd();
                }
            }
        }
        catch (WebException ex) {
            Debug.Log(ex.Message);
            Debug.Log(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
        }

        Debug.Log(responseContent);
    }

}