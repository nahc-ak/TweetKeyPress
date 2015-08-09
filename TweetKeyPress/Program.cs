using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CoreTweet;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Threading;
using System.IO;

namespace TweetKeyPress
{
    static partial class Program
    {
        // CoreTweet
        private static OAuth.OAuthSession session;
        private static Tokens tokens;

        // キーログ
        public static MyDataTable myDataTable;

        // 1秒ごとに呼び出されるタイマー
        private static System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
        // 自動バックアップ
        private static System.Windows.Forms.Timer timer2 = new System.Windows.Forms.Timer();

        // メニュー「終了する」がクリックされるとtrue
        public static bool exiting = false;

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // http://techracho.bpsinc.jp/baba/2009_02_10/164
            // エラーハンドラを登録
            // 例外をキャッチしたらerror.txtに吐いて終了する
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            Thread.GetDomain().UnhandledException += new UnhandledExceptionEventHandler(Program_UnhandledException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new MainForm());

            // 二重起動をチェックする
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                // 同じプロセス名で他にプロセスが起動してたら、終了
                MessageBox.Show("他にTweetKeyPressが起動しているため、終了します。。。");
                return;
            }

            // アプリケーションの設定を読み込む
            Settings.LoadFromXmlFile();

            // Twitterの認証を行う
            if (Settings.FileExists)
            {
                tokens = Tokens.Create(
                    Settings.Instance.ConsumerKey,
                    Settings.Instance.ConsumerSecret,
                    Settings.Instance.AccessToken,
                    Settings.Instance.AccessTokenSecret);
            }
            else
            {
                Auth();
                MessageBox.Show(
                    "認証が完了しました。" + Environment.NewLine +
                    "タスクトレイに常駐開始します。" + Environment.NewLine +
                    "0時になると一番押されたキーやマウスのボタンを回数と共にツイートします。" + Environment.NewLine +
                    "（既定ではプログラムの終了時にもツイートします）");
            }

            // ログファイルの読み込み
            myDataTable = new MyDataTable();

            // メインフォームのインスタンスを生成
            MainForm mainForm = new MainForm();

            // 1秒ごとに呼び出されるタイマー
            timer1.Tick += new EventHandler(MyClock1);
            timer1.Interval = 1000;  // 1秒ごとに呼び出す
            timer1.Enabled = true;   // タイマー開始

            // 自動バックアップ処理
            timer2.Tick += new EventHandler(MyClock2);
            timer2.Interval = 600000;  // 10分ごとに呼び出す
            timer2.Enabled = true;   // タイマー開始

            // フックの開始
            Hook();

