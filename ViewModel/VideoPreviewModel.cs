using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using 素材合成.Model;

namespace 素材合成
{
    public class VideoPreviewModel : DMSkin.Core.ViewModelBase
    {
        public VideoPreviewModel()
        {
            _PlayList = new ObservableCollection<Model.PlayListNode>();
            _ScreecList = new ObservableCollection<Model.ScreenNode>();
            _CommandList = new ObservableCollection<Model.Instruction>();
            _VideoGroupNodeList = new ObservableCollection<VideoGroupNode>();
            _offsetMax_1 = 0;
            _offsetMax_2 = 0;
            _offsetMax_3 = 0;
            _offsetMax_4 = 0;
            _CutPixel_1 = 0;
            _CutPixel_2= 0;
            _CutPixel_3 = 0;
            _CutPixel_4 = 0;
            _offsetMin_1 = 0;
            _offsetMin_2 = 0;
            _offsetMin_3= 0;
            _offsetMin_4 = 0;
            MaxId = 0;
        }
        public int MaxId { get; set; }
        public long CutSize { get; set; }

        private string _LumaPower;
        public string LumaPower
        {
            get { return _LumaPower; }
            set
            {
                _LumaPower = value;
                OnPropertyChanged(nameof(LumaPower));
            }
        }


        private string _PadHeight;
        public string PadHeight
        {
            get { return _PadHeight; }
            set
            {
                _PadHeight = value;
                OnPropertyChanged(nameof(PadHeight));
            }
        }
        private ObservableCollection<Model.ScreenNode> _ScreecList;
        public ObservableCollection<Model.ScreenNode> ScreecList
        {
            get { return _ScreecList; }
            set
            {
                _ScreecList = value;
                OnPropertyChanged(nameof(ScreecList));
            }
        }

        private ObservableCollection<Model.VideoGroupNode> _VideoGroupNodeList;
        public ObservableCollection<Model.VideoGroupNode> VideoGroupNodeList
        {
            get { return _VideoGroupNodeList; }
            set
            {
                _VideoGroupNodeList = value;
                OnPropertyChanged(nameof(VideoGroupNodeList));
            }
        }


        private string _ImageTopPath;
        public string ImageTopPath
        {
            get { return _ImageTopPath; }
            set
            {
                _ImageTopPath = value;
                OnPropertyChanged(nameof(ImageTopPath));
            }
        }
        private string _ImageBottomPath;
        public string ImageBottomPath
        {
            get { return _ImageBottomPath; }
            set
            {
                _ImageBottomPath = value;
                OnPropertyChanged(nameof(ImageBottomPath));
            }
        }
        private ImageSource _FillImageBottom;
        public ImageSource FillImageBottom
        {
            get { return _FillImageBottom; }
            set
            {
                _FillImageBottom = value;
                OnPropertyChanged(nameof(FillImageBottom));
            }
        }
        private ImageSource _FillImageTop;
        public ImageSource FillImageTop
        {
            get { return _FillImageTop; }
            set
            {
                _FillImageTop = value;
                OnPropertyChanged(nameof(FillImageTop));
            }
        }
        private string _LumaPowerBottom;
        public string LumaPowerBottom
        {
            get { return _LumaPowerBottom; }
            set
            {
                _LumaPowerBottom = value;
                OnPropertyChanged(nameof(LumaPowerBottom));
            }
        }
        private string _LumaPowerTop;
        public string LumaPowerTop
        {
            get { return _LumaPowerTop; }
            set
            {
                _LumaPowerTop = value;
                OnPropertyChanged(nameof(LumaPowerTop));
            }
        }


        #region 剪切

        private ImageSource _BackgroundImage;
        public ImageSource BackgroundImage
        {
            get { return _BackgroundImage; }
            set
            {
                _BackgroundImage = value;
                OnPropertyChanged(nameof(BackgroundImage));
            }
        }


