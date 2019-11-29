using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Vlc.DotNet.Forms;

namespace 素材合成
{
    /// <summary>
    /// ScreenWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ScreenWindow
    {

        #region 构造函数
        string _Path;
        Model.ScreenNode screenNode;
        public ScreenWindow(Model.ScreenNode _screenNode)
        {
            InitializeComponent();
            screenNode = _screenNode;
            this.Left = screenNode.ScreenItem.Bounds.X;
            this.Top = screenNode.ScreenItem.Bounds.Y;
            _Path = screenNode.VideoPath;
            VlcPlayer.Playing += VlcPlayer_Playing;
            this.Loaded += ScreenWindow_Loaded;
        }

        private void VlcPlayer_Playing(object sender, Vlc.DotNet.Core.VlcMediaPlayerPlayingEventArgs e)
        {
            var time = VlcPlayer.Length / 1000;

        }

        private void ScreenWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var senderWindow = sender as ScreenWindow;
            senderWindow.Left = screenNode.ScreenItem.Bounds.X;
            senderWindow.Top = screenNode.ScreenItem.Bounds.Y;
            senderWindow.Width = screenNode.ScreenItem.Bounds.Width;
            senderWindow.Height = screenNode.ScreenItem.Bounds.Height;

            new Thread(Play).Start();
            //VlcPlayer.Position = pos;
        }

        void Play()
        {
           // Thread.Sleep(1000);
            this.Dispatcher.BeginInvoke(new Action(() => {
                VlcPlayer.Play(new FileInfo(_Path));
            }));
        }

        #endregion

        private void VlcPlayer_VlcLibDirectoryNeeded(object sender, VlcLibDirectoryNeededEventArgs e)
        {
            var vlcLibDirectory = new DirectoryInfo(System.IO.Path.Combine("libvlc", "win-x86"));//Environment.Is64BitProcess ? "win-x64" : "win-x86"

            e.VlcLibDirectory = vlcLibDirectory;
        }

        private void HideWindow(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Header_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
                if (e.ClickCount == 2)
                {
                    //Max_MouseLeftButtonDown(null, null);
                    e.Handled = true;
                    return;
                }
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    DragMove();
                }
        }
    }
}
