using System;
using System.Collections.Generic;
using System.IO;
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
using DevExpress.Xpf.Bars;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using Yamed.Core;
using Yamed.Server;

namespace Yamed.Settings
{
    /// <summary>
    /// Логика взаимодействия для ReportControl.xaml
    /// </summary>
    public partial class ReportControl : UserControl
    {
        private object _row;
        public ReportControl(object row)
        {
            InitializeComponent();
            _row = row;
        }

        private void CloseDocumentButton_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            if (reportDesigner.ActiveDocument != null)
                reportDesigner.ActiveDocument.Close();
        }

        private void AboutButton_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            MessageBox.Show("This example demonstrates how to customize the Report Designer's toolbar.", "About");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            reportDesigner.OpenDocument(new XtraReport());
        }

        private void SaveToSql_OnItemClick(object sender, ItemClickEventArgs e)
        {
            Stream mStream = new MemoryStream();

            reportDesigner.ActiveDocument.Save(mStream);
            mStream.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(mStream);

            ObjHelper.SetAnonymousValue(ref _row, reader.ReadToEnd(), "Template");
            var upd = Reader2List.CustomUpdateCommand("YamedReports", _row, "ID");
            Reader2List.CustomExecuteQuery(upd, SprClass.LocalConnectionString);

            mStream.Close();
            reader.Close();
        }

        private void ReportControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            //var rep = new XtraReport();
            var rl = (string)ObjHelper.GetAnonymousValue(_row, "Template");

            if (!string.IsNullOrWhiteSpace(rl))
            {
                MemoryStream stream = new MemoryStream();
                StreamWriter writer = new StreamWriter(stream);

                writer.Write(rl);
                writer.Flush();
                stream.Seek(0, SeekOrigin.Begin);

                //rep.LoadLayout(stream);
                reportDesigner.OpenDocument(stream);

                stream.Close();
                writer.Close();
            }
            else
            {
                //var p = new Parameter
                //{
                //    Name = "ID",
                //    Type = typeof(int)
                //};

                //rep.Parameters.Add(p);
            }


        }
    }
}
