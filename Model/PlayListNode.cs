using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace 素材合成.Model
{
    public class PlayListNode:DMSkin.Core.ViewModelBase
    {
        private string _ID;
        public string ID
        {
            get { return _ID; }
            set
            {
                _ID = value;
                OnPropertyChanged(nameof(ID));
            }
        }
        private long  _Size;
        public long Size
        {
            get { return _Size; }
            set
            {
                _Size = value;
                OnPropertyChanged(nameof(Size));
            }
        }
        private string _VideoPath;
        public string VideoPath
        {
            get { return _VideoPath; }
            set
            {
               _VideoPath = value;
                OnPropertyChanged(nameof(VideoPath));
            }
        }
        private string _VideoName;
        public string VideoName
        {
            get { return _VideoName; }
            set
            {
                _VideoName = value;
                OnPropertyChanged(nameof(VideoName));
            }
        }

        private ImageSource _VideoImg;
        public ImageSource VideoImg
        {
            get { return _VideoImg; }
            set
            {
                _VideoImg = value;
                OnPropertyChanged(nameof(VideoImg));
            }
        }

    }
}
