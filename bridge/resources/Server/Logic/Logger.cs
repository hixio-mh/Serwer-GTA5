using System;
using System.Collections.Generic;
using System.Text;
using Main;

namespace Logger
{
    public static class CLogger
    {
        /// test opis
        public static void LogMoney(uint pid, long previous, long current, string description)
        {
            Globals.Mysql.Update("insert into account_logs_money (pid,previous,current,description)values(@p1,@p2,@p3,@p4)",
                pid.ToString(), previous.ToString(), current.ToString(), description);
        }
    }
}
