using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole.Crawlers
{
    public class CrawlShorouk : CrawlBase
    {
        public override string RootURL
        {
            get { return "http://www.shorouknews.com/"; }
        }

        public override string SeedURL
        {
            get { return "http://www.shorouknews.com/"; }
        }

        public override string NewsURL
        {
            get { return "http://www.shorouknews.com/news/view.aspx?"; }
        }

        public override int NewsSourceID
        {
            get { return 14; }
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
                item.Title = doc.DocumentNode.SelectNodes("//*[@id=\"Body_Div1\"]/div[5]/div[1]/div[1]/h2")
                    .First().InnerText.Trim();

                // content
                var paragraphs = doc.DocumentNode.SelectNodes("//*[@class=\"copy-script\"]").First()
                    .Descendants("p");
                //  var nodes = doc.DocumentNode.SelectNodes("//*[@id=\"Body_Div1\"]/div[5]/div[1]/div[1]/div[4]");
                foreach (var node in paragraphs)
                {
                    item.ItemContent += node.InnerText.Trim();
                    item.ItemContent += Environment.NewLine;
                }

                // datetime
                item.DateTime = DateTime.Now;

                // image url
                item.ImageURL = doc.DocumentNode.SelectNodes("//*[@id=\"Body_Body_DivnewsInnerImg\"]/img")
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
