using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkerRole.Crawlers;

namespace WorkerRole
{
    interface ICrawlWeb
    {
        CrawlWebsiteResult Crawl(List<SItem> oldSItems);

        NewsItem GetInfo(HtmlAgilityPack.HtmlDocument doc, string url);
    }
}
