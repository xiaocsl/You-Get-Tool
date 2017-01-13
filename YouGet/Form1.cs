using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace YouGet
{
    public partial class Form1 : Form
    {
        string[] args = null;
        public Form1(string[] args)
        {
            InitializeComponent();
            this.args = args;
        }

        public delegate void MyInvoke(string str);

        private void btnTest_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(RunYouGet));
            thread.Start("http://v.youku.com/v_show/id_XMjIwNDQ0NTY0OA==.html?f=29183671&from=y1.3-idx-beta-1519-23042.223465.1-1"); 
        }






        private void RunYouGet(object arg) {
            string url = (string)arg;

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            //p.StartInfo.Arguments = "cd /d \"" + Application.StartupPath + "\"";
            p.Start();//启动程序

            p.StandardInput.WriteLine("cd /d \"" + Application.StartupPath + "\"");

            //向cmd窗口发送输入信息
            p.StandardInput.WriteLine("you-get -p vlc " + url + "&exit");

            p.StandardInput.AutoFlush = true;
            //p.StandardInput.WriteLine("exit");
            //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
            //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令


            bool canOutPut = false;

            //获取cmd窗口的输出信息
            while (!p.HasExited)
            {
                string output = p.StandardOutput.ReadLine();

                

                if(!canOutPut){
                    if(output.IndexOf(url) != -1){
                        canOutPut = true;
                    }
                }else{
                    MyInvoke mi = new MyInvoke(SetTxt);
                    BeginInvoke(mi, output);
                }


                
            }

            MyInvoke endmi = new MyInvoke(SetTxt);
            BeginInvoke(endmi, "------------------------------------------------割------------------------------------------------\r\n");

            p.WaitForExit();//等待程序执行完退出进程
            p.Close();


        }


        public void SetTxt(string str)
        {
            txtTest.AppendText(str + "\r\n");
        }


        
        private void Form1_Load(object sender, EventArgs e)
        {
            if (this.args.Length != 0)
            {
                Thread thread = new Thread(new ParameterizedThreadStart(RunYouGet));
                thread.Start(this.args[0].Substring(19));
            }
        }






        







        //WM_COPYDATA消息所要求的数据结构
        public struct CopyDataStruct
        {
            public IntPtr dwData;
            public int cbData;

            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        private const int WM_COPYDATA = 0x004A;
        //接收第二个窗口传来的消息
        protected override void WndProc(ref System.Windows.Forms.Message e)
        {
            if (e.Msg == WM_COPYDATA)
            {
                CopyDataStruct cds = (CopyDataStruct)e.GetLParam(typeof(CopyDataStruct));
                string msgStr = cds.lpData.ToString();

                if (this.args.Length != 0)
                {
                    Thread thread = new Thread(new ParameterizedThreadStart(RunYouGet));
                    thread.Start(msgStr.Substring(19));
                }


            }
            base.WndProc(ref e);
        }



        //清空TextBox
        private void btnClearText_Click(object sender, EventArgs e)
        {
            txtTest.Text = "";
        }



        //添加注册表
        private void btnRegistry_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.ClassesRoot;
            RegistryKey aliixx = key.CreateSubKey("ygtxxx");
            aliixx.SetValue("URL Protocol","");
            RegistryKey command = aliixx.CreateSubKey("shell\\open\\command");
            command.SetValue("","\""+Application.StartupPath + "\\YouGet.exe\" \"%1\"");
        }










        private void frmMain_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                notifyIcon1.Visible = true; //托盘图标隐藏
            }
            if (this.WindowState == FormWindowState.Minimized)//最小化事件
            {
                this.Hide();//最小化时窗体隐藏
            }
        }



        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal; //还原窗体 
            }
        }


        private void frmMain_FormClosing(object sender, FormClosingEventArgs e) //关闭事件
        {
            DialogResult result;
            result = MessageBox.Show("确定退出吗？", "退出", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {

                Application.ExitThread();
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}
