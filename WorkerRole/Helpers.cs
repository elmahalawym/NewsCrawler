using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using Abot;
using Abot.Poco;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage.Table;
using System.Diagnostics;
using System.Data.Services.Client;

namespace WorkerRole
{
    public static class Helpers
    {
        private static string TABLE_NAME = "crawlerlogs";
        
        #region logs

        
        public static void InsertLog(string message, string type)
        {
            try
            {
                // connect to storage account
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                     CloudConfigurationManager.GetSetting("StorageConnectionString"));

                // Create the table client.
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                //Create the CloudTable that represents the "log" table.
                CloudTable table = tableClient.GetTableReference(TABLE_NAME);

                // Create the table if it doesn't exist.
                //table.CreateIfNotExists();

                // create message
                LogEntity log = new LogEntity(message, type + " " + DateTime.UtcNow.Year + " " + DateTime.UtcNow.Month +
                     " " + DateTime.UtcNow.Day + " " + DateTime.UtcNow.Hour + " " + DateTime.UtcNow.Minute + " " + DateTime.UtcNow.Millisecond);

                // Create the TableOperation that inserts the customer entity.
                TableOperation insertOperation = TableOperation.Insert(log);

                table.Execute(insertOperation);
            }
            catch(Exception ex)
            {
                Trace.TraceInformation("Failed to insert to logtable, Exception: {0}", ex.Message);
                Trace.Flush();
            }
        }
        

        #endregion

        #region OrganizerService
        /*
        public static List<NewsItem> GetNewsItemsBySourceID(int sourceId)
        {
            SNEntities context = new SNEntities(new Uri("http://smartnewsws.cloudapp.net/SNDataService.svc"));
            List<Item> items = context.Items.Where(n => n.IDNewsSources == sourceId).ToList();

            List<NewsItem> res = new List<NewsItem>();
            foreach (var item in items)
            {
                res.Add(new NewsItem()
                    {
                        Title = item.Title,
                        URL = item.URL
                    });
            }
            return res;
        }

        public static bool DatabaseInsert(NewsItem[] items)
        {
            try
            {
                SNEntities entities = new SNEntities(new Uri("http://smartnewsws.cloudapp.net/SNDataService.svc", UriKind.Absolute));
                foreach (var item in items)
                {

                    entities.AddToItems(new Item()
                        {
                            Title = item.Title,
                            Content = item.ItemContent,
                            ImageUrl = item.ImageURL,
                            URL = item.URL,
                            IDNewsSources = item.NewsSourceID,
                            DateOfItem = item.DateTime,
                            CategoryID = item.CategoryID
                        });
                }

                //Handling requests Asynchronous
                entities.BeginSaveChanges(ExecutionCompleted, entities);

                Trace.WriteLine("Run");
                return true;
            }
            catch (DataServiceQueryException)
            {
          //      throw new ApplicationException(
          //  "An error occurred during query execution.", ex);
                return false;
            }
        }

        private static void ExecutionCompleted(IAsyncResult result)
        {
            SNEntities entities = (SNEntities)result.AsyncState;
            if (entities.EndSaveChanges(result).IsBatchResponse)
            {
                Trace.WriteLine("Process Succeeded !");
            }
        }
        */
        #endregion


        #region SqlServerDB
        
        private const string connectionString = @"
Server=tcp:v4ngt5uch2.database.windows.net,1433;Database=SmartNews;User ID=SmartNewsDB@v4ngt5uch2;Password=smartnewsPa55;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;
";

