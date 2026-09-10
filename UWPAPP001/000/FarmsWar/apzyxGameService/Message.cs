using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmsWar.Classes
{
    [Serializable]
    public class Message
    {
        public User IdUser { get; set; }
        public String Mess { get; set; }
    }
}