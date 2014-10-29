namespace GodLesZ.Tools.RtlNowReader {
    partial class FormMain {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.listShows = new System.Windows.Forms.ListView();
            this.colShowName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imagesShowThumbs = new System.Windows.Forms.ImageList(this.components);
            this.listEpisodes = new System.Windows.Forms.ListView();
            this.colEpisodesName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtRtmpdump = new System.Windows.Forms.TextBox();
            this.imagesEpisodeThumbs = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // listShows
            // 
            this.listShows.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listShows.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colShowName});
            this.listShows.FullRowSelect = true;
            this.listShows.GridLines = true;
            this.listShows.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listShows.HideSelection = false;
            this.listShows.Location = new System.Drawing.Point(13, 13);
            this.listShows.MultiSelect = false;
            this.listShows.Name = "listShows";
            this.listShows.Size = new System.Drawing.Size(355, 395);
            this.listShows.SmallImageList = this.imagesShowThumbs;
            this.listShows.TabIndex = 0;
            this.listShows.UseCompatibleStateImageBehavior = false;
            this.listShows.View = System.Windows.Forms.View.Details;
            this.listShows.SelectedIndexChanged += new System.EventHandler(this.listShows_SelectedIndexChanged);
            // 
            // colShowName
            // 
            this.colShowName.Text = "Name";
            this.colShowName.Width = 319;
            // 
            // imagesShowThumbs
            // 
            this.imagesShowThumbs.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imagesShowThumbs.ImageSize = new System.Drawing.Size(50, 28);
            this.imagesShowThumbs.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // listEpisodes
            // 
            this.listEpisodes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listEpisodes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colEpisodesName});
            this.listEpisodes.FullRowSelect = true;
            this.listEpisodes.GridLines = true;
            this.listEpisodes.HideSelection = false;
            this.listEpisodes.Location = new System.Drawing.Point(375, 13);
            this.listEpisodes.MultiSelect = false;
            this.listEpisodes.Name = "listEpisodes";
            this.listEpisodes.Size = new System.Drawing.Size(412, 176);
            this.listEpisodes.SmallImageList = this.imagesEpisodeThumbs;
            this.listEpisodes.TabIndex = 1;
            this.listEpisodes.UseCompatibleStateImageBehavior = false;
            this.listEpisodes.View = System.Windows.Forms.View.Details;
            this.listEpisodes.SelectedIndexChanged += new System.EventHandler(this.listEpisodes_SelectedIndexChanged);
            // 
            // colEpisodesName
            // 
            this.colEpisodesName.Text = "Name";
            this.colEpisodesName.Width = 377;
            // 
            // txtRtmpdump
            // 
            this.txtRtmpdump.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRtmpdump.Location = new System.Drawing.Point(375, 195);
            this.txtRtmpdump.Multiline = true;
            this.txtRtmpdump.Name = "txtRtmpdump";
            this.txtRtmpdump.ReadOnly = true;
            this.txtRtmpdump.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtRtmpdump.Size = new System.Drawing.Size(412, 213);
            this.txtRtmpdump.TabIndex = 2;
            // 
            // imagesEpisodeThumbs
            // 
            this.imagesEpisodeThumbs.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imagesEpisodeThumbs.ImageSize = new System.Drawing.Size(50, 28);
            this.imagesEpisodeThumbs.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(799, 420);
            this.Controls.Add(this.txtRtmpdump);
            this.Controls.Add(this.listEpisodes);
            this.Controls.Add(this.listShows);
            this.MaximumSize = new System.Drawing.Size(815, 459);
            this.Name = "FormMain";
            this.Text = "RtlNow RTMPE/HDS Dumper - by GodLesZ";
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listShows;
        private System.Windows.Forms.ColumnHeader colShowName;
        private System.Windows.Forms.ImageList imagesShowThumbs;
        private System.Windows.Forms.ListView listEpisodes;
        private System.Windows.Forms.ColumnHeader colEpisodesName;
        private System.Windows.Forms.TextBox txtRtmpdump;
        private System.Windows.Forms.ImageList imagesEpisodeThumbs;
    }
}

