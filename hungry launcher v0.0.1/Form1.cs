using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.IO.Compression;
using Microsoft.Win32;
using System.Threading;

namespace hungry_launcher_v0._0._1
{
    public partial class Form1 : Form
    {
        string mdir;
        string mversion;
        string[] mver;
        bool console,autoclose;

        public Form1()
        {
            InitializeComponent();
        }
        private void button2_Click(object sender, EventArgs e)
        {       
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mdir = Properties.Settings.Default.mdir;
            if (mdir == "")
            {

                            string path;
            using (var dialog = new FolderBrowserDialog())
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    path = dialog.SelectedPath;
                    mdir = path;
                }

                else
                {
                    path = mdir;
                }
             Properties.Settings.Default.mdir = mdir;

            }
            checkBox1.Checked = Properties.Settings.Default.chBox;
            checkBox2.Checked = Properties.Settings.Default.chBox2;
            checkBox3.Checked = Properties.Settings.Default.chBox3;
            if (checkBox2.Checked == true) {
                textBox2.Text = Properties.Settings.Default.Textbox2;
            }
            textBox1.Text = Properties.Settings.Default.Textbox;

            comboBox1.Text = Properties.Settings.Default.combobox;
            mver = utils.mineversions(mdir);
            if (mver != null)
            {
               foreach (Object i in mver)
                {
                    comboBox1.Items.Add(i);  
                }                        
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {   
            string javahome;
            long memory = 0;
            javahome = utils.getjavapath();
            if (javahome == null)
            {
                MessageBox.Show("Java не найдена");
            }
            else
            {
                if (mdir == null)
                {
                    MessageBox.Show("Не указана папка с игрой");
                }
                else
                {
                    autoclose = checkBox3.Checked;
                    console = checkBox1.Checked;
                    mversion = comboBox1.Text;


                    char a = '"';
                    string username = textBox1.Text;
                    //     string token = "--session token:"; //+ tokenGenerated;

                    string zipPath = "{0}\\libraries\\org\\lwjgl\\lwjgl\\lwjgl-platform\\2.9.0\\lwjgl-platform-2.9.0-natives-windows.jar";
                    string extractPath = "{0}\\libraries\\org\\lwjgl\\lwjgl\\lwjgl-platform\\2.9.0\\natives";
                    zipPath = string.Format(zipPath, mdir);
                    extractPath = string.Format(extractPath, mdir);
                    if (Directory.Exists(extractPath))
                    {
                        Directory.Delete(extractPath, true);
                    }
                    ZipFile.ExtractToDirectory(zipPath, extractPath);
                    Directory.Delete(extractPath + "\\META-INF", true);

                    string launch1 = " -Xms1G -Xmx1G -Djava.library.path={0}\\libraries\\org\\lwjgl\\lwjgl\\lwjgl-platform\\2.9.0\\natives -cp ";
                    string launch2 = "{0}\\libraries\\net\\sf\\jopt-simple\\jopt-simple\\4.5\\jopt-simple-4.5.jar;{0}\\libraries\\com\\paulscode\\codecjorbis\\20101023\\codecjorbis-20101023.jar;{0}\\libraries\\com\\paulscode\\codecwav\\20101023\\codecwav-20101023.jar;{0}\\libraries\\com\\paulscode\\libraryjavasound\\20101123\\libraryjavasound-20101123.jar;";   //Begin and Mem and Sound  syst
                    string launch3 = "{0}\\libraries\\com\\paulscode\\librarylwjglopenal\\20100824\\librarylwjglopenal-20100824.jar;{0}\\libraries\\com\\paulscode\\soundsystem\\20120107\\soundsystem-20120107.jar;{0}\\libraries\\argo\\argo\\2.25_fixed\\argo-2.25_fixed.jar;{0}\\libraries\\org\\bouncycastle\\bcprov-jdk15on\\1.47\\bcprov-jdk15on-1.47.jar;{0}\\libraries\\com\\google\\guava\\guava\\14.0\\guava-14.0.jar;{0}\\libraries\\org\\apache\\commons\\commons-lang3\\3.1\\commons-lang3-3.1.jar;";               //Sound Syst and argo and guava and apche
                    string launch4 = "{0}\\libraries\\commons-io\\commons-io\\2.4\\commons-io-2.4.jar;{0}\\libraries\\net\\java\\jinput\\jinput\\2.0.5\\jinput-2.0.5.jar;{0}\\libraries\\net\\java\\jutils\\jutils\\1.0.0\\jutils-1.0.0.jar;{0}\\libraries\\com\\google\\code\\gson\\gson\\2.2.2\\gson-2.2.2.jar;{0}\\libraries\\org\\lwjgl\\lwjgl\\lwjgl\\2.9.0\\lwjgl-2.9.0.jar;{0}\\libraries\\org\\lwjgl\\lwjgl\\lwjgl_util\\2.9.0\\lwjgl_util-2.9.0.jar;{0}\\libraries\\net\\java\\jinput\\jinput-platform\\2.0.5\\jinput-platform-2.0.5-natives-windows.jar;{0}\\versions\\{1}\\{1}.jar;";    //LWJGl and GSON and Version
                    string launch5 = "{0}\\libraries\\net\\minecraftforge\\minecraftforge\\9.11.1.965\\minecraftforge-9.11.1.965.jar;{0}\\libraries\\org\\ow2\\asm\\asm-all\\4.1\\asm-all-4.1.jar;{0}\\libraries\\org\\scala-lang\\scala-library\\2.10.2\\scala-library-2.10.2.jar;{0}\\libraries\\org\\scala-lang\\scala-compiler\\2.10.2\\scala-compiler-2.10.2.jar;{0}\\libraries\\com\\mumfrey\\liteloader\\1.6.4\\liteloader-1.6.4.jar;{0}\\libraries\\net\\minecraft\\launchwrapper\\1.8\\launchwrapper-1.8.jar;{0}\\libraries\\lzma\\lzma\\0.0.1\\lzma-0.0.1.jar"; // Forge and Liteloader
                    string launch6 = " net.minecraft.launchwrapper.Launch --username " + a + username + a + " --version 1.6.4" + " --gameDir {0} --assetsDir {0}\\assets --tweakClass com.mumfrey.liteloader.launch.LiteLoaderTweaker --tweakClass cpw.mods.fml.common.launcher.FMLTweaker"; //Main and Other         
                    string launch = launch1 + launch2 + launch3 + launch4 + launch5 + launch6;
                    launch = string.Format(launch, mdir, mversion);
                    if (console == true)
                    {
                        ProcessStartInfo mcStartInfo = new ProcessStartInfo(javahome + "\\bin\\java.exe", launch);              
                        Process.Start(mcStartInfo);
                        if (autoclose == true)
                        {
                            while (memory < 400000)
                            {
                                System.Diagnostics.Process[] pr = Process.GetProcessesByName("java");
                                foreach (Process process in pr)
                                {
                                    memory = process.WorkingSet64 / 1024;
                                }

                            }
                            this.Close();
                        }
                    }
                    else
                    {
                        ProcessStartInfo mcStartInfo = new ProcessStartInfo(javahome + "\\bin\\javaw.exe", launch);
                        Process.Start(mcStartInfo);
                        if (autoclose == true)
                        {
                            while (memory < 400000)
                            {
                                System.Diagnostics.Process[] pr = Process.GetProcessesByName("javaw");
                                foreach (Process process in pr)
                                {
                                    memory = process.WorkingSet64 / 1024;
                                }

                            }
                            this.Close();
                        }
                    }
                   
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Textbox = textBox1.Text;
            Properties.Settings.Default.Save();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            string path;
            using (var dialog = new FolderBrowserDialog())
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    path = dialog.SelectedPath;
                    mdir = path;
                }

                else
                {
                    path = mdir;
                }
             Properties.Settings.Default.mdir = mdir;
             Properties.Settings.Default.Save();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.chBox = checkBox1.Checked;
            Properties.Settings.Default.Save();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.chBox2 = checkBox2.Checked;
            Properties.Settings.Default.Save();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                Properties.Settings.Default.Textbox2 = textBox2.Text;
                Properties.Settings.Default.Save();
            }

        }
        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.chBox3 = checkBox3.Checked;
            Properties.Settings.Default.Save();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.combobox = comboBox1.Text;
            Properties.Settings.Default.Save();
        }
        private void comboBox1_Dropdown(object sender, System.EventArgs e)
        {
            mver = utils.mineversions(mdir);
            comboBox1.Items.Clear();
            if (mver != null)
            {
                foreach (Object i in mver)
                {
                    comboBox1.Items.Add(i);
                }
            }
            Properties.Settings.Default.combobox = comboBox1.Text;
            Properties.Settings.Default.Save();
        }
    }



