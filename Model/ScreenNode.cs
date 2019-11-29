using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace 素材合成.Model
{
    public class ScreenNode : DMSkin.Core.ViewModelBase
    {
        public ScreenNode()
        {
            _PathList = new ObservableCollection<string>();
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

        private ObservableCollection<string> _PathList;
        public ObservableCollection<string> PathList
        {
            get { return _PathList; }
            set
            {
                _PathList = value;
                OnPropertyChanged(nameof(PathList));
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

        private string _ScreenName;
        public string ScreenName
        {
            get { return _ScreenName; }
            set
            {
                _ScreenName = value;
                OnPropertyChanged(nameof(ScreenName));
            }
        }

        private Screen _ScreenItem;
        public Screen ScreenItem
        {
            get { return _ScreenItem; }
            set
            {
                _ScreenItem = value;
                OnPropertyChanged(nameof(ScreenItem));
            }
        }
    }
}
