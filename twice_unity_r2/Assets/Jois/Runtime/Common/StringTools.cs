using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Jois
{
    public static class StringTools
    {
        public static string DictionaryToString(Dictionary<string, string> dict)
        {
            StringBuilder sb = new StringBuilder(128);
            foreach (var item in dict)
            {
                sb.Append(item.Key + "," + item.Value + ",");
            }

            return sb.ToString();
        }

        public static Dictionary<string, string> StringToDictionary(string str)
        {
            var dict = new Dictionary<string, string>();
            var strList = str.Split(',');
            for (int i = 0; i < strList.Length / 2; i += 2)
            {
                dict.Add(strList[i], strList[i + 1]);
            }

            return dict;
        }
    }
}