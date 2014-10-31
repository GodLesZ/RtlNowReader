using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using CsQuery;
using GodLesZ.Tools.RtlNowReader.Library.Model;
using GodLesZ.Tools.RtlNowReader.Properties;

namespace GodLesZ.Tools.RtlNowReader {

    public partial class FormMain : Form {
        private readonly List<ShowEntry> _shows = new List<ShowEntry>();

        public FormMain() {
            InitializeComponent();
        }


        private void FormMain_Shown(object sender, EventArgs e) {
            var loader = new BackgroundWorker();
            loader.DoWork += ShowLoaderOnDoWork;
            loader.RunWorkerCompleted += ShowLoaderOnRunWorkerCompleted;
            loader.RunWorkerAsync();
        }

        #region Show Loader
        private void ShowLoaderOnDoWork(object sender, DoWorkEventArgs args) {
            _shows.Clear();

            var client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/38.0.2125.111 Safari/537.36");
            var html = client.DownloadString("http://rtl-now.rtl.de/sendung_a_z.php");
            var split = html.Split(new[] { "<div class=\"m03medium\"" }, StringSplitOptions.None);
            foreach (var glue in split) {
                var match = Regex.Match(glue, "(<h.>.+?</h.>)", RegexOptions.IgnoreCase);
                if (match.Success == false) {
                    continue;
                }

                var title = MakeUtf8(CQ.Create(match.Groups[0].Captures[0].Value)[0].InnerText.Trim());
                match = Regex.Match(glue, "href=\"(.+?)\"", RegexOptions.IgnoreCase);
                var url = match.Groups[1].Captures[0].Value.Trim();
                match = Regex.Match(glue, "src=\"(.+?)\"", RegexOptions.IgnoreCase);
                var thumb = match.Groups[1].Captures[0].Value.Trim();

                if (title.Length == 0) {
                    title = url;
                }
                /*
                var glueLower = glue.ToLower();
                var isFreeOrNew = glueLower.Contains("class=\"m03date\">free") || glueLower.Contains("class=\"m03date\">new");
                if (isFreeOrNew == false) {
                    continue;
                }
                */

                _shows.Add(new ShowEntry {
                    Name = title,
                    ThumbUrl = thumb,
                    Link = url
                });
            }

        }

