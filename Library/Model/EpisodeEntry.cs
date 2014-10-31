using System.Drawing;
using System.IO;
using System.Net;

namespace GodLesZ.Tools.RtlNowReader.Library.Model {

    public class EpisodeEntry {
        protected Image _poster = null;

        public string Name {
            get;
            set;
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
            if (_poster != null) {
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