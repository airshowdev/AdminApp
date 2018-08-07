namespace AirshowAddmin
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using System.Net;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;


    public static class InfoStore
    {
        public static Database database
        {
            get;
            set;
        }

        public static string Selected { get; set; }

        public static User CurrentUser { get; set; }

        public static int performerIndex { get; set; }

        public static int foodIndex { get; set; }

        public static int staticIndex;

        public static List<string> getStaticNames()
        {
            List<string> names = new List<string>();
            foreach (Static stat in Database.Airshows[InfoStore.getAirshowIndex(InfoStore.Selected)].Statics)
            {
                if (stat.Name != null)
                {
                    names.Add(stat.Name);
                }
            }
            return names.Count >= 1 ? names : null;
        }

        public static List<string> statusOptions = new List<string>();

        public static Static getStaticByName(string staticName)
        {
            int intIndex = 0;
            foreach (Static stat in Database.Airshows[InfoStore.getAirshowIndex(InfoStore.Selected)].Statics)
            {
                if (stat.Name == staticName)
                {
                    staticIndex = intIndex;
                    return stat;
                }
                intIndex++;
            }
            return new Static("", "", "");
        }

        public static Direction getDirectionByName(string directionName)
        {
            int intIndex = 0;
            List<Direction> directions = Database.Airshows[(InfoStore.getAirshowIndex(InfoStore.Selected))].Directions ?? new List<Direction>();
            foreach (Direction dir in directions)
            {
                if (dir.Name == directionName)
                {
                    return dir;
                }
                intIndex++;
            }
            return new Direction(false, "", "", 0, 0);
        }

        public static List<string> getDirectionNames()
        {
            List<string> Names = new List<string>();
            List<Direction> directions = Database.Airshows[InfoStore.getAirshowIndex(InfoStore.Selected)].Directions ?? new List<Direction>();
            foreach (Direction dir in directions)
            {
                Names.Add(dir.Name);
            }
            return Names;
        }

        public static Performer getPerformerByName(string performerName)
        {
            int intIndex = 0;
            foreach (Performer perf in Database.Airshows[InfoStore.getAirshowIndex(InfoStore.Selected)].Performers)
            {
                if (perf.Name == performerName)
                {
                    performerIndex = intIndex;
                    return perf;
                }
                intIndex++;
            }
            return new Performer("", "", "Preparing", "", Convert.ToString(Database.Airshows[InfoStore.getAirshowIndex(InfoStore.Selected)].Performers.Count + 1));
        }

        public static Food getFoodByName(string foodName)
        {
            int intIndex = 0;
            foreach (Food foo in Database.Airshows[InfoStore.getAirshowIndex(InfoStore.Selected)].Foods)
            {
                if (foo.Name == foodName)
                {
                    foodIndex = intIndex;
                    return foo;
                }
                intIndex++;
            }
            return new Food("", "");
        }

        public static int getAirshowIndex(string airshowName)
        {
            for (int i = 0; i < Database.Airshows.Count; i++)
            {
                if (Database.Airshows[i].Name == InfoStore.Selected)
                {
                    return i;
                }
            }
            return -1;
        }

        public static List<string> getPerformerNames()
        {
            List<string> Names = new List<string>();
            foreach (Performer perf in Database.Airshows[InfoStore.getAirshowIndex(InfoStore.Selected)].Performers)
            {
                Names.Add(perf.Name);
            }
            return Names;
        }

        public static List<string> getFoodNames()
        {
            List<string> Foods = new List<string>();
            foreach (Food foo in Database.Airshows[InfoStore.getAirshowIndex(InfoStore.Selected)].Foods)
            {
                Foods.Add(foo.Name);
            }
            return Foods;
        }

        public static void Clear()
        {
            InfoStore.database = null;
            Database.Airshows = null;
        }

        public static void getDatabase()
        {
            InfoStore.Clear();
            statusOptions.Clear();
            statusOptions.Add("Preparing");
            statusOptions.Add("On-Deck");
            statusOptions.Add("In-Air");
            statusOptions.Add("Completed");


            string target = "https://airshowapp-d193b.firebaseio.com/.json?auth=" + InfoStore.CurrentUser.token;

            ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => { return true; };

            try
            {
                WebClient client = new WebClient();
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                string rawResponse = client.DownloadString(new Uri(target));
                Console.WriteLine(rawResponse);
                database = Database.FromJson(rawResponse);;
            }
            catch (Exception E)
            {
                Console.WriteLine(E.ToString());
            }

        }
    }

    public partial class Database
    {
        [JsonProperty("Airshows")]
        public static List<Airshow> Airshows { get; set; }

        [JsonProperty("Messages")]
        public static Dictionary<string, Message> Messages { get; set; }

        [JsonProperty("Questions")]
        public static List<Question> Questions { get; set; }

        [JsonIgnore]
        public static List<string> AirshowNames
        {
            get
            {
                List<string> names = new List<string>();
                foreach (Airshow airshow in Airshows)
                {
                    if (airshow != null)
                    {
                        names.Add(airshow.Name);
                    }
                }
                return names;
            }
        }



        public static Dictionary<string, object> getAirshowInfo(string airshowName)
        {
            IList<Airshow> airshows = Airshows;
            Dictionary<string, object> properties = new Dictionary<string, object>();
            Airshow airshowSelected = null;
            foreach (Airshow airshow in Airshows)
            {
                if (airshow != null && airshow.Name == airshowName)
                {
                    airshowSelected = airshow;
                }
            }
            properties.Add("Performers", airshowSelected.Performers);
            properties.Add("Statics", airshowSelected.Statics);
            properties.Add("Foods", airshowSelected.Foods);
            properties.Add("Directions", airshowSelected.Directions);
            properties.Add("Name", airshowSelected.Name);
            properties.Add("Description", airshowSelected.Description);
            properties.Add("Date", airshowSelected.Date);
            properties.Add("Base", airshowSelected.Base);
            properties.Add("Last Update", airshowSelected.LastUpdate);
            properties.Add("Sponsors", airshowSelected.Sponsors);
            properties.Add("Facebook Link", airshowSelected.FacebookLink);
            properties.Add("Twitter Link", airshowSelected.TwitterLink);
            properties.Add("Website Link", airshowSelected.WebsiteLink);
            properties.Add("Instagram Link", airshowSelected.InstagramLink);

            return properties;
        }
    }

    public partial class Airshow
    {
        [JsonProperty("Base")]
        public string Base { get; set; }

        [JsonProperty("Date")]
        public string Date { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Directions")]
        public List<Direction> Directions { get; set; }

        [JsonProperty("Facebook Link")]
        public string FacebookLink { get; set; }

        [JsonProperty("Foods")]
        public List<Food> Foods { get; set; }

        [JsonProperty("Instagram Link")]
        public string InstagramLink { get; set; }

        [JsonProperty("Last Updated By")]
        public string LastUpdate { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Performers")]
        public List<Performer> Performers { get; set; }

        [JsonProperty("Sponsors")]
        public string Sponsors { get; set; }

        [JsonProperty("Statics")]
        public List<Static> Statics { get; set; }

        [JsonProperty("Twitter Link")]
        public string TwitterLink { get; set; }

        [JsonProperty("Website Link")]
        public string WebsiteLink { get; set; }
    }

    public partial class Direction
    {
        [JsonProperty("Full")]
        public bool Full { get; set; }

        [JsonProperty("Last Updated By")]
        public string UpdatedBy { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("X-Coord")]
        public double XCoord { get; set; }

        [JsonProperty("Y-Coord")]
        public double YCoord { get; set; }

        public Direction(bool full, string name, string type, double xCoord, double yCoord)
        {
            Full = full;
            Name = name;
            Type = type;
            XCoord = xCoord;
            YCoord = yCoord;
            UpdatedBy = InfoStore.CurrentUser.localId;
        }
    }

    public partial class Food
    {

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Last Updated By", NullValueHandling = NullValueHandling.Ignore)]
        public string UpdatedBy { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty(" Last Updated By ", NullValueHandling = NullValueHandling.Ignore)]
        public string LastUpdatedBy { get; set; }

        [JsonProperty("Image", NullValueHandling = NullValueHandling.Ignore)]
        public string Image { get; set; }

        public Food(string name, string description)
        {
            Name = name;
            Description = (String.IsNullOrEmpty(description)) ? "" : description;
            UpdatedBy = InfoStore.CurrentUser.localId;
        }
    }

    public partial class Static
    {
        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Image")]
        public string Image { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Last Updated By")]
        public string UpdatedBy { get; set; }

        public Static(string name, string description, string imageLink)
        {
            Name = name;
            Description = description;
            Image = imageLink;
            UpdatedBy = InfoStore.CurrentUser.localId;
        }
    }

    public partial class Performer
    {
        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Image")]
        public string Image { get; set; }

        [JsonProperty("In Air")]
        public string InAir { get; set; }

        [JsonProperty("Last Updated By")]
        public string PerformerLastUpdatedBy { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Order Number")]
        public string OrderNumber { get; set; }

        [JsonProperty(" Last Updated By ", NullValueHandling = NullValueHandling.Ignore)]
        public string UpdatedBy { get; set; }

        public Performer(string name, string description, string inAir, string imageLink, string orderNumber)
        {
            Name = name;
            Description = description;
            InAir = inAir;
            Image = imageLink;
            OrderNumber = orderNumber;
            UpdatedBy = InfoStore.CurrentUser.localId;
        }
    }

    public partial class Question
    {
        [JsonProperty("Answer")]
        public string Answer { get; set; }

        [JsonProperty("Question")]
        public string QuestionQuestion { get; set; }

        [JsonProperty("Last Updated By")]
        public string UpdatedBy { get; set; }

        public Question(string question, string answer)
        {
            QuestionQuestion = question;
            Answer = answer;
            UpdatedBy = InfoStore.CurrentUser.localId;

        }
    }

    public class Message
    {
        [JsonProperty("body")]
        public string body { get; set; }
        [JsonProperty("date")]
        public string date { get; set; }
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("title")]
        public string title { get; set; }
        [JsonProperty("uid")]
        public string uid { get; set; }
        [JsonProperty("topic")]
        public string topic { get; set; }
    }

    public partial class Database
    {
        public static Database FromJson(string json) => JsonConvert.DeserializeObject<Database>(json, AirshowAddmin.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Database self) => JsonConvert.SerializeObject(self, AirshowAddmin.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
