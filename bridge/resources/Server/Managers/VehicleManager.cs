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
using Extend;
using Vehicle = GTANetworkAPI.Vehicle;

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
                    Globals.Systems.publicVehicles.OnPlayerEnterPublicVehicle(player, vehicle, seatID);
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
            vehicle.SetSharedData("type", type);
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

        public bool IsPrivateVehicleAlreadySpawned(uint vid)
        {
            return GetPrivateVehicleByUID(vid) != null;
        }

        public bool Destroy(Vehicle vehicle, bool clearing = false)
        {
            if(vehicle == null) return false;

            EVehicleType type = vehicle.GetVehicleType();

            vehicles[type].Remove(vehicle);

            switch (type)
            {
                case EVehicleType.UNKNOWN:

                    break;
                case EVehicleType.PRIVATE:
                    vehicle.Save();
                    break;
                case EVehicleType.FRACTION:

                    break;
                case EVehicleType.WORK:

                    break;
                case EVehicleType.PUBLIC:
                    Globals.Systems.publicVehicles.RemovePublicVehicle(vehicle, clearing);
                    break;
                case EVehicleType.EXAM:

                    break;
                case EVehicleType.SALON:

                    break;
                case EVehicleType.EVENT:

                    break;
            }

            if(vehicle != null)
                NAPI.Entity.DeleteEntity(vehicle);

            return true;
        }

        public uint GetLastVid()
        {
            return Convert.ToUInt32(Globals.Mysql.GetValue("select max(vid) from vehicles limit 1"));
        }

        public uint CreatePrivateVehicle(VehicleHash vehicleHash, Client owner, Vector3 position = null, Vector3 rotation = null)
        {
            if(position != null && rotation != null)
            {
                Globals.Mysql.UpdateBlocking("insert into vehicles (vehiclehash,pid,firstowner,position,rotation)values(@p1,@p2,@p2.@p3,@p3)", (uint)vehicleHash, owner.UID(), position.ToStr(), rotation.ToStr());
            }
            else
            {
                Globals.Mysql.UpdateBlocking("insert into vehicles (vehiclehash,pid,firstowner)values(@p1,@p2,@p2)", (uint)vehicleHash, owner.UID());
            }
            return GetLastVid();
        }

        public Vehicle SpawnPrivateVehicle(uint vid, Vector3 position = null, Vector3 rotation = null)
        {
            if(IsPrivateVehicleAlreadySpawned(vid))
            {
                return null;
            }

            Database.CVehiclesRow vehRow = Globals.Mysql.select.GetVehicleByUID(vid);
            if (vehRow.isResult)
            {
                vehRow.position = position==null?vehRow.position:position;
                vehRow.rotation = position==null?vehRow.rotation:rotation;
                if (vehRow.position != null && vehRow.rotation != null)
                {
                    Vehicle veh = Create(EVehicleType.PRIVATE, vehRow.vehicleHash, vehRow.position, vehRow.rotation);
                    veh.AssignUID(vid);
                    veh.SetOwner(vehRow.pid);
                    return veh;
                }
            }

            return null;
        }
    }
}
