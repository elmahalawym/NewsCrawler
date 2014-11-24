using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Crawler;
using Abot.Poco;

namespace WorkerRole.Crawlers
{
    public class CrawlWebsiteResult
    {
        public CrawlWebsiteResult(CrawlResult result, List<NewsItem> newItems)
        {
            this.Result = result;
            this.NewItems = newItems;
        }

        public CrawlResult Result { get; set; }

        public List<NewsItem> NewItems { get; set; }
    }
}
