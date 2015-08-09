using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TweetKeyPress
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            // 自動的にツイートするかどうかの設定を読み込み、反映させる。
            NoAutoTweetStripMenuItem.Checked = Settings.Instance.NoAutoTweet;
            // 終了時にツイートするかどうかの設定を読み込み、反映させる。
            TweetOnExitToolStripMenuItem.Checked = Settings.Instance.TweetOnExit;
            // DataGridViewにデータソースを指定
            dataGridView1.DataSource = Program.myDataTable.data_table;
            // カラム名を英語から日本語にする処理
            dataGridView1.DataBindingComplete += dataGridView1_DataBindingComplete;
        }

        //
        // 終了するメニュー
        //
        private void ExitStripMenuItem_Click(object sender, EventArgs e)
        {
            // 終了時の処理
            Program.Exiting();
            // メインフォームを破棄する
            this.Dispose();
            // 終了
            Application.Exit();
        }

        //
        // 押した回数の一覧を見るメニュー
        //
        private void ShowPressStripMenuItem1_Click(object sender, EventArgs e)
        {
            // フォームを表示
            this.Show();
            // 最小化されていたら通常のサイズに戻す
            if (this.WindowState == FormWindowState.Minimized) this.WindowState = FormWindowState.Normal;
            // フォームをアクティブにする
            this.Activate();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // フォームを破棄しないで隠す
            e.Cancel = true;
            this.Hide();
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                // ヘッダーをTweetVKeysの表記に差し替える
                column.HeaderText = Program.TweetVKeys[((DataTable)dataGridView1.DataSource).Columns[column.Index].Caption];
            }
        }

        private void ResetStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                    "今日の押した回数を0にリセットしますか？",
                    "TweetKeyPress",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Program.myDataTable.Reset();
            }
        }

        private void SetTweetTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTweetTimeForm setTweetTimeForm = new SetTweetTimeForm();
            setTweetTimeForm.ShowDialog(this);

            //DialogResultプロパティを調べる
            if (setTweetTimeForm.DialogResult == DialogResult.OK)
            {
                Settings.Instance.TweetTime = setTweetTimeForm.dateTimePicker1.Value;
            }

            setTweetTimeForm.Dispose();
            Settings.SaveToXmlFile();
        }

        private void SetTweetKeyStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTweetKeyForm setTweetKeyForm = new SetTweetKeyForm();
            setTweetKeyForm.ShowDialog(this);

            if (setTweetKeyForm.DialogResult == DialogResult.OK)
            {
                Settings.Instance.TweetKeyLimited = setTweetKeyForm.checkBox1.Checked;
                Settings.Instance.LimitedKey1 = setTweetKeyForm.comboBox1.SelectedValue.ToString();
                Settings.Instance.LimitedKey2 = setTweetKeyForm.comboBox2.SelectedValue.ToString();
            }

            setTweetKeyForm.Dispose();
            Settings.SaveToXmlFile();
        }

        private void TweetNowStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.TweetMaxKeys();
        }

        private void NoAutoTweetStripMenuItem_Click(object sender, EventArgs e)
        {
            // チェックを反転
            NoAutoTweetStripMenuItem.Checked = !NoAutoTweetStripMenuItem.Checked;
            // チェックの状態を設定に反映させる
            Settings.Instance.NoAutoTweet = NoAutoTweetStripMenuItem.Checked;
            // 設定を保存
            Settings.SaveToXmlFile();
        }

        private void TweetOnExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // チェックを反転
            TweetOnExitToolStripMenuItem.Checked = !TweetOnExitToolStripMenuItem.Checked;
            // チェックの状態を設定に反映させる
            Settings.Instance.TweetOnExit = TweetOnExitToolStripMenuItem.Checked;
            // 設定を保存
            Settings.SaveToXmlFile();
        }

        private void ReAuthenticationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 一旦フックを解除
            Program.Unhook();
            // 認証
            Program.Auth();
            // フックを再開
            Program.Hook();

            MessageBox.Show("再認証が完了しました。");
        }

        private void VerInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VerInfoForm verInfoForm = new VerInfoForm();
            verInfoForm.ShowDialog(this);
            verInfoForm.Dispose();
        }
    }

    class DoubleBufferedDataGridView : DataGridView
    {
        public DoubleBufferedDataGridView()
        {
            // ダブルバッファを有効にしたDataGridView
            DoubleBuffered = true;
        }
    }
}
