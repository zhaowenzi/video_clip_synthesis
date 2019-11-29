using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        static string root_path = (System.AppDomain.CurrentDomain.BaseDirectory) + "\\";
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = _viewModel;
            //播放器
            VlcPlayer.Playing += MediaPlayer_Playing;

            this.Loaded += MainWindow_Loaded;
            _viewModel.LumaPower = 1+"";
            _viewModel.LumaPowerBottom = 1+"";
            _viewModel.LumaPowerTop = "1";
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.TotalCount = 0;
            _viewModel.IsVisibility = Visibility.Collapsed;
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

                image.DecodePixelWidth = 300;

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
                if (!Directory.Exists(imgPath))
                {
                    Directory.CreateDirectory(imgPath);
                }
                startInfo.Arguments = $"-ss 00:00:05 -i \"{VideoName}\" -f image2 -y \"{imgFullFileName}\"";
                System.Diagnostics.Process.Start(startInfo);

                while (imageSource == null)
                {
                    Thread.Sleep(300);
                    imageSource = SetSource(imgFullFileName);
                    if (_viewModel.MaxId == 1)
                    {
                        _viewModel.BackgroundImage = SetFullSizeSource(imgFullFileName);
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
                if (index + 1 >= MaxTime * 5)
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
                        if (index != 0 && index % 5 == 0)
                        {
                            if (VlcPlayer.IsPlaying)
                            {
                                Console.WriteLine($"{ _viewModel.CurrentTime }||{_viewModel.CurrentPos}");
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
                            }
                        }

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
            VlcPlayer.Position = (int.Parse(tbTime.Text) + 0.0001f) / MaxTime;
            _viewModel.CurrentFrame = (int)(VlcPlayer.Position * _viewModel.AllFrame);
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

        #endregion

        #region 导出素材

        private void 合成视频_Click(object sender, RoutedEventArgs e)
        {
         
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
            if (int.Parse(tbOutPutCount.Text) ==0)
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
        void Comose(object arr)
        {
            var temp = arr as List<List<int>>;
            flag = 1;
            ComposePathList.Clear();
            foreach (var item in temp)
            {
                StringBuilder sb = new StringBuilder();
                _viewModel.TotalSize = 0;
                _viewModel.Size = 0;
                this.Dispatcher.Invoke(() =>
                {
                    _viewModel.PlayList.Clear();
                    for (int i = 0; i < item.Count; i++)
                    {
                        _viewModel.PlayList.Add(_viewModel.VideoGroupNodeList[i].VideoNode[item[i] - 1]);
                    }
                    foreach (var item1 in _viewModel.PlayList)
                    {

                        sb.AppendLine("file '" + item1.VideoPath + "'");
                        _viewModel.TotalSize += item1.Size;
                    }
                });

                if (!Directory.Exists("temp\\"))
                {
                    Directory.CreateDirectory("temp\\");
                }

                string OutPutVideoName = $"temp\\temp_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_Video.mp4";

                FileHelper.SaveFile_Create("fileList.txt", sb.ToString());
                FileInfo fileInfo = new FileInfo("fileList.txt");
                string Arguments = $" -f concat -safe 0  -i  {fileInfo.FullName} -c copy {OutPutVideoName}";
                ExecuteAsAdmin(Arguments);
                Thread.Sleep(300);
                GetComposeProgress(OutPutVideoName);
                _viewModel.CurrentIndex = flag;
                flag++;
            }
            _viewModel.IsVisibility = Visibility.Collapsed;
        }

        public void ExecuteAsAdmin(string Arg, string fileName = "ffmpeg.exe")
        {
            Process proc = new Process();
            proc.StartInfo.FileName = root_path + fileName;
            proc.StartInfo.Arguments = Arg;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.Verb = "runas";
            proc.Start();
        }

        void GetComposeProgress(object fullFileName)
        {
            long currentSize = 0;
            while (true)
            {
                if (_viewModel.Size > 0 && _viewModel.Size == currentSize)
                {
                    _viewModel.Size = _viewModel.TotalSize;
                  
                        this.Dispatcher.Invoke(() =>
                        {
                            if (flag == int.Parse(tbOutPutCount.Text))
                            {
                                GridStepOne.Visibility = Visibility.Collapsed;
                                GridStepTwo.Visibility = Visibility.Visible;
                                _viewModel.FillColor = Colors.Black;
                                btnOut.IsEnabled = true;
                            }
                        });
                    break;
                }
                if (File.Exists(fullFileName.ToString()))
                {
                    FileInfo fileInfo = new FileInfo(fullFileName.ToString());
                    ComposePathList.Add(fileInfo.FullName);
                    _viewModel.Size = fileInfo.Length;
                    Thread.Sleep(1000);
                    currentSize = fileInfo.Length;
                }
                Thread.Sleep(100);
            }
        }

        void GetCutProgress( string fileName)
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

        #endregion

        #region 剪切视频
        private void 剪切视频_Click(object sender, RoutedEventArgs e)
        {
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

        List<string> CutVideoList = new List<string>();
        void CutVideo(object selectPath)
        {
            flag = 1;
            int? videoWidth = 0;
            int? videoHeight = 0;
            string time = "";
            bool IsHorizontal;
            CutVideoList.Clear();
            _viewModel.CurrentIndex = 0;
            GetMovWidthAndHeight(ComposePathList[0], out videoWidth, out videoHeight, out time, out IsHorizontal);
            Random randomer = new Random();
            foreach (var item in ComposePathList)
            {

                
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
                string OutPutVideoName = $"{selectPath}\\tempCut_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_Video.mp4";
                FileInfo fileInfo = new FileInfo(item);
                _viewModel.TotalSize = fileInfo.Length * 2;
                //裁剪
                string CutArguments = $"  -i {item} -vf crop=in_w-{offsetLeft}-{offsetRight}:in_h-{offsetTop}-{offsetBottom}:{offsetLeft}:{offsetTop} {OutPutVideoName} -y ";
                ExecuteCommandCut(CutArguments, OutPutVideoName);
                // ExecuteAsAdmin(CutArguments);
                Thread.Sleep(300);
                //  GetComposeProgress(OutPutVideoName);
                _viewModel.CutSize = fileInfo.Length;
                //填充后输出路径
                string OutPutPaddVideoName = $"{selectPath}\\PaddVideo_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_Video.mp4";

                //填充
                string PaddArguments;
                if (IsHorizontal)
                {
                    //横版参数
                    PaddArguments = $" -i {OutPutVideoName} -vf pad={videoWidth}:{videoHeight}:{offsetLeft}:{offsetTop}:0x{CmdHelper.To10_16_Length_2(_viewModel.FillColor.R)}{CmdHelper.To10_16_Length_2(_viewModel.FillColor.G)}{CmdHelper.To10_16_Length_2(_viewModel.FillColor.B)}  {OutPutPaddVideoName} -y";
                }
                else
                {
                    //  竖版参数
                    PaddArguments = $" -i {OutPutVideoName} -vf pad={videoHeight}:{videoWidth}:{offsetLeft}:{offsetTop}:0x{CmdHelper.To10_16_Length_2(_viewModel.FillColor.R)}{CmdHelper.To10_16_Length_2(_viewModel.FillColor.G)}{CmdHelper.To10_16_Length_2(_viewModel.FillColor.B)}  {OutPutPaddVideoName} -y";
                }
                ExecuteCommandCut(PaddArguments, OutPutPaddVideoName);
                Thread.Sleep(300);

                _viewModel.CurrentIndex = flag;
                flag++;
                if (_viewModel.IsSame == false)
                {
                        _viewModel.FillColor = ColorUtil.GetRandomColor();
                }
                File.Delete(OutPutVideoName);
                CutVideoList.Add(OutPutPaddVideoName);
            }
            _viewModel.IsVisibility = Visibility.Collapsed;
            Directory.Delete("temp\\", true);
            this.Dispatcher.Invoke(() =>
            {
                btnCut.IsEnabled = true;
                if(IsHorizontal==false)
                {
                    GridStepThree.Visibility = Visibility.Visible;
                    GridStepFour.Visibility = Visibility.Collapsed;
                    GridStepTwo.Visibility = Visibility.Collapsed;
                    _viewModel.LumaPower = "1";
                    _viewModel.TotalSize = _viewModel.CutSize;
                    InitVlaue();
                }
                else
                {
                    GridStepThree.Visibility = Visibility.Collapsed;
                    GridStepFour.Visibility = Visibility.Visible;
                    GridStepTwo.Visibility = Visibility.Collapsed;
                    _viewModel.LumaPower = "1";
                    _viewModel.TotalSize = _viewModel.CutSize;
                    InitVlaue();
                }

            });

        }


        public void ExecuteCommandCut(string command, string fileFullPath)
        {
            try
            {
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



        #endregion

        #region 竖版转横版


        private void 竖版转横版_Click(object sender, RoutedEventArgs e)
        {
            btnVeritalToHorizontal.IsEnabled = false;
            _viewModel.IsVisibility = Visibility.Visible;
            new Thread(TransformVideo).Start(tbOutPut.Text);
        }

        void TransformVideo( object selectPath)
        {
            flag = 1;
            int? videoWidth = 0;
            int? videoHeight = 0;
            string time = "";
            bool IsHorizontal;
            _viewModel.CurrentIndex = 0;
            GetMovWidthAndHeight(CutVideoList[0], out videoWidth, out videoHeight, out time, out IsHorizontal);
            Random randomer = new Random();
            foreach (var item in CutVideoList)
            {
                _viewModel.Size = 0;
                _viewModel.CutSize = 0;

                string PaddArguments;

                string OutPutPaddVideoName = $"{selectPath}\\tempTransformVertial_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_Video.mp4";
                //横版参数

                PaddArguments = $" -i {item} -lavfi \"" +
             $"[0:v]scale=1080*2:1920*2,boxblur=luma_radius=min(h\\,w)/20:luma_power={_viewModel.LumaPower}:chroma_radius=min(cw\\,ch)/20:chroma_power=1[bg];" +
             $"[0:v]scale=-1:1080[ov];[bg][ov]overlay=(W-w)/2:(H-h)/2,crop=w=1920:h=1080" +
             $"\" {OutPutPaddVideoName} -y";
                ExecuteCommandCut(PaddArguments, OutPutPaddVideoName);
                Thread.Sleep(300);
                //  GetComposeProgress(OutPutPaddVideoName);
                _viewModel.CurrentIndex = flag;
                flag++;
            }
            this.Dispatcher.Invoke(() =>
            {
               btnVeritalToHorizontal.IsEnabled = true;
                _viewModel.IsVisibility = Visibility.Collapsed;
            });

        }



        #endregion

        #region  横版转竖版_
        private void 横版转竖版_Click(object sender, RoutedEventArgs e)
        {
            btnHorizontalToVertical.IsEnabled = false;
            _viewModel.IsVisibility = Visibility.Visible;
            new Thread(TransformVideoHorizontal).Start(tbOutPut.Text);
        }

        void TransformVideoHorizontal(object selectPath)
        {
            flag = 1;
         //   int? videoWidth = 0;
        //    int? videoHeight = 0;
            string time = "";
        //    bool IsHorizontal;
         
            _viewModel.CurrentIndex = 0;
           // GetMovWidthAndHeight(CutVideoList[0], out videoWidth, out videoHeight, out time, out IsHorizontal);
            Random randomer = new Random();
            foreach (var item in CutVideoList)
            {
                int offsetLeft = 0;
                int offsetRight = 0;
                _viewModel.Size = 0;
                _viewModel.CutSize = 0;
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

                //横版左右裁剪输出路径
                string OutPutVideoName = $"{selectPath}\\tempCutHorizontal_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_Video.mp4";
                FileInfo fileInfo = new FileInfo(item);
                _viewModel.TotalSize = fileInfo.Length * 2;
                //裁剪
                string CutArguments = $"  -i {item} -vf crop=in_w-{offsetLeft}-{offsetRight}:in_h-{0}-{0}:{offsetLeft}:{0} {OutPutVideoName} -y ";
                ExecuteCommandCut(CutArguments, OutPutVideoName);
                Thread.Sleep(300);
                _viewModel.CutSize = fileInfo.Length;
                string PaddArguments;

                string OutPutPaddVideoName = $"{selectPath}\\TransformHorizontal_{flag}_{System.DateTime.Now.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "_")}_Video.mp4";
                //填充
                if(IsImage==false)
                {
                    PaddArguments = $" -i {OutPutVideoName} -vf \"" +
     $"scale=1080:-2,pad=1080:1920:0:{_viewModel.PadHeight}:0x{CmdHelper.To10_16_Length_2(_viewModel.FillColor.R)}{CmdHelper.To10_16_Length_2(_viewModel.FillColor.G)}{CmdHelper.To10_16_Length_2(_viewModel.FillColor.B)}" +
     $"\" {OutPutPaddVideoName} -y";
                }
                else
                {
                    int image2Ypos = 1920 - int.Parse(_viewModel.PadHeight) - 608;
                    int yellow = 608 + int.Parse(_viewModel.PadHeight);
                    PaddArguments = $" -i {OutPutVideoName} -i {_viewModel.ImageTopPath} -i {_viewModel.ImageBottomPath} -filter_complex \""
                        + $"[0:v]scale=1080:-2,pad=1080:1920:0:{_viewModel.PadHeight}:blue[outvideo];" +
                        $"[1:v]scale=1080:{_viewModel.PadHeight},boxblur={_viewModel.LumaPowerTop}:1:cr=0:ar=0[outup];" +
                        $"[2:v]scale=1080:{image2Ypos},boxblur={_viewModel.LumaPowerBottom}:1:cr=0:ar=0[outdown];" +
                        $"[outvideo][outup]overlay=0:0[output1];" +
                        $"[output1][outdown]overlay=0:{yellow}"+
                        $"\" {OutPutPaddVideoName}"
                        ;
                }
         
                ExecuteCommandCut(PaddArguments, OutPutPaddVideoName);
                Thread.Sleep(300);
                _viewModel.CurrentIndex = flag;
                flag++;
                if (_viewModel.IsSame == false)
                {
                    _viewModel.FillColor = ColorUtil.GetRandomColor();
                }
                File.Delete(OutPutVideoName);
            }
            this.Dispatcher.Invoke(() => 
            {
                btnVeritalToHorizontal.IsEnabled = true;
                _viewModel.IsVisibility = Visibility.Collapsed;
            });
        }



        #endregion

        void  InitVlaue()
        {
            _viewModel.CutPixel_1 = 0;
            _viewModel.CutPixel_2 = 0;
            _viewModel.CutPixel_3 = 0;
            _viewModel.CutPixel_4 = 0;
            _viewModel.offsetMin_1 = 0;
            _viewModel.offsetMin_2 = 0;
            _viewModel.offsetMin_3 = 0;
            _viewModel.offsetMin_4 = 0;
            _viewModel.LumaPower = 1 + "";
            _viewModel.LumaPowerBottom = 1 + "";
            _viewModel.LumaPowerTop = "1";
            _viewModel.FillColor = Colors.Black;
        }

        #region 删除
        private void Image_PreviewMouseLeftButtonUp_删除(object sender, MouseButtonEventArgs e)
        {
            if (sender is StackPanel image)
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

        private void Image_PreviewMouseLeftButtonUp_上移(object sender, MouseButtonEventArgs e)
        {
            if (sender is StackPanel image)
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

        private void Image_PreviewMouseLeftButtonUp_下移(object sender, MouseButtonEventArgs e)
        {
            if (sender is StackPanel image)
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
            return indexOfMin(differance);

        }

        #endregion

        #region 获取视频宽高

        public void GetMovWidthAndHeight(string videoFilePath, out int? width, out int? height,out string time,out bool IsHorizontal)
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
                if(error.Replace(" ","").Contains("rotate:90"))
                {
                    IsHorizontal = false;
                }
                else
                {
                    IsHorizontal = true;
                }

                if (m.Success)
                {
                    width = int.Parse(m.Groups[1].Value);
                    height = int.Parse(m.Groups[2].Value);
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

        public  void ExecuteCommand(string command, out string output, out string error)
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
            string  path =   FileHelper.OpenFile_getPath("");
            if(string.IsNullOrEmpty(path))
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

        private void 随机从图库选择图片_Click_1(object sender, RoutedEventArgs e)
        {

        }
        bool IsImage = false;
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
    }
}
