using System;

public class PushNote
{
    public PushNote()
    {

    }

    public async void SendPushNotification(string strTitle, string strBody, string strSound)
    {



        var registrationId = "/topics/all";

        using (var sender = new Sender("AIzaSyBStVTKLxZC21zf0UbQlm7KinAOuc3slJ4"))
        {
            var message = new Message
            {
                RegistrationIds = new List<string> { registrationId },
                Notification = new Notification
                {
                    Title = strTitle,
                    Body = strBody,

                }
            };
            var result = await sender.SendAsync(message);
            Console.WriteLine($"Success: {result.MessageResponse.Success}");

            //var json = "{\"notification\":{\"title\":\"json message\",\"body\":\"works like a charm!\"},\"to\":\"" + registrationId + "\"}";
            //result = await sender.SendAsync(json);
            //Console.WriteLine($"Success: {result.MessageResponse.Success}");
        }
        //    try
        //    {

        //        string appID = "AAAAckUMpUU:APA91bE47QVDVjxEaNKYfZwX3CX0iW0jyP6dBVrivora4Hky95zpgtOYh88-9UloNd_o8ueDxo1G7dw5ym3VK7gYlJmFgjZSI34ba-oGphiSBnqpyuwJ2OZ2Z-_mDKL2DRH278ZvZeqy";
        //        string senderID = "490784728389";
        //        string deviceID = "topics/all";
        //        var data = new
        //        {
        //            to = deviceID,
        //            notification = new
        //            {
        //                body = strBody,
        //                title = strTitle,
        //                //delivery_receipt_requested = true
        //            }
        //        };

        //        string toJson = JsonConvert.SerializeObject(data);

        //        var request = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
        //        request.Method = "post";
        //        request.ContentType = "application/json";
        //        request.Headers.Add(string.Format("Authorization: key={0}", appID));
        //        request.Headers.Add(string.Format("Sender: id={0}", senderID));
        //        Byte[] dataArray = Encoding.UTF8.GetBytes(toJson);
        //        request.ContentLength = dataArray.Length;
        //        using (Stream dataStream = request.GetRequestStream())
        //        {
        //            dataStream.Write(dataArray, 0, dataArray.Length);
        //            using (WebResponse response = request.GetResponse())
        //            {
        //                using (Stream dataStreamResponse = response.GetResponseStream())
        //                {
        //                    using(StreamReader reader = new StreamReader(dataStreamResponse))
        //                    {
        //                        Console.WriteLine("Request looks like: "+ request.ToString());
        //                        String sResponseFromServer = reader.ReadToEnd();
        //                        string strResponse = sResponseFromServer;
        //                        Console.WriteLine(strResponse);
        //                    }
        //                }
        //            }
        //        }
        //        //    request.GetRequestStream().Write(buffer, 0, buffer.Length);
        //        //var response = request.GetResponse();
        //        //Console.WriteLine(response.ToString());
        //        //toJson = (new StreamReader(response.GetResponseStream())).ReadToEnd();

        //        //Console.WriteLine();

        //    }
        //    catch(Exception e)
        //    {
        //        Console.WriteLine(e.ToString());
        //    }
    }
}
