using System;
using System.Runtime;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GTANetworkAPI;
using GTANetworkInternals;
using GTANetworkMethods;
using Main;
using Managers;
using Database;
using Logger;
using Data.Account;
using Extend;
using Newtonsoft.Json.Linq;
using Vehicle = GTANetworkAPI.Vehicle;

namespace Logic.Inventory
{
    public struct SInventoryCapacity
    {
        public ushort x, y;
        public int size { get { return x * y; } }
        public SInventoryCapacity(ushort x, ushort y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class CInventory
    {
        public static List<CInventory> inventoryPool = new List<CInventory>();

        public uint id;
        public List<CItem> items;
        public SInventoryCapacity capacity;

        public static uint FindFreeID()
        {
            if (inventoryPool.Count == 0)
                return 1;

            uint freeID = inventoryPool.Max(inventory => inventory.id);

            return freeID + 1;
        }

        public CInventory(ushort sizeX, ushort sizeY)
        {
            id = FindFreeID();
            CDebug.Debug("nadaj id", id);
            inventoryPool.Add(this);
            items = new List<CItem>();
            capacity = new SInventoryCapacity(0, 0);
            Resize(sizeX, sizeY);
        }

        ~CInventory()
        {
            inventoryPool.Remove(this);
        }

        public string ToJSON()
        {
            JObject jsonObject = new JObject();
            jsonObject["capacity"] = new JArray(capacity.x, capacity.y);
            JArray itemsArray = new JArray();
            items.ForEach(item => itemsArray.Add(item.ToJson()));
            jsonObject["items"] = itemsArray;

            return jsonObject.ToString();
        }

        bool IsPositionInInventory(ushort x, ushort y)
        {
            if (x >= 0 && x <= capacity.x)
                if (y >= 0 && y <= capacity.y)
                    return true;
            return false;
        }

        public CItem GetItemBySlot(ushort x, ushort y)
        {
            if (IsPositionInInventory(x, y))
                return items.Find(item => item.position.x == x && item.position.y == y);
            return null;
        }
        
        public bool IsInThisInventory(CItem item)
        {
            return items.Contains(item);
        }

        public bool IsSlotOccupied(ushort x, ushort y) => GetItemBySlot(x, y) != null;

        public ushort[] FindFreeSlot()
        {
            if (items.Count >= capacity.size)
                return null;

            if (items.Count == 0)
            {
                return new ushort[2] { 0, 0 };
            }

            for (ushort y = 0; y < capacity.y; y++)
            {
                for (ushort x = 0; x < capacity.x; x++)
                {
                    if (!IsSlotOccupied(x, y))
                        return new ushort[] { x, y };
                }
            }

            return null;
        }

        public int FreeSlots
        {
            get { return capacity.size - items.Count; }
        }

        public bool HasFreeSlot
        {
            get { return (capacity.size - items.Count) > 0; }
        }

        public bool SyncItem(CItem item, Client player)
        {
            if (!IsInThisInventory(item)) return false;

            CDebug.Debug("item tojson false: ",item.ToJson(false));
            return true;
        }

        public bool Resize(ushort x, ushort y, Func<CItem> toSmall = null)
        {
            if (capacity.x == x || capacity.x == y) return false;
            capacity.x = x;
            capacity.y = y;
            items.Capacity = x * y;
            return true;
        }

        public bool GiveItem(CItem item, ushort? x, ushort? y)
        {
            if (!HasFreeSlot)
                return false;

            ushort[] freeSlot;
            if (x.HasValue && y.HasValue)
                freeSlot = new ushort[2] { x.Value, y.Value };
            else
                freeSlot = FindFreeSlot();

            item.position.x = freeSlot[0];
            item.position.y = freeSlot[1];
            items.Add(item);
            return true;
        }

        public bool TakeItem(CItem item, bool preventRemove = false)
        {
            if (!IsInThisInventory(item)) return false;

            items.Remove(item);

            if(!preventRemove)
                item = null;

            return true;
        }

        public bool TakeItem(ushort x, ushort y, bool preventRemove = false)
        {
            CItem item = GetItemBySlot(x, y);

            if (!IsInThisInventory(item)) return false;

            items.Remove(item);

            if(!preventRemove)
                item = null;
            return true;
        }

        public CItem GiveItem(uint item, ushort? x = null, ushort? y = null)
        {
            CItem it = Globals.Managers.item.GetItemByID(item);
            if (ReferenceEquals(it, null)) return null;

            if (GiveItem(it, x, y))
            {
                return it;
            }
            return null;
        }

        public bool MoveItem(CItem item, ushort x, ushort y)
        {
            if (!IsInThisInventory(item)) return false;

            if (ReferenceEquals(item, null)) return false;

            if (!IsPositionInInventory(x, y)) return false;

            if (IsSlotOccupied(x, y)) return false;

            item.position.x = x;
            item.position.y = y;
            return true;
        }

        public bool MoveItem(CItem item, CInventory inventory, ushort x, ushort y)
        {
            if (ReferenceEquals(item, null)) return false;

            if (!IsInThisInventory(item)) return false;

            if (ReferenceEquals(inventory, null)) return false;

            if (!inventory.IsPositionInInventory(x, y)) return false;

            if (inventory.IsSlotOccupied(x, y)) return false;

            TakeItem(item, true);
            inventory.GiveItem(item, x, y);

            return true;
        }
    }
}
