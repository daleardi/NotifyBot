using System;

namespace NotifyBot.Utility
{
    public static class Parser
    {
        public static Tuple<string,string> SplitOnFirstWord(string s)
        {
            for (var i = 0; i < s.Length-1; i++)
            {
                if (s[i] == ' ')
                {
                    if (s[i + 1] == ' ') { continue; }
                    return new Tuple<string, string>(s.Substring(0, i + 1), s.Substring(i + 1));
                }
            }
            return new Tuple<string, string>(s, "");
        }
    }
}