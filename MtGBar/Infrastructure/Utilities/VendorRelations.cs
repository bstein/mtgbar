using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Bazam.Slugging;
using Melek.Models;
using Melek.Utilities;

namespace MtGBar.Infrastructure.Utilities
{
    public static class VendorRelations
    {
        public static string GetAmazonLink(Card card, Set set)
        {
            string cardName = card.Name.Replace("/", string.Empty).ToLower();
            return "http://www.amazon.com/s/field-keywords=mtg+" + HttpUtility.UrlEncode(set.Name).ToLower() + "+" + HttpUtility.UrlEncode(cardName);
        }

        public static string GetAmazonPrice(Card card, Set set)
        {
            try {
                string url = VendorRelations.GetAmazonLink(card, set);
                string html = new WebClient().DownloadString(url);

                Match match = Regex.Match(html, "<div id=\"atfResults\"[\\s\\S]+?(\\$[0-9]+\\.[0-9]{2})");
                if (match != null && match.Groups.Count == 2) {
                    return match.Groups[1].Value;
                }
            }
            catch (WebException ex) {
                AppState.Instance.LoggingNinja.LogError(ex);
            }
            return string.Empty;
        }

        public static string GetCFLink(string cardName, Set set)
        {
            try {
                using (WebClient client = new WebClient()) {
                    string searchHtml = client.DownloadString(GetCFSearchLink(cardName, set));
                    string searchPattern = string.Format("<a href=\"(\\S+?)\">\\s+<h3 class=\"hover-title\">{0}: {1}</h3>", (string.IsNullOrEmpty(set.CFName) ? set.Name : set.CFName), cardName);
                    Match match = Regex.Match(searchHtml, searchPattern);

                    if (match != null && match.Groups.Count == 2) {
                        return "http://store.channelfireball.com" + match.Groups[1].Value;
                    }
                }
                return GetCFSearchLink(cardName, set);
            }
            catch (WebException ex) {
                AppState.Instance.LoggingNinja.LogError(ex);
            }
            return string.Empty;
        }

        public static string GetCFPrice(string cardName, Set set)
        {
            string url = VendorRelations.GetCFSearchLink(cardName, set);
            string html = string.Empty;
            string pattern = string.Format("<h3 class=\"grid-item-price\">(.+?)</h3>", (string.IsNullOrEmpty(set.CFName) ? set.Name : set.CFName), cardName);

            try {
                using (WebClient client = new WebClient()) {
                    html = client.DownloadString(url);
                    Match match = Regex.Match(html, pattern);
                    if (match != null && match.Groups.Count == 2) {
                        return match.Groups[1].Value;
                    }
                }
            }
            catch (Exception) {
                AppState.Instance.LoggingNinja.LogMessage("Couldn't download CF price for " + cardName + " from " + set.Name + ".");
            }
            return string.Empty;
        }

        public static string GetCFSearchLink(string cardName, Set set)
        {
            return "http://store.channelfireball.com/products/search?query=" + HttpUtility.UrlEncode(cardName + " " + set.CFName);
        }

        public static string GetGathererLink(string multiverseID)
        {
            return "http://gatherer.wizards.com/Pages/Card/Details.aspx?multiverseid=" + multiverseID;
        }

        public static string GetMagicCardsInfoLink(string cardName)
        {
            return string.Format("http://magiccards.info/query?q={0}&v=card&s=cname", HttpUtility.UrlEncode(cardName));
        }

        public static string GetTCGPlayerLinkDefault(string cardName, Set set)
        {
            return string.Format("http://store.tcgplayer.com/magic/{0}/{1}", Slugger.Slugify(string.IsNullOrEmpty(set.TCGPlayerName) ? set.Name : set.TCGPlayerName), Slugger.Slugify(cardName));
        }

        public static string GetTCGPlayerAPIData(string cardName, Set set)
        {
            //http://partner.tcgplayer.com/x3/pv.asmx/p?pk=MTGBAR&p=Sword+of+War+and+Peace&s=New+Phyrexia&v=3
            // resolve their set name - sometimes it's special
            string setName = (set.TCGPlayerName == string.Empty ? set.Name : set.TCGPlayerName);
            using (WebClient client = new WebClient()) {
                return client.DownloadString(string.Format("http://partner.tcgplayer.com/x3/pv.asmx/p?pk=MTGBAR&p={0}&s={1}&v=3", HttpUtility.UrlEncode(cardName), HttpUtility.UrlEncode(setName)));
            }
        }

        public static string GetTCGPlayerLink(string apiData)
        {
            MatchCollection matches = Regex.Matches(apiData, "<link>([\\s\\S]+?)</link>");
            if (matches.Count > 0) {
                return matches[0].Groups[1].Value.Trim();
            }
            return string.Empty;
        }

        public static string GetTCGPlayerPrice(string apiData)
        {
            MatchCollection matches = Regex.Matches(apiData, "<price>([0-9]*?\\.[0-9]{2})</price>");
            if (matches.Count > 0) {
                return "$" + matches[0].Groups[1].Value.Trim();
            }
            return string.Empty;
        }
    }
}