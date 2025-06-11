using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Medic
{
    internal static class Program
    {
        /// Главная точка входа для приложения.
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ApplicationExit += OnApplicationExit;

            Application.Run(new Window_1());
        }
        private static void OnApplicationExit(object sender, EventArgs e)
        {
            SharedData.DisposeSharedPic1();
            SharedData.DisposeSharedPic2();
            SharedData.DisposeSharedPic3();
            SharedData.DisposeSharedPic4();
        }
    }
}
