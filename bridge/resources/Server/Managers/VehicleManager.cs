using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GTANetworkAPI;
using GTANetworkInternals;
using GTANetworkMethods;
using DevOne.Security.Cryptography.BCrypt;
using Database;
using Main;
using Logic.Account;
using Extend.Vehicle;
using Vehicle = GTANetworkAPI.Vehicle;
using Extend.Entity;

namespace Managers
{
    public class CVehicleManager
    {
        public Dictionary<EVehicleType, List<Vehicle>> vehicles = new Dictionary<EVehicleType, List<Vehicle>>();
        public CVehicleManager()
        {
            foreach (EVehicleType type in Enum.GetValues(typeof(EVehicleType)))
            {
                vehicles[type] = new List<Vehicle>();
            }
        }

        public void OnPlayerEnterVehicle(Client player, Vehicle vehicle, sbyte seatID)
        {
            EVehicleType type = vehicle.GetVehicleType();
            switch (type)
            {
                case EVehicleType.UNKNOWN:

                    break;
                case EVehicleType.PRIVATE:

                    break;
                case EVehicleType.FRACTION:

                    break;
                case EVehicleType.WORK:

                    break;
                case EVehicleType.PUBLIC:

                    break;
                case EVehicleType.EXAM:

                    break;
                case EVehicleType.SALON:

                    break;
                case EVehicleType.EVENT:

                    break;
            }
        }

        public Vehicle Create(EVehicleType type, VehicleHash vehicleHash, Vector3 pos, Vector3 rot)
        {
            Vehicle vehicle = NAPI.Vehicle.CreateVehicle(vehicleHash, pos, rot, new Color(255, 255, 255).ToInt32(), new Color(255, 255, 255).ToInt32());
            vehicle.AddExtension();
            vehicle.SetVehicleType(type);
            vehicles[type].Add(vehicle);
            switch(type)
            {
                case EVehicleType.UNKNOWN:

                    break;
                case EVehicleType.PRIVATE:

                    break;
                case EVehicleType.FRACTION:

                    break;
                case EVehicleType.WORK:

                    break;
                case EVehicleType.PUBLIC:

                    break;
                case EVehicleType.EXAM:

                    break;
                case EVehicleType.SALON:

                    break;
                case EVehicleType.EVENT:

                    break;
            }
            return vehicle;
        }

        public Vehicle GetPrivateVehicleByUID(uint vid)
        {
            foreach(Vehicle vehicle in vehicles[EVehicleType.PRIVATE])
            {
                if(vehicle.UID() == vid)
                {
                    return vehicle;
                }
            }
            return null;
        }

        public bool Destroy(Vehicle vehicle)
        {
            if(vehicle.IsType(EVehicleType.PRIVATE))
            {
                vehicle.Save();
            }
            NAPI.Entity.DeleteEntity(vehicle);
            return true;
        }
        public uint GetLastVid()
        {
            return Convert.ToUInt32(Globals.Mysql.GetValue("select max(vid) from vehicles limit 1"));
        }

        public uint CreatePrivateVehicle(VehicleHash vehicleHash, Client owner)
        {
            Globals.Mysql.UpdateBlocking("insert into vehicles (hash,pid,firstowner)values(@p1,@p2,@p2)", vehicleHash, owner.UID());
            return GetLastVid();
        }
        public Vehicle SpawnPrivateVehicle(uint vid)
        {
            return null;
        }
    }
}
