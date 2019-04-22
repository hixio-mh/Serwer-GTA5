using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GTANetworkAPI;
using GTANetworkInternals;
using GTANetworkMethods;
using Extend.Entity;

namespace Extend.Vehicle
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
        private readonly GTANetworkAPI.Vehicle vehicle = null;
        private EVehicleType type = EVehicleType.UNKNOWN;
        private GTANetworkAPI.Client owner = null;
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
        public GTANetworkAPI.Client Owner()
        {
            return owner;
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
        public static GTANetworkAPI.Client Owner(this GTANetworkAPI.Vehicle vehicle)
        {
            return vehicle.GetExtension().Owner();
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