        private void ShowLoaderOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs args) {
            listShows.Items.Clear();
            listEpisodes.Items.Clear();
            imagesShowThumbs.Images.Clear();

            if (_shows.Count == 0) {
                MessageBox.Show(Resources.Error_ShowLoader_NoShowFound_Msg, Resources.Error_ShowLoader_NoShowFound_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (var showEntry in _shows) {
                imagesShowThumbs.Images.Add(showEntry.FetchThumb());
                listShows.Items.Add(new ListViewItem(new[] {
                    showEntry.Name
                }, imagesShowThumbs.Images.Count - 1));
            }

            listShows.Focus();
        }
        #endregion

        #region Episode Loader
        private void EpisodeLoaderOnDoWork(object sender, DoWorkEventArgs args) {
            var showIndex = (int)args.Argument;
            var showEntry = _shows[showIndex];

            var client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, "");
            var html = client.DownloadString("http://rtl-now.rtl.de/" + showEntry.Link);
            CQ doc = html;
            var episodeContainer = doc["div.line"];

            var episodeList = (
                from container
                    in episodeContainer.Elements
                select
                    CQ.Create(container) into containerCq
                let minibtn = containerCq["a.minibutton:first"]
                where
                    minibtn.Length != 0 &&
                    minibtn[0].InnerText.ToLower().Trim() == "kostenlos"
                let videoName = containerCq["div.title a:first"][0].InnerText.Trim()
                let videoDate = CQ.Create(containerCq["div.time:first"][0].InnerHTML).Remove("div").Render().Trim().Split(' ')[0]
                let videoTitle = string.Format("{0} - {1}", videoDate, videoName)
                let videoUrl = MakeUtf8(containerCq["div.title a:first"].Attr("href").Trim())
                let match = Regex.Match(html, "<meta property=\"og:image\" content=\"(.+?)\"", RegexOptions.IgnoreCase)
                let videoPoster = (match.Success ? match.Groups[1].Captures[0].Value.Trim() : "")
                select new EpisodeEntry {
                    Name = videoTitle,
                    Link = videoUrl,
                    Poster = videoPoster
                }
            ).ToList();

            showEntry.Episodes = episodeList;
        }

        private void EpisodeLoaderOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs args) {
            listEpisodes.Items.Clear();
            imagesEpisodeThumbs.Images.Clear();

            var showEntry = _shows[listShows.SelectedIndices[0]];
            if (showEntry.Episodes == null || showEntry.Episodes.Count == 0) {
                MessageBox.Show(Resources.Error_EpisodeLoader_NoFreeEpisodes_Msg, Resources.Error_EpisodeLoader_NoFreeEpisodes_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                UnblockUi();
                listEpisodes.Focus();
                return;
            }

            foreach (var episodeEntry in showEntry.Episodes) {
                imagesEpisodeThumbs.Images.Add(episodeEntry.FetchPoster());
                var item = new ListViewItem(new[] {
                    episodeEntry.Name
                }, imagesEpisodeThumbs.Images.Count - 1);
                if (File.Exists(episodeEntry.Filepath)) {
                    foreach (var subItem in item.SubItems) {
                        ((ListViewItem.ListViewSubItem) subItem).ForeColor = Color.LimeGreen;
                    }
                }
                listEpisodes.Items.Add(item);
            }

            UnblockUi();
            listEpisodes.Focus();
        }
        #endregion

        #region Video Loader
        private void VideoLoaderOnDoWork(object sender, DoWorkEventArgs args) {
            var worker = (BackgroundWorker)sender;
            var indexData = (int[])args.Argument;
            var showEntry = _shows[indexData[0]];
            var episodeEntry = showEntry.Episodes[indexData[1]];

            args.Result = null;

            var client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, "");
            var pageUrl = "http://rtl-now.rtl.de/" + episodeEntry.Link;
            var html = client.DownloadString(pageUrl);
            if (html.Contains("<div>DAS TUT UNS LEID!</div>")) {
                return;
            }

            var match = Regex.Match(html, "data:'(.+?)'", RegexOptions.IgnoreCase);
            if (match.Success == false) {
                return;
            }

            var streamUrlPart = match.Groups[1].Captures[0].Value.Trim();
            var streamUrl = string.Format("http://rtl-now.rtl.de/{0}", HttpUtility.UrlDecode(streamUrlPart));
            html = client.DownloadString(streamUrl);

            match = Regex.Match(html, "<filename.+?><(.+?)>", RegexOptions.IgnoreCase);
            streamUrl = match.Groups[1].Captures[0].Value.Replace("![CDATA[", "").Trim();
            var matchRtmpe = Regex.Match(streamUrl, "rtmpe://(.+?)/(.+?)/(.+?)]", RegexOptions.IgnoreCase);
            var matchHds = Regex.Match(streamUrl, "http://(.+?)/(.+?)/(.+?)/(.+?)/(.+?)\\?", RegexOptions.IgnoreCase);
            if (matchRtmpe.Success) {
                // @TODO
                throw new NotImplementedException("TRMPE stream are not yet supported");
            }
            if (matchHds.Success) {
                var sUrl = string.Format("rtmpe://fms-fra{0}.rtl.de/{1}/", new Random().Next(1, 34), matchHds.Groups[3].Captures[0].Value);
                var sPlaypath = string.Format("mp4:{0}", matchHds.Groups[5].Captures[0].Value.Replace(".f4m", ""));
                // ReSharper disable once ConvertToConstant.Local
                var sSwfVfy = 1;
                var sSwfUrl = string.Format("http://rtl-now.rtl.de/includes/vodplayer.swf");
                var sApp = string.Format("{0}/_definst_", matchHds.Groups[3].Captures[0].Value);
                var sTcUrl = string.Format("rtmpe://fms-fra{0}.rtl.de/{1}/", new Random().Next(1, 34), matchHds.Groups[3].Captures[0].Value);
                var finalUrl = string.Format("{0} playpath={1} swfVfy={2} swfUrl={3} app={4} tcUrl={5} pageUrl={6}", sUrl, sPlaypath, sSwfVfy, sSwfUrl, sApp, sTcUrl, pageUrl);
                /*
                     rtmpe://fms-fra14.rtl.de/rtlnow/ 
                     playpath=mp4:2/V_599458_CRVA_E92407_110379_h264-mq_92d5d4111b04faac734e824b6ec873f.f4v 
                     swfVfy=1 
                     swfUrl=http://rtl-now.rtl.de/includes/vodplayer.swf 
                     app=rtlnow/_definst_ 
                     tcUrl=rtmpe://fms-fra12.rtl.de/rtlnow/ 
                     pageUrl=http://rtl-now.rtl.de/alles-was-zaehlt/familienzuwachs.php?film_id=173505&player=1&season=2014
                 */
                var rtmpdumpArgs = string.Format("-V -i \"{0}\" -o \"{1}\"", finalUrl, episodeEntry.Filename);

                SpawnRtmpdump(rtmpdumpArgs, worker);

                args.Result = episodeEntry.Filename;
            }
        }

