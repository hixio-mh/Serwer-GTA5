using System;
using System.Collections.Generic;
using System.Text;
using Main;
using Database;
using Extend;
using GTANetworkAPI;
using System.Threading.Tasks;

namespace Systems
{
    public class CPublicVehicles
    {
        /*
        List<CVehiclePublicRow> PublicVehiclesSpawns = new List<CVehiclePublicRow>();
        List<Vehicle> PublicVehicles = new List<Vehicle>();

        public void UpdateSpawnList()
        {
            PublicVehiclesSpawns.Clear();
            Globals.Mysql.GetTableRows(ref PublicVehiclesSpawns);
        }

        public void RemovePublicVehicle(Vehicle publicVehicle, bool clearing = false)
        {
            if(!clearing)
                PublicVehicles.Remove(publicVehicle);
            publicVehicle.Delete();
        }

        public void RegenPublicVehicle(Vehicle publicVehicle)
        {
            if (!IsOriginal(publicVehicle)) return;

            CVehiclePublicRow vehicleRow = (CVehiclePublicRow)publicVehicle.GetData("originalPosition");
            Vehicle regenedPublicVehicle = Globals.Managers.vehicle.Create(EVehicleType.PUBLIC, vehicleRow.vehicleHash, vehicleRow.position, new Vector3(0, 0, vehicleRow.rotation));
            PublicVehicles.Add(publicVehicle);
            publicVehicle.SetData("original", false);
            publicVehicle.ResetData("originalPosition");
            regenedPublicVehicle.SetData("original", true);
            regenedPublicVehicle.SetSharedData("owner", null);
            regenedPublicVehicle.SetData("originalPosition", vehicleRow);
            //NAPI.Entity.SetEntityCollisionless(regenedPublicVehicle, true);
        }
        
        public bool IsOriginal(Vehicle publicVehicle)
        {
            return publicVehicle.GetData("original");
        }

        public void SpawnPublicVehicles()
        {
            foreach(CVehiclePublicRow vehicleRow in PublicVehiclesSpawns)
            {
                Vehicle publicVehicle = Globals.Managers.vehicle.Create(EVehicleType.PUBLIC, vehicleRow.vehicleHash, vehicleRow.position, new Vector3(0,0,vehicleRow.rotation));
                PublicVehicles.Add(publicVehicle);
                publicVehicle.SetData("original", true);
                publicVehicle.SetSharedData("owner", null);
                publicVehicle.SetData("originalPosition", vehicleRow);
            }
        }

        public void ClearPlayerPublicVehicle(Client player)
        {
            Vehicle publicVehicle = player.GetData("publicVehicle");
            if(publicVehicle != null)
            {
                publicVehicle.SafeDelete();
                player.ResetData("publicVehicle");
            }
        }

        public void OnPlayerEnterPublicVehicle(Client player, Vehicle vehicle, sbyte seatID)
        {
            Client owner;
            if (seatID == -1)
            {
                Vehicle publicVehicle = player.GetData("publicVehicle");
                if(publicVehicle != vehicle)
                    ClearPlayerPublicVehicle(player);

                owner = vehicle.GetSharedData("owner");
                if(owner == null)
                {
                    player.SetData("publicVehicle", vehicle);
                    vehicle.SetSharedData("owner", player);
                }
                NAPI.Task.Run(() =>
                {
                    RegenPublicVehicle(vehicle);
                }, delayTime: 2000);
            }
            owner = vehicle.GetSharedData("owner");
        }

        public void Update()
        {
            UpdateSpawnList();
            PublicVehicles.ClearAndRemoveEntities();
            SpawnPublicVehicles();
            Console.WriteLine("PublicVehicles {0}", PublicVehicles.Count);
        }

        public CPublicVehicles()
        {
            UpdateSpawnList();
            SpawnPublicVehicles();
        }*/
    }
}
