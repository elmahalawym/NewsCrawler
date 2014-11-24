using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WorkerRole.Crawlers
{
    public class CrawlBBC : CrawlBase
    {
        public override string RootURL
        {
            get { return "http://www.bbc.co.uk/arabic/"; }
        }

        public override string SeedURL
        {
            get { return RootURL; }
        }

        public override string NewsURL
        {
            get { return "http://www.bbc.co.uk/arabic/"; }
        }

        public override int NewsSourceID
        {
            get { return 6; }
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
                item.Title = HttpUtility.HtmlDecode(doc.DocumentNode.SelectNodes("//*[@id=\"blq-content\"]/div[2]/div[1]/div[1]/h1").
                    First().InnerText.Trim());

                // content
                var nodes = doc.DocumentNode.SelectNodes("//*[@id=\"blq-content\"]/div[2]/div[1]/div[3]/div").First().Descendants("p");
                item.ItemContent = string.Empty;
                foreach (HtmlNode node in nodes)
                {
                    item.ItemContent += HttpUtility.HtmlDecode(node.InnerText.Trim());
                    item.ItemContent += Environment.NewLine;
                }

                // datetime
                item.DateTime = DateTime.Now;

                // image url
                item.ImageURL = doc.DocumentNode.SelectNodes("//*[@id=\"blq-content\"]/div[2]/div[1]/div[3]/div/div[1]/div/img").
                    First().Attributes["src"].Value;
            }

            catch (Exception)
            {
                return null;
            }

            return item;
        }
    }
}
