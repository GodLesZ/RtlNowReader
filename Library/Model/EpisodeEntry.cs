using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace GodLesZ.Tools.RtlNowReader.Library.Model {

    public class EpisodeEntry {
        protected Image _poster = null;
        protected string _name;

        public string Name {
            get { return _name; }
            set {
                _name = Regex.Replace(value, "[^a-z0-9-_]", "", RegexOptions.IgnoreCase);
            }
        }

        public string Filename {
            get { return string.Format("{0}.f4v", Name); }
        }

        public string Filepath {
            get { return Path.Combine(Directory.GetCurrentDirectory(), Filename); }
        }

        public string Link {
            get;
            set;
        }

        public string Poster {
            get;
            set;
        }


        public Image FetchPoster() {
            if (_poster != null || string.IsNullOrEmpty(Poster)) {
                return _poster;
            }
              
            var client = new WebClient();
            var buf = client.DownloadData(Poster);
            using (var stream = new MemoryStream(buf)) {
                return Image.FromStream(stream);
            }
        }

    }

}