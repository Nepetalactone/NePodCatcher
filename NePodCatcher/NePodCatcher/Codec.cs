using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NePodCatcher
{
    public enum Codec
    {
        AAC,
        MP3,
        NoCodec
    }

    public static class Extension
    {
        public static String GetEnding(this Codec codec)
        {
            switch(codec)
            {
                case Codec.AAC:
                    return ".aac";
                case Codec.MP3:
                    return ".mp3";
                default:
                    throw new CodecNotFoundException();
            }
        }
    }
}
