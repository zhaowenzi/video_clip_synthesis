using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace 素材合成
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        
        #region Constructor
        public UserControl1()
        {
            InitializeComponent();
        }

        #endregion

        long count = 1;
        private void Canvas_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            System.Windows.Media.Animation.Storyboard storyboard = myCanvas.Resources["myStoryboard"] as System.Windows.Media.Animation.Storyboard;
            if (count % 2 == 0)
            {
                storyboard.Pause(myCanvas);
            }
            else
            {
                storyboard.Begin(myCanvas, true);
            }
            count++;
            // Console.WriteLine("ssss:" + count);

        }
    }
}
