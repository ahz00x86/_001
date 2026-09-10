using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileToVideo_VideoToFile
{
    public class apzExperimentalCode
    {
        public Int32 lbS { get; set; }

        public List<apzExpCodSearch> apzExpCodSearches { get; set; }

    }

    public class apzExpCodSearch
    {
        public apzExpCodSearch Padre { get; set; }

        public apzExpCodSearch Izq0 { get; set; }

        public apzExpCodSearch Der1 { get; set; }

        public String S01 { get; set; }

        public Int32 Count { get; set; }

        public String BinaryCode { get; set; }

        public List<Int32> Where { get; set; }
    }

    public class apzExpCodSearchBool
    {
        public apzExpCodSearchBool Padre { get; set; }

        public apzExpCodSearchBool Izq0 { get; set; }

        public apzExpCodSearchBool Der1 { get; set; }

        public Boolean[] S01 { get; set; }

        public Int32 Count { get; set; }

        public String BinaryCode { get; set; }

        public List<Int32> Where { get; set; }
    }
}
