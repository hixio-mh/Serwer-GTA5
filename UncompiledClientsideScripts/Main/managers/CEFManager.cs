using RAGE;
using System;
using System.Reflection;
using CEF = RAGE.Ui.HtmlWindow;
using Extend;
using System.Text.RegularExpressions;

namespace Manager
{
    public class CCEFEvent
    {
    }
    public class COnError : CCEFEvent
    {
        public string message;
        public string source;
        public int line;
        public int colno;
        public int error;
    }
    public class CTest : CCEFEvent
    {
        public string bla1;
        public string bla2;
        public string bla3;
    }

    public class CPrint : CCEFEvent
    {
        public string text;
    }
    public class CCEFManager
    {
        CEF browser;
        public CCEFManager()
        {
            browser = new CEF("package://html/index.html");
            browser.Active = true;

            RAGE.Events.Add("CEF->C#", OnCEFEvent);
        }

        bool ProcessCEF(CCEFEvent cSignal, params object[] parametrs)
        {
            Type type = cSignal.GetType();
            FieldInfo[] properties = cSignal.GetType().GetFields();

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

                if (param != null)
                {
                    //ChatExtend.Chat("cmp {0} {1}", property.FieldType, param.GetType());
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

        void OnCEFEventTest(CTest obj)
        {
            ChatExtend.Chat("XD 1 1={0} 2={1} 3={2}", obj.bla1, obj.bla2, obj.bla3);
        }

        void OnCEFEventOnError(COnError obj)
        {
            ChatExtend.Chat("{0} {1} {2}:{3} {4}", obj.message, obj.source, obj.line, obj.colno, obj.error);
        }
        
        void OnCEFEventPrint(CPrint obj)
        {
            ChatExtend.Chat("print {0}", obj.text);
        }

        void OnCEFEvent(object[] obj)
        {
            string eventName = obj[0].ToString();
            if(eventName != null)
            {
                switch(eventName)
                {
                    case "onError":
                        COnError onError = new COnError();
                        if (ProcessCEF(onError, obj))
                            OnCEFEventOnError(onError);
                        break;
                    case "print":
                        CPrint print = new CPrint();
                        if (ProcessCEF(print, obj))
                            OnCEFEventPrint(print);
                        break;
                    case "test":
                        CTest test = new CTest();
                        CallCEF("testowa", "pierwszy", "drugi", "trzeci", "czwarty");
                        if (ProcessCEF(test, obj))
                            OnCEFEventTest(test);
                        break;
                }
            }
        }

        public void CallCEF(params object[] parametrs)
        {
            string str = parametrs.Serialize();
            browser.ExecuteJs(string.Format("onCSharp(\"{0}\")",str.ToBase64()));
        }
    }
}
