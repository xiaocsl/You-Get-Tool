using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace YouGet
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1(args));





            bool ret;
            System.Threading.Mutex mutex = new System.Threading.Mutex(true, Application.ProductName, out ret);
            if (ret)
            {
                System.Windows.Forms.Application.EnableVisualStyles();   //这两行实现   XP   可视风格   
                System.Windows.Forms.Application.DoEvents();             //这两行实现   XP   可视风格   
                System.Windows.Forms.Application.Run(new Form1(args));
                //   Main   为你程序的主窗体，如果是控制台程序不用这句   
                mutex.ReleaseMutex();
            }
            else
            {

                if (args.Length != 0)
                {
                    doSend(args[0]);
                }

                
                //   提示信息，可以删除。   
                Application.Exit();//退出程序   
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

        public const int WM_COPYDATA = 0x004A;

        //通过窗口的标题来查找窗口的句柄 
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);

        //在DLL库中的发送消息函数
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage
            (
            int hWnd,                         // 目标窗口的句柄  
            int Msg,                          // 在这里是WM_COPYDATA
            int wParam,                       // 第一个消息参数
            ref  CopyDataStruct lParam        // 第二个消息参数
           );

        private static void doSend(string arg)
        {
            //将文本框中的值， 发送给接收端           
            string strURL = arg;
            CopyDataStruct cds;
            cds.dwData = (IntPtr)1; //这里可以传入一些自定义的数据，但只能是4字节整数      
            cds.lpData = strURL;    //消息字符串
            cds.cbData = System.Text.Encoding.Default.GetBytes(strURL).Length + 1;  //注意，这里的长度是按字节来算的
            SendMessage(FindWindow(null, "You-Get Tool"), WM_COPYDATA, 0, ref cds);       // 这里要修改成接收窗口的标题“接收端”
            //this.Close();
        }
    }
}
