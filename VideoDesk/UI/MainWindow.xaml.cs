using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using AnimaPaper.W32Interop;
using Button = System.Windows.Controls.Button;

namespace AnimaPaper.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static string File;
        public static Uri FileMedia;
        public static Window Win2;
        public static MediaElement Media;
      //  public static List<MediaElement> mediaList;
      //  public static List<Grid> gridList;

        public static List<Window> WindowList;
        public static List<Rectangle> ScreenList;
        public static List<Button> ButtonList;


        public static IntPtr Workerw;
        public static IntPtr WorkerwHidden;


        public static MediaPlayer Player; //for thumbnails

        public static bool SoundOrNot;
        public static bool CurrentlyPlaying;
        NotifyIcon ni;
        public MainWindow()
        {
            InitializeComponent();

            Media = null;
            SoundOrNot = false;
            CurrentlyPlaying = false;

            Player = new MediaPlayer { Volume = 0, ScrubbingEnabled = true }; //Used for thumbnails, not yet implemented

            // Tray icon and balloon
            ni = new NotifyIcon();
            ni.Icon = new Icon("./AnimePaper.ico");

            ni.DoubleClick +=
                delegate
                {
                    Show();
                    WindowState = WindowState.Normal;
                };


            /*
             * 
             * Check monitor numbers and put them in differents Arraylist
             * ScreenList is used to get the actual resolution of a screen
             * windowList will be used to create 1 window per monitor *Not yet implemented*
             * ButtonList will be used to create dynamically button in order to select and load a file for each monitor *Not yet implemented*
             */
           // mediaList = new List<MediaElement>();
            WindowList = new List<Window>();
           // gridList = new List<Grid>();
            ScreenList = new List<Rectangle>();
            ButtonList = new List<Button>();
            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                ScreenList.Add(Screen.AllScreens[i].WorkingArea);
            }
            for (int i = 0; i < ScreenList.Count; i++)
            {
                WindowList.Add(new Window());
               // mediaList.Add(new MediaElement());
            }

            for (int i = 0; i < ScreenList.Count; i++)
            {
                ButtonList.Add(new Button());
                ButtonList[i].Name = "Screen" + i;
                ButtonList[i].Content = "Screen " + i;
                
            }
        }


        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                ni.BalloonTipTitle = @"Anima Paper";
                ni.BalloonTipText = @"Anima Paper is minimized";
                ni.Visible = true;
                ni.ShowBalloonTip(250);
                Hide();
            }
            else if (WindowState.Normal == WindowState)
            {
                ni.Visible = false;
            }

            base.OnStateChanged(e);
        }

        public void CloseAll()
        {
            if (Media != null)
            {
                Media.Stop();
                Media = null;
                for (int i = 0; i < WindowList.Count; i++)
                {
                    WindowList[i].Close();
                }

            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {

            ni.Visible = false;

            if (Media != null)
            {
                Media.Stop();
                Media = null;
                for (int i = 0; i < WindowList.Count; i++)
                {
                    //MainWindow.mediaList[i].Stop();
                    //MainWindow.mediaList[i] = null;
                    WindowList[i].Close();
                }

            }
            CloseAll();
            base.OnClosing(e);
        }

        /// <summary>
        /// This is used to create and find the actual worker window.
        /// Program manager is the root of all windows
        /// Icons are drawn on a window
        /// Must attach a new worker to Progman in order to draw behind icons. 
        /// </summary>
        public static void FindWorker()
        {
            IntPtr progman = W32.FindWindow("Progman", null);

            IntPtr result;

            W32.SendMessageTimeout(progman,
                                   0x052C,//user code
                                   new IntPtr(0),
                                   IntPtr.Zero,
                                   W32.SendMessageTimeoutFlags.SMTO_NORMAL,
                                   1000,
                                   out result);

            Workerw = IntPtr.Zero;

            W32.EnumWindows((tophandle, topparamhandle) =>
            {
                IntPtr p = W32.FindWindowEx(tophandle,
                    IntPtr.Zero,
                    "SHELLDLL_DefView",
                    IntPtr.Zero);

                if (p != IntPtr.Zero)
                {
                    Workerw = W32.FindWindowEx(IntPtr.Zero,
                        tophandle,
                        "WorkerW",
                        IntPtr.Zero);
                    
                }

                
                return true;
            }, IntPtr.Zero);

            //
            /*
            W32.EnumWindows(new W32.EnumWindowsProc((tophandle, topparamhandle) =>
            {
                IntPtr p = W32.FindWindowEx(tophandle,
                                            IntPtr.Zero,
                                            "Progman",
                                            IntPtr.Zero);
                    
                if (p != IntPtr.Zero)
                {
                    // Gets the WorkerW Window after the current one.
                    workerwHidden = W32.FindWindowEx(IntPtr.Zero,
                                               tophandle,
                                               "WorkerW",
                                               IntPtr.Zero);

                }

                return true;
            }), IntPtr.Zero);

    */
        }

    }
}
