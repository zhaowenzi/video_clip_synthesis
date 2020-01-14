using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 素材合成.Model
{
    public class VideoGroupNode : DMSkin.Core.ViewModelBase
    {
        public VideoGroupNode()
        {
            _VideoNode = new ObservableCollection<PlayListNode>();
            _逆时针旋转90 = false;
            _顺时针旋转90 = false;
            _GUID = new Guid().ToString();
        }

        private string _GUID;
        public string GUID
        {
            get { return _GUID; }
            set
            {
                _GUID = value;
                OnPropertyChanged(nameof(GUID));
            }
        }

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
        private int _Index;
        public int Index
        {
            get { return _Index; }
            set
            {
                _Index = value;
                OnPropertyChanged(nameof(Index));
            }
        }

        private System.Windows.Media.Brush _BackgroundColor;
        public System.Windows.Media.Brush BackgroundColor
        {
            get { return _BackgroundColor; }
            set
            {
                _BackgroundColor = value;
                OnPropertyChanged(nameof(BackgroundColor));
            }
        }
        private double _Width;
        public double Width
        {
            get { return _Width; }
            set
            {
                _Width = value;
                OnPropertyChanged(nameof(Width));
            }
        }

        private ObservableCollection<Model.PlayListNode> _VideoNode;
        public ObservableCollection<Model.PlayListNode> VideoNode
        {
            get { return _VideoNode; }
            set
            {
                _VideoNode = value;
                OnPropertyChanged(nameof(VideoNode));
            }
        }

        private bool _逆时针旋转90;
        public bool 逆时针旋转90
        {
            get { return _逆时针旋转90; }
            set
            {
                _逆时针旋转90 = value;
                OnPropertyChanged(nameof(逆时针旋转90));
            }
        }

        private bool _顺时针旋转90;
        public bool 顺时针旋转90
        {
            get { return _顺时针旋转90; }
            set
            {
                _顺时针旋转90 = value;
                OnPropertyChanged(nameof(顺时针旋转90));
            }
        }
    }

}
