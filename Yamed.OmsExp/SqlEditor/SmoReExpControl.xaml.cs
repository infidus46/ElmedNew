using System.Linq;
using System.Windows.Controls;
using Yamed.Server;

namespace Yamed.OmsExp.SqlEditor
{
    /// <summary>
    /// Логика взаимодействия для SmoReExpControl.xaml
    /// </summary>
    public partial class SmoReExpControl : UserControl
    {
        public SmoReExpControl()
        {
            InitializeComponent();
            smoBox.DataContext = SprClass.smo.Where(x=>x.smocod.StartsWith("46"));

        }
    }
}
