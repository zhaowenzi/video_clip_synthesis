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
    }

}
