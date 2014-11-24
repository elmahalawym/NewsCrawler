using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole.Crawlers
{
    public class CrawlKeeftech : CrawlBase
    {
        public override string RootURL
        {
            get { return "http://keeftech.com/"; }
        }

        public override string SeedURL
        {
            get { return "http://keeftech.com/"; }
        }

        public override string NewsURL
        {
            get { return "http://keeftech.com/"; }
        }

        public override int NewsSourceID
        {
            get { return 17; }
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
                item.Title = doc.DocumentNode.SelectNodes("/html/body/section/div[1]/div/h1")
                    .First().InnerText.Trim();

                // content
                var contentNodes = doc.DocumentNode.Descendants("div").Where(n =>
                    n.Attributes.Contains("class") && n.Attributes["class"].Value.Equals("entry_content span12"))
                    .First().Descendants("p");
                foreach (var node in contentNodes)
                {
                    item.ItemContent += node.InnerText.Trim();
                    item.ItemContent += Environment.NewLine;
                }

                // datetime
                item.DateTime = DateTime.Now;

                // image url
                try
                {
                    //item.ImageURL = doc.DocumentNode.Descendants("div").Where(n =>
                    //  n.Attributes.Contains("class") && n.Attributes["class"].Value.Equals("entry_content span12"))
                    //  .First().Descendants("img").First().Attributes["src"].Value;
                    item.ImageURL = doc.DocumentNode.Descendants("header")
                        .ElementAt(1).Descendants("img").First().Attributes["src"].Value;
                }
                catch (Exception)
                { item.ImageURL = "http://keeftech.com/wp-content/uploads/2013/03/logo-keeftech.png"; }
            }

            catch (Exception ex)
            {
                return null;
            }

            return item;
        }
    }
}
