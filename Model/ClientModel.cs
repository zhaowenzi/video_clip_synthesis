using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace 素材合成.Model
{
    public class ClientModel : DMSkin.Core.ViewModelBase
    {
        public ClientModel(string SeverIP, string Port)
        {
            _IP = GetLocalIP();
            _ServerIP = SeverIP;
            _Port = Port;
            Success = true;
        }
        public void Connect()
        {
            client = new SimpleTCP.SimpleTcpClient();
            try
            {
                client.Connect(ServerIP, int.Parse(Port));
            }
            catch 
            {
               MessageBox.Show("连接失败!请确保网络畅通并且服务器已开启!") ;
                Environment.Exit(0);
                return;
            }
         
            client.Write($"你好,我是{_IP}建立的连接!");
            Success = false;
        }

        public static string GetLocalIP()
        {
            try
            {

                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        string ip = "";
                        ip = IpEntry.AddressList[i].ToString();
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public SimpleTCP.SimpleTcpClient client { get; set; }

        private bool _Success;
        public bool Success
        {
            get => _Success;
            set
            {
                _Success = value;
                OnPropertyChanged(nameof(Success));
            }
        }

        private string _IP;
        public string IP
        {
            get => _IP;
            set
            {
                _IP = value;
                OnPropertyChanged(nameof(IP));
            }
        }

        private string _Port;
        public string Port
        {
            get => _Port;
            set
            {
                _Port = value;
                OnPropertyChanged(nameof(Port));
            }
        }

        private string _ServerIP;
        public string ServerIP
        {
            get => _ServerIP;
            set
            {
                _ServerIP = value;
                OnPropertyChanged(nameof(ServerIP));
            }
        }

        private string _SendMessage;
        public string SendMessage
        {
            get => _SendMessage;
            set
            {
                _SendMessage = value;
                OnPropertyChanged(nameof(SendMessage));
            }
        }


        private ObservableCollection<string> _RecviveMessage = new ObservableCollection<string>();
        public ObservableCollection<string> RecviveMessage
        {
            get => _RecviveMessage;
            set
            {
                _RecviveMessage = value;
                OnPropertyChanged(nameof(RecviveMessage));
            }
        }
    }
}
