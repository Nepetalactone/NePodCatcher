using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NePodCatcher
{
    class FeedItem
    {
        public String Title { get; set; }
        public DateTime DatePublished { get; set; }
        public String URL { get; set; }
        public String Description { get; set; }
        public String DownloadLink { get; set; }
        public Codec AudioCodec { get; set; }
    }
}
