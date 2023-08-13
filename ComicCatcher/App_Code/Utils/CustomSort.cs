using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ComicCatcher.Utils;

public static class MyExtensions
{
    public static IEnumerable<string> CustomSort(this IEnumerable<string> list)
    {
        Regex r = new Regex(@"(\d+)", RegexOptions.Compiled);
        //int maxLen = list.Select(s => s.Length).Max();
        int maxLen = 5;
        //int maxTimes = list.Select(s => r.Matches(s).Count).Max(); // 數字出現幾次

        //r.Matches(list[0]).Cast<Match>().ToList().ForEach(m => m.Value.PadLeft(maxLen, '0')).ToArray().Join();
        return list.Select(s => new
        {
            OrgStr = s,
            //SortStr = Regex.Replace(s, @"(\d+)|(\D+)", m => m.Value.PadLeft(maxLen, char.IsDigit(m.Value[0]) ? ' ' : '\xffff'))
            SortStr = (char.IsDigit(s[0]) ? (s[0] == '第' ? "_" : "#") : Regex.Match(s, @"(\D+)").Value.Trim()) +
                    String.Join("-", r.Matches(s).Cast<Match>().Select(m => m.Value.PadLeft(maxLen, '0')).ToArray()) + s
        })
        .OrderBy(x => x.SortStr)
        .Select(x => x.OrgStr);
    }

}