using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole.Crawlers
{
    public class CrawlSasapost : CrawlBase
    {
        public override string RootURL
        {
            get { return "http://www.sasapost.com/"; }
        }

        public override string SeedURL
        {
            get { return "http://www.sasapost.com/"; }
        }

        public override string NewsURL
        {
            get { return "http://www.sasapost.com/"; }
        }

        public override int NewsSourceID
        {
            get { return 13; }
        }

        public override bool IsClassified
        {
            get { return false; }
        }

        public override int CategoryID
        {
            get { throw new NotImplementedException(); }
        }

        public override NewsItem GetInfo(HtmlAgilityPack.HtmlDocument doc, string url)
        {
            NewsItem item = new NewsItem();

            // try to get item info from HtmlDocument
            try
            {
                // uri
                item.URL = url;

                // Title
                item.Title = doc.DocumentNode.Descendants("h1").Where(n =>
                    n.Attributes.Contains("itemprop") &&
                    n.Attributes["itemprop"].Value.Equals("headline")).First().InnerText.Trim(); ;

                // content
                var nodes = doc.DocumentNode.Descendants("section").Where(n =>
                    n.Attributes.Contains("itemprop") &&
                    n.Attributes["itemprop"].Value.Equals("articleBody")).First().Descendants("p");
                foreach (var node in nodes)
                {
                    item.ItemContent += node.InnerText;
                    item.ItemContent += Environment.NewLine;
                }

                // datetime
                item.DateTime = DateTime.Now;

                // image url
                item.ImageURL = doc.DocumentNode.SelectNodes("//*[@id=\"main\"]").First().Descendants("img")
                    .First().Attributes["src"].Value;
            }

            catch (Exception ex)
            {
                return null;
            }

            return item;
        }
    }
}
