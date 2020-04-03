using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Xml;
using DevExpress.Xpf.Core;
using Ionic.Zip;
using Microsoft.Win32;
using Yamed.Control;
using Yamed.Core;
using Yamed.Entity;
using Yamed.Hospital;
using Yamed.Reports;
using Yamed.Server;

namespace Yamed.Oms
{
    /// <summary>
    /// Логика взаимодействия для EconomyControl.xaml
    /// </summary>
    public partial class EconomyControl : UserControl
    {
        public EconomyControl()
        {
            InitializeComponent();
        }

        //private void Hosp_OnClick(object sender, RoutedEventArgs e)
        //{
        //    //Menu.Hide();
        //    //((Button)sender).IsEnabled = false;
        //    //Decorator.IsSplashScreenShown = true;
        //    var sc = ObjHelper.ClassConverter<D3_SCHET_OMS>(DxHelper.GetSelectedGridRow(EconomyWindow11.gridControl));

        //    СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
        //    {
        //        Header = "Реестр Стационар",
        //        MyControl = new ReestrControl1(sc),
        //        IsCloseable = "True",
        //        //TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
        //    });

        //    //((Button)sender).IsEnabled = true;
        //}

        private void Amb_OnClick(object sender, RoutedEventArgs e)
        {
            var sc = ObjHelper.ClassConverter<D3_SCHET_OMS>(DxHelper.GetSelectedGridRow(EconomyWindow11.gridControl));
            var rc = new SchetRegisterControl(sc);
            rc.SchetRegisterGrid1.Scids = new List<int> { sc.ID };
            rc.SchetRegisterGrid1.BindDataZsl();

            СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            {
                Header = "Реестр поликлиника",
                MyControl = rc,
                IsCloseable = "True",
                //TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
            });
        }

