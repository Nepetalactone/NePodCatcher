using System;
using System.Collections.Generic;
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
        private String _rssLocation;
        public FeedItem[] FeedItems { get; private set; }

        public PodCatcher(String rssLocation, String downloadFolder)
        {
            _rssLocation = rssLocation;
            DownloadFolder = downloadFolder;
        }

        public void ParseRss(String rss)
        {
            var rssFeed = XDocument.Parse(rss);
            FeedItems = (from item in rssFeed.Descendants("item")
                        select new FeedItem()
                        {
                            Title = item.Element("title").Value,
                            DatePublished = DateTime.Parse(item.Element("pubDate").Value),
                            URL = item.Element("link").Value,
                            Description = item.Element("description").Value,
                            AudioCodec = GetMediaType(item.ToString()),
                            DownloadLink = GetMediaLink(item.ToString(), GetMediaType(item.ToString()))
                        }).ToArray();
        }

        public void DownloadRss()
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                client.Headers.Add("Accept-Language", "de,en-US;q=0.7,en;q=0.3");
                client.Headers.Add("Accept-Encoding", "gzip, deflate");
                client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:34.0) Gecko/20100101 Firefox/34.0");
                String rssContent = client.DownloadString(_rssLocation);
                ParseRss(rssContent);
            }
        }

        public void DownloadRssItem(FeedItem feedItem)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                client.Headers.Add("Accept-Language", "de,en-US;q=0.7,en;q=0.3");
                client.Headers.Add("Accept-Encoding", "gzip, deflate");
                client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:34.0) Gecko/20100101 Firefox/34.0");
                client.DownloadFile(feedItem.DownloadLink, DownloadFolder + "//" + FeedItems[0].Title + feedItem.AudioCodec.GetEnding());
            }
        }

        private String GetMediaLink(String rssDescription, Codec codec)
        {
            String[] asdf = rssDescription.Split('"');

            var media = (from item in asdf
                        where item.StartsWith("http://") && item.Contains(".mp3")
                        select item).First();
            return media;
        }

        private Codec GetMediaType(String rssDescription)
        {
            foreach (Codec codec in Enum.GetValues(typeof(Codec)))
            {
                if (rssDescription.Contains(codec.GetEnding()))
                {
                    return codec;
                }
            }

            throw new CodecNotFoundException();
        }
    }
}
