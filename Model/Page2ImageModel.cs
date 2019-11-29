using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 素材合成.Model
{
    public class Page2ImageModel : DMSkin.Core.ViewModelBase
    {
        private string _Key;
        public string Key
        {
            get { return _Key; }
            set
            {
                _Key = value;
                OnPropertyChanged(nameof(Key));
            }
        }

        private bool _IsChecked;
        public bool IsChecked
        {
            get { return _IsChecked; }
            set
            {
                _IsChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        private string _Content;
        public string Content
        {
            get { return _Content; }
            set
            {
                _Content = value;
                OnPropertyChanged(nameof(Content));
            }
        }
        private double _Command;
        public double Command
        {
            get { return _Command; }
            set
            {
                _Command = value;
                OnPropertyChanged(nameof(Command));
            }
        }

        private double _BorderWidth;
        public double BorderWidth
        {
            get { return _BorderWidth; }
            set
            {
                _BorderWidth = value;
                OnPropertyChanged(nameof(BorderWidth));
            }
        }

        private double _BorderHeight;
        public double BorderHeight
        {
            get { return _BorderHeight; }
            set
            {
                _BorderHeight = value;
                OnPropertyChanged(nameof(BorderHeight));
            }
        }

       
    }
}
