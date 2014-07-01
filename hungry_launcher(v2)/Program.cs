using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace hungry_launcher_v2_
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
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            Application.Run(new Form1());
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("hungry_launcher(v2).Newtonsoft.Json.dll"))
            {
                byte[] AssemblyData = new byte[stream.Length];
                stream.Read(AssemblyData, 0, AssemblyData.Length);
                return Assembly.Load(AssemblyData);
            }
        }
    }
}
