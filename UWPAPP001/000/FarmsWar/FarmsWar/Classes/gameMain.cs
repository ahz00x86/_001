using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Media3D;

namespace FarmsWar.Classes
{
    public class gameMain
    {
        public UIElement Sprite { get; set; }

        public Guid Id { get; set; }

        public Int16 Type { get; set; }

        public CompositeTransform3D Perspective { get; set; }
    }
}
