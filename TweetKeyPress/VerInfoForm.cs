using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace TweetKeyPress
{
    public partial class VerInfoForm : Form
    {
        public VerInfoForm()
        {
            InitializeComponent();
        }

        private void VerInfoForm_Load(object sender, EventArgs e)
        {
            // http://www.atmarkit.co.jp/fdotnet/dotnettips/282verinfodlg/VerInfoDialog.cs

            // ***** Applicationクラスのプロパティにより取得 *****
            // バージョン名（AssemblyInformationalVersionAttribute属性）を取得
            string appVersion = Application.ProductVersion;
            // 製品名（AssemblyProductAttribute属性）を取得
            string appProductName = Application.ProductName;
            // 会社名（AssemblyCompanyAttribute属性）を取得
            string appCompanyName = Application.CompanyName;

            // ***** アセンブリから直接取得 *****
            Assembly mainAssembly = Assembly.GetEntryAssembly();

            // コピーライト情報を取得
            string appCopyright = "-";
            object[] CopyrightArray =
                mainAssembly.GetCustomAttributes(
                typeof(AssemblyCopyrightAttribute), false);
            if ((CopyrightArray != null) && (CopyrightArray.Length > 0))
            {
                appCopyright =
                    ((AssemblyCopyrightAttribute)CopyrightArray[0]).Copyright;
            }

            // 詳細情報を取得
            string appDescription = "-";
            object[] DescriptionArray =
                mainAssembly.GetCustomAttributes(
                typeof(AssemblyDescriptionAttribute), false);
            if ((DescriptionArray != null) && (DescriptionArray.Length > 0))
            {
                appDescription =
                    ((AssemblyDescriptionAttribute)DescriptionArray[0]).Description;
            }

            // ラベルなどにバージョン情報をセット
            Text = appProductName + " のバージョン情報";
            label1.Text = /* appCompanyName + " " + */ appProductName + Environment.NewLine
                + "Version " + appVersion + Environment.NewLine
                + appCopyright + Environment.NewLine;
            //+ appDescription;
        }
    }
}
