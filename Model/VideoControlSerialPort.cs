using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 素材合成.Model
{
    public class Flow 
    {

        public string Movie { get; set; }
        public string ID { get; set; }
        public string Allow { get; set; }
        public string EndTime { get; set; }
        public string Goto { get; set; }
        public string Password { get; set; }
       
        public List<Instruction> Instructions { get; set; }
    }
    public class Instruction : DMSkin.Core.ViewModelBase
    {
        public Instruction()
        {
        }
        private string _T时间;
        public string T时间
        {
            get { return _T时间; }
            set
            {
                _T时间 = value;
                OnPropertyChanged(nameof(T时间));
            }
        }
        private string _Cmd灯光;
        public string Cmd灯光
        {
            get { return _Cmd灯光; }
            set
            {
                _Cmd灯光 = value;
                OnPropertyChanged(nameof(Cmd灯光));
            }
        }
        private string _Cmd继电器;
        public string Cmd继电器
        {
            get { return _Cmd继电器; }
            set
            {
                _Cmd继电器 = value;
                OnPropertyChanged(nameof(Cmd继电器));
            }
        }
        private string _CmdTcp;
        public string CmdTcp
        {
            get { return _CmdTcp; }
            set
            {
                _CmdTcp = value;
                OnPropertyChanged(nameof(CmdTcp));
            }
        }
        private string _Other;
        public string Other
        {
            get { return _Other; }
            set
            {
                _Other = value;
                OnPropertyChanged(nameof(Other));
            }
        }
    }

}
