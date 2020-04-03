using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using Ionic.Zip;
using Microsoft.Win32;
using Yamed.Control;
using Yamed.Core;
using Yamed.Emr;
using Yamed.Entity;
using Yamed.Server;
using System.Data.SqlClient;

namespace Yamed.Oms
{
    /// <summary>
    /// Логика взаимодействия для PacientReserveControl.xaml
    /// </summary>
    public partial class ReestrControl : UserControl
    {
        private bool _isSaved;
        private D3_SCHET_OMS _sc;
        
        public ReestrControl()
        {
            InitializeComponent();
        }

        public ReestrControl(D3_SCHET_OMS sc)
        {
            InitializeComponent();
            
            _sc = sc;
            SchetRegisterGrid1.Scids = new List<int> () {_sc.ID};
        }

        private void ZslRefresh_OnClick(object sender, RoutedEventArgs e)
        {
            SchetRegisterGrid1._linqInstantFeedbackDataSource.Refresh();
        }

        private void ZslAdd_OnClick(object sender, RoutedEventArgs e)
        {
            var zslTempl = new SluchTemplateD3();
            zslTempl.BindEmptySluch2(_sc);
            СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            {
                Header = "Случай поликлиники",
                MyControl = zslTempl,
                IsCloseable = "True",
                //TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
            });
        }

