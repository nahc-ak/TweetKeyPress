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
    public partial class SetTweetTimeForm : Form
    {
        private bool Modified = false;
        private DateTime d1, d2;

        public SetTweetTimeForm()
        {
            InitializeComponent();
            d1 = d2 = Settings.Instance.TweetTime;
            dateTimePicker1.Value = Settings.Instance.TweetTime;
        }

        private void SetTweetTimeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ツイートする時間が変更されたが、キャンセルボタンが押された時の問い合わせ
            if (Modified && this.DialogResult == DialogResult.Cancel && !d1.ToString("HH:mm:ss").Equals(d2.ToString("HH:mm:ss")))
            {
                DialogResult result = MessageBox.Show(
                    "ツイートする時間が変更されていますが、変更を保存しますか？",
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

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            Modified = true;
            d2 = dateTimePicker1.Value;
        }
    }
}
