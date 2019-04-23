using System;
using System.Collections.Generic;
using System.Text;
using Utils;
using Database;

namespace Extend
{
    public static class CList
    {
        public static T GetRandom<T>(this List<T> obj)
        {
            return obj[CUtils.Random.Next(obj.Count)];
        }
    }
}