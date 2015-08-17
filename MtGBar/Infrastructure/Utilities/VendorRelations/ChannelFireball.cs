using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Melek.Domain;

namespace MtGBar.Infrastructure.Utilities.VendorRelations
{
    public class ChannelFireball : Vendor
    {
        private string GetSearchLink(Card card, Set set)
        {
            return "http://store.channelfireball.com/products/search?query=" + HttpUtility.UrlEncode(card.Name + " " + set.CFName);
        }

        public override string GetLink(Card card, Set set)
        {
            string searchLink = GetSearchLink(card, set);

            using (WebClient client = new WebClient()) {
                string searchHtml = client.DownloadString(searchLink);
                string searchPattern = string.Format("<a href=\"(\\S+?)\">\\s+<h3 class=\"hover-title\">{0}: {1}</h3>", (string.IsNullOrEmpty(set.CFName) ? set.Name : set.CFName), card.Name);
                Match match = Regex.Match(searchHtml, searchPattern);

                if (match != null && match.Groups.Count == 2) {
                    return "http://store.channelfireball.com" + match.Groups[1].Value;
                }
            }
            return searchLink;
        }

        public override string GetName()
        {
            return "ChannelFireball.com";
        }

        public override string GetPrice(Card card, Set set)
        {
            string url = VendorRelationsUtilities.GetCFSearchLink(card.Name, set);
            string html = string.Empty;
            string pattern = string.Format("<h3 class=\"grid-item-price\">(.+?)</h3>", (string.IsNullOrEmpty(set.CFName) ? set.Name : set.CFName), card.Name);

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
                AppState.Instance.LoggingNinja.LogMessage("Couldn't download CF price for " + card.Name + " from " + set.Name + ".");
            }
            return string.Empty;
        }
    }
}