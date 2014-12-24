using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NePodCatcher
{
    class PodCatcher
    {
        public String DownloadFolder { get; private set; }
        private readonly String _rssLocation;
        public FeedItem[] FeedItems { get; private set; }
        public DateTime MinPublishDate { get; set; }

        public PodCatcher(String rssLocation, String downloadFolder)
        {
            _rssLocation = rssLocation;
            DownloadFolder = downloadFolder;
            MinPublishDate = DateTime.MinValue;
        }

        public PodCatcher(String rssLocation, String downloadFolder, DateTime minPublishDate) : this(rssLocation, downloadFolder)
        {
            MinPublishDate = minPublishDate;
        }

        public void ParseRss(String rss)
        {
            var rssFeed = XDocument.Parse(rss);
            var feedItemList = (from item in rssFeed.Descendants("item")
                        select new FeedItem
                        {
                            Title = item.Element("title").Value,
                            DatePublished = DateTime.Parse(item.Element("pubDate").Value),
                            URL = item.Element("link").Value,
                            Description = item.Element("description").Value,
                        }).ToList();

            List<int> itemsToRemove = new List<int>();

            foreach (FeedItem item in feedItemList)
            {
                item.AudioCodec = GetMediaType(item.Description);

                if (item.AudioCodec != Codec.NoCodec)
                {
                    item.DownloadLink = GetMediaLink(item.Description, item.AudioCodec);
                }
            }

            FeedItems = (from item in feedItemList
                where item.AudioCodec != Codec.NoCodec &&
                (DateTime.Compare(item.DatePublished, MinPublishDate) > 0)
                select item).ToArray();
        }

        public void DownloadRss()
        {
            using (WebClient client = new WebClient())
            {
                InitWebClient(client);
                String rssContent = client.DownloadString(_rssLocation);
                ParseRss(rssContent);
            }
        }

        public void DownloadRssItem(FeedItem feedItem)
        {
            using (WebClient client = new WebClient())
            {
                InitWebClient(client);
                try
                {
                    client.DownloadFileAsync(new Uri(feedItem.DownloadLink),
                        DownloadFolder + "//" + FeedItems[0].Title + feedItem.AudioCodec.GetEnding());
                }
                catch (WebException w)
                {
                    Console.WriteLine("Error: " + w.Response);
                }
            }
        }

        public void DownloadAllRssItems()
        {
            foreach (FeedItem item in FeedItems)
            {
                if (DateTime.Compare(item.DatePublished, MinPublishDate) > 0)
                {
                    DownloadRssItem(item);
                }
            }
        }

        private static void InitWebClient(WebClient client)
        {
            client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            client.Headers.Add("Accept-Language", "de,en-US;q=0.7,en;q=0.3");
            client.Headers.Add("Accept-Encoding", "gzip, deflate");
            client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:34.0) Gecko/20100101 Firefox/34.0");
        }

        private String GetMediaLink(String rssDescription, Codec codec)
        {
            String[] asdf = rssDescription.Split('"');

            var media = (from item in asdf
                        where item.StartsWith("http://") && item.Contains(codec.GetEnding())
                        select item).First();
            return media;
        }

        private Codec GetMediaType(String rssDescription)
        {
            foreach (Codec codec in Enum.GetValues(typeof(Codec)))
            {
                if (codec != Codec.NoCodec)
                {
                    if (rssDescription.Contains(codec.GetEnding()))
                    {
                        return codec;
                    }
                }
            }

            return Codec.NoCodec;
        }

        public String[] GetTitles()
        {
            return (from item in FeedItems
                select item.Title).ToArray();
        }

        public bool IsFeedItemLocallyAvailable(FeedItem item)
        {
            DirectoryInfo downloadDir = new DirectoryInfo(DownloadFolder);
            foreach (FileInfo file in downloadDir.GetFiles())
            {
                if (file.Name.Equals(item.Title + item.AudioCodec.GetEnding()))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
