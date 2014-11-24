using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole.Crawlers
{
    public class CrawlDostor : CrawlBase
    {
        public override string RootURL
        {
            get { return "http://www.dostor.org/"; }
        }

        public override string SeedURL
        {
            get { return RootURL; }
        }

        public override string NewsURL
        {
            get { return "http://www.dostor.org/"; }
        }

        public override int NewsSourceID
        {
            get { return 8; }
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

                // newsTitle
                item.Title = doc.DocumentNode.SelectNodes("/html/body/div[3]/div/div[3]/div[2]/div[1]/h1")
                    .First().InnerText.Trim();

                // get content
                item.ItemContent = doc.DocumentNode.SelectNodes("//*[@id=\"start\"]/div")
                    .First().InnerText.Trim();

                // get datetime of the news item
                item.DateTime = DateTime.Now;

                // get image
                item.ImageURL = "http://www.dostor.org/" + doc.DocumentNode.SelectNodes("/html/body/div[3]/div/div[3]/div[2]/img")
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
