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
    public class PathGroup : Dictionary<string, string>
    {
        public string GetGroupName(string cName)
        {
            if (false == this.ContainsKey(cName)) return cName;
            return this[cName];
        }

        public void Load()
        {
            try
            {
                DataTable result = SQLiteHelper.GetPathGroup();
                this.Clear();
                foreach (DataRow row in result.Rows)
                {
                    string groupName = row["GroupName"].ToString().Trim();
                    for (int i = 1; i <= 10; i++)
                    {
                        string key = row["ComicName" + i.ToString()].ToString().Trim();
                        if (true == String.IsNullOrEmpty(key)) break;
                        if (false == this.ContainsKey(key)) this.Add(key, groupName);
                    }
                }
            }
            catch { }
        }
    }
}
