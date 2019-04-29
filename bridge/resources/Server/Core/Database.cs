using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using GTANetworkAPI;
using GTANetworkInternals;
using GTANetworkMethods;
using Main;
using Attributes;
using Logic.Account;
using Utils;

namespace Database
{
    using MySql.Data.MySqlClient;
    using MySql;

    public class CMysqlTable
    {
        public string nameNoPrefix;
        public string name;
        private List<string> columns;
        public CMysqlTable(string name)
        {
            columns = new List<string>();
            this.nameNoPrefix = name;
            this.name = name;
        }

        public void AddColumn(string column)
        {
            columns.Add(column);
        }
        public List<string> GetColumns()
        {
            return columns;
        }
        public string GetColumnsAsString()
        {
            return string.Join(",",columns);
        }
    }

    public class CMysql
    {
        public readonly string strConnection = "server=87.98.236.134;uid=db_42756;password=UuUMYlaU8Pu3;database=db_42756;port=3306;CharSet=utf8;";
        private MySqlConnection pConnection;
        public CSelect select;
        public List<CMysqlTable> tables = new List<CMysqlTable>();

        public long LastInsertedID = 0;
        public string tablePrefix = "";
        private int RecordsAffected = 0;

        public static CMysql initialize()
        {
            CMysql pMysql = new CMysql();
            pMysql.select = new CSelect();

            Type[] typelist = CUtils.GetTypesInNamespace(Assembly.GetExecutingAssembly(), "Database");
            for (int i = 0; i < typelist.Length; i++)
            {
                Type type = typelist[i];
                object[] attributes = type.GetCustomAttributes(false); // @todo Zrobić to lepiej
                if (attributes.Length == 1)
                {
                    MysqlTable table = (MysqlTable)attributes[0];
                    if(table == null)
                    {
                        continue;
                    }

                    CMysqlTable pTable = new CMysqlTable(table.name);

                    FieldInfo[] properties = type.GetFields();

                    foreach (FieldInfo property in properties)
                    {
                        MysqlColumn column = property.GetCustomAttribute<MysqlColumn>();
                        if (column != null)
                        {
                            pTable.AddColumn(column.name);
                        }
                    }
                    pMysql.tables.Add(pTable);
                }
            }

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

        public bool UpdateBlocking(string strQuery, params object[] parametrsList)
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

        public MySqlDataReader RawGet(string strQuery, params object[] parametrsList)
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

        public CMysqlTable GetTableByClass<T>() where T : MysqlRow
        {
            object[] attributes = typeof(T).GetCustomAttributes(false); // @todo Zrobić to lepiej
            if (attributes.Length == 1)
            {
                MysqlTable table = (MysqlTable)attributes[0];
                if (table != null)
                {
                    foreach(CMysqlTable mysqlTable in tables)
                    {
                        if(mysqlTable.nameNoPrefix == table.name)
                        {
                            return mysqlTable;
                        }
                    }
                }
            }
            return null;
        }

        public void GetTableRow<T>(ref T row, string whereQuery, params object[] parametrsList) where T : MysqlRow
        {
            CMysqlTable mysqlTable = GetTableByClass<T>();

            if (mysqlTable == null) return;

            using (MySqlDataReader reader = RawGet("select " + mysqlTable.GetColumnsAsString() + " from " + mysqlTable.name + " where " + whereQuery + " limit 1", parametrsList))
            {
                if(reader.HasRows)
                {
                    Globals.Mysql.select.ReadRow(reader, ref row);
                    row.isResult = true;
                }
            }
            Finish();
        }
        public void GetTableRows<T>(ref List<T> rows, string whereQuery = "", params object[] parametrsList) where T : MysqlRow
        {
            CMysqlTable mysqlTable = GetTableByClass<T>();

            if (mysqlTable == null) return;

            if(whereQuery != "")
            {
                whereQuery = "where " + whereQuery;
            }
            using (MySqlDataReader reader = RawGet("select " + mysqlTable.GetColumnsAsString() + " from " + mysqlTable.name + whereQuery, parametrsList))
            {
                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        T row = (T)Activator.CreateInstance(typeof(T));
                        Globals.Mysql.select.ReadRow(reader, ref row, true);
                        row.isResult = true;
                        rows.Add(row);
                    }
                }
            }
            Finish();
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
        public CSelect(){}

