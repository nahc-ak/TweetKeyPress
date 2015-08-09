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
    public partial class SetTweetKeyForm : Form
    {
        // コピー
        private Dictionary<string, string> TweetVKeys = new Dictionary<string, string>(Program.TweetVKeys);

        public SetTweetKeyForm()
        {
            InitializeComponent();
            TweetVKeys.Remove("Date");
        }

        private void SetTweetKeyForm_Load(object sender, EventArgs e)
        {
            // チェックボックスやコンボボックスなどの設定
            checkBox1.Checked = Settings.Instance.TweetKeyLimited;
            comboBox1.Enabled = checkBox1.Checked;
            comboBox2.Enabled = checkBox1.Checked;
            comboBox1.DataSource = new BindingSource(TweetVKeys, null);
            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
            comboBox1.SelectedValue = Settings.Instance.LimitedKey1;
            comboBox2.DataSource = new BindingSource(TweetVKeys, null);
            comboBox2.DisplayMember = "Value";
            comboBox2.ValueMember = "Key";
            comboBox2.SelectedValue = Settings.Instance.LimitedKey2;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = checkBox1.Checked;
            comboBox2.Enabled = checkBox1.Checked;
        }

        private void SetTweetKeyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (comboBox1.SelectedValue == comboBox2.SelectedValue)
            {
                MessageBox.Show("一つ目のキーと二つ目のキーを重複させることはできません。",
                    this.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                e.Cancel = true;
                return;
            }
            if (this.DialogResult == DialogResult.Cancel &&
                (!Settings.Instance.LimitedKey1.Equals(comboBox1.SelectedValue.ToString())
                || !Settings.Instance.LimitedKey2.Equals(comboBox2.SelectedValue.ToString())
                || Settings.Instance.TweetKeyLimited != checkBox1.Checked))
            {
                DialogResult result = MessageBox.Show(
                    "設定が変更されていますが、変更を保存しますか？",
                    "TweetKeyPress",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);
                // キャンセルボタン
                if (result == DialogResult.Cancel) e.Cancel = true;
                // はいボタン
                else if (result == DialogResult.Yes) this.DialogResult = DialogResult.OK;
                // いいえボタン
                else this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}
