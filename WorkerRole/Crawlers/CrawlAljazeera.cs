using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WorkerRole.Crawlers
{
    public class CrawlAljazeera : CrawlBase
    {
        public override string RootURL
        {
            get { return "http://mubasher-misr.aljazeera.net/"; }
        }


        public override string SeedURL
        {
            get { return RootURL; }
        }

        public override string NewsURL
        {
            get { return "http://mubasher-misr.aljazeera.net/news/"; }
        }

        public override int NewsSourceID
        {
            get { return 4; }
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
            item.URL = url;
            item.DateTime = DateTime.Now;

            try
            {
                // title
                item.Title = doc.DocumentNode.SelectNodes("//div[@class='posTitle']").First().InnerText.Trim();
                item.Title = HttpUtility.HtmlDecode(item.Title);

                // content
                item.ItemContent = doc.DocumentNode.SelectNodes("//div[@class='posBody']").First().InnerText.Trim();
                item.ItemContent = HttpUtility.HtmlDecode(item.ItemContent);

                // image
                item.ImageURL = RootURL + doc.DocumentNode.SelectNodes("//div[@class='posBody']").First().Descendants("img").First().Attributes["src"].Value;

            }
            catch (Exception ex)
            {
                return null;
            }



            return item;
        }



    }
}
