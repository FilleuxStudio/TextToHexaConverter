using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextToHex
{
    public partial class FRM_Menu : Form
    {
        public FRM_Menu()
        {
            InitializeComponent();
        }

        public string ConvHex( string intp) // fonction convertie en texte string en hexa.
        {
            string outp = string.Empty; 
            char[] value = intp.ToCharArray(); // tableau de char.
            foreach(char Longueur in value) // boucle foreach 
            {
                int v = Convert.ToInt32(Longueur);
                outp += string.Format("{0:x}", v);
            }
            return outp;
        }

        private void BT_convertisseursUno_Click(object sender, EventArgs e)
        {
            TXT_Hexa.Text = ConvHex(TXT_string.Text);
        }

        private void BT_clear_Click(object sender, EventArgs e)
        {
            TXT_nombres.Clear();
            TXT_hex.Clear();            // nettoie les textbox
            TXT_bin.Clear();
        }

        private void BT_convertisseurs_Click(object sender, EventArgs e)
        {
            int numConv = int.Parse(TXT_nombres.Text); // cast txtnombres en entier
            TXT_bin.Text = Convert.ToString(numConv, 2); // convertie txtnombres en bin (base 2)
            TXT_hex.Text = Convert.ToString(numConv, 16); // convertie txtnombres en hex (base 16)
        }
    }
}