    /// <summary>
    ///                             GETJAVA CLASS
    /// </summary>

    public class utils
    {
        public static string getjavapath()
        {
            string javapath = null;
            string jdkKey = "SOFTWARE\\JavaSoft\\Java Development Kit";
            string jreKey = "SOFTWARE\\JavaSoft\\Java Runtime Environment";

            var basejdk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(jdkKey);
            var basejre = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(jreKey);

            if (basejdk != null)
            {
                using (basejdk)
                {
                    String currentVersion = basejdk.GetValue("CurrentVersion").ToString();
                    using (var homeKey = basejdk.OpenSubKey(currentVersion))
                    javapath = homeKey.GetValue("JavaHome").ToString();
                }
            }
            else if (basejre != null)
            {
                String currentVersion = basejre.GetValue("CurrentVersion").ToString();
                using (var homeKey = basejre.OpenSubKey(currentVersion))
                javapath = homeKey.GetValue("JavaHome").ToString();
            }
            return javapath;
        }

        public static string[] mineversions(string mdir){

            List<string> mver = new List<string>();
            string versions = "{0}\\versions\\";
            versions = string.Format(versions, mdir);
            DirectoryInfo verpath = new DirectoryInfo(versions);
            FileInfo[] vers =  verpath.GetFiles("*.jar", SearchOption.AllDirectories);
                    foreach (FileInfo file in vers)
                    {
                        for (int i = 0; i <= 9; i++)
                        {
                            if (file.Name.Contains("1." + i.ToString()) && (file.DirectoryName.Equals(versions+ Truncates(file.Name))))  
                            {                
                                mver.Add(Truncates(file.Name));
                            }
                        }
                    }

            return mver.ToArray();
        }

        public static string Truncates(string trunc) {
            string trunced = "";
            for (int i = 0; i <= trunc.Length; i++) {
                char x = trunc[i];
                char j = trunc[i + 1];
                char a = trunc[i + 2];
                char r = trunc[i + 3];
                if ((x == '.') && (j == 'j') && (a == 'a') && (r == 'r'))
                {
                    break;
                }
                trunced = trunced + x;
            }
            return trunced;
        }
    }
}