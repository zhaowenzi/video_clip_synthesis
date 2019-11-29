
using DMSkin.Core;
using System.Windows.Input;

namespace 素材合成.ViewModel
{
    public class SampleViewModel:DMSkin.Core.ViewModelBase
    {
        public SampleViewModel()
        {

        }
        #region 属性
        Model.SampleModel _SampleModel;
        public Model.SampleModel SampleModel
        {
            get { return _SampleModel; }
            set
            {
                _SampleModel = value;
                OnPropertyChanged(nameof(SampleModel));
            }
        }

        #endregion

        #region 命令

        /// <summary>
        /// 取消点击
        /// </summary>    
        public ICommand BtnCancel => new DelegateCommand(obj =>
        {
           // (obj as Views.Shutdown).Close();
        });
        /// <summary>
        /// 确认点击
        /// </summary>
        public ICommand BtnConfirm => new DelegateCommand(obj =>
        {
            //写入缓存中的配置
           // MessageBox.Show("确触发");
          //  (obj as Views.Shutdown).Close();
        });


        #endregion
    }
}
