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
using Yamed.Server;

namespace Yamed.Ambulatory
{
    /// <summary>
    /// Логика взаимодействия для WorkSpaceTile.xaml
    /// </summary>
    public partial class WorkSpaceTile : UserControl
    {
        public WorkSpaceTile()
        {
            InitializeComponent();
        }

        private void Tile_OnClick(object sender, EventArgs e)
        {
            SqlReader.Select("Select * from Yamed_View_Ophthalmologist", SprClass.LocalConnectionString);
        }
    }
}
