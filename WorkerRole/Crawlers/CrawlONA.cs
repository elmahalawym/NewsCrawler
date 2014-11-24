using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole.Crawlers
{
    public class CrawlONA : CrawlBase
    {
        public override string RootURL
        {
            get { return "http://onaeg.com/"; }
        }
        public override string SeedURL
        {
            get { return RootURL; }
        }

        public override string NewsURL
        {
            get { return "http://onaeg.com/?p"; }
        }

        public override int NewsSourceID
        {
            get { return 7; }
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

            // try to get item infor from HtmlDocument
            try
            {
                // uri
                item.URL = url;

                // newsTitle
                item.Title = doc.DocumentNode.Descendants("h1").Where(n => n.Attributes.Contains("class") &&
                    n.Attributes["class"].Value.Equals("post-title entry-title")).First().InnerText.Trim();

                // get content
                var nodes = doc.DocumentNode.Descendants("div").Where(n => n.Attributes.Contains("class") &&
                    n.Attributes["class"].Value.Equals("entry")).First().Descendants("p");
                foreach (var node in nodes)
                    item.ItemContent += node.InnerText;
                item.ItemContent = item.ItemContent.Trim();

                // get datetime of the news item
                item.DateTime = DateTime.Now;

                // get image
                item.ImageURL = doc.DocumentNode.Descendants("div").Where(n => n.Attributes.Contains("class") &&
                    n.Attributes["class"].Value.Contains("entry")).First().Descendants("img").First()
                    .Attributes["src"].Value;
            }

            catch (Exception ex)
            {
                return null;
            }

            return item;
        }
    }
}