            // フォームを非表示のままメッセージループを開始する
            Application.Run();
        }

        //
        // 1秒ごとに呼び出されるタイマー
        //
        public static void MyClock1(object sender, EventArgs e)
        {
            // 指定時刻になったらつぶやく
            if (Settings.Instance.TweetTime.ToString("HH:mm:ss").Equals(DateTime.Now.ToString("HH:mm:ss")))
            {
                if (!Settings.Instance.NoAutoTweet) TweetMaxKeys();
            }
            // 注目するローを設定する
            myDataTable.FindTodaysDataRow();
        }

        //
        // 自動バックアップ処理
        //
        public static void MyClock2(object sender, EventArgs e)
        {
            // キーのカウント数をファイルに保存
            myDataTable.SaveCSV();
            myDataTable.SaveXML();
            // プログラムの設定をXMLファイルに保存する
            Settings.SaveToXmlFile();
        }

        //
        // 一番多く押したキーやマウスのボタンをつぶやく
        //
        public static void TweetMaxKeys()
        {
            // 一旦フックを解除
            Unhook();

            // 一旦キーのカウント数をファイルに保存
            myDataTable.SaveCSV();
            myDataTable.SaveXML();

            // 一番多く押したキーの名前、二番目に多く押したキーの名前
            String[] column1, column2;

            if (Settings.Instance.TweetKeyLimited)
            {
                column1 = new String[] { Settings.Instance.LimitedKey1 };
                column2 = new String[] { Settings.Instance.LimitedKey2 };
            }
            else
            {
                column1 = myDataTable.getMaxValueColumns(myDataTable.data_row);
                if (myDataTable.getCount(column1[0]) == 0) // 一番目に多く押したキーが0回の時→一日中何も押してない時
                {
                    column2 = null;
                    tokens.Statuses.Update(status => "[TweetKeyPress]今日は何も押していません。");
                }
                else
                {
                    column2 = myDataTable.getSecondValueColumns(myDataTable.data_row);
                }
            }

            if (column2 != null)
            {
                // キーの名前を日本語にする
                string[] KeyName1 = new string[column1.Length];
                column1.CopyTo(KeyName1, 0);
                for (int i = 0; i < column1.Length; i++)
                {
                    KeyName1[i] = TweetVKeys[column1[i]];
                }
                string[] KeyName2 = new string[column2.Length];
                column2.CopyTo(KeyName2, 0);
                for (int i = 0; i < column2.Length; i++)
                {
                    KeyName2[i] = TweetVKeys[column2[i]];
                }

                // つぶやく
                if (myDataTable.getCount(column2[0]) != 0 // 二番目に多く押したキーが０じゃ無い時（通常）
                    || Settings.Instance.TweetKeyLimited) // ツイートするキーを制限している時は、二個目のキーが0回であっても強制的につぶやく
                {
                    string tweet = "[TweetKeyPress]今日は" + string.Join("と", KeyName1) + "を" + myDataTable.getCount(column1[0]) + "回、" +
                        string.Join("と", KeyName2) + "を" + myDataTable.getCount(column2[0]) + "回押しました。";
                    tokens.Statuses.Update(status => tweet);
                }
                else
                {
                    // 二番目に多く押したキーが0の時
                    string tweet = "[TweetKeyPress]今日は" + string.Join("と", KeyName1) + "を" + myDataTable.getCount(column1[0]) + "回押しました。";
                    tokens.Statuses.Update(status => tweet);
                }
            }

            // プログラムの終了時でなければ、フックを再開する
            if (!exiting)
            {
                Hook();
            }
        }

        //
        // プログラム終了時の処理
        //
        public static void Exiting()
        {
            // 終了フラグ
            Program.exiting = true;
            // キーのカウント数をファイルに保存
            myDataTable.SaveCSV();
            myDataTable.SaveXML();
            // プログラムの設定をXMLファイルに保存する
            Settings.SaveToXmlFile();
            //「終了時につぶやく」にチェックが入っているなら、つぶやく
            if (Settings.Instance.TweetOnExit) Program.TweetMaxKeys();
        }

        //
        // フックの開始
        //
        public static void Hook()
        {
            _hookID_mouse = SetHook(_proc_mouse);
            _hookID_keyboard = SetHook(_proc_keyboard);
        }

        //
        // フックの解除
        //
        public static void Unhook()
        {
            UnhookWindowsHookEx(_hookID_mouse);
            UnhookWindowsHookEx(_hookID_keyboard);
        }

        //
        // Twitterの認証を行う
        //
        public static void Auth()
        {
            MessageBox.Show("Twitter認証画面を開きます。" + Environment.NewLine + "認証を終えたら、表示された7桁の数字を記入してください。");
            session = OAuth.Authorize(Settings.Instance.ConsumerKey, Settings.Instance.ConsumerSecret);
            var url = session.AuthorizeUri;
            Process.Start(url.ToString()); // Twitter認証画面を開く

            string pin = Interaction.InputBox("7桁の数字を入力してください。", "", "", -1, -1);
            tokens = OAuth.GetTokens(session, pin);

            // 取得したトークン
            Settings.Instance.AccessToken = tokens.AccessToken;
            Settings.Instance.AccessToken_Secret = tokens.AccessTokenSecret;

            // 取得したトークンを保存
            Settings.SaveToXmlFile();
        }

        //
        // エラー処理
        //
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            ShowError(e.Exception, "ThreadException");
        }

        static void Program_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                ShowError(ex, "UnhandledException");
            }
        }

        static void ShowError(Exception ex, string title)
        {
            MessageBox.Show(
                "申し訳ありません。TweetKeyPressでエラーが発生しました。" + Environment.NewLine +
                "TweetKeyPressのフォルダにerror.txtを作成しました。" + Environment.NewLine +
                "error.txtをカーちゃん(@nahc_ak)へお送りください。",
                "TweetKeyPress",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            StreamWriter stream = new StreamWriter("error.txt", true);
            stream.WriteLine(System.DateTime.Now);
            stream.WriteLine("[" + title + "]");
            stream.WriteLine("[message]\r\n" + ex.Message);
            stream.WriteLine("[source]\r\n" + ex.Source);
            stream.WriteLine("[stacktrace]\r\n" + ex.StackTrace);
            stream.WriteLine();
            stream.Close();

            Environment.FailFast("TweetKeyPressでエラーが発生しました。");
        }
    }
}
