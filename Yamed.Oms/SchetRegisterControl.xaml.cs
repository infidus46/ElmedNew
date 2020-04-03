using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using Ionic.Zip;
using Microsoft.Win32;
using Yamed.Control;
using Yamed.Control.Editors;
using Yamed.Core;
using Yamed.Emr;
using Yamed.Entity;
using Yamed.OmsExp.ExpEditors;
using Yamed.Reports;
using Yamed.Server;
using PreviewControl = Yamed.Reports.PreviewControl;

namespace Yamed.Oms
{
    /// <summary>
    /// Логика взаимодействия для PacientReserveControl.xaml
    /// </summary>
    public partial class SchetRegisterControl : UserControl
    {
        private bool _isSaved;
        private string _reqCmd;
        private D3_SCHET_OMS _sc;
        public int? _arid;

        public SchetRegisterControl(D3_SCHET_OMS sc)
        {
            InitializeComponent();
            if (SprClass.Region != "22")
            {
                Export22.IsVisible = false;
                Load22.IsVisible = false;
            }
            if (SprClass.ProdSett.OrgTypeStatus == OrgType.Lpu)
            {
                zaprosPD.Visibility = Visibility.Collapsed;
                zapPD.IsVisible = false;
                aktExp.IsVisible = false;
                mek.IsVisible = false;
                reexpertise.IsVisible = false;
                docs.IsVisible = false;
                sank.IsVisible = false;
            }
            else if (SprClass.ProdSett.OrgTypeStatus == OrgType.Smo || SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms)
            {
                add_sl.IsVisible = false;
                sl_del.IsVisible = false;
                flk_osp.IsVisible = false;
                perenos.IsVisible = false;
                compilezsl.IsVisible = false;
            }
            _sc = sc;
            SchetRegisterGrid1.Scids = new List<int>() { _sc.ID };
        }
        public SchetRegisterControl()
        {
            InitializeComponent();
            if (SprClass.Region != "22")
            {
                Export22.IsVisible = false;
                Load22.IsVisible = false;
            }
            if (SprClass.ProdSett.OrgTypeStatus == OrgType.Lpu)
            {
                zaprosPD.Visibility = Visibility.Collapsed;
                zapPD.IsVisible = false;
                aktExp.IsVisible = false;
                mek.IsVisible = false;
                reexpertise.IsVisible = false;
                docs.IsVisible = false;
                sank.IsVisible = false;
            }
            else if (SprClass.ProdSett.OrgTypeStatus == OrgType.Smo || SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms)
            {
                add_sl.IsVisible = false;
                sl_del.IsVisible = false;
                flk_osp.IsVisible = false;
                perenos.IsVisible = false;
                compilezsl.IsVisible = false;
            }
        }

        private List<int> _scids;
        public SchetRegisterControl(List<int> scids)
        {
            InitializeComponent();
            if (SprClass.Region != "22")
            {
                Export22.IsVisible = false;
                Load22.IsVisible = false;
            }
            _scids = scids;

            SchetRegisterGrid1.Scids = scids;
            SchetRegisterGrid1.BindDataZsl();
            if (SprClass.ProdSett.OrgTypeStatus == OrgType.Lpu)
            {
                zaprosPD.Visibility = Visibility.Collapsed;
                zapPD.IsVisible = false;
                aktExp.IsVisible = false;
                mek.IsVisible = false;
                reexpertise.IsVisible = false;
                docs.IsVisible = false;
                sank.IsVisible = false;
            }
            else if (SprClass.ProdSett.OrgTypeStatus == OrgType.Smo || SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms)
            {
                add_sl.IsVisible = false;
                sl_del.IsVisible = false;
                flk_osp.IsVisible = false;
                perenos.IsVisible = false;
                compilezsl.IsVisible = false;
            }

            if (scids.Any())
            {
                var ids = "(";
                foreach (var sc in scids)
                {
                    ids += sc + ", ";
                }
                ids += ")";
                ids = ids.Replace(", )", ")");
                _reqCmd = $@"
Select distinct r.* from YamedRequests r
join D3_ZSL_OMS zsl on r.ID = zsl.ReqID
where zsl.D3_SCID in {ids}";
                ReqBind();

                ZaprosCount();
            }
        }

