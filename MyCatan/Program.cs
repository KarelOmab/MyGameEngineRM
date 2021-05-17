using MyGameEngineRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyCatan
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            RenderWindow rWindow = new RenderWindow("Catan");
            rWindow.SetScreenSize(RenderWindow.ScreenSize.LARGE); //using my template
            Catan gameCatan = new Catan(rWindow);
            Application.Run(rWindow);
        }
    }
}
