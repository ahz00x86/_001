using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace c666wayServ
{
    [Serializable]
    internal class c666wayUser
    {
        public Guid Id { get; set; }

        public Guid Password { get; set; }

        public Int16 State { get; set; }

        public DateTime LastUpdate { get; set; }

        public c666way Way { get; set; }
        
    }
}
