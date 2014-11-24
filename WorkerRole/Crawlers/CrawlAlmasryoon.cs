using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole.Crawlers
{
    public class CrawlAlmasryoon : CrawlBase
    {
        public override string RootURL
        {
            get { return "http://almesryoon.com/"; }
        }

        public override string SeedURL
        {
            get { return "http://almesryoon.com/"; }
        }

        public override string NewsURL
        {
            get { return "http://almesryoon.com/"; }
        }

        public override int NewsSourceID
        {
            get { return 20; }
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
                item.Title = doc.DocumentNode.SelectNodes("//*[@class=\"item-page\"]")
                    .First().Descendants("h1").First().InnerText.Trim();

                // content
                var contentNodes = doc.DocumentNode.SelectNodes("//*[@class=\"row details\"]").First().Descendants("p");
                foreach (var node in contentNodes)
                {
                    item.ItemContent += node.InnerText.Trim();
                    item.ItemContent += Environment.NewLine;
                }

                // datetime
                item.DateTime = DateTime.Now;

                // image url
                item.ImageURL = "http://almesryoon.com/" + doc.DocumentNode.SelectNodes("//*[@class=\"article_image\"]")
                    .First().Descendants("img").First().Attributes["src"].Value;
            }

            catch (Exception)
            {
                return null;
            }

            return item;
        }
    }
}
