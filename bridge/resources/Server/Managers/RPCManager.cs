using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Extend;

namespace Managers
{
    public class CRPC { }

    class CRPCTest : CRPC
    {
        public string test1;
        public string test2;
        public int test4;
        public bool OnDone() { return true; } // wykonywane po wczytaniu
    }

    public class CRPCManager
    {
        public CRPCManager()
        {
            /*CRPCTest t1 = new CRPCTest();
            CRPCTest t2 = new CRPCTest();
            CRPCTest t3 = new CRPCTest();
            bool a = ProcessRPC(t1, 3, "ASD", 56);
            bool b = ProcessRPC(t2, "asd", "ASD", 56);
            bool c = ProcessRPC(t3);
            Console.WriteLine("Test a {0} {1}",a,t1.Serialize());
            Console.WriteLine("Test b {0} {1}",b,t2.Serialize());
            Console.WriteLine("Test c {0} {1}",c,t3.Serialize());*/
        }

        public bool ProcessRPC(CRPC cSignal, params object[] parametrs)
        {
            Type type = cSignal.GetType();
            FieldInfo[] properties = cSignal.GetType().GetFields();

            if (parametrs.Length != properties.Length) return false;

            object param = null;
            int i = 0;
            foreach (FieldInfo property in properties)
            {
                if (i <= parametrs.Length)
                {
                    param = parametrs[i];
                    i++;
                }
                else
                {
                    param = null;
                }

                if (param != null)
                {
                    if (property.FieldType == param.GetType())
                    {
                        property.SetValue(cSignal, param);
                    }
                    else
                    {
                        cSignal = null;
                        return false;
                    }
                }
            }

            MethodInfo onDone = cSignal.GetType().GetMethod("OnDone");
            if(onDone != null)
            {
                if(!(bool)onDone.Invoke(cSignal, null))
                {
                    cSignal = null;
                    return false;
                }
            }
            return true;
        }
    }
}