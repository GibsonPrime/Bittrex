using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;

namespace NoWinUpdate
{
    public partial class Form1 : Form
    {
        private static string PROCESS_NAME = "Windows10UpgraderApp";

        public Form1()
        {
            InitializeComponent();
            Visible = false;
            ShowInTaskbar = false;
            run();      
        }
    
        private void run()
        {
            while (true)
            {
                foreach (Process p in System.Diagnostics.Process.GetProcesses())
                    if (p.ProcessName.Equals(Form1.PROCESS_NAME))
                        p.Kill();
                Thread.Sleep(1000);
            }
        }
    }
}
