using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole.Crawlers
{
    public class CrawlUnlimitedtech : CrawlBase
    {
        public override string RootURL
        {
            get { return "http://www.unlimit-tech.com/blog/"; }
        }
        public override string SeedURL
        {
            get { return RootURL; }
        }

        public override string NewsURL
        {
            get { return "http://www.unlimit-tech.com/blog/?p"; }
        }

        public override int NewsSourceID
        {
            get { return 11; }
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
                item.Title = doc.DocumentNode.SelectNodes("//*[@id=\"cb-standard-featured\"]/div[2]/span/h1")
                    .First().InnerText.Trim();

                // content
                var nodes = doc.DocumentNode.SelectNodes("//*[@class=\"entry-content clearfix\"]")
                    .First().Descendants("p");
                foreach (var node in nodes)
                {
                    item.ItemContent += node.InnerText.Trim();
                    item.ItemContent += Environment.NewLine;
                }
                //item.ItemContent = doc.DocumentNode.SelectNodes("//*[@class=\"entry-content clearfix\"]")
                //    .First().InnerText.Trim();

                // datetime
                item.DateTime = DateTime.Now;

                // image url
                try
                {
                    item.ImageURL = doc.DocumentNode.SelectNodes("//*[@id=\"cb-standard-featured\"]/div[1]/img")
                        .First().Attributes["src"].Value;
                }
                catch (Exception) // video
                {
                    item.ImageURL = "http://www.unlimit-tech.com/blog/wp-content/themes/unlimit-tech-theme/library/images/logo.png";
                }
            }

            catch (Exception ex)
            {
                return null;
            }

            return item;
        }
    }
}
