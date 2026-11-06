using System;
using System.Threading;
using System.Windows.Forms;

namespace TextToHex
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Une exception inattendue affiche un message clair au lieu de fermer l'application.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += OnThreadException;

            Application.Run(new FRM_Main());
        }

        private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show("Une erreur inattendue s'est produite :\n\n" + e.Exception.Message,
                "Texte vers Hexadécimal", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
