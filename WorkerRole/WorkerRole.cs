using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using WorkerRole;
using WorkerRole.Crawlers;

namespace WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        public List<SItem> items;

        public override void Run()
        {
            Trace.AutoFlush = true;

            // test logs
            // Helpers.InsertLog("crawler", "started the service");


            // retrieve items from database
            Trace.WriteLine("Getting Items from the database");
            do
            {
                items = Helpers.GetAllNewsItems();
            } while (items == null);

            Trace.WriteLine("***********************************************");
            Trace.WriteLine(string.Format("{0} Items retrieved from the database.", items.Count));
            Trace.WriteLine("***********************************************");

            while (true)
            {
                // wait 10 minutes between crawling tasks.
                //Thread.Sleep(TimeSpan.FromMinutes(10));

                // start.
                StartCrawlTasks();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }

        public void StartCrawlTasks()
        {
            // Helpers.InsertLog("crawler", " task started");

            List<CrawlBase> crawlers = new List<CrawlBase>();

            // add website crawlers.
            crawlers.Add(new CrawlEchoroukonline());           
            crawlers.Add(new CrawlAlmasryoon());
            crawlers.Add(new CrawlCNN());
            crawlers.Add(new CrawlArdroid());
            crawlers.Add(new CrawlKeeftech());
            crawlers.Add(new CrawlAlaraby());
            crawlers.Add(new CrawlShorouk());
            crawlers.Add(new CrawlSasapost());
            crawlers.Add(new CrawlONA());
            crawlers.Add(new CrawlDostor());
            crawlers.Add(new CrawlAITnews());
            crawlers.Add(new CrawlDoniatech());
            crawlers.Add(new CrawlUnlimitedtech());
            crawlers.Add(new CrawlSkynews());
            crawlers.Add(new CrawlAljazeera());
            crawlers.Add(new CrawlGoal());
            crawlers.Add(new CrawlAlmasryAlyoumN());
            crawlers.Add(new CrawlMoheetTask());
            crawlers.Add(new CrawlYoum7N());
            crawlers.Add(new CrawlBBC());

            // all tasks.
            List<Task<CrawlWebsiteResult>> crawl_tasks = new List<Task<CrawlWebsiteResult>>();

            // start crawlers.
            foreach (CrawlBase crawler in crawlers)
            {
                // get old items for the crawler
                int sourceID = crawler.NewsSourceID;
                List<SItem> c_items = items.Where(i => i.SourceID == sourceID).ToList();

                // initialize and start crawl task.
                Task<CrawlWebsiteResult> crawl_task = Task.Factory.StartNew((arg) =>
                    {
                        // log.
                        // Helpers.InsertLog("crawler " +  crawler.NewsSourceID, "started");

                        // start crawler.
                        CrawlWebsiteResult result = crawler.Crawl((List<SItem>)arg);

                        // log.
                        // Helpers.InsertLog("crawler " + crawler.NewsSourceID, string.Format("finished with {0} new items", result.NewItems.Count));

                        // return result.
                        return result;
                    }
                    , c_items, TaskCreationOptions.LongRunning // << Create new thread for each task.
                    );

                // add task to list.
                crawl_tasks.Add(crawl_task);
            }

            // wait for all crawlers to finish.
            bool allTasksCompleted = Task.WaitAll(crawl_tasks.ToArray(), TimeSpan.FromHours(1));

            // log.
            // Helpers.InsertLog("crawler", "all tasks finished");

            // collect results.
            foreach (var task in crawl_tasks)
            {
                CrawlWebsiteResult res = task.Result;

                List<NewsItem> resItems = res.NewItems;

                foreach (NewsItem item in resItems)
                {
                    items.Add(new SItem()
                    {
                        Title = item.Title,
                        SourceID = item.NewsSourceID,
                        URL = item.URL
                    });
                }

            }
        }

    }//class
}//namespace
