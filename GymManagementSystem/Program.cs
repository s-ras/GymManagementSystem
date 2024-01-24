using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GymManagementSystem
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static bool isLoggedIn = false;
        public static Registration acc;
        public static bool keepOpen = false;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmSplash());

            do
            {
                keepOpen = false;
                Application.Run(new frmLogin());
                if (isLoggedIn)
                {
                    Application.Run(new frmMainMenu());
                }
            } while (keepOpen);
        }
    }
}
