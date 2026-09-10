using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apzyxGames.Models
{
    public class DominoTabModel
    {
        public Guid Id { get; set; }

        public Int16[] Values { get; set; }

        public Int16 Player { get; set; }

        public Boolean Disabled { get; set; }

    }
}