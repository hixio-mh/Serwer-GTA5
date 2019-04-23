using System;
using System.Collections.Generic;
using System.Text;

namespace Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class MysqlColumn : Attribute
    {
        public string name;
        public object defaultValue;
        public MysqlColumn(string name, object defaultValue = null)
        {
            this.name = name;
            this.defaultValue = defaultValue;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MysqlTable : Attribute
    {
        public string name;
        public MysqlTable(string name)
        {
            this.name = name;
        }
    }
}
