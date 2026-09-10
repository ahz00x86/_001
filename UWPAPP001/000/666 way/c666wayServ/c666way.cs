using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace c666wayServ
{
    [Serializable]
    internal class c666way
    {
        public Guid IdWay { get; set; }

        public c666wayUser User { get; set; }
                
        public Int32 Diff { get; set; }

        public Int32 PosX { get; set; }
        public Int32 PosY { get; set; }

        [NonSerialized]
        public Int32 APosX;
        [NonSerialized]
        public Int32 APosY;

        public Boolean Start { get; set; }

        public Boolean Continue { get; set; }
    }
}
