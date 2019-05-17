using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GTANetworkAPI;
using GTANetworkInternals;
using Extend;
using Main;

namespace Extend
{
    public enum EVehicleType
    {
        UNKNOWN = 0,
        PRIVATE,
        FRACTION,
        WORK,
        PUBLIC,
        EXAM,
        SALON,
        EVENT,
    }

    public class CVehicleExtend
    {
        private readonly Vehicle vehicle = null;
        private EVehicleType type = EVehicleType.UNKNOWN;
        private Client owner = null;
        private uint pid = 0;
        private Tuple<float, float> fuel = Tuple.Create(0.0f, 0.0f);

        public CVehicleExtend(GTANetworkAPI.Vehicle vehicle)
        {
            this.vehicle = vehicle;
        }
        public void SetVehicleType(EVehicleType eType)
        {
            type = eType;
        }

        public EVehicleType GetVehicleType()
        {
            return type;
        }

        public Client Owner()
        {
            return owner;
        }

        public uint OwnerPID()
        {
            return pid;
        }

        public void SetOwner(Client owner)
        {
            this.owner = owner;
            long? tmpPid = owner.UID();
            if (tmpPid.HasValue)
            {
                pid = (uint)tmpPid;
                vehicle.SetSharedData("owner", pid);
            }
        }

        public void SetOwner(uint pid)
        {
            vehicle.SetSharedData("owner", pid);
            this.pid = pid;
        }
    }

    public static class CVehicleExtension
    {

        private static EVehicleType type = EVehicleType.UNKNOWN;

        public static void AddExtension(this GTANetworkAPI.Vehicle vehicle)
        {
            CVehicleExtend extension = new CVehicleExtend(vehicle);
            vehicle.SetData("extension", extension);
        }
        
        public static CVehicleExtend GetExtension(this GTANetworkAPI.Vehicle vehicle)
        {
            if(!vehicle.HasData("extension"))
            {
                vehicle.AddExtension();
            }
            return vehicle.GetData("extension");
        }

        public static void SetVehicleType(this GTANetworkAPI.Vehicle vehicle, EVehicleType newType)
        {
            vehicle.GetExtension().SetVehicleType(newType);
        }

        public static EVehicleType GetVehicleType(this GTANetworkAPI.Vehicle vehicle)
        {
            return vehicle.GetExtension().GetVehicleType();
        }

        public static bool IsType(this GTANetworkAPI.Vehicle vehicle, EVehicleType eType)
        {
            return vehicle.GetVehicleType() == eType;
        }

        public static Client Owner(this GTANetworkAPI.Vehicle vehicle)
        {
            return vehicle.GetExtension().Owner();
        }

        public static uint OwnerPID(this GTANetworkAPI.Vehicle vehicle)
        {
            return vehicle.GetExtension().OwnerPID();
        }

        public static void SetOwner(this GTANetworkAPI.Vehicle vehicle, Client player)
        {
            vehicle.GetExtension().SetOwner(player);
        }

        public static void SetOwner(this GTANetworkAPI.Vehicle vehicle, uint pid)
        {
            vehicle.GetExtension().SetOwner(pid);
        }
        
        public static void SafeDelete(this GTANetworkAPI.Vehicle vehicle)
        {
            Globals.Managers.vehicle.Destroy(vehicle);
        }

        public static bool Save(this GTANetworkAPI.Vehicle vehicle)
        {
            if(vehicle.IsType(EVehicleType.PRIVATE))
            {
                if (vehicle.UID() != null)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
