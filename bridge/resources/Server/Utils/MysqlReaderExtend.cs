using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using MySql;

namespace Extend
{
    public static class CMysqlReaderExtension
    {
        public static bool SetAccount(this MySqlDataReader reader)
        {
            return true;
        }
    }
}
