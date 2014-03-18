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

namespace hungry_launcher_v0._0._1
{
    public partial class Form1 : Form
    {
        string mdir;
        string mversion;
        string alocmem;
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

            utils.Launch("1","1",mdir,"1","1");
           
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

                            string memorys = " -Xms512M -Xmx{0}";
                            memorys = string.Format(memorys, alocmem);
                            string launch1 = " -Djava.library.path={0}\\libraries\\org\\lwjgl\\lwjgl\\lwjgl-platform\\2.9.0\\natives -cp ";
                            string launch2 = "{0}\\libraries\\net\\sf\\jopt-simple\\jopt-simple\\4.5\\jopt-simple-4.5.jar;{0}\\libraries\\com\\paulscode\\codecjorbis\\20101023\\codecjorbis-20101023.jar;{0}\\libraries\\com\\paulscode\\codecwav\\20101023\\codecwav-20101023.jar;{0}\\libraries\\com\\paulscode\\libraryjavasound\\20101123\\libraryjavasound-20101123.jar;";   //Begin and Mem and Sound  syst
                            string launch3 = "{0}\\libraries\\com\\paulscode\\librarylwjglopenal\\20100824\\librarylwjglopenal-20100824.jar;{0}\\libraries\\com\\paulscode\\soundsystem\\20120107\\soundsystem-20120107.jar;{0}\\libraries\\argo\\argo\\2.25_fixed\\argo-2.25_fixed.jar;{0}\\libraries\\org\\bouncycastle\\bcprov-jdk15on\\1.47\\bcprov-jdk15on-1.47.jar;{0}\\libraries\\com\\google\\guava\\guava\\14.0\\guava-14.0.jar;{0}\\libraries\\org\\apache\\commons\\commons-lang3\\3.1\\commons-lang3-3.1.jar;";               //Sound Syst and argo and guava and apche
                            string launch4 = "{0}\\libraries\\commons-io\\commons-io\\2.4\\commons-io-2.4.jar;{0}\\libraries\\net\\java\\jinput\\jinput\\2.0.5\\jinput-2.0.5.jar;{0}\\libraries\\net\\java\\jutils\\jutils\\1.0.0\\jutils-1.0.0.jar;{0}\\libraries\\com\\google\\code\\gson\\gson\\2.2.2\\gson-2.2.2.jar;{0}\\libraries\\org\\lwjgl\\lwjgl\\lwjgl\\2.9.0\\lwjgl-2.9.0.jar;{0}\\libraries\\org\\lwjgl\\lwjgl\\lwjgl_util\\2.9.0\\lwjgl_util-2.9.0.jar;{0}\\libraries\\net\\java\\jinput\\jinput-platform\\2.0.5\\jinput-platform-2.0.5-natives-windows.jar;{0}\\versions\\{1}\\{1}.jar;";    //LWJGl and GSON and Version
                            string launch5 = "{0}\\libraries\\net\\minecraftforge\\minecraftforge\\9.11.1.965\\minecraftforge-9.11.1.965.jar;{0}\\libraries\\org\\ow2\\asm\\asm-all\\4.1\\asm-all-4.1.jar;{0}\\libraries\\org\\scala-lang\\scala-library\\2.10.2\\scala-library-2.10.2.jar;{0}\\libraries\\org\\scala-lang\\scala-compiler\\2.10.2\\scala-compiler-2.10.2.jar;{0}\\libraries\\com\\mumfrey\\liteloader\\1.6.4\\liteloader-1.6.4.jar;{0}\\libraries\\net\\minecraft\\launchwrapper\\1.8\\launchwrapper-1.8.jar;{0}\\libraries\\lzma\\lzma\\0.0.1\\lzma-0.0.1.jar"; // Forge and Liteloader
                            string launch6 = " net.minecraft.launchwrapper.Launch --username " + a + username + a + " --version 1.6.4" + " --gameDir {0} --assetsDir {0}\\assets --tweakClass com.mumfrey.liteloader.launch.LiteLoaderTweaker --tweakClass cpw.mods.fml.common.launcher.FMLTweaker"; //Main and Other         
                            string launch = memorys + launch1 + launch2 + launch3 + launch4 + launch5 + launch6;
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
                    for (int i = 0; i <= 9; i++)
                    {
                        if (file.Name.Contains("1." + i.ToString()) && (file.DirectoryName.Equals(versions + Truncates(file.Name))))
                        {
                            mver.Add(Truncates(file.Name));
                        }
                    }
                }
                return mver.ToArray();
            }
            else return null;
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


        public static string Launch(string username, string version, string mdir, string vers, string client)
        {
            char a = '"';
            string launch, natives, jopt, argo, bouncy, guava, apache, comonio, gson, asm, scala, wrap, lzma;
            string scalac = null;
            string scalal = null;

            natives = "{0}\\libraries\\org\\lwjgl\\lwjgl\\lwjgl-platform\\";
            natives = String.Format(natives, mdir);                
            DirectoryInfo natpath = new DirectoryInfo(natives);                      //Natives detection
            FileInfo[] nats = natpath.GetFiles("*.jar", SearchOption.AllDirectories);
            foreach (FileInfo file in nats)
            {
                for (int i = 0; i <= 9; i++)
                {
                    for (int j = 0; j <= 9; j++)
                    {
                        for (int k = 0; k <= 9; k++)
                        {
                            if (file.Name.Contains("lwjgl-platform-" + i.ToString() + "." + j.ToString() + "." + k.ToString()) && file.Name.Contains("-natives-windows"))
                            {
                                natives = natives + i.ToString() + "." + j.ToString() + "." + k.ToString() + "\\natives";
                            }
                        }
                    }
                }
            }

            jopt = "{0}\\libraries\\net\\sf\\jopt-simple\\jopt-simple\\";
            jopt = String.Format(jopt, mdir);
            DirectoryInfo joptpath = new DirectoryInfo(jopt);                    //Jopt detection                                                                                        
            FileInfo[] jopts = joptpath.GetFiles("*.jar", SearchOption.AllDirectories);
            foreach (FileInfo file in jopts)
            {
                for (int i = 0; i <= 9; i++)
                {
                    for (int j = 0; j <= 9; j++)
                    {
                        if (file.Name.Contains("jopt-simple-" + i.ToString() + "." + j.ToString()) && file.DirectoryName.Equals(jopt + i.ToString() + "." + j.ToString()))
                            {
                                jopt = file.DirectoryName +"\\"+ file.Name;
                            }
                    }
                }
            }


            argo = "{0}\\libraries\\argo\\argo\\";
            argo = String.Format(argo, mdir);
            DirectoryInfo argopath = new DirectoryInfo(argo);                    //Agro detection                                                                                        
            FileInfo[] argos = argopath.GetFiles("*.jar", SearchOption.AllDirectories);
            foreach (FileInfo file in argos)
            {
                for (int i = 0; i <= 9; i++)
                {
                    for (int j = 0; j <= 9; j++)
                    {
                        if (file.Name.Contains("argo-" + i.ToString() + "." + j.ToString()) && file.DirectoryName.Contains(argo + i.ToString() + "." + j.ToString()))
                        {
                            argo = file.DirectoryName + "\\" + file.Name;
                        }
                    }
                }
            }


            bouncy = "{0}\\libraries\\org\\bouncycastle\\";
            bouncy = String.Format(bouncy, mdir);
            DirectoryInfo bouncypath = new DirectoryInfo(bouncy);                    //Bouncycastle detection                                                                                     
            FileInfo[] bouncys = bouncypath.GetFiles("*.jar", SearchOption.AllDirectories);
            foreach (FileInfo file in bouncys)
            {
                for (int i = 0; i <= 9; i++)
                {
                    for (int j = 0; j <= 9; j++)
                    {
                        if (file.Name.Contains("bcprov-") && file.Name.Contains("jdk") && file.Name.Contains("-" + i.ToString() + "." + j.ToString()) && file.DirectoryName.Contains(bouncy + "bcprov-"))
                        {
                            bouncy = file.DirectoryName + "\\" + file.Name;
                        }
                    }
                }
            }

            guava = "{0}\\libraries\\com\\google\\guava\\guava\\";
            guava = String.Format(guava, mdir);
            DirectoryInfo guavapath = new DirectoryInfo(guava);                    //Guava detection                                                                                     
            FileInfo[] guavas = guavapath.GetFiles("*.jar", SearchOption.AllDirectories);
            foreach (FileInfo file in guavas)
            {
                for (int i = 0; i <= 9; i++)
                {
                    for (int j = 0; j <= 9; j++)
                    {
                        for (int k = 0; k <= 9; k++)
                        {
                            if ((file.Name.Contains("guava-" + i.ToString() + j.ToString() + "." + k.ToString())) && file.DirectoryName.Contains(guava + i.ToString() + j.ToString()))
                            {
                                guava = file.DirectoryName + "\\" + file.Name;
                            }
                        }
                    }
                }
            }


            apache = "{0}\\libraries\\org\\apache\\commons\\commons-lang3\\";
            apache = String.Format(apache, mdir);
            DirectoryInfo apachepath = new DirectoryInfo(apache);                    //Apache detection                                                                                     
            FileInfo[] apaches = apachepath.GetFiles("*.jar", SearchOption.AllDirectories);
            foreach (FileInfo file in apaches)
            {
                for (int i = 0; i <= 9; i++)
                {
                    for (int j = 0; j <= 9; j++)
                    {
                        if (file.Name.Contains("commons-lang3-" + i.ToString() + "." + j.ToString()) && file.DirectoryName.Equals(apache + i.ToString() +"."+ j.ToString()))
                        {
                            apache = file.DirectoryName + "\\" + file.Name;
                        }
                    }
                }
            }

            comonio = "{0}\\libraries\\commons-io\\commons-io\\";
            comonio = String.Format(comonio, mdir);
            DirectoryInfo comoniopath = new DirectoryInfo(comonio);                    //Comonio detection                                                                                     
            FileInfo[] comonios = comoniopath.GetFiles("*.jar", SearchOption.AllDirectories);
            foreach (FileInfo file in comonios)
            {
                for (int i = 0; i <= 9; i++)
                {
                    for (int j = 0; j <= 9; j++)
                    {
                        if (file.Name.Contains("commons-io-" + i.ToString() + "." + j.ToString()) && file.DirectoryName.Equals(comonio + i.ToString() + "." + j.ToString()))
                        {
                            comonio = file.DirectoryName + "\\" + file.Name;
                        }
                    }
                }
            }

            gson = "{0}\\libraries\\com\\google\\code\\gson\\gson\\";
            gson = String.Format(gson, mdir);
            DirectoryInfo gsonpath = new DirectoryInfo(gson);                    //Gson detection                                                                                     
            FileInfo[] gsons = gsonpath.GetFiles("*.jar", SearchOption.AllDirectories);
            foreach (FileInfo file in gsons)
            {
                for (int i = 0; i <= 9; i++)
                {
                    for (int j = 0; j <= 9; j++)
                    {
                        for (int k = 0; k <= 9; k++)
                        {
                            if (file.Name.Contains("gson-" + i.ToString() + "." + j.ToString() + "." + k.ToString()) && file.DirectoryName.Equals(gson + i.ToString() + "." + j.ToString() + "." + k.ToString()))
                            {
                                gson = file.DirectoryName + "\\" + file.Name;
                            }
                        }
                    }
                }
            }

            asm = "{0}\\libraries\\org\\ow2\\asm\\asm-all\\";
            asm = String.Format(asm, mdir);
            DirectoryInfo asmpath = new DirectoryInfo(asm);                    //Asm detection                                                                                     
            FileInfo[] asms = asmpath.GetFiles("*.jar", SearchOption.AllDirectories);
            foreach (FileInfo file in asms)
            {
                for (int i = 0; i <= 9; i++)
                {
                    for (int j = 0; j <= 9; j++)
                    {
                        if (file.Name.Contains("asm-all-" + i.ToString() + "." + j.ToString()) && file.DirectoryName.Equals(asm + i.ToString() + "." + j.ToString()))
                            {
                                asm = file.DirectoryName + "\\" + file.Name;
                            }
                    }
                }
            }

            scala = "{0}\\libraries\\org\\scala-lang\\";
            scala = String.Format(scala, mdir);
            DirectoryInfo scalapath = new DirectoryInfo(scala);                    //SCALA detection                                                                                     
            FileInfo[] scalas = scalapath.GetFiles("*.jar", SearchOption.AllDirectories);
            foreach (FileInfo file in scalas)
            {
                for (int i = 0; i <= 9; i++)
                {
                    for (int j = 0; j <= 10; j++)
                    {
                        if (file.Name.Contains("scala-library-" + i.ToString() + "." + j.ToString()) && file.DirectoryName.Contains(scala + "scala-library\\" + i.ToString() + "." + j.ToString()))
                        {
                            scalal = file.DirectoryName + "\\" + file.Name;
                        }
                        if (file.Name.Contains("scala-compiler-" + i.ToString() + "." + j.ToString()) && file.DirectoryName.Contains(scala + "scala-compiler\\" + i.ToString() + "." + j.ToString()))
                        {
                            scalac = file.DirectoryName + "\\" + file.Name;
                        }
                    }
                }
            }

            wrap = "{0}\\libraries\\net\\minecraft\\launchwrapper\\";
            wrap = String.Format(wrap, mdir);
            DirectoryInfo wrapath = new DirectoryInfo(wrap);                    //Launchwrapper detection                                                                                     
            FileInfo[] wraps = wrapath.GetFiles("*.jar", SearchOption.AllDirectories);
            foreach (FileInfo file in wraps)
            {
                for (int i = 0; i <= 9; i++)
                {
                    for (int j = 0; j <= 9; j++)
                    {
                        if (file.Name.Contains("launchwrapper-" + i.ToString() + "." + j.ToString()) && file.DirectoryName.Equals(wrap + i.ToString() + "." + j.ToString()))
                        {
                            wrap = file.DirectoryName + "\\" + file.Name;
                        }
                    }
                }
            }

            lzma = "{0}\\libraries\\lzma\\lzma\\  0.0.1\\lzma-0.0.1.jar";
            lzma = String.Format(lzma, mdir);
            DirectoryInfo lzmapath = new DirectoryInfo(lzma);                    //lzma detection                                                                                     
            FileInfo[] lzmas = wrapath.GetFiles("*.jar", SearchOption.AllDirectories);
            foreach (FileInfo file in lzmas)
            {
                for (int i = 0; i <= 9; i++)
                {
                    for (int j = 0; j <= 9; j++)
                    {
                        for (int k = 0; k <= 9; k++)
                        {
                            if (file.Name.Contains("lzma-" + i.ToString() + "." + j.ToString() + "." + k.ToString()) && file.DirectoryName.Equals(lzma + i.ToString() + "." + j.ToString() + "." + k.ToString()))
                            {
                                lzma = file.DirectoryName + "\\" + file.Name;
                            }
                        }
                    }
                }
            }


            MessageBox.Show(lzma);

            string launch1 = " -Djava.library.path="+natives+" -cp ";
            string launch2 = jopt+";{0}\\libraries\\com\\paulscode\\codecjorbis\\20101023\\codecjorbis-20101023.jar;{0}\\libraries\\com\\paulscode\\codecwav\\20101023\\codecwav-20101023.jar;{0}\\libraries\\com\\paulscode\\libraryjavasound\\20101123\\libraryjavasound-20101123.jar;";   //Begin and Mem and Sound  syst
            string launch3 = "{0}\\libraries\\com\\paulscode\\librarylwjglopenal\\20100824\\librarylwjglopenal-20100824.jar;{0}\\libraries\\com\\paulscode\\soundsystem\\20120107\\soundsystem-20120107.jar;" + argo + ";" + bouncy + ";" + guava + ";" + apache + ";";               //Sound Syst and argo and guava and apche

            string launch4 =  comonio+";{0}\\libraries\\net\\java\\jinput\\jinput\\2.0.5\\jinput-2.0.5.jar;{0}\\libraries\\net\\java\\jutils\\jutils\\1.0.0\\jutils-1.0.0.jar;"+gson+";{0}\\libraries\\org\\lwjgl\\lwjgl\\lwjgl\\2.9.0\\lwjgl-2.9.0.jar;{0}\\libraries\\org\\lwjgl\\lwjgl\\lwjgl_util\\2.9.0\\lwjgl_util-2.9.0.jar;{0}\\libraries\\net\\java\\jinput\\jinput-platform\\2.0.5\\jinput-platform-2.0.5-natives-windows.jar;{0}\\versions\\{1}\\{1}.jar;";    //LWJGl and GSON and Version
            string launch5 = "{0}\\libraries\\net\\minecraftforge\\minecraftforge\\9.11.1.965\\minecraftforge-9.11.1.965.jar;"+asm+";"+scalal+";"+scalac+";"+wrap+";"+lzma; // Forge and Liteloader
            string launch6 = " net.minecraft.launchwrapper.Launch --username " + a + username + a + " --version {3}" + " --gameDir {0} --assetsDir {0}\\assets --tweakClass cpw.mods.fml.common.launcher.FMLTweaker"; //Main and Other         


            launch = launch1 + launch2 + launch3 + launch4 + launch5 + launch6;
            return null;
        }

    }
}