        private int _CutPixel_1;
        public int CutPixel_1
        {
            get { return _CutPixel_1; }
            set
            {
                _CutPixel_1 = value;
                OnPropertyChanged(nameof(CutPixel_1));
            }
        }
        private int _CutPixel_2;
        public int CutPixel_2
        {
            get { return _CutPixel_2; }
            set
            {
                _CutPixel_2 = value;
                OnPropertyChanged(nameof(CutPixel_2));
            }
        }
        private int _CutPixel_3;
        public int CutPixel_3
        {
            get { return _CutPixel_3; }
            set
            {
                _CutPixel_3 = value;
                OnPropertyChanged(nameof(CutPixel_3));
            }
        }
        private int _CutPixel_4;
        public int CutPixel_4
        {
            get { return _CutPixel_4; }
            set
            {
                _CutPixel_4 = value;
                OnPropertyChanged(nameof(CutPixel_4));
            }
        }

        private int _offsetMin_1;
        public int offsetMin_1
        {
            get { return _offsetMin_1; }
            set
            {
                _offsetMin_1 = value;
                OnPropertyChanged(nameof(offsetMin_1));
            }
        }
        private int _offsetMin_2;
        public int offsetMin_2
        {
            get { return _offsetMin_2; }
            set
            {
                _offsetMin_2 = value;
                OnPropertyChanged(nameof(offsetMin_2));
            }
        }
        private int _offsetMin_3;
        public int offsetMin_3
        {
            get { return _offsetMin_3; }
            set
            {
                _offsetMin_3 = value;
                OnPropertyChanged(nameof(offsetMin_3));
            }
        }
        private int _offsetMin_4;
        public int offsetMin_4
        {
            get { return _offsetMin_4; }
            set
            {
                _offsetMin_4 = value;
                OnPropertyChanged(nameof(offsetMin_4));
            }
        }

        private int _offsetMax_1;
        public int offsetMax_1
        {
            get { return _offsetMax_1; }
            set
            {
                _offsetMax_1 = value;
                OnPropertyChanged(nameof(offsetMax_1));
            }
        }
        private int _offsetMax_2;
        public int offsetMax_2
        {
            get { return _offsetMax_2; }
            set
            {
                _offsetMax_2 = value;
                OnPropertyChanged(nameof(offsetMax_2));
            }
        }
        private int _offsetMax_3;
        public int offsetMax_3
        {
            get { return _offsetMax_3; }
            set
            {
                _offsetMax_3 = value;
                OnPropertyChanged(nameof(offsetMax_3));
            }
        }
        private int _offsetMax_4;
        public int offsetMax_4
        {
            get { return _offsetMax_4; }
            set
            {
                _offsetMax_4 = value;
                OnPropertyChanged(nameof(offsetMax_4));
            }
        }

        private Color _FillColor;
        public Color FillColor
        {
            get { return _FillColor; }
            set
            {
                _FillColor = value;
                FillBrush = new SolidColorBrush(value);
                OnPropertyChanged(nameof(FillColor));
            }
        }

        private Brush _FillBrush;
        public Brush FillBrush
        {
            get { return _FillBrush; }
            set
            {
                _FillBrush = value;
                OnPropertyChanged(nameof(FillBrush));
            }
        }
        private bool _IsSame;
        public bool IsSame
        {
            get { return _IsSame; }
            set
            {
                _IsSame = value;
                OnPropertyChanged(nameof(IsSame));
            }
        }
        #endregion


        private Visibility _IsVisibility;
        public Visibility IsVisibility
        {
            get { return _IsVisibility; }
            set
            {
                _IsVisibility = value;
                OnPropertyChanged(nameof(IsVisibility));
            }
        }

        private ObservableCollection<Model.Instruction> _CommandList;
        public ObservableCollection<Model.Instruction> CommandList
        {
            get { return _CommandList; }
            set
            {
                _CommandList = value;
                OnPropertyChanged(nameof(CommandList));
            }
        }

        private long _TotalSize;
        public long TotalSize
        {
            get { return _TotalSize; }
            set
            {
                _TotalSize = value;
                OnPropertyChanged(nameof(TotalSize));
            }
        }
        public string SavePath { get; set; }
        private long _Size;
        public long Size
        {
            get { return _Size; }
            set
            {
                _Size = value;
                OnPropertyChanged(nameof(Size));
            }
        }