        private void readAccountLicenseResult(MySqlDataReader reader, ref CAccountsLicensesRow accountLicenseResult)
        {
            accountLicenseResult.pid = reader.GetUInt32("pid");
            accountLicenseResult.lid = reader.GetByte("lid");
            if(!reader.IsDBNull(2))
                accountLicenseResult.suspended = reader.GetDateTime("suspended");

            if (!reader.IsDBNull(3))
                accountLicenseResult.suspendedreason = reader.GetString("suspendedreason");

        }
        private void readAccountLicenseResults(MySqlDataReader reader, ref List<CAccountsLicensesRow> accountLicenseResults)
        {
            while (reader.Read())
            {
                CAccountsLicensesRow license = new CAccountsLicensesRow();
                readAccountLicenseResult(reader, ref license);
                accountLicenseResults.Add(license);
            }
        }
        private void readVehicleResults(MySqlDataReader reader, ref CVehiclesRow accountLicenseResults)
        {
            if (reader.Read())
            {
                accountLicenseResults.vid = reader.GetUInt32("vid");
                accountLicenseResults.pid = reader.GetUInt32("pid");
                accountLicenseResults.firstOwner = reader.GetUInt32("firstOwner");
                accountLicenseResults.vehicleHash = (VehicleHash)reader.GetUInt32("hash");
            }
        }

        public CAccountsRow PlayerByUID(uint uid)
        {
            CAccountsRow result = new CAccountsRow();
            using (MySqlDataReader reader = RawGet("select pid,login,pass,email,money,xp,accessory,clothes,health,lastposition from accounts where pid = @p1 limit 1", uid.ToString()))
            {
                if (ReadRow(reader, ref result))
                {
                    result.isResult = true;
                }
            }
            Finish();
            return result;
        }
        public CAccountsRow PlayerByLogin(string login)
        {
            CAccountsRow result = new CAccountsRow();
            using (MySqlDataReader reader = RawGet("select pid,login,pass,email,money,xp,accessory,clothes,health,lastposition from accounts where lower(login) = @p1 limit 1", login.ToLower()))
            {
                if(ReadRow(reader, ref result))
                {
                    result.isResult = true;
                }
            }
            Finish();
            return result;
        }
        public CAccountsRow PlayerByEmail(string email)
        {
            CAccountsRow result = new CAccountsRow();
            using (MySqlDataReader reader = RawGet("select pid,login,pass,email,money,xp,accessory,clothes,health,lastposition from accounts where lower(email) = @p1 limit 1", email.ToLower()))
            {
                if (ReadRow(reader, ref result))
                {
                    result.isResult = true;
                }
            }
            Finish();
            return result;
        }

        public List<CAccountsLicensesRow> GetAccountLicenses(uint pid)
        {
            List<CAccountsLicensesRow> results = new List<CAccountsLicensesRow>();
            using (MySqlDataReader reader = RawGet("select pid,lid,suspended,suspendedreason from accounts_licenses where pid = @p1", pid))
            {
                while(reader.Read())
                {
                    CAccountsLicensesRow result = new CAccountsLicensesRow();
                    ReadRow(reader, ref result, true);
                    results.Add(result);
                }
            }
            Finish();
            return results;
        }

        public CVehiclesRow GetVehicleByUID(uint vid)
        {
            CVehiclesRow result = new CVehiclesRow();
            using (MySqlDataReader reader = RawGet("select vid,vehiclehash,pid,position,rotation,fuel,createdat,firstowner from vehicles where vid = @p1 limit 1", vid))
            {
                ReadRow(reader, ref result);
                if(result.vid != 0)
                {
                    result.isResult = true;
                }
            }
            Finish();
            return result;
        }

        private object ReadValue(MySqlDataReader reader, string columnName, Type columnType, object defaultValue)
        {
            int id = reader.GetOrdinal(columnName);


