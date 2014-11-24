using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole.Crawlers
{
    public class CrawlEchoroukonline : CrawlBase   
    {
        public override string RootURL
        {
            get { return "http://www.echoroukonline.com/ara/"; }
        }

        public override string SeedURL
        {
            get { return "http://www.echoroukonline.com/ara/"; }
        }

        public override string NewsURL
        {
            get { return "http://www.echoroukonline.com/ara/articles/"; }
        }

        public override int NewsSourceID
        {
            get { return 15; }
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
                item.Title = doc.DocumentNode.SelectNodes("//*[@id=\"box_center_holder\"]/div[1]/h1")
                    .First().InnerText.Trim();

                // content
                item.ItemContent = doc.DocumentNode.SelectNodes("//*[@id=\"dzarticleBody\"]")
                    .First().InnerText.Trim();

                // datetime
                item.DateTime = DateTime.Now;

                // image url
                try
                {
                    item.ImageURL = doc.DocumentNode.SelectNodes("//*[@id=\"article_body\"]/div[2]/img")
                        .First().Attributes["src"].Value;
                }
                catch (Exception) { item.ImageURL = "http://static.echoroukonline.com/ara/themes/echorouk/img/logo.png"; }
            }

            catch (Exception ex)
            {
                return null;
            }

            return item;
        }
    }
}
