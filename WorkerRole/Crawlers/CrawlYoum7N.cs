using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole.Crawlers
{
    public class CrawlYoum7N : CrawlBase
    {
        public override string RootURL
        {
            get { return @"http://www.youm7.com"; }
        }
        public override string SeedURL
        {
            get { return RootURL; }
        }

        public override string NewsURL
        {
            get { return @"http://www.youm7.com/news.asp"; }
        }

        public override int NewsSourceID
        {
            get { return 1; }
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
            item.NewsSourceID = NewsSourceID;
            if (IsClassified)
                item.CategoryID = this.CategoryID;

            // try to get item content from HtmlDocument
            try
            {
                // uri
                item.URL = url;

                // newsTitle
                string newsTitle;
                try
                {
                    // News 
                    newsTitle = doc.GetElementbyId("newsStoryHeader").Descendants("h2").First().InnerText;
                }
                catch (NullReferenceException)
                {
                    // Atricle
                    newsTitle = doc.GetElementbyId("articleData").Descendants("h2").First().InnerText;
                }
                item.Title = newsTitle;

                // get content
                item.ItemContent = doc.GetElementbyId("newsStoryTxt").ChildNodes["p"].InnerText;

                // get datetime of the news item
                string date;
                try
                {
                    date = doc.GetElementbyId("newsStoryHeader").ChildNodes["p"].InnerText.Trim();
                }
                catch (Exception)
                {
                    date = doc.GetElementbyId("articleData").ChildNodes["p"].InnerText.Trim();
                }

                date = date.Substring(date.IndexOf("،") + 1).Trim();

                date = dateFix(date);

                DateTime res = new DateTime();

                if (!DateTime.TryParseExact(date, "d MM  yyyy - HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out res))
                    throw new Exception();

                item.DateTime = res;

                // try get image
                try
                {
                    item.ImageURL = doc.GetElementbyId("newsStoryImg").Descendants("img").First().Attributes["src"].Value;
                }
                catch (Exception)
                {
                    // if failed .... main logo of youm7
                    item.ImageURL = @"http://www1.youm7.com/images/graphics/logo.gif";
                }
            }
            catch
            {
                // failed to get info from this url
                // write the result to a log file
                //Helpers.WriteToLogFile(string.Format("Failed to get content from {0}", url));
                return null;
            }

            return item;
        }

        private string dateFix(string s)
        {

            List<KeyValuePair<string, string>> monthNumber = new List<KeyValuePair<string, string>>();
            monthNumber.Add(new KeyValuePair<string, string>("يناير", "01"));
            monthNumber.Add(new KeyValuePair<string, string>("فبراير", "02"));
            monthNumber.Add(new KeyValuePair<string, string>("مارس", "03"));
            monthNumber.Add(new KeyValuePair<string, string>("إبريل", "04"));
            monthNumber.Add(new KeyValuePair<string, string>("مايو", "05"));
            monthNumber.Add(new KeyValuePair<string, string>("يونيو", "06"));
            monthNumber.Add(new KeyValuePair<string, string>("يوليو", "07"));
            monthNumber.Add(new KeyValuePair<string, string>("أغسطس", "08"));
            monthNumber.Add(new KeyValuePair<string, string>("سبتمبر", "09"));
            monthNumber.Add(new KeyValuePair<string, string>("أكتوبر", "10"));
            monthNumber.Add(new KeyValuePair<string, string>("نوفمبر", "11"));
            monthNumber.Add(new KeyValuePair<string, string>("ديسمبر", "12"));
            foreach (KeyValuePair<string, string> kv in monthNumber)
            {
                s = s.Replace(kv.Key, kv.Value);
            }
            return s;

        }
    }
}
