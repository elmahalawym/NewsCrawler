using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole.Crawlers
{
    public class CrawlAITnews : CrawlBase
    {
        public override string RootURL
        {
            get { return "http://aitnews.com/"; }
        }

        public override string SeedURL
        {
            get { return RootURL; }
        }

        public override string NewsURL
        {
            get { return "http://aitnews.com/"; }
        }

        public override int NewsSourceID
        {
            get { return 9; }
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
                item.Title = doc.DocumentNode.Descendants("h1").Where(n =>
                n.Attributes.Contains("class") && n.Attributes["class"].Value.Equals("entry-title")).
                First().InnerText.Trim();

                // content
                item.ItemContent = doc.DocumentNode.Descendants("section").Where(n =>
                n.Attributes.Contains("class") && n.Attributes["class"].Value.Equals("entry-content")).
                First().InnerText.Trim();

                // datetime
                item.DateTime = DateTime.Parse(doc.DocumentNode.Descendants("time")
                    .First().Attributes["datetime"].Value);

                // image url
                item.ImageURL = doc.DocumentNode.Descendants("div").Where(n => n.Attributes.Contains("class") &&
                n.Attributes["class"].Value.Equals("single-post-thumbnail post-thumbnail")).
                    First().Descendants("img").First().Attributes["src"].Value;
            }

            catch (Exception ex)
            {
                return null;
            }

            return item;
        }

    }
}
