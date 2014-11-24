using Abot.Crawler;
using Abot.Poco;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WorkerRole.Crawlers
{
    public abstract class CrawlBase : ICrawlWeb
    {
        private string ARABIC_CHARACHTERS = "اأإآبتثجحخدذرزسشصضطظعغفقكلمنهويىئءؤة .,\"";

        protected List<SItem> oldItems;
        protected List<NewsItem> newItems;

        abstract public string RootURL { get; }

        abstract public string SeedURL { get; }

        abstract public string NewsURL { get; }

        abstract public int NewsSourceID { get; }

        abstract public bool IsClassified { get; }

        abstract public int CategoryID { get; }

        public CrawlWebsiteResult Crawl(List<SItem> oldSItems)
        {
            // cwaler configurations
            CrawlConfiguration config = Helpers.GetCrawlConfigurations();

            // crawler
            PoliteWebCrawler crawler = new PoliteWebCrawler(config, null, null, null, null, null, null, null, null);

            // attaching event handlers.
            crawler.PageCrawlStartingAsync += crawler_ProcessPageCrawlStarting;
            crawler.PageCrawlCompletedAsync += crawler_ProcessPageCrawlCompleted;
            crawler.PageCrawlDisallowedAsync += crawler_PageCrawlDisallowed;
            crawler.PageLinksCrawlDisallowedAsync += crawler_PageLinksCrawlDisallowed;

            // old items.
            oldItems = oldSItems;

            // if failed to get old items.
            if (oldItems == null)
            {
                Trace.WriteLine("******************************");
                Trace.WriteLine("Crawler didn't recieve old items");
                Trace.WriteLine("******************************");
                return null; // terminate.
            }

            // initialize newItems.
            newItems = new List<NewsItem>();

            // adding crawling constraints.
            crawler.ShouldCrawlPage((pageToCrawl, crawlContext) =>
            {
                CrawlDecision decision = new CrawlDecision() { Allow = true, Reason = "" };

                // prevent the crawler from crawling pages outside the root url of the website.
                if (!pageToCrawl.Uri.AbsoluteUri.ToLower().Contains(RootURL.ToLower()))
                    return new CrawlDecision { Allow = false, Reason = "don't crawl outside the root" };

                // prevent the crawler from crawling pages that are crawled before.
                if (oldItems.Where(n => n.URL.ToLower().Equals(pageToCrawl.Uri.AbsoluteUri.ToLower())).Count() > 0)
                    return new CrawlDecision { Allow = false, Reason = "don't crawl existing items" };

                return decision;
            });

            // start crawling and return the result.
            CrawlResult result = crawler.Crawl(new Uri(SeedURL));

            // collect info
            if (result.ErrorOccurred)
            {
                Trace.WriteLine(string.Format("Crawl of {0} completed with error: {1}", result.RootUri.AbsoluteUri, result.ErrorException.Message));
                // Helpers.InsertLog(string.Format("Crawl of {0} completed with error: {1}", result.RootUri.AbsoluteUri, result.ErrorException.Message), "Error");
            }
            else
                Trace.WriteLine(string.Format("Crawl of {0} completed without error.", result.RootUri.AbsoluteUri));

            // return result.
            return new CrawlWebsiteResult(result, newItems);
        }

        /// <summary>
        /// A handler that is invoked when a page crawling is completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            string absoluteUri = e.CrawledPage.Uri.AbsoluteUri;

            // detect if the crawled page represents a news item.
            if (absoluteUri.ToLower().Contains(NewsURL.ToLower()))
            {
                // remove html comments.
                string html = e.CrawledPage.HtmlDocument.DocumentNode.OuterHtml;
                string htmlWithoutComments = RemoveHTMLComments(html);

                // load html document.
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlWithoutComments);

                // remove scripts and styles.
                doc.DocumentNode.Descendants()
                        .Where(n => n.Name == "script" || n.Name == "style")
                        .ToList()
                        .ForEach(n => n.Remove());

                // get info from the crawled page.
                NewsItem item = GetInfo(doc, absoluteUri);

                if (item != null && !string.IsNullOrEmpty(item.Title))
                    item.Title = HttpUtility.HtmlDecode(item.Title);

                if (item != null && !string.IsNullOrEmpty(item.ItemContent))
                    item.ItemContent = HttpUtility.HtmlDecode(item.ItemContent);

                // collect new item.
                if (item != null &&
                    oldItems.Where(n => n.Title.Equals(item.Title)).Count() == 0 &&
                    newItems.Where(n => n.Title.Equals(item.Title)).Count() == 0)
                {
                    // add NewsSourceID.
                    item.NewsSourceID = NewsSourceID;                   

                    // add category if the source is classified.
                    if (this.IsClassified)
                        item.CategoryID = this.CategoryID;
                    else
                        item.CategoryID = null;

                    // fix datetime problem.
                    item.DateTime = DateTime.UtcNow;

                    // add item to list.
                    newItems.Add(item);

                    // save new item in the database.
                    bool insertionResult = Helpers.DatabaseInsert(item);

                    // track result
                    Trace.WriteLine((insertionResult ? "item inserted successfully from sourceID " + NewsSourceID :
                    "an error occured while inserting item to the database from sourceID " + NewsSourceID));
                }
            }

            Trace.WriteLine(string.Format("Crawled : {0}", e.CrawledPage.Uri.AbsoluteUri));
        }

        #region OtherHandlers
        private void crawler_PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e) { }

        private void crawler_PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e) { }

        private void crawler_ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e) { }
        #endregion

        /// <summary>
        /// Abstract method to be overrided to define content scrapping from each news page.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        abstract public NewsItem GetInfo(HtmlAgilityPack.HtmlDocument doc, string url);


        protected string PreProcess(string s, HtmlAgilityPack.HtmlDocument doc)
        {
            s = HttpUtility.HtmlDecode(s);

            for (int i = 0; i < s.Length; i++)
            {
                if (!ARABIC_CHARACHTERS.Contains(s[i]))
                    s.Remove(i, 1);
            }

            //byte[] bytes = Encoding.Default.GetBytes(s);
            //return Encoding.UTF8.GetString(bytes);

            return s;
        }

        private int CountArabicChars(string s)
        {
            int res = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (ARABIC_CHARACHTERS.Contains(s[i]))
                    res++;
            }
            return res;
        }

        /// <summary>
        /// removes all html comments from a string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string RemoveHTMLComments(string input)
        {
            string output = string.Empty;
            string[] temp = System.Text.RegularExpressions.Regex.Split(input, "<!--");
            foreach (string s in temp)
            {
                string str = string.Empty;
                if (!s.Contains("-->"))
                {
                    str = s;
                }
                else
                {
                    str = s.Substring(s.IndexOf("-->") + 3);
                }
                if (str.Trim() != string.Empty)
                {
                    output = output + str.Trim();
                }
            }
            return output;
        }

    }
}
