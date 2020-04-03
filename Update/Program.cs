using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Ionic.Zip;
using Yamed.Core;
using Yamed.Server;

namespace Update
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {            
            //upd.Updater upd = new upd.Updater();
            //upd.Update();


            //
            //
            //
            //
            SprClass.LocalConnectionString = @"Data Source=192.168.201.251;Initial Catalog=ElmedicineNewIngos;User ID=igsm;Password=12345678";  //строка подключения
            //
            //
            //
            //
            //

            try
            {
                for (int i = 1; i < 13; i++)
                {
                    Console.WriteLine("Выгружаю " + i + " месяц");

                    var query = SqlReader.Select($@"
select 'КРК' F_SMO, YEAR,
CASE WHEN MONTH <10 THEN '0' + convert (nvarchar(2), MONTH) ELSE convert (nvarchar(2), MONTH) END [MONTH], 
ID_PAC, VPOLIS, SPOLIS, NPOLIS
-- sluch
,IDCASE
,LPU
,VBR
,DATE_Z_1 DATE_1
,DATE_Z_2 DATE_2
,CASE 
    WHEN OS_SLUCH_REGION in (1, 22) THEN 'ДВ1'
    WHEN OS_SLUCH_REGION in (2, 23, 48) THEN 'ДВ2' 
    WHEN OS_SLUCH_REGION in (37, 38) THEN 'ДВ3' 
    WHEN OS_SLUCH_REGION = 47 THEN 'ДВ4' 
    ELSE 'ОПВ' 
END DISP
,CASE 
WHEN RSLT in (select idrmp from V009 where rmpname like '%Присвоена I группа здоровья%') THEN	1
WHEN RSLT in (select idrmp from V009 where rmpname like '%Присвоена II группа здоровья%') THEN	2
WHEN RSLT in (select idrmp from V009 where rmpname like '%Присвоена III группа здоровья%') THEN	3
WHEN RSLT in (select idrmp from V009 where rmpname like '%Присвоена IV группа здоровья%') THEN	4
WHEN RSLT in (select idrmp from V009 where rmpname like '%Присвоена V группа здоровья%') THEN	5
WHEN RSLT in (select idrmp from V009 where rmpname like '%Направлен на II этап диспансеризации%') THEN	6
WHEN RSLT in (select idrmp from V009 where rmpname like '%предварительно присвоена I группа здоровья%') THEN	11
WHEN RSLT in (select idrmp from V009 where rmpname like '%предварительно присвоена II группа здоровья%') THEN	12
WHEN RSLT in (select idrmp from V009 where rmpname like '%предварительно присвоена III группа здоровья%') THEN	13
WHEN RSLT in (select idrmp from V009 where rmpname like '%предварительно присвоена IIIа группа здоровья%') THEN	14
WHEN RSLT in (select idrmp from V009 where rmpname like '%предварительно присвоена IIIб группа здоровья%') THEN	15
WHEN RSLT in (select idrmp from V009 where rmpname like '%Присвоена IIIа группа здоровья%') THEN	31
WHEN RSLT in (select idrmp from V009 where rmpname like '%Присвоена IIIб группа здоровья%') THEN	32
ELSE 1
END RSLT_D
,P_OTK
,DS1
,DS1_PR
,DS_ONK
,DN PR_D_N
,NAZ_R
,OPLATA
--/sluch


from D3_ZSL_OMS zsl
join D3_SL_OMS sl on zsl.ID = sl.D3_ZSLID
join D3_NAZ_OMS naz on naz.D3_SLID = sl.ID
join D3_PACIENT_OMS pa on zsl.d3_pid = pa.id
join D3_SCHET_OMS sc on zsl.D3_SCID = sc.ID and sc.YEAR = 2019 and sc.MONTH = {i}
where zsl.OS_SLUCH_REGION in (1,2,22,23,9, 37, 38, 47,48,49)", SprClass.LocalConnectionString);

                    if (!query.Any()) continue;

                    XmlStreem2(query);

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + Environment.NewLine + ex.InnerException?.Message);
            }

            Console.WriteLine("Выгрузка завершена");
            Console.ReadLine();




        }

        public static void XmlStreem2(ObservableCollection<DynamicBaseClass> query)
        {
            int nzap1 = 1;
            int idcase1 = 1;

            string filename1 = $"DISP_KRK18{query[0].GetValue("MONTH")}1";

            /*
             ZL_LIST	ZGLV	О	S	Заголовок файла	Информация о передаваемом файле
                SMO_INFO	О	S	СМО	Информация о филиале СМО
                ZAP	ОМ	S	Записи	Записи о случаях диспансеризации
            Заголовок файла
            ZGLV	VERSION	O	T(5)	Версия взаимодействия 	Текущей редакции соответствует значение «2.1».
                DATA	О	D	Дата	В формате ГГГГ-ММ-ДД
                FILENAME	О	T(26)	Имя файла	Имя файла без расширения.
                SD_Z	O	N(9)	Количество записей	Указывается количество лиц, прошедших диспансеризацию.
            СМО
            SMO_INFO	F_SMO	О	T(3)	Филиал СМО	Заполняется в соответствии с классификатором в Табл. 2 в русскоязычном формате
                YEAR	O	N(4)	Отчетный год	
                MONTH	O	N(2)	Отчетный месяц	
            Записи
            */

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

                using (XmlWriter writer1 = XmlWriter.Create(stream1, writerSettings))
                {
                    // открыли
                    writer1.WriteStartElement("ZL_LIST");
                    // открыли
                    writer1.WriteStartElement("ZGLV");
                    writer1.WriteElementString("VERSION", "3.1");
                    writer1.WriteElementString("DATA", DateTime.Today.ToString("yyy-MM-yy"));
                    writer1.WriteElementString("FILENAME", filename1);
                    writer1.WriteElementString("SD_Z", query.Count.ToString());
                    writer1.WriteEndElement();
                    // закрыли

                    // открыли
                    writer1.WriteStartElement("SMO_INFO");
                    writer1.WriteElementString("F_SMO", query[0].GetValue("F_SMO")?.ToString());
                    writer1.WriteElementString("YEAR", query[0].GetValue("YEAR")?.ToString());
                    writer1.WriteElementString("MONTH", query[0].GetValue("MONTH")?.ToString());
                    writer1.WriteEndElement();
                    // закрыли

                    foreach (var q in query)
                    {
                        // открыли
                        writer1.WriteStartElement("ZAP");
                        writer1.WriteElementString("N_ZAP", nzap1++.ToString());

                        // открыли
                        writer1.WriteStartElement("PACIENT");
                        writer1.WriteElementString("ID_PAC", q.GetValue("ID_PAC")?.ToString());
                        writer1.WriteElementString("VPOLIS", q.GetValue("VPOLIS")?.ToString());
                        //writer1.WriteElementString("SPOLIS", q.GetValue("SPOLIS").ToString());
                        writer1.WriteElementString("NPOLIS", q.GetValue("NPOLIS")?.ToString());
                        writer1.WriteEndElement();
                        // закрыли

                        // открыли
                        //writer1.WriteStartElement("SLUCH");
                        //writer1.WriteElementString("IDCASE", idcase1++.ToString());
                        //writer1.WriteElementString("LPU", q.GetValue("LPU").ToString());
                        //writer1.WriteElementString("DATE_1", ((DateTime)q.GetValue("DATE_Z_1")).ToString("yyyy-MM-dd"));
                        //writer1.WriteElementString("DATE_2", ((DateTime)q.GetValue("DATE_Z_2")).ToString("yyyy-MM-dd"));
                        //writer1.WriteElementString("DISP", q.GetValue("DISP").ToString());
                        //writer1.WriteElementString("RSLT_D", q.GetValue("RSLT_D").ToString());
                        //writer1.WriteEndElement();
                        // закрыли

                        // открыли
                        writer1.WriteStartElement("SLUCH");
                        writer1.WriteElementString("IDCASE", q.GetValue("IDCASE")?.ToString());
                        writer1.WriteElementString("LPU", q.GetValue("LPU")?.ToString());
                        writer1.WriteElementString("VBR", q.GetValue("VBR")?.ToString());
                        writer1.WriteElementString("DATE_1", ((DateTime?)q.GetValue("DATE_1"))?.ToString("yyyy-MM-dd"));
                        writer1.WriteElementString("DATE_2", ((DateTime?)q.GetValue("DATE_2"))?.ToString("yyyy-MM-dd"));
                        writer1.WriteElementString("DISP", q.GetValue("DISP")?.ToString());
                        writer1.WriteElementString("RSLT_D", q.GetValue("RSLT_D")?.ToString());

                        writer1.WriteElementString("P_OTK", q.GetValue("P_OTK")?.ToString());
                        writer1.WriteElementString("DS1", q.GetValue("DS1")?.ToString());
                        writer1.WriteElementString("DS1_PR", q.GetValue("DS1_PR")?.ToString());
                        writer1.WriteElementString("DS_ONK", q.GetValue("DS_ONK")?.ToString());
                        writer1.WriteElementString("PR_D_N", q.GetValue("PR_D_N")?.ToString());
                        writer1.WriteElementString("NAZ_R", q.GetValue("NAZ_R")?.ToString());
                        writer1.WriteElementString("OPLATA", q.GetValue("OPLATA")?.ToString());
                        writer1.WriteEndElement();
                        // закрыли


                        writer1.WriteEndElement();
                        // закрыли

                    }

                    writer1.WriteEndElement();
                    // закрыли

                    writer1.Flush();
                    writer1.Close();
                }

                string result1 = Encoding.Default.GetString(stream1.ToArray());

                zip.AddEntry(filename1 + ".xml", result1);
                var zipFile = @"D:\" + filename1 + ".oms";
                zip.Save(zipFile);
            }
        }
        }

    }