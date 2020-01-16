using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using SimpleTCP;
using Vlc.DotNet.Core;
using WK.Libraries.BetterFolderBrowserNS;
using 素材合成.Helper;
using 素材合成.Model;

namespace 素材合成
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        VideoPreviewModel _viewModel = new VideoPreviewModel();
        Stopwatch stopwatch = new Stopwatch();
        static string root_path = (System.AppDomain.CurrentDomain.BaseDirectory);
        public MainWindow()
        {
            InitializeComponent();
            //播放器
            VlcPlayer.Playing += MediaPlayer_Playing;

            this.Loaded += MainWindow_Loaded;
            _viewModel.LumaPower = 1 + "";
            _viewModel.LumaPowerBottom = 1 + "";
            _viewModel.LumaPowerTop = "1";
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            是否停止 = false;
            this.DataContext = _viewModel;
            _viewModel.TotalCount = 0;
            _viewModel.IsVisibility = Visibility.Collapsed;
            GoToWhichStep(1);
            GetFontNames();
        }

        ClientModel client;

        #region VLC 初始化
        private void VlcPlayer_VlcLibDirectoryNeeded(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e)
        {
            var vlcLibDirectory = new DirectoryInfo(System.IO.Path.Combine("libvlc", "win-x86"));//Environment.Is64BitProcess ? "win-x64" : "win-x86"

            e.VlcLibDirectory = vlcLibDirectory;

        }
        #endregion

        #region 获取 ImageSource
        private ImageSource SetSource(string fileName)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(fileName);
             
                if (string.IsNullOrEmpty(fileName))
                {
                    return null;
                }
                BitmapImage image = new BitmapImage();

                image.BeginInit();

                image.UriSource = new System.Uri(fileInfo.FullName);

                image.DecodePixelWidth = 200;

                image.EndInit();

                image.Freeze();
                return image;
            }
            catch
            {
                return null;
            }

        }

        private ImageSource SetFullSizeSource(string fileName)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(fileName);
                if (string.IsNullOrEmpty(fileName))
                {
                    return null;
                }
                BitmapImage image = new BitmapImage();

                image.BeginInit();

                image.UriSource = new System.Uri(fileInfo.FullName);

                image.EndInit();

                image.Freeze();
                return image;
            }
            catch
            {
                return null;
            }

        }
        #endregion

        #region 递归获取所有文件
        List<string> GetAllFiles(string rootDirectoryPath)
        {
            List<string> temp = new List<string>();
            string[] newPath = Directory.GetDirectories(rootDirectoryPath);
            if (newPath.Length > 0)
            {
                for (int i = 0; i < newPath.Length; i++)
                {
                    temp.AddRange(GetAllFiles(newPath[i]));
                }
            }
            temp.AddRange(Directory.GetFiles(rootDirectoryPath));
            return temp;
        }
        #endregion

        #region 截取缩略图
        public ImageSource GetPicFromVideo(string VideoName)
        {
            try
            {
                FileInfo fi = new FileInfo(VideoName);
                ImageSource imageSource = null;
       
                string ffmpeg = root_path + "ffmpeg.exe";//ffmpeg执行文件的路径
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(ffmpeg);
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                string imgPath = fi.DirectoryName + "\\img\\";
                string imgFullFileName = imgPath + fi.Name.Replace(fi.Extension, ".jpg");
                int sssss = 0;
                while (File.Exists(imgFullFileName))
                {
                    try
                    {
                        File.Delete(imgFullFileName);
                    }
                    catch
                    {
                    }
                    Thread.Sleep(50);
                    sssss++;
                    if (sssss == 20)
                    {
                        break;
                    }
                }
                if (!Directory.Exists(imgPath))
                {
                    Directory.CreateDirectory(imgPath);
                }
                startInfo.Arguments = $"-ss 00:00:01 -i \"{VideoName}\" -f image2 -y \"{imgFullFileName}\"";
                System.Diagnostics.Process.Start(startInfo);
                int index = 0;
           
                while (imageSource == null)
                {
                    Thread.Sleep(100);
                    imageSource = SetSource(imgFullFileName);

                    index++;
                    if (index == 30)
                    {
                        this.Dispatcher.Invoke(() => {
                            MessageBox.Show($"这个视频有问题:{VideoName}");

                        });
                        return null;
                    }
                }
                return imageSource;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public ImageSource GetPicFromRotateVideo(string VideoName)
        {
            try
            {
                FileInfo fi = new FileInfo(VideoName);
                ImageSource imageSource = null;
                string ffmpeg = root_path + "ffmpeg.exe";//ffmpeg执行文件的路径
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(ffmpeg);
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                string imgPath = fi.DirectoryName + "\\img\\";
                string imgFullFileName = imgPath + fi.Name.Replace(fi.Extension, "_r.jpg");
                if (!Directory.Exists(imgPath))
                {
                    Directory.CreateDirectory(imgPath);
                }
                int sssss = 0;
                while (File.Exists(imgFullFileName))
                {
                    try
                    {
                        File.Delete(imgFullFileName);
                    }
                    catch
                    {
                    }
                    Thread.Sleep(50);
                    sssss++;
                    if (sssss == 20)
                    {
                        break;
                    }
                }
                startInfo.Arguments = $"-ss 00:00:01 -i \"{VideoName}\" -f image2 -y \"{imgFullFileName}\"";
                System.Diagnostics.Process.Start(startInfo);
                int index = 0;
                while (imageSource == null)
                {
                    Thread.Sleep(100);
                    imageSource = SetSource(imgFullFileName);

                    index++;
                    if (index == 30)
                    {
                        this.Dispatcher.Invoke(() => {
                            MessageBox.Show($"这个视频有问题:{VideoName}");

                        });
                        return null;
                    }
                }
                return imageSource;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region 播放器进度条刷新
        private void MediaPlayer_Playing(object sender, Vlc.DotNet.Core.VlcMediaPlayerPlayingEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (IsComplete == true)
                {
                    imgPause.Visibility = Visibility.Visible;
                    imgPlay.Visibility = Visibility.Collapsed;
                    IsJump = false;
                    IsComplete = false;
                    var time = VlcPlayer.Length / 1000;//毫秒数
                    _viewModel.CurrentPos = "00:00:00";
                    _viewModel.PlayTime = time;
                    _viewModel.AllFrame = (int)(VlcPlayer.VlcMediaPlayer.FramesPerSecond * time);
                    _viewModel.Progress = 0;
                    _viewModel.CurrentTime = 0;
                    _viewModel.LastPos = 0;
                    _viewModel.MaxPos = TransTimeSecondIntToString(time);
                    _viewModel.MinPos = "00:00:00";
                    #region 无法播放
                    if (time <= 0)
                    {
                        time = 0;
                        MessageBox.Show(" 该文件已损坏或格式不支持播放！");
                        return;
                    }
                    #endregion
                    MaxTime = time;
                    Thread thMonitorPlayer = new Thread(MediaPlayer_EndReached);
                    thMonitorPlayer.IsBackground = true;
                    thMonitorPlayer.Start();
                }
            });
        }
        long MaxTime = 0;
        bool IsComplete = true;
        double LastPos = 0;
        int SameCount = 0;
        int index = 0;

        public void MediaPlayer_EndReached()
        {
            _viewModel.Progress = 0;
            index = 0;
            while (true)
            {
                if (index + 1 >= MaxTime * 5 - 5)
                {
                    //播放完毕
                    this.Dispatcher.Invoke(() =>
                    {
                        Console.WriteLine("结束" + _viewModel.CurrentTime);
                        _viewModel.Progress = 100;
                        IsComplete = true;
                        Thread.Sleep(200);
                        _viewModel.CurrentPos = TransTimeSecondIntToString((MaxTime));
                        VlcPlayer.Play(CurrentPlayStream);
                        _viewModel.Progress = 0;
                        index = 0;
                        imgPause.Visibility = Visibility.Visible;
                        imgPlay.Visibility = Visibility.Collapsed;
                    });
                }

                this.Dispatcher.Invoke(() =>
                {
                    if (VlcPlayer.IsPlaying)
                    {
                        _viewModel.CurrentTime = index * 0.2f;
                        Console.WriteLine(_viewModel.CurrentTime);
                        _viewModel.CurrentPos = TransTimeSecondIntToString((long)(_viewModel.CurrentTime));
                        _viewModel.CurrentFrame = (int)(VlcPlayer.VlcMediaPlayer.FramesPerSecond * _viewModel.CurrentTime);
                        _viewModel.Progress = _viewModel.CurrentTime / MaxTime * 100;
                        index++;
                    }
                });
                if (IsComplete)
                {
                    return;
                }

                Thread.Sleep(200);
            }
        }


        #endregion

        #region 跳转进度条播放
        bool IsJump = false;
        private void PlaySilder_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            double x = e.GetPosition(playSilder).X;
            double postion = x / playSilder.ActualWidth;
            _viewModel.Progress = x / playSilder.ActualWidth * 100;
            index = (int)(postion * MaxTime * 5);
            VlcPlayer.Position = (float)postion;
            _viewModel.CurrentTime = index * 0.2f;
            _viewModel.CurrentPos = TransTimeSecondIntToString((long)(_viewModel.CurrentTime));
            _viewModel.CurrentFrame = (int)(VlcPlayer.VlcMediaPlayer.FramesPerSecond * _viewModel.CurrentTime);
        }
        #endregion

        #region 音量

        private void Image_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            VolPopup.IsOpen = VolPopup.IsOpen == false ? true : false;
        }

        private void Slider_LostFocus(object sender, RoutedEventArgs e)
        {
            VolPopup.IsOpen = false;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                VlcPlayer.Audio.Volume = (int)(sender as Slider).Value;
            }
            catch
            {
                //VlcHelper.Volume(VlcPlayer, 100);
            }

        }
        private void MenuPopup_MouseLeave(object sender, MouseEventArgs e)
        {
            VolPopup.IsOpen = false;
        }


        #endregion

        #region 暂停与播放
        private void PlayOrPause(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image temp)
            {
                switch (temp.Name)
                {
                    case "imgPlay":
                        VlcPlayer.VlcMediaPlayer.Play();
                        temp.Visibility = Visibility.Collapsed;
                        imgPause.Visibility = Visibility.Visible;
                        break;
                    default:
                        VlcPlayer.VlcMediaPlayer.Pause();
                        temp.Visibility = Visibility.Collapsed;
                        imgPlay.Visibility = Visibility.Visible;
                        break;

                }

            }
        }

        #endregion

        #region 输出到其他屏幕
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                if (btn.DataContext is Model.ScreenNode screenNode)
                {
                    if (string.IsNullOrEmpty(screenNode.VideoPath))
                    {
                        MessageBox.Show("请先选择视频!");
                        return;
                    }
                    ScreenWindow screenWindow = new ScreenWindow(screenNode);

                    screenWindow.Show();
                }
            }

        }
        #endregion

        #region 00:00:00 函数
        string TransTimeSecondIntToString(long second)
        {
            string str = "";
            long hour = second / 3600;
            long min = second % 3600 / 60;
            long sec = second % 60;
            if (hour < 10)
            {
                str += "0" + hour.ToString();
            }
            else
            {
                str += hour.ToString();
            }
            str += ":";
            if (min < 10)
            {
                str += "0" + min.ToString();
            }
            else
            {
                str += min.ToString();
            }
            str += ":";
            if (sec < 10)
            {
                str += "0" + sec.ToString();
            }
            else
            {
                str += sec.ToString();
            }
            return str;
        }

        #endregion

        #region 框架内置函数

        private void ButtonMin_Click(object sender, RoutedEventArgs e)
        {
            var temp = sender as Button;
            switch (temp.Name)
            {
                case "Min":
                    this.WindowState = WindowState.Minimized;
                    break;
                //case "Max":
                //    this.WindowState = WindowState.Maximized;
                //    Max.Visibility = Visibility.Collapsed;
                //    Full.Visibility = Visibility.Visible;
                //    break;
                //case "Full":
                //    this.WindowState = WindowState.Normal;
                //    Full.Visibility = Visibility.Collapsed;
                //    Max.Visibility = Visibility.Visible;
                //    break;
                case "Close":
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var current = Process.GetCurrentProcess();
                        var processes = Process.GetProcesses();

                        foreach (var t in processes)
                        {
                            if (t.ProcessName == current.ProcessName)
                            {
                                t.Kill();
                            }
                        }
                        Environment.Exit(0);
                    }));
                    break;
            }
        }
        #endregion

        #region Popup 控制
        private void BPop_MouseLeave(object sender, MouseEventArgs e)
        {
            VideoQualityPopup.IsOpen = false;
        }

        private void BPop_MouseEnter(object sender, MouseEventArgs e)
        {
            VideoQualityPopup.IsOpen = true;
        }

        private void VideoQualityPopup_MouseEnter(object sender, MouseEventArgs e)
        {
            VideoQualityPopup.IsOpen = true;
        }

        private void VideoQualityPopup_MouseLeave(object sender, MouseEventArgs e)
        {
            VideoQualityPopup.IsOpen = false;
        }
        #endregion

        #region 没用
        Stream CurrentPlayStream;
        string MovieName = "";

        private void 跳转指定帧_Click(object sender, RoutedEventArgs e)
        {
            VlcPlayer.Position = (int.Parse(tbFrame.Text) + 0.0001f) / _viewModel.AllFrame;
            _viewModel.CurrentFrame = int.Parse(tbFrame.Text);
            _viewModel.Progress = VlcPlayer.Position * 100;
            index = (int)(VlcPlayer.Position * MaxTime * 5);
            _viewModel.CurrentTime = index * 0.2f;
            _viewModel.CurrentPos = TransTimeSecondIntToString((long)(_viewModel.CurrentTime));
            if (CurrentFlow != null)
            {
                List<Instruction> tempNodeList = new List<Instruction>();
                foreach (var item in CurrentFlow.Instructions)
                {
                    if (item.T时间 == _viewModel.CurrentPos)
                    {
                        tempNodeList.Add(item);
                    }
                }
                if (tempNodeList.Count > 0)
                {
                    //Thread threadSend = new Thread(SendCommanderInOneSecond);
                    //threadSend.IsBackground = true;
                    //threadSend.Start(tempNodeList);
                }
            }
            //     _viewModel.CurrentCMD = CurrentFlow.Instructions.Where(u => u.T时间 == _viewModel.CurrentPos).FirstOrDefault().Cmd灯光;
        }

        private void PlaySilder帧_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Slider slider)
            {
                _viewModel.CurrentFrame = (int)slider.Value;
                VlcPlayer.Position = (_viewModel.CurrentFrame + 0.0001f) / _viewModel.AllFrame;
                _viewModel.Progress = VlcPlayer.Position * 100;
                index = (int)(VlcPlayer.Position * MaxTime * 5);
                _viewModel.CurrentTime = index * 0.2f;
                _viewModel.CurrentPos = TransTimeSecondIntToString((long)(_viewModel.CurrentTime));

                //     _viewModel.CurrentCMD = CurrentFlow.Instructions.Where(u => u.T时间 == _viewModel.CurrentPos).FirstOrDefault().Cmd灯光;
            }
        }

        private void 跳转指定秒_Click(object sender, RoutedEventArgs e)
        {
            //VlcPlayer.Position = (int.Parse(tbTime.Text) + 0.0001f) / MaxTime;
            //_viewModel.CurrentFrame = (int)(VlcPlayer.Position * _viewModel.AllFrame);
            //_viewModel.Progress = VlcPlayer.Position * 100;
            //index = (int)(VlcPlayer.Position * MaxTime * 5);
            //_viewModel.CurrentTime = index * 0.2f;
            //_viewModel.CurrentPos = TransTimeSecondIntToString((long)(_viewModel.CurrentTime));
            //if (CurrentFlow != null)
            //{
            //    List<Instruction> tempNodeList = new List<Instruction>();
            //    foreach (var item in CurrentFlow.Instructions)
            //    {
            //        if (item.T时间 == _viewModel.CurrentPos)
            //        {
            //            tempNodeList.Add(item);
            //        }
            //    }
            //    if (tempNodeList.Count > 0)
            //    {
            //        //Thread threadSend = new Thread(SendCommanderInOneSecond);
            //        //threadSend.IsBackground = true;
            //        //threadSend.Start(tempNodeList);
            //    }
            //}
            //   _viewModel.CurrentCMD = CurrentFlow.Instructions.Where(u => u.T时间 == _viewModel.CurrentPos).FirstOrDefault().Cmd灯光;
        }

        List<Flow> Flows;
        Flow CurrentFlow;
        private void 重新加载配置文件_Click(object sender, RoutedEventArgs e)
        {
            Flows = ConfigureHelper.GetAllTopicType1();
            foreach (var item in Flows)
            {
                if (item.Movie.Contains(MovieName))
                {
                    CurrentFlow = item;
                    _viewModel.CurrentFlow = item;
                    _viewModel.CommandList = new ObservableCollection<Instruction>(item.Instructions);
                }
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if(sender is  ListBox listBox)
            //{
            //    if(listBox.SelectedItem is PlayListNode playListNode)
            //    {
            //        CurrentPlayStream = new FileStream(playListNode.VideoPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            //        Thread.Sleep(200);
            //        VlcPlayer.Play(CurrentPlayStream);
            //        MovieName = playListNode.VideoPath;
            //    }
            //}
        }


        private void Image_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image image)
            {
                _viewModel.PlayList.Remove(image.DataContext as PlayListNode);
            }
        }

        #endregion


        #region 导入素材

        private void 添加素材_Click(object sender, RoutedEventArgs e)
        {
            是否停止 = false;
            var openFolder = new BetterFolderBrowser();
            openFolder.Title = "选择文件夹(可多选)";
            if (ConfigureHelper.Read("LastestImportPath") != "")
            {
                openFolder.RootFolder = ConfigureHelper.Read("LastestImportPath");
            }


            // Allow multi-selection of folders.
            openFolder.Multiselect = true;

            if (openFolder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] ALL = openFolder.SelectedPath.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                string rootPath = "";
                for (int i = 0; i < ALL.Length - 1; i++)
                {
                    rootPath += ALL[i] + "\\";
                }
                ConfigureHelper.Write("LastestImportPath", rootPath);
                btnAdd.IsEnabled = false;
                _viewModel.IsVisibility = Visibility.Visible;
                string[] selectedFolders = openFolder.SelectedFolders;
                new Thread(OpenFolder).Start(selectedFolders);

            }
        }

        private void OpenFolder(object selectedFolders)
        {
            int CurrentIndex = 0;
           
            foreach (var item in (string[])selectedFolders)
            {
                if(是否停止)
                {
                    break;
                }
                DirectoryInfo di = new DirectoryInfo(item);
                //添加一级节点
                var tempnode = new VideoGroupNode();
                if (_viewModel.VideoGroupNodeList.Count + 1 > _viewModel.MaxId)
                {
                    tempnode.ID = (_viewModel.VideoGroupNodeList.Count + 1) + "";
                    _viewModel.MaxId = _viewModel.VideoGroupNodeList.Count + 1;
                }
                else
                {
                    _viewModel.MaxId += 1;
                    tempnode.ID = _viewModel.MaxId + "";

                }

                tempnode.Index = _viewModel.VideoGroupNodeList.Count;

                this.Dispatcher.Invoke(() =>
                {
                    tempnode.Width = this.Width - 100;
                    _viewModel.VideoGroupNodeList.Add(tempnode);
                });

                CurrentIndex = _viewModel.VideoGroupNodeList.Count - 1;
                if (CurrentIndex == 8)
                {
                    Console.WriteLine(CurrentIndex);
                }
                int BackgroundIndex = 0;

                foreach (var file in di.GetFiles())
                {
                    if (file.Name.ToLower().Contains(".mp4")
                        || file.Name.ToLower().Contains(".avi"))
                    {
                        int Id = 0;
                        if (CheckName(file.Name, ref Id))
                        {
                            var tempPlaylistNode = new PlayListNode();
                            tempPlaylistNode.ID = Id + "";
                            tempPlaylistNode.VideoPath = file.FullName;
                            tempPlaylistNode.VideoImg = GetPicFromVideo(file.FullName);
                            if (BackgroundIndex == 0)
                            {

                                BackgroundIndex++;
                            }
                            if (tempPlaylistNode.VideoImg == null)
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    _viewModel.VideoGroupNodeList.Clear();
                                    btnAdd.IsEnabled = true;
                                    _viewModel.IsVisibility = Visibility.Collapsed;
                                });
                                return;
                            }
                            tempPlaylistNode.Size = file.Length;
                            this.Dispatcher.Invoke(() =>
                            {
                                _viewModel.VideoGroupNodeList[CurrentIndex].VideoNode.Add(tempPlaylistNode);
                            });

                        }
                        else
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                _viewModel.VideoGroupNodeList.Clear();
                                btnAdd.IsEnabled = true;
                                _viewModel.IsVisibility = Visibility.Collapsed;
                                MessageBox.Show($"{file.FullName}命名不规范 需要命名指定为[xxxx_数字.*]");
                            });
                            return;
                        }
                    }
                }
                this.Dispatcher.Invoke(() =>
                {
                    //排序
                    for (int i = 0; i < _viewModel.VideoGroupNodeList[CurrentIndex].VideoNode.Count; i++)
                    {
                        for (int j = i + 1; j < _viewModel.VideoGroupNodeList[CurrentIndex].VideoNode.Count; j++)
                        {
                            if (int.Parse(_viewModel.VideoGroupNodeList[CurrentIndex].VideoNode[i].ID) > int.Parse(_viewModel.VideoGroupNodeList[CurrentIndex].VideoNode[j].ID))
                            {
                                var temp = _viewModel.VideoGroupNodeList[CurrentIndex].VideoNode[i];
                                _viewModel.VideoGroupNodeList[CurrentIndex].VideoNode[i] = _viewModel.VideoGroupNodeList[CurrentIndex].VideoNode[j];
                                _viewModel.VideoGroupNodeList[CurrentIndex].VideoNode[j] = temp;
                            }
                        }
                    }
                });

            }
            _viewModel.TotalCount = 1;
            //更新总数量
            foreach (var item in _viewModel.VideoGroupNodeList)
            {
                _viewModel.TotalCount *= item.VideoNode.Count;
            }
            this.Dispatcher.Invoke(() =>
            {
                btnAdd.IsEnabled = true;
                _viewModel.IsVisibility = Visibility.Collapsed;
            });
        }

        bool CheckName(string input, ref int index)
        {
            input = input.Replace("Do_", "");
            //提取日期的正则表达式
            Regex reg = new Regex(@"(_[0-9]+\.+)");
            MatchCollection temp = reg.Matches(input);
            if (temp.Count > 0)
            {
                index = int.Parse(temp[0].Value.Replace("_", "").Replace(".", ""));
            }
            return temp.Count > 0;

        }

        void 旋转(string VideoPath)
        {
            string RotateOutPutName = root_path + $"temp\\tempRotate_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_Video.mp4";
            string RotateArguments = $" -i {VideoPath} -vf transpose=2 -y  {RotateOutPutName}";
            FileHelper.AppandLog("逆时针旋转90: ffmpeg " + RotateArguments);
            ExecuteCommandCut(RotateArguments, RotateOutPutName);

            File.Delete(VideoPath);
            FileInfo fileInfo = new FileInfo(RotateOutPutName);
            fileInfo.CopyTo(VideoPath);
            fileInfo.Delete();

        }

        void 顺时针旋转(string VideoPath)
        {
            string RotateOutPutName = root_path + $"temp\\tempRotate_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_Video.mp4";
            string RotateArguments = $" -i {VideoPath} -vf transpose=1 -y  {RotateOutPutName}";
            FileHelper.AppandLog("逆时针旋转90: ffmpeg " + RotateArguments);
            ExecuteCommandCut(RotateArguments, RotateOutPutName);
            File.Delete(VideoPath);
            FileInfo fileInfo = new FileInfo(RotateOutPutName);
            fileInfo.CopyTo(VideoPath);
            fileInfo.Delete();
        }

        #endregion

        #region 合成素材

        Queue<string> 预览_待处理队列 = new Queue<string>();

        private void 合成视频_Click(object sender, RoutedEventArgs e)
        {
            是否停止 = false;
            if (string.IsNullOrEmpty(tbOutPutCount.Text))
            {
                MessageBox.Show("请输入需要导出的素材数量！");
                return;
            }
            if (int.Parse(tbOutPutCount.Text) > _viewModel.TotalCount)
            {
                MessageBox.Show($"导出的素材数量不能超过{_viewModel.TotalCount}！");
                tbOutPutCount.Text = _viewModel.TotalCount + "";
                return;
            }
            if (int.Parse(tbOutPutCount.Text) == 0)
            {
                MessageBox.Show($"请输入的素材导出数量！");
                //  tbOutPutCount.Text = _viewModel.TotalCount + "";
                return;
            }
            _viewModel.SavePath = tbOutPut.Text;

            int[] arr = new int[_viewModel.VideoGroupNodeList.Count];
            int num = int.Parse(tbOutPutCount.Text);

            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = _viewModel.VideoGroupNodeList[i].VideoNode.Count;
            }
            List<List<int>> result = returnResult(arr, num);
            new Thread(Comose).Start(result);
            btnOut.IsEnabled = false;
            _viewModel.IsVisibility = Visibility.Visible;
        }

        List<string> ComposePathList = new List<string>();
        int flag = 1;
        int? videoWidth = 0;
        int? videoHeight = 0;
        string time = "";
        bool IsHorizontal = false;
        StringBuilder ComposeName = new StringBuilder();

        void Comose(object arr)
        {
            var temp = arr as List<List<int>>;
            flag = 1;
            for (int i = 0; i < _viewModel.VideoGroupNodeList.Count; i++)
            {
                if (_viewModel.VideoGroupNodeList[i].逆时针旋转90 == true)
                {
                    for (int index = 0; index < _viewModel.VideoGroupNodeList[i].VideoNode.Count; index++)
                    {
                        旋转(_viewModel.VideoGroupNodeList[i].VideoNode[index].VideoPath);
                        _viewModel.VideoGroupNodeList[i].VideoNode[index].VideoImg = null;
                        
                        _viewModel.VideoGroupNodeList[i].VideoNode[index].VideoImg = GetPicFromRotateVideo(_viewModel.VideoGroupNodeList[i].VideoNode[index].VideoPath);
                    }
                }
                if (_viewModel.VideoGroupNodeList[i].顺时针旋转90 == true)
                {
                    for (int index = 0; index < _viewModel.VideoGroupNodeList[i].VideoNode.Count; index++)
                    {
                        顺时针旋转(_viewModel.VideoGroupNodeList[i].VideoNode[index].VideoPath);
                        _viewModel.VideoGroupNodeList[i].VideoNode[index].VideoImg = null;
                        _viewModel.VideoGroupNodeList[i].VideoNode[index].VideoImg = GetPicFromRotateVideo(_viewModel.VideoGroupNodeList[i].VideoNode[index].VideoPath);
                    }
                }
            }
            foreach (var item in temp)
            {
                StringBuilder sb = new StringBuilder();
                _viewModel.TotalSize = 0;
                _viewModel.Size = 0;
                ComposeName.Clear();
                this.Dispatcher.Invoke(() =>
                {
                    _viewModel.PlayList.Clear();
                    for (int i = 0; i < item.Count; i++)
                    {
                        _viewModel.PlayList.Add(_viewModel.VideoGroupNodeList[i].VideoNode[item[i] - 1]);
                    }
                    foreach (var item1 in _viewModel.PlayList)
                    {
                        ComposeName.Append(item1.ID);
                        sb.AppendLine("file '" + item1.VideoPath + "'");
                    }
                });

                if (flag == 1)
                {

                    if (!Directory.Exists("temp\\"))
                    {
                        Directory.CreateDirectory("temp\\");
                    }
                    string OutPutVideoName = root_path + $"temp\\temp_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_Video.mov";

                    FileHelper.SaveFile_Create("fileList.txt", sb.ToString());
                    FileInfo fileInfo = new FileInfo("fileList.txt");

                    string Arguments = $" -f concat -safe 0  -i  {fileInfo.FullName} -map 0:0 -map 0:1 -c copy -y  {OutPutVideoName}";
                    FileHelper.AppandLog("合成命令: ffmpeg " + Arguments);
                    ExecuteCommandCut(Arguments, OutPutVideoName);
                    Thread.Sleep(100);
                    //mov转MP4
                    string OutPutVideoMp4Name = root_path + $"temp\\temp_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_Video.mp4";
                    string ToMP4Arguments = $"-i {OutPutVideoName} -vcodec copy -acodec aac -y  {OutPutVideoMp4Name}";
                    FileHelper.AppandLog("转MP4格式: ffmpeg " + ToMP4Arguments);
                    ExecuteCommandCut(ToMP4Arguments, OutPutVideoMp4Name);
                    Thread.Sleep(100);
                    OutPutVideoName = OutPutVideoMp4Name;
                    GetMovWidthAndHeight(OutPutVideoName, out videoWidth, out videoHeight, out time, out IsHorizontal);
                    if (IsHorizontal)
                    {
                        _viewModel.RowHeight = 140;
                        _viewModel.ColumnWidth = 300;
                    }
                    else
                    {
                        _viewModel.RowHeight = 60;
                        _viewModel.ColumnWidth = 380;
                    }
                    string ScaleOutPutName = root_path + $"temp\\tempScale_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_Video.mp4";
                    string ScaleArguments = "";
                    //转换为标准尺寸
                    if ((videoHeight == 1920 && videoWidth == 1080) || (videoHeight == 1080 && videoWidth == 1920))
                    {

                    }
                    else
                    {
                        if (IsHorizontal)
                        {
                            ScaleArguments = $"-i {OutPutVideoName} -vf scale=1920:1080  -y {ScaleOutPutName}";
                        }
                        else
                        {
                            ScaleArguments = $"-i {OutPutVideoName} -vf scale=1080:1920 -y  {ScaleOutPutName}";
                        }
                        FileHelper.AppandLog("尺寸规整命令: ffmpeg " + ScaleArguments);
                        ExecuteCommandCut(ScaleArguments, ScaleOutPutName);
                        Thread.Sleep(100);
                        OutPutVideoName = ScaleOutPutName;
                    }
                    _viewModel.BackgroundImage = GetPicFromVideo(OutPutVideoName);
                    预览_待处理队列.Enqueue(OutPutVideoName);
                    _viewModel.CurrentIndex = flag;

                }

                flag++;
            }
            this.Dispatcher.Invoke(() =>
            {
                GoToWhichStep(2);
                _viewModel.FillColor = Colors.Black;
                _viewModel.FillColor横转竖 = Colors.Black;
                _viewModel.IsVisibility = Visibility.Collapsed;
                btnOut.IsEnabled = true;
            });
        }
        #endregion

        #region 最终合成

        void ComposeAllVideo()
        {
            int[] arr = new int[_viewModel.VideoGroupNodeList.Count];
            int num = 0;
            this.Dispatcher.Invoke(() => {
                num = int.Parse(tbOutPutCount.Text);
            });
            Thread.Sleep(100);
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = _viewModel.VideoGroupNodeList[i].VideoNode.Count;
            }
            List<List<int>> result = returnResult(arr, num);
            _viewModel.IsVisibility = Visibility.Visible;
            flag = 1;
            foreach (var item in result)
            {

                StringBuilder sb = new StringBuilder();
                ComposeName.Clear();
                this.Dispatcher.Invoke(() =>
                {
                    _viewModel.PlayList.Clear();
                    for (int i = 0; i < item.Count; i++)
                    {
                        _viewModel.PlayList.Add(_viewModel.VideoGroupNodeList[i].VideoNode[item[i] - 1]);
                    }
                    int index = 0;
                    foreach (var item1 in _viewModel.PlayList)
                    {
                        if (index == 0)
                        {


                            if (IsHorizontal)
                            {
                                _viewModel.RowHeight = 140;
                                _viewModel.ColumnWidth = 300;
                            }
                            else
                            {
                                _viewModel.RowHeight = 60;
                                _viewModel.ColumnWidth = 380;
                            }
                            index++;
                        }
                        sb.AppendLine("file '" + item1.VideoPath + "'");
                        ComposeName.Append($"{item1.ID}+");
                    }
                    ComposeName.Remove(ComposeName.Length - 1, 1);
                });
                _viewModel.TotalSize = 1024 * 1024 * 1024;
                _viewModel.Size = 0;
                if (!Directory.Exists("temp\\"))
                {
                    Directory.CreateDirectory("temp\\");
                }
                if(是否停止)
                {
                    return;
                }
                #region 合成
                string ProgressVideoPath = "";
                string OutPutVideoName_合成 = root_path + $"temp\\temp_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_Video.mov";

                FileHelper.SaveFile_Create("fileList.txt", sb.ToString());
                FileInfo fileInfo = new FileInfo("fileList.txt");
                string Arguments = $" -f concat -safe 0  -i  {fileInfo.FullName} -map 0:0 -map 0:1 -c copy -y  {OutPutVideoName_合成}";
                FileHelper.AppandLog("合成命令: ffmpeg " + Arguments);
                ExecuteCommandCut(Arguments, OutPutVideoName_合成);
                Thread.Sleep(100);
                if (是否停止)
                {
                    return;
                }
                //mov转MP4
                string OutPutVideoMp4Name = root_path + $"temp\\temp_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_Video.mp4";
                string ToMP4Arguments = $"-i {OutPutVideoName_合成} -vcodec copy -acodec aac -y  {OutPutVideoMp4Name}";
                FileHelper.AppandLog("转MP4格式: ffmpeg " + ToMP4Arguments);
                ExecuteCommandCut(ToMP4Arguments, OutPutVideoMp4Name);
                Thread.Sleep(100);
                //转换为标准尺寸
                if ((videoWidth == 1920 && videoHeight == 1080) || (videoWidth == 1080 && videoHeight == 1920))
                {

                }
                else
                {
                    string ScaleOutPutName = root_path + $"temp\\tempScale_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_Video.mp4";
                    string ScaleArguments = "";
                    if (IsHorizontal)
                    {
                        ScaleArguments = $"-i {OutPutVideoMp4Name} -vf scale=1920:1080 -y  {ScaleOutPutName}";
                    }
                    else
                    {
                        ScaleArguments = $"-i {OutPutVideoMp4Name} -vf scale=1080:1920 -y  {ScaleOutPutName}";
                    }

                    ExecuteCommandCut(ScaleArguments, ScaleOutPutName);
                    if (是否停止)
                    {
                        return;
                    }
                    Thread.Sleep(100);
                    OutPutVideoMp4Name = ScaleOutPutName;
                    FileHelper.AppandLog("尺寸规整: ffmpeg " + ScaleArguments);
                }

                #endregion

                fileInfo = new FileInfo(OutPutVideoMp4Name);
                _viewModel.TotalSize = fileInfo.Length * 9;
                _viewModel.Size = fileInfo.Length;

                ProgressVideoPath = OutPutVideoMp4Name;
                Random randomer = new Random();
                #region  剪切+填充
                if (_viewModel.Arguments_1_剪切视频 != "")
                {
                    int offsetLeft = 0;
                    int offsetRight = 0;
                    int offsetTop = 0;
                    int offsetBottom = 0;
                    _viewModel.CutSize = fileInfo.Length;


                    if (_viewModel.offsetMin_4 != 0)
                    {
                        offsetLeft = randomer.Next(_viewModel.CutPixel_4 - _viewModel.offsetMin_4, _viewModel.CutPixel_4 + _viewModel.offsetMin_4);
                    }
                    else
                    {
                        offsetLeft = _viewModel.CutPixel_4;
                    }
                    if (_viewModel.offsetMin_2 != 0)
                    {
                        offsetRight = randomer.Next(_viewModel.CutPixel_2 - _viewModel.offsetMin_2, _viewModel.CutPixel_2 + _viewModel.offsetMin_2);
                    }
                    else
                    {
                        offsetRight = _viewModel.CutPixel_2;
                    }
                    if (_viewModel.offsetMin_1 != 0)
                    {
                        offsetTop = randomer.Next(_viewModel.CutPixel_1 - _viewModel.offsetMin_1, _viewModel.CutPixel_1 + _viewModel.offsetMin_1);
                    }
                    else
                    {
                        offsetTop = _viewModel.CutPixel_1;
                    }
                    if (_viewModel.offsetMin_3 != 0)
                    {
                        offsetBottom = randomer.Next(_viewModel.CutPixel_3 - _viewModel.offsetMin_1, _viewModel.CutPixel_3 + _viewModel.offsetMin_1);
                    }
                    else
                    {
                        offsetBottom = _viewModel.CutPixel_3;
                    }


                    //裁剪输出路径
                    string OutPutVideoName_剪切 = root_path + $"temp\\tempCut_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_Video.mp4";
                    //裁剪
                    string CutArguments = $"  -i {ProgressVideoPath} -vf crop=in_w-{offsetLeft}-{offsetRight}:in_h-{offsetTop}-{offsetBottom}:{offsetLeft}:{offsetTop} {OutPutVideoName_剪切} -y ";
                    ExecuteCommandCut(CutArguments, OutPutVideoName_剪切);
                    FileHelper.AppandLog("剪切视频: ffmpeg " + OutPutVideoName_剪切);
                    // ExecuteAsAdmin(CutArguments);
                    ProgressVideoPath = OutPutVideoName_剪切;
                    Thread.Sleep(100);
                    if (是否停止)
                    {
                        return;
                    }
                    //  GetComposeProgress(OutPutVideoName);
                    _viewModel.CutSize = 2 * fileInfo.Length;

                    //填充后输出路径
                    string OutPutPaddVideoName_填充 = root_path + $"temp\\PaddVideo_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_Video.mp4";

                    //填充
                    string PaddArguments;
                    if (IsHorizontal == true)
                    {
                        //横版参数
                        PaddArguments = $" -i {OutPutVideoName_剪切} -vf pad={1920}:{1080}:{offsetLeft}:{offsetTop}:0x{CmdHelper.To10_16_Length_2(_viewModel.FillColor.R)}{CmdHelper.To10_16_Length_2(_viewModel.FillColor.G)}{CmdHelper.To10_16_Length_2(_viewModel.FillColor.B)}  {OutPutPaddVideoName_填充} -y";
                    }
                    else
                    {
                        //  竖版参数
                        PaddArguments = $" -i {OutPutVideoName_剪切} -vf pad={1080}:{1920}:{offsetLeft}:{offsetTop}:0x{CmdHelper.To10_16_Length_2(_viewModel.FillColor.R)}{CmdHelper.To10_16_Length_2(_viewModel.FillColor.G)}{CmdHelper.To10_16_Length_2(_viewModel.FillColor.B)}  {OutPutPaddVideoName_填充} -y";
                    }
                    ExecuteCommandCut(PaddArguments, OutPutPaddVideoName_填充);
                    FileHelper.AppandLog("填充视频: ffmpeg " + PaddArguments);
                    _viewModel.CutSize = 3 * fileInfo.Length;
                    ProgressVideoPath = OutPutPaddVideoName_填充;
                    if (是否停止)
                    {
                        return;
                    }
                    Thread.Sleep(100);

                }


                #endregion

                string OutPutPaddVideoName_竖转横 = "";
                string OutPutPaddVideoName_横转竖 = "";
                #region 竖转横
                if (_viewModel.Arguments_3_竖版转横版 != "")
                {
                    string PaddArguments_竖转横;

                    OutPutPaddVideoName_竖转横 = root_path + $"temp\\tempTransformVertial_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_Video.mp4";
                    //横版参数

                    PaddArguments_竖转横 = $" -i {ProgressVideoPath} -lavfi \"" +
                 $"[0:v]scale=1080*2:1920*2,boxblur=luma_radius=min(h\\,w)/20:luma_power={_viewModel.LumaPower}:chroma_radius=min(cw\\,ch)/20:chroma_power=1[bg];" +
                 $"[0:v]scale=-1:1080[ov];[bg][ov]overlay=(W-w)/2:(H-h)/2,crop=w=1920:h=1080" +
                 $"\" {OutPutPaddVideoName_竖转横} -y";
                    ExecuteCommandCut(PaddArguments_竖转横, OutPutPaddVideoName_竖转横);
                    _viewModel.CutSize = 4 * fileInfo.Length;
                    ProgressVideoPath = OutPutPaddVideoName_竖转横;
                    FileHelper.AppandLog("竖转横: ffmpeg " + PaddArguments_竖转横);
                    if (是否停止)
                    {
                        return;
                    }
                    Thread.Sleep(100);
                }
                #endregion

                #region 横转竖

                if (_viewModel.Arguments_4_横版转竖版裁剪 != "")
                {
                    int offsetLeft_横转竖 = 0;
                    int offsetRight_横转竖 = 0;
                    if (_viewModel.offsetMin_4 != 0)
                    {
                        offsetLeft_横转竖 = randomer.Next(_viewModel.CutPixel_横版左 - _viewModel.offset_横版左, _viewModel.CutPixel_横版左 + _viewModel.offset_横版左);
                    }
                    else
                    {
                        offsetLeft_横转竖 = _viewModel.CutPixel_横版左;
                    }
                    if (_viewModel.offsetMin_2 != 0)
                    {
                        offsetRight_横转竖 = randomer.Next(_viewModel.CutPixel_横版右 - _viewModel.offset_横版右, _viewModel.CutPixel_横版右 + _viewModel.offset_横版右);
                    }
                    else
                    {
                        offsetRight_横转竖 = _viewModel.CutPixel_横版右;
                    }

                    //横版左右裁剪输出路径
                    string OutPutVideoName_横转竖_裁剪 = root_path + $"temp\\tempCutHorizontal_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_Video.mp4";


                    //裁剪
                    string CutArguments_横转竖 = $"  -i {ProgressVideoPath} -vf crop=in_w-{offsetLeft_横转竖}-{offsetRight_横转竖}:in_h-{0}-{0}:{offsetLeft_横转竖}:{0} {OutPutVideoName_横转竖_裁剪} -y ";
                    ExecuteCommandCut(CutArguments_横转竖, OutPutVideoName_横转竖_裁剪);
                    FileHelper.AppandLog("横转竖: ffmpeg " + CutArguments_横转竖);
                    _viewModel.CutSize = 5 * fileInfo.Length;
                    if (是否停止)
                    {
                        return;
                    }
                    Thread.Sleep(100);

                    string PaddArguments_横转竖_填充;
                    OutPutPaddVideoName_横转竖 = root_path + $"temp\\TransformHorizontal_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_Video.mp4";
                    //填充
                    if (IsImage == false)
                    {
                        PaddArguments_横转竖_填充 = $" -i {OutPutVideoName_横转竖_裁剪} -vf \"" +
         $"scale=1080:-2,pad=1080:1920:0:{_viewModel.PadHeight}:0x{CmdHelper.To10_16_Length_2(_viewModel.FillColor横转竖.R)}{CmdHelper.To10_16_Length_2(_viewModel.FillColor横转竖.G)}{CmdHelper.To10_16_Length_2(_viewModel.FillColor横转竖.B)}" +
         $"\" {OutPutPaddVideoName_横转竖} -y";
                    }
                    else
                    {

                        if (_viewModel.是否随机图片)
                        {
                            if (!Directory.Exists("图库"))
                            {
                                Directory.CreateDirectory("图库");
                            }
                            DirectoryInfo directoryInfo = new DirectoryInfo("图库");
                            Random random = new Random();
                            FileInfo[] fileInfos = directoryInfo.GetFiles();
                            if (fileInfos.Length < 2)
                            {
                                MessageBox.Show("图库图片不足,请再图库中添加更多图片!");
                                return;
                            }
                            int index = 0;
                            int flag = 0;
                            flag = random.Next(0, fileInfos.Length - 1);
                            index = 0;
                            foreach (var item123 in fileInfos)
                            {
                                if (index == flag)
                                {
                                    _viewModel.ImageBottomPath = item123.FullName;
                                    _viewModel.FillImageBottom = SetSource(item123.FullName);
                                }
                                index++;
                            }
                            flag = random.Next(0, fileInfos.Length - 1);
                            index = 0;
                            foreach (var item123 in fileInfos)
                            {
                                if (index == flag)
                                {
                                    _viewModel.ImageTopPath = item123.FullName;
                                    _viewModel.FillImageTop = SetSource(item123.FullName);
                                }
                                index++;
                            }
                        }

                        int image2Ypos = 1920 - int.Parse(_viewModel.PadHeight) - 608;
                        int yellow = 608 + int.Parse(_viewModel.PadHeight);
                        PaddArguments_横转竖_填充 = $" -i {OutPutVideoName_横转竖_裁剪} -i {_viewModel.ImageTopPath} -i {_viewModel.ImageBottomPath} -filter_complex \""
                            + $"[0:v]scale=1080:-2,pad=1080:1920:0:{_viewModel.PadHeight}:blue[outvideo];" +
                            $"[1:v]scale=1080:{_viewModel.PadHeight},boxblur={_viewModel.LumaPowerTop}:1:cr=0:ar=0[outup];" +
                            $"[2:v]scale=1080:{image2Ypos},boxblur={_viewModel.LumaPowerBottom}:1:cr=0:ar=0[outdown];" +
                            $"[outvideo][outup]overlay=0:0[output1];" +
                            $"[output1][outdown]overlay=0:{yellow}" +
                            $"\" {OutPutPaddVideoName_横转竖} -y " 
                            ;
                    }

                    ExecuteCommandCut(PaddArguments_横转竖_填充, OutPutPaddVideoName_横转竖);
                    FileHelper.AppandLog("横转竖填充: ffmpeg " + PaddArguments_横转竖_填充);
                    _viewModel.CutSize = 6 * fileInfo.Length;
                    ProgressVideoPath = OutPutPaddVideoName_横转竖;
                    if (是否停止)
                    {
                        return;
                    }
                    Thread.Sleep(100);
                }

                #endregion

                #region 加字幕
                //获取视频语音
                if (_viewModel.Arguments_6_添加字幕 != "")
                {
                    fileInfo = new FileInfo(ProgressVideoPath);
                    string voiceOutPutName = fileInfo.FullName.Replace(fileInfo.Extension, ".wav");
                    string GetVoiceArguments = $"-i {fileInfo.FullName} -f wav {voiceOutPutName} -y ";
                    ExecuteCommandCut(GetVoiceArguments, voiceOutPutName);
                    _viewModel.CutSize = 7 * fileInfo.Length;
                    //语音转字幕
                    Thread.Sleep(100);
                    if (是否停止)
                    {
                        return;
                    }
                    string textOutPutName = fileInfo.FullName.Replace(fileInfo.Extension, ".srt");
                    string appid = ConfigureHelper.Read("讯飞appid");
                    string secret_key = ConfigureHelper.Read("讯飞secret_key");
                    string GetTextArguments = $" {appid} {secret_key} {voiceOutPutName} {textOutPutName}";
                    ExecuteCommandPython(GetTextArguments, textOutPutName);
                    if (是否停止)
                    {
                        return;
                    }
                    Thread.Sleep(100);
                    //烧录字幕至视频文件
                    string VideoOutPutName_加字幕 = root_path + $"temp\\AddSubtitle_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_{flag}.mp4";
                    string SetSubtitleArguments = $"-i {fileInfo.FullName} -vf \"subtitles = '{textOutPutName.Replace("\\", "\\\\").Replace(":", "\\:")}':force_style = 'FontName={_viewModel.FontStyle.Replace(" ", "").Replace("System.Windows.Controls.ComboBoxItem:", "")}," +
                        $"FontSize={_viewModel.FontSize},PrimaryColour=&H" +
                        $"{CmdHelper.To10_16_Length_2(_viewModel.FontColor.B)}" +
                        $"{CmdHelper.To10_16_Length_2(_viewModel.FontColor.G)}{CmdHelper.To10_16_Length_2(_viewModel.FontColor.R)}&," +
                        $"MarginV={_viewModel.TextHeight},BorderStyle=1,Outline={_viewModel.TextWidth.Replace(" ", "").Replace("System.Windows.Controls.ComboBoxItem:", "")},shadow={_viewModel.TextDeepth.Replace(" ", "").Replace("System.Windows.Controls.ComboBoxItem:", "")}'\"" +
                        $" {VideoOutPutName_加字幕} -y ";
                    FileHelper.AppandLog("加字幕: ffmpeg " + SetSubtitleArguments);
                    ExecuteCommandCut(SetSubtitleArguments, VideoOutPutName_加字幕);
                    _viewModel.CutSize = 8 * fileInfo.Length;
                    ProgressVideoPath = VideoOutPutName_加字幕;
                    if (是否停止)
                    {
                        return;
                    }
                    Thread.Sleep(100);
                }

                #endregion

                #region 加水印
             
              
                if (_viewModel.Arguments_7_添加水印 != "")
                {
                    //烧录字幕至视频文件
                    string VideoOutPutName_加水印 = root_path + $"temp\\AddLogo" +
                        $"_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_{flag}.mp4";
                    string SetLogoArguments = $" -i {ProgressVideoPath} -i {_viewModel.水印路径} " +
                        $"-filter_complex overlay={_viewModel.水印坐标X}:{_viewModel.水印坐标Y} {VideoOutPutName_加水印} -y ";
                    ExecuteCommandCut(SetLogoArguments, VideoOutPutName_加水印);
                    ProgressVideoPath = VideoOutPutName_加水印;
                    FileHelper.AppandLog("加水印: ffmpeg " + SetLogoArguments);
                    _viewModel.CutSize = 9 * fileInfo.Length;
                    if (是否停止)
                    {
                        return;
                    }
                    Thread.Sleep(100);
                }

                if (_viewModel.是否静音 == true)
                {
                    string VideoOutPutName_消音 = root_path + $"temp\\RemoveVoice" +
     $"_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_{flag}.mp4";
                    string SetLogoArguments = $" -i {ProgressVideoPath} -an {VideoOutPutName_消音} -y ";
                    ExecuteCommandCut(SetLogoArguments, VideoOutPutName_消音);
                    FileHelper.AppandLog("消音: ffmpeg " + SetLogoArguments);
                    ProgressVideoPath = VideoOutPutName_消音;
                    _viewModel.CutSize = 9 * fileInfo.Length;
                }
                DateTime now = System.DateTime.Now;
                string dateTime = now.Year + "年" + now.Month + "月" + now.Hour + "时" + now.Minute + "分" + now.Second + "秒" + now.Millisecond + "毫秒";
                string VideoOutPutName = $"{_viewModel.SavePath}\\{dateTime}_{ComposeName.ToString()}.mp4";
                File.Copy(ProgressVideoPath, VideoOutPutName);
                if (是否停止)
                {
                    return;
                }
                #endregion

                #region 消音


                #endregion

                if (_viewModel.IsSame == false)
                {
                    _viewModel.FillColor = ColorUtil.GetRandomColor();
                }
                if (_viewModel.IsSame横转竖 == false)
                {
                    _viewModel.FillColor横转竖 = ColorUtil.GetRandomColor();
                }
                _viewModel.CurrentIndex = flag;
                flag++;
            }
         

            this.Dispatcher.Invoke(() => {
                _viewModel.IsVisibility = Visibility.Collapsed;
                myMessgBoxShow("视频编辑完成!");
                // GoToWhichStep(1);
            });

        }
    
        #endregion

        #region 剪切视频
        private void 剪切视频_Click(object sender, RoutedEventArgs e)
        {
            是否停止 = false;
            if (string.IsNullOrEmpty(tbOutPut.Text))
            {
                MessageBox.Show("请选择导出素材的路径！");
                return;
            }
            if (tbOutPut.Text.Contains(" "))
            {
                MessageBox.Show("路径含有非法字符,请重新选择！");
                return;
            }
            btnCut.IsEnabled = false;
            _viewModel.IsVisibility = Visibility.Visible;
            new Thread(CutVideo).Start(tbOutPut.Text);
        }

        void CutVideo(object selectPath)
        {
            flag = 1;
            _viewModel.CurrentIndex = 0;
           
            Random randomer = new Random();
            int Total = 预览_待处理队列.Count;
            for (int i = 0; i < Total; i++)
            {
                string item = 预览_待处理队列.Dequeue();
                int offsetLeft = 0;
                int offsetRight = 0;
                int offsetTop = 0;
                int offsetBottom = 0;
                _viewModel.CutSize = 0;
                _viewModel.Size = 0;
                if (_viewModel.offsetMin_4 != 0)
                {
                    offsetLeft = randomer.Next(_viewModel.CutPixel_4 - _viewModel.offsetMin_4, _viewModel.CutPixel_4 + _viewModel.offsetMin_4);
                }
                else
                {
                    offsetLeft = _viewModel.CutPixel_4;
                }
                if (_viewModel.offsetMin_2 != 0)
                {
                    offsetRight = randomer.Next(_viewModel.CutPixel_2 - _viewModel.offsetMin_2, _viewModel.CutPixel_2 + _viewModel.offsetMin_2);
                }
                else
                {
                    offsetRight = _viewModel.CutPixel_2;
                }
                if (_viewModel.offsetMin_1 != 0)
                {
                    offsetTop = randomer.Next(_viewModel.CutPixel_1 - _viewModel.offsetMin_1, _viewModel.CutPixel_1 + _viewModel.offsetMin_1);
                }
                else
                {
                    offsetTop = _viewModel.CutPixel_1;
                }
                if (_viewModel.offsetMin_3 != 0)
                {
                    offsetBottom = randomer.Next(_viewModel.CutPixel_3 - _viewModel.offsetMin_1, _viewModel.CutPixel_3 + _viewModel.offsetMin_1);
                }
                else
                {
                    offsetBottom = _viewModel.CutPixel_3;
                }


                //裁剪输出路径
                string OutPutVideoName = root_path + $"temp\\tempCut_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_Video.mp4";
                FileInfo fileInfo = new FileInfo(item);
                _viewModel.TotalSize = fileInfo.Length * 2;
                //裁剪
                string CutArguments = $"  -i {fileInfo.FullName} -vf crop=in_w-{offsetLeft}-{offsetRight}:in_h-{offsetTop}-{offsetBottom}:{offsetLeft}:{offsetTop} {OutPutVideoName} -y ";
                FileHelper.AppandLog("剪切: ffmpeg " + CutArguments);
                ExecuteCommandCut(CutArguments, OutPutVideoName);
                // ExecuteAsAdmin(CutArguments);
                Thread.Sleep(100);
                //  GetComposeProgress(OutPutVideoName);
                _viewModel.CutSize = fileInfo.Length;
                //填充后输出路径
                string OutPutPaddVideoName = root_path + $"temp\\PaddVideo_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_Video.mp4";

                //填充
                string PaddArguments;
                if (IsHorizontal == true)
                {
                    //横版参数
                    PaddArguments = $" -i {OutPutVideoName} -vf pad={1920}:{1080}:{offsetLeft}:{offsetTop}:0x{CmdHelper.To10_16_Length_2(_viewModel.FillColor.R)}{CmdHelper.To10_16_Length_2(_viewModel.FillColor.G)}{CmdHelper.To10_16_Length_2(_viewModel.FillColor.B)}  {OutPutPaddVideoName} -y";
                }
                else
                {
                    //  竖版参数
                    PaddArguments = $" -i {OutPutVideoName} -vf pad={1080}:{1920}:{offsetLeft}:{offsetTop}:0x{CmdHelper.To10_16_Length_2(_viewModel.FillColor.R)}{CmdHelper.To10_16_Length_2(_viewModel.FillColor.G)}{CmdHelper.To10_16_Length_2(_viewModel.FillColor.B)}  {OutPutPaddVideoName} -y";
                }
                ExecuteCommandCut(PaddArguments, OutPutPaddVideoName);
                FileHelper.AppandLog("填充: ffmpeg " + PaddArguments);
                Thread.Sleep(100);


                if (flag == 1)
                {
                    进入预览(OutPutPaddVideoName);
                }
                //返回重新调整参数
                if (_viewModel.IsCancel == true)
                {
                    预览_待处理队列.Enqueue(item);
                    this.Dispatcher.Invoke(() =>
                    {
                        btnCut.IsEnabled = true;
                        GoToWhichStep(2);
                    });
     
                    return;
                }
                //开始处理 或者保存参数 进入下一步
                if (_viewModel.IsConfirm == true)
                {
                    _viewModel.Arguments_1_剪切视频 = "剪切";
                    _viewModel.Arguments_2_填充视频 = "填充";
                    预览_待处理队列.Enqueue(OutPutPaddVideoName);
                    _viewModel.CurrentIndex = flag;
                    flag++;
                    if (_viewModel.IsSame == false)
                    {
                        _viewModel.FillColor = ColorUtil.GetRandomColor();
                    }
                    if (_viewModel.IsSame横转竖 == false)
                    {
                        _viewModel.FillColor横转竖 = ColorUtil.GetRandomColor();
                    }
                }
                //跳过
                if (_viewModel.IsIgnore == true)
                {
                    预览_待处理队列.Enqueue(item);
                    _viewModel.Arguments_1_剪切视频 = "";
                    _viewModel.Arguments_2_填充视频 = "";
                    this.Dispatcher.Invoke(() =>
                    {
                        btnCut.IsEnabled = true;
                        if (IsHorizontal == false)
                        {
                            GoToWhichStep(3);
                            _viewModel.LumaPower = "1";
                            _viewModel.TotalSize = _viewModel.CutSize;
                            InitVlaue();
                        }
                        else
                        {
                            GoToWhichStep(4);
                            _viewModel.LumaPower = "1";
                            _viewModel.TotalSize = _viewModel.CutSize;
                            InitVlaue();
                        }
                    });
                    
                    return;
                }
            }
            _viewModel.IsVisibility = Visibility.Collapsed;
         
            this.Dispatcher.Invoke(() =>
            {
                btnCut.IsEnabled = true;
                if (IsHorizontal == false)
                {
                    GoToWhichStep(3);
                    _viewModel.LumaPower = "1";
                    _viewModel.TotalSize = _viewModel.CutSize;
                    InitVlaue();
                }
                else
                {
                    GoToWhichStep(4);
                    _viewModel.LumaPower = "1";
                    _viewModel.TotalSize = _viewModel.CutSize;
                    InitVlaue();
                }

            });

        }



        #endregion

        #region 竖版转横版


        private void 竖版转横版_Click(object sender, RoutedEventArgs e)
        {
            是否停止 = false;
            btnVeritalToHorizontal.IsEnabled = false;
            _viewModel.IsVisibility = Visibility.Visible;
            new Thread(TransformVideo).Start(tbOutPut.Text);
        }
        void TransformVideo(object selectPath)
        {
            flag = 1;
            _viewModel.CurrentIndex = 0;
            Random randomer = new Random();
            int Total = 预览_待处理队列.Count;
            for (int i = 0; i < Total; i++)
            {
                string item = 预览_待处理队列.Dequeue();

                _viewModel.Size = 0;
                _viewModel.CutSize = 0;

                string PaddArguments;

                string OutPutPaddVideoName = root_path + $"temp\\tempTransformVertial_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_Video.mp4";
                //横版参数
                _viewModel.TotalSize = new FileInfo(item).Length ;
                PaddArguments = $" -i {item} -lavfi \"" +
             $"[0:v]scale=1080*2:1920*2,boxblur=luma_radius=min(h\\,w)/20:luma_power={_viewModel.LumaPower}:chroma_radius=min(cw\\,ch)/20:chroma_power=1[bg];" +
             $"[0:v]scale=-1:1080[ov];[bg][ov]overlay=(W-w)/2:(H-h)/2,crop=w=1920:h=1080" +
             $"\" {OutPutPaddVideoName} -y";
                ExecuteCommandCut(PaddArguments, OutPutPaddVideoName);
                FileHelper.AppandLog("竖转横: ffmpeg " + PaddArguments);
                Thread.Sleep(100);
                if (flag == 1)
                {
                    进入预览(OutPutPaddVideoName);
                }
                //返回重新调整参数
                if (_viewModel.IsCancel == true)
                {
                    预览_待处理队列.Enqueue(item);
                    this.Dispatcher.Invoke(() =>
                    {
                        btnVeritalToHorizontal.IsEnabled = true;
                        GoToWhichStep(3);
                    });
  
                    return;
                }
                //开始处理 或者保存参数 进入下一步
                if (_viewModel.IsConfirm == true)
                {
                    _viewModel.CurrentIndex = flag;
                    flag++;
                    预览_待处理队列.Enqueue(OutPutPaddVideoName);
                    _viewModel.Arguments_3_竖版转横版 = "竖转横";
                }
                //跳过
                if (_viewModel.IsIgnore == true)
                {
                    _viewModel.Arguments_3_竖版转横版 = "";
                    预览_待处理队列.Enqueue(item);
                    this.Dispatcher.Invoke(() =>
                    {
                        btnVeritalToHorizontal.IsEnabled = true;
                        GoToWhichStep(5);
                    });
         
                    return;
                }
  
            }
            this.Dispatcher.Invoke(() =>
            {
                btnVeritalToHorizontal.IsEnabled = true;
                GoToWhichStep(5);
            });

        }



        #endregion

        #region  横版转竖版_
        private void 横版转竖版_Click(object sender, RoutedEventArgs e)
        {
            是否停止 = false;
            btnHorizontalToVertical.IsEnabled = false;
            _viewModel.IsVisibility = Visibility.Visible;
            if (_viewModel.ImageTopPath == "" || _viewModel.ImageBottomPath == "")
            {
                this.Dispatcher.Invoke(() => {
                    MessageBox.Show("请选择填充的图片!");
                        });
                return;
            }
            new Thread(TransformVideoHorizontal).Start(tbOutPut.Text);
        }

        void TransformVideoHorizontal(object selectPath)
        {
            flag = 1;
            _viewModel.CurrentIndex = 0;

            Random randomer = new Random();
            int Total = 预览_待处理队列.Count;
            for (int i = 0; i < Total; i++)
            {
                string item = 预览_待处理队列.Dequeue();
                int offsetLeft = 0;
                int offsetRight = 0;
                _viewModel.Size = 0;
                _viewModel.CutSize = 0;
                if (_viewModel.offsetMin_4 != 0)
                {
                    offsetLeft = randomer.Next(_viewModel.CutPixel_横版左 - _viewModel.offset_横版左, _viewModel.CutPixel_横版左 + _viewModel.offset_横版左);
                }
                else
                {
                    offsetLeft = _viewModel.CutPixel_横版左;
                }
                if (_viewModel.offsetMin_2 != 0)
                {
                    offsetRight = randomer.Next(_viewModel.CutPixel_横版右 - _viewModel.offset_横版右, _viewModel.CutPixel_横版右 + _viewModel.offset_横版右);
                }
                else
                {
                    offsetRight = _viewModel.CutPixel_横版右;
                }

                //横版左右裁剪输出路径
                string OutPutVideoName = root_path + $"temp\\tempCutHorizontal_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_Video.mp4";
                FileInfo fileInfo = new FileInfo(item);
                _viewModel.TotalSize = fileInfo.Length * 2;
                //裁剪
                string CutArguments = $"  -i {item} -vf crop=in_w-{offsetLeft}-{offsetRight}:in_h-{0}-{0}:{offsetLeft}:{0} {OutPutVideoName} -y ";
                FileHelper.AppandLog("横转竖: ffmpeg " + CutArguments);
                ExecuteCommandCut(CutArguments, OutPutVideoName);
                Thread.Sleep(100);
                _viewModel.CutSize = fileInfo.Length;
                string PaddArguments;

                string OutPutPaddVideoName = root_path + $"temp\\TransformHorizontal_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_Video.mp4";
                //填充
                if (IsImage == false)
                {
                    PaddArguments = $" -i {OutPutVideoName} -vf \"" +
     $"scale=1080:-2,pad=1080:1920:0:{_viewModel.PadHeight}:0x{CmdHelper.To10_16_Length_2(_viewModel.FillColor横转竖.R)}{CmdHelper.To10_16_Length_2(_viewModel.FillColor横转竖.G)}{CmdHelper.To10_16_Length_2(_viewModel.FillColor横转竖.B)}" +
     $"\" {OutPutPaddVideoName} -y";
                }
                else
                {
                    if (_viewModel.是否随机图片)
                    {
                        if (!Directory.Exists("图库"))
                        {
                            Directory.CreateDirectory("图库");
                        }
                        DirectoryInfo directoryInfo = new DirectoryInfo("图库");
                        Random random = new Random();
                        FileInfo[] fileInfos = directoryInfo.GetFiles();
                        if (fileInfos.Length < 2)
                        {
                            MessageBox.Show("图库图片不足,请再图库中添加更多图片!");
                            return;
                        }
                        int index = 0;
                        int flag = 0;
                        flag = random.Next(0, fileInfos.Length - 1);
                        index = 0;
                        foreach (var item123 in fileInfos)
                        {
                            if (index == flag)
                            {
                                _viewModel.ImageBottomPath = item123.FullName;
                                _viewModel.FillImageBottom = SetSource(item123.FullName);
                            }
                            index++;
                        }
                        flag = random.Next(0, fileInfos.Length - 1);
                        index = 0;
                        foreach (var item123 in fileInfos)
                        {
                            if (index == flag)
                            {
                                _viewModel.ImageTopPath = item123.FullName;
                                _viewModel.FillImageTop = SetSource(item123.FullName);
                            }
                            index++;
                        }
                    }
                    int image2Ypos = 1920 - int.Parse(_viewModel.PadHeight) - 608;
                    int yellow = 608 + int.Parse(_viewModel.PadHeight);
                    PaddArguments = $" -i {OutPutVideoName} -i {_viewModel.ImageTopPath} -i {_viewModel.ImageBottomPath} -filter_complex \""
                        + $"[0:v]scale=1080:-2,pad=1080:1920:0:{_viewModel.PadHeight}:blue[outvideo];" +
                        $"[1:v]scale=1080:{_viewModel.PadHeight},boxblur={_viewModel.LumaPowerTop}:1:cr=0:ar=0[outup];" +
                        $"[2:v]scale=1080:{image2Ypos},boxblur={_viewModel.LumaPowerBottom}:1:cr=0:ar=0[outdown];" +
                        $"[outvideo][outup]overlay=0:0[output1];" +
                        $"[output1][outdown]overlay=0:{yellow}" +
                        $"\" -y  {OutPutPaddVideoName}"
                        ;
                }
                FileHelper.AppandLog("横转竖填充: ffmpeg " + PaddArguments);
                ExecuteCommandCut(PaddArguments, OutPutPaddVideoName);
                Thread.Sleep(100);


                if (flag == 1)
                {
                    进入预览(OutPutPaddVideoName);
                }
                //返回重新调整参数
                if (_viewModel.IsCancel == true)
                {
                    预览_待处理队列.Enqueue(item);
                    this.Dispatcher.Invoke(() =>
                    {
                        btnHorizontalToVertical.IsEnabled = true;
                        GoToWhichStep(4);
                    });
          
                    return;
                }
                //开始处理 或者保存参数 进入下一步
                if (_viewModel.IsConfirm == true)
                {
                    _viewModel.Arguments_4_横版转竖版裁剪 = "横转竖";
                    _viewModel.Arguments_5_横版转竖版填充 = "横转竖填充";
                    预览_待处理队列.Enqueue(OutPutPaddVideoName);
                    _viewModel.CurrentIndex = flag;
                    flag++;
                    if (_viewModel.IsSame == false)
                    {
                        _viewModel.FillColor = ColorUtil.GetRandomColor();
                    }
                    if (_viewModel.IsSame横转竖 == false)
                    {
                        _viewModel.FillColor横转竖 = ColorUtil.GetRandomColor();
                    }

                }
                //跳过
                if (_viewModel.IsIgnore == true)
                {
                    _viewModel.Arguments_4_横版转竖版裁剪 = "";
                    _viewModel.Arguments_5_横版转竖版填充 = "";
                    预览_待处理队列.Enqueue(item);
                    this.Dispatcher.Invoke(() =>
                    {
                        btnHorizontalToVertical.IsEnabled = true;
                        GoToWhichStep(5);
                    });
  
                    return;
                }
            }
            this.Dispatcher.Invoke(() =>
            {
                btnHorizontalToVertical.IsEnabled = true;
                GoToWhichStep(5);
            });
        }



        #endregion

        #region 添加字幕
        private void 合成字幕_Click(object sender, RoutedEventArgs e)
        {
            是否停止 = false;
            btn合成字幕.IsEnabled = false;
            _viewModel.IsVisibility = Visibility.Visible;
            new Thread(AddText).Start(tbOutPut.Text);

        }
        void AddText(object selectPath)
        {
            flag = 1;

            string time = "";
            _viewModel.CurrentIndex = 0;
            int Total = 预览_待处理队列.Count;
            for (int i = 0; i < Total; i++)
            {
                string item = 预览_待处理队列.Dequeue();
                _viewModel.Size = 0;
                _viewModel.CutSize = 0;
                //获取视频语音
                FileInfo fileInfo = new FileInfo(item);
                _viewModel.TotalSize = 3 * fileInfo.Length;
                string voiceOutPutName = fileInfo.FullName.Replace(fileInfo.Extension, ".wav");
                string GetVoiceArguments = $"-i {fileInfo.FullName} -f wav -y {voiceOutPutName}";
                ExecuteCommandCut(GetVoiceArguments, voiceOutPutName);
                _viewModel.CutSize = fileInfo.Length;
                //语音转字幕
                Thread.Sleep(100);
                string textOutPutName = fileInfo.FullName.Replace(fileInfo.Extension, ".srt");
                string appid = ConfigureHelper.Read("讯飞appid");
                string secret_key = ConfigureHelper.Read("讯飞secret_key");
                string GetTextArguments = $" {appid} {secret_key} {voiceOutPutName} {textOutPutName}";
                ExecuteCommandPython(GetTextArguments, textOutPutName);
                Thread.Sleep(100);
                _viewModel.CutSize = fileInfo.Length;
                //烧录字幕至视频文件
                string VideoOutPutName = root_path + $"temp\\AddSubtitle_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_{flag}.mp4";
                string SetSubtitleArguments = $"-i {fileInfo.FullName} -vf \"subtitles = '{textOutPutName.Replace("\\", "\\\\").Replace(":", "\\:")}':force_style = 'FontName={_viewModel.FontStyle.Replace(" ", "").Replace("System.Windows.Controls.ComboBoxItem:", "")}," +
                    $"FontSize={_viewModel.FontSize},PrimaryColour=&H" +
                    $"{CmdHelper.To10_16_Length_2(_viewModel.FontColor.B)}" +
                    $"{CmdHelper.To10_16_Length_2(_viewModel.FontColor.G)}{CmdHelper.To10_16_Length_2(_viewModel.FontColor.R)}&," +
                    $"MarginV={_viewModel.TextHeight},BorderStyle=1,Outline={_viewModel.TextWidth},shadow={_viewModel.TextDeepth}'\"" +
                    $" -y {VideoOutPutName}";
                ExecuteCommandCut(SetSubtitleArguments, VideoOutPutName);
                Thread.Sleep(100);
                if(flag==1)
                {
                    进入预览(VideoOutPutName);
                }
                //返回重新调整参数
                if(_viewModel.IsCancel==true)
                {
                    预览_待处理队列.Enqueue(item);
                    this.Dispatcher.Invoke(() =>
                    {
                        btn合成字幕.IsEnabled = true;
                        GoToWhichStep(5);
                    });

                    return;
                }
                //开始处理 或者保存参数 进入下一步
                if(_viewModel.IsConfirm==true)
                {
                    _viewModel.Arguments_6_添加字幕 = "加字幕";
                    _viewModel.CurrentIndex = flag;
                    flag++;
                    预览_待处理队列.Enqueue(VideoOutPutName);
                }
                //跳过
                if(_viewModel.IsIgnore==true)
                {
                    _viewModel.Arguments_6_添加字幕 = "";
                    预览_待处理队列.Enqueue(item);
                    this.Dispatcher.Invoke(() =>
                    {
                        btn合成字幕.IsEnabled = true;
                        GoToWhichStep(6);
                    });
          
                    return;
                }
            }
            this.Dispatcher.Invoke(() =>
            {
                btn合成字幕.IsEnabled = true;
                GoToWhichStep(6);
            });
        }
        #endregion

        #region 添加水印
        private void 添加水印_Click(object sender, RoutedEventArgs e)
        {
            是否停止 = false;
            btn添加水印.IsEnabled = false;
            _viewModel.IsVisibility = Visibility.Visible;
            new Thread(AddLogo).Start(tbOutPut.Text);
        }

        void AddLogo(object selectPath)
        {
            flag = 1;

            string time = "";
            _viewModel.CurrentIndex = 0;
            int Total = 预览_待处理队列.Count;
            for (int i = 0; i < Total; i++)
            {
                string item = 预览_待处理队列.Dequeue();
                _viewModel.Size = 0;
                _viewModel.CutSize = 0;
                //获取视频语音
                FileInfo fileInfo = new FileInfo(item);
                _viewModel.TotalSize =  fileInfo.Length;
       
                //烧录字幕至视频文件
                string VideoOutPutName = root_path + $"temp\\AddLogo_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_{flag}.mp4";
                string SetLogoArguments = $" -i {fileInfo.FullName} -i {_viewModel.水印路径} -filter_complex overlay={_viewModel.水印坐标X}:{_viewModel.水印坐标Y} {VideoOutPutName} -y ";
                ExecuteCommandCut(SetLogoArguments, VideoOutPutName);
                Thread.Sleep(100);
                if (flag == 1)
                {
                    进入预览(VideoOutPutName);
                }
                //返回重新调整参数
                if (_viewModel.IsCancel == true)
                {
                    预览_待处理队列.Enqueue(item);
                    this.Dispatcher.Invoke(() =>
                    {
                        btn添加水印.IsEnabled = true;
                        GoToWhichStep(6);
                    });
                   
                    return;
                }
                //开始处理 或者保存参数 进入下一步
                if (_viewModel.IsConfirm == true)
                {
                    _viewModel.Arguments_7_添加水印 = "加水印";
                    _viewModel.CurrentIndex = flag;
                    flag++;
                    预览_待处理队列.Enqueue(VideoOutPutName);
                    this.Dispatcher.Invoke(() =>
                    {
                        btn添加水印.IsEnabled = true;
                        GoToWhichStep(7);
                    });
                   
                }
                //跳过
                if (_viewModel.IsIgnore == true)
                {
                    _viewModel.Arguments_7_添加水印 = "";
                    预览_待处理队列.Enqueue(item);
                    this.Dispatcher.Invoke(() =>
                    {
                        btn添加水印.IsEnabled = true;
                        GoToWhichStep(7);
                    });
                 
                    return;
                }
            }
            this.Dispatcher.Invoke(() =>
            {
                btn添加水印.IsEnabled = true;
                GoToWhichStep(7);
            });
        }
        #endregion

        private void 开始合成_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.IsVisibility = Visibility.Visible;
            是否停止 = false;
            new Thread(ComposeAllVideo).Start();
        }


        public void ExecuteCommandCut(string command, string fileFullPath)
        {
            try
            {
                if (是否停止)
                {
                   
                    return;
                }
                //创建一个进程
                Process pc = new Process();
                pc.StartInfo.FileName = root_path + "ffmpeg.exe";
                pc.StartInfo.Arguments = command;
                pc.StartInfo.UseShellExecute = false;
                pc.StartInfo.RedirectStandardOutput = true;
                pc.StartInfo.RedirectStandardError = true;
                pc.StartInfo.CreateNoWindow = true;
                pc.StartInfo.Verb = "runas";
                //启动进程
                pc.Start();

                //准备读出输出流和错误流
                string outputData = string.Empty;
                string errorData = string.Empty;
                pc.BeginOutputReadLine();
                pc.BeginErrorReadLine();

                pc.OutputDataReceived += (ss, ee) =>
                {
                    GetCutProgress(fileFullPath);
                    if (是否停止)
                    {
                        pc.Close();
                        //关闭进程
                        return;
                    }
                };

                pc.ErrorDataReceived += (ss, ee) =>
                {

                    if (ee.Data == null)
                    {
                        pc.Close();
                        //关闭进程
                    }
                    GetCutProgress(fileFullPath);
                };

                //等待退出
                pc.WaitForExit();

                pc.Close();
                //if (_viewModel.CutSize != 0)
                //{
                //    _viewModel.Size = _viewModel.TotalSize;
                //}

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void GetCutProgress(string fileName)
        {
            try
            {
                if (File.Exists(fileName.ToString()))
                {
                    FileInfo fileInfo = new FileInfo(fileName.ToString());
                    _viewModel.Size = _viewModel.CutSize + fileInfo.Length;
                }
            }
            catch
            {
            }
        }

        public void ExecuteCommandPython(string command, string fileFullPath)
        {
            try
            {
                //创建一个进程
                Process pc = new Process();
                pc.StartInfo.FileName = root_path + "wav2srt.exe";
                pc.StartInfo.Arguments = command;
                pc.StartInfo.UseShellExecute = false;
                pc.StartInfo.RedirectStandardOutput = true;
                pc.StartInfo.RedirectStandardError = true;
                pc.StartInfo.CreateNoWindow = true;
                pc.StartInfo.Verb = "runas";
                //启动进程
                pc.Start();

                //准备读出输出流和错误流
                string outputData = string.Empty;
                string errorData = string.Empty;
                pc.BeginOutputReadLine();
                pc.BeginErrorReadLine();

                pc.OutputDataReceived += (ss, ee) =>
                {
                    GetCutProgress(fileFullPath);
                };

                pc.ErrorDataReceived += (ss, ee) =>
                {

                    if (ee.Data == null)
                    {
                        pc.Close();
                        //关闭进程
                    }
                    GetCutProgress(fileFullPath);
                };

                //等待退出
                pc.WaitForExit();

                pc.Close();
                if (_viewModel.CutSize != 0)
                {
                    _viewModel.Size = _viewModel.TotalSize;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void InitVlaue()
        {
            //_viewModel.CutPixel_1 = 0;
            //_viewModel.CutPixel_2 = 0;
            //_viewModel.CutPixel_3 = 0;
            //_viewModel.CutPixel_4 = 0;
            //_viewModel.offsetMin_1 = 0;
            //_viewModel.offsetMin_2 = 0;
            //_viewModel.offsetMin_3 = 0;
            //_viewModel.offsetMin_4 = 0;
            //_viewModel.LumaPower = 1 + "";
            //_viewModel.LumaPowerBottom = 1 + "";
            //_viewModel.LumaPowerTop = "1";
            //_viewModel.FillColor = Colors.Black;
        }


        /// <summary>
        /// 1合成 2剪切 3竖转横 4 横转竖 5加字幕 6加水印
        /// </summary>
        /// <param name="step"></param>
        void GoToWhichStep(int step)
        {
            _viewModel.IsVisibility = Visibility.Collapsed;
            GridStepOne.Visibility = Visibility.Collapsed;
            GridStepFive.Visibility = Visibility.Collapsed;
            GridStepThree.Visibility = Visibility.Collapsed;
            GridStepFour.Visibility = Visibility.Collapsed;
            GridStepTwo.Visibility = Visibility.Collapsed;
            GridStepSix.Visibility = Visibility.Collapsed;
            GridStepSeven.Visibility = Visibility.Collapsed;
            switch (step+"")
            {
                case "1":
                    GridStepOne.Visibility = Visibility.Visible;
                    _viewModel.Arguments_0_合成视频 = "";
                    _viewModel.Arguments_1_剪切视频 = "";
                    _viewModel.Arguments_2_填充视频 = "";
                    _viewModel.Arguments_3_竖版转横版 = "";
                    _viewModel.Arguments_4_横版转竖版裁剪 = "";
                    _viewModel.Arguments_5_横版转竖版填充 = "";
                    _viewModel.Arguments_6_添加字幕 = "";
                    _viewModel.Arguments_7_添加水印 = "";
                    _viewModel.TotalSize = 0;
                    _viewModel.CurrentIndex = 0;
                    _viewModel.Size = 0;
                    btnAdd.IsEnabled = true;
                    btnCut.IsEnabled = true;
                    btnHorizontalToVertical.IsEnabled = true;
                    btnOut.IsEnabled = true;
                    btnVeritalToHorizontal.IsEnabled = true;
                    btn合成字幕.IsEnabled = true;
                    btn添加水印.IsEnabled = true;
                   
                    if(预览_待处理队列.Count>0)
                    {
                        for (int i = 0; i <= 预览_待处理队列.Count; i++)
                        {
                            预览_待处理队列.Dequeue();
                        }
                    }
                   // _viewModel = new VideoPreviewModel();
                    break;
                case "2":
                    _viewModel.TotalSize = 0;
                    _viewModel.CurrentIndex = 0;
                    _viewModel.Size = 0;
                    GridStepTwo.Visibility = Visibility.Visible;
                    break;
                case "3":
                    _viewModel.TotalSize = 0;
                    _viewModel.CurrentIndex = 0;
                    _viewModel.Size = 0;
                    GridStepThree.Visibility = Visibility.Visible;
                    break;
                case "4":
                    _viewModel.TotalSize = 0;
                    _viewModel.CurrentIndex = 0;
                    _viewModel.Size = 0;
                    GridStepFour.Visibility = Visibility.Visible;
                    break;
                case "5":
                    _viewModel.TotalSize = 0;
                    _viewModel.CurrentIndex = 0;
                    _viewModel.Size = 0;
                    GridStepFive.Visibility = Visibility.Visible;
                    break;
                case "6":
                    _viewModel.TotalSize = 0;
                    _viewModel.CurrentIndex = 0;
                    _viewModel.Size = 0;
                    GridStepSix.Visibility = Visibility.Visible;
                    break;
                case "7":
                    _viewModel.TotalSize = 0;
                    _viewModel.CurrentIndex = 0;
                    _viewModel.Size = 0;
                    GridStepSeven.Visibility = Visibility.Visible;
                    break;
            }
        }



        #region 随机颜色
        public class ColorUtil
        {
            public static Color GetRandomColor()
            {
                Random randomNum_1 = new Random(Guid.NewGuid().GetHashCode());
                System.Threading.Thread.Sleep(randomNum_1.Next(1));
                int int_Red = randomNum_1.Next(255);

                Random randomNum_2 = new Random((int)DateTime.Now.Ticks);
                int int_Green = randomNum_2.Next(255);

                Random randomNum_3 = new Random(Guid.NewGuid().GetHashCode());

                int int_Blue = randomNum_3.Next(255);
                int_Blue = (int_Red + int_Green > 380) ? int_Red + int_Green - 380 : int_Blue;
                int_Blue = (int_Blue > 255) ? 255 : int_Blue;


                return GetDarkerColor(System.Drawing.Color.FromArgb(int_Red, int_Green, int_Blue));
            }

            //获取加深颜色
            public static Color GetDarkerColor(System.Drawing.Color color)
            {
                const int max = 255;
                int increase = new Random(Guid.NewGuid().GetHashCode()).Next(30, 255); //还可以根据需要调整此处的值

                byte a = (byte)Math.Abs(Math.Min(color.A - increase, max));
                byte r = (byte)Math.Abs(Math.Min(color.R - increase, max));
                byte g = (byte)Math.Abs(Math.Min(color.G - increase, max));
                byte b = (byte)Math.Abs(Math.Min(color.B - increase, max));


                return Color.FromArgb(a, r, g, b);
            }
        }
        #endregion

        #region 设置导出路径
        private void 选择导出路径_Click(object sender, MouseButtonEventArgs e)
        {
            var openFolder = new BetterFolderBrowser();
            openFolder.Title = "选择导出路径";
            //   openFolder.RootFolder = "C:\\";

            // Allow multi-selection of folders.
            openFolder.Multiselect = false;

            if (openFolder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tbOutPut.Text = openFolder.SelectedFolder;
            }
        }


        #endregion

        #region 导出规则
        public static List<List<int>> returnResult(int[] arr, int num)
        {
            List<List<int>> allCombination = getAllCombination(arr);
            List<List<int>> existCombination = new List<List<int>>(num);

            int minIndex = indexOfMin(arr);

            if (arr[minIndex] >= num)
            {
                for (int i = 1; i <= num; i++)
                {
                    List<int> insertToexist = new List<int>();

                    foreach (var item in allCombination[0])
                    {
                        insertToexist.Add(i);
                    }
                    allCombination.Remove(insertToexist);
                    existCombination.Add(insertToexist);
                }
            }

            else
            {

                for (int i = 1; i <= arr[minIndex]; i++)
                {
                    List<int> insertToexist = new List<int>();

                    foreach (var item in allCombination[0])
                    {
                        insertToexist.Add(i);
                    }
                    allCombination.Remove(insertToexist);
                    existCombination.Add(insertToexist);
                }

                int needNum = num - existCombination.Count;
                for (int i = 0; i < needNum; i++)
                {
                    int insertOfIndex = findIndexOfMinOverlap(allCombination, existCombination);
                    List<int> insertObject = allCombination.ElementAt(insertOfIndex);
                    allCombination.Remove(insertObject);
                    existCombination.Add(insertObject);
                }

            }

            return existCombination;


        }



        // 求最小值的索引
        public static int indexOfMin(int[] arr)
        {
            int min = arr[0];
            int index = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] < min)
                {
                    min = arr[i];
                    index = i;
                }
            }
            return index;
        }

        public static int indexOfMax(int[] arr)
        {
            int max = arr[0];
            int index = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] > max)
                {
                    max = arr[i];
                    index = i;
                }
            }
            return index;
        }


        // 用笛卡尔积求所有组合
        public static List<List<int>> getAllCombination(int[] arr)
        {
            int num = 1;
            for (int i = 0; i < arr.Length; i++)
            {
                num *= arr[i];
            }

            // 求笛卡尔积
            int total = 1;
            int cIndex = arr.Length - 1;
            int[] counterMap = new int[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                counterMap[i] = 0;
            }
            List<List<int>> rt = new List<List<int>>(num);
            while (cIndex >= 0)
            {
                List<int> element = new List<int>(arr.Length);
                for (int j = 0; j < arr.Length; j++)
                {
                    int[] set = new int[arr[j]];
                    for (int k = 1; k <= arr[j]; k++)
                    {
                        set[k - 1] = k;
                    }
                    if (set != null && set.Length > 0)
                    {
                        element.Add(set[counterMap[j]]);
                    }
                    if (j == arr.Length - 1)
                    {
                        if (set == null || ++counterMap[j] > set.Length - 1)
                        {
                            counterMap[j] = 0;
                            int cidx = j;
                            while (--cidx >= 0)
                            {
                                if (arr[cidx] == 0 || ++counterMap[cidx] > arr[cidx] - 1)
                                {
                                    counterMap[cidx] = 0;
                                    continue;
                                }
                                break;
                            }
                            if (cidx < cIndex)
                            {
                                cIndex = cidx;
                            }
                        }
                    }
                }
                if (element.Count > 0)
                {
                    rt.Add(element);
                }
            }
            return rt;
        }

        public static int findIndexOfMinOverlap(List<List<int>> arr, List<List<int>> existCombination)
        {
            int[] differance = new int[arr.Count];
            for (int i = 0; i < arr.Count; i++)
            {
                int[] eachDifferance = new int[existCombination.Count];
                for (int p = 0; p < existCombination.Count; p++)
                {
                    eachDifferance[p] = 0;
                }
                for (int j = 0; j < existCombination.Count; j++)
                {
                    for (int k = 0; k < arr[i].Count; k++)
                    {
                        if (arr[i][k] == existCombination[j][k])
                        {
                            eachDifferance[j] += 1;
                        }
                    }
                }
                differance[i] = eachDifferance[indexOfMax(eachDifferance)];

            }
            // 新增代码片段

            // 所有已用视频片段
           var  allSet = new HashSet<int>();
            for (int i = 0; i < existCombination.Count; i++)
            {
                for (int j = 0; j < existCombination[i].Count; j++)
                {
                    allSet.Add(existCombination[i][j]);
                }
            }

            for (int i = 0; i < arr.Count(); i++)
            {
                // 待加入集合中未使用的视频片段

                int hasUse = 0;
                for (int j = 0; j < arr[i].Count; j++)
                {
                    if (allSet.Contains(arr[i][j]))
                    {
                        hasUse++;
                    }
                }
                differance[i] = differance[i] + 10000 * hasUse;
            }

            return indexOfMin(differance);

        }

        #endregion

        #region 获取视频宽高

        public void GetMovWidthAndHeight(string videoFilePath, out int? width, out int? height, out string time, out bool IsHorizontal)
        {
            try
            {
                //判断文件是否存在
                if (!File.Exists(videoFilePath))
                {
                    width = null;
                    height = null;
                }
                time = "";
                //执行命令获取该文件的一些信息 
                string ffmpegPath = "ffmpeg.exe";

                string output;
                string error;
                ExecuteCommand("\"" + ffmpegPath + "\"" + " -i " + "\"" + videoFilePath + "\"", out output, out error);
                if (string.IsNullOrEmpty(error))
                {
                    width = null;
                    height = null;
                    time = "";
                }
                //获取时长
                if (error.Replace(" ", "").Contains("Duration:"))
                {
                    time = error.Replace(" ", "").Split(new string[] { "Duration:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(',')[0];
                }

                //通过正则表达式获取信息里面的宽度信息
                Regex regex = new Regex("(\\d{2,4})x(\\d{2,4})", RegexOptions.Compiled);
                Match m = regex.Match(error);
                //if (error.Replace(" ", "").Contains("rotate:90"))
                //{
                //    IsHorizontal = false;
                //}
                //else
                //{
                //    IsHorizontal = true;
                //}

                if (m.Success)
                {
                    width = int.Parse(m.Groups[1].Value);
                    height = int.Parse(m.Groups[2].Value);
                    if(width>height)
                    {
                        IsHorizontal = true;
                    }
                    else
                    {
                        IsHorizontal = false;
                    }
                }
                else
                {
                    IsHorizontal = true;
                    width = null;
                    height = null;
                    time = "";
                }
            }
            catch (Exception)
            {
                IsHorizontal = true;
                width = null;
                height = null;
                time = "";
            }
        }

        public void ExecuteCommand(string command, out string output, out string error)
        {
            try
            {
                //创建一个进程
                Process pc = new Process();
                pc.StartInfo.FileName = command;
                pc.StartInfo.UseShellExecute = false;
                pc.StartInfo.RedirectStandardOutput = true;
                pc.StartInfo.RedirectStandardError = true;
                pc.StartInfo.CreateNoWindow = true;
                pc.StartInfo.Verb = "runas";
                //启动进程
                pc.Start();

                //准备读出输出流和错误流
                string outputData = string.Empty;
                string errorData = string.Empty;
                pc.BeginOutputReadLine();
                pc.BeginErrorReadLine();

                pc.OutputDataReceived += (ss, ee) =>
                {
                    outputData += ee.Data;
                };

                pc.ErrorDataReceived += (ss, ee) =>
                {
                    errorData += ee.Data;
                };

                //等待退出
                pc.WaitForExit();

                //关闭进程
                pc.Close();

                //返回流结果
                output = outputData;
                error = errorData;
            }
            catch (Exception)
            {
                output = null;
                error = null;
            }
        }





        #endregion

        private void 选择导图片路径上_Click(object sender, MouseButtonEventArgs e)
        {
            string path = FileHelper.OpenFile_getPath("");
            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show("请选择有效路径");
            }
            else
            {
                _viewModel.ImageTopPath = path;
                _viewModel.FillImageTop = SetSource(path);
            }

        }

        private void 选择图片路径下_Click(object sender, MouseButtonEventArgs e)
        {
            string path = FileHelper.OpenFile_getPath("");
            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show("请选择有效路径");
            }
            else
            {
                _viewModel.ImageBottomPath = path;
                _viewModel.FillImageBottom = SetSource(path);
            }
        }

        private void 选择水印图片_Click(object sender, MouseButtonEventArgs e)
        {
            string path = FileHelper.OpenFile_getPath("");
            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show("请选择有效路径");
            }
            else
            {
                _viewModel.水印路径 = path;
                _viewModel.水印预览 = SetSource(path);
            }
        }

        private void 随机从图库选择图片_Click_1(object sender, RoutedEventArgs e)
        {
           

        }
        bool IsImage = true;
        private void SwitchChannel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                switch (radioButton.Content + "")
                {
                    case "颜色填充":
                        IsImage = false;
                        gridColorPad.Visibility = Visibility.Visible;
                        gridImagePad.Visibility = Visibility.Collapsed;
                        break;
                    case "图片填充":
                        IsImage = true;
                        gridColorPad.Visibility = Visibility.Collapsed;
                        gridImagePad.Visibility = Visibility.Visible;
                        break;

                }
               
            }
        }



        private void 返回_Click(object sender, RoutedEventArgs e)
        {
            grid预览页.Visibility = Visibility.Collapsed;
            _viewModel.IsCancel = true;
            _viewModel.IsIgnore = false;
            _viewModel.IsConfirm = false;
            IsComplete = true;
            VlcPlayer.Play(Stream.Null);
        }

        private void 开始处理_Click(object sender, RoutedEventArgs e)
        {
            grid预览页.Visibility = Visibility.Collapsed;
            _viewModel.IsCancel = false;
            _viewModel.IsIgnore = false;
            _viewModel.IsConfirm = true;
            IsComplete = true;
            VlcPlayer.Play(Stream.Null);
        }

        private void 跳过_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button btn)
            {
                switch(btn.Tag+"")
                {
                    case "剪切":
                        if (string.IsNullOrEmpty(tbOutPut.Text))
                        {
                            MessageBox.Show("请选择导出素材的路径！");
                            return;
                        }
                        if (tbOutPut.Text.Contains(" "))
                        {
                            MessageBox.Show("路径含有非法字符,请重新选择！");
                            return;
                        }
                        _viewModel.Arguments_1_剪切视频 = "";
                       if(IsHorizontal)
                        {
                            GoToWhichStep(4);
                        }
                       else
                        {
                            GoToWhichStep(3);
                        }
                        break;
                    case "横转竖":
                        if (string.IsNullOrEmpty(tbOutPut.Text))
                        {
                            MessageBox.Show("请选择导出素材的路径！");
                            return;
                        }
                        if (tbOutPut.Text.Contains(" "))
                        {
                            MessageBox.Show("路径含有非法字符,请重新选择！");
                            return;
                        }
                        _viewModel.Arguments_4_横版转竖版裁剪 = "";
                        _viewModel.Arguments_5_横版转竖版填充 = "";
                        GoToWhichStep(5);
                        break;
                    case "竖转横":
                        if (string.IsNullOrEmpty(tbOutPut.Text))
                        {
                            MessageBox.Show("请选择导出素材的路径！");
                            return;
                        }
                        if (tbOutPut.Text.Contains(" "))
                        {
                            MessageBox.Show("路径含有非法字符,请重新选择！");
                            return;
                        }
                        _viewModel.Arguments_3_竖版转横版 = "";
                        GoToWhichStep(5);
                        break;
                    case "字幕":
                        _viewModel.Arguments_6_添加字幕 = "";
                        GoToWhichStep(6);
                        break;
                    case "水印":
                        _viewModel.Arguments_7_添加水印 = "";
                        GoToWhichStep(7);
                       
                        break;
                    case "预览":
                        grid预览页.Visibility = Visibility.Collapsed;
                        _viewModel.IsCancel = false;
                        _viewModel.IsIgnore = true;
                        _viewModel.IsConfirm = false;
                        IsComplete = true;
                        VlcPlayer.Play(Stream.Null);
                        break;
                }
            }
          
        }



        void 进入预览(string videoPath)
        {
            _viewModel.PreViewVisibility = Visibility.Visible;
            Play(videoPath);
            this.Dispatcher.Invoke(() => {
                btnCut.IsEnabled = true;
                borCannotControl.Visibility = Visibility.Collapsed;
                _viewModel.IsVisibility = Visibility.Collapsed;
            });
            while (true)
            {
                Thread.Sleep(200);
                if (_viewModel.PreViewVisibility == Visibility.Collapsed)
                {
                    break;
                }
            }
        }

        void Play(string FileName)
        {
            VlcPlayer.VlcMediaPlayer.Audio.IsMute = true;
            CurrentPlayStream?.Close();
            CurrentPlayStream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            IsComplete = true;
            Thread.Sleep(250);
            VlcPlayer.Play(CurrentPlayStream);
        }


        #region 删除
        private void 删除_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button image)
            {
                _viewModel.VideoGroupNodeList.Remove(image.DataContext as VideoGroupNode);
                //更新总数量
                _viewModel.TotalCount = 1;
                foreach (var item in _viewModel.VideoGroupNodeList)
                {
                    _viewModel.TotalCount *= item.VideoNode.Count;
                }
            }
        }
        #endregion

        #region 上移 下移


        private void 上移_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button image)
            {
                if (image.DataContext is VideoGroupNode videoGroupNode)
                {
                    var temp = videoGroupNode;
                    int index = 0;
                    foreach (var item in _viewModel.VideoGroupNodeList)
                    {

                        if (item.ID == temp.ID)
                        {
                            break;
                        }
                        index++;
                    }
                    if (index == 0)
                    {
                        for (int i = 0; i < _viewModel.VideoGroupNodeList.Count - 1; i++)
                        {
                            _viewModel.VideoGroupNodeList[i] = _viewModel.VideoGroupNodeList[i + 1];
                        }
                        _viewModel.VideoGroupNodeList[_viewModel.VideoGroupNodeList.Count - 1] = temp;
                    }
                    else
                    {
                        _viewModel.VideoGroupNodeList[index] = _viewModel.VideoGroupNodeList[index - 1];
                        _viewModel.VideoGroupNodeList[index - 1] = temp;
                    }
                }
            }
        }

        private void 下移_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button image)
            {
                if (image.DataContext is VideoGroupNode videoGroupNode)
                {
                    var temp = videoGroupNode;
                    int index = 0;
                    foreach (var item in _viewModel.VideoGroupNodeList)
                    {

                        if (item.ID == temp.ID)
                        {
                            break;
                        }
                        index++;
                    }
                    if (index == _viewModel.VideoGroupNodeList.Count - 1)
                    {
                        for (int i = _viewModel.VideoGroupNodeList.Count - 1; i > 0; i--)
                        {
                            _viewModel.VideoGroupNodeList[i] = _viewModel.VideoGroupNodeList[i - 1];
                        }
                        _viewModel.VideoGroupNodeList[0] = temp;
                    }
                    else
                    {
                        _viewModel.VideoGroupNodeList[index] = _viewModel.VideoGroupNodeList[index + 1];
                        _viewModel.VideoGroupNodeList[index + 1] = temp;
                    }
                }
            }
        }


        #endregion

        private void 返回上一步_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button button)
            {
                if(button.Tag is Grid grid)
                {
                    _viewModel.IsVisibility = Visibility.Collapsed;
                    GridStepOne.Visibility = Visibility.Collapsed;
                    GridStepFive.Visibility = Visibility.Collapsed;
                    GridStepThree.Visibility = Visibility.Collapsed;
                    GridStepFour.Visibility = Visibility.Collapsed;
                    GridStepTwo.Visibility = Visibility.Collapsed;
                    GridStepSix.Visibility = Visibility.Collapsed;
                    grid.Visibility = Visibility.Visible;
                }
            }
        }

        private void 返回上一步横或者竖_Click(object sender, RoutedEventArgs e)
        {
            if(IsHorizontal)
            {
                GoToWhichStep(4);
            }
            else
            {
                GoToWhichStep(3);
            }
        }


        public void GetFontNames()
        {
      
            InstalledFontCollection installedFontCollection = new InstalledFontCollection();
            var fontFamilies = installedFontCollection.Families;
            foreach (var item in fontFamilies)
            {
                _viewModel.字体集合.Add(item.Name);
            }
        }

        private void 打开所在文件夹_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", _viewModel.SavePath);
        }

        private void 返回主界面Click(object sender, RoutedEventArgs e)
        {
           // _viewModel = new VideoPreviewModel();
            grid提示.Visibility = Visibility.Collapsed;
            try
            {
                Directory.Delete("temp\\", true);
            }
            catch (Exception ex)
            {

                FileHelper.AppandLog(ex.ToString());
            }
            _viewModel = new VideoPreviewModel();
            this.DataContext = _viewModel;
            _viewModel.TotalCount = 0;
            _viewModel.IsVisibility = Visibility.Collapsed;
            GoToWhichStep(1);
            GetFontNames();
        }

        void myMessgBoxShow(string Tip)
        {
            grid提示.Visibility = Visibility.Visible;
            tbTip.Text = Tip;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBox_Checked_逆时针(object sender, RoutedEventArgs e)
        {
            if(sender is CheckBox box)
            {
                ((VideoGroupNode)box.DataContext).顺时针旋转90 = false;
            }
        }

        private void CheckBox_Checked_顺时针(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox box)
            {
                ((VideoGroupNode)box.DataContext).逆时针旋转90 = false;
            }
        }

        bool 是否停止 = false;
        private void Button_Click_停止(object sender, RoutedEventArgs e)
        {
            是否停止 = true;
            GoToWhichStep(1);
        }
    }
}
