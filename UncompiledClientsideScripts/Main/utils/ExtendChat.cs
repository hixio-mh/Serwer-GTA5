using System;
using System.Collections.Generic;
using System.Text;
using RAGE;

namespace RAGE
{
    public static class ChatExtend
    {
        public static void Chat(string message, params object[] format)
        {
            RAGE.Chat.Output(string.Format(message, format));
        }
    }
}
