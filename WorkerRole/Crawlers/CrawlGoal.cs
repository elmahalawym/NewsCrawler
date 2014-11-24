using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole.Crawlers
{
    public class CrawlGoal : CrawlBase
    {
        public override string RootURL
        {
            get { return @"http://www.goal.com/ar-eg"; }
        }
        public override string SeedURL
        {
            get { return @"http://www.goal.com/ar-eg"; }
        }

        public override string NewsURL
        {
            get { return @"http://www.goal.com/ar-eg/news"; }
        }

        public override int NewsSourceID
        {
            get { return 2; }
        }

        public override bool IsClassified
        {
            get { return true; }
        }

        public override int CategoryID
        {
            get { return 2; }
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
                item.Title = doc.DocumentNode.SelectNodes("//div[@class='headlines']")
                    .First().ChildNodes[1].InnerText;

                // get content
                item.ItemContent = doc.DocumentNode.SelectNodes("//div[@itemprop='articleBody']")
                    .First().InnerText;

                // get datetime of the news item
                string dateStr = doc.DocumentNode.SelectNodes("//div[@class='module module-article-body clearfix']").First().Descendants("time").First().Attributes["datetime"].Value;
                item.DateTime = DateTime.Parse(dateStr);

                // get image
                //item.ImageURL = doc.DocumentNode.SelectNodes("//header[@class='article-header default']").First().FirstChild.Attributes["src"].Value;
                item.ImageURL = doc.DocumentNode.SelectNodes("//header[@class='article-header default']").First().Descendants("img").First().Attributes["src"].Value;

            }

            catch (Exception)
            {
                // failed to get info from this url
                // write the result to a log file
                //Helpers.WriteToLogFile(string.Format("Failed to get content from {0}", url));
                return null;
            }

            return item;
        }
    }
}
