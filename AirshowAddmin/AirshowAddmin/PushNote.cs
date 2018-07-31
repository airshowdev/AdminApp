using System;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;

namespace AirshowAddmin
{
    public class PushNote
    {
        public static void SendPushNotification(string strTitle, string strBody)
        {
            try
            {
                string APIKey = "AAAAckUMpUU:APA91bE47QVDVjxEaNKYfZwX3CX0iW0jyP6dBVrivora4Hky95zpgtOYh88-9UloNd_o8ueDxo1G7dw5ym3VK7gYlJmFgjZSI34ba-oGphiSBnqpyuwJ2OZ2Z-_mDKL2DRH278ZvZeqy";
                string target = "https://fcm.googleapis.com/fcm/send";


                string message = JsonConvert.SerializeObject(new
                {
                    to = "/topics/Messages",
                    notification = new
                    {
                        body = strBody,
                        title = strTitle,
                        sound = "Enabled"
                    }
                });

                ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => { return true; };

                WebClient client = new WebClient();
                //client.Headers[HttpRequestHeader.ContentType] = "application/json";
                //client.Headers[HttpRequestHeader.Authorization] = "key=" + APIKey;
                client.Headers.Add("Authorization:key=" + APIKey);
                client.Headers.Add("Content-Type:application/json");
                string rawResponse = client.UploadString(new Uri(target), "POST", message);
                //JObject response = JObject.Parse(rawResponse);
                Console.WriteLine(rawResponse);
            }
            catch (Exception E)
            {
                Console.WriteLine(E.ToString());
            }
            /* try
             {
                 string applicationID = "AAAAckUMpUU:APA91bE47QVDVjxEaNKYfZwX3CX0iW0jyP6dBVrivora4Hky95zpgtOYh88-9UloNd_o8ueDxo1G7dw5ym3VK7gYlJmFgjZSI34ba-oGphiSBnqpyuwJ2OZ2Z-_mDKL2DRH278ZvZeqy";
                 string senderId = "490784728389";
                 WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                 tRequest.Method = "post";
                 tRequest.ContentType = "application/json";
                 string data = JsonConvert.SerializeObject(new
                 {
                     to = "/topics/all",
                     notification = new
                     {
                         body = strBody,
                         title = strTitle,
                         sound = "Enabled"
                     }
                 });

                 Byte[] byteArray = Encoding.UTF8.GetBytes(data);
                 tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                 tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                 tRequest.ContentLength = byteArray.Length;

                 using (Stream dataStream = tRequest.GetRequestStream())
                 {
                     dataStream.Write(byteArray, 0, byteArray.Length);
                     using (WebResponse tResponse = tRequest.GetResponse())
                     {
                         using (Stream dataStreamResponse = tResponse.GetResponseStream())
                         {
                             using (StreamReader tReader = new StreamReader(dataStreamResponse))
                             {
                                 String sResponseFromServer = tReader.ReadToEnd();
                                 string str = sResponseFromServer;
                                 Console.WriteLine(str);
                             }
                         }
                     }
                 }
             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex.ToString());
             }*/
        }
    }

}
