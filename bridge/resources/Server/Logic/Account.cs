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
using Logic.Inventory;
using Extend;
using Vehicle = GTANetworkAPI.Vehicle;

namespace Logic.Account
{
    public class CLicense
    {
        public byte id;
        public DateTime suspended;
        public string reason;
        public bool isSuspended()
        {
            if (suspended != null && suspended > DateTime.Now)
            {
                return true;
            }
            return false;
        }
    }
    public class CAccessory
    {
        public int slot;
        public int drawable;
        public int texture;
    }
    public class CAccessories
    {
        public List<CAccessory> accessories;

        public CAccessories()
        {
            accessories = new List<CAccessory>();
        }

        public void RebuildPlayer(Client player)
        {
            for(int i=0;i<20;i++)
                player.ClearAccessory(i);

            foreach(CAccessory accessory in accessories)
            {
                player.SetAccessories(accessory.slot, accessory.drawable, accessory.texture);
            }
        }

        public void AddAccessory(int slot, int drawable, int texture)
        {
            accessories.RemoveAll(a => a.slot == slot);
            accessories.Add(new CAccessory
            {
                slot = slot,
                drawable = drawable,
                texture = texture,
            });
        }

        public void RemoveAccessory(int slot)
        {
            accessories.RemoveAll(a => a.slot == slot);
        }
        public string GetSaveString()
        {
            List<string> accessoryAsStr = new List<string>();
            foreach (CAccessory accessory in accessories)
            {
                accessoryAsStr.Add($"{accessory.slot},{accessory.drawable},{accessory.texture}");
            }
            if (accessoryAsStr.Count == 0)
            {
                return "";
            }
            else
            {
                return String.Join(";", accessoryAsStr);
            }
        }

    }

    public class CClothesSet
    {
        public int slot;
        public int drawable;
        public int texture;
    }
    public class CClothes
    {
        public List<CClothesSet> clothes;

        public CClothes()
        {
            clothes = new List<CClothesSet>();
        }

        public void RebuildPlayer(Client player)
        {
            player.SetDefaultClothes();

            for (int i=0;i<20;i++)
                player.ClearAccessory(i);

            foreach(CClothesSet clothesSet in clothes)
            {
                player.SetClothes(clothesSet.slot, clothesSet.drawable, clothesSet.texture);
            }
        }

        public void AddClothes(int slot, int drawable, int texture)
        {
            clothes.RemoveAll(a => a.slot == slot);
            clothes.Add(new CClothesSet
            {
                slot = slot,
                drawable = drawable,
                texture = texture,
            });
        }

        public void RemoveClothes(int slot)
        {
            clothes.RemoveAll(a => a.slot == slot);
        }

        public string GetSaveString()
        {
            List<string> setsAsStr = new List<string>();
            foreach(CClothesSet clothesSet in clothes)
            {
                setsAsStr.Add($"{clothesSet.slot},{clothesSet.drawable},{clothesSet.texture}");
            }
            if (setsAsStr.Count == 0)
            {
                return "";
            }
            else
            {
                return String.Join(";", setsAsStr);
            }
        }

    }

    public class CAccount
    {

        public readonly uint pid;
        public readonly string login;
        public readonly string email;
        public long money;
        public uint xp;
        public ushort level;

        public Client player;
        private bool licensesUpdatedFromDB = false;
        public List<CLicense> licenses = new List<CLicense>();
        public CAccessories accessories = new CAccessories();
        public CClothes clothes = new CClothes();
        public Vector3 lastPosition = null;
        public CInventory inventory;

        public void SetXP(uint xp)
        {
            this.xp = xp;
            level = Globals.Managers.account.GetLevelFromXP(xp);
            player.TriggerClient(CRPCManager.ERPCs.PLAYER_UPDATE_EXP, xp, level);
        }

        public void AddXP(uint xp)
        {
            SetXP(this.xp + xp);
        }

        public void SetPlayer(Client player)
        {
            player.AssignUID(pid);
            accessories.RebuildPlayer(player);
            clothes.RebuildPlayer(player);

            this.player = player;

            SetXP(xp);
        }

        public void CleanUp()
        {

        }

