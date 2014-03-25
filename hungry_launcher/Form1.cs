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
using Newtonsoft.Json;


namespace hungry_launcher
{
    public partial class Form1 : Form
    {
        string mdir;
        string mversion;
        string alocmem;
        string[] mver;
        hungry_launcher.utils.Version[] downver;
        bool console, autoclose;

        public Form1()
        {
            InitializeComponent();
        }
        private void button2_Click(object sender, EventArgs e)
        {
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            string path;
            mdir = Properties.Settings.Default.mdir;

            if (mdir == null)
            {
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
            if (checkBox2.Checked == true)
            {
                textBox2.Text = Properties.Settings.Default.Textbox2;
            }
            textBox1.Text = Properties.Settings.Default.Textbox;
            textBox3.Text = Properties.Settings.Default.Textbox3;

            mver = utils.mineversions(mdir);
            if (mver != null)
            {
                foreach (Object i in mver)
                {
                    comboBox1.Items.Add(i);
                }
            }
            comboBox1.Text = Properties.Settings.Default.combobox;

            downver = utils.getversions(mdir);
            if (downver != null)
            {
                foreach (var item in downver)
                {
                    comboBox3.Items.Add(item.id);
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
                    if (string.IsNullOrEmpty(comboBox1.Text))
                    {
                        MessageBox.Show("Не выбрана версия клиента");
                    }
                    else
                    {
                        memory = Convert.ToInt64(textBox3.Text);
                        if (memory < 512)
                        {
                            MessageBox.Show("Слишком мало памяти");
                        }
                        else
                        {
                            memory = 0;
                            autoclose = checkBox3.Checked;
                            console = checkBox1.Checked;
                            mversion = comboBox1.Text;
                            alocmem = textBox3.Text + "M";
                            string username = textBox1.Text;
                            char a = '"';
                            string memorys = " -Xms512M -Xmx{0}";
                            memorys = string.Format(memorys, alocmem);
                            string launch = utils.donwlibs(mversion, mdir);

                            launch = launch.Replace("${auth_player_name}", a + username + a);
                            launch = launch.Replace("${version_name}", a + mversion + a);
                            launch = launch.Replace("${game_directory}", a + mdir + a);
                            launch = launch.Replace("${game_assets}", a + mdir + "\\assets" + a);
                            //launch = launch.Replace(" --uuid ${auth_uuid}", "");
                            //launch = launch.Replace(" --accessToken ${auth_access_token}", "");
                            if (launch.Contains("${user_properties}"))
                            {
                                launch = launch.Replace(" --userProperties ${user_properties}", "");
                            }
                            if (launch.Contains("${user_type}"))
                            {
                                launch = launch.Replace(" --userType ${user_type}", "");
                            }
                            launch = memorys + launch;
                            if (console == true)
                            {
                                ProcessStartInfo mcstart = new ProcessStartInfo(javahome + "\\bin\\java.exe", launch);
                                Process.Start(mcstart);
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
                                ProcessStartInfo mcstart = new ProcessStartInfo(javahome + "\\bin\\javaw.exe", launch);
                                Process.Start(mcstart);
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
                                }
                                this.Close();
                            }
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
            comboBox1.Text = null;
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

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Textbox3 = textBox3.Text;
            Properties.Settings.Default.Save();
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(8))
            {
                e.Handled = true;
            }
        }
        private void comboBox3_Dropdown(object sender, System.EventArgs e)
        {
            downver = utils.getversions(mdir);
            comboBox3.Items.Clear();
            if (downver != null)
            {
                foreach (var item in downver)
                {
                    comboBox3.Items.Add(item.id);
                }
            }
        }
    }



    /// <summary>
    ///                             UTILITS CLASS
    /// </summary>

    public class utils
    {
        public static string getjavapath()
        {
            string javapath = null;
            string jdkKey = "SOFTWARE\\JavaSoft\\Java Development Kit";
            string jreKey = "SOFTWARE\\JavaSoft\\Java Runtime Environment";
            bool is64bit = System.Environment.Is64BitOperatingSystem;
            RegistryKey basejdk, basejre;

            if (is64bit == true)
            {
                basejdk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(jdkKey);
                basejre = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(jreKey);
            }
            else
            {
                basejdk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(jdkKey);
                basejre = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(jreKey);
            }
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
        public static string[] mineversions(string mdir)
        {
            string versions = "{0}\\versions\\";
            versions = string.Format(versions, mdir);
            if (Directory.Exists(versions))
            {

                List<string> mver = new List<string>();
                DirectoryInfo verpath = new DirectoryInfo(versions);
                FileInfo[] vers = verpath.GetFiles("*.jar", SearchOption.AllDirectories);
                foreach (FileInfo file in vers)
                {
                    string folname = file.Name.Replace(".jar", "");
                    string jsonname = file.Name.Replace("jar", "json");
                    if ((file.DirectoryName.Equals(versions + folname)) && (File.Exists(versions + folname + "\\" + jsonname)))
                    {
                        mver.Add(folname);
                    }
                }
                return mver.ToArray();
            }
            else return null;
        }

        public static hungry_launcher.utils.Version[] getversions(string mdir)
        {
            string jsonurl = "http://s3.amazonaws.com/Minecraft.Download/versions/versions.json";
            string vertext = String.Empty;
            WebRequest req = WebRequest.Create(jsonurl);
            WebResponse resp = req.GetResponse();
            using (Stream stream = resp.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    vertext = sr.ReadToEnd();
                }
            }         
            McVersion versions = JsonConvert.DeserializeObject<McVersion>(vertext);
            List<hungry_launcher.utils.Version> allver = versions.versions;
            List<hungry_launcher.utils.Version> release = new List<hungry_launcher.utils.Version>();

            foreach (var item in allver)
            {
                if (item.type == "release")
                {
                    release.Add(item);
                }
            }
            return release.ToArray();
        }

        public class Version
        {
            public string id { get; set; }
            public string time { get; set; }
            public string releaseTime { get; set; }
            public string type { get; set; }
        }

        public class McVersion
        {
            public List<Version> versions { get; set; }
        }

        public static void getver(string ver, string mdir)
        {
            string verjson = "http://s3.amazonaws.com/Minecraft.Download/versions/" + ver + "/" + ver + ".json";
            string verget = "http://s3.amazonaws.com/Minecraft.Download/versions/" + ver + "/" + ver + ".jar";
            WebClient jsondown = new WebClient();
            WebClient verdown = new WebClient();
            System.IO.Directory.CreateDirectory(mdir + "\\versions\\" + ver);
            Directory.SetCurrentDirectory(mdir + "\\versions\\" + ver);
            jsondown.DownloadFile(verjson, ver + ".json");
            verdown.DownloadFile(verget, ver + ".jar");
        }

        public class Natives
        {
            public string windows { get; set; }
        }
        public class Os
        {
            public string name { get; set; }
        }

        public class Rule
        {
            public string action { get; set; }
            public Os os { get; set; }
        }
        public class Extract
        {
            public List<string> exclude { get; set; }
        }
        public class Library
        {
            public string name { get; set; }
            public string url { get; set; }
            public Extract extract { get; set; }
            public List<Rule> rules { get; set; }
            public Natives natives { get; set; }
        }

        public class Libraries
        {
            public string id { get; set; }
            public string type { get; set; }
            public string mainClass { get; set; }
            public string minecraftArguments { get; set; }
            public List<Library> libraries { get; set; }
        }

        public static string donwlibs(string vers, string mdir)
        {
            Libraries libs = JsonConvert.DeserializeObject<Libraries>(File.ReadAllText(mdir + "\\versions\\" + vers + "\\" + vers + ".json"));

            string cp = "";

            mdir = mdir + "\\libraries\\";

            if (Directory.Exists(mdir + "natives"))
            {
                Directory.Delete(mdir + "natives", true);
            }
            else
            {
                Directory.CreateDirectory(mdir + "natives");
            }

            foreach (var item in libs.libraries)
            {
                bool osx = false;
                if (item.rules != null)
                {
                    foreach (var rul in item.rules)
                    {
                        if ((rul.action != null) && (rul.action == "allow"))
                        {
                            if ((rul.os != null) && (rul.os.name == "osx"))
                            {
                                osx = true;
                            }

                        }
                    }
                }
                if (osx == true)
                {
                    continue;
                }

                string libr = "";
                string url = "";
                string fname = "";
                string forge = "";
                string chname = "";
                int j = 0;

                for (int i = 0; i < item.name.Length; i++)
                {
                    if (item.name[i] == ':')
                    {
                        libr = libr + "\\";
                        i++;
                        j = i;
                        break;
                    }
                    if (item.name[i] == '.')
                    {
                        libr = libr + "\\";
                    }
                    else
                    {
                        libr = libr + item.name[i];
                    }
                }

                for (int k = j; k < item.name.Length; k++)
                {
                    if (item.name[k] == ':')
                    {
                        fname = fname + "-";
                        libr = libr + "\\";
                    }
                    else
                    {
                        fname = fname + item.name[k];
                        libr = libr + item.name[k];
                    }
                }

                if ((item.natives != null) && (item.natives.windows != null))
                {
                    fname = fname + "-natives-windows";
                    if (item.natives.windows.Contains("${arch}"))
                    {
                        bool is64bit = System.Environment.Is64BitOperatingSystem;
                        if (is64bit == true)
                        {
                            fname = fname + "-64";
                        }
                        else
                        {
                            fname = fname + "-32";
                        }
                    }
                }
                if (item.name.Contains("forge"))
                {
                    forge = fname + ".jar";
                    fname = fname + "-universal";
                    chname = forge;
                }
                else
                {
                    chname = fname + ".jar";
                }
                fname = fname + ".jar";
                url = libr + '/' + fname;
                url = url.Replace("\\", "/");
                bool fexist = File.Exists(mdir + libr + "\\" + chname);

                if (fexist == false)
                {
                    try
                    {
                        if ((item.natives == null) && (item.url == null))
                        {
                            string getlib = "https://libraries.minecraft.net/" + url;
                            WebClient libdown = new WebClient();
                            System.IO.Directory.CreateDirectory(mdir + libr);
                            libdown.DownloadFile(getlib, mdir + libr + "\\" + Path.GetFileName(getlib));

                        }
                        else if ((item.natives != null) && (item.natives.windows != null) && (item.url == null))
                        {
                            string getlib = "https://libraries.minecraft.net/" + url;
                            WebClient libdown = new WebClient();
                            System.IO.Directory.CreateDirectory(mdir + libr);
                            libdown.DownloadFile(getlib, mdir + libr + "\\" + Path.GetFileName(getlib));
                        }
                        else if (item.url != null)
                        {
                            string getlib = "";
                            if (item.name.Contains("scala"))
                            {
                                getlib = "http://repo1.maven.org/maven2/" + url;
                            }
                            else
                            {
                                getlib = item.url + url;
                            }
                            WebClient libdown = new WebClient();
                            System.IO.Directory.CreateDirectory(mdir + libr);
                            libdown.DownloadFile(getlib, mdir + libr + "\\" + Path.GetFileName(getlib));
                            if (item.name.Contains("forge"))
                            {
                                File.Move(mdir + libr + "\\" + Path.GetFileName(getlib), mdir + libr + "\\" + forge);
                            }
                        }
                    }

                    catch (System.Net.WebException e)
                    {
                        MessageBox.Show("Cant download file " + fname);
                    }
                }
                if (item.name.Contains("forge"))
                {
                    cp = cp + mdir + libr + "\\" + forge + ";";
                }
                else
                {
                    cp = cp + mdir + libr + "\\" + fname + ";";
                }
                if (((item.name.Contains("org.lwjgl.lwjgl:lwjgl-platform")) && (item.natives.windows != null)) || ((item.name.Contains("net.java.jinput:jinput-platform")) && (item.natives.windows != null)))
                {
                    string zipPath = mdir + libr + "\\" + fname;
                    string extractPath = mdir + "natives";
                    ZipFile.ExtractToDirectory(zipPath, extractPath);
                    if (Directory.Exists(extractPath + "\\META-INF"))
                    {
                        Directory.Delete(extractPath + "\\META-INF", true);
                    }
                    if (!cp.Contains(" -Djava.library.path="))
                    {
                        cp = " -Djava.library.path=" + extractPath + " -cp " + cp;
                    }
                }
            }
            mdir = mdir.Replace("libraries", "versions");
            cp = cp + mdir + vers + "\\" + vers + ".jar";
            cp = cp + " " + libs.mainClass + " " + libs.minecraftArguments;
            return cp;
        }
    }
}