namespace 素材合成.Model
{
    public class SampleModel:DMSkin.Core.ViewModelBase
    {
        private string _name;
        /// <summary>
        /// 文件上传的名字 preview
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }
}
