using System;
using System.Collections.Generic;
using System.Text;
using RAGE;
using System.Reflection;
using Main;
using Extend;
using Newtonsoft.Json.Linq;

namespace Manager
{
    public class CRPC { }

    public class CRPCPlayerUpdateExp : CRPC
    {
        public int xp;
        public int level;
        public bool OnDone() { return true; } // wykonywane po wczytaniu
    }

    public class CRPCExamQuestionsCallback : CRPC
    {
        public JArray questions;
        public bool OnDone() { return true; }
    }


    public class CRPCManager
    {
        public enum ERPCs
        {
            PLAYER_UPDATE_EXP,
            EXAMS_QUESTIONS,
            EXAMS_QUESTIONS_CALLBACK,
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

        void OnPlayerUpdateEXP(CRPCPlayerUpdateExp obj)
        {
            Globals.localPlayer.SetData("xp", obj.xp);
            Globals.localPlayer.SetData("level", obj.level);
        }

        void OnExamQuestionsCallback(CRPCExamQuestionsCallback obj) => Globals.Systems.exams.OnExamQuestionsCallback(obj);

        public CRPCManager()
        {
            RAGE.Events.Add(ERPCs.PLAYER_UPDATE_EXP.ToString(), (object[] obj) => {
                CRPCPlayerUpdateExp playerUpdateEXP = new CRPCPlayerUpdateExp();
                if (ProcessRPC(playerUpdateEXP, obj))
                    OnPlayerUpdateEXP(playerUpdateEXP);
            });

            RAGE.Events.Add(ERPCs.EXAMS_QUESTIONS_CALLBACK.ToString(), (object[] obj) => {
                CRPCExamQuestionsCallback examQuestionsCallback = new CRPCExamQuestionsCallback();
                if (ProcessRPC(examQuestionsCallback, obj))
                    OnExamQuestionsCallback(examQuestionsCallback);
            });

        }

        public void TriggerServer(ERPCs eRPC, params object[] arguments )
        {
            int len = arguments.Length;
            if (len == 1) // @todo zrefaktoryzować
                Events.CallRemote("onClientEvent", eRPC, arguments[0]);
            else if (len == 2)
                Events.CallRemote("onClientEvent", eRPC, arguments[0], arguments[1]);
            else if (len == 3)
                Events.CallRemote("onClientEvent", eRPC, arguments[0], arguments[1], arguments[2]);
            else if (len == 4)
                Events.CallRemote("onClientEvent", eRPC, arguments[0], arguments[1], arguments[2], arguments[3]);
            else if (len == 5)
                Events.CallRemote("onClientEvent", eRPC, arguments[0], arguments[1], arguments[2], arguments[3], arguments[4]);
            else if (len == 6)
                Events.CallRemote("onClientEvent", eRPC, arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5]);

        }
    }
}
