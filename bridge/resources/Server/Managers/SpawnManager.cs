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
using Newtonsoft.Json;
using Utils;

namespace Managers
{
    public class CSpawnManager
    {
        List<CSpawnRow> spawnList = new List<CSpawnRow>();

        public void UpdateSpawnList()
        {
            spawnList.Clear();
            Globals.Mysql.GetTableRows(ref spawnList);
        }
        public CSpawnManager()
        {
            NAPI.Server.SetAutoRespawnAfterDeath(false);
            NAPI.Server.SetAutoSpawnOnConnect(false);
            UpdateSpawnList();
        }

        public CSpawnRow GetNearest(Vector3 position)
        {
            CSpawnRow nearest = null;
            float dis = 999999;
            float tempDis;
            foreach(CSpawnRow spawn in spawnList)
            {
                tempDis = spawn.position.DistanceTo(position);
                if(dis > tempDis)
                {
                    dis = tempDis;
                    nearest = spawn;
                }
            }
            return nearest;
        }

        public CSpawnRow GetRandom()
        {
            return spawnList.GetRandom();
        }

        public void SpawnPlayer(Client player, Vector3 position, float rotation)
        {
            //NAPI.Entity.SetEntityInvincible(player, false);
            //NAPI.Entity.SetEntityPositionFrozen(player, false);
            NAPI.Entity.SetEntityTransparency(player, 255);
            NAPI.Player.SpawnPlayer(player, position);
            player.Health = 100;
            player.Rotation.Z = rotation;
        }

        public void SpawnPlayer(Client player, bool lastPosition = false)
        {
            CSpawnRow spawn = GetRandom();
            if ( lastPosition )
            {
                Vector3 lastPos = player.GetLastPosition();
                if (lastPos != null)
                {
                    spawn.position = lastPos;
                    spawn.rotation = 0;
                }
            }
            SpawnPlayer(player, spawn.position, spawn.rotation);
        }
    }
}
