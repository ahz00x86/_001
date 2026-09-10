using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace c666wayServ
{
    [Serializable]
    public class c666way
    {
        public Guid IdWay { get; set; }
                        
        public Int32 Diff { get; set; }

        public Int32 PosX { get; set; }
        public Int32 PosY { get; set; }

        public Boolean Start { get; set; }

        public Boolean Continue { get; set; }
    }
}
