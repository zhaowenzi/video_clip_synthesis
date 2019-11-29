using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using 素材合成.Helper;
using System.IO.Ports;
using 素材合成.Model;

namespace 素材合成
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CommandGenerate
    {
        ViewModel.MainViewModel _viewModel;

        Stopwatch stopwatch = new Stopwatch();
        VideoPreviewModel videoPreviewModel;
        SerialPort serialPort;
        string movieName;
        public CommandGenerate( VideoPreviewModel value, SerialPort port,string movie)
        {
            // var res =  GetHexCode("1");
            movieName = movie;
           var list =    ConfigureHelper.GetSerialNodeList();
            InitializeComponent();
            videoPreviewModel = value;
            serialPort = port;
            if(port!=null)
            {
                port.DataReceived += SPort_DataReceived;
            }
         
            this.Loaded += MainWindow_Loaded;
            _viewModel = new ViewModel.MainViewModel();
            foreach (var item in list)
            {
                _viewModel.Page2ImagemModelList.Add(new Model.Page2ImageModel() { Key=item.key,Content=item.Content,IsChecked=false});
            }
            if(value.CurrentFlow!=null)
            {
                _viewModel.CommandList = new ObservableCollection<Model.Instruction>(value.CurrentFlow.Instructions.Where(u => u.T时间 == value.CurrentPos).ToList());
            }
         
            this.DataContext = _viewModel;

        }


        public static string[] GetAllportName()
        {
            string[] ports = SerialPort.GetPortNames();
            return ports;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            cmbAddress.Items.Add("01");
            cmbAddress.Items.Add("02");
            cmbAddress.Items.Add("03");
            cmbAddress.Items.Add("04");
            cmbAddress.SelectedIndex = 0;
         
            tbCommander.Text = $"<Instructions   时间=\"{videoPreviewModel.CurrentPos}\"  灯光指令=\"\" 继电器指令=\"\" 网络指令=\"\"  其他信息=\"1\"></Instructions >";

            // MessageBox.Show("高：" + this.ActualHeight + "宽：" + this.ActualWidth);
        }

        #region 串口连接 和串口消息接受

        private void SPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sPort = (SerialPort)sender;
            byte[] rbuf;
            rbuf = new byte[sPort.BytesToRead];
            sPort.DiscardOutBuffer();
            sPort.Read(rbuf, 0, rbuf.Length);
            //   sPort.ReadLine(rbuf, 0, sPort.BytesToRead);

            this.Dispatcher.Invoke(() =>
            {
                if (lbRecvive.Items.Count > 20)
                {
                    lbRecvive.Items.Clear();
                }
                lbRecvive.Items.Add(CmdHelper.ByteToString(rbuf));
            });
        }


        private void 发送_Click(object sender, RoutedEventArgs e)
        {
            var data = CmdHelper.StringToByte(tbResult.Text);
            serialPort.Write(data, 0, data.Length);
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
                case "Max":
                    this.WindowState = WindowState.Maximized;
                    Max.Visibility = Visibility.Collapsed;
                    Full.Visibility = Visibility.Visible;
                    break;
                case "Full":
                    this.WindowState = WindowState.Normal;
                    Full.Visibility = Visibility.Collapsed;
                    Max.Visibility = Visibility.Visible;
                    break;
                case "Close":
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.Close();
                    }));
                    break;
            }
        }
        #endregion

        string GetHexCode(List<Model.Page2ImageModel> source)
        {
            string hex= "";
            string binary = "";
            int index = 0;
            if(source.Count>0)
            {
                for (int i = 0; i < 32; i++)
                {
                    int flag = int.Parse(source[index].Key);
                    if (flag == i + 1)
                    {
                        if (index < source.Count - 1)
                        {
                            index++;
                        }

                        binary += "1";
                    }
                    else
                    {
                        binary += "0";
                    }
                }
            }
            else
            {
                for (int i = 0; i < 32; i++)
                {
                      binary += "0";
                }
            }
           
            hex = CmdHelper.BitTo16(binary);
            return hex;
        }

        string GetHexCode1(List<Model.Page2ImageModel> source)
        {
            string hex = "";
            string binary = "";
            int index = 0;
            if (source.Count > 0)
            {
                for (int i = 0; i < 16; i++)
                {
                    int flag = int.Parse(source[index].Key);
                    if (flag == i + 1)
                    {
                        if (index < source.Count - 1)
                        {
                            index++;
                        }

                        binary += "1";
                    }
                    else
                    {
                        binary += "0";
                    }
                }
            }
            else
            {
                for (int i = 0; i < 16; i++)
                {
                    binary += "0";
                }
            }


            hex = CmdHelper.BitTo16(binary);
            return hex;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button  btn)
            {
                if(btn.DataContext is Model.Page2ImageModel item)
                {
                    //StringBuilder sb = new StringBuilder();
                    //sb.Append(item.Key + "控制指令:  ");
                    //sb.Append("55 AA ");
                    //sb.Append(" 01 ");
                    //sb.Append(" 09 02 ");
                    //int cs = 12;
                    //#region len
                    //sb.Append(" " + To10_16_Length_2(6 + item.通道数) + " ");
                    //cs = cs + 6 + item.通道数;
                    //#endregion
                    //#region data[0]
                    //if (item.OutputImmediately)
                    //{
                    //    sb.Append(" 01 ");
                    //    cs = cs + 1;
                    //}
                    //else
                    //{
                    //    sb.Append(" 00 ");
                    //}
                    //#endregion
                    //#region data[1~2]
                    //sb.Append(To10_16_Length_4(int.Parse(item.地址)));
                    //cs = cs + int.Parse(item.地址);
                    //#endregion
                    //#region data[3]
                    //sb.Append(" 01 ");
                    //cs = cs + 1;
                    //#endregion
                    //#region data[4]
                    //sb.Append(" 00 ");
                    //#endregion

                    //#region data[5]
                    //sb.Append(To10_16_Length_2(item.通道数));
                    //cs = cs + item.通道数;
                    //#endregion
                    //foreach (var item1 in item.各通道值)
                    //{
                    //    sb.Append(" " + item1.Vlaue + " ");
                    //    cs = cs + To16_10_ff(item1.Vlaue);
                    //}
                    //#region 校验位
                    //sb.Append(" " + To10_16_Length_2(256 - cs));
                    //#endregion
                    //sb.AppendLine(" ");
                }
               
            }
           
        }


        private void 输出_Click(object sender, RoutedEventArgs e)
        {
            if(sender is  Button btn)
            {
                switch(btn.Content+"")
                {
                    case "32路输出":
                        var selectList = this._viewModel.Page2ImagemModelList.Where(l => l.IsChecked == true).ToList();
                        string addressCode = cmbAddress.SelectedValue + "";
                        string functionCode = "33";
                        if(selectList.Count>1)
                        {
                            functionCode = "13";
                        }
                     string logicCode =     GetHexCode(selectList);
                     string checkCode = CmdHelper.To10_16_Length_2((CmdHelper.To16_10_ff("55") 
                         + CmdHelper.To16_10_ff(addressCode)
                         + CmdHelper.To16_10_ff(functionCode)
                         + CmdHelper.To16_10_ff(logicCode.Substring(0,2))
                         + CmdHelper.To16_10_ff(logicCode.Substring(2,2))
                         + CmdHelper.To16_10_ff(logicCode.Substring(4,2))
                         + CmdHelper.To16_10_ff(logicCode.Substring(6,2))));
                        StringBuilder sb = new StringBuilder();
                        sb.Append("55 ");
                        sb.Append(addressCode+" ");
                        sb.Append(functionCode + " ");
                        sb.Append(logicCode + " ");
                        sb.Append(checkCode + " ");
                        tbResult.Text = sb.ToString();
                        tbCommander.Text = $"<Instructions   时间=\"{videoPreviewModel.CurrentPos}\"  灯光指令=\"{tbResult.Text}\" 继电器指令=\"\" 网络指令=\"\"  其他信息=\"1\"></Instructions >";
                        break;
                    case "16进16出":
                       
                        var selectList16 = this._viewModel.Page2ImagemModelList.Where(U=>U.IsChecked==true&&int.Parse(U.Key)>16).ToList();
                       if(selectList16.Count>0)
                        {
                            MessageBox.Show("只能选择16路之内的 按钮!");
                            break;
                        }
                       else
                        {
                            var selectList1 = this._viewModel.Page2ImagemModelList.Where(U => U.IsChecked == true).ToList();
                            string addressCode1 = cmbAddress.SelectedValue + "";
                            string functionCode1 = "11";

                            string logicCode1 = "0000"+GetHexCode1(selectList1);
                            string checkCode1 = CmdHelper.To10_16_Length_2((CmdHelper.To16_10_ff("22")
                                + CmdHelper.To16_10_ff(addressCode1)
                                + CmdHelper.To16_10_ff(functionCode1)
                                + CmdHelper.To16_10_ff(logicCode1.Substring(4, 2))
                                + CmdHelper.To16_10_ff(logicCode1.Substring(6, 2))));
                            StringBuilder sb1 = new StringBuilder();
                            sb1.Append("22 ");
                            sb1.Append(addressCode1 + " ");
                            sb1.Append(functionCode1 + " ");
                            sb1.Append(logicCode1 + " ");
                            sb1.Append(checkCode1 + " ");
                            tbResult.Text = sb1.ToString();
                            tbCommander.Text = $"<Instructions   时间=\"{videoPreviewModel.CurrentPos}\"  灯光指令=\"{tbResult.Text}\" 继电器指令=\"\" 网络指令=\"\"  其他信息=\"1\"></Instructions >";
                        }
                        break;
                }
            }
        }

        private void 复制_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetDataObject(tbResult.Text);
        }

        private void 选中全部_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in this._viewModel.Page2ImagemModelList)
            {
                if(int.Parse(item.Key)<17)
                {
                    item.IsChecked = true;
                }
            }
        }

        private void 取消选中_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in this._viewModel.Page2ImagemModelList)
            {
                    item.IsChecked = false;
            }
        }
        string selectOther;
        private void 修改并保存_Click(object sender, RoutedEventArgs e)
        {
            ConfigureHelper.SetAllTopicType1(videoPreviewModel.CurrentFlow.Movie, videoPreviewModel.CurrentPos, selectOther, tbResult.Text);
            tbCommander.Text = $"<Instructions   时间=\"{videoPreviewModel.CurrentPos}\"  灯光指令=\"{tbResult.Text}\" 继电器指令=\"\" 网络指令=\"\"  其他信息=\"{selectOther}\"></Instructions >";
            MessageBox.Show("修改成功!,已写入配置文件!");
        }

        private void 新增并保存_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            if(videoPreviewModel.CurrentFlow==null)
            {
                count = 0;
            }
            else
            {
                count = videoPreviewModel.CurrentFlow.Instructions.Where(u => u.T时间 == videoPreviewModel.CurrentPos).Count();
            }
            ConfigureHelper.SetAllTopicType1(movieName, videoPreviewModel.CurrentPos, count + 1 + "", tbResult.Text);
            tbCommander.Text = $"<Instructions   时间=\"{videoPreviewModel.CurrentPos}\"  灯光指令=\"{tbResult.Text}\" 继电器指令=\"\" 网络指令=\"\"  其他信息=\"{count+ 1}\"></Instructions >";
            MessageBox.Show("新增成功!,已写入配置文件!");
        }


        private void xuanzhe_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox)
            {
                if (listBox.SelectedItem is Instruction instruction)
                {
                    btnUpdate.IsEnabled = true;
                    selectOther = instruction.Other;
                    tbCommander.Text = $"<Instructions   时间=\"{videoPreviewModel.CurrentPos}\"  灯光指令=\"{instruction.Cmd灯光}\" 继电器指令=\"\" 网络指令=\"\"  其他信息=\"{selectOther}\"></Instructions >";
                }
            }
        }
        
    }
}