            if (reader.IsDBNull(id))
            {
                if (columnType == typeof(Vector3))
                {
                    return new Vector3(0, 0, 0);
                }
                return defaultValue;
            }

            if (columnType == typeof(int))
            {
                return reader.GetInt32(id);
            }
            else if (columnType == typeof(float))
            {
                return reader.GetFloat(id);
            }
            else if (columnType == typeof(uint))
            {
                return reader.GetUInt32(id);
            }
            else if (columnType == typeof(ushort))
            {
                return reader.GetUInt16(id);
            }
            else if (columnType == typeof(long))
            {
                return reader.GetInt64(id);
            }
            else if (columnType == typeof(byte))
            {
                return reader.GetByte(id);
            }
            else if (columnType == typeof(string))
            {
                return reader.GetString(id);
            }
            else if (columnType == typeof(DateTime))
            {
                return reader.GetDateTime(id);
            }
            else if (columnType == typeof(VehicleHash))
            {
                return (VehicleHash)reader.GetUInt32(id);
            }
            else if (columnType == typeof(CAccessories))
            {
                CAccessories pAccessories = new CAccessories();
                string strAccessoriers = reader.GetString(id);
                string[] accessories = strAccessoriers.Split(";");
                foreach (string accessory in accessories)
                {
                    string[] accessorySplt = accessory.Split(",");
                    if(accessorySplt.Length == 3)
                    {
                        pAccessories.accessories.Add(new CAccessory
                        {
                            slot = Convert.ToInt32(accessorySplt[0]),
                            drawable = Convert.ToInt32(accessorySplt[1]),
                            texture = Convert.ToInt32(accessorySplt[2])
                        });
                    }
                }
                return pAccessories;
            }
            else if (columnType == typeof(CClothes))
            {
                CClothes pClothes = new CClothes();
                string strClothes = reader.GetString(id);
                string[] clothes = strClothes.Split(";");
                foreach (string clothesSet in clothes)
                {
                    string[] clothesSetSplt = clothesSet.Split(",");
                    if(clothesSetSplt.Length == 3)
                    {
                        pClothes.clothes.Add(new CClothesSet
                        {
                            slot = Convert.ToInt32(clothesSetSplt[0]),
                            drawable = Convert.ToInt32(clothesSetSplt[1]),
                            texture = Convert.ToInt32(clothesSetSplt[2])
                        });
                    }
                }
                return pClothes;
            }
            else if (columnType == typeof(Vector3))
            {
                string strColor4 = reader.GetString(id);
                string[] splt = strColor4.Split(",");
                if (splt.Length == 3)
                {
                    return new Vector3(System.Convert.ToDouble(splt[0]), System.Convert.ToDouble(splt[1]), System.Convert.ToDouble(splt[2]));
                }
                else
                {
                    return new Vector3(0, 0, 0);
                }

            }
            else if (columnType == typeof(Color))
            {
                string strVector3 = reader.GetString(id);
                string[] splt = strVector3.Split(",");
                if (splt.Length == 4)
                {
                    return new Color(System.Convert.ToInt32(splt[0]), System.Convert.ToInt32(splt[1]), System.Convert.ToInt32(splt[2]), System.Convert.ToInt32(splt[3]));
                }
                else
                {
                    return new Color(255,255,255,255);
                }
            }
            else if(columnType == typeof(object))
            {
                return reader.GetValue(id);
            }
            return defaultValue;
        }
        public bool ReadRow<T>(MySqlDataReader reader, ref T row, bool skipRead = false)
        {
            if (!skipRead)
            {
                if (!reader.Read())
                    return false;
            }

            FieldInfo[] properties = row.GetType().GetFields();

            foreach (FieldInfo property in properties)
            {
                MysqlColumn column = property.GetCustomAttribute<MysqlColumn>();
                if (column != null)
                {
                    Type fieldType = property.FieldType;
                    object result = ReadValue(reader, column.name, fieldType, column.defaultValue);
                    if(result != null)
                    {
                        property.SetValue(row, Convert.ChangeType(result, fieldType));
                    }
                }
            }
            return true;
        }
    }

