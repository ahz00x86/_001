using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace WINAPP001
{
    public partial class W001 : Form
    {
        public W001()
        {            
            InitializeComponent();

            foreach (CultureInfo cInf in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                if (cInf.DisplayName.ToLower().Contains("internacional"))
                {
                    CultureInfo.CurrentUICulture = cInf;
                    CultureInfo.CurrentCulture = cInf;
                    Update();
                    break;
                }
            }

        }

        /// <summary>Acción incial precursora en la intención de la utilidad en uso descrita o propuesta</summary>
        private void btAction001_Click(object sender, EventArgs e)
        {


            //lvO001.Controls.Add();
        }

        /// <summary>Acción en la selección de modificadores en las opciones para la composición</summary>
        private void btAction002_Click(object sender, EventArgs e)
        {
            //lvO001.Controls;

            //lvO002.Controls.Add();
        }

        /// <summary>Acción en la elección y modelado para los datos interpretados obtenidos</summary>
        private void btAction003_Click(object sender, EventArgs e)
        {

        }
    }
}
