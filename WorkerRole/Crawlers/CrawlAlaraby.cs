using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole.Crawlers
{
    public class CrawlAlaraby : CrawlBase
    {
        public override string RootURL
        {
            get { return "http://www.alaraby.co.uk/"; }
        }

        public override string SeedURL
        {
            get { return "http://www.alaraby.co.uk/portal"; }
        }

        public override string NewsURL
        {
            get { return "http://www.alaraby.co.uk/"; }
        }

        public override int NewsSourceID
        {
            get { return 16; }
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
                item.Title = doc.DocumentNode.SelectNodes("//*[@id=\"SiteDPId\"]/div[2]/div[4]/div/h1")
                    .First().InnerText.Trim();

                // content
                item.ItemContent = doc.DocumentNode.SelectNodes("//*[@id=\"SiteDPId\"]/div[2]/div[4]/div/div[4]")
                    .First().InnerText.Trim();

                // datetime
                item.DateTime = DateTime.Now;

                // image url
                item.ImageURL = "http://www.alaraby.co.uk/" + doc.DocumentNode.SelectNodes("//*[@id=\"SiteDPId\"]/div[2]/div[4]/div/div[1]/div[1]/img")
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
