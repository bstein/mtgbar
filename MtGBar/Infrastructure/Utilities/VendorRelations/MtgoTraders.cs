using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Melek.Models;

namespace MtGBar.Infrastructure.Utilities.VendorRelations
{
    public class MtgoTraders : Vendor
    {
        public override string GetLink(Card card, Set set)
        {
            string setCode = set.Code;
            if (set.IsPromo) {
                setCode = "PRM";
            }

            return string.Format("http://www.mtgotraders.com/store/{0}_{1}.html", set.Code, card.Name.Replace(' ', '_').Replace(",", ""));
        }

        public override string GetName()
        {
            return "MtgoTraders.com";
        }

        public override string GetPrice(Card card, Set set)
        {
            WebClient client = new WebClient();
            using(WebClient webClient = new WebClient()) {
                string pageHtml = webClient.DownloadString(GetLink(card, set));
                Match match = Regex.Match(pageHtml, "<span class=\"price\">(\\S+)</span>");

                if (match != null) return match.Groups[1].Value;
            }

            return string.Empty;
        }
    }
}