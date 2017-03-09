using System;
using System.Windows.Forms;

namespace MerkleTreeDemo
{
    static partial class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Bootstrap("modules.xml");
            Application.Run(new Demo());
        }
    }
}
