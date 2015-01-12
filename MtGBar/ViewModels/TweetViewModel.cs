using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace MtGBar.ViewModels
{
    public class TweetViewModel
    {
        public Uri AuthorImage { get; set; }
        public DateTime Date { get; set; }
        public string AuthorRealName { get; set; }
        public string AuthorTwitterName { get; set; }
        public string Text { get; set; }
        public long TweetID { get; set; }

        public static TweetViewModel[] FromJson(string apiResult)
        {
            List<TweetViewModel> retVal = new List<TweetViewModel>();
            JToken data = JArray.Parse(apiResult);

            foreach (JToken tweetData in data.Children()) {
                JToken user = tweetData["user"];

                TweetViewModel tweet = new TweetViewModel() {
                    AuthorImage = new Uri(user["profile_image_url"].ToString()),
                    AuthorRealName = user["name"].ToString(),
                    AuthorTwitterName = user["screen_name"].ToString(),
                    Date = DateTime.ParseExact(
                        tweetData["created_at"].ToString(),
                        "ddd MMM dd HH:mm:ss %K yyyy",
                        CultureInfo.InvariantCulture.DateTimeFormat
                    ),
                    Text = tweetData["text"].ToString(),
                    TweetID = long.Parse(tweetData["id"].ToString()),
                };

                retVal.Add(tweet);
            }

            return retVal.ToArray();
        }
    }
}