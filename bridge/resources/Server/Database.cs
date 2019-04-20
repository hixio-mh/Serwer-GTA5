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
    using MySql.Data.MySqlClient; // https://www.cryptool.org/trac/CrypTool2/browser/trunk/AppReferences/x64/MySql.Data.dll?rev=2020
    using MySql;

    public class Mysql
    {
        public readonly string strConnection = "server=87.98.236.134;uid=db_42756;password=UuUMYlaU8Pu3;database=db_42756;port=3306;CharSet=utf8;";
        private MySqlConnection pConnection;
        private int RecordsAffected = 0;
        public Select select;

        public static Mysql initialize()
        {
            Mysql pMysql = new Mysql();
            pMysql.select = new Select();
            return pMysql;
        }
        protected Mysql()
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
        public bool UpdateBlocking(string strQuery, Dictionary<string, object> parameters = null)
        {
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
    }
    public class Select : Mysql
    {
        public Select()
        {
        }

        private void readPlayerResult(MySqlDataReader reader, ref PlayersResult playersResult)
        {
            if (reader.Read())
            {
                playersResult.isResult = true;
                playersResult.pid = reader.GetUInt32("pid");
                playersResult.login = reader.GetString("login");
                playersResult.pass = reader.GetString("pass");
                playersResult.email = reader.GetString("email");
                playersResult.email = reader.GetString("email");
            }
            else
                playersResult.isResult = false;

        }
        public PlayersResult PlayerByUID(uint uid)
        {
            PlayersResult result = new PlayersResult();
            using (MySqlDataReader reader = RawGet("select pid,login,pass,email from players where pid = @p1 limit 1", uid.ToString()))
            {
                readPlayerResult(reader, ref result);
            }
            Finish();
            return result;
        }
        public PlayersResult PlayerByLogin(string login)
        {
            PlayersResult result = new PlayersResult();
            using (MySqlDataReader reader = RawGet("select pid,login,pass,email from players where lower(login) = @p1 limit 1", login.ToLower()))
            {
                readPlayerResult(reader, ref result);
            }
            Finish();
            return result;
        }
        public PlayersResult PlayerByEmail(string email)
        {
            PlayersResult result = new PlayersResult();
            using (MySqlDataReader reader = RawGet("select pid,login,pass,email from players where lower(email) = @p1 limit 1", email.ToLower()))
            {
                readPlayerResult(reader, ref result);
            }
            Finish();
            return result;
        }
    }

    public class PlayersResult
    {
        public bool isResult;
        public uint pid;
        public string login;
        public string pass;
        public string email;
        public PlayersResult()
        {

        }
    }

    public class PlayersResults : PlayersResult
    {
        public List<PlayersResult> results;
    }
}