        private void ZslEdit_OnClick(object sender, RoutedEventArgs e)
        {
            var tab = SchetRegisterGrid1;
            //var sl = Reader2List.CustomSelect<SLUCH>($"Select * From D3_ZSL_OMS Where ID={ObjHelper.GetAnonymousValue(DxHelper.GetSelectedGridRow(tab.gridControl1), "ID")}",
            //        SprClass.LocalConnectionString).Single();
            if (SprClass.Region != "37")
            {
                var id = tab.GetSelectedRowId();
                var slt = new SluchTemplateD31(SchetRegisterGrid1.gridControl1);
                slt.BindSluch(id, _sc);

                СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
                {
                    Header = "Поликлиника",
                    MyControl = slt,
                    IsCloseable = "True",
                    //TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
                });
            }
            else if (SprClass.Region == "37")
            {
                var id = tab.GetSelectedRowId();
                var slt = new SluchTemplateD31Ivanovo(SchetRegisterGrid1.gridControl1);
                slt.BindSluch(id, _sc);

                СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
                {
                    Header = "Поликлиника",
                    MyControl = slt,
                    IsCloseable = "True",
                    //TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
                });
            }
        }

        private void Zsl_OnClick(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            DxHelper.GetSelectedGridRowsAsync(ref SchetRegisterGrid1.gridControl1);
            bool isLoaded = false;
            SchetRegisterGrid1.gridControl1.IsEnabled = false;

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        if (SchetRegisterGrid1.gridControl1.IsAsyncOperationInProgress == false)
                        {
                            isLoaded = true;
                        }
                    });
                    if (isLoaded) break;
                    Thread.Sleep(200);
                }

            }).ContinueWith(lr =>
            {
                if (DxHelper.LoadedRows.Count > 0)
                {
                    var ids =
                        DxHelper.LoadedRows.Select(x => ObjHelper.GetAnonymousValue(x, "ID"))
                            .OfType<int>()
                            .Distinct()
                            .ToArray();

                    MessageBoxResult resdel = MessageBox.Show($"Удалить {ids.Length} записей?", "Удаление",
MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (resdel == MessageBoxResult.Yes)
                    {
                        try
                        {
                            var tab = SchetRegisterGrid1;
                            var idstr = ObjHelper.GetIds(ids);
                            var connectionString = SprClass.LocalConnectionString;
                            SqlConnection con = new SqlConnection(connectionString);
                            SqlCommand comm = new SqlCommand($@"select convert(nvarchar, d3_pid)+',' from d3_zsl_oms where id in({idstr}) for xml path('') ", con);
                            con.Open();
                            string pacid=(string)comm.ExecuteScalar();
                            con.Close();
                            string pacids= pacid.Substring(0, pacid.Length - 1);
                            Reader2List.CustomExecuteQuery($"DELETE D3_USL_OMS WHERE D3_ZSLID in({idstr})", SprClass.LocalConnectionString);
                            //Reader2List.CustomExecuteQuery($"DELETE SLUCH_DS2 WHERE SLID={id}", SprClass.LocalConnectionString);
                            //Reader2List.CustomExecuteQuery($"DELETE SLUCH_DS3 WHERE SLID={id}", SprClass.LocalConnectionString);
                            Reader2List.CustomExecuteQuery($"DELETE D3_SL_OMS WHERE D3_ZSLID in({idstr})", SprClass.LocalConnectionString);
                            
                            Reader2List.CustomExecuteQuery($"DELETE D3_ZSL_OMS WHERE ID in({idstr})", SprClass.LocalConnectionString);
                            Reader2List.CustomExecuteQuery($"DELETE D3_PACIENT_OMS WHERE ID in({pacids})", SprClass.LocalConnectionString);
                            DXMessageBox.Show($"Удаление успешно выполнено. Удалено {ids.Length} записей");
                            tab._linqInstantFeedbackDataSource.Refresh();
                        }
                        catch (Exception exception)
                        {
                            DXMessageBox.Show(exception.Message + Environment.NewLine +
                                              exception.InnerException?.Message);
                        }
                        finally
                        {
                            SchetRegisterGrid1.gridControl1.IsEnabled = true;
                            DxHelper.LoadedRows.Clear();
                        }
                        


                    }
                   

                }
                else
                {
                    DXMessageBox.Show("Не выбрано ни одной записи");

                }
                SchetRegisterGrid1.gridControl1.IsEnabled = true;
                DxHelper.LoadedRows.Clear();

            }, TaskScheduler.FromCurrentSynchronizationContext());

            
        }

        private void ZslCompile_OnClick(object sender, RoutedEventArgs e)
        {
            //            var sluch = SqlReader.Select($@"
            //SELECT zsl.ID ZID, sl.ID ID, NPOLIS, PROFIL,  left(ds1, 3) DS1, RSLT, DATE_1
            //FROM[D3_ZSL_OMS] zsl
            //join[D3_SL_OMS] sl on sl.d3_zslid = zsl.id and sl.P_CEL = '1.3'
            //join D3_PACIENT_OMS p on d3_pid = p.id
            //where zsl.D3_SCID = {_sc.ID} AND RSLT in (301, 304) and OS_SLUCH_REGION IS NULL and sl.PROFIL not in (63,85,86,87,88,89,90) AND IDSP =29", SprClass.LocalConnectionString);

            //            var gr = from c in sluch
            //                group c by new { NPOLIS = c.GetValue("NPOLIS"), PROFIL = c.GetValue("PROFIL"), DS1 = c.GetValue("DS1") }
            //                into grp
            //                where grp.Count() > 1
            //                select new {grp.Key, grp };
            //            foreach (var g in gr.Where(x=>x.grp.Any(r=> (int)r.GetValue("RSLT") == 301) && x.grp.Any(r => (int)r.GetValue("RSLT") == 304)))
            //            {
            //                var z = g.grp.OrderBy(x=>x.GetValue("DATE_1"));
            //                var zid = z.First().GetValue("ZID");

            //                foreach (var sl in z)
            //                {
            //                    var s = SqlReader.Select($@"Select * from D3_SL_OMS WHERE ID = {sl.GetValue("ID")}",
            //                        SprClass.LocalConnectionString)[0];
            //                    s.SetValue("D3_ZSLID", zid);
            //                    s.SetValue("P_CEL", "3.0");
            //                    var upd = Reader2List.CustomUpdateCommand("D3_SL_OMS", s, "ID");
            //                    Reader2List.CustomExecuteQuery(upd, SprClass.LocalConnectionString);

            //                }
            //                var zs = SqlReader.Select($@"Select * from D3_ZSL_OMS WHERE ID = {zid}",
            //                    SprClass.LocalConnectionString)[0];
            //                zs.SetValue("IDSP", 30);
            //                zs.SetValue("RSLT", 301);
            //                zs.SetValue("DATE_Z_2", z.Last().GetValue("DATE_1"));

            //                var zupd = Reader2List.CustomUpdateCommand("D3_ZSL_OMS", zs, "ID");
            //                Reader2List.CustomExecuteQuery(zupd, SprClass.LocalConnectionString);

            //            }

            //            Reader2List.CustomExecuteQuery($@"            
            //delete D3_ZSL_OMS
            //from D3_ZSL_OMS zsl
            //left
            //join D3_SL_OMS sl on zsl.ID = sl.D3_ZSLID
            //where sl.id is null and D3_SCID = {_sc.ID}", SprClass.LocalConnectionString);

            var qxml = SqlReader.Select($@"exec p_magic_visit {_sc.ID}"
                , SprClass.LocalConnectionString);
        }

        private void FlkLoad_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "OMS File |*.oms;*.zip";


            var result = openFileDialog.ShowDialog();

            var rfile = openFileDialog.FileName;

            string ft = "";
            string zapfn = "";
            var zapms = new MemoryStream();

            if (result != true) return;

            ((Button)sender).IsEnabled = false;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (ZipFile zip = ZipFile.Read(rfile))
                    {
                        foreach (ZipEntry zipEntry in zip)
                        {
                            if (zipEntry.FileName.StartsWith("V") )
                            {
                                ft = "flk";
                                zapfn = zipEntry.FileName;
                                zipEntry.Extract(zapms);
                                zapms.Position = 0;
                            }

                            if (zipEntry.FileName.StartsWith("O") )
                            {
                                ft = "osp";
                                zapfn = zipEntry.FileName;
                                zipEntry.Extract(zapms);
                                zapms.Position = 0;
                            }
                        }
                    }
                    //"0x" + BitConverter.ToString(arraytoinsert).Replace("-", "")
                }
                catch (Exception ex)
                {
                    Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        DXMessageBox.Show(ex.Message + Environment.NewLine + ex.InnerException?.Message);
                    });
                }


                var zapsr = new StreamReader(zapms, Encoding.Default);
                string zapxml = zapsr.ReadToEnd();
                zapms.Dispose();
                zapms.Close();

                if (string.IsNullOrEmpty(zapxml))
                {
                    //Reader2List.CustomExecuteQuery($@"Update DOX_SCHET SET DOX_STATUS=12 ", SprClass.LocalConnectionString);
                }
                else
                {
                    try
                    {
                        var q = "";
                        if (ft == "flk")
                            q = $@"    EXEC p_oms_load_flk '{zapxml}', {_sc.ID}";

                        if (ft == "osp")
                            q = $@"    EXEC p_oms_load_osp '{zapxml}', {_sc.ID}";

                        Reader2List.CustomExecuteQuery(q, SprClass.LocalConnectionString);
                        q = null;
                        zapxml = null;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            DXMessageBox.Show(ex.Message + Environment.NewLine + ex.InnerException?.Message);
                        });
                        //Reader2List.CustomExecuteQuery($@"Update DOX_SCHET SET DOX_STATUS=13 WHERE ID = {id}", _connectionString);
                        //return;
                    }
                }
                zapsr.Dispose();
                zapsr.Close();

                //Console.WriteLine("Распакован " + id);

                GC.WaitForPendingFinalizers();
                GC.Collect();

            }).ContinueWith(x =>
            {
                DXMessageBox.Show("Загрузка успешно завершена");
                SchetRegisterGrid1._linqInstantFeedbackDataSource.Refresh();
                //SchetRegisterGrid1.gridControl1.ClearGrouping();
                //SchetRegisterGrid1.gridControl1.GroupBy("FLK_COMENT");
                if (ft == "flk") SchetRegisterGrid1.FlkGroup();

                ((Button)sender).IsEnabled = true;


            }, TaskScheduler.FromCurrentSynchronizationContext());
        }


        private void Perenos_OnClick(object sender, RoutedEventArgs e)
        {
            DxHelper.GetSelectedGridRowsAsync(ref SchetRegisterGrid1.gridControl1);
            bool isLoaded = false;
            SchetRegisterGrid1.gridControl1.IsEnabled = false;

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Dispatcher.BeginInvoke((Action) delegate()
                    {
                        if (SchetRegisterGrid1.gridControl1.IsAsyncOperationInProgress == false)
                        {
                            isLoaded = true;
                        }
                    });
                    if (isLoaded) break;
                    Thread.Sleep(200);
                }
            }).ContinueWith(lr =>
            {

                var window = new DXWindow
                {
                    ShowIcon = false,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Content = new ReestrChooseControl(_sc),
                    Title = "Выбор реестра для переноса",
                    Width = 350, Height = 300
                };
                window.ShowDialog();

                SchetRegisterGrid1.gridControl1.IsEnabled = true;
                DxHelper.LoadedRows.Clear();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void TestAshurcov(object sender, RoutedEventArgs e)
        {

            DxHelper.GetSelectedGridRowsAsync(ref SchetRegisterGrid1.gridControl1);
            bool isLoaded = false;
            SchetRegisterGrid1.gridControl1.IsEnabled = false;

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        if (SchetRegisterGrid1.gridControl1.IsAsyncOperationInProgress == false)
                        {
                            isLoaded = true;
                        }
                    });
                    if (isLoaded) break;
                    Thread.Sleep(200);
                }

            }).ContinueWith(lr =>
            {
                if (DxHelper.LoadedRows.Count > 0)
                {
                    var ids =
                        DxHelper.LoadedRows.Select(x => ObjHelper.GetAnonymousValue(x, "ID"))
                            .OfType<int>()
                            .Distinct()
                            .ToArray();

                    MessageBoxResult result = MessageBox.Show($"Объединить {ids.Length} записей в обращение?", "Объединение",
MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            var idstr = ObjHelper.GetIds(ids);
                            var q = $@"EXEC [dbo].[p_MergingEvents] '{idstr}'";
                            Reader2List.CustomExecuteQuery(q, SprClass.LocalConnectionString);
                        }
                        catch (Exception exception)
                        {
                            DXMessageBox.Show(exception.Message + Environment.NewLine +
                                              exception.InnerException?.Message);
                        }
                        finally
                        {
                            SchetRegisterGrid1.gridControl1.IsEnabled = true;
                            DxHelper.LoadedRows.Clear();
                        }
                        DXMessageBox.Show("Объединение успешно выполнено");


                    }
                    DXMessageBox.Show("Не выбрано ни одной записи");

                }
                else
                {
                    DXMessageBox.Show("Не выбрано ни одной записи");

                }
                SchetRegisterGrid1.gridControl1.IsEnabled = true;
                DxHelper.LoadedRows.Clear();

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ZslAdd31_OnClick(object sender, RoutedEventArgs e)
        {
            if (SprClass.Region != "37")
            {
                var zslTempl = new SluchTemplateD31(SchetRegisterGrid1.gridControl1);
                zslTempl.BindEmptySluch2(_sc);
                СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
                {
                    Header = "Случай поликлиники",
                    MyControl = zslTempl,
                    IsCloseable = "True"
                });
            }
            else if (SprClass.Region == "37")
            {
                var zslTempl = new SluchTemplateD31Ivanovo(SchetRegisterGrid1.gridControl1);
                zslTempl.BindEmptySluch2(_sc);
                СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
                {
                    Header = "Случай поликлиники",
                    MyControl = zslTempl,
                    IsCloseable = "True"
                });
            }
        }

        private void ExcelButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel File (*.xlsx)|*.xlsx";

            if (saveFileDialog.ShowDialog() == true)
                SchetRegisterGrid1.gridControl1.View.ExportToXlsx(saveFileDialog.FileName);
        }
    }
}
