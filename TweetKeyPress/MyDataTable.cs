using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace TweetKeyPress
{
    class MyDataTable
    {
        private DataSet data_set;
        public DataTable data_table;
        public DataRow data_row;
        private string fileName;

        public MyDataTable()
        {
            data_set = new DataSet("default_set");
            data_table = new DataTable("default_table");
            fileName = "keylog";

            if (File.Exists(fileName + ".xml"))
            {
                // ファイルがあればXMLファイルから逆シリアライズする
                LoadXML();
            }
            else
            {
                // ファイルが無ければ空のテーブルを追加する
                data_set.Tables.Add(data_table);

                // カラムの定義を行う
                // 日付のカラム
                data_table.Columns.Add("Date", Type.GetType("System.String"));
                // キーのカラム
                for (int i = 1; i <= 254; i++)
                {
                    data_table.Columns.Add(((VKeys)i).ToString(), Type.GetType("System.Int32"));
                }

                // 主キーの設定
                data_table.PrimaryKey = new DataColumn[] { data_table.Columns["Date"] };
            }

            // 注目するローを設定
            FindTodaysDataRow();
        }

        private void LoadXML()
        {
            // StreamReaderオブジェクトの生成
            System.IO.StreamReader sr = null;
            // XmlSerializerオブジェクトの作成
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(data_table.GetType());
            try
            {
                sr = new System.IO.StreamReader(fileName + ".xml");
                // 逆シリアライズ
                data_table = (DataTable)serializer.Deserialize(sr);
                // 逆シリアライズしたテーブルを追加する
                data_set.Tables.Add(data_table);
            }
            catch (System.Exception ex)
            {
                throw; // 例外を再スローし、error.txtにログを吐いて終了する。
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
            }
        }

        public void SaveXML()
        {
            // StreamWriterオブジェクトの生成
            System.IO.StreamWriter sw = null;
            // XmlSerializerオブジェクトの作成
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(data_table.GetType());
            try
            {
                sw = new System.IO.StreamWriter(fileName + ".xml");
                // シリアライズ
                serializer.Serialize(sw, data_table);
            }
            catch (System.Exception ex)
            {
                throw; // 例外を再スローし、error.txtにログを吐いて終了する。
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }

        // http://okwakatta.net/code/dst24.html

        public void SaveCSV()
        {
            string sp = string.Empty;
            System.IO.StreamWriter sw = null;
            List<int> filterIndex = new List<int>();

            try
            {
                sw = new System.IO.StreamWriter(fileName + ".csv", false, System.Text.Encoding.GetEncoding("Shift_JIS"));

                //----------------------------------------------------------//
                // DataColumnの型から値を出力するかどうか判別します         //
                // 出力対象外となった項目は[データ]という形で出力します     //
                //----------------------------------------------------------//
                for (int i = 0; i < data_table.Columns.Count; i++)
                {
                    switch (data_table.Columns[i].DataType.ToString())
                    {
                        case "System.Boolean":
                        case "System.Byte":
                        case "System.Char":
                        case "System.DateTime":
                        case "System.Decimal":
                        case "System.Double":
                        case "System.Int16":
                        case "System.Int32":
                        case "System.Int64":
                        case "System.SByte":
                        case "System.Single":
                        case "System.String":
                        case "System.TimeSpan":
                        case "System.UInt16":
                        case "System.UInt32":
                        case "System.UInt64":
                            break;

                        default:
                            filterIndex.Add(i);
                            break;
                    }
                }

                //----------------------------------------------------------//
                // ヘッダーを出力します。                                   //
                //----------------------------------------------------------//
                foreach (DataColumn col in data_table.Columns)
                {
                    // CSVで出力する時は、カラム名をTweetVKeysにしてわかりやすく
                    sw.Write(sp + "\"" + Program.TweetVKeys[col.ToString()].Replace("\"", "\"\"").Replace("\"", "\"\"") + "\"");
                    sp = ",";
                }
                sw.WriteLine();

                //----------------------------------------------------------//
                // 内容を出力します。                                       //
                //----------------------------------------------------------//
                foreach (DataRow row in data_table.Rows)
                {
                    sp = string.Empty;
                    for (int i = 0; i < data_table.Columns.Count; i++)
                    {
                        if (filterIndex.Contains(i))
                        {
                            sw.Write(sp + "\"[データ]\"");
                            sp = ",";
                        }
                        else
                        {
                            sw.Write(sp + "\"" + row[i].ToString().Replace("\"", "\"\"") + "\"");
                            sp = ",";
                        }
                    }
                    sw.WriteLine();
                }
            }
            catch (System.Exception ex)
            {
                throw; // 例外を再スローし、error.txtにログを吐いて終了する。
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }

        public void CountUp(string keyname)
        {
            data_row[keyname] = (int)data_row[keyname] + 1;
        }

        public int getCount(string keyname)
        {
            if (keyname == "Date") return 0;
            else return (int)data_row[keyname];
        }

        //
        // 一番多く押したキー名（マウスのボタン名）を返す関数
        //
        public string[] getMaxValueColumns(DataRow dr)
        {
            var t = new DataTable();
            t.Columns.Add("ColumnName", typeof(string));
            t.Columns.Add("Value", typeof(int));

            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                var r = t.NewRow();
                r["ColumnName"] = dr.Table.Columns[i].ColumnName;
                if (i == 0) r["Value"] = 0;
                else r["Value"] = dr[i];
                t.Rows.Add(r);
            }

            var result = (from x in t.AsEnumerable()
                          where (int)(x["Value"]) == (int)((from y in t.AsEnumerable() select y["Value"]).Max())
                          select (string)(x["ColumnName"])).ToArray();

            return result;
        }

        //
        // 二番目に多く押したキー名（マウスのボタン名）を返す関数
        //
        public string[] getSecondValueColumns(DataRow dr)
        {
            // DataTableをコピーし、一旦全てのローを削除し、今日のローだけを追加する
            DataTable t = dr.Table.Copy();
            t.Rows.Clear();
            DataRow r = t.NewRow();
            r.ItemArray = dr.ItemArray;
            t.Rows.Add(r);

            foreach (string str in getMaxValueColumns(dr))
            {
                // 一番多く押したキー（マウス）のカラムを削除
                t.Columns.Remove(str);
            }

            return getMaxValueColumns(t.Rows[0]);
        }

        public void FindTodaysDataRow()
        {
            // 今日の日付のローが無いか確認
            DataRow foundRow = data_table.Rows.Find(System.DateTime.Today.ToString("yyyy/MM/dd"));

            if (foundRow != null)
            {
                // 今日の日付のローがあったら、注目するローを設定
                data_row = foundRow;
            }
            else
            {
                // なければ、今日の日付のローを作る
                data_row = data_table.NewRow();
                data_row["Date"] = System.DateTime.Today.ToString("yyyy/MM/dd");
                Reset();
                // 作成したローをテーブルへ追加する
                data_table.Rows.Add(data_row);
            }
        }

        public void Reset()
        {
            // 今日の日付のローを0でリセットする
            for (int i = 1; i <= 254; i++)
            {
                data_row[((VKeys)i).ToString()] = 0;
            }
        }
    }
}
