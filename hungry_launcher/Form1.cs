﻿using System;
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
using System.Security.Cryptography;
using Newtonsoft.Json;


namespace hungry_launcher
{
    public partial class Form1 : Form
    {
        string mdir;
        string mversion;
        string alocmem;
        string[] mver;
        utils.Version[] downver;
        bool console, autoclose, downloading;

        public Form1()
        {
            InitializeComponent();
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
            if (comboBox3.Items.Count > 0)
                button3.Enabled = true;
            else
                button3.Enabled = false;
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
                            string launch = utils.donwlibs(mversion, mdir, false);

                            launch = launch.Replace("${auth_player_name}", a + username + a);
                            launch = launch.Replace("${version_name}", a + mversion + a);
                            launch = launch.Replace("${game_directory}", a + mdir + a);
                            //launch = launch.Replace("${game_assets}", a + mdir + "\\assets" + a);
                            //launch = launch.Replace(" --uuid ${auth_uuid}", "");
                            //launch = launch.Replace(" --accessToken ${auth_access_token}", "");
                            launch = launch.Replace("${user_properties}", "{}");
                            launch = launch.Replace("${user_type}", a + "mojang" + a);

                            launch = memorys + launch;
                            if (console == true)
                            {
                                Process minecraft = new Process();
                                ProcessStartInfo mcstart = new ProcessStartInfo(javahome + "\\bin\\java.exe", launch);
                                minecraft.StartInfo = mcstart;
                                minecraft.Start();
                                int procid = minecraft.Id;
                                if (autoclose == true && checkBox3.Enabled == true)
                                {
                                    while (memory < 300000)
                                    {
                                        System.Diagnostics.Process pr = Process.GetProcessById(procid);
                                        memory = pr.WorkingSet64 / 1024;
                                    }
                                    this.Close();
                                }
                            }
                            else
                            {
                                Process minecraft = new Process();
                                ProcessStartInfo mcstart = new ProcessStartInfo(javahome + "\\bin\\java.exe", launch);
                                minecraft.StartInfo = mcstart;
                                minecraft.Start();
                                int procid = minecraft.Id;
                                if (autoclose == true && checkBox3.Enabled == true)
                                {
                                    while (memory < 300000)
                                    {
                                        System.Diagnostics.Process pr = Process.GetProcessById(procid);
                                        memory = pr.WorkingSet64 / 1024;
                                    }
                                    this.Close();
                                }
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
        private void button2_Click(object sender, EventArgs e)
        {
            string path;
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = mdir;
                dialog.ShowNewFolderButton = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    path = dialog.SelectedPath;
                    mdir = path;
                }
                else
                {
                    path = mdir;
                }
            }
            Properties.Settings.Default.mdir = mdir;
            Properties.Settings.Default.Save();
            comboBox1.Text = null;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            comboBox3.Enabled = false;
            checkBox3.Enabled = false;
            downloading = true;

            backgroundWorker1.RunWorkerAsync();

        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            string combo3text = "";
            bool but3enab = false;
            bool com3enab = false;
            MethodInvoker getValues = new MethodInvoker(delegate()
            {
                combo3text = comboBox3.Text;
                but3enab = button3.Enabled;
                com3enab = comboBox3.Enabled;
            });

            if (this.InvokeRequired)
            {
                this.Invoke(getValues);
            }
            else
            {
                getValues();
            }

            mversion = combo3text;
            if (mversion != null)
            {
                utils.getver(mversion, mdir);
                utils.donwlibs(mversion, mdir, true);
                utils.getassets(mdir);
            }

        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.button3.Enabled = true;
            this.comboBox3.Enabled = true;
            comboBox3.Text = "";
            this.checkBox3.Enabled = true;
            this.downloading = false;
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
            string text = comboBox1.Text;
            mver = utils.mineversions(mdir);
            comboBox1.Items.Clear();
            if (mver != null)
            {
                foreach (Object i in mver)
                {
                    if (downloading == true && i.ToString() == comboBox3.Text) continue;
                    else
                    {
                        comboBox1.Items.Add(i);
                        if (i.ToString() == text) comboBox1.Text = i.ToString();
                    }
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
        static string assetsversion = "";


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
            string vertext = "";
            try
            {
                using (var client = new WebClient())
                using (var checknet = client.OpenRead(jsonurl))
                {
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
            }
            catch
            {
                return null;
            }
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
            //Дописать проверку если существует - окошко bool fexist = File.Exists(mdir + libr + "\\" + chname);
            string verjson = "http://s3.amazonaws.com/Minecraft.Download/versions/" + ver + "/" + ver + ".json";
            string verget = "http://s3.amazonaws.com/Minecraft.Download/versions/" + ver + "/" + ver + ".jar";
            WebClient jsondown = new WebClient();
            WebClient verdown = new WebClient();
            System.IO.Directory.CreateDirectory(mdir + "\\versions\\" + ver);
            Directory.SetCurrentDirectory(mdir + "\\versions\\" + ver);
            bool fexist = File.Exists(mdir + "\\versions\\" + ver);
            if (fexist == true)
            {
                File.Delete(mdir + "\\versions\\" + ver + ".json");
                File.Delete(mdir + "\\versions\\" + ver + ".jar");
            }
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
            public string assets { get; set; }
            public List<Library> libraries { get; set; }
        }

        public static string donwlibs(string vers, string mdir, bool redownload)
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

                if (fexist == false || redownload == true)
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
                    catch
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
                    try
                    {
                        ZipFile.ExtractToDirectory(zipPath, extractPath);
                        if (Directory.Exists(extractPath + "\\META-INF"))
                        {
                            Directory.Delete(extractPath + "\\META-INF", true);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
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
            mdir = mdir.Replace("\\versions", "");
            char a = '"';
            if (libs.assets != null)
            {
                cp = cp.Replace("${assets_index_name}", libs.assets);
                assetsversion = libs.assets;
                cp = cp.Replace("${assets_root}", a + mdir + "\\assets" + a);
            }
            else
            {
                assetsversion = "old";
                cp = cp.Replace("${game_assets}", a + mdir + "\\assets\\virtual\\legacy" + a);
            }
            return cp;
        }

        public class Assets
        {
            public Dictionary<string, Objects> objects { get; set; }
        }
        public class Objects
        {
            public string hash { get; set; }
            public int size { get; set; }
        }

        public static void getassets(string mdir)
        {
            if (assetsversion == "old")
            {
                assetsversion = "0";
            }
            string assetsjson = "";
            string format = "";
            string names = "";
            for (int i = 0; i < assetsversion.Length; i++)
                if (assetsversion[i] != '.') assetsjson = assetsjson + assetsversion[i];
            int version = Convert.ToInt32(assetsjson);

            Assets assets;

            if (version < 172)
            {
                format = assetsjson = "legacy.json";
                Directory.CreateDirectory(mdir + "\\assets\\virtual\\legacy\\indexes\\");
                WebClient assetsjsondown = new WebClient();

                assetsjson = "http://s3.amazonaws.com/Minecraft.Download/indexes/" + assetsjson;
                assetsjsondown.DownloadFile(assetsjson, mdir + "\\assets\\virtual\\legacy\\indexes\\" + format);
                assets = JsonConvert.DeserializeObject<Assets>(File.ReadAllText(mdir + "\\assets\\virtual\\legacy\\indexes\\" + "legacy.json"));
            }
            else
            {
                format = assetsjson = assetsversion + ".json";
                Directory.CreateDirectory(mdir + "\\assets\\indexes\\");

                WebClient assetsjsondown = new WebClient();

                assetsjson = "http://s3.amazonaws.com/Minecraft.Download/indexes/" + assetsjson;
                assetsjsondown.DownloadFile(assetsjson, mdir + "\\assets\\indexes\\" + format);
                assets = JsonConvert.DeserializeObject<Assets>(File.ReadAllText(mdir + "\\assets\\indexes\\" + format));
            }

            foreach (KeyValuePair<string, Objects> i in assets.objects)
            {
                string hash = Convert.ToString(i.Value.hash);
                int size = Convert.ToInt32(i.Value.size);
                bool hashok = false;
                bool fexist = false;
                string fSHA1 = "";

                WebClient assetsdown = new WebClient();
                names = i.Key.ToString();
                if (names.Contains("/"))
                {
                    if (names.LastIndexOf("/") > 0)
                        names = names.Substring(0, names.LastIndexOf("/"));
                    names = names.Replace("/", "\\");
                    names = "\\" + names;
                }
                else
                {
                    names = null;
                }

                if (version < 172)
                {
                    fexist = File.Exists(mdir + "\\assets\\virtual\\legacy" + names + "\\" + i.Key.ToString().Substring(i.Key.ToString().LastIndexOf("/") + 1));
                    if (fexist == true)
                    {
                        using (FileStream stream = File.OpenRead(mdir + "\\assets\\virtual\\legacy" + names + "\\" + i.Key.ToString().Substring(i.Key.ToString().LastIndexOf("/") + 1)))
                        {
                            SHA1Managed sha = new SHA1Managed();
                            byte[] checksum = sha.ComputeHash(stream);
                            fSHA1 = BitConverter.ToString(checksum).Replace("-", string.Empty);
                            fSHA1 = fSHA1.ToLower();
                        }
                        if (fSHA1 == hash) hashok = true;
                    }
                }
                else
                {
                    fexist = File.Exists(mdir + "\\assets\\objects\\" + hash.Substring(0, 2) + "\\" + hash);
                    if (fexist == true)
                    {
                        using (FileStream stream = File.OpenRead(mdir + "\\assets\\objects\\" + hash.Substring(0, 2) + "\\" + hash))
                        {
                            SHA1Managed sha = new SHA1Managed();
                            byte[] checksum = sha.ComputeHash(stream);
                            fSHA1 = BitConverter.ToString(checksum).Replace("-", string.Empty);
                            fSHA1 = fSHA1.ToLower();
                        }
                        if (fSHA1 == hash) hashok = true;
                    }
                }

                if ((fexist == false) || (fexist == true && hashok == false))
                {
                    if (!Directory.Exists(mdir + "\\assets\\vers\\" + names))
                    {
                        Directory.CreateDirectory(mdir + "\\assets\\virtual\\legacy\\" + names);
                    }

                    if (!Directory.Exists(mdir + "\\assets\\objects\\" + hash.Substring(0, 2)))
                    {
                        Directory.CreateDirectory(mdir + "\\assets\\objects\\" + hash.Substring(0, 2));
                    }

                    if (fexist == false)
                    {
                        assetsdown.DownloadFile("http://resources.download.minecraft.net/" + hash.Substring(0, 2) + "/" + hash, mdir + "\\assets\\objects\\" + hash.Substring(0, 2) + "\\" + hash);
                        File.Copy(mdir + "\\assets\\objects\\" + hash.Substring(0, 2) + "\\" + hash, mdir + "\\assets\\virtual\\legacy" + names + "\\" + i.Key.ToString().Substring(i.Key.ToString().LastIndexOf("/") + 1), true);
                    }
                    else
                    {
                        File.Delete(mdir + "\\assets\\objects\\" + hash.Substring(0, 2) + "\\" + hash);
                        File.Delete(mdir + "\\assets\\virtual\\legacy" + names + "\\" + i.Key.ToString().Substring(i.Key.ToString().LastIndexOf("/") + 1));
                        assetsdown.DownloadFile("http://resources.download.minecraft.net/" + hash.Substring(0, 2) + "/" + hash, mdir + "\\assets\\objects\\" + hash.Substring(0, 2) + "\\" + hash);
                        File.Copy(mdir + "\\assets\\objects\\" + hash.Substring(0, 2) + "\\" + hash, mdir + "\\assets\\virtual\\legacy" + names + "\\" + i.Key.ToString().Substring(i.Key.ToString().LastIndexOf("/") + 1), true);
                    }
                }
            }
        }
    }
}