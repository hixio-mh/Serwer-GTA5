using System;
using System.Collections.Generic;
using System.Text;

namespace Systems
{
    public class CSystems
    {
        public CPublicVehicles publicVehicles;
        public CAdmin admin;
        public CSystems()
        {
            publicVehicles = new CPublicVehicles();
            admin = new CAdmin();
        }
    }
}
