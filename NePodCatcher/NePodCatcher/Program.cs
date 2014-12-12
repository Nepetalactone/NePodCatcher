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
            PodCatcher catcher = new PodCatcher(@"http://www.droppinggems.com/podcasts?format=RSS", @"C:\Users\Tobias\Downloads");
            catcher.DownloadRss();
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
