using System.Windows.Forms;
namespace TweetKeyPress
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ShowPressStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ResetStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.SetTweetTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SetTweetKeyStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.TweetNowStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NoAutoTweetStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TweetOnExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.ReAuthenticationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.VerInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridView1 = new TweetKeyPress.DoubleBufferedDataGridView();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "TweetKeyPress";
            this.notifyIcon1.Visible = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ShowPressStripMenuItem,
            this.ResetStripMenuItem,
            this.toolStripSeparator1,
            this.SetTweetTimeToolStripMenuItem,
            this.SetTweetKeyStripMenuItem,
            this.toolStripSeparator2,
            this.TweetNowStripMenuItem,
            this.NoAutoTweetStripMenuItem,
            this.TweetOnExitToolStripMenuItem,
            this.toolStripSeparator3,
            this.ReAuthenticationToolStripMenuItem,
            this.VerInfoToolStripMenuItem,
            this.ExitToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowCheckMargin = true;
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(192, 242);
            // 
            // ShowPressStripMenuItem
            // 
            this.ShowPressStripMenuItem.Name = "ShowPressStripMenuItem";
            this.ShowPressStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.ShowPressStripMenuItem.Text = "押した回数の一覧を見る";
            this.ShowPressStripMenuItem.Click += new System.EventHandler(this.ShowPressStripMenuItem1_Click);
            // 
            // ResetStripMenuItem
            // 
            this.ResetStripMenuItem.Name = "ResetStripMenuItem";
            this.ResetStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.ResetStripMenuItem.Text = "押した回数のリセット";
            this.ResetStripMenuItem.Click += new System.EventHandler(this.ResetStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(188, 6);
            // 
            // SetTweetTimeToolStripMenuItem
            // 
            this.SetTweetTimeToolStripMenuItem.Name = "SetTweetTimeToolStripMenuItem";
            this.SetTweetTimeToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.SetTweetTimeToolStripMenuItem.Text = "ツイートする時間の設定";
            this.SetTweetTimeToolStripMenuItem.Click += new System.EventHandler(this.SetTweetTimeToolStripMenuItem_Click);
            // 
            // SetTweetKeyStripMenuItem
            // 
            this.SetTweetKeyStripMenuItem.Name = "SetTweetKeyStripMenuItem";
            this.SetTweetKeyStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.SetTweetKeyStripMenuItem.Text = "ツイートするキーの設定";
            this.SetTweetKeyStripMenuItem.Click += new System.EventHandler(this.SetTweetKeyStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(188, 6);
            // 
            // TweetNowStripMenuItem
            // 
            this.TweetNowStripMenuItem.Name = "TweetNowStripMenuItem";
            this.TweetNowStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.TweetNowStripMenuItem.Text = "今すぐツイートする";
            this.TweetNowStripMenuItem.Click += new System.EventHandler(this.TweetNowStripMenuItem_Click);
            // 
            // NoAutoTweetStripMenuItem
            // 
            this.NoAutoTweetStripMenuItem.Name = "NoAutoTweetStripMenuItem";
            this.NoAutoTweetStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.NoAutoTweetStripMenuItem.Text = "自動的にツイートしない";
            this.NoAutoTweetStripMenuItem.Click += new System.EventHandler(this.NoAutoTweetStripMenuItem_Click);
            // 
            // TweetOnExitToolStripMenuItem
            // 
            this.TweetOnExitToolStripMenuItem.Name = "TweetOnExitToolStripMenuItem";
            this.TweetOnExitToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.TweetOnExitToolStripMenuItem.Text = "終了時にツイートする";
            this.TweetOnExitToolStripMenuItem.Click += new System.EventHandler(this.TweetOnExitToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(188, 6);
            // 
            // ReAuthenticationToolStripMenuItem
            // 
            this.ReAuthenticationToolStripMenuItem.Name = "ReAuthenticationToolStripMenuItem";
            this.ReAuthenticationToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.ReAuthenticationToolStripMenuItem.Text = "Twitterの再認証";
            this.ReAuthenticationToolStripMenuItem.Click += new System.EventHandler(this.ReAuthenticationToolStripMenuItem_Click);
            // 
            // VerInfoToolStripMenuItem
            // 
            this.VerInfoToolStripMenuItem.Name = "VerInfoToolStripMenuItem";
            this.VerInfoToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.VerInfoToolStripMenuItem.Text = "バージョン情報";
            this.VerInfoToolStripMenuItem.Click += new System.EventHandler(this.VerInfoToolStripMenuItem_Click);
            // 
            // ExitToolStripMenuItem
            // 
            this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            this.ExitToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.ExitToolStripMenuItem.Text = "終了する";
            this.ExitToolStripMenuItem.Click += new System.EventHandler(this.ExitStripMenuItem_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            //this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            // http://tt195361.hatenablog.com/entry/2015/06/12/084110
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 21;
            this.dataGridView1.Size = new System.Drawing.Size(784, 561);
            this.dataGridView1.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.dataGridView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TweetKeyPress";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private NotifyIcon notifyIcon1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem ExitToolStripMenuItem;
        private ToolStripMenuItem ShowPressStripMenuItem;
        private DoubleBufferedDataGridView dataGridView1;
        private ToolStripMenuItem ResetStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem SetTweetTimeToolStripMenuItem;
        private ToolStripMenuItem SetTweetKeyStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem TweetNowStripMenuItem;
        private ToolStripMenuItem NoAutoTweetStripMenuItem;
        private ToolStripMenuItem TweetOnExitToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem ReAuthenticationToolStripMenuItem;
        private ToolStripMenuItem VerInfoToolStripMenuItem;
    }
}

