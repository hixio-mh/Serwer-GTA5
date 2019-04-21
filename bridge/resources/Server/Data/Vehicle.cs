using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GTANetworkAPI;
using GTANetworkInternals;
using GTANetworkMethods;

namespace Data.Vehicle
{
    public enum EVehicleClass
    {
        Compact = 0,
        Sedan,
        SUV,
        Coupe,
        Muscle,
        Sport1,
        Sport2,
        Super,
        Motorcycle,
        OffRoad,
        Industrial,
        Utility,
        Van,
        Cycle,
        Boat,
        Helicopter,
        Plane,
        Service,
        Emergency,
        Military,
        Commercial,
        Train,
        Unknown,
    }

    public class CVehicleData
    {
        public bool hasFuel = true;
        public float fuel = 40; // domyślna pojemność baku
        public float fuelConsumption = 5; // zużycie paliwa "na 100 kilometrów"
        public CVehicleData() { }
        public CVehicleData(EVehicleClass vehicleClass)
        {
            switch(vehicleClass)
            {
                case EVehicleClass.Cycle:
                    hasFuel = false;
                    break;
                default:
                    break;
            }
        }
    }

    public static class VehicleData
    {
        public static EVehicleClass ConvertClass(int vehicleClass)
        {
            EVehicleClass eClass = (EVehicleClass)vehicleClass;
            return eClass;
        }
        public static CVehicleData GetVehicleDefaultData(VehicleHash vehicleHash)
        {
            CVehicleData vehicleData;
            if (data.TryGetValue(vehicleHash, out vehicleData))
            {
                return vehicleData;
            }
            return null;
        }
        
        public static void InitiliazeDefault()
        {
            foreach (VehicleHash type in Enum.GetValues(typeof(VehicleHash)))
            {
                if(!data.ContainsKey(type))
                {
                    data[type] = new CVehicleData(ConvertClass(NAPI.Vehicle.GetVehicleClass(type)));
                }
            }

        }

        // Domyślne wartości każdego pojazdu.
        private static Dictionary<VehicleHash, CVehicleData> data = new Dictionary<VehicleHash, CVehicleData>
        {
            [VehicleHash.Chimera] = new CVehicleData
            {
                fuel = 40,
                fuelConsumption = 6,
            },
        };

    }
}