        void ReqBind()
        {
            if (_reqCmd == null) return;

            ReqGridControl.DataContext = Reader2List.CustomAnonymousSelect(_reqCmd, SprClass.LocalConnectionString);
        }

        private void ZslRefresh_OnClick(object sender, RoutedEventArgs e)
        {
            SchetRegisterGrid1._linqInstantFeedbackDataSource.Refresh();
        }


        //private void ZslEdit_OnClick(object sender, RoutedEventArgs e)
        //{
        //    var tab = SchetRegisterGrid1;
        //    //var sl = Reader2List.CustomSelect<SLUCH>($"Select * From D3_ZSL_OMS Where ID={ObjHelper.GetAnonymousValue(DxHelper.GetSelectedGridRow(tab.gridControl1), "ID")}",
        //    //        SprClass.LocalConnectionString).Single();

        //    var id = (int) ObjHelper.GetAnonymousValue(DxHelper.GetSelectedGridRow(tab.gridControl1), "ID");
        //    var slt = new SluchTemplateD3(SchetRegisterGrid1.gridControl1);
        //    slt.BindSluch(id);

        //    СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
        //    {
        //        Header = "Карта пациента",
        //        MyControl = slt,
        //        IsCloseable = "True",
        //        //TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
        //    });
        //}


        private void SankDel_OnClick(object sender, RoutedEventArgs e)
        {
            DxHelper.GetSelectedGridRowsAsync(ref SchetRegisterGrid1.gridControl1);
            bool isLoaded = false;
            SchetRegisterGrid1.gridControl1.IsEnabled = false;

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(200);

                    if (isLoaded) break;

                    Dispatcher.BeginInvoke((Action) delegate()
                    {
                        if (SchetRegisterGrid1.gridControl1.IsAsyncOperationInProgress == false)
                        {
                            isLoaded = true;
                        }
                    });
                }

            }).ContinueWith(lr =>
            {
                var sankGroupDelete = new SankGroupDelete(DxHelper.LoadedRows);
                sankGroupDelete.ShowDialog();
                SchetRegisterGrid1.gridControl1.IsEnabled = true;
                DxHelper.LoadedRows.Clear();
            }, TaskScheduler.FromCurrentSynchronizationContext());

        }

        private void Req_OnClick(object sender, RoutedEventArgs e)
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
                bool isMek = false;
                var sluids = new List<int>();//gridControl1.SelectedItems.OfType<SluPacClass>().Select(x=>x.ID).ToArray();
                var rows = (IEnumerable<object>)DxHelper.LoadedRows;
                foreach (var row in DxHelper.LoadedRows)
                // Parallel.ForEach(rows, (x) =>
                {
                    if ((int?)ObjHelper.GetAnonymousValue(row, "OPLATA") == 1 || SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms)
                    {
                        sluids.Add((int)ObjHelper.GetAnonymousValue(row, "ID"));
                    }
                    else
                    {
                        isMek = true;
                    }
                }//});
                
                if (!sluids.Any())
                {
                    DXMessageBox.Show("Не выбрано ни одной записи или записи имеют некорректный статус оплаты");
                    SchetRegisterGrid1.gridControl1.IsEnabled = true;
                    DxHelper.LoadedRows.Clear();
                    return;
                }

                if (isMek)
                {
                    DXMessageBox.Show("Внимание выбраны записи с некорректным статусом оплаты, которые исключены из запроса первичной документации");
                }

                var window = new DXWindow
                {
                    ShowIcon = false,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Content = new AktMeeChoose(sluids.ToArray()),
                    Title = "Запрос на первичную документацию",
                    SizeToContent = SizeToContent.WidthAndHeight
                };
                window.ShowDialog();

                SchetRegisterGrid1.gridControl1.IsEnabled = true;
                DxHelper.LoadedRows.Clear();

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ReqDel_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Удалить выбранные запросы первичной документации?", "Удаление",
    MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }

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
                StringBuilder cmd = new StringBuilder();
                foreach (var row in DxHelper.LoadedRows)
                {
                    cmd.AppendLine(
                        string.Format(
                            @"UPDATE D3_ZSL_OMS SET EXP_COMENT = NULL, EXP_TYPE = NULL, EXP_DATE = NULL, USERID = NULL WHERE ID = {0}",
                            ObjHelper.GetAnonymousValue(row, "ID")));
                }
                Reader2List.CustomExecuteQuery(cmd.ToString(), SprClass.LocalConnectionString);


                SchetRegisterGrid1.gridControl1.IsEnabled = true;
                DxHelper.LoadedRows.Clear();

            }, TaskScheduler.FromCurrentSynchronizationContext());

        }

        private void AddMee_OnClick(object sender, RoutedEventArgs e)
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

                var window = new DXWindow
                {
                    ShowIcon = false,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Content = new MedicExpControl(2, arid:_arid),
                    Title = "Акт МЭЭ",
                    SizeToContent = SizeToContent.Height,
                    Width = 1450
                };
                window.ShowDialog();

                SchetRegisterGrid1.gridControl1.IsEnabled = true;
                DxHelper.LoadedRows.Clear();

            }, TaskScheduler.FromCurrentSynchronizationContext());

        }

        private void AddEkmp_OnClick(object sender, RoutedEventArgs e)
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

                var window = new DXWindow
                {
                    ShowIcon = false,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Content = new MedicExpControl(3, arid:_arid),
                    Title = "Акт ЭКМП",
                    SizeToContent = SizeToContent.Height,
                    Width = 1450

                };
                window.ShowDialog();

                SchetRegisterGrid1.gridControl1.IsEnabled = true;
                DxHelper.LoadedRows.Clear();

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void AddRMee_OnClick(object sender, RoutedEventArgs e)
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

                var window = new DXWindow
                {
                    ShowIcon = false,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Content = new MedicExpControl(2, re: 1),
                    Title = "Акт МЭЭ",
                    SizeToContent = SizeToContent.Height,
                    Width = 1450

                };
                window.ShowDialog();

                SchetRegisterGrid1.gridControl1.IsEnabled = true;
                DxHelper.LoadedRows.Clear();

            }, TaskScheduler.FromCurrentSynchronizationContext());

        }

        private void AddREkmp_OnClick(object sender, RoutedEventArgs e)
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

                var window = new DXWindow
                {
                    ShowIcon = false,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Content = new MedicExpControl(3, re:1),
                    Title = "Акт ЭКМП",
                    SizeToContent = SizeToContent.Height,
                    Width = 1450

                };
                window.ShowDialog();

                SchetRegisterGrid1.gridControl1.IsEnabled = true;
                DxHelper.LoadedRows.Clear();

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }


        private void ReqAddItem_OnItemClick(object sender, ItemClickEventArgs e)
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
                    if (DxHelper.LoadedRows.GroupBy(x => ObjHelper.GetAnonymousValue(x, "LPU")).Select(gr => gr.Key).Count() > 1)
                    {
                        DXMessageBox.Show("Документ можно создать только в рамках одной МО");
                    }
                    else
                    {
                        var window = new DXWindow
                        {
                            ShowIcon = false,
                            WindowStartupLocation = WindowStartupLocation.CenterScreen,
                            Content = new ReqControl(),
                            Title = "Запрос на первичную документацию",
                            SizeToContent = SizeToContent.Height,
                            Width = 300
                        };
                        window.ShowDialog();
                        ReqBind();
                    }
                }
                SchetRegisterGrid1.gridControl1.IsEnabled = true;
                DxHelper.LoadedRows.Clear();

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ReqDelItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var ans = DXMessageBox.Show("Удалить запрос?", "Удаление", MessageBoxButton.YesNo);
            if (ans != MessageBoxResult.Yes) return;
            var sc = ReqGridControl.SelectedItem;

            Reader2List.CustomExecuteQuery($@"UPDATE D3_ZSL_OMS SET ReqID = NULL WHERE ReqID = {ObjHelper.GetAnonymousValue(sc, "ID")}", SprClass.LocalConnectionString);
            Reader2List.CustomAnonymousSelect($@"DELETE [dbo].[YamedRequests] Where ID = {ObjHelper.GetAnonymousValue(sc, "ID")}", SprClass.LocalConnectionString);
            
            ReqBind();
        }

        private void RepReqItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var yr = SqlReader.Select($"Select * from YamedReports where RepName = '_reqAkt'", SprClass.LocalConnectionString);
            var rl = (string)ObjHelper.GetAnonymousValue(yr[0], "Template");
            var sc = ReqGridControl.SelectedItem;

            СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            {
                Header = "Отчет",
                MyControl = new PreviewControl(rl, null, (int)ObjHelper.GetAnonymousValue(sc, "ID")),
                IsCloseable = "True",
                //TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
            });
        }

        private void ExcelButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel File (*.xlsx)|*.xlsx";

            if (saveFileDialog.ShowDialog() == true)
                SchetRegisterGrid1.gridControl1.View.ExportToXlsx(saveFileDialog.FileName);
        }

        private void AddMek_OnClick(object sender, RoutedEventArgs e)
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

                var window = new DXWindow
                {
                    ShowIcon = false,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Content = new SankControl(true),
                    Title = "Пакетный МЭК",
                    SizeToContent = SizeToContent.Height, Width = 500
                };
                window.ShowDialog();

                SchetRegisterGrid1.gridControl1.IsEnabled = true;
                DxHelper.LoadedRows.Clear();

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void RepReReqItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var yr = SqlReader.Select($"Select * from YamedReports where RepName = '_req2Akt'", SprClass.LocalConnectionString);
            var rl = (string)ObjHelper.GetAnonymousValue(yr[0], "Template");
            var sc = ReqGridControl.SelectedItem;

            СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            {
                Header = "Отчет",
                MyControl = new PreviewControl(rl, null, (int)ObjHelper.GetAnonymousValue(sc, "ID")),
                IsCloseable = "True",
                //TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
            });
        }

        private void RepReqCustomItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //var yr = SqlReader.Select($"Select * from YamedReports where RepName = '_reqCustomAkt'", SprClass.LocalConnectionString);
            //var rl = (string)ObjHelper.GetAnonymousValue(yr[0], "Template");
            //var sc = ReqGridControl.SelectedItem;

            //СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            //{
            //    Header = "Отчет",
            //    MyControl = new PreviewControl(rl, null, (int)ObjHelper.GetAnonymousValue(sc, "ID")),
            //    IsCloseable = "True",
            //    //TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
            //});


                var yr = SqlReader.Select($"Select * from YamedReports where RepName = '_reqAkt_5_in_1'", SprClass.LocalConnectionString);
                var rl = (string)ObjHelper.GetAnonymousValue(yr[0], "Template");
                var sc = ReqGridControl.SelectedItem;

                СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
                {
                    Header = "Отчет",
                    MyControl = new FRPreviewControl(rl, new ReportParams { ReqID = (int)ObjHelper.GetAnonymousValue(sc, "ID") }),
                    IsCloseable = "True",
                    //TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
                });

     


            //_reqAkt_app
            //_reqAkt_stac
            //_reqAkt_disp
            //_reqAkt_onko
            //_reqAkt_vmp
        }

        private void ReqEditItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void ZaprosCount()
        {
            var sc = ObjHelper.GetIds(_scids.ToArray());
            var q = Reader2List.CustomAnonymousSelect($@"exec GetExpCount '{sc}'", SprClass.LocalConnectionString);
            ExpGridControl.DataContext = q;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
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
                    if (DxHelper.LoadedRows.GroupBy(x => ObjHelper.GetAnonymousValue(x, "LPU")).Select(gr => gr.Key).Count() > 1)
                    {
                        DXMessageBox.Show("Документ можно создать только в рамках одной МО");
                    }
                    else
                    {
                        var rows = DxHelper.LoadedRows.Select(x => ObjHelper.GetAnonymousValue(x, "ID")).ToArray();

                        var window = new DXWindow
                        {
                            ShowIcon = false,
                            WindowStartupLocation = WindowStartupLocation.CenterScreen,
                            Content = new StatisticReports(rows, 1000),
                            Title = "Печатные формы",
                            SizeToContent = SizeToContent.Width,
                            Height = 600
                        };
                        window.ShowDialog();
                        ReqBind();
                    }
                }
                SchetRegisterGrid1.gridControl1.IsEnabled = true;
                DxHelper.LoadedRows.Clear();

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Zsl31Edit_OnClickMO()
        {
            var tab = SchetRegisterGrid1;
            if (SprClass.Region != "37")
            {
                var id = (int)ObjHelper.GetAnonymousValue(DxHelper.GetSelectedGridRow(tab.gridControl1), "ID");
                var slt = new SluchTemplateD31(SchetRegisterGrid1.gridControl1);
                slt.BindSluch(id, _sc);
                СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
                {
                    Header = "Карта пациента",
                    MyControl = slt,
                    IsCloseable = "True",
                    //TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
                });
            }
            else if (SprClass.Region == "37")
            {
                var id = (int)ObjHelper.GetAnonymousValue(DxHelper.GetSelectedGridRow(tab.gridControl1), "ID");
                var slt = new SluchTemplateD31Ivanovo(SchetRegisterGrid1.gridControl1);
                slt.BindSluch(id, _sc);
                СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
                {
                    Header = "Карта пациента",
                    MyControl = slt,
                    IsCloseable = "True",
                    //TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
                });
            }

        }

        private void Zsl31Edit_OnClick(object sender, RoutedEventArgs e)
        {
            var tab = SchetRegisterGrid1;
            if (SprClass.ProdSett.OrgTypeStatus == OrgType.Lpu)
            {
                Zsl31Edit_OnClickMO();
            }
            else
            {
                if (SprClass.Region != "37")
                {
                    var id = (int)ObjHelper.GetAnonymousValue(DxHelper.GetSelectedGridRow(tab.gridControl1), "ID");
                    var slt = new SluchTemplateD31(SchetRegisterGrid1.gridControl1);
                    slt.BindSluch(id, new Entity.D3_SCHET_OMS());
                    СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
                    {
                        Header = "Карта пациента",
                        MyControl = slt,
                        IsCloseable = "True",
                        //TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
                    });
                }
                else if (SprClass.Region == "37")
                {
                    var id = (int)ObjHelper.GetAnonymousValue(DxHelper.GetSelectedGridRow(tab.gridControl1), "ID");
                    var slt = new SluchTemplateD31Ivanovo(SchetRegisterGrid1.gridControl1);
                    slt.BindSluch(id, new Entity.D3_SCHET_OMS());
                    СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
                    {
                        Header = "Карта пациента",
                        MyControl = slt,
                        IsCloseable = "True",
                        //TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
                    });
                }
            }
        }

        private void MtrCheckItem_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            if (((BarCheckItem)sender).IsChecked == true)
            {
                if (string.IsNullOrEmpty(SchetRegisterGrid1.gridControl1.FilterString))
                {
                    SchetRegisterGrid1.gridControl1.FilterString = $"([SMO] Is Null Or [SMO] Not Like '46%')";

                }
                else
                {
                    SchetRegisterGrid1.gridControl1.FilterString += $"And ([SMO] Is Null Or [SMO] Not Like '46%')";

                }
            }
            else
            {
                
                SchetRegisterGrid1.gridControl1.FilterString =
                    SchetRegisterGrid1.gridControl1.FilterString.
                        Replace($" And ([SMO] Is Null Or [SMO] Not Like '46%')", "").
                        Replace($"([SMO] Is Null Or [SMO] Not Like '46%') And ", "").

                        Replace($"([SMO] Is Null Or [SMO] Not Like '46%')", "").
                        Replace($"[SMO] Is Null Or [SMO] Not Like '46%'", "");

            }
            
        }

        private void ViewCheckItem_CheckedChanged(object sender, ItemClickEventArgs e)
        {

            if (SchetRegisterGrid1 == null) return;

            var tag = (string)((BarCheckItem)sender).Tag;
            var isChecked = (bool)((BarCheckItem)sender).IsChecked;

            if (tag == "ZSL" && isChecked)
            {
                SchetRegisterGrid1.BindDataZsl();
            }
            else if(tag == "SL" && isChecked)
            {
                SchetRegisterGrid1.BindDataSl();
            }
            else if(tag == "USL" && isChecked)
            {
                SchetRegisterGrid1.BindDataUsl();
            }
            else if (tag == "SANK" && isChecked)
            {
                SchetRegisterGrid1.BindDataSank();
            }
        }

        private void Spisok_Form_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                var sc = ReqGridControl.SelectedItem;
                var ReqID = (int)ObjHelper.GetAnonymousValue(sc, "ID");
                var window = new DXWindow
                {
                    ShowIcon = false,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Content = new StatisticReportsRequest(ReqID, 2000),
                    Title = "Печатные формы",
                    SizeToContent = SizeToContent.Width,
                    Height = 600
                };
                window.ShowDialog();
                ReqBind();
            }
            catch
            {
                MessageBox.Show("Не выбран запрос");
            }
        }
        private void TestBarnaul(object sender, RoutedEventArgs e)
        {
            string fname1 = $@"IS{SprClass.DbSettings[8].Parametr}T{SprClass.Region}_{DateTime.Today.Year.ToString().Substring(0, 2) + (DateTime.Today.Month.ToString().Length < 2 ? "0" + DateTime.Today.Month.ToString() : DateTime.Today.Month.ToString())}1.csv";
            SaveFileDialog SF = new SaveFileDialog();
            SF.FileName = fname1;
            SF.DefaultExt = ".csv";
            SF.Filter = "Файлы CSV (.csv)|*.csv";
            bool res = SF.ShowDialog().Value;
            string fname = SF.FileName;
            string scs = String.Join(", ", _scids.ToArray());


            if (res == true)
            {
                var connectionString = SprClass.LocalConnectionString;
                SqlConnection con = new SqlConnection(connectionString);

                SqlCommand comm = new SqlCommand($@"declare @r char='|';
            SELECT convert(nvarchar(36), newid()) + @r + isnull(fam, '') + @r + isnull(im, '') + @r + isnull(ot, '') + @r + convert(nvarchar, isnull(cast(dr as date), '')) + @r
            + convert(nvarchar, isnull(w, '')) + @r + convert(nvarchar, isnull(DOCTYPE, '')) + @r + isnull(DOCSER, '') + @r + isnull(DOCNUM, '') + @r + isnull(SNILS, '') + @r
            + '1027739449913' + @r + '01000' + @r + npolis + @r + convert(nvarchar, isnull(vpolis, '')) + @r + '0' + @r + '0' + @r
            + convert(nvarchar, DATE_Z_1, 23) + @r + convert(nvarchar, DATE_Z_2, 23) + @r + '1022200897840' + @r + '0'
from D3_PACIENT_OMS p
left
join D3_ZSL_OMS z on p.id = z.D3_PID
where z.D3_SCID in({scs})", con);
                con.Open();
                SqlDataReader dr = comm.ExecuteReader();
                StreamWriter sr = new StreamWriter(fname, false, Encoding.GetEncoding(1251));

                while (dr.Read())
                {

                    sr.Write("{0}", dr[0]);
                    sr.WriteLine();

                }

                sr.Close();
                con.Close();
                string fnamezip = fname.Replace(".csv", "");
                using (ZipFile zip = new ZipFile())
                {
                    zip.AddFile(fname, "");

                    zip.Save(fnamezip + ".zip");

                }
                DXMessageBox.Show("Выгрузка завершена");
                File.Delete(fname);
            }

        }
        private DataTable tb;
        private void TestBarnaul1(object sender, RoutedEventArgs e)
        {
            string scs = String.Join(", ", _scids.ToArray());
            OpenFileDialog OF = new OpenFileDialog();
            OF.Multiselect = false;
            bool res = OF.ShowDialog().Value;
            string ex_path = OF.FileName;
            string polis_up_file = OF.SafeFileName;
            ZipFile zip = new ZipFile(ex_path);
            if (res == true)
            {
                zip.ExtractAll(ex_path.Replace(OF.SafeFileName, ""), ExtractExistingFileAction.OverwriteSilently);
                ex_path = ex_path.Replace(OF.SafeFileName, zip.EntryFileNames.First());


                //Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                //    new Action(delegate ()
                //    {
                tb = new DataTable();

                    string filename = ex_path;
                    string[] attache = File.ReadAllLines(filename);
                    var cls0 = attache[0].Split('|');
                    for (int i = 0; i < cls0.Count(); i++)
                    {
                        tb.Columns.Add("Column" + i.ToString(), typeof(string));
                    }
                    //tb.Columns.AddRange();
                    foreach (string row in attache)
                    {
                        // получаем все ячейки строки

                        var cls = row.Split('|');

                        tb.LoadDataRow(cls, LoadOption.Upsert);
                        //Attache_mo.Add(new ATTACHED_MO { GUID = cls[0], OKATO = cls[1], SMO = cls[2], DPFS = cls[3], SER = cls[4], NUM = cls[5], ENP = cls[6], MO = cls[7] });
                    }
                //}));
                var connectionString = SprClass.LocalConnectionString;
                SqlConnection con = new SqlConnection(connectionString);

                SqlCommand comm = new SqlCommand($@"update d3_pacient_oms set mo_att=amo.MO, OMS_STATUS=1 from @AttachedMO amo where d3_pacient_oms.npolis=amo.ENP and d3_pacient_oms.d3_scid in({scs})", con);

                SqlParameter Att_Mo = comm.Parameters.AddWithValue("@AttachedMO", tb);
                Att_Mo.SqlDbType = SqlDbType.Structured;
                Att_Mo.TypeName = "dbo.AttacheTableType";
                con.Open();
                comm.CommandTimeout = 0;
                comm.ExecuteNonQuery();

                con.Close();
                File.Delete(ex_path);
                DXMessageBox.Show("Прикрепление успешно загружено");
            }
        }

        private void AddRMek_OnClick(object sender, ItemClickEventArgs e)
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

                var window = new DXWindow
                {
                    ShowIcon = false,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Content = new MedicExpControl(1, re: 1),
                    Title = "Акт МЭК",
                    SizeToContent = SizeToContent.Height,
                    Width = 1450

                };
                window.ShowDialog();

                SchetRegisterGrid1.gridControl1.IsEnabled = true;
                DxHelper.LoadedRows.Clear();

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Add_sl_ItemClick(object sender, ItemClickEventArgs e)
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

        private void Compilezsl_ItemClick(object sender, ItemClickEventArgs e)
        {
            var qxml = SqlReader.Select($@"exec p_magic_visit {_sc.ID}"
               , SprClass.LocalConnectionString);
        }

        private void Flk_osp_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "OMS File |*.oms;*.zip";


            var result = openFileDialog.ShowDialog();

            var rfile = openFileDialog.FileName;

            string ft = "";
            string zapfn = "";
            var zapms = new MemoryStream();

            if (result != true) return;

            ((DevExpress.Xpf.Bars.BarButtonItem)sender).IsEnabled = false;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (ZipFile zip = ZipFile.Read(rfile))
                    {
                        foreach (ZipEntry zipEntry in zip)
                        {
                            if (zipEntry.FileName.StartsWith("V"))
                            {
                                ft = "flk";
                                zapfn = zipEntry.FileName;
                                zipEntry.Extract(zapms);
                                zapms.Position = 0;
                            }

                            if (zipEntry.FileName.StartsWith("O"))
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

                ((DevExpress.Xpf.Bars.BarButtonItem)sender).IsEnabled = true;


            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Perenos_ItemClick(object sender, ItemClickEventArgs e)
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

                var window = new DXWindow
                {
                    ShowIcon = false,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Content = new ReestrChooseControl(_sc),
                    Title = "Выбор реестра для переноса",
                    Width = 350,
                    Height = 300
                };
                window.ShowDialog();

                SchetRegisterGrid1.gridControl1.IsEnabled = true;
                DxHelper.LoadedRows.Clear();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Sl_del_ItemClick(object sender, ItemClickEventArgs e)
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
                            string pacid = (string)comm.ExecuteScalar();
                            con.Close();
                            string pacids = pacid.Substring(0, pacid.Length - 1);
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

        private void AddAktExp_OnItemClick(object sender, ItemClickEventArgs e)
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
                bool isMek = false;
                var sluids = new List<int>();
                foreach (var row in DxHelper.LoadedRows)
                {
                    if ((int?)ObjHelper.GetAnonymousValue(row, "OPLATA") == 1 || SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms)
                    {
                        sluids.Add((int)ObjHelper.GetAnonymousValue(row, "ID"));
                    }
                    else
                    {
                        isMek = true;
                    }
                }

                if (!sluids.Any())
                {
                    DXMessageBox.Show("Не выбрано ни одной записи или записи имеют некорректный статус оплаты");
                    SchetRegisterGrid1.gridControl1.IsEnabled = true;
                    DxHelper.LoadedRows.Clear();
                    return;
                }

                if (isMek)
                {
                    DXMessageBox.Show("Внимание выбраны записи с некорректным статусом оплаты, которые не могут быть включены в акт экспертизы");
                }


                var item = new D3_AKT_REGISTR_OMS();
                item.USERID_NOTEDIT = SprClass.userId;
                var sprEditWindow = new UniSprEditControl("D3_AKT_REGISTR_OMS", item, false, SprClass.LocalConnectionString);
                var window = new DXWindow
                {
                    ShowIcon = false,
                    WindowStartupLocation = WindowStartupLocation.Manual,
                    SizeToContent = SizeToContent.Height,
                    Width = 600,
                    Content = sprEditWindow,
                    Title = "Новый акт экспертизы"
                };

                if (window.ShowDialog() == true)
                {
                    var arid = item.ID;
                    List<D3_REQ_OMS> rlist = new List<D3_REQ_OMS>();

                    foreach (int slid in sluids.ToArray())
                    {
                        var rq = new D3_REQ_OMS()
                        {
                            D3_ARID = arid,
                            D3_ZSLID = slid
                        };
                        rlist.Add(rq);
                    }
                    Reader2List.AnonymousInsertCommand("D3_REQ_OMS", rlist, "ID", SprClass.LocalConnectionString);
                }




                SchetRegisterGrid1.gridControl1.IsEnabled = true;
                DxHelper.LoadedRows.Clear();

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void EditAktExp_OnItemClick(object sender, ItemClickEventArgs e)
        {
            DXMessageBox.Show("Не реализовано");
        }
    }
}
