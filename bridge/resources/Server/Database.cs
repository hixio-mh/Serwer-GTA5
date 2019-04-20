using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GTANetworkAPI;
using GTANetworkInternals;
using GTANetworkMethods;
using Main;

namespace Database
{
    using MySql.Data.MySqlClient;
    using MySql;

    public class CMysql
    {
        public readonly string strConnection = "server=87.98.236.134;uid=db_42756;password=UuUMYlaU8Pu3;database=db_42756;port=3306;CharSet=utf8;";
        private MySqlConnection pConnection;
        private int RecordsAffected = 0;
        public CSelect select;
        public long LastInsertedID = 0;

        public static CMysql initialize()
        {
            CMysql pMysql = new CMysql();
            pMysql.select = new CSelect();
            return pMysql;
        }
        protected CMysql()
        {
            pConnection = new MySqlConnection(strConnection);
        }

        public void Finish()
        {
            pConnection.Close();
        }
        public MySqlCommand Prepare(string strQuery, Dictionary<string, object> parameters = null)
        {
            if (parameters == null) parameters = new Dictionary<string, object>();
            MySqlCommand command = pConnection.CreateCommand();

            command.CommandText = strQuery;
            foreach (KeyValuePair<string, object> parametr in parameters)
            {
                command.Parameters.AddWithValue(parametr.Key, parametr.Value);
            }
            return command;
        }

        public bool Update(string strQuery, Dictionary<string, object> parameters = null)
        {
            MySqlCommand command = Prepare(strQuery, parameters);
            pConnection.Open();
            command.ExecuteNonQueryAsync();
            pConnection.Close();
            return true;
        }
        public bool Update(string strQuery, params object[] parametrsList)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            for (int i = 0; i < parametrsList.Length; i++)
            {
                parameters.Add("p" + (i + 1), parametrsList[i]);
            }

            MySqlCommand command = Prepare(strQuery, parameters);
            pConnection.Open();
            command.ExecuteNonQueryAsync();
            LastInsertedID = command.LastInsertedId;
            pConnection.Close();
            return true;
        }

        public bool UpdateBlocking(string strQuery, Dictionary<string, object> parameters = null)
        {
            MySqlCommand command = Prepare(strQuery, parameters);
            pConnection.Open();
            command.ExecuteNonQuery();
            LastInsertedID = command.LastInsertedId;
            pConnection.Close();
            return true;
        }

        public bool UpdateBlocking(string strQuery, params string[] parametrsList)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            for (int i = 0; i < parametrsList.Length; i++)
            {
                parameters.Add("p" + (i + 1), parametrsList[i]);
            }

            MySqlCommand command = Prepare(strQuery, parameters);
            pConnection.Open();
            command.ExecuteNonQuery();
            pConnection.Close();
            return true;
        }

        public MySqlDataReader RawGet(string strQuery, Dictionary<string,object> parameters = null)
        {
            MySqlCommand command = Prepare(strQuery, parameters);

            pConnection.Open();
            return command.ExecuteReader();
        }
        public MySqlDataReader RawGet(string strQuery, params string[] parametrsList)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            for (int i = 0; i < parametrsList.Length; i++)
            {
                parameters.Add("p" + (i+1), parametrsList[i]);
            }
            MySqlCommand command = Prepare(strQuery, parameters);

            pConnection.Open();
            return command.ExecuteReader();
        }
        public List<object[]> Get(string strQuery, Dictionary<string,object> parameters = null)
        {
            List<object[]> result;
            using (MySqlDataReader reader = RawGet(strQuery, parameters))
            {
                result = new List<object[]>();
                RecordsAffected = reader.RecordsAffected;
                while (reader.Read())
                {
                    object[] rowResult = new object[reader.FieldCount];
                    reader.GetValues(rowResult);
                    result.Add(rowResult);
                }
            }
            pConnection.Close();
            return result;
        }

        public object GetValue(string strQuery, Dictionary<string,object> parameters = null)
        {
            object result = null;
            using (MySqlDataReader reader = RawGet(strQuery, parameters))
            {
                if(reader.Read())
                {
                    result = reader.GetValue(0);
                }
            }
            pConnection.Close();
            return result;
        }
    }
    public class CSelect : CMysql
    {
        public CSelect()
        {
        }

        private void readPlayerResult(MySqlDataReader reader, ref CPlayersResult CPlayersResult)
        {
            if (reader.Read())
            {
                CPlayersResult.isResult = true;
                CPlayersResult.pid = reader.GetUInt32("pid");
                CPlayersResult.login = reader.GetString("login");
                CPlayersResult.pass = reader.GetString("pass");
                CPlayersResult.email = reader.GetString("email");
                CPlayersResult.money = reader.GetInt64("money");
                CPlayersResult.xp = reader.GetUInt32("xp");
            }
            else
                CPlayersResult.isResult = false;

        }
        public CPlayersResult PlayerByUID(uint uid)
        {
            CPlayersResult result = new CPlayersResult();
            using (MySqlDataReader reader = RawGet("select pid,login,pass,email,money,xp from accounts where pid = @p1 limit 1", uid.ToString()))
            {
                readPlayerResult(reader, ref result);
            }
            Finish();
            return result;
        }
        public CPlayersResult PlayerByLogin(string login)
        {
            CPlayersResult result = new CPlayersResult();
            using (MySqlDataReader reader = RawGet("select pid,login,pass,email,money,xp from accounts where lower(login) = @p1 limit 1", login.ToLower()))
            {
                readPlayerResult(reader, ref result);
            }
            Finish();
            return result;
        }
        public CPlayersResult PlayerByEmail(string email)
        {
            CPlayersResult result = new CPlayersResult();
            using (MySqlDataReader reader = RawGet("select pid,login,pass,email,money,xp from accounts where lower(email) = @p1 limit 1", email.ToLower()))
            {
                readPlayerResult(reader, ref result);
            }
            Finish();
            return result;
        }
    }

    public class CPlayersResult
    {
        public bool isResult;
        public uint pid;
        public string login;
        public string pass;
        public string email;
        public long money;
        public uint xp;
        public CPlayersResult()
        {
            isResult = false;
        }
    }

    public class CPlayersResults : CPlayersResult
    {
        public List<CPlayersResult> results;
    }
}