using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole.Crawlers
{
    public class CrawlDoniatech : CrawlBase
    {
        public override string RootURL
        {
            get { return "http://doniatech.com/"; }
        }

        public override string SeedURL
        {
            get { return RootURL; }
        }

        public override string NewsURL
        {
            get { return "http://doniatech.com/"; }
        }

        public override int NewsSourceID
        {
            get { return 10; }
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
                item.Title = doc.DocumentNode.SelectNodes("//*[@id=\"main-content\"]/div[1]/div/article/div[2]/h1")
                    .First().InnerText.Trim();

                // content
                var nodes = doc.DocumentNode.SelectNodes("//*[@id=\"main-content\"]/div[1]/div/article/div[2]/div[2]")
                    .First().Descendants("p");
                foreach (var node in nodes)
                {
                    item.ItemContent += node.InnerText.Trim();
                    item.ItemContent += Environment.NewLine;
                }

                // datetime
                item.DateTime = DateTime.Now;

                // image url
                item.ImageURL = doc.DocumentNode.SelectNodes("//*[@id=\"main-content\"]/div[1]/div/article/div[1]/img")
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