        private void VideoLoaderOnProgressChanged(object sender, ProgressChangedEventArgs args) {
            txtRtmpdump.Text += args.UserState + Environment.NewLine;
            txtRtmpdump.SelectionStart = txtRtmpdump.Text.Length;
            txtRtmpdump.ScrollToCaret();
        }

        private void VideoLoaderOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs args) {
            if (args.Result == null) {
                MessageBox.Show(Resources.Error_VideoLoader_FailedToDump_Msg, Resources.Error_VideoLoader_FailedToDump_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                UnblockUi();
                return;
            }

            MessageBox.Show(string.Format("Done!{0}Saved as: {1}", Environment.NewLine, args.Result), Resources.VideoLoader_DumpFinished_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);

            UnblockUi();
        }
        #endregion



        private void listShows_SelectedIndexChanged(object sender, EventArgs e) {
            if (listShows.SelectedIndices.Count == 0) {
                return;
            }

            var showIndex = listShows.SelectedIndices[0];
            var showEntry = _shows[showIndex];
            if (showEntry.Episodes != null && showEntry.Episodes.Count > 0) {
                EpisodeLoaderOnRunWorkerCompleted(null, new RunWorkerCompletedEventArgs(null, null, false));
                return;
            }

            BlockUi();

            var loader = new BackgroundWorker();
            loader.DoWork += EpisodeLoaderOnDoWork;
            loader.RunWorkerCompleted += EpisodeLoaderOnRunWorkerCompleted;
            loader.RunWorkerAsync(showIndex);
        }

        private void listEpisodes_SelectedIndexChanged(object sender, EventArgs e) {
            if (listEpisodes.SelectedIndices.Count == 0) {
                return;
            }

            var showIndex = listShows.SelectedIndices[0];
            var episodeIndex = listEpisodes.SelectedIndices[0];

            if (File.Exists(_shows[showIndex].Episodes[episodeIndex].Filepath)) {
                if (MessageBox.Show(Resources.VideoLoader_FileExists_Msg, Resources.VideoLoader_FileExists_Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) {
                    return;
                }
            }

            txtRtmpdump.Text = "";
            BlockUi();

            var loader = new BackgroundWorker {
                WorkerReportsProgress = true
            };
            loader.DoWork += VideoLoaderOnDoWork;
            loader.RunWorkerCompleted += VideoLoaderOnRunWorkerCompleted;
            loader.ProgressChanged += VideoLoaderOnProgressChanged;
            loader.RunWorkerAsync(new[] { showIndex, episodeIndex });
        }


        private static string MakeUtf8(string text) {
            return Encoding.UTF8.GetString(Encoding.Default.GetBytes(text));
        }

        private void SpawnRtmpdump(string rtmpdumpArgs, BackgroundWorker worker) {
            var startInfo = new ProcessStartInfo {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                FileName = Path.Combine(Directory.GetCurrentDirectory(), "rtmpdump", "rtmpdump.exe"),
                Arguments = rtmpdumpArgs
            };
            var p = new Process {
                StartInfo = startInfo
            };
            p.OutputDataReceived += (s, e) => worker.ReportProgress(1, e.Data);
            p.ErrorDataReceived += (s, e) => worker.ReportProgress(1, e.Data);

            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();

            p.WaitForExit();
        }
        protected void BlockUi() {
            listShows.Enabled = false;
            listEpisodes.Enabled = false;
        }

        protected void UnblockUi() {
            listShows.Enabled = true;
            listEpisodes.Enabled = true;
        }

    }

}
