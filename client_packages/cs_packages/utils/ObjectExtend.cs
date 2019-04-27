using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Extend
{
    public static class CObject
    {
        public static string Serialize(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
