using System;
using System.Collections.Generic;
using System.Text;
using Database;
using Main;
using GTANetworkAPI;
using Extend;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Linq;
using Utils;
using Logic.Inventory;

namespace Managers
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    class ItemAction : Attribute
    {
        public string description;
        public int id;
        public ItemAction(string description, int id)
        {
            this.id = id;
            this.description = description;
        }
    }

    public struct SItemPosition
    {
        public ushort x, y;
        public SItemPosition(ushort x,ushort y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class CItem
    {
        public uint id;
        public string name;
        public CItemManager.EItemType type;
        public string icon;
        public ushort stack;

        // pozycja w ekwipunku
        public SItemPosition position;
        //public string data;

        public void SyncToClient(Inventory inventory, Client player)
        {
            string data = ToJson(false);
        }

        public void LoadDataFromJson(string jsonData)
        {
            JObject jObject = JObject.Parse(jsonData);
            FieldInfo[] properties = GetType().GetFields();
            JToken token;
            foreach (FieldInfo field in properties)
            {
                if (jObject.TryGetValue(field.Name, out token))
                {
                    try
                    {
                        field.SetValue(this, token.ToObject(field.FieldType));
                    }
                    catch { }
                }
            }
        }

        public string ToJson(bool skipDefault = true)
        {
            CItem original = Globals.Managers.item.GetItemByID(id, true);
            JObject jsonObject = new JObject();

            if (skipDefault)
            {
                jsonObject["id"] = id;
                FieldInfo[] propertiesA = original.GetType().GetFields(); // @todo dodać filtr żeby zwracał tylko pola przedmiotu a nie wszystkie
                FieldInfo[] propertiesB = this.GetType().GetFields();
                object objA, objB;
                foreach (FieldInfo field in propertiesA)
                {
                    objA = field.GetValue(original);
                    objB = field.GetValue(this);
                    if (objA != null && objB != null)
                    {
                        if (!objA.Equals(objB) || field.Name == "position")
                        {
                            jsonObject[field.Name] = JsonConvert.SerializeObject(objB);
                        }
                    }
                }
            }
            else
            {
                FieldInfo[] properties = GetType().GetFields();
                foreach (FieldInfo field in properties)
                    jsonObject[field.Name] = JsonConvert.SerializeObject(field.GetValue(this));
            }
            return jsonObject.ToString(Formatting.None);
        }

        public CItem Copy()
        {
            CItem copy = (CItem)MemberwiseClone();
            return copy;
        }

        public static bool IsSimillar(CItem itemA, CItem itemB)
        {
            if(itemA.GetType() != itemB.GetType())
            {
                return false;
            }

            FieldInfo[] propertiesA = itemA.GetType().GetFields(); // @todo dodać filtr żeby zwracał tylko pola przedmiotu a nie wszystkie
            FieldInfo[] propertiesB = itemB.GetType().GetFields();
            object objA, objB;
            foreach (FieldInfo fieldA in propertiesA)
            {
                foreach (FieldInfo fieldB in propertiesB)
                {
                    if(fieldA.Name == fieldB.Name)
                    {
                        objA = fieldA.GetValue(itemA);
                        objB = fieldB.GetValue(itemB);
                        if(objA != null && objB != null)
                        {
                            if(!objA.Equals(objB))
                            {
                                return false;
                            }
                        }

                        break;
                    }
                }
            }
            return true;
        }

        public Dictionary<int, string> GetActions()
        {
            Dictionary<int, string> actions = new Dictionary<int, string>();

            IEnumerable<MethodInfo> methods = ReflectionHelper.GetMethodsWithAttribute(GetType(), typeof(ItemAction));

            ItemAction action;
            methods.ToList().ForEach(method =>
            {
                action = method.GetCustomAttribute<ItemAction>();
                actions[action.id] = action.description;
            });

            return actions;
        }

        public bool DoAction(Client player, int action)
        {
            IEnumerable<MethodInfo> methods = ReflectionHelper.GetMethodsWithAttribute(GetType(), typeof(ItemAction));

            ItemAction itemAction;
            MethodInfo method = methods.ToList().Find(memberMethod =>
            {
                itemAction = memberMethod.GetCustomAttribute<ItemAction>();
                return itemAction.id == action;
            });
            if(method != null)
            {
                method.Invoke(this,new object[] { player } );
            }
            return false;
        }
    }

    public class CItemTest : CItem
    {
        public int A;
        public string B;

    }

    public class CItemIdCard : CItem
    {
        public string owner;
        public int cardId;

    }
    public class CItemTest2 : CItem
    {
        public int A;
        public string B;

        [ItemAction("zrób coś tym przedmiotem", 1)]
        public bool onDoSomething(Client player)
        {
            Console.WriteLine("gracz coś zrobił1 {0}", player.Name);
            return true;
        }

        [ItemAction("zrób coś inego tym przedmiotem", 2)]
        public bool onDoSomething2(Client player)
        {
            Console.WriteLine("gracz coś zrobił2 {0}", player.Name);
            return true;
        }

        [ItemAction("zrób coś 3 tym przedmiotem", 3)]
        public bool onDoSomething3(Client player)
        {
            Console.WriteLine("gracz coś zrobił3 {0}", player.Name);
            return true;
        }
    }

    public class CItemManager : Manager
    {
        List<CItem> defaultItems = new List<CItem>();

        public enum EItemType : byte
        {
            Test,
            Test2,
            IdCard,
        }

        public void UpdateDefaultItems()
        {
            /*
            defaultItems.Clear();
            List<CItemBaseRow> items = new List<CItemBaseRow>();
            Globals.Mysql.GetTableRows(ref items);
            defaultItems.Capacity = items.Count;
            items.ForEach(item =>
            {
                CItem newItem = CreateItem((EItemType)item.type);
                if (ReferenceEquals(newItem, null))
                {
                    CLogger.TellSomethingImportant($"Przedmiot o id {item.id} ma niewłaściwy typ.");
                }
                else
                {
                    newItem.id = item.id;
                    newItem.name = item.name;
                    newItem.icon = item.icon;
                    newItem.stack = item.stack;
                    newItem.position = new SItemPosition(0, 0);
                    newItem.LoadDataFromJson(item.data);
                    defaultItems.Add(newItem);
                }
            });*/
        }

        public CItem CreateItem(EItemType itemType)
        {
            if(Enum.IsDefined(typeof(EItemType), itemType))
            {
                string itemClass = "Managers.CItem" + itemType.ToString();
                CItem item = (CItem)System.Reflection.Assembly.GetAssembly(typeof(CItem)).CreateInstance(itemClass);
                return item;
            }
            return null;
        }

        public CItem GetItemByID(uint id, bool dontCopy = false)
        {
            CItem item = defaultItems.Single(i => i.id == id);
            if(ReferenceEquals(item, null))
                return null;

            if (dontCopy)
                return item;

            return item.Copy();
        }

        public CItemManager()
        {
            UpdateDefaultItems();
            /*
            string testjson = "{\"A\":5,\"B\":\"asdf\"}";
            CItem it = CreateItem(EItemType.Test);
            it.LoadDataFromJson(testjson);

            string testjson2 = "{\"A\":5,\"B\":\"asdf\"}";
            CItem it2 = CreateItem(EItemType.Test2);
            it2.LoadDataFromJson(testjson2);

            string testjson3 = "{\"A\":6,\"B\":\"asdf\"}";
            CItem it3 = CreateItem(EItemType.Test2);
            it2.LoadDataFromJson(testjson3);

            NAPI.Task.Run(() =>
            {
                Client player = CUtils.GetRandomClient();
                it2.DoAction(player, 2);
                CDebug.Debug("a,b", player.Name, it.GetActions().Serialize(), it2.GetActions().Serialize());
            }, 3000);*/
        }
    }
}
