using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace TweetKeyPress
{
    [Serializable()]
    public class Settings
    {
        // 設定を保存するフィールド
        // Twitter関連
        private string _consumerKey;
        private string _consumerSecret;
        private string _accessToken;
        private string _accessTokenSecret;
        // 自動的につぶやかない
        private bool _noAutoTweet;
        // 終了時につぶやく設定 true->つぶやく, false->つぶやかない
        private bool _tweetOnExit;
        // ツイートする時間
        private DateTime _tweetTime;
        // ツイートするキーを制限する
        private bool _tweetKeyLimited;
        // 制限された場合のツイートするキー1個目
        private string _limitedKey1;
        // 制限された場合のツイートするキー2個目
        private string _limitedKey2;

        // プロパティ
        public string ConsumerKey
        {
            get { return _consumerKey; }
        }
        public string ConsumerSecret
        {
            get { return _consumerSecret; }
        }
        public string AccessToken
        {
            get { return _accessToken; }
            set { _accessToken = value; }
        }
        public string AccessTokenSecret
        {
            get { return _accessTokenSecret; }
            set { _accessTokenSecret = value; }
        }
        public string AccessToken_Secret // TweetKeyPress 1.2以前の対策
        {
            get { return _accessTokenSecret; }
            set { _accessTokenSecret = value; }
        }
        public bool NoAutoTweet
        {
            get { return _noAutoTweet; }
            set { _noAutoTweet = value; }
        }
        public bool TweetOnExit
        {
            get { return _tweetOnExit; }
            set { _tweetOnExit = value; }
        }
        public DateTime TweetTime
        {
            get { return _tweetTime; }
            set { _tweetTime = value; }
        }
        public bool TweetKeyLimited
        {
            get { return _tweetKeyLimited; }
            set { _tweetKeyLimited = value; }
        }
        public string LimitedKey1
        {
            get { return _limitedKey1; }
            set { _limitedKey1 = value; }
        }
        public string LimitedKey2
        {
            get { return _limitedKey2; }
            set { _limitedKey2 = value; }
        }

        // コンストラクタ
        public Settings()
        {
            _consumerKey = "6bHXvpksQgroDJ4mKY5u7w";
            _consumerSecret = "mQDekmYVCdyg9k4oQo6vCItCZTwPqiRqmZxmlcRmw";
            _accessToken = "";
            _accessTokenSecret = "";
            _noAutoTweet = false;
            _tweetOnExit = true;
            _tweetTime = DateTime.Today;
            _tweetKeyLimited = false;
            _limitedKey1 = "Left_MouseButton";
            _limitedKey2 = "Enter";
            _fileExists = false;
        }

        // Settingsクラスのただ一つのインスタンス
        [NonSerialized()]
        private static Settings _instance;
        [System.Xml.Serialization.XmlIgnore]
        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Settings();
                return _instance;
            }
            set { _instance = value; }
        }
        // 設定ファイルの有無
        [NonSerialized()]
        private static bool _fileExists;
        [System.Xml.Serialization.XmlIgnore]
        public static bool FileExists
        {
            get
            {
                return _fileExists;
            }
        }

        /// <summary>
        /// 設定をXMLファイルから読み込み復元する
        /// </summary>
        public static void LoadFromXmlFile()
        {
            string path = GetSettingPath();
            object obj;

            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path,
                    FileMode.Open,
                    FileAccess.Read);
                System.Xml.Serialization.XmlSerializer xs =
                    new System.Xml.Serialization.XmlSerializer(
                        typeof(Settings));
                // 読み込んで逆シリアル化する
                obj = xs.Deserialize(fs);
                fs.Close();
                _fileExists = true;
            }
            else
            {
                obj = null;
                _fileExists = false;
            }

            Instance = (Settings)obj;
        }

        /// <summary>
        /// 現在の設定をXMLファイルに保存する
        /// </summary>
        public static void SaveToXmlFile()
        {
            string path = GetSettingPath();
            string toPath = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData),
                Application.CompanyName + "\\" + Application.ProductName + "\\");

            if (!Directory.Exists(toPath))
            {
                Directory.CreateDirectory(toPath);
            }

            FileStream fs = new FileStream(path,
                FileMode.Create,
                FileAccess.Write);
            System.Xml.Serialization.XmlSerializer xs =
                new System.Xml.Serialization.XmlSerializer(
                typeof(Settings));
            // シリアル化して書き込む
            xs.Serialize(fs, Instance);
            fs.Close();
        }


        private static string GetSettingPath()
        {
            string path = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData),
                Application.CompanyName + "\\" + Application.ProductName +
                "\\" + Application.ProductName + ".config");
            return path;
        }

    }
}
