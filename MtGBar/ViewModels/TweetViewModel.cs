using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using MtGBar.Infrastructure.DataNinjitsu.Models.TweetWords;
using Newtonsoft.Json.Linq;

namespace MtGBar.ViewModels
{
    public class TweetViewModel
    {
        public Uri AuthorImage { get; set; }
        public DateTime Date { get; set; }
        public string AuthorRealName { get; set; }
        public string AuthorTwitterName { get; set; }
        public TweetWord[] Text { get; set; }
        public long TweetID { get; set; }

        public static TweetViewModel[] FromJson(string apiResult)
        {
            List<TweetViewModel> retVal = new List<TweetViewModel>();
            JToken data = JObject.Parse(apiResult);

            foreach (JToken tweetData in data["statuses"]) {
                retVal.Add(TweetFromJToken(tweetData));
            }

            return retVal.ToArray();
        }

        public static TweetViewModel[] FromTimelineJson(string apiResult)
        {
            List<TweetViewModel> retVal = new List<TweetViewModel>();
            JToken data = JArray.Parse(apiResult);

            foreach (JToken tweetData in data.Children()) {
                retVal.Add(TweetFromJToken(tweetData));
            }

            return retVal.ToArray();
        }

        private static TweetViewModel TweetFromJToken(JToken tweetData)
        {
            JToken user = tweetData["user"];
            List<TweetWord> words = new List<TweetWord>();

            foreach (string token in tweetData["text"].ToString().Split(' ')) {
                if (Regex.IsMatch(token, "^#[a-zA-Z0-9_]+$")) {
                    words.Add(new Hashtag(token));
                }
                else if (Regex.IsMatch(token, "^@[a-zA-Z0-9_]+:*$")) {
                    words.Add(new Mention(token));
                }
                else if(Regex.IsMatch(token, "(?i)\\b((?:https?:(?:/{1,3}|[a-z0-9%])|[a-z0-9.\\-]+[.](?:com|net|org|edu|gov|mil|aero|asia|biz|cat|coop|info|int|jobs|mobi|museum|name|post|pro|tel|travel|xxx|ac|ad|ae|af|ag|ai|al|am|an|ao|aq|ar|as|at|au|aw|ax|az|ba|bb|bd|be|bf|bg|bh|bi|bj|bm|bn|bo|br|bs|bt|bv|bw|by|bz|ca|cc|cd|cf|cg|ch|ci|ck|cl|cm|cn|co|cr|cs|cu|cv|cx|cy|cz|dd|de|dj|dk|dm|do|dz|ec|ee|eg|eh|er|es|et|eu|fi|fj|fk|fm|fo|fr|ga|gb|gd|ge|gf|gg|gh|gi|gl|gm|gn|gp|gq|gr|gs|gt|gu|gw|gy|hk|hm|hn|hr|ht|hu|id|ie|il|im|in|io|iq|ir|is|it|je|jm|jo|jp|ke|kg|kh|ki|km|kn|kp|kr|kw|ky|kz|la|lb|lc|li|lk|lr|ls|lt|lu|lv|ly|ma|mc|md|me|mg|mh|mk|ml|mm|mn|mo|mp|mq|mr|ms|mt|mu|mv|mw|mx|my|mz|na|nc|ne|nf|ng|ni|nl|no|np|nr|nu|nz|om|pa|pe|pf|pg|ph|pk|pl|pm|pn|pr|ps|pt|pw|py|qa|re|ro|rs|ru|rw|sa|sb|sc|sd|se|sg|sh|si|sj|Ja|sk|sl|sm|sn|so|sr|ss|st|su|sv|sx|sy|sz|tc|td|tf|tg|th|tj|tk|tl|tm|tn|to|tp|tr|tt|tv|tw|tz|ua|ug|uk|us|uy|uz|va|vc|ve|vg|vi|vn|vu|wf|ws|ye|yt|yu|za|zm|zw)/)(?:[^\\s()<>{}\\[\\]]+|\\([^\\s()]*?\\([^\\s()]+\\)[^\\s()]*?\\)|\\([^\\s]+?\\))+(?:\\([^\\s()]*?\\([^\\s()]+\\)[^\\s()]*?\\)|\\([^\\s]+?\\)|[^\\s`!()\\[\\]{};:'\".,<>?«»“”‘’])|(?:(?<!@)[a-z0-9]+(?:[.\\-][a-z0-9]+)*[.](?:com|net|org|edu|gov|mil|aero|asia|biz|cat|coop|info|int|jobs|mobi|museum|name|post|pro|tel|travel|xxx|ac|ad|ae|af|ag|ai|al|am|an|ao|aq|ar|as|at|au|aw|ax|az|ba|bb|bd|be|bf|bg|bh|bi|bj|bm|bn|bo|br|bs|bt|bv|bw|by|bz|ca|cc|cd|cf|cg|ch|ci|ck|cl|cm|cn|co|cr|cs|cu|cv|cx|cy|cz|dd|de|dj|dk|dm|do|dz|ec|ee|eg|eh|er|es|et|eu|fi|fj|fk|fm|fo|fr|ga|gb|gd|ge|gf|gg|gh|gi|gl|gm|gn|gp|gq|gr|gs|gt|gu|gw|gy|hk|hm|hn|hr|ht|hu|id|ie|il|im|in|io|iq|ir|is|it|je|jm|jo|jp|ke|kg|kh|ki|km|kn|kp|kr|kw|ky|kz|la|lb|lc|li|lk|lr|ls|lt|lu|lv|ly|ma|mc|md|me|mg|mh|mk|ml|mm|mn|mo|mp|mq|mr|ms|mt|mu|mv|mw|mx|my|mz|na|nc|ne|nf|ng|ni|nl|no|np|nr|nu|nz|om|pa|pe|pf|pg|ph|pk|pl|pm|pn|pr|ps|pt|pw|py|qa|re|ro|rs|ru|rw|sa|sb|sc|sd|se|sg|sh|si|sj|Ja|sk|sl|sm|sn|so|sr|ss|st|su|sv|sx|sy|sz|tc|td|tf|tg|th|tj|tk|tl|tm|tn|to|tp|tr|tt|tv|tw|tz|ua|ug|uk|us|uy|uz|va|vc|ve|vg|vi|vn|vu|wf|ws|ye|yt|yu|za|zm|zw)\b/?(?!@)))")) {
                    words.Add(new Url(token));
                }
                else {
                    words.Add(new TweetWord(token));
                }
            }


            return new TweetViewModel() {
                AuthorImage = new Uri(user["profile_image_url"].ToString().Replace("normal", "bigger")),
                AuthorRealName = user["name"].ToString(),
                AuthorTwitterName = user["screen_name"].ToString(),
                Date = DateTime.ParseExact(
                    tweetData["created_at"].ToString(),
                    "ddd MMM dd HH:mm:ss %K yyyy",
                    CultureInfo.InvariantCulture.DateTimeFormat
                ),
                Text = words.ToArray(),
                TweetID = long.Parse(tweetData["id"].ToString()),
            };

        }
    }
}