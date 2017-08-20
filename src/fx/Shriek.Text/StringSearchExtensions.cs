using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Text
{
    public static class StringSearchExtensions
    {
        static StringSearchExtensions()
        {

        }
        public static bool Contains(this string origin,params string[] keyword)
        {
            var search = new WordsSearch();
            search.SetKeywords(keyword);
            return search.ContainsAny(origin);
        }
    }
}
