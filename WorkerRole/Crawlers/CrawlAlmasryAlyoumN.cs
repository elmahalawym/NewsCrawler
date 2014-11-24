using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Crawler;
using Abot.Poco;
using HtmlAgilityPack;
using System.Globalization;

namespace WorkerRole.Crawlers
{
    public class CrawlAlmasryAlyoumN : CrawlBase
    {
        public override string RootURL
        {
            get { return @"http://www.almasryalyoum.com"; }
        }


        public override string SeedURL
        {
            get { return RootURL; }
        }

        public override string NewsURL
        {
            get { return @"http://www.almasryalyoum.com/news/details/"; }
        }

        public override int NewsSourceID
        {
            get { return 3; }
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

            // try to get item content from HtmlDocument.
            try
            {
                // uri
                item.URL = url;

                #region Title_Scraping

                // newsTitle
                string newsTitle;
                try
                {
                    // News
                    newsTitle = HtmlEntity.DeEntitize(doc.DocumentNode.SelectNodes("//h1[@class='tit_2']").First().InnerText.Trim());
                    item.Title = newsTitle;

                }
                catch (Exception)
                {
                    // Article
                    newsTitle = HtmlEntity.DeEntitize(doc.DocumentNode.SelectNodes("//h1[@class='tit_2 OpenionPage']").First().SelectNodes("//span").First().InnerText.Trim());
                    item.Title = newsTitle;
                }
                #endregion

                #region Content_Scraping
                // get content 

                //removing the "Also read" div
                //try
                //{
                //    HtmlNode node = doc.DocumentNode.SelectNodes("//div[@class='embeded-read-also']").First();
                //    node.ParentNode.RemoveChild(node);
                //}
                //catch (Exception e)
                //{
                //    // no "Also read" div
                //}

                item.ItemContent = HtmlEntity.DeEntitize(doc.DocumentNode.SelectNodes("//div[@id='NewsStory']").First().InnerText.Trim());
                #endregion

                #region DateTime_Scraping
                // get datetime of the news item               
                //string date;
                //try
                //{
                //    // News
                //    date = doc.DocumentNode.SelectNodes("//div[@class='panel-pane pane-custom pane-1']").First().InnerText;
                //    date = date.Substring(date.IndexOfAny("0123456789".ToCharArray())).Trim();
                //    date = dateFix(date);
                //    DateTime res = new DateTime();
                //    if (!DateTime.TryParseExact(date, "dd MM  yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out res))
                //        throw new Exception();

                //    item.DateTime = res;
                //}
                //catch (Exception)
                //{
                //    // Article
                //    date = doc.DocumentNode.SelectNodes("//div[@class='panel-pane pane-custom pane-1']").First().InnerText.Trim();
                //    string[] weekDays = { "السبت", "الأحد", "الإثنين", "الثلاثاء", "الأربعاء", "الخميس", "الجمعة" };
                //    foreach (string weekDay in weekDays)
                //    {
                //        if (date.Contains(weekDay))
                //            date = date.Remove(date.IndexOf(weekDay), weekDay.Length);
                //    }

                //    date = dateFix(date);

                //    DateTime res = new DateTime();

                //    if (!DateTime.TryParseExact(date, "HH:mm  dd MM  yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out res))
                //        throw new Exception();

                //    item.DateTime = res;

                //}
                item.DateTime = DateTime.Now;


                #endregion

                #region ImageURL_Scraping
                try
                {
                    //News
                    item.ImageURL = doc.DocumentNode.SelectNodes("//div[@class='articleimg']").First().ChildNodes.ElementAt(1).Attributes["src"].Value;
                }
                catch (Exception)
                {
                    item.ImageURL = @"http://www.almasryalyoum.com/content/images/header_logo.png";
                }
                #endregion
            }
            catch
            {
                // failed to get info from this url
                // write the result to a log file
                //Helpers.WriteToLogFile(string.Format("Failed to get content from {0}", url));
            }


            return item;
        }
    }
}
