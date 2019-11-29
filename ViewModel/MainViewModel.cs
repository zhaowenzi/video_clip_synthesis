using DMSkin.Core;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace 素材合成.ViewModel
{
    public class MainViewModel : DMSkin.Core.ViewModelBase
    {
        public  MainViewModel( )
        {
            _Page2ImagemModelList = new ObservableCollection<Model.Page2ImageModel>();
            _CommandList = new ObservableCollection<Model.Instruction>();
            _PlayLists = new ObservableCollection<Model.PlayListNode>();
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

        private ObservableCollection<Model.Page2ImageModel> _Page2ImagemModelList;
        public ObservableCollection<Model.Page2ImageModel> Page2ImagemModelList
        {
            get { return _Page2ImagemModelList; }
            set
            {
                _Page2ImagemModelList = value;
                OnPropertyChanged(nameof(Page2ImagemModelList));
            }
        }


        private ObservableCollection<Model.PlayListNode> _PlayLists;
        public ObservableCollection<Model.PlayListNode> PlayLists
        {
            get { return _PlayLists; }
            set
            {
                _PlayLists = value;
                OnPropertyChanged(nameof(PlayLists));
            }
        }

        

        #region Command
        public ICommand btnDeletePlayListNode => new DelegateCommand(obj =>
        {
            TextBlock deleteInfo = obj as TextBlock;
            for (int i = 0; i < PlayLists.Count; i++)
            {
                //if(PlayLists[i].VideoPath== deleteInfo.Text)
                //{
                //    PlayLists.RemoveAt(i);
                //    break;
                //}
            }
        });
        #endregion
    }
}
