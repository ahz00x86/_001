using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GWService
{
    [Serializable]
    public class GWUser
    {
        public Guid Id { get; set; }

        public Guid Password { get; set; }

        public Int16 State { get; set; }

        public DateTime LastUpdate { get; set; }

        public Room Room { get; set; }
    }
}