        private int _CurrentIndex;
        public int CurrentIndex
        {
            get { return _CurrentIndex; }
            set
            {
                _CurrentIndex = value;
                OnPropertyChanged(nameof(CurrentIndex));
            }
        }

        private int _TotalCount;
        public int TotalCount
        {
            get { return _TotalCount; }
            set
            {
                _TotalCount = value;
                OnPropertyChanged(nameof(TotalCount));
            }
        }

        private int _CurrentFrame;
        public int CurrentFrame
        {
            get { return _CurrentFrame; }
            set
            {
                _CurrentFrame = value;
                OnPropertyChanged(nameof(CurrentFrame));
            }
        }

        private int _AllFrame;
        public int AllFrame
        {
            get { return _AllFrame; }
            set
            {
                _AllFrame = value;
                OnPropertyChanged(nameof(AllFrame));
            }
        }

        public long PlayTime { get; set; }
        private double _Progress;
        public double Progress
        {
            get { return _Progress; }
            set
            {
                _Progress = value;
                OnPropertyChanged(nameof(Progress));
            }
        }
        private double _LastPos;
        public double LastPos
        {
            get { return _LastPos; }
            set
            {
                _LastPos = value;
                OnPropertyChanged(nameof(LastPos));
            }
        }
        private string _CurrentPos;
        public string CurrentPos
        {
            get { return _CurrentPos; }
            set
            {
                _CurrentPos = value;
                OnPropertyChanged(nameof(CurrentPos));
            }
        }
        private float _CurrentTime;
        public float CurrentTime
        {
            get { return _CurrentTime; }
            set
            {
                _CurrentTime = value;
                OnPropertyChanged(nameof(CurrentTime));
            }
        }

        private string _CurrentCMD;
        public string CurrentCMD
        {
            get { return _CurrentCMD; }
            set
            {
                _CurrentCMD = value;
                OnPropertyChanged(nameof(CurrentCMD));
            }
        }

        private string _MinPos;
        public string MinPos
        {
            get { return _MinPos; }
            set
            {
                _MinPos = value;
                OnPropertyChanged(nameof(MinPos));
            }
        }

        private string _MaxPos;
        public string MaxPos
        {
            get { return _MaxPos; }
            set
            {
                _MaxPos = value;
                OnPropertyChanged(nameof(MaxPos));
            }
        }
        private string _VideoTitle;
        public string VideoTitle
        {
            get { return _VideoTitle; }
            set
            {
                _VideoTitle = value;
                OnPropertyChanged(nameof(VideoTitle));
            }
        }

        private bool _IsVIP;
        public bool IsVIP
        {
            get { return _IsVIP; }
            set
            {
                _IsVIP = value;
                if(value)
                {
                    ISVIPVisibility = Visibility.Collapsed;
                }
                else
                {
                    ISVIPVisibility = Visibility.Visible;
                }
                OnPropertyChanged(nameof(IsVIP));
            }
        }

        private Visibility _ISVIPVisibility;
        public Visibility ISVIPVisibility
        {
            get { return _ISVIPVisibility; }
            set
            {
                _ISVIPVisibility = value;
                OnPropertyChanged(nameof(ISVIPVisibility));
            }
        }
        


        private string _BtnLongText;
        public string BtnLongText
        {
            get { return _BtnLongText; }
            set
            {
                _BtnLongText = value;
                OnPropertyChanged(nameof(BtnLongText));
            }
        }


        private string _BtnText;
        public string BtnText
        {
            get { return _BtnText; }
            set
            {
                _BtnText = value;
                OnPropertyChanged(nameof(BtnText));
            }
        }
        private ObservableCollection<Model.PlayListNode> _PlayList;
        public ObservableCollection<Model.PlayListNode> PlayList
        {
            get { return _PlayList; }
            set
            {
                _PlayList= value;
                OnPropertyChanged(nameof(PlayList));
            }
        }

        public Flow CurrentFlow { get; set; }
    }
}