        public static bool DatabaseInsert(NewsItem item)
        {
            // database connection
            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                // try to open connection.
                connection.Open();

                // setting command type and text.
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;

                // specifying connection for the MySqlCommand.
                cmd.Connection = connection;

                if (item.CategoryID > 0)
                {
                    cmd.CommandText = @"
INSERT INTO [dbo].[Items]
           ([URL]
           ,[Title]
           ,[Content]
           ,[DateOfItem]
           ,[ImageUrl]
           ,[IDNewsSources]
		   ,[CategoryID])
     VALUES(
            @url, @title, @content, @dateOfItem, @imageUrl, @sourceId, @categoryId)
";
                    // specify command parameters
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    parameters.Add(new SqlParameter("@title", item.Title));
                    parameters.Add(new SqlParameter("@url", item.URL));
                    parameters.Add(new SqlParameter("@content", item.ItemContent));
                    parameters.Add(new SqlParameter("@dateOfItem", item.DateTime));
                    parameters.Add(new SqlParameter("@imageUrl", item.ImageURL));
                    parameters.Add(new SqlParameter("@sourceId", item.NewsSourceID));
                    parameters.Add(new SqlParameter("@categoryId", item.CategoryID));

                    // clear and add new parameters
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddRange(parameters.ToArray());

                    // execute query
                    cmd.ExecuteNonQuery();
                }
                else    // without category ID
                {
                    cmd.CommandText = @"
INSERT INTO [dbo].[Items]
           ([URL]
           ,[Title]
           ,[Content]
           ,[DateOfItem]
           ,[ImageUrl]
           ,[IDNewsSources])
     VALUES(
            @url, @title, @content, @dateOfItem, @imageUrl, @sourceId)
";
                    // specify command parameters
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    parameters.Add(new SqlParameter("@title", item.Title));
                    parameters.Add(new SqlParameter("@url", item.URL));
                    parameters.Add(new SqlParameter("@content", item.ItemContent));
                    parameters.Add(new SqlParameter("@dateOfItem", item.DateTime));
                    parameters.Add(new SqlParameter("@imageUrl", item.ImageURL));
                    parameters.Add(new SqlParameter("@sourceId", item.NewsSourceID));

                    // clear and add new parameters
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddRange(parameters.ToArray());

                    // execute query
                    cmd.ExecuteNonQuery();
                }

                // close connection
                connection.Close(); connection.Dispose();

                // return success
                return true;
            }
            catch (Exception) 
            {
                // close connection
                connection.Close(); connection.Dispose();

                return false; // failure 
            }
        }

        public static bool DatabaseInsert(NewsItem[] items)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                // try to open connection.
                connection.Open();

                // setting command type and text.
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;

                // specifying connection for the MySqlCommand.
                cmd.Connection = connection;

                // iterate over news items and insert each news item to database.
                for (int i = 0, n = items.Length; i < n; i++)
                {
                    NewsItem item = items[i];

                    if (item.CategoryID > 0)
                    {
                        cmd.CommandText = @"
INSERT INTO [dbo].[Items]
           ([URL]
           ,[Title]
           ,[Content]
           ,[DateOfItem]
           ,[ImageUrl]
           ,[IDNewsSources]
		   ,[CategoryID])
     VALUES(
            @url, @title, @content, @dateOfItem, @imageUrl, @sourceId, @categoryId)
";
                        // specify command parameters
                        List<SqlParameter> parameters = new List<SqlParameter>();
                        parameters.Add(new SqlParameter("@title", item.Title));
                        parameters.Add(new SqlParameter("@url", item.URL));
                        parameters.Add(new SqlParameter("@content", item.ItemContent));
                        parameters.Add(new SqlParameter("@dateOfItem", item.DateTime));
                        parameters.Add(new SqlParameter("@imageUrl", item.ImageURL));
                        parameters.Add(new SqlParameter("@sourceId", item.NewsSourceID));
                        parameters.Add(new SqlParameter("@categoryId", item.CategoryID));

                        // clear and add new parameters
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddRange(parameters.ToArray());

                        // execute query
                        cmd.ExecuteNonQuery();
                    }
                    else    // without category ID
                    {
                        cmd.CommandText = @"
INSERT INTO [dbo].[Items]
           ([URL]
           ,[Title]
           ,[Content]
           ,[DateOfItem]
           ,[ImageUrl]
           ,[IDNewsSources])
     VALUES(
            @url, @title, @content, @dateOfItem, @imageUrl, @sourceId)
";
                        // specify command parameters
                        List<SqlParameter> parameters = new List<SqlParameter>();
                        parameters.Add(new SqlParameter("@title", item.Title));
                        parameters.Add(new SqlParameter("@url", item.URL));
                        parameters.Add(new SqlParameter("@content", item.ItemContent));
                        parameters.Add(new SqlParameter("@dateOfItem", item.DateTime));
                        parameters.Add(new SqlParameter("@imageUrl", item.ImageURL));
                        parameters.Add(new SqlParameter("@sourceId", item.NewsSourceID));

                        // clear and add new parameters
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddRange(parameters.ToArray());

                        // execute query
                        cmd.ExecuteNonQuery();
                    }

                }

                // log
              //  InsertLog(items.Length + " items added to DB", "");

                // close connection
                connection.Close(); connection.Dispose();

                // return success
                return true;
            }
            catch (Exception ex) // failure
            {
            //    InsertLog("Failed to insert items to the database", "Exception: " + ex.Message);
                // close connection
                connection.Close(); connection.Dispose();

                return false;
            }
            finally {  }     // close connection
        }

        public static List<SItem> GetAllNewsItems()
        {
            // a list for the result.
            List<SItem> result = new List<SItem>();

            // create SqlConnection.
            SqlConnection cn = new SqlConnection(connectionString);

            try
            {
                // try to open connection
                cn.Open();

                // create MySqlCommand
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;

                // specifying sql command text 
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "select URL, Title, IDNewsSources from Items";

                // read query execution result in a MySqlDataReader object.
                SqlDataReader reader = cmd.ExecuteReader();

                List<SItem> res = new List<SItem>();

                // iterate over all rows.
                while (reader.Read())
                {
                    SItem item = new SItem();
                    item.Title = reader["Title"].ToString();
                    item.URL = reader["URL"].ToString();
                    item.SourceID = int.Parse(reader["IDNewsSources"].ToString());

                    res.Add(item);
                }

                // close connection
                cn.Close();
                cn.Dispose();

                // return the list of the news items.
                return res;
            }
            catch(Exception)
            {
                // close connection
                cn.Close();
                cn.Dispose();

                return null;
            }
        }

        public static List<SItem> GetNewsItemsBySourceID(int sourceId)
        {
            // create SqlConnection.
            SqlConnection cn = new SqlConnection(connectionString);

            try
            {
                // try to open connection
                cn.Open();

                // create MySqlCommand
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;

                // specifying sql command text 
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "select URL, Title, IDNewsSources from Items where IDNewsSources = @sourceId";

                // add parameter
                cmd.Parameters.Add(new SqlParameter("@sourceId", sourceId));

                // read query execution result in a MySqlDataReader object.
                SqlDataReader reader = cmd.ExecuteReader();

                List<SItem> res = new List<SItem>();

                // iterate over all rows.
                while (reader.Read())
                {
                    SItem item = new SItem();
                    item.SourceID = sourceId;
              //      item.Id = int.Parse(reader["ItemID"].ToString());
                    item.Title = reader["Title"].ToString();
                    item.URL = reader["URL"].ToString();
               //     item.ItemContent = reader["Content"].ToString();
               //     item.DateTime = (DateTime)reader["DateOfItem"];
               //     item.ImageURL = reader["ImageURL"].ToString();

                    res.Add(item);
                }

                // return the list of the news items.
                return res;
            }
            catch (Exception)
            {
           //     Helpers.InsertLog("Failed to get old items from the database", "Exception: " + ex.Message);
                return null;
            }
            finally { cn.Close(); cn.Dispose(); }
        }

        #endregion

        #region MySqlDB
        /*
        //        private const string connectionString = @"
//Database=SmartNews;Data Source=us-cdbr-azure-west-b.cleardb.com;User Id=bc697408592aa5;Password=f0c7a79b
//";
        private const string connectionString = @"
Database=as_a0577233d0369fa;Data Source=us-cdbr-azure-west-b.cleardb.com;User Id=be8732ad83f646;Password=2acecdc8

";

        /// <summary>
        /// inserts a news item to the database.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool DatabaseInsert(NewsItem item)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                // try to open connection
                connection.Open();

                // setting command type and text
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = @"
insert into items (Title, URL, Content, DateOfItem, ImageURL, IDNewsSources)
values (@title, @url, @content, @dateOfItem, @imageUrl, @sourceId)
";
                // specify command parameters
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                parameters.Add(new MySqlParameter("@title", item.Title));
                parameters.Add(new MySqlParameter("@url", item.URL));
                parameters.Add(new MySqlParameter("@content", item.ItemContent));
                parameters.Add(new MySqlParameter("@dateOfItem", item.DateTime));
                parameters.Add(new MySqlParameter("@imageUrl", item.ImageURL));
                parameters.Add(new MySqlParameter("@sourceId", item.NewsSourceID));

                // clear and add new parameters
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(parameters.ToArray());

                // specifying connection for the MySqlCommand
                cmd.Connection = connection;
                
                // execute the query and return the result ( returns the number of rows affected).
                // if res > 0 then the row is inserted successfully.
                return (cmd.ExecuteNonQuery() > 0);
            }
            catch (Exception) { return false; }    // failure
            finally { connection.Close(); connection.Dispose(); }        // close connection
        }

        /// <summary>
        /// inserts a collection of news items to the database.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static bool DatabaseInsert(NewsItem[] items)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                // try to open connection.
                connection.Open();

                // setting command type and text.
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;

                // specifying connection for the MySqlCommand.
                cmd.Connection = connection;

                // iterate over news items and insert each news item to database.
                for (int i = 0, n = items.Length; i < n; i++ )
                {
                    NewsItem item = items[i];

                    cmd.CommandText = @"
insert into items (Title, URL, Content, DateOfItem, ImageURL, IDNewsSources)
values (@title, @url, @content, @dateOfItem, @imageUrl, @sourceId)
";
                    // specify command parameters
                    List<MySqlParameter> parameters = new List<MySqlParameter>();
                    parameters.Add(new MySqlParameter("@title", item.Title));
                    parameters.Add(new MySqlParameter("@url", item.URL));
                    parameters.Add(new MySqlParameter("@content", item.ItemContent));
                    parameters.Add(new MySqlParameter("@dateOfItem", item.DateTime));
                    parameters.Add(new MySqlParameter("@imageUrl", item.ImageURL));
                    parameters.Add(new MySqlParameter("@sourceId", item.NewsSourceID));

                    // clear and add new parameters
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddRange(parameters.ToArray());

                    // execute query
                    cmd.ExecuteNonQuery();
                }

                // return success
                return true;
            }
            catch (Exception) { return false; }                       // failure
            finally { connection.Close(); connection.Dispose(); }     // close connection
        }

        /// <summary>
        /// gets all news items from the database.
        /// </summary>
        public static List<NewsItem> GetAllNewsItems()
        {
            // create MySqlConnection.
            MySqlConnection cn = new MySqlConnection(connectionString);

            try
            {
                // try to open connection
                cn.Open();

                // create MySqlCommand
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = cn;

                // specifying sql command text 
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "select * from items";

                // read query execution result in a MySqlDataReader object.
                MySqlDataReader reader = cmd.ExecuteReader();

                List<NewsItem> res = new List<NewsItem>();

                // iterate over all rows.
                while (reader.Read())
                {
                    // generate and add new NewsItem.
                    res.Add(new NewsItem()
                    {
                        Title = reader["Title"].ToString(),
                        URL = reader["URL"].ToString(),
                        ItemContent = reader["Content"].ToString(),
                        DateTime = DateTime.Parse(reader["DateOfItem"].ToString()),
                        ImageURL = reader["ImageURL"].ToString()
                    });
                }
                
                // return the list of the news items.
                return res;
            }
            catch (Exception) { return null; }
            finally { cn.Close(); cn.Dispose(); }
        }

        /// <summary>
        /// gets all news items from the given source id.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        public static List<NewsItem> GetNewsItemsBySourceID(int sourceId)
        {
            // create MySqlConnection.
            MySqlConnection cn = new MySqlConnection(connectionString);

            try
            {
                // try to open connection
                cn.Open();

                // create MySqlCommand
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = cn;

                // specifying sql command text 
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "select * from items where IDNewsSources = @sourceId";

                // add parameter
                cmd.Parameters.Add(new MySqlParameter("@sourceId", sourceId));

                // read query execution result in a MySqlDataReader object.
                MySqlDataReader reader = cmd.ExecuteReader();

                List<NewsItem> res = new List<NewsItem>();

                // iterate over all rows.
                while (reader.Read())
                {
                    // generate and add new NewsItem.
                    res.Add(new NewsItem()
                    {
                        Title = reader["Title"].ToString(),
                        URL = reader["URL"].ToString(),
                        ItemContent = reader["Content"].ToString(),
                        DateTime = DateTime.Parse(reader["DateOfItem"].ToString()),
                        ImageURL = reader["ImageURL"].ToString()
                    });
                }

                // return the list of the news items.
                return res;
            }
            catch (Exception ex) { return null; }
            finally { cn.Close(); cn.Dispose(); }
        }
         
        */
        #endregion


        /// <summary>
        /// gets crawl configurations.
        /// </summary>
        /// <returns></returns>
        public static CrawlConfiguration GetCrawlConfigurations()
        {
            // configuration
            CrawlConfiguration config = new CrawlConfiguration();
            config.MaxConcurrentThreads = 1;
            config.MaxPagesToCrawl = 500;              // <<<<<<<<<<<<<<<<<<<<<<<< Needs to be changed after debugging
            config.MaxPagesToCrawlPerDomain = 500;     // <<<<<<<<<<<<<<<<<<<<<< ******************************
            config.MaxPageSizeInBytes = 0;
            config.IsRespectRobotsDotTextEnabled = false;
            config.UserAgentString = @"Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; abot v@ABOTASSEMBLYVERSION@ http://code.google.com/p/abot)";
            config.CrawlTimeoutSeconds = 0;
            config.IsUriRecrawlingEnabled = false;
            config.IsExternalPageCrawlingEnabled = false;
            config.IsExternalPageLinksCrawlingEnabled = false;
            config.DownloadableContentTypes = "text/html,text/plain";
            config.HttpServicePointConnectionLimit = 0;
            config.HttpRequestTimeoutInSeconds = 0;
            config.HttpRequestMaxAutoRedirects = 0;
            config.IsHttpRequestAutoRedirectsEnabled = false;
            config.IsHttpRequestAutomaticDecompressionEnabled = false;
            config.MinAvailableMemoryRequiredInMb = 0;
            config.MaxCrawlDepth = 5000;
            return config;
        }
    }


    public class LogEntity : TableEntity
    {
        public LogEntity(string message, string type)
        {
            this.PartitionKey = message;
            this.RowKey = type;
        }

    }
}
