using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NePodCatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            PodCatcher catcher = new PodCatcher(@"http://www.droppinggems.com/podcasts?format=RSS", @"H:\Downloads\Podcatcher");
            catcher.DownloadRss();

            foreach (String title in catcher.GetTitles())
            {
                Console.WriteLine(title);
            }

            foreach (FeedItem item in catcher.FeedItems)
            {
                if (catcher.IsFeedItemLocallyAvailable(item))
                {
                    Console.WriteLine("File already exists");
                }
                else
                {
                    catcher.DownloadRssItem(item);
                }
            }

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
