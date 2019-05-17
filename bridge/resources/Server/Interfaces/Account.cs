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
using Models;
using Logic.Inventory;

namespace Interfaces
{
    public interface ICollection<T>
    {
        [BsonIgnore]
        string CollectionName { get; set; }
        [BsonIgnore]
        IMongoCollection<T> collection { get; set; }

        [BsonIgnore]
        IMongoCollection<T> Collection { get; set; }

    }
    
    public interface ILicense
    {
        int LicenseId { get; set; }

        DateTime Suspended { get; set; }

        string SuspendedReason { get; set; }

        bool isSuspended();
    }

    public interface ILevel
    {
        int Lvl { get; set; }
        long Xp { get; set; }
    }
    public interface IPlayerLook
    {
        int Slot { get; set; }
        int Drawable { get; set; }
        int Texture { get; set; }
    }

    public interface IAdminMethods
    {
        bool HasPermissionTo();
    }
    
    public interface IAccount
    {
        bool Save(params string[] vs);
        long AccountId { get; set; }
        string Name { get; set; }
        string Password { get; set; }
        string Email { get; set; }
        int Health { get; set; }
        uint Xp { get; set; }
        int Level { get; set; }
        long Money { get; set; }
        Vector3 LastPosition { get; set; }
        uint IP { get; set; }
        DateTime RegisterTime { get; set; }
        DateTime LastLogin { get; set; }
        List<License> Licenses { get; set; }
        List<Clothes> Clothes { get; set; }
        List<Accessory> Accessories { get; set; }
        List<string> Groups { get; set; }
        Inventory Inventory { get; set; }
        Admin Admin { get; set; }
        List<Vehicle> GetPrivateVehicles();

        void SetXP(uint xp);
        void AddXP(uint xp);
        void RebuildPlayerAccessories();
        void AddAccessory(int slot, int drawable, int texture);
        void RemoveAccessory(int slot);
        void RebuildPlayerClothes();
        void AddClothes(int slot, int drawable, int texture);
        void RemoveClothes(int slot);
        bool SetPlayer(Client player);
        License GetLicense(int lid);
        bool HasLicense(int lid, bool checkIfIsSuspended = false);
        bool GiveLicense(byte lid);
        void CleanUp();
        void GiveMoney(long amount, string description);
        void TakeMoney(long amount, string description);
        void SetMoney(long amount, string description);
    }
}