        public void UpdateLicensesFromDB(bool force = false)
        {
            if (force || !licensesUpdatedFromDB)
            {
                licensesUpdatedFromDB = true;
                licenses.Clear();
                List<CAccountsLicensesRow> dbResult = Globals.Mysql.select.GetAccountLicenses(pid);
                foreach(CAccountsLicensesRow result in dbResult)
                {
                    licenses.Add(new CLicense {
                        id = result.lid,
                        suspended = result.suspended,
                        reason = result.suspendedreason,
                    });
                }
            }
        }

        public CLicense GetLicense(byte lid)
        {
            if(HasLicense(lid))
            {
                CLicense license = licenses.Find(delegate (CLicense i) { return i.id == lid; });
                if (license == null)
                {
                    return null;
                }
                return license;
            }
            return null;
        }
        public bool HasLicense(byte lid, bool checkIfIsSuspended = false)
        {
            UpdateLicensesFromDB();
            CLicense license = licenses.Find(i => i.id == lid);
            if(license == null)
            {
                return false;
            }
            
            if(checkIfIsSuspended)
            {
                if(license.isSuspended())
                {
                    return false;
                }
            }
            return true;
        }

        public bool GiveLicense(byte lid)
        {
            if(HasLicense(lid))
            {
                return false;
            }
            Globals.Mysql.UpdateBlocking("insert into accounts_licenses (pid,lid)values(@p1,@p2)", pid, lid);
            UpdateLicensesFromDB(true);
            return true;
        }

        public List<Vehicle> GetPrivateVehicles()
        {
            List<Vehicle> playerVehicles = new List<Vehicle>();
            foreach (Vehicle vehicle in Globals.Managers.vehicle.vehicles[EVehicleType.PRIVATE])
            {
                if (vehicle.Owner() == player)
                {
                    playerVehicles.Add(vehicle);
                }
            }
            return playerVehicles;
        }

        public CAccount(uint pid)
        {
            CAccountsRow result = Globals.Mysql.select.PlayerByUID(pid);

            this.pid = result.pid;
            login = result.login;
            email = result.email;
            money = result.money;
            xp = result.xp;
            accessories = result.accessory;
            clothes = result.clothes;
            if(result.lastPosition.Length() > 1)
            {
                lastPosition = result.lastPosition;
            }

            inventory = new CInventory(3, 3);
            Globals.Managers.account.setAccountUsed(pid, true);
        }

        ~CAccount()
        {
            Globals.Managers.account.setAccountUsed(pid, false);
        }

        public void GiveMoney(long amount, string description)
        {
            CLogger.LogMoney(pid, money, money + amount, description);
            money += amount;
            Save(CAccountData.ESave.MONEY);
        }

        public void TakeMoney(long amount, string description)
        {
            CLogger.LogMoney(pid, money, money - amount, description);
            money -= amount;
            Save(CAccountData.ESave.MONEY);
        }

        public void SetMoney(long amount, string description)
        {
            CLogger.LogMoney(pid, money, amount, description);
            money = amount;
            Save(CAccountData.ESave.MONEY);
        }

        public Vector3 GetLastPosition()
        {
            return lastPosition;
        }

        public bool Save(CAccountData.ESave save = CAccountData.ESave.ALL)
        {
            switch(save)
            {
                case CAccountData.ESave.ALL:
                    Globals.Mysql.Update("update accounts set money = @p2, health = @p3, xp = @p4, clothes = @p5, accessory = @p6, lastposition = @p7 where pid = @p1 limit 1",
                        pid, money, player.Health, xp, clothes.GetSaveString(), accessories.GetSaveString(), player.Position.ToStr());
                    return true;
                case CAccountData.ESave.APPEARANCE:
                    Globals.Mysql.Update("update accounts set clothes = @p2, accessory = @p3 where pid = @p1 limit 1",
                        pid, clothes.GetSaveString(), accessories.GetSaveString());
                    return true;
                case CAccountData.ESave.MONEY:
                    Globals.Mysql.Update("update accounts set money = @p1 where pid = @p2 limit 1", money, pid);
                    return true;
                case CAccountData.ESave.XP:
                    Globals.Mysql.Update("update accounts set xp = @p1 where pid = @p2 limit 1", xp, pid);
                    return true;
            }
            return false;
        }
    }
}
