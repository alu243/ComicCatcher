using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Drawing;
using Helpers;
using Utils;
using Models;
using ComicModels;
using System.Threading;

using System.Data;
namespace ComicModels
{
    public class IgnoreComic : Dictionary<string, string>
    {
        public string GetIgnoreComic(string url)
        {
            if (false == this.ContainsKey(url)) return url;
            return this[url];
        }

        public void AddIgnoreComic(string url, string name)
        {
            SQLiteHelper.AddIgnoreComic(url, name);
            if (false == this.ContainsKey(url)) this.Add(url, name);
        }

        public void Load()
        {
            try
            {
                DataTable result = SQLiteHelper.GetIgnoreComic();
                this.Clear();
                foreach (DataRow row in result.Rows)
                {
                    string key = row["ComicUrl"].ToString().Trim();
                    string value = row["ComicName"].ToString().Trim();

                    if (true == String.IsNullOrEmpty(key)) break;
                    if (false == this.ContainsKey(key)) this.Add(key, value);
                }
            }
            catch { }
        }
    }
}