        private void LoadOldPoliclinic_OnClick(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "OMS File |*.oms;*.zip";


            var result = openFileDialog.ShowDialog();

            var rfile = openFileDialog.FileName;
            var omszfn = openFileDialog.SafeFileName;

            string zapfn = "";
            string persfn = "";
            var zapms = new MemoryStream();
            var persms = new MemoryStream();

            if (result != true) return;

            var sc = ObjHelper.ClassConverter<D3_SCHET_OMS>(DxHelper.GetSelectedGridRow(EconomyWindow11.gridControl));
            ((DevExpress.Xpf.Bars.BarButtonItem)sender).IsEnabled = false;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (ZipFile zip = ZipFile.Read(rfile))
                    {
                        foreach (ZipEntry zipEntry in zip)
                        {
                            if (zipEntry.FileName.StartsWith("HM") || zipEntry.FileName.StartsWith("D") ||
                                zipEntry.FileName.StartsWith("T"))
                            {
                                zapfn = zipEntry.FileName;
                                zipEntry.Extract(zapms);
                                zapms.Position = 0;
                            }

                            if (zipEntry.FileName.StartsWith("L"))
                            {
                                persfn = zipEntry.FileName;
                                zipEntry.Extract(persms);
                                persms.Position = 0;
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
                    return;
                }


                var zapsr = new StreamReader(zapms, Encoding.Default);
                string zapxml = zapsr.ReadToEnd();
                zapms.Dispose();
                zapms.Close();

                var perssr = new StreamReader(persms, Encoding.Default);
                string persxml = perssr.ReadToEnd();
                persms.Dispose();
                persms.Close();

                if (string.IsNullOrEmpty(zapxml) || string.IsNullOrEmpty(persxml))
                {
                    //Reader2List.CustomExecuteQuery($@"Update DOX_SCHET SET DOX_STATUS=12 ", SprClass.LocalConnectionString);
                }
                else
                {
                    try
                    {
                        var q = $@"
                            EXEC [dbo].[LoadFromMedialog] '{zapxml.Replace("'", "")}', '{persxml.Replace("'", "")}', '{zapfn}', '{persfn}'
                        ";

                        zapxml = null;
                        persxml = null;
                        Reader2List.CustomExecuteQuery(q, SprClass.LocalConnectionString);
                        q = null;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            DXMessageBox.Show(ex.Message + Environment.NewLine + ex.InnerException?.Message);
                        });
                    }
                }
                zapsr.Dispose();
                zapsr.Close();
                perssr.Dispose();
                perssr.Close();


                //Console.WriteLine("Распакован " + id);

                GC.WaitForPendingFinalizers();
                GC.Collect();

            }).ContinueWith(x =>
            {
                EconomyWindow11.linqInstantFeedbackDataSource.Refresh();

                DXMessageBox.Show("Загрузка успешно завершена");

                ((DevExpress.Xpf.Bars.BarButtonItem)sender).IsEnabled = true;


            }, TaskScheduler.FromCurrentSynchronizationContext());
        }


        private void Calculate_OnClick(object sender, RoutedEventArgs e)
        {
            var region = SprClass.Region;
            var sc = ObjHelper.ClassConverter<D3_SCHET_OMS>(DxHelper.GetSelectedGridRow(EconomyWindow11.gridControl));
            ((DevExpress.Xpf.Bars.BarButtonItem)sender).IsEnabled = false;

            var calcs = SqlReader.Select(
                $@"select [name] from sys.procedures where [name] like '%p_autocalc_0{region}%' order by [name]",
                SprClass.LocalConnectionString);

            Reader2List.CustomExecuteQuery($@"
                    exec [dbo].[p_fix] {sc.ID}", SprClass.LocalConnectionString);
            foreach (var calc in calcs)
            {
                Reader2List.CustomExecuteQuery($@"
                    exec [dbo].[{calc.GetValue("name")}] {sc.ID}", SprClass.LocalConnectionString);
            }


            if (region == "46" || region == "046")
            {
                Reader2List.CustomExecuteQuery($@"
                    exec[dbo].[p_oms_calc_kslp_sum] {0}, {sc.ID}, 'scid'", SprClass.LocalConnectionString);

                Reader2List.CustomExecuteQuery($@"
                    exec[dbo].[p_oms_calc_ksg] {0}, {sc.ID}, 'scid'", SprClass.LocalConnectionString);
            }

            DXMessageBox.Show("Расчет завершен.");

            ((DevExpress.Xpf.Bars.BarButtonItem)sender).IsEnabled = true;

        }





        private void ExportOms_OnClick(object sender, RoutedEventArgs e)
        {
            var sc = ObjHelper.ClassConverter<D3_SCHET_OMS>(DxHelper.GetSelectedGridRow(EconomyWindow11.gridControl));

            ((DevExpress.Xpf.Bars.BarButtonItem)sender).IsEnabled = false;

            //Task.Factory.StartNew(() =>
            //{

            //XmlStreem2(sc, true);
            ExportOms30K(sc);
            //}).ContinueWith(x =>
            //{
            ((DevExpress.Xpf.Bars.BarButtonItem)sender).IsEnabled = true;

            //DXMessageBox.Show("Невозможно выполнить экспорт, не все случаи просчитаны, проверте случаи с пустыми суммами и выполните расчет.");
            //}, TaskScheduler.FromCurrentSynchronizationContext());
        }





        void ExportOms30K(object schet)
        {
      //      Reader2List.CustomExecuteQuery($@"
      //                  UPDATE D3_ZSL_OMS SET ZSL_ID = newid() WHERE ZSL_ID IS NULL
      //                  UPDATE D3_SL_OMS SET SL_ID = newid() WHERE SL_ID IS NULL
      //                  UPDATE D3_PACIENT_OMS SET ID_PAC = newid() WHERE ID_PAC IS NULL
      //                  UPDATE PACIENT SET ID_PAC = newid() WHERE ID_PAC IS NULL
      //                  UPDATE SLUCH SET IDSLG = newid() WHERE IDSLG IS NULL

						//UPDATE D3_SL_OMS SET SL_ID = newid() where SL_ID in (
						//SELECT SL_ID FROM D3_SL_OMS GROUP by SL_ID HAVING count(*) > 1)

						//UPDATE D3_ZSL_OMS SET ZSL_ID = newid() where ZSL_ID in (
						//SELECT ZSL_ID FROM D3_ZSL_OMS GROUP by ZSL_ID HAVING count(*) > 1)

						//UPDATE D3_PACIENT_OMS SET ID_PAC = newid() where ID_PAC in (
						//SELECT ID_PAC FROM D3_PACIENT_OMS GROUP by ID_PAC HAVING count(*) > 1)

						//UPDATE SLUCH SET IDSLG = newid() where IDSLG in (
						//SELECT IDSLG FROM SLUCH GROUP by IDSLG HAVING count(*) > 1)

						//UPDATE D3_USL_OMS SET D3_SLGID = sl.SL_ID
						//FROM D3_SL_OMS sl
      //                  join D3_ZSL_OMS zsl on sl.D3_ZSLID = zsl.ID and D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")}
						//join D3_USL_OMS u on sl.ID = u.D3_SLID
						//where SL_ID <> D3_SLGID


      //                ", SprClass.LocalConnectionString);

            //Reader2List.CustomExecuteQuery($@"
            //            UPDATE D3_SL_OMS SET USL_OK = 3 WHERE USL_OK is null
            //            UPDATE D3_SL_OMS SET P_CEL = '1.1' WHERE P_CEL is null and USL_OK = 3
            //            UPDATE D3_SL_OMS SET P_CEL = NULL WHERE USL_OK = 4
            //", SprClass.LocalConnectionString);

//            Reader2List.CustomExecuteQuery($@"
//            UPDATE SLUCH SET IDDOKT = d.SNILS 
//            FROM SLUCH sl
//            Join DoctorBd d on sl.IDDOKTO = d.id
//            where schet_id = {ObjHelper.GetAnonymousValue(schet, "ID")}
//            ", SprClass.LocalConnectionString);

//            Reader2List.CustomExecuteQuery($@"
//            UPDATE USL SET CODE_MD = d.SNILS 
//            FROM USL u
//            Join DoctorBd d on u.CODE_MDLPU = d.id
//            where schet_id = {ObjHelper.GetAnonymousValue(schet, "ID")}
//            ", SprClass.LocalConnectionString);

//            Reader2List.CustomExecuteQuery($@"
// UPDATE D3_USL_OMS
//            SET	LPU = z.LPU   
            
//            FROM D3_SL_OMS AS s 
//            JOIN D3_USL_OMS AS u ON u.D3_SLID = s.ID
//			JOIN D3_ZSL_OMS AS z ON s.D3_ZSLID = z.ID
//            WHERE z.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")} and u.LPU is NULL
// UPDATE D3_USL_OMS
//            SET	PROFIL = s.PROFIL   
            
//            FROM D3_SL_OMS AS s 
//            JOIN D3_USL_OMS AS u ON u.D3_SLID = s.ID
//			JOIN D3_ZSL_OMS AS z ON s.D3_ZSLID = z.ID
//            WHERE z.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")} and u.PROFIL is NULL
//UPDATE D3_USL_OMS
//            SET	DET = s.DET   
            
//            FROM D3_SL_OMS AS s 
//            JOIN D3_USL_OMS AS u ON u.D3_SLID = s.ID
//			JOIN D3_ZSL_OMS AS z ON s.D3_ZSLID = z.ID
//            WHERE z.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")} and u.DET is NULL
//UPDATE D3_USL_OMS
//            SET	DATE_IN = s.DATE_1  
            
//            FROM D3_SL_OMS AS s 
//            JOIN D3_USL_OMS AS u ON u.D3_SLID = s.ID
//			JOIN D3_ZSL_OMS AS z ON s.D3_ZSLID = z.ID
//            WHERE z.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")} and u.DATE_IN is NULL
//UPDATE D3_USL_OMS
//            SET	DATE_OUT = s.DATE_2   
            
//            FROM D3_SL_OMS AS s 
//            JOIN D3_USL_OMS AS u ON u.D3_SLID = s.ID
//			JOIN D3_ZSL_OMS AS z ON s.D3_ZSLID = z.ID
//            WHERE z.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")} and u.DATE_OUT is NULL
//UPDATE D3_USL_OMS
//            SET	DS = s.DS1   
            
//            FROM D3_SL_OMS AS s 
//            JOIN D3_USL_OMS AS u ON u.D3_SLID = s.ID
//			JOIN D3_ZSL_OMS AS z ON s.D3_ZSLID = z.ID
//            WHERE z.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")} and u.DS is NULL
//UPDATE D3_USL_OMS
//            SET	CODE_USL = u.VID_VME   
            
//            FROM D3_SL_OMS AS s 
//            JOIN D3_USL_OMS AS u ON u.D3_SLID = s.ID
//			JOIN D3_ZSL_OMS AS z ON s.D3_ZSLID = z.ID
//            WHERE z.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")} and u.CODE_USL is NULL
//UPDATE D3_USL_OMS
//            SET	PRVS = s.PRVS   
            
//            FROM D3_SL_OMS AS s 
//            JOIN D3_USL_OMS AS u ON u.D3_SLID = s.ID
//			JOIN D3_ZSL_OMS AS z ON s.D3_ZSLID = z.ID
//            WHERE z.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")} and u.PRVS is NULL
//UPDATE D3_USL_OMS
//            SET	CODE_MD = s.IDDOKT   
            
//            FROM D3_SL_OMS AS s 
//            JOIN D3_USL_OMS AS u ON u.D3_SLID = s.ID
//			JOIN D3_ZSL_OMS AS z ON s.D3_ZSLID = z.ID
//            WHERE z.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")} and u.CODE_MD is NULL
//UPDATE D3_USL_OMS
//            SET	KOL_USL = 1  
            
//            FROM D3_SL_OMS AS s 
//            JOIN D3_USL_OMS AS u ON u.D3_SLID = s.ID
//			JOIN D3_ZSL_OMS AS z ON s.D3_ZSLID = z.ID
//            WHERE z.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")} and u.KOL_USL is NULL
//UPDATE D3_USL_OMS
//            SET	TARIF = 0.00   
            
//            FROM D3_SL_OMS AS s 
//            JOIN D3_USL_OMS AS u ON u.D3_SLID = s.ID
//			JOIN D3_ZSL_OMS AS z ON s.D3_ZSLID = z.ID
//            WHERE z.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")} and u.TARIF is NULL
//UPDATE D3_USL_OMS
//            SET	SUMV_USL = 0.00   
            
//            FROM D3_SL_OMS AS s 
//            JOIN D3_USL_OMS AS u ON u.D3_SLID = s.ID
//			JOIN D3_ZSL_OMS AS z ON s.D3_ZSLID = z.ID
//            WHERE z.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")} and u.SUMV_USL is NULL
//            ", SprClass.LocalConnectionString);


            var qxml = SqlReader.Select($@"
            exec p_oms_export_30K {ObjHelper.GetAnonymousValue(schet, "ID")}"
, SprClass.LocalConnectionString);



            string result1 = "<?xml version=\"1.0\" encoding=\"windows-1251\"?>" + (string)qxml[0].GetValue("HM");
            string result2 = "<?xml version=\"1.0\" encoding=\"windows-1251\"?>" + (string)qxml[0].GetValue("LM");
            using (ZipFile zip = new ZipFile(Encoding.GetEncoding("windows-1251")))
            {
                zip.AddEntry((string)qxml[0].GetValue("hf_name") + ".xml", result1);
                zip.AddEntry((string)qxml[0].GetValue("lf_name") + ".xml", result2);

                //if (isLoop)
                //{
                //    if (!Directory.Exists(Environment.CurrentDirectory + @"\Out"))
                //        Directory.CreateDirectory(Environment.CurrentDirectory + @"\Out");

                //    var zipFile = Environment.CurrentDirectory + @"\Out\" + (string)qxml[0].GetValue("hf_name") + ".oms";
                //    zip.Save(zipFile);
                //}
                //else
                {
                    Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();

                        saveFileDialog.Filter = "OMS File (*.oms)|*.oms";
                        saveFileDialog.FileName = (string)qxml[0].GetValue("hf_name") + ".oms";

                        bool? result = saveFileDialog.ShowDialog();
                        if (result == true)
                            zip.Save(saveFileDialog.FileName);
                    });
                }

            }

        }

        public void XmlStreem2(object schet, bool isTfomsSchet, bool isLoop = false, int loopCount = 1)
        {

            //LoadingDecorator1.IsSplashScreenShown = true;

            int i = 0;
            var schetp = schet;
            string plat = string.IsNullOrEmpty((string)ObjHelper.GetAnonymousValue(schetp, "PLAT")) ? "46" : (string)ObjHelper.GetAnonymousValue(schetp, "PLAT");
            string m = Convert.ToString(ObjHelper.GetAnonymousValue(schetp, "MONTH"));
            string mm = (int)ObjHelper.GetAnonymousValue(schetp, "MONTH") < 10 ? "0" + m : m;
            string sf = plat.Length > 2 ? "S" : "T";

            string filename1 = "";
            string filename2 = "";

            if (SprClass.ProdSett.OrgTypeStatus == OrgType.Lpu)
            {
                filename1 = "HM" + ObjHelper.GetAnonymousValue(schetp, "CODE_MO") + sf + plat + "_" +
                            ObjHelper.GetAnonymousValue(schetp, "YEAR").ToString().Substring(2) +
                            mm + "1";
                filename2 = "LM" + ObjHelper.GetAnonymousValue(schetp, "CODE_MO") + sf + plat + "_" +
                            ObjHelper.GetAnonymousValue(schetp, "YEAR").ToString().Substring(2) + mm + "1";
            }
            else if (ObjHelper.GetAnonymousValue(schetp, "PLAT") != null && ((string)ObjHelper.GetAnonymousValue(schetp, "PLAT")).StartsWith("04"))
            {

                var com1 = (string)ObjHelper.GetAnonymousValue(schetp, "COMENTS");
                var com2 = com1.Substring(com1.IndexOf("M") + 1);
                var codemoreg = com2.Remove(com2.IndexOf("S"));

                string codef = "";
                if (((string)ObjHelper.GetAnonymousValue(schetp, "COMENTS")).StartsWith("D"))
                {
                    codef = ((string)ObjHelper.GetAnonymousValue(schetp, "COMENTS")).Substring(0, 2);

                }
                else
                    codef = ((string)ObjHelper.GetAnonymousValue(schetp, "COMENTS")).Substring(0, 1);

                filename1 = codef + "S" + ((string)ObjHelper.GetAnonymousValue(schetp, "PLAT")).Substring(3) + "T" +
                            ((string)ObjHelper.GetAnonymousValue(schetp, "PLAT")).Substring(0, 2) + "_" +
                            ObjHelper.GetAnonymousValue(schetp, "YEAR").ToString().Substring(2) +
                            mm + loopCount + "_" + codemoreg;
                filename2 = "L" + codef.Remove(0, 1) + "S" + "" + ((string)ObjHelper.GetAnonymousValue(schetp, "PLAT")).Substring(3) + "T" +
                            ((string)ObjHelper.GetAnonymousValue(schetp, "PLAT")).Substring(0, 2) + "_" +
                            ObjHelper.GetAnonymousValue(schetp, "YEAR").ToString().Substring(2) +
                            mm + loopCount + "_" + codemoreg;
            }
            else
            {
                filename1 = (string)ObjHelper.GetAnonymousValue(schetp, "ZAPFILENAME");
                filename2 = (string)ObjHelper.GetAnonymousValue(schetp, "PERSFILENAME");
            }
            string filename3 = "XM" + ObjHelper.GetAnonymousValue(schetp, "CODE_MO") + sf + plat + "_" + ObjHelper.GetAnonymousValue(schetp, "YEAR").ToString().Substring(2) +
                    mm + "1";


            Reader2List.CustomExecuteQuery($@"
                        UPDATE D3_ZSL_OMS SET ZSL_ID = newid() WHERE ZSL_ID IS NULL
                        UPDATE D3_SL_OMS SET SL_ID = newid() WHERE SL_ID IS NULL
                        UPDATE D3_PACIENT_OMS SET ID_PAC = newid() WHERE ID_PAC IS NULL
                        UPDATE SLUCH SET IDSLG = newid() WHERE IDSLG IS NULL

						UPDATE D3_SL_OMS SET SL_ID = newid() where SL_ID in (
						SELECT SL_ID FROM D3_SL_OMS GROUP by SL_ID HAVING count(*) > 1)

						UPDATE D3_ZSL_OMS SET ZSL_ID = newid() where ZSL_ID in (
						SELECT ZSL_ID FROM D3_ZSL_OMS GROUP by ZSL_ID HAVING count(*) > 1)

						UPDATE D3_PACIENT_OMS SET ID_PAC = newid() where ID_PAC in (
						SELECT ID_PAC FROM D3_PACIENT_OMS GROUP by ID_PAC HAVING count(*) > 1)

						UPDATE SLUCH SET IDSLG = newid() where IDSLG in (
						SELECT IDSLG FROM SLUCH GROUP by IDSLG HAVING count(*) > 1)

						UPDATE D3_USL_OMS SET D3_SLGID = sl.SL_ID
						FROM D3_SL_OMS sl
                        join D3_ZSL_OMS zsl on sl.D3_ZSLID = zsl.ID and D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")}
						join D3_USL_OMS u on sl.ID = u.D3_SLID
						where SL_ID <> D3_SLGID


                      ", SprClass.LocalConnectionString);

            Reader2List.CustomExecuteQuery($@"
                        UPDATE D3_SL_OMS SET PROFIL = 136 WHERE PROFIL = 2-- or PROFIL = 184
                        UPDATE SLUCH SET PROFIL = 136 WHERE PROFIL = 2-- or PROFIL = 184
                        UPDATE USL SET PROFIL = 136 WHERE PROFIL = 2-- or PROFIL = 184
                        UPDATE D3_USL_OMS SET PROFIL = 136 WHERE PROFIL = 2-- or PROFIL = 184
            ", SprClass.LocalConnectionString);

            Reader2List.CustomExecuteQuery($@"
                        UPDATE D3_SL_OMS SET PROFIL = 162 WHERE PROFIL = 64
                        UPDATE SLUCH SET PROFIL = 162 WHERE PROFIL = 64
                        UPDATE USL SET PROFIL = 162 WHERE PROFIL = 64
                        UPDATE D3_USL_OMS SET PROFIL = 162 WHERE PROFIL = 64
            ", SprClass.LocalConnectionString);

            Reader2List.CustomExecuteQuery($@"
                        UPDATE D3_SL_OMS SET USL_OK = 3 WHERE USL_OK is null
                        UPDATE D3_SL_OMS SET P_CEL = '1.1' WHERE P_CEL is null and USL_OK = 3
                        UPDATE D3_SL_OMS SET P_CEL = NULL WHERE USL_OK = 4
            ", SprClass.LocalConnectionString);

            Reader2List.CustomExecuteQuery($@"
            UPDATE SLUCH SET IDDOKT = d.SNILS 
            FROM SLUCH sl
            Join DoctorBd d on sl.IDDOKTO = d.id
            where schet_id = {ObjHelper.GetAnonymousValue(schet, "ID")}
            ", SprClass.LocalConnectionString);

            Reader2List.CustomExecuteQuery($@"
            UPDATE USL SET CODE_MD = d.SNILS 
            FROM USL u
            Join DoctorBd d on u.CODE_MDLPU = d.id
            where schet_id = {ObjHelper.GetAnonymousValue(schet, "ID")}
            ", SprClass.LocalConnectionString);

            Reader2List.CustomExecuteQuery($@"
 UPDATE D3_USL_OMS
            SET	LPU = z.LPU   
            
            FROM D3_SL_OMS AS s 
            JOIN D3_USL_OMS AS u ON u.D3_SLID = s.ID
			JOIN D3_ZSL_OMS AS z ON s.D3_ZSLID = z.ID
            WHERE z.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")} and u.LPU is NULL
 UPDATE D3_USL_OMS
            SET	PROFIL = s.PROFIL   
            
            FROM D3_SL_OMS AS s 
            JOIN D3_USL_OMS AS u ON u.D3_SLID = s.ID
			JOIN D3_ZSL_OMS AS z ON s.D3_ZSLID = z.ID
            WHERE z.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")} and u.PROFIL is NULL
UPDATE D3_USL_OMS
            SET	DET = s.DET   
            
            FROM D3_SL_OMS AS s 
            JOIN D3_USL_OMS AS u ON u.D3_SLID = s.ID
			JOIN D3_ZSL_OMS AS z ON s.D3_ZSLID = z.ID
            WHERE z.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")} and u.DET is NULL
UPDATE D3_USL_OMS
            SET	DATE_IN = s.DATE_1  
            
            FROM D3_SL_OMS AS s 
            JOIN D3_USL_OMS AS u ON u.D3_SLID = s.ID
			JOIN D3_ZSL_OMS AS z ON s.D3_ZSLID = z.ID
            WHERE z.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")} and u.DATE_IN is NULL
UPDATE D3_USL_OMS
            SET	DATE_OUT = s.DATE_2   
            
            FROM D3_SL_OMS AS s 
            JOIN D3_USL_OMS AS u ON u.D3_SLID = s.ID
			JOIN D3_ZSL_OMS AS z ON s.D3_ZSLID = z.ID
            WHERE z.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")} and u.DATE_OUT is NULL
UPDATE D3_USL_OMS
            SET	DS = s.DS1   
            
            FROM D3_SL_OMS AS s 
            JOIN D3_USL_OMS AS u ON u.D3_SLID = s.ID
			JOIN D3_ZSL_OMS AS z ON s.D3_ZSLID = z.ID
            WHERE z.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")} and u.DS is NULL
UPDATE D3_USL_OMS
            SET	CODE_USL = u.VID_VME   
            
            FROM D3_SL_OMS AS s 
            JOIN D3_USL_OMS AS u ON u.D3_SLID = s.ID
			JOIN D3_ZSL_OMS AS z ON s.D3_ZSLID = z.ID
            WHERE z.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")} and u.CODE_USL is NULL
UPDATE D3_USL_OMS
            SET	PRVS = s.PRVS   
            
            FROM D3_SL_OMS AS s 
            JOIN D3_USL_OMS AS u ON u.D3_SLID = s.ID
			JOIN D3_ZSL_OMS AS z ON s.D3_ZSLID = z.ID
            WHERE z.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")} and u.PRVS is NULL
UPDATE D3_USL_OMS
            SET	CODE_MD = s.IDDOKT   
            
            FROM D3_SL_OMS AS s 
            JOIN D3_USL_OMS AS u ON u.D3_SLID = s.ID
			JOIN D3_ZSL_OMS AS z ON s.D3_ZSLID = z.ID
            WHERE z.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")} and u.CODE_MD is NULL
UPDATE D3_USL_OMS
            SET	KOL_USL = 1  
            
            FROM D3_SL_OMS AS s 
            JOIN D3_USL_OMS AS u ON u.D3_SLID = s.ID
			JOIN D3_ZSL_OMS AS z ON s.D3_ZSLID = z.ID
            WHERE z.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")} and u.KOL_USL is NULL
UPDATE D3_USL_OMS
            SET	TARIF = 0.00   
            
            FROM D3_SL_OMS AS s 
            JOIN D3_USL_OMS AS u ON u.D3_SLID = s.ID
			JOIN D3_ZSL_OMS AS z ON s.D3_ZSLID = z.ID
            WHERE z.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")} and u.TARIF is NULL
UPDATE D3_USL_OMS
            SET	SUMV_USL = 0.00   
            
            FROM D3_SL_OMS AS s 
            JOIN D3_USL_OMS AS u ON u.D3_SLID = s.ID
			JOIN D3_ZSL_OMS AS z ON s.D3_ZSLID = z.ID
            WHERE z.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")} and u.SUMV_USL is NULL
            ", SprClass.LocalConnectionString);





            var pacientList = Reader2List.CustomSelect<D3_PACIENT_OMS>($@"
			Select distinct t.* FROM (
			Select pa.* From D3_ZSL_OMS zsl
            Join D3_PACIENT_OMS pa on zsl.D3_PID = pa.id
            where zsl.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")}
            --order by pa.FAM, pa.IM, pa.OT, pa.DR
			UNION
			Select pa.* From SLUCH sl
            Join D3_PACIENT_OMS pa on sl.PID = pa.id
            where sl.SCHET_ID = {ObjHelper.GetAnonymousValue(schet, "ID")}
			and sl.USL_OK in (1,2)
			) as t
            order by FAM, IM, OT, DR
            ", SprClass.LocalConnectionString);

            var zsluchList = Reader2List.CustomSelect<D3_ZSL_OMS>($@"
            Select ID, D3_PID, D3_SCID,  [VIDPOM]      ,[FOR_POM]      ,[NPR_MO]      ,[LPU]      ,[VBR]      ,[DATE_Z_1]      ,[DATE_Z_2]      ,[P_OTK]      ,[RSLT_D]      ,[KD_Z]      ,[VNOV_M]      ,[RSLT]      ,[ISHOD]
					,[OS_SLUCH]      ,[OS_SLUCH_REGION]      ,[VOZR]      ,[VB_P]      ,[IDSP]      ,[SUMV]      ,NULL [OPLATA]      ,[SUMP]      ,[SANK_IT]      ,[IDCASE]      ,ISNULL ( [ZSL_ID], NEWID()) [ZSL_ID],
                      NULL D3_PGID, NULL FLK_COMENT, NULL OSP_COMENT, NULL USERID, VETERAN
			From D3_ZSL_OMS zsl
            where zsl.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")} 
			UNION ALL
            Select ID, PID D3_PID, SCHET_ID D3_SCID,  [VIDPOM]      ,
            case 
                when extr = 2 and USL_OK in (1,2) then 1
                when (extr = 1 OR EXTR IS NULL) and USL_OK in (1,2) then 3
                when SLUCH_TYPE = 1 and USL_OK = 4 then 1
                when SLUCH_TYPE = 3 and USL_OK = 4 then 2
                else 3 
            end [FOR_POM]      
            ,[NPR_MO]      ,[LPU]      ,NULL [VBR]      ,DATE_1 [DATE_Z_1]      ,DATE_2 [DATE_Z_2]      ,NULL [P_OTK]      ,[RSLT_D]      ,							
			(CASE 
				WHEN sl.USL_OK = 1 THEN (CASE WHEN sl.DATE_1 = sl.DATE_2 THEN 1 ELSE DATEDIFF(DAY, sl.DATE_1, sl.DATE_2) END)
				WHEN sl.USL_OK = 2 THEN DATEDIFF(DAY, sl.DATE_1, sl.DATE_2) + 1 -
				(SELECT COUNT(*) FROM dbo.WORK_DAY wd WHERE wd.LPU = sl.LPU and wd.PODR_ID = sl.LPU_1 and H_DATE BETWEEN sl.DATE_1 AND sl.DATE_2)
			END) [KD_Z]      ,NULL [VNOV_M]      ,[RSLT]      ,[ISHOD]
					,[OS_SLUCH]      ,[OS_SLUCH_REGION]      ,[VOZR]      ,NULL [VB_P]      ,[IDSP]      ,[SUMV]      ,NULL [OPLATA]      ,[SUMP]      ,[SANK_IT]      ,[IDCASE]      ,IDSLG [ZSL_ID],
                      NULL D3_PGID, NULL FLK_COMENT, NULL OSP_COMENT, NULL USERID, NULL VETERAN
			From SLUCH sl
            where sl.SCHET_ID = {ObjHelper.GetAnonymousValue(schet, "ID")}
			and USL_OK in (1,2) 
            ", SprClass.LocalConnectionString).OrderBy(x => x.D3_PID);


            var sluchList = Reader2List.CustomSelect<D3_SL_OMS>($@"
            Select sl.ID  ,[D3_ZSLID]      ,ISNULL ( [SL_ID], NEWID()) [SL_ID]      ,[USL_OK]      ,[VID_HMP]      ,[METOD_HMP]      ,[LPU_1]      ,[PODR]      ,[PROFIL]      ,[DET]      ,[P_CEL]      ,[TAL_N]      ,[TAL_D]      ,[TAL_P]
				,[NHISTORY]      ,[P_PER]      ,[DATE_1]      ,[DATE_2]      ,[KD]      ,[DS0]      ,[DS1]      ,[DS1_PR]      ,[DN]      ,[CODE_MES1]      ,[CODE_MES2]      ,[N_KSG]      ,[KSG_PG]
					,[SL_K]      ,[IT_SL]      ,[REAB]      ,[PRVS]      ,'V015' [VERS_SPEC]      ,[IDDOKT]      ,[ED_COL]      ,[TARIF]      ,[SUM_M]      ,[COMENTSL]      ,[KSG_DKK]
			From D3_SL_OMS sl
			JOIN D3_ZSL_OMS zsl on sl.D3_ZSLID = zsl.ID
            where zsl.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")} 
			UNION ALL
            Select u.slid ID , SLID [D3_ZSLID]      ,/*ISNULL (IDSLG, NEWID()) [SL_ID]*/ NEWID() [SL_ID]  ,[USL_OK]      ,[VID_HMP]      ,[METOD_HMP]      ,u.[LPU_1]      ,u.[PODR]      ,u.[PROFIL]      ,u.[DET]      ,NULL [P_CEL]      ,NNAPR [TAL_N]      ,DNAPR [TAL_D]      ,NULL [TAL_P]
				,[NHISTORY]      ,NULL [P_PER]      ,[DATE_IN]      ,[DATE_OUT]      ,
				(CASE 
					WHEN sl.USL_OK = 1 THEN (CASE WHEN u.DATE_IN = u.DATE_OUT THEN 1 ELSE DATEDIFF(DAY, u.DATE_IN, u.DATE_OUT) END)
					WHEN sl.USL_OK = 2 THEN DATEDIFF(DAY, u.DATE_IN, u.DATE_OUT) + 1 -
					(SELECT COUNT(*) FROM dbo.WORK_DAY wd WHERE wd.LPU = u.LPU and wd.PODR_ID = u.LPU_1 and H_DATE BETWEEN u.DATE_IN AND u.DATE_OUT)
                END) [KD]      ,[DS0]      ,[DS]      , null [DS1_PR]      ,null [DN]      ,[CODE_MES1]      ,[CODE_MES2]      ,(select KSGNUM from SprKsg where id = u.IDKSG) [N_KSG]
				      ,null [KSG_PG]
					,case when ISNULL(u.DIFF_K, 0.00) <> 0.00 then 1 else null end [SL_K]      ,case when ISNULL(u.DIFF_K, 0.00) <> 0.00 then u.DIFF_K else null end  [IT_SL]      ,null [REAB]      ,u.MSPUID [PRVS]      ,'V015' [VERS_SPEC]      ,CODE_MD [IDDOKT]      ,u.KOL_USL      ,u.[TARIF]      ,u.SUMV_USL [SUM_M]      ,[COMENTSL]      ,[KSG_DKK]
			From USL u
			join SLUCH sl on u.SLID = sl.ID
            where sl.SCHET_ID = {ObjHelper.GetAnonymousValue(schet, "ID")}
			and USL_OK in (1,2) and u.CODE_USL like 'TF%'
			--UNION ALL
			--SELECT [ID]       , ID [D3_ZSLID]      , ISNULL (IDSLG, NEWID()) [SL_ID]      ,[USL_OK]      ,[VID_HMP]      ,[METOD_HMP]      ,[LPU_1]      ,[PODR]      ,[PROFIL]      ,[DET]
			--,CASE
			--	WHEN SLUCH_TYPE = 1 and USL_OK = 3 THEN  '1.3'
			--	WHEN SLUCH_TYPE = 2 and USL_OK = 3 THEN  '1.1'
			--	WHEN SLUCH_TYPE = 3 and USL_OK = 3 THEN  '2.0'
			--	END [P_CEL]      
			--	, NULL [TAL_N]      ,NULL [TAL_D]      ,NULL [TAL_P]      ,[NHISTORY]      ,NULL [P_PER]      ,[DATE_1]      ,[DATE_2]
			--		 ,NULL [KD]      ,[DS0]      ,[DS1]      ,NULL [DS1_PR]      ,NULL [DN]      ,[CODE_MES1]      ,[CODE_MES2]      ,NULL [N_KSG]      ,NULL [KSG_PG]      ,NULL [SL_K]      ,NULL [IT_SL]      ,NULL [REAB]      , MSPID [PRVS]      ,'V015' [VERS_SPEC]      ,[IDDOKT]      ,[ED_COL]      ,[TARIF]      ,SUMV [SUM_M]      ,[COMENTSL]      ,NULL [KSG_DKK]
			--FROM [dbo].SLUCH sl
			--where sl.SCHET_ID = {ObjHelper.GetAnonymousValue(schet, "ID")} and 
			--USL_OK in (3,4)
            ", SprClass.LocalConnectionString).OrderBy(x => x.D3_ZSLID);

            var ds2List = Reader2List.CustomSelect<SLUCH_DS2>($@"
            Select distinct ds2.* From SLUCH sl
            Join SLUCH_DS2 ds2 on sl.ID = ds2.SLID
            where sl.schet_id = {ObjHelper.GetAnonymousValue(schet, "ID")}
            ", SprClass.LocalConnectionString).OrderBy(x => x.ID);

            var ds3List = Reader2List.CustomSelect<SLUCH_DS2>($@"
            Select distinct ds3.* From SLUCH sl
            Join SLUCH_DS3 ds3 on sl.ID = ds3.SLID
            where sl.schet_id = {ObjHelper.GetAnonymousValue(schet, "ID")}
            ", SprClass.LocalConnectionString).OrderBy(x => x.ID);

            var kslpList = Reader2List.CustomSelect<USL_KSLP>($@"
			SELECT distinct 	ks.[ID], ks.[IDSL], ks.[Z_SL], sl.[ID] SLID
			From SLUCH sl
            Join USL u on sl.id = u.SLID
            JOIN USL_KSLP ks on ks.SLID = u.ID
            where sl.SCHET_ID = {ObjHelper.GetAnonymousValue(schet, "ID")}
			and USL_OK in (1,2)
            ", SprClass.LocalConnectionString).OrderBy(x => x.ID);

            var uslList = Reader2List.CustomSelect<D3_USL_OMS>($@"
			SELECT  u.ID      ,[D3_SLID]      ,u.[D3_ZSLID]      ,[D3_SLGID]      ,[IDSERV]      ,u.[LPU]      ,u.[LPU_1]      ,u.[PODR]      ,u.[PROFIL]
					,[VID_VME]      ,u.[DET]      ,[DATE_IN]      ,[DATE_OUT]      ,u.[P_OTK]      ,[DS]      ,[CODE_USL]      ,[KOL_USL]      ,u.[TARIF]
						,[SUMV_USL]      ,u.[PRVS]      ,[CODE_MD]      ,[NPL]      ,[COMENTU]
			From D3_SL_OMS sl
            Join D3_USL_OMS u on sl.id = u.D3_SLID
			join D3_ZSL_OMS zsl on sl.D3_ZSLID = zsl.ID
            where zsl.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")}
			UNION
						
			SELECT  u.ID  , u.slid [D3_SLID]      ,null [D3_ZSLID]      ,null [D3_SLGID]      ,[IDSERV]      ,u.[LPU]      ,u.[LPU_1]      ,u.[PODR]      ,u.[PROFIL]
					,[VID_VME]      ,u.[DET]      ,[DATE_IN]      ,[DATE_OUT]      ,null [P_OTK]      ,[DS]      ,[CODE_USL]      ,[KOL_USL]      ,u.[TARIF]
						,[SUMV_USL]      ,u.MSPUID [PRVS]      ,[CODE_MD]      ,null [NPL]      ,[COMENTU]
			From SLUCH sl
            Join USL u on sl.id = u.SLID
            where sl.SCHET_ID = {ObjHelper.GetAnonymousValue(schet, "ID")}
			and (USL_OK in (1,2) and u.CODE_USL like 'VM%')-- or USL_OK in (3,4)
            ", SprClass.LocalConnectionString);

            //var sankList = Reader2List.CustomSelect<SANK>(
            //(SprClass.ProdSett.OrgTypeStatus == OrgType.Smo || SprClass.ProdSett.OrgTypeStatus == OrgType.Lpu) && isTfomsSchet ? $@"
            //Select distinct sa.* From SLUCH sl
            //Join sank sa on sl.id = sa.slid
            //where sl.schet_id = {ObjHelper.GetAnonymousValue(schet, "ID")} and (sl.vopl is null or sl.vopl = 1)" :
            //SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms && isTfomsSchet ? $@"
            //Select distinct sa.* From SLUCH sl
            //Join D3_PACIENT_OMS pa on sl.pid = pa.id and (pa.SMO is null or pa.SMO not like '46%')
            //Join sank sa on sl.id = sa.slid
            //where sl.schet_id = {ObjHelper.GetAnonymousValue(schet, "ID")}" :
            //$@"
            //Select distinct sa.* From SLUCH sl
            //Join sank sa on sl.id = sa.slid
            //where sl.SCHET_OMS_ID = {ObjHelper.GetAnonymousValue(schet, "CODE")} and (sl.vopl is null or sl.vopl = 1)", SprClass.LocalConnectionString);

            decimal? sumInTer = null;
            //if (SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms)
            {
                var sumInTerQuery = Reader2List.CustomAnonymousSelect(
                        $@"
                Select sum(tt.SUMV) as SUMV from 
				(Select distinct sl.SUMV From SLUCH sl
            Join D3_PACIENT_OMS pa on sl.pid = pa.id and(pa.SMO is null or pa.SMO not like '46%')
            where sl.schet_id = {ObjHelper.GetAnonymousValue(schet, "ID")} 
			union
			Select distinct zsl.SUMV From D3_ZSL_OMS zsl
            Join D3_PACIENT_OMS pa on zsl.D3_PID = pa.id and(pa.SMO is null or pa.SMO not like '46%')
            where zsl.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")} 
			) as tt
            ", SprClass.LocalConnectionString);
                sumInTer = (decimal?)ObjHelper.GetAnonymousValue(((IList)sumInTerQuery)[0], "SUMV");
            }


            var isWriter3 = false;

            int nzap1 = 1;
            int nzap3 = 1;
            int idcase1 = 1;
            int idcase3 = 1;
            XmlWriterSettings writerSettings
                = new XmlWriterSettings()
                {
                    Encoding = Encoding.GetEncoding("windows-1251"),
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = Environment.NewLine,
                    ConformanceLevel = ConformanceLevel.Document
                };
            using (ZipFile zip = new ZipFile())
            {
                var stream1 = new MemoryStream();
                var stream2 = new MemoryStream();
                var stream3 = new MemoryStream();

                using (XmlWriter writer1 = XmlWriter.Create(stream1, writerSettings))
                {
                    using (XmlWriter writer2 = XmlWriter.Create(stream2, writerSettings))
                    {
                        using (XmlWriter writer3 = XmlWriter.Create(stream3, writerSettings))
                        {
                            // writer1
                            writer1.WriteStartElement("ZL_LIST");
                            writer1.WriteStartElement("ZGLV");
                            writer1.WriteElementString("VERSION", "3.0");
                            writer1.WriteElementString("DATA", DateTime.Now.Date.ToString("yyyy-MM-dd"));
                            writer1.WriteElementString("FILENAME", filename1);
                            writer1.WriteElementString("SD_Z", zsluchList.Count().ToString());
                            writer1.WriteEndElement();
                            writer1.WriteStartElement("D3_SCHET_OMS");
                            if (ObjHelper.GetAnonymousValue(schet, "CODE") != null) writer1.WriteElementString("CODE", Convert.ToString(ObjHelper.GetAnonymousValue(schet, "CODE")));
                            else writer1.WriteElementString("CODE", ObjHelper.GetAnonymousValue(schet, "ID").ToString());
                            if (ObjHelper.GetAnonymousValue(schet, "CODE_MO") != null) writer1.WriteElementString("CODE_MO", (string)ObjHelper.GetAnonymousValue(schet, "CODE_MO"));
                            if (ObjHelper.GetAnonymousValue(schet, "YEAR") != null) writer1.WriteElementString("YEAR", Convert.ToString(ObjHelper.GetAnonymousValue(schet, "YEAR")));
                            if (ObjHelper.GetAnonymousValue(schet, "MONTH") != null) writer1.WriteElementString("MONTH", Convert.ToString(ObjHelper.GetAnonymousValue(schet, "MONTH")));
                            if (ObjHelper.GetAnonymousValue(schet, "NSCHET") != null) writer1.WriteElementString("NSCHET", (string)ObjHelper.GetAnonymousValue(schet, "NSCHET"));
                            if (ObjHelper.GetAnonymousValue(schet, "DSCHET") != null) writer1.WriteElementString("DSCHET", ((DateTime)ObjHelper.GetAnonymousValue(schet, "DSCHET")).Date.ToString("yyyy-MM-dd"));
                            if (ObjHelper.GetAnonymousValue(schet, "PLAT") != null) writer1.WriteElementString("PLAT", (string)ObjHelper.GetAnonymousValue(schet, "PLAT"));
                            writer1.WriteElementString("SUMMAV", Convert.ToString(sumInTer).Replace(",", "."));
                            if (ObjHelper.GetAnonymousValue(schet, "COMENTS") != null) writer1.WriteElementString("COMENTS", (string)ObjHelper.GetAnonymousValue(schet, "COMENTS"));
                            if (ObjHelper.GetAnonymousValue(schet, "SUMMAP") != null && (SprClass.ProdSett.OrgTypeStatus == OrgType.Smo || SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms)) writer1.WriteElementString("SUMMAP", Convert.ToString(ObjHelper.GetAnonymousValue(schet, "SUMMAP")).Replace(",", "."));
                            if (ObjHelper.GetAnonymousValue(schet, "SANK_MEK") != null && (SprClass.ProdSett.OrgTypeStatus == OrgType.Smo || SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms)) writer1.WriteElementString("SANK_MEK", Convert.ToString(ObjHelper.GetAnonymousValue(schet, "SANK_MEK")).Replace(",", "."));
                            if (ObjHelper.GetAnonymousValue(schet, "SANK_MEE") != null && (SprClass.ProdSett.OrgTypeStatus == OrgType.Smo || SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms)) writer1.WriteElementString("SANK_MEE", Convert.ToString(ObjHelper.GetAnonymousValue(schet, "SANK_MEE")).Replace(",", "."));
                            if (ObjHelper.GetAnonymousValue(schet, "SANK_EKMP") != null && (SprClass.ProdSett.OrgTypeStatus == OrgType.Smo || SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms)) writer1.WriteElementString("SANK_EKMP", Convert.ToString(ObjHelper.GetAnonymousValue(schet, "SANK_EKMP")).Replace(",", "."));
                            if ((string)ObjHelper.GetAnonymousValue(schet, "COMENTS") != null)
                            {
                                var com = (string)ObjHelper.GetAnonymousValue(schet, "COMENTS");
                                if (com.StartsWith("DP"))
                                    writer1.WriteElementString("DISP", "ДВ1");
                                if (com.StartsWith("DV"))
                                    writer1.WriteElementString("DISP", "ДВ2");
                                if (com.StartsWith("DO"))
                                    writer1.WriteElementString("DISP", "ОПВ");
                                if (com.StartsWith("DS"))
                                    writer1.WriteElementString("DISP", "ДС1");
                                if (com.StartsWith("DU"))
                                    writer1.WriteElementString("DISP", "ДС2");
                                if (com.StartsWith("DF"))
                                    writer1.WriteElementString("DISP", "ОН1");
                                if (com.StartsWith("DD"))
                                    writer1.WriteElementString("DISP", "ОН2");
                                if (com.StartsWith("DR"))
                                    writer1.WriteElementString("DISP", "ОН3");
                            }

                            writer1.WriteEndElement();
                            //
                            // writer2
                            writer2.WriteStartElement("PERS_LIST");
                            writer2.WriteStartElement("ZGLV");
                            writer2.WriteElementString("VERSION", "3.0");
                            writer2.WriteElementString("DATA", DateTime.Now.Date.ToString("yyyy-MM-dd"));
                            writer2.WriteElementString("FILENAME", filename2);
                            writer2.WriteElementString("FILENAME1", filename1);
                            writer2.WriteEndElement();
                            //

                            foreach (var zap in pacientList)
                            {
                                var pacWriter3 = false;
                                var pacWriter1 = false;

                                //writer2
                                writer2.WriteStartElement("PERS");
                                if (zap.ID_PAC != null) writer2.WriteElementString("ID_PAC", zap.ID_PAC);
                                if (zap.FAM != null) writer2.WriteElementString("FAM", (zap.FAM));
                                if (zap.IM != null) writer2.WriteElementString("IM", zap.IM);
                                if (zap.OT != null) writer2.WriteElementString("OT", zap.OT);
                                if (zap.W != null) writer2.WriteElementString("W", Convert.ToString(zap.W));
                                if (zap.DR != null) writer2.WriteElementString("DR", zap.DR.Value.ToString("yyyy-MM-dd"));
                                //if (zap.DOST != null) writer2.WriteElementString("DOST", zap.DOST.ToString());
                                if (zap.FAM_P != null) writer2.WriteElementString("FAM_P", zap.FAM_P);
                                if (zap.IM_P != null) writer2.WriteElementString("IM_P", zap.IM_P);
                                if (zap.OT_P != null) writer2.WriteElementString("OT_P", zap.OT_P);
                                if (zap.W_P != null) writer2.WriteElementString("W_P", zap.W_P.ToString());
                                if (zap.DR_P != null) writer2.WriteElementString("DR_P", zap.DR_P.Value.ToString("yyyy-MM-dd"));
                                if (zap.MR != null) writer2.WriteElementString("MR", zap.MR);
                                if (zap.DOCTYPE != null) writer2.WriteElementString("DOCTYPE", zap.DOCTYPE.ToString());
                                if (zap.DOCSER != null) writer2.WriteElementString("DOCSER", (zap.DOCSER));
                                if (zap.DOCNUM != null) writer2.WriteElementString("DOCNUM", (zap.DOCNUM));
                                if (zap.SNILS != null) writer2.WriteElementString("SNILS", zap.SNILS);
                                if (zap.OKATOG != null) writer2.WriteElementString("OKATOG", zap.OKATOG);
                                if (zap.OKATOP != null) writer2.WriteElementString("OKATOP", zap.OKATOP);
                                if (zap.COMENTP != null) writer2.WriteElementString("COMENTP", zap.COMENTP);
                                writer2.WriteEndElement();
                                //
                                foreach (var zslu in zsluchList.Where(x => x.D3_PID == zap.ID))
                                {
                                    if (pacWriter1 == false)
                                    {
                                        // writer1
                                        writer1.WriteStartElement("ZAP");
                                        writer1.WriteElementString("N_ZAP", Convert.ToString(nzap1++));
                                        writer1.WriteElementString("PR_NOV", "0");
                                        writer1.WriteStartElement("D3_PACIENT_OMS");
                                        if (zap.ID_PAC != null) writer1.WriteElementString("ID_PAC", zap.ID_PAC);
                                        if (zap.VPOLIS != null) writer1.WriteElementString("VPOLIS", Convert.ToString(zap.VPOLIS));
                                        if (zap.SPOLIS != null) writer1.WriteElementString("SPOLIS", zap.SPOLIS);
                                        if (zap.NPOLIS != null) writer1.WriteElementString("NPOLIS", zap.NPOLIS);
                                        if (zap.SMO != null) writer1.WriteElementString("SMO", zap.SMO);
                                        if (zap.SMO_OGRN != null && zap.SMO == null) writer1.WriteElementString("SMO_OGRN", zap.SMO_OGRN);
                                        if (zap.SMO_OK != null && zap.SMO == null) writer1.WriteElementString("SMO_OK", zap.SMO_OK);
                                        if (zap.SMO_NAM != null && zap.SMO == null) writer1.WriteElementString("SMO_NAM", zap.SMO_NAM);
                                        if (zap.NOVOR != null) writer1.WriteElementString("NOVOR", zap.NOVOR);
                                        else writer1.WriteElementString("NOVOR", "0");
                                        writer1.WriteEndElement();
                                        pacWriter1 = true;
                                    }

                                    writer1.WriteStartElement("Z_SL");
                                    writer1.WriteElementString("IDCASE", idcase1++.ToString());
                                    writer1.WriteElementString("ZSL_ID", zslu.ZSL_ID.ToString());
                                    if (zslu.VIDPOM != null) writer1.WriteElementString("VIDPOM", zslu.VIDPOM.ToString());
                                    if (zslu.FOR_POM != null) writer1.WriteElementString("FOR_POM", zslu.FOR_POM.ToString());
                                    if (zslu.NPR_MO != null) writer1.WriteElementString("NPR_MO", zslu.NPR_MO);
                                    if (zslu.LPU != null) writer1.WriteElementString("LPU", zslu.LPU);
                                    else
                                        writer1.WriteElementString("LPU",
                                            (ObjHelper.GetAnonymousValue(schet, "CODE_MO").ToString()));
                                    if (zslu.VBR != null) writer1.WriteElementString("VBR", zslu.VBR.ToString());
                                    //if (!string.IsNullOrWhiteSpace(slu.NHISTORY)) writer1.WriteElementString("NHISTORY", slu.NHISTORY);
                                    //else writer1.WriteElementString("NHISTORY", slu.ID.ToString());
                                    if (zslu.DATE_Z_1 != null) writer1.WriteElementString("DATE_Z_1", zslu.DATE_Z_1.Value.ToString("yyyy-MM-dd"));
                                    if (zslu.DATE_Z_2 != null) writer1.WriteElementString("DATE_Z_2", zslu.DATE_Z_2.Value.ToString("yyyy-MM-dd"));
                                    if (zslu.RSLT_D != null) writer1.WriteElementString("RSLT_D", zslu.RSLT_D.ToString());
                                    if (zslu.KD_Z != null) writer1.WriteElementString("KD_Z", zslu.KD_Z.ToString());

                                    //foreach (var ds2 in ds2List.Where(x => x.SLID == slu.ID))
                                    //{
                                    //    if (ds2.DS != null) writer1.WriteElementString("DS2", ds2.DS);
                                    //}
                                    //foreach (var ds3 in ds3List.Where(x => x.SLID == slu.ID))
                                    //{
                                    //    if (ds3.DS != null) writer1.WriteElementString("DS3", ds3.DS);
                                    //}
                                    if (zslu.VNOV_M != null) writer1.WriteElementString("VNOV_M", zslu.VNOV_M.ToString());
                                    if (zslu.RSLT != null) writer1.WriteElementString("RSLT", zslu.RSLT.ToString());
                                    if (zslu.ISHOD != null) writer1.WriteElementString("ISHOD", zslu.ISHOD.ToString());
                                    if (zslu.OS_SLUCH           != null) writer1.WriteElementString("OS_SLUCH", zslu.OS_SLUCH.ToString());
                                    if (zslu.OS_SLUCH_REGION    != null) writer1.WriteElementString("OS_SLUCH_REGION", zslu.OS_SLUCH_REGION.ToString());
                                    if (zslu.VOZR               != null) writer1.WriteElementString("VOZR", zslu.VOZR);
                                    //if (zslu.VETERAN != null) writer1.WriteElementString("VETERAN", zslu.VETERAN.ToString());

                                    //if (zslu.VETERAN          !  = null && plat.StartsWith("57")) writer1.WriteElementString("CODE_KSG", slu.IDKSG.ToString());
                                    //if (zslu.WORK_STAT        !  = null && plat.StartsWith("57")) writer1.WriteElementString("CODE_KSG", slu.IDKSG.ToString());
                                    if (zslu.VB_P               != null) writer1.WriteElementString("VB_P", zslu.VB_P.ToString());

                                    if (zslu.IDSP == 33)
                                    {
                                        var xx = sluchList.Where(x => x.D3_ZSLID == zslu.ID).ToList();
                                    }
                                    foreach (var sl in sluchList.Where(x => x.D3_ZSLID == zslu.ID))
                                    {
                                        writer1.WriteStartElement("SL");
                                        writer1.WriteElementString("SL_ID", sl.SL_ID.ToString());
                                        
                                        if (sl.VID_HMP      != null) writer1.WriteElementString("VID_HMP", sl.VID_HMP.ToString());
                                        if (sl.METOD_HMP    != null) writer1.WriteElementString("METOD_HMP", sl.METOD_HMP.ToString());
                                        if (sl.USL_OK       != null) writer1.WriteElementString("USL_OK", sl.USL_OK.ToString());
                                        if (sl.LPU_1        != null) writer1.WriteElementString("LPU_1", sl.LPU_1.ToString());
                                        if (sl.PODR         != null) writer1.WriteElementString("PODR", sl.PODR.ToString());
                                        if (sl.PROFIL       != null) writer1.WriteElementString("PROFIL", sl.PROFIL.ToString());
                                        if (sl.DET          != null) writer1.WriteElementString("DET", sl.DET.ToString());
                                        if (sl.P_CEL        != null) writer1.WriteElementString("P_CEL", sl.P_CEL.ToString());
                                        if (sl.TAL_NUM != null) writer1.WriteElementString("TAL_NUM", sl.TAL_NUM.ToString());
                                        if (sl.TAL_D        != null) writer1.WriteElementString("TAL_D", sl.TAL_D.ToString());
                                        if (sl.TAL_P        != null) writer1.WriteElementString("TAL_P", sl.TAL_P.ToString());
                                                                                if (sl.NHISTORY != null) writer1.WriteElementString("NHISTORY", sl.NHISTORY);
                                        else writer1.WriteElementString("NHISTORY", sl.ID.ToString());
                                                                                if (sl.P_PER        != null) writer1.WriteElementString("P_PER", sl.P_PER.ToString());
                                        if (sl.DATE_1       != null) writer1.WriteElementString("DATE_1", sl.DATE_1.Value.ToString("yyyy-MM-dd"));
                                        if (sl.DATE_2       != null) writer1.WriteElementString("DATE_2", sl.DATE_2.Value.ToString("yyyy-MM-dd"));
                                        //if (sl.P_OTK        != null) writer1.WriteElementString("P_OTK", sl.P_OTK.ToString());
                                        if (sl.KD           != null) writer1.WriteElementString("KD", sl.KD.ToString());
                                        if (sl.DS0          != null) writer1.WriteElementString("DS0", sl.DS0.ToString());
                                        if (sl.DS1          != null) writer1.WriteElementString("DS1", sl.DS1.ToString());
                                        if (sl.DS1_PR       != null) writer1.WriteElementString("DS1_PR", sl.DS1_PR.ToString());
                                        if (sl.DN != null) writer1.WriteElementString("DN", sl.DN.ToString());

                                        foreach (var ds2 in ds2List.Where(x => x.SLID == sl.ID))
                                        {
                                            if (ds2.DS != null)
                                            {
                                                writer1.WriteStartElement("DS2_N");
                                                writer1.WriteElementString("DS2", ds2.DS);
                                                //DS2_PR
                                                writer1.WriteEndElement();
                                            }
                                        }
                                        //if (sl.NAZ          != null) writer1.WriteElementString("S_TIP", sl.sank.S_TIP.ToString());

                                        foreach (var ds3 in ds3List.Where(x => x.SLID == sl.ID))
                                        {
                                            if (ds3.DS != null) writer1.WriteElementString("DS3", ds3.DS);
                                        }
                                        //if (sl.DS2_N        != null) writer1.WriteElementString("S_TIP", sl.sank.S_TIP.ToString());
                                        if (sl.CODE_MES1    != null) writer1.WriteElementString("CODE_MES1", sl.CODE_MES1.ToString());
                                        if (sl.CODE_MES2    != null) writer1.WriteElementString("CODE_MES2", sl.CODE_MES2.ToString());
                                        if (sl.KSG_DKK      != null) writer1.WriteElementString("KSG_DKK", sl.KSG_DKK.ToString());
                                        if (sl.N_KSG != null)
                                        {
                                            writer1.WriteStartElement("KSG");
                                            if (sl.N_KSG != null) writer1.WriteElementString("N_KSG", sl.N_KSG.ToString());
                                            if (sl.KSG_PG != null) writer1.WriteElementString("KSG_PG", sl.KSG_PG.ToString());
                                            if (sl.SL_K != null) writer1.WriteElementString("SL_K", sl.SL_K.ToString());
                                            if (sl.IT_SL != null) writer1.WriteElementString("IT_SL", sl.IT_SL.ToString().Replace(",", "."));
                                                foreach (var kslp in kslpList.Where(x => x.SLID == sl.ID))
                                                {
                                                    writer1.WriteStartElement("SL_KOEF");
                                                    writer1.WriteElementString("IDSL", kslp.IDSL.ToString());
                                                    writer1.WriteElementString("Z_SL", kslp.Z_SL.ToString().Replace(",", "."));

                                                    writer1.WriteEndElement();
                                                }

                                            writer1.WriteEndElement();

    //                                        SL_KOEF IDSL    O N(4)    Номер коэффициента сложности лечения пациента   В соответствии с региональным справочником.
    //Z_SL    O   N(1.5)  Значение коэффициента сложности лечения пациента

                                        }
                                        if (sl.REAB         != null) writer1.WriteElementString("REAB", sl.REAB.ToString());
                                        if (sl.PRVS         != null) writer1.WriteElementString("PRVS", sl.PRVS.ToString());
                                        if (sl.VERS_SPEC    != null) writer1.WriteElementString("VERS_SPEC", sl.VERS_SPEC.ToString());
                                        if (sl.IDDOKT       != null) writer1.WriteElementString("IDDOKT", sl.IDDOKT.ToString());
                                        if (sl.ED_COL       != null) writer1.WriteElementString("ED_COL", sl.ED_COL.ToString().Replace(",", "."));
                                        if (sl.TARIF        != null) writer1.WriteElementString("TARIF", sl.TARIF.ToString().Replace(",", "."));
                                        if (sl.SUM_M        != null) writer1.WriteElementString("SUM_M", sl.SUM_M.ToString().Replace(",", "."));
                                        
                                        int idserv = 1;
                                        foreach (var usl in uslList.Where(x => x.D3_SLID == sl.ID))
                                        {
                                            writer1.WriteStartElement("USL");
                                            writer1.WriteElementString("IDSERV", idserv++.ToString());
                                            if (usl.LPU != null) writer1.WriteElementString("LPU", usl.LPU);
                                            if (usl.LPU_1       != null) writer1.WriteElementString("LPU_1", usl.LPU_1);
                                            if (usl.PODR        != null) writer1.WriteElementString("PODR", usl.PODR);
                                            if (usl.PROFIL      != null) writer1.WriteElementString("PROFIL", usl.PROFIL.ToString());
                                            if (usl.VID_VME     != null) writer1.WriteElementString("VID_VME", usl.VID_VME);
                                            if (usl.DET         != null) writer1.WriteElementString("DET", usl.DET.ToString());
                                            if (usl.DATE_IN     != null) writer1.WriteElementString("DATE_IN", usl.DATE_IN.Value.ToString("yyyy-MM-dd"));
                                            if (usl.DATE_OUT    != null) writer1.WriteElementString("DATE_OUT", usl.DATE_OUT.Value.ToString("yyyy-MM-dd"));
                                            if (usl.DS          != null) writer1.WriteElementString("DS", usl.DS);
                                            if (usl.CODE_USL    != null) writer1.WriteElementString("CODE_USL", usl.CODE_USL);
                                            if (usl.KOL_USL     != null) writer1.WriteElementString("KOL_USL", usl.KOL_USL.ToString().Replace(",", "."));
                                            if (usl.TARIF       != null) writer1.WriteElementString("TARIF", usl.TARIF.ToString().Replace(",", "."));
                                            if (usl.SUMV_USL    != null) writer1.WriteElementString("SUMV_USL", usl.SUMV_USL.ToString().Replace(",", "."));
                                            if (usl.PRVS        != null) writer1.WriteElementString("PRVS", usl.PRVS.ToString());
                                            if (usl.CODE_MD     != null) writer1.WriteElementString("CODE_MD", usl.CODE_MD);
                                            if (usl.P_OTK       != null) writer1.WriteElementString("P_OTK", usl.P_OTK.ToString());
                                            if (usl.NPL         != null) writer1.WriteElementString("NPL", usl.NPL.ToString());
                                            if (usl.COMENTU != null) writer1.WriteElementString("COMENTU", usl.COMENTU);

                                            writer1.WriteEndElement();
                                        }

                                        if (sl.COMENTSL != null) writer1.WriteElementString("COMENTSL", sl.COMENTSL.ToString());


                                        writer1.WriteEndElement();
                                    }

                                    if (zslu.IDSP               != null) writer1.WriteElementString("IDSP", zslu.IDSP.ToString());
                                    if (zslu.SUMV               != null) writer1.WriteElementString("SUMV", zslu.SUMV.ToString().Replace(",", "."));
                                    if (zslu.OPLATA             != null) writer1.WriteElementString("OPLATA", zslu.OPLATA.ToString().Replace(",", "."));
                                    if (zslu.SUMP               != null) writer1.WriteElementString("SUMP", zslu.SUMP.ToString().Replace(",", "."));
                                    if (zslu.SANK_IT            != null) writer1.WriteElementString("SANK_IT", zslu.SANK_IT.ToString().Replace(",", "."));

                                    //foreach (var sank in sankList.Where(x => x.SLID == slu.ID))
                                    //{
                                    //    writer1.WriteStartElement("SANK");
                                    //    writer1.WriteElementString("S_CODE", Guid.NewGuid().ToString());
                                    //    if (sank.S_SUM != null) writer1.WriteElementString("S_SUM", sank.S_SUM.ToString().Replace(",", "."));
                                    //    if (sank.S_TIP != null) writer1.WriteElementString("S_TIP", sank.S_TIP.ToString());
                                    //    if (sank.S_OSN != null) writer1.WriteElementString("S_OSN", sank.S_OSN);
                                    //    if (sank.S_DATE != null) writer1.WriteElementString("S_DATE", sank.S_DATE.Value.ToString("yyyy-MM-dd"));
                                    //    if (sank.S_COM != null) writer1.WriteElementString("S_COM", sank.S_COM);
                                    //    if (sank.S_IST != null) writer1.WriteElementString("S_IST", sank.S_IST.ToString());
                                    //    writer1.WriteEndElement();
                                    //}

                                    writer1.WriteEndElement();
                                    if (isWriter3)
                                    {
                                        writer3.WriteEndElement();
                                    }
                                }
                                if (pacWriter1 == true)
                                    writer1.WriteEndElement();
                                if (isWriter3)
                                {
                                    writer3.WriteEndElement();
                                }

                            }
                            if (isWriter3)
                            {
                                writer3.WriteEndElement();
                                writer3.Flush();
                            }
                            writer3.Close();
                        }
                        writer2.WriteEndElement();
                        writer2.Flush();
                        writer2.Close();
                    }
                    writer1.WriteEndElement();
                    writer1.Flush();
                    writer1.Close();
                }

                string result1 = Encoding.Default.GetString(stream1.ToArray());
                string result2 = Encoding.Default.GetString(stream2.ToArray());
                string result3 = Encoding.Default.GetString(stream3.ToArray());

                zip.AddEntry(filename1 + ".xml", result1);
                zip.AddEntry(filename2 + ".xml", result2);
                if (isWriter3)
                    zip.AddEntry(filename3 + ".xml", result3);
                if (isLoop)
                {
                    if (!Directory.Exists(Environment.CurrentDirectory + @"\Out"))
                        Directory.CreateDirectory(Environment.CurrentDirectory + @"\Out");

                    var zipFile = Environment.CurrentDirectory + @"\Out\" + filename1 + ".oms";
                    zip.Save(zipFile);
                }
                else
                {
                    Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        if (plat.StartsWith("57"))
                        {
                            var num = isWriter3 ? "3" : "2";
                            saveFileDialog.Filter = "ZIP File (*.zip)|*.zip";
                            saveFileDialog.FileName = filename1.Insert(1, num) + ".zip";
                        }
                        else
                        {
                            saveFileDialog.Filter = "OMS File (*.oms)|*.oms";
                            saveFileDialog.FileName = filename1 + ".oms";
                        }

                        bool? result = saveFileDialog.ShowDialog();
                        if (result == true)
                            zip.Save(saveFileDialog.FileName);
                    });
                }
            }
            //LoadingDecorator1.IsSplashScreenShown = false;
        }

        public static MemoryStream ExportOspToXml(object sankExpList, string ifn)
        {
            XmlWriterSettings writerSettings
                = new XmlWriterSettings()
                {
                    Encoding = Encoding.GetEncoding("windows-1251"),
                    Indent = true,
                    IndentChars = "     ",
                    NewLineChars = Environment.NewLine,
                    ConformanceLevel = ConformanceLevel.Document
                };

            MemoryStream ms = new MemoryStream();

            using (ZipFile zip = new ZipFile())
            {
                var stream1 = new MemoryStream();

                using (XmlWriter writer1 = XmlWriter.Create(stream1, writerSettings))
                {
                    // writer1
                    writer1.WriteStartElement("ELMEDICINE");
                    //writer1.WriteElementString("FNAME", "O" + ifn.Remove(0, 1));
                    //writer1.WriteElementString("FNAME_I", ifn);

                    List2Xml("SLUCH", (IEnumerable<dynamic>)sankExpList, writer1);

                    writer1.WriteEndElement();
                    writer1.Flush();
                    writer1.Close();
                }

                string result1 = Encoding.Default.GetString(stream1.ToArray());
                zip.AddEntry("O" + ifn.Remove(0, 1) + ".xml", result1);

                zip.Save(ms);
            }
            return ms;
        }

        private static void List2Xml(string startElement, IEnumerable<dynamic> objects, XmlWriter writer)
        {

            foreach (var exp in objects)
            {
                writer.WriteStartElement(startElement);
                var obj = exp.GetType().GetProperties();
                foreach (var property in obj)
                {
                    var name = property.Name;
                    string value = null;
                    var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                    if (type == Type.GetType("System.String")) value = (string)property.GetValue(exp, null);
                    else if (type == Type.GetType("System.DateTime"))
                    {
                        var dv = property.GetValue(exp, null);
                        if (dv != null) value = ((DateTime)dv).ToString("yyyy-MM-dd");
                    }
                    else value = Convert.ToString(property.GetValue(exp, null), CultureInfo.InvariantCulture);
                    if (!string.IsNullOrEmpty(value)) writer.WriteElementString(name, value);
                }
                writer.WriteEndElement();
            }
        }

        private void ImportOms_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "OMS File |*.oms;*.zip";


            var result = openFileDialog.ShowDialog();

            var rfile = openFileDialog.FileName;
            var omszfn = openFileDialog.SafeFileName;

            string zapfn = "";
            string persfn = "";
            var zapms = new MemoryStream();
            var persms = new MemoryStream();

            if (result != true) return;

            var sc = ObjHelper.ClassConverter<D3_SCHET_OMS>(DxHelper.GetSelectedGridRow(EconomyWindow11.gridControl));
            ((DevExpress.Xpf.Bars.BarButtonItem)sender).IsEnabled = false;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (ZipFile zip = ZipFile.Read(rfile))
                    {
                        foreach (ZipEntry zipEntry in zip)
                        {
                            if (zipEntry.FileName.StartsWith("HM") || zipEntry.FileName.StartsWith("D") ||
                                zipEntry.FileName.StartsWith("T"))
                            {
                                zapfn = zipEntry.FileName;
                                zipEntry.Extract(zapms);
                                zapms.Position = 0;
                            }

                            if (zipEntry.FileName.StartsWith("L"))
                            {
                                persfn = zipEntry.FileName;
                                zipEntry.Extract(persms);
                                persms.Position = 0;
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

                var perssr = new StreamReader(persms, Encoding.Default);
                string persxml = perssr.ReadToEnd();
                persms.Dispose();
                persms.Close();

                if (string.IsNullOrEmpty(zapxml) || string.IsNullOrEmpty(persxml))
                {
                    //Reader2List.CustomExecuteQuery($@"Update DOX_SCHET SET DOX_STATUS=12 ", SprClass.LocalConnectionString);
                }
                else
                {
                    try
                    {
                        //                        var q = $@"

                        //BEGIN TRANSACTION;  
                        //BEGIN TRY

                        //    Declare @sc int
                        //    INSERT INTO D3_SCHET_OMS (ZAPXMLFILE, PERSXMLFILE, ZAPFILENAME, PERSFILENAME) VALUES (CAST('{zapxml.Replace("'", "")}' AS XML), CAST('{persxml.Replace("'", "")}' AS XML), '{zapfn}', '{persfn}')
                        //    Select @sc = SCOPE_IDENTITY()

                        //    EXEC p_oms_load_schet @sc
                        //    EXEC p_oms_load_pacient @sc
                        //    EXEC p_oms_load_zsl @sc
                        //    EXEC p_oms_load_sl @sc
                        //    EXEC p_oms_load_usl @sc

                        //END TRY
                        //BEGIN CATCH
                        //    IF @@TRANCOUNT > 0  
                        //        ROLLBACK TRANSACTION;  
                        //    --PRINT 'In catch block.';
                        //DECLARE
                        //   @ErMessage NVARCHAR(2048),
                        //   @ErSeverity INT,
                        //   @ErState INT

                        // SELECT
                        //   @ErMessage = ERROR_MESSAGE(),
                        //   @ErSeverity = ERROR_SEVERITY(),
                        //   @ErState = ERROR_STATE()

                        // RAISERROR (@ErMessage,
                        //             @ErSeverity,
                        //             @ErState )
                        //END CATCH;

                        //IF @@TRANCOUNT > 0  
                        //    COMMIT TRANSACTION;  
                        //";

                        var q = $@"
                            EXEC [dbo].[p_oms_load_all_newformat] '{zapxml.Replace("'", "")}', '{persxml.Replace("'", "")}', '{zapfn}', '{persfn}'
                        ";

                        //                        var q = $@"

                        //BEGIN TRANSACTION;  
                        //BEGIN TRY

                        //    Declare @sc int
                        //    Update D3_SCHET_OMS SET ZAPXMLFILE=CAST('{zapxml.Replace("'", "")}' AS XML), PERSXMLFILE=CAST('{persxml.Replace("'", "")}' AS XML), ZAPFILENAME='{zapfn}', PERSFILENAME='{persfn}' WHERE ID = {sc.ID}
                        //    Select @sc = {sc.ID}

                        //    --EXEC p_oms_load_schet @sc
                        //    EXEC p_oms_load_pacient @sc
                        //    EXEC p_oms_load_zsl @sc
                        //    EXEC p_oms_load_sl @sc
                        //    EXEC p_oms_load_usl @sc

                        //END TRY
                        //BEGIN CATCH
                        //    IF @@TRANCOUNT > 0  
                        //        ROLLBACK TRANSACTION;  
                        //    --PRINT 'In catch block.';
                        //DECLARE
                        //   @ErMessage NVARCHAR(2048),
                        //   @ErSeverity INT,
                        //   @ErState INT

                        // SELECT
                        //   @ErMessage = ERROR_MESSAGE(),
                        //   @ErSeverity = ERROR_SEVERITY(),
                        //   @ErState = ERROR_STATE()

                        // RAISERROR (@ErMessage,
                        //             @ErSeverity,
                        //             @ErState )
                        //END CATCH;

                        //IF @@TRANCOUNT > 0  
                        //    COMMIT TRANSACTION;  
                        //";
                        zapxml = null;
                        persxml = null;
                        Reader2List.CustomExecuteQuery(q, SprClass.LocalConnectionString);
                        q = null;
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
                perssr.Dispose();
                perssr.Close();


                //Console.WriteLine("Распакован " + id);

                GC.WaitForPendingFinalizers();
                GC.Collect();

            }).ContinueWith(x =>
            {
                EconomyWindow11.linqInstantFeedbackDataSource.Refresh();

                DXMessageBox.Show("Загрузка успешно завершена");

                ((DevExpress.Xpf.Bars.BarButtonItem)sender).IsEnabled = true;


            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var schets = DxHelper.GetSelectedGridRows(EconomyWindow11.gridControl)?.Select(x => ObjHelper.GetAnonymousValue(x, "ID")).ToArray();

            СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            {
                Header = "Печатные формы",
                MyControl = new StatisticReports(schets),
                IsCloseable = "True",
            });

        }

        private void LoadMis_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var sc = ObjHelper.ClassConverter<D3_SCHET_OMS>(DxHelper.GetSelectedGridRow(EconomyWindow11.gridControl));
                var idSchet = (int)ObjHelper.GetAnonymousValue(sc, "ID");
                var month = (int)ObjHelper.GetAnonymousValue(sc, "MONTH");
                var year = (int)ObjHelper.GetAnonymousValue(sc, "YEAR");
                var lpu = (string)ObjHelper.GetAnonymousValue(sc, "CODE_MO");
                StreamWriter file = new StreamWriter(Environment.CurrentDirectory + "\\run.cmd");
                file.Write("RegistryUpload.SlashScreenYamed.exe" + " -reestruploadYamed" + " -lpu=" + lpu + " -year=" + year + " -month=" + month + " -idSchet=" + idSchet);
                file.Close();
                ProcessStartInfo startInfo = new ProcessStartInfo(Environment.CurrentDirectory + "\\run.cmd");
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(startInfo).WaitForExit();
                if (Process.GetProcessesByName("RegistryUpload.SlashScreenYamed").Length == 0)
                {
                    DXMessageBox.Show("Данные загружены!");
                }
            }
            catch
            {
                DXMessageBox.Show("Не выбран счет для загрузки МИС");
            }
        }
    }
}
