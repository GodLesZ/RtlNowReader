using System.Drawing;
using System.IO;
using System.Net;

namespace GodLesZ.Tools.RtlNowReader.Library.Model {

    public class EpisodeEntry {

        public string Name {
            get;
            set;
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
            var client = new WebClient();
            var buf = client.DownloadData(Poster);
            using (var stream = new MemoryStream(buf)) {
                return Image.FromStream(stream);
            }
        }

    }

}