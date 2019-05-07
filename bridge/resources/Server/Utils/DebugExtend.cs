using System;
using System.Collections.Generic;

namespace Extend
{
    public static class CDebug
    {
        public static void Debug(params object[] arguments)
        {
            int len = arguments.Length;
            List<string> str = new List<string>(len);

            int i = 0;
            while(i < len)
            {
                str.Add("{" +i+ "}");
                i++;
            }

            Console.WriteLine("["+DateTime.Now.ToString() + "]DEBUG: "+string.Join(" ", str), arguments);
        }
    }
}