using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileToVideo_VideoToFile
{
    internal class NodoArbolHuffman
    {
        public Int32 Repeticion { get; set; }

        public String S { get; set; }

        public NodoArbolHuffman Padre { get; set; }

        public NodoArbolHuffman Izquierdo { get; set; }

        public NodoArbolHuffman Derecho { get; set; }


    }
}
