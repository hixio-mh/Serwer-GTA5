using System;
using System.Collections.Generic;
using System.Text;
using RAGE;
using System.Reflection;
using Main;

namespace Manager
{
    public class CRPC { }

    class CRPCPlayerUpdateExp : CRPC
    {
        public int xp;
        public int level;
        public bool OnDone() { return true; } // wykonywane po wczytaniu
    }


    public class CRPCManager
    {
        enum ERPCs
        {
            PLAYER_UPDATE_EXP,
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

        public CRPCManager()
        {
            RAGE.Events.Add(ERPCs.PLAYER_UPDATE_EXP.ToString(), (object[] obj) => {
                CRPCPlayerUpdateExp playerUpdateEXP = new CRPCPlayerUpdateExp();
                if (ProcessRPC(playerUpdateEXP, obj))
                    OnPlayerUpdateEXP(playerUpdateEXP);
            });

        }

        
    }
}
