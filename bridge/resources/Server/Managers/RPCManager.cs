using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Extend;
using GTANetworkAPI;
using GTANetworkInternals;
using GTANetworkMethods;
using Main;

namespace Managers
{
    public class CRPC { }

    class CRPCExamsQuestion : CRPC
    {
        public int examID;
        public bool OnDone() { return true; }
    }
    class CRPCTest : CRPC
    {
        public string test1;
        public string test2;
        public int test4;
        public bool OnDone() { return true; } // wykonywane po wczytaniu
    }

    public class CRPCManager : Script
    {
        public enum ERPCs
        {
            PLAYER_UPDATE_EXP,
            EXAMS_QUESTIONS,
            EXAMS_QUESTIONS_CALLBACK,
        }

        Dictionary<ERPCs, Func<CRPC, object>> RPCEndPoints = new Dictionary<ERPCs, Func<CRPC, object>>();

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

        [RemoteEvent("onClientEvent")]
        public void onClientEvent(Client player, params object[] arguments)
        {
            if (arguments.Length == 0) return;
            ERPCs rpc = (ERPCs)arguments[0];
            switch(rpc)
            {
                case ERPCs.EXAMS_QUESTIONS:
                    CRPCExamsQuestion examsQuestions = new CRPCExamsQuestion();
                    if (ProcessRPC(examsQuestions, arguments))
                        OnExamsQuestions(player, examsQuestions);
                    break;
            }
        }


        bool ProcessRPC(CRPC cSignal, params object[] parametrs)
        {
            Type type = cSignal.GetType();
            FieldInfo[] properties = cSignal.GetType().GetFields();

            //Console.WriteLine("cmp len {0} {1}", parametrs.Length, properties.Length);
            if (parametrs.Length != properties.Length + 1) return false;

            object param = null;
            int i = 1;
            foreach (FieldInfo property in properties)
            {

                if (i <= parametrs.Length + 1)
                {
                    param = parametrs[i];
                    i++;
                }
                else
                {
                    param = null;
                }

                //Console.WriteLine("param {0}", param);
                if (param != null)
                {
                    //Console.WriteLine("cmp {0} {1}", property.FieldType, param.GetType());
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
            if (onDone != null)
            {
                if (!(bool)onDone.Invoke(cSignal, null))
                {
                    cSignal = null;
                    return false;
                }
            }
            return true;
        }

        void OnExamsQuestions(Client player, CRPCExamsQuestion rpc)
        {
        //    player.TriggerClient(ERPCs.EXAMS_QUESTIONS_CALLBACK, Globals.Systems.exams.GetExamQuestions((byte)rpc.examID));
        }
    }
}