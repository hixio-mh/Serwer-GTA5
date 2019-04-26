using System;
using System.Collections.Generic;
using System.Text;
using Database;
using Main;
using Newtonsoft.Json;
using Extend;
using Newtonsoft.Json.Linq;

namespace Main
{
    public class CConfig
    {
        Dictionary<string, object> config = new Dictionary<string, object>();

        bool TryToDouble(object obj, ref double result)
        {
            try
            {
                result = Convert.ToDouble(obj);
                return true;
            }
            catch(FormatException e)
            {
                return false;
            }
        }
        public void Update()
        {
            List<CConfigRow> rows = new List<CConfigRow>();
            Globals.Mysql.GetTableRows(ref rows);
            config.Clear();
            foreach(CConfigRow configRow in rows)
            {
                string str = configRow.value.ToString();
                if (str == "true")
                {
                    config.Add(configRow.key, true);
                    continue;
                }
                else if (str == "false")
                {
                    config.Add(configRow.key, false);
                    continue;
                }

                double result = 0;
                if(TryToDouble(configRow.value, ref result))
                {
                    int i = Convert.ToInt32(result);
                    if (i == result)
                    {
                        config.Add(configRow.key, i);
                    }
                    else
                    {
                        config.Add(configRow.key, result);
                    }
                }
                else
                {
                    string valueStr = configRow.value.ToString();
                    object obj = default;
                    bool isValidObject = valueStr.Deserialize(out obj);
                    if(isValidObject)
                    {
                        config.Add(configRow.key, obj);
                    }
                    else
                        config.Add(configRow.key, valueStr);
                }
            }
        }

        public CConfig()
        {
            Update();
        }

        public object GetConfig(string key, object defaultValue = null)
        {
            object value;
            if(config.TryGetValue(key, out value))
            {
                return value;
            }
            return defaultValue;
        }

        public object this[string key]
        {
            get
            {
                return GetConfig(key);
            }
        }
    }
}
