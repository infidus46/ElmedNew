using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Core;
using Yamed.Control;
using Yamed.Core;
using Yamed.Server;

namespace Yamed.OmsExp.ExpEditors
{
    /// <summary>
    /// Логика взаимодействия для ReqControl.xaml
    /// </summary>
    public partial class ReqControl : UserControl
    {
        public ReqControl()
        {
            InitializeComponent();

            ReqDateEdit.EditValue = SprClass.WorkDate;

        }

        public ReqControl(int v)
        {
            InitializeComponent();

            _v = v;

            if (_v == 1)
            {
                ReqDateLayoutItem.Visibility = Visibility.Collapsed;
                ReqTextLayoutItem.Label = "Комментарий";
            }

        }

        private readonly int _v;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_v == 1)
            {
                StringBuilder cmd = new StringBuilder();

                foreach (var row in DxHelper.LoadedRows)
                {
                    cmd.AppendLine(
                        String.IsNullOrWhiteSpace((string) ReqTextEdit.EditValue)
                            ? $@"UPDATE D3_ZSL_OMS SET USER_COMENT = NULL WHERE ID = {ObjHelper.GetAnonymousValue(row,
                                "ID")}"
                            : $@"UPDATE D3_ZSL_OMS SET USER_COMENT = '{ReqTextEdit.EditValue}' WHERE ID = {ObjHelper
                                .GetAnonymousValue(row, "ID")}");
                }
                Reader2List.CustomExecuteQuery(cmd.ToString(), SprClass.LocalConnectionString);
            }
            else
            {
                var idrow = (IList)Reader2List.CustomAnonymousSelect($@"
INSERT INTO [dbo].[YamedRequests]           ([Name], [ReqDate], [UserID], [ReqNum])
     VALUES  ('{ReqTextEdit.EditValue}', '{(DateTime)ReqDateEdit.EditValue:yyyyMMdd}', {SprClass.userId}, '{ReqNumEdit.EditValue}')
select SCOPE_IDENTITY() ID", SprClass.LocalConnectionString);
                var id = ObjHelper.GetAnonymousValue(idrow[0], "ID");

                StringBuilder cmd = new StringBuilder();

                foreach (var row in DxHelper.LoadedRows)
                {
                    cmd.AppendLine(
                        $@"UPDATE D3_ZSL_OMS SET ReqID = {id} WHERE ID = {ObjHelper.GetAnonymousValue(row, "ID")}");
                }
                Reader2List.CustomExecuteQuery(cmd.ToString(), SprClass.LocalConnectionString);

            }

            ((DXWindow)this.Parent).Close();

        }
    }
}
