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
            _ComBox集合 = new ObservableCollection<string>();
            _ComBox集合.Add("0");
            _ComBox集合.Add("1");
            _ComBox集合.Add("2");
            _ComBox集合.Add("3");
            _ComBox集合.Add("4");
            _ComBox集合123 = new ObservableCollection<string>();
            _ComBox集合123.Add("0");
            _ComBox集合123.Add("1");
            _ComBox集合123.Add("2");
            _ComBox集合123.Add("3");
            _ComBox集合123.Add("4");
            offsetMax_1 = 0;
            offsetMax_2 = 0;
            offsetMax_3 = 0;
            offsetMax_4 = 0;
            CutPixel_1 = 0;
            CutPixel_2= 0;
            CutPixel_3 = 0;
            CutPixel_4 = 0;
            offsetMin_1 = 0;
            offsetMin_2 = 0;
            offsetMin_3= 0;
            offsetMin_4 = 0;
            MaxId = 0;
            FontSize = "20";
            FontStyle = "DejaVu Serif";
            PadHeight = "100";
            TextDeepth = "0";
            TextHeight = "10";
            TextWidth = "0";
            FontColor = Colors.Black;
            CutPixel_横版右 = 0;
            CutPixel_横版左 = 0;
            水印坐标X = "10";
            水印坐标Y = "10";
            PreViewVisibility = Visibility.Collapsed;
        }

        private ObservableCollection<string> _ComBox集合;
        public ObservableCollection<string> ComBox集合
        {
            get { return _ComBox集合; }
            set
            {
                _ComBox集合 = value;
                OnPropertyChanged(nameof(ComBox集合));
            }
        }
        private ObservableCollection<string> _ComBox集合123;
        public ObservableCollection<string> ComBox集合123
        {
            get { return _ComBox集合123; }
            set
            {
                _ComBox集合123 = value;
                OnPropertyChanged(nameof(ComBox集合123));
            }
        }


        private string _Arguments_0_合成视频;
        public string Arguments_0_合成视频
        {
            get { return _Arguments_0_合成视频; }
            set
            {
                _Arguments_0_合成视频 = value;
                OnPropertyChanged(nameof(Arguments_0_合成视频));
            }
        }
        private string _Arguments_1_剪切视频;
        public string Arguments_1_剪切视频
        {
            get { return _Arguments_1_剪切视频; }
            set
            {
                _Arguments_1_剪切视频 = value;
                OnPropertyChanged(nameof(Arguments_1_剪切视频));
            }
        }
        private string _Arguments_2_填充视频;
        public string Arguments_2_填充视频
        {
            get { return _Arguments_2_填充视频; }
            set
            {
                _Arguments_2_填充视频 = value;
                OnPropertyChanged(nameof(Arguments_2_填充视频));
            }
        }
        private string _Arguments_3_竖版转横版;
        public string Arguments_3_竖版转横版
        {
            get { return _Arguments_3_竖版转横版; }
            set
            {
                _Arguments_3_竖版转横版 = value;
                OnPropertyChanged(nameof(Arguments_3_竖版转横版));
            }
        }
        private string _Arguments_4_横版转竖版裁剪;
        public string Arguments_4_横版转竖版裁剪
        {
            get { return _Arguments_4_横版转竖版裁剪; }
            set
            {
                _Arguments_4_横版转竖版裁剪 = value;
                OnPropertyChanged(nameof(Arguments_4_横版转竖版裁剪));
            }
        }
        private string _Arguments_5_横版转竖版填充;
        public string Arguments_5_横版转竖版填充
        {
            get { return _Arguments_5_横版转竖版填充; }
            set
            {
                _Arguments_5_横版转竖版填充 = value;
                OnPropertyChanged(nameof(Arguments_5_横版转竖版填充));
            }
        }
        private string _Arguments_6_添加字幕;
        public string Arguments_6_添加字幕
        {
            get { return _Arguments_6_添加字幕; }
            set
            {
                _Arguments_6_添加字幕 = value;
                OnPropertyChanged(nameof(Arguments_6_添加字幕));
            }
        }
        private string _Arguments_7_添加水印;
        public string Arguments_7_添加水印
        {
            get { return _Arguments_7_添加水印; }
            set
            {
                _Arguments_7_添加水印 = value;
                OnPropertyChanged(nameof(Arguments_7_添加水印));
            }
        }

        private string _ExportPath;
        public string ExportPath
        {
            get { return _ExportPath; }
            set
            {
                _ExportPath = value;
                OnPropertyChanged(nameof(ExportPath));
            }
        }

        private int __CutPixel_横版左;
        public int CutPixel_横版左
        {
            get { return __CutPixel_横版左; }
            set
            {
                __CutPixel_横版左 = value;
                OnPropertyChanged(nameof(CutPixel_横版左));
            }
        }
        private int __CutPixel_横版右;
        public int CutPixel_横版右
        {
            get { return __CutPixel_横版右; }
            set
            {
                __CutPixel_横版右 = value;
                OnPropertyChanged(nameof(CutPixel_横版右));
            }
        }
        private int _offset_横版左;
        public int offset_横版左
        {
            get { return _offset_横版左; }
            set
            {
                _offset_横版左 = value;
                OnPropertyChanged(nameof(offset_横版左));
            }
        }
        private int _offset_横版右;
        public int offset_横版右
        {
            get { return _offset_横版右; }
            set
            {
                _offset_横版右 = value;
                OnPropertyChanged(nameof(offset_横版右));
            }
        }

        private double _RowHeight;
        public double RowHeight
        {
            get { return _RowHeight; }
            set
            {
                _RowHeight = value;
                OnPropertyChanged(nameof(RowHeight));
            }
        }
        private double _ColumnWidth;
        public double ColumnWidth
        {
            get { return _ColumnWidth; }
            set
            {
                _ColumnWidth = value;
                OnPropertyChanged(nameof(ColumnWidth));
            }
        }
        public int MaxId { get; set; }
        public long CutSize { get; set; }

        private Visibility _PreViewVisibility;
        public Visibility PreViewVisibility
        {
            get { return _PreViewVisibility; }
            set
            {
                _PreViewVisibility = value;
                OnPropertyChanged(nameof(PreViewVisibility));
            }
        }
        private bool _IsCancel;
        public bool IsCancel
        {
            get { return _IsCancel; }
            set
            {
                _IsCancel = value;
                OnPropertyChanged(nameof(IsCancel));
            }
        }
        private bool _IsConfirm;
        public bool IsConfirm
        {
            get { return _IsConfirm; }
            set
            {
                _IsConfirm = value;
                OnPropertyChanged(nameof(IsConfirm));
            }
        }
        private bool _IsIgnore;
        public bool IsIgnore
        {
            get { return _IsIgnore; }
            set
            {
                _IsIgnore = value;
                OnPropertyChanged(nameof(IsIgnore));
            }
        }

        private string _水印坐标Y;
        public string 水印坐标Y
        {
            get { return _水印坐标Y; }
            set
            {
                _水印坐标Y = value;
                OnPropertyChanged(nameof(水印坐标Y));
            }
        }
        private string _水印坐标X;
        public string 水印坐标X
        {
            get { return _水印坐标X; }
            set
            {
                _水印坐标X = value;
                OnPropertyChanged(nameof(水印坐标X));
            }
        }
        private string _水印路径;
        public string 水印路径
        {
            get { return _水印路径; }
            set
            {
                _水印路径 = value;
                OnPropertyChanged(nameof(水印路径));
            }
        }
        private ImageSource _水印预览;
        public ImageSource 水印预览
        {
            get { return _水印预览; }
            set
            {
                _水印预览 = value;
                OnPropertyChanged(nameof(水印预览));
            }
        }

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

        private string _FontSize;
        public string FontSize
        {
            get { return _FontSize; }
            set
            {
                _FontSize = value;
                OnPropertyChanged(nameof(FontSize));
            }
        }
        private Color _FontColor;
        public Color FontColor
        {
            get { return _FontColor; }
            set
            {
                _FontColor = value;
                OnPropertyChanged(nameof(FontColor));
            }
        }
        private string _FontStyle;
        public string FontStyle
        {
            get { return _FontStyle; }
            set
            {
                _FontStyle = value;
                OnPropertyChanged(nameof(FontStyle));
            }
        }
        private string _TextHeight;
        public string TextHeight
        {
            get { return _TextHeight; }
            set
            {
                _TextHeight = value;
                OnPropertyChanged(nameof(TextHeight));
            }
        }
        private string _TextWidth;
        public string TextWidth
        {
            get { return _TextWidth; }
            set
            {
                _TextWidth = value;
                OnPropertyChanged(nameof(TextWidth));
            }
        }
        private string _TextDeepth;
        public string TextDeepth
        {
            get { return _TextDeepth; }
            set
            {
                _TextDeepth = value;
                OnPropertyChanged(nameof(TextDeepth));
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

        private Color _FillColor横转竖;
        public Color FillColor横转竖
        {
            get { return _FillColor横转竖; }
            set
            {
                _FillColor横转竖 = value;
                OnPropertyChanged(nameof(FillColor横转竖));
            }
        }
        private bool _IsSame横转竖;
        public bool IsSame横转竖
        {
            get { return _IsSame横转竖; }
            set
            {
                _IsSame横转竖 = value;
                OnPropertyChanged(nameof(IsSame横转竖));
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
