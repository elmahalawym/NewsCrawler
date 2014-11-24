using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WorkerRole.Crawlers
{
    public class CrawlMoheetTask : CrawlBase
    {
        public override string RootURL
        {
            get { return "http://moheet.com/"; }
        }
        public override string SeedURL
        {
            get { return RootURL; }
        }

        public override string NewsURL
        {
            get { return "http://moheet.com/"; }
        }

        public override int NewsSourceID
        {
            get { return 5; }
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
                item.Title = HttpUtility.HtmlDecode(doc.DocumentNode.Descendants("h1").Where(n => n.Attributes.
                    Contains("class") && n.Attributes["class"].Value.Equals("title")).
                    First().InnerText.Trim());

                // get content
                item.ItemContent = HttpUtility.HtmlDecode(doc.DocumentNode.Descendants("div").Where(n => n.Attributes.Contains("class") &&
                    n.Attributes["class"].Value.Equals("text wb")).First().InnerText.Trim());

                // get datetime of the news item
                item.DateTime = DateTime.Parse(doc.DocumentNode.Descendants("strong").Where(n => n.Attributes.Contains("class") &&
                    n.Attributes["class"].Value.Equals("date")).First().InnerText);

                // get image
                item.ImageURL = doc.DocumentNode.Descendants("div").Where(n => n.Attributes.Contains("class") &&
                    n.Attributes["class"].Value.Equals("col image")).First().Descendants("img").First().
                    Attributes["src"].Value;
            }

            catch (Exception)
            {
                return null;
            }

            return item;
        }
    }
}
