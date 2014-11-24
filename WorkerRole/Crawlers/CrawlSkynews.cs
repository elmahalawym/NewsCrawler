using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole.Crawlers
{
    public class CrawlSkynews : CrawlBase
    {
        public override string RootURL
        {
            get { return "http://www.skynewsarabia.com/"; }
        }

        public override string SeedURL
        {
            get { return "http://www.skynewsarabia.com/web/home"; }
        }

        public override string NewsURL
        {
            get { return "http://www.skynewsarabia.com/web/article/"; }
        }

        public override int NewsSourceID
        {
            get { return 12; }
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
                item.Title = doc.DocumentNode.SelectNodes("//*[@id=\"main\"]/h1")
                    .First().InnerText.Trim();

                // content
                var nodes = doc.DocumentNode.SelectNodes("//*[@id=\"main\"]")
                    .First().ChildNodes.Where(n => n.Name.Equals("h3") ||
                    n.Name.Equals("p"));
                foreach (var node in nodes)
                {
                    item.ItemContent += node.InnerText;
                    item.ItemContent += Environment.NewLine;
                }

                // datetime
                item.DateTime = DateTime.Now;

                // image url
                try
                {
                    item.ImageURL = doc.DocumentNode.SelectNodes("//*[@id=\"main\"]/img")
                        .First().Attributes["src"].Value;
                }
                catch (Exception) // video
                {
                    item.ImageURL = "http://asset1.skynewsarabia.com/web/asset/8.1/images/logo.png";
                }
            }

            catch (Exception)
            {
                return null;
            }

            return item;
        }

    }
}
