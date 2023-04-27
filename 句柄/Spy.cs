using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace 句柄
{
    static class Spy
    {
        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr hwnd);
        [DllImport("user32.dll")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern int FindWindowEx(IntPtr hwnd1, IntPtr hwnd2, string lpsz1, string lpsz2);
        [DllImport("user32.dll")]
        public static extern int PostMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern void SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, int lParam);

        const int BM_CLICK = 0xF5;
        const int MOUSEEVENTF_LEFTDOWN = 0x2;
        const int MOUSEEVENTF_LEFTUP = 0x4;
        const int WM_LBUTTONDOWN = 0x0201;
        const int WM_LBUTTONUP = 0x0202;
        const int MK_LBUTTON = 0x1;
        const int WM_MY = 0x10001;

        public static bool Send_Click(string AppName, string ClassName, String BtnDeep)
        {
            int App = 0;
            int Swin = 0;
            string AppHex = "";
            //----------------------------------------寻找主窗口
            if (ClassName != "" && AppName != "")
                App = FindWindow(ClassName, AppName);
            else if (ClassName == "")
                App = FindWindow(null, AppName);
            else if (AppName == "")
                App = FindWindow(ClassName, null);
            else
                return false;
            //----------------------------------------寻找子窗口
            if (App > 0)
            {
                SetForegroundWindow((IntPtr)App);
                Swin = FindWindowEx((IntPtr)App, IntPtr.Zero, null, null);
                Regex reg = new Regex(@"\d+");
                MatchCollection mch = reg.Matches(BtnDeep);
                Swin = App;
                if (mch.Count > 0)
                {
                    for (int deep = 0; deep < mch.Count; deep++)
                    {
                        int inx = Convert.ToInt32(mch[deep].ToString());
                        App = Swin;
                        Swin = 0;
                        
                        for (int Finx = 0; Finx < inx; Finx++)
                        {
                            Swin = FindWindowEx((IntPtr)App, (IntPtr)Swin, null, null);
                        }
                    }
                    App = Swin;
                    if (App > 0)
                    {
                        IntPtr ptr = (IntPtr)App;
                        SetForegroundWindow((IntPtr)App);
                        //SendMessage((IntPtr)App, BM_CLICK, (IntPtr)0, 0);
                        //SendMessage((IntPtr)App, BM_CLICK, (IntPtr)0, 0);
                        PostMessage((IntPtr)App, WM_LBUTTONDOWN, MK_LBUTTON, WM_MY);
                        Thread.Sleep(300);
                        PostMessage((IntPtr)App, WM_LBUTTONUP, MK_LBUTTON, WM_MY);
                        //-----------------------------------------------------------
                        AppHex = Convert.ToString(App, 16).ToUpper();
                        //MessageBox.Show(AppHex);
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
