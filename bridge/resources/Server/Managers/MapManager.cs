using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GTANetworkAPI;

namespace Managers
{
    public class CMapManager
    {
        string[] maps = new string[] { "prawko", "urzad" };
        string path = "bridge/resources/maps/";

        public CMapManager()
        {
            for(int j = 0; j < maps.Length; j++)
            {
                string json = File.ReadAllText($"{path}{maps[j]}");
                JObject jsonObject = JObject.Parse(json);

                JArray arr_objects = (JArray)jsonObject["Map"]["Objects"]["MapObject"];
                var obj_objects = jsonObject["Map"]["Objects"]["MapObject"];

                int numobjects = arr_objects.Count;

                for(int i = 0; i < numobjects; i++)
                {
                    var obj = obj_objects[i];
                    Vector3 pos = new Vector3((float)obj["Position"]["X"], (float)obj["Position"]["Y"], (float)obj["Position"]["Z"]);
                    Vector3 rot = new Vector3((float)obj["Rotation"]["X"], (float)obj["Rotation"]["Y"], (float)obj["Rotation"]["Z"]);

                    NAPI.Object.CreateObject((int)obj["Hash"], pos, rot, 255, 0);
                }
            }
        }
    }
}
