using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole.Crawlers
{
    public class CrawlCNN : CrawlBase
    {
        public override string RootURL
        {
            get { return "http://arabic.cnn.com/"; }
        }

        public override string SeedURL
        {
            get { return "http://arabic.cnn.com/"; }
        }

        public override string NewsURL
        {
            get { return "http://arabic.cnn.com/"; }
        }

        public override int NewsSourceID
        {
            get { return 19; }
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
                item.Title = doc.DocumentNode.SelectNodes("//*[@class=\"news-details clearfix\"]").
                    First().Descendants("h1").First().InnerText.Trim();

                // content
                var contentNodes = doc.DocumentNode.SelectNodes("//*[@class=\"article-left clearfix inline-image\"]").Descendants("p");
                foreach (var node in contentNodes)
                {
                    item.ItemContent += node.InnerText.Trim();
                    item.ItemContent += Environment.NewLine;
                }

                // datetime
                item.DateTime = DateTime.Now;

                // image url
                item.ImageURL = doc.DocumentNode.SelectNodes("//*[@class=\"breakpoint\"]").First()
                    .Descendants("img").First().Attributes["src"].Value;
            }

            catch (Exception ex)
            {
                return null;
            }

            return item;
        }
    }
}
