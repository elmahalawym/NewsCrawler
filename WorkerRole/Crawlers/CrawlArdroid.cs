using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole.Crawlers
{
    public class CrawlArdroid : CrawlBase
    {
        public override string RootURL
        {
            get { return "http://ardroid.com/"; }
        }

        public override string SeedURL
        {
            get { return "http://ardroid.com/"; }
        }

        public override string NewsURL
        {
            get { return "http://ardroid.com/"; }
        }

        public override int NewsSourceID
        {
            get { return 18; }
        }

        public override bool IsClassified
        {
            get { return true; }
        }

        public override int CategoryID
        {
            get { return 3; }
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
                item.Title = doc.DocumentNode.SelectNodes("//*[@class=\"entry-title\"]")
                    .First().InnerText.Trim();

                // content
                var contentNodes = doc.DocumentNode.SelectNodes("//*[@class=\"entry-content\"]").First().Descendants("p");
                foreach (var node in contentNodes)
                {
                    item.ItemContent += node.InnerText.Trim();
                    item.ItemContent += Environment.NewLine;
                }

                // datetime
                item.DateTime = DateTime.Now;

                // image url
                item.ImageURL = "http://ardroid.com/wp-content/themes/ardroid/images/sprite.png";
            }

            catch (Exception ex)
            {

                return null;
            }

            return item;
        }
    }
}
