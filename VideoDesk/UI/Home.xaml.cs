using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Interop;
using AnimaPaper.UI.Controls;
using AnimaPaper.W32Interop;

namespace AnimaPaper.UI
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home
    {

        private bool winSeven;

        // For 1 video per monitor *not yet in*


        public Home()
        {
            InitializeComponent();
            PlayButton.IsEnabled = false;
            winSeven = false;
            Each.Visibility = Visibility.Hidden;
        }

        //One monitor or all the same
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            // Set filter for file extension and default file extension 
            fileDialog.Filter = @"All files (*.*)|*.*";


            var result = fileDialog.ShowDialog();
            switch (result)
            {
                case DialogResult.OK:
                    MainWindow.File = fileDialog.FileName;
                    MainWindow.FileMedia = new Uri(fileDialog.FileName);
                    FileSelected.Text = MainWindow.File;
                    PlayButton.IsEnabled = true;
                    break;
                default:
                    FileSelected.Text = "nothing";
                    PlayButton.IsEnabled = false;

                    break;
            }
        }



        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.FindWorker();

            if (winSeven)
            {
                var sev = new Window();
                var windowSev = new WindowInteropHelper(sev).Handle;

                W32.SetParent(windowSev, MainWindow.Workerw);

            }
            for (var i = 0; i < MainWindow.WindowList.Count; i++)
            {
                MainWindow.WindowList[i] = new Window
                {
                    WindowStyle = WindowStyle.None,
                    AllowsTransparency = true,
                    Top = MainWindow.ScreenList[i].Top,
                    Left = MainWindow.ScreenList[i].Left,
                    Width = MainWindow.ScreenList[i].Width,
                    Height = MainWindow.ScreenList[i].Height
                };



                var i1 = i;
                MainWindow.WindowList[i].Initialized += (s, ea) =>
                {

                    var grid = new Grid();
                    MainWindow.WindowList[i1].Content = grid;
                    grid.Children.Add(new SpectrumVisualizer());
                    

                    var windowHandle = new WindowInteropHelper(MainWindow.WindowList[i1]).Handle;
                    W32.SetParent(windowHandle, winSeven ? MainWindow.WorkerwHidden : MainWindow.Workerw);
                };
                MainWindow.WindowList[i].UpdateLayout();

                MainWindow.WindowList[i].Show();
            }

        }

        
        public static void m_MediaEnded(object sender, RoutedEventArgs e)
        {
            MainWindow.Media.Position = new TimeSpan(0, 0, 1);
            //MainWindow.media.Position = TimeSpan.FromSeconds(0);
            MainWindow.Media.Play();
        }
        
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.CurrentlyPlaying)
            {
                MainWindow.Media.Stop();
                MainWindow.Media = null;
                for (var i = 0; i < MainWindow.WindowList.Count; i++)
                {
                    //MainWindow.mediaList[i].Stop();
                    //MainWindow.mediaList[i] = null;
                    //MainWindow.mediaList[i] = new MediaElement();
                    MainWindow.WindowList[i].Close();
                }

                MainWindow.CurrentlyPlaying = false;
            }
        }

        private void Sound_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow.SoundOrNot = true;

            if (MainWindow.CurrentlyPlaying)
                MainWindow.Media.IsMuted = false;
            Sound.IsChecked = true;
            Volume.IsEnabled = true;
        }

        private void Sound_Unchecked(object sender, RoutedEventArgs e)
        {
            MainWindow.SoundOrNot = false;
            if (MainWindow.CurrentlyPlaying)
                MainWindow.Media.IsMuted = true;
            Sound.IsChecked = false;
            Volume.IsEnabled = false;
        }

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MainWindow.CurrentlyPlaying)
                MainWindow.Media.Volume = Volume.Value;
        }

        private void Multiple_Unchecked(object sender, RoutedEventArgs e)
        {
            MonoScreen.IsEnabled = true;

            foreach (var t in MainWindow.ButtonList)
            {
                t.IsEnabled = false;
            }
        }

        private void Multiple_Checked(object sender, RoutedEventArgs e)
        {
            MonoScreen.IsEnabled = false;
            for (var i = 0; i < MainWindow.ButtonList.Count; i++)
            {
                MainWindow.ButtonList[i].IsEnabled = true;
            }

        }

        private void Windows_Checked(object sender, RoutedEventArgs e)
        {
            winSeven = true;
        }

        private void Windows_Unchecked(object sender, RoutedEventArgs e)
        {
            winSeven = false;
        }
    }
}