    public class MysqlRow
    {
        public bool isResult = false;
    }

    [MysqlTable("accounts")]
    public class CAccountsRow : MysqlRow
    {

        [MysqlColumn("pid",0)]
        public uint pid;

        [MysqlColumn("login","")]
        public string login;

        [MysqlColumn("pass","")]
        public string pass;

        [MysqlColumn("email","")]
        public string email;

        [MysqlColumn("health", 0)]
        public int health;

        [MysqlColumn("money",0)]
        public long money;

        [MysqlColumn("xp",0)]
        public uint xp;

        [MysqlColumn("accessory", 0)]
        public CAccessories accessory;

        [MysqlColumn("clothes", 0)]
        public CClothes clothes;

        [MysqlColumn("lastposition")]
        public Vector3 lastPosition;
    }

    [MysqlTable("accounts_licenses")]
    public class CAccountsLicensesRow : MysqlRow
    {
        [MysqlColumn("pid", 0)]
        public uint pid;

        [MysqlColumn("lid", 0)]
        public byte lid;

        [MysqlColumn("suspended", null)]
        public DateTime suspended;

        [MysqlColumn("suspendedreason", 0)]
        public string suspendedreason;
    }

    [MysqlTable("vehicles")]
    public class CVehiclesRow : MysqlRow
    {

        [MysqlColumn("vid", 0)]
        public uint vid;

        [MysqlColumn("pid", 0)]
        public uint pid;

        [MysqlColumn("firstowner", 0)]
        public uint firstOwner;

        [MysqlColumn("vehiclehash", 0)]
        public VehicleHash vehicleHash;

        [MysqlColumn("position")]
        public Vector3 position;

        [MysqlColumn("rotation")]
        public Vector3 rotation;

        [MysqlColumn("fuel", 0)]
        public float fuel;

        [MysqlColumn("createdat", 0)]
        public DateTime createdAt;
    }

    [MysqlTable("spawnpoints")]
    public class CSpawnRow : MysqlRow
    {
        [MysqlColumn("position")]
        public Vector3 position;

        [MysqlColumn("rotation", 0)]
        public float rotation;
    }

    [MysqlTable("vehicles_public")]
    public class CVehiclePublicRow : MysqlRow
    {
        [MysqlColumn("vehiclehash", 0)]
        public VehicleHash vehicleHash;

        [MysqlColumn("position")]
        public Vector3 position;

        [MysqlColumn("rotation", 0)]
        public float rotation;
    }

    [MysqlTable("config")]
    public class CConfigRow : MysqlRow
    {
        [MysqlColumn("configkey")]
        public string key;

        [MysqlColumn("value", 0)]
        public object value;
    }

    [MysqlTable("levels")]
    public class CLevelRow : MysqlRow
    {
        [MysqlColumn("level")]
        public ushort level;

        [MysqlColumn("xp", 0)]
        public uint xp;
    }

    [MysqlTable("markers")]
    public class CMarkerRow : MysqlRow
    {
        [MysqlColumn("mid")]
        public uint mid;

        [MysqlColumn("type")]
        public uint type;

        [MysqlColumn("scale")]
        public float scale;

        [MysqlColumn("color")]
        public Color color;

        [MysqlColumn("position")]
        public Vector3 position;

        [MysqlColumn("blip")]
        public int blip;

        [MysqlColumn("action")]
        public int action;
    }

    [MysqlTable("admins")]
    public class CAdminRow : MysqlRow
    {
        [MysqlColumn("pid")]
        public uint pid;

        [MysqlColumn("rank")]
        public byte rank;
    }

    [MysqlTable("admins_ranks")]
    public class CAdminRankRow : MysqlRow
    {
        [MysqlColumn("rank")]
        public byte rank;
        
        [MysqlColumn("rankname")]
        public string rankname;

        [MysqlColumn("color")]
        public int color;

        [MysqlColumn("level")]
        public byte level;
    }

    [MysqlTable("admins_commands")]
    public class CAdminCommandRow : MysqlRow
    {
        [MysqlColumn("command")]
        public string command;

        [MysqlColumn("level")]
        public byte level;
    }
}