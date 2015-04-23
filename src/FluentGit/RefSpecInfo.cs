using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FluentGit
{
    public class RefSpecInfo
    {
        public string Source { get; set; }
        public string Destination { get; set; }
        public bool ForceUpdateWhenFastForwardNotAllowed { get; set; }

        public override string ToString()
        {
            // example refspec:
            // +refs/heads/*:refs/remotes/origin/*
            var builder = new StringBuilder();
            if (ForceUpdateWhenFastForwardNotAllowed)
            {
                builder.Append('+');
            }
            builder.Append(Source);
            builder.Append(':');
            builder.Append(Destination);

            return builder.ToString();
        }

        public static RefSpecInfo Parse(string refSpec)
        {
            var refSpecInfo = new RefSpecInfo();

            using (StringReader reader = new StringReader(refSpec))
            {
                var peakChar = (char)reader.Peek();
                if (peakChar == '+')
                {
                    reader.Read();
                    refSpecInfo.ForceUpdateWhenFastForwardNotAllowed = true;
                }

                var line = reader.ReadLine();
                var segments = line.Split(':');
                if (segments.Count() != 2)
                {
                    throw new FormatException("Invalid refspec string. It should contain a colon seperator.");
                }

                refSpecInfo.Source = segments[0];
                refSpecInfo.Destination = segments[1].TrimEnd(); // remove any whitespace from end.
            }

            return refSpecInfo;

        }

        public static bool TryParse(string refSpec, out RefSpecInfo refSpecInfo)
        {
            refSpecInfo = null;
            try
            {
                refSpecInfo = RefSpecInfo.Parse(refSpec);
                return true;
            }
            catch (Exception)
            {
                refSpecInfo = null;
                return false;
            }
        }
    }
}
