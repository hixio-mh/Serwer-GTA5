using System;
using System.Collections.Generic;
using MongoDB.Driver;
using Extend;
using MongoDB.Bson.Serialization.Attributes;
using Main;
using Managers;
using GTANetworkAPI;
using Mongo;
using Vehicle = GTANetworkAPI.Vehicle;
using Model.Database;
using Interfaces;
using Logic.Inventory;

namespace Models
{
    public class License : ILicense
    {
        public int LicenseId { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Suspended { get; set; }

        public string SuspendedReason { get; set; }

        public bool isSuspended()
        {
            if (Suspended != null && Suspended > DateTime.Now)
            {
                return true;
            }
            return false;
        }
    }

    public class Configuration : MongoResult<Configuration>
    {
        public static string CollectionName = "Configuration";

        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, Tuple<Vector3, float, string>> Spawnpoints { get; set; } = new Dictionary<string, Tuple<Vector3, float, string>>();
        public List<Tuple<Vector3, float, VehicleHash>> PublicVehicles { get; set; } = new List<Tuple<Vector3, float, VehicleHash>>();
        public List<ExamQuestion> ExamsQuestions { get; set; } = new List<ExamQuestion>();
        public List<Tuple<int, long>> Levels { get; set; } = new List<Tuple<int, long>>();

    }

    public class Clothes : IPlayerLook
    {
        public int Slot { get; set; }
        public int Drawable { get; set; }
        public int Texture { get; set; }
    }

    public class Accessory : IPlayerLook
    {
        public int Slot { get; set; }
        public int Drawable { get; set; }
        public int Texture { get; set; }
    }

    public class AdminACL
    {
        public HashSet<string> Permissions { get; set; }
    }

    public class AdminGroup
    {
        public string GroupName { get; set; }
        HashSet<string> Permissions { get; set; }
        public bool HasPermission(string permission) => Permissions.Contains(permission);
    }

    public class Admin
    {
        public bool IsAdmin { get; set; } = false;
        public List<AdminGroup> Groups { get; set; }
        public int Color { get; set; }
        public string VisualRank { get; set; }

        public bool HasPermissionTo(string permission)
        {
            foreach(var group in Groups)
                if (group.HasPermission(permission))
                    return true;

            return false;
        }
    }

    public class Account : MongoResult<Account>, IAccount
    {
        public static string CollectionName = "Accounts";

        #region Zmienne z dokumentu

        public long AccountId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int Health { get; set; }
        public uint Xp { get; set; }

        public long Money { get; set; }
        public Vector3 LastPosition { get; set; }
        public uint IP { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime RegisterTime { get; set; } = DateTime.Now;

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastLogin { get; set; } = DateTime.Now;
        public List<License> Licenses { get; set; } = new List<License>();
        public List<Clothes> Clothes { get; set; } = new List<Clothes>();
        public List<Accessory> Accessories { get; set; } = new List<Accessory>();
        public List<string> Groups { get; set; } = new List<string>();
        public Inventory Inventory { get; set; } = new Inventory(3,3);

        [BsonIgnoreIfNull]
        public Admin Admin { get; set; } = null;

        #endregion

        #region Zmienne tymczasowe
        [BsonIgnore]
        private Client player;

        [BsonIgnore]
        public Client Player { get => player; set { if (player is null) player = value; } }

        [BsonIgnore]
        public int Level { get; set; }

        #endregion

        #region Metody
        ~Account()
        {
            Globals.Managers.account.setAccountUsed(AccountId, false);
        }

        public List<Vehicle> GetPrivateVehicles() =>
            Globals.Managers.vehicle.vehicles[EVehicleType.PRIVATE].FindAll(veh => veh.OwnerPID() == AccountId);

        public void SetXP(uint xp)
        {
            Xp = xp;
            Level = Globals.Managers.account.GetLevelFromXP(Xp);
            if(!ReferenceEquals(Player, null))
                Player.TriggerClient(CRPCManager.ERPCs.PLAYER_UPDATE_EXP, Xp, Level);
        }

        public void AddXP(uint xp)
        {
            SetXP(Xp + xp);
        }

        public void RebuildPlayerAccessories()
        {
            for (int i = 0; i < 20; i++)
                player.ClearAccessory(i);

            foreach (Accessory accessory in Accessories)
            {
                player.SetAccessories(accessory.Slot, accessory.Drawable, accessory.Texture);
            }
        }

        public void AddAccessory(int slot, int drawable, int texture)
        {
            Accessories.RemoveAll(a => a.Slot == slot);
            Accessories.Add(new Accessory
            {
                Slot = slot,
                Drawable = drawable,
                Texture = texture,
            });
        }

        public void RemoveAccessory(int slot)
        {
            Accessories.RemoveAll(a => a.Slot == slot);
        }

        public void RebuildPlayerClothes()
        {
            player.SetDefaultClothes();

            for (int i = 0; i < 20; i++)
                player.ClearAccessory(i);

            foreach (Clothes clothesSet in Clothes)
            {
                player.SetClothes(clothesSet.Slot, clothesSet.Drawable, clothesSet.Texture);
            }
        }

        public void AddClothes(int slot, int drawable, int texture)
        {
            Clothes.RemoveAll(a => a.Slot == slot);
            Clothes.Add(new Clothes
            {
                Slot = slot,
                Drawable = drawable,
                Texture = texture,
            });
        }

        public void RemoveClothes(int slot)
        {
            Clothes.RemoveAll(a => a.Slot == slot);
        }

        public bool SetPlayer(Client player)
        {
            if (this.player != null) return false;

            this.player = player;
            LastLogin = DateTime.Now;
            player.AssignUID(AccountId);
            RebuildPlayerAccessories();
            RebuildPlayerClothes();

            SetXP(Xp);
            return true;
        }

        public License GetLicense(int lid)
        {
            if (HasLicense(lid))
            {
                License license = Licenses.Find(l => l.LicenseId == lid);
                if (license == null)
                {
                    return null;
                }
                return license;
            }
            return null;
        }
        public bool HasLicense(int lid, bool checkIfIsSuspended = false)
        {
            License license = Licenses.Find(i => i.LicenseId == lid);
            if (license == null)
            {
                return false;
            }

            if (checkIfIsSuspended)
            {
                if (license.isSuspended())
                {
                    return false;
                }
            }
            return true;
        }

        public bool GiveLicense(byte lid)
        {
            if (HasLicense(lid))
            {
                return false;
            }

            Licenses.Add(new License { LicenseId = lid });

            return true;
        }

        public void CleanUp()
        {

        }

        public void GiveMoney(long amount, string description)
        {
            //CLogger.LogMoney(AccountId, Money, Money + amount, description);
            Money += amount;
            //Save(CAccountData.ESave.MONEY);
        }

        public void TakeMoney(long amount, string description)
        {
            //CLogger.LogMoney(pid, money, money - amount, description);
            Money -= amount;
            //Save(CAccountData.ESave.MONEY);
        }

        public void SetMoney(long amount, string description)
        {
            //CLogger.LogMoney(pid, money, amount, description);
            Money = amount;
            //Save(CAccountData.ESave.MONEY);
        }

        #endregion

        #region Metody statyczne

        public static long GetNextAccountId()
        {
            return Globals.Mongo.CountDocuments(Collection) + 1;
        }

        #endregion

    }
}
