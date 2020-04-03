using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Xml;
using System.Xml.Linq;
using Yamed.Core;
using Yamed.Server;

namespace Yamed.Control.Editors
{
    /// <summary>
    /// Логика взаимодействия для Period.xaml
    /// </summary>
    public partial class Period : UserControl
    {
        public Period()
        {
            InitializeComponent();

        }
        public class Dost
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }
        public ObservableCollection<Dost> list0 = new ObservableCollection<Dost>();
        public ObservableCollection<Dost> list1 = new ObservableCollection<Dost>();

        public void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
          
            Export.IsEnabled = false;
            date_exp.IsEnabled = false;
            //список приказов
            list0.Add(new Dost { ID = 1, Name = "Приказ № 15 от 25.02.2014" });
            list0.Add(new Dost { ID = 2, Name = "Приказ № 17 от 26.02.2014" });
            list0.Add(new Dost { ID = 3, Name = "Приказ № 23 от 24.02.2016" });
            list0.Add(new Dost { ID = 4, Name = "Приказ № 104 от 04.06.2018" });
            list0.Add(new Dost { ID = 5, Name = "Приказ № 260 от 29.11.2018" });
            this.Prikaz.ItemsSource = list0.OrderByDescending(i => i.ID);
            //список типов для выгрузки по 104 пр.
            list1.Add(new Dost { ID = 1, Name = "Случаи" });
            list1.Add(new Dost { ID = 2, Name = "ЭКМП" });
            this.sluch_ekmp.ItemsSource = list1;
            sluch_ekmp.Visibility = Visibility.Collapsed;
            vibor_slekmp.Visibility = Visibility.Collapsed;
            sluch_ekmp.IsEnabled = false;
        }

        public void Pod_Click(object sender, RoutedEventArgs e)
        {
            
            if (Prikaz.EditValue == null)
            {
                Per1.EditValue = null;
                Per2.EditValue = null;
                MessageBox.Show("Не выбран приказ!");
            }
            else
            {
                var y1 = Per1.DateTime.Year.ToString();
                var y2 = Per2.DateTime.Year.ToString();
                var m1 = Per1.DateTime.Month.ToString();
                var m2 = Per2.DateTime.Month.ToString();
                if (Prikaz.EditValue.ToString() == "3" && y1 != null && y2 != null && m1 != null && m2 != null)
                {
                    Decorator.IsSplashScreenShown = true;
                    var qxml = SqlReader.Select($@"
                exec p_EISSOI_23_Data '{y1}','{y2}','{m1}','{m2}','20190101','0'"
                      , SprClass.LocalConnectionString);
                    MessageBox.Show("Данные успешно подготовлены.");
                    Prikaz.IsEnabled = false;
                    Per1.IsEnabled = false;
                    Per2.IsEnabled = false;
                    Export.IsEnabled = true;
                    date_exp.IsEnabled = true;
                }
               else if (Prikaz.EditValue.ToString() == "4" && y1 != null && m1 != null && m2 != null)
                {
                    Decorator.IsSplashScreenShown = true;
                    var qxml = SqlReader.Select($@"
                exec p_104_Sluch_Data_2 '{y1}','{m1}','{m2}'"
                      , SprClass.LocalConnectionString);
                    MessageBox.Show("Данные успешно подготовлены.");
                    Prikaz.IsEnabled = false;
                    Per1.IsEnabled = false;
                    Per2.IsEnabled = false;
                    Export.IsEnabled = true;
                    date_exp.IsEnabled = true;
                    vibor_slekmp.IsEnabled = true;
                    sluch_ekmp.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show("Заполните данные!");
                }
            }
            Decorator.IsSplashScreenShown = false;
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            
            if (Export.IsEnabled == false && date_exp.IsEnabled == false)
            {
                MessageBox.Show("Данные для экспорта не подготовлены!");
            }
            else
            {
                if (Prikaz.EditValue == null)
                {
                    date_exp.EditValue = null;
                    MessageBox.Show("Не выбран приказ!");
                }
                else
                {
                    var y1 = date_exp.DateTime.Year.ToString();
                    var m1 = date_exp.DateTime.Month.ToString();
                    var np = npp.Text.ToString();
                    if (Prikaz.EditValue.ToString() == "3" && y1 != null && m1 != null && np != "")
                    {
                        var qxml = SqlReader.Select($@"
                exec p_EISSOI_23_Data_XmlExport '{y1}','{m1}','{np}'"
                    , SprClass.LocalConnectionString);
                        string result1 = "<?xml version=\"1.0\" encoding=\"windows-1251\"?>" + (string)qxml[0].GetValue("Column1");
                        XDocument doc = XDocument.Parse(result1);
                        var xname = doc.Element("ISP_OB").Element("ZGLV").Element("FILENAME").Value;
                        Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            SaveFileDialog saveFileDialog = new SaveFileDialog();
                            saveFileDialog.Filter = "XML File (*.xml)|*.xml";
                            saveFileDialog.FileName = xname + ".xml";
                            bool? result = saveFileDialog.ShowDialog();
                            if (result == true)
                            {
                                doc.Save(saveFileDialog.FileName);
                                MessageBox.Show("Файл сохранен");
                            }
                        });
                    }
                      else  if (Prikaz.EditValue.ToString() == "4" && y1 != null && m1 != null)
                        {
                        if (sluch_ekmp.EditValue.ToString() == "1")
                        {
                            var qxml = SqlReader.Select($@"
                        exec p_104_Sluch_xml_3 '{SprClass.Region}','{y1}','{m1}','sluch'"
                        , SprClass.LocalConnectionString);
                            string result1 = "<?xml version=\"1.0\" encoding=\"windows-1251\"?>" + (string)qxml[0].GetValue("Column1");
                            XDocument doc = XDocument.Parse(result1);
                            var xname = doc.Element("MR_OB").Element("ZGLV").Element("FILENAME").Value;
                            Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                SaveFileDialog saveFileDialog = new SaveFileDialog();
                                saveFileDialog.Filter = "XML File (*.xml)|*.xml";
                                saveFileDialog.FileName = xname + ".xml";
                                bool? result = saveFileDialog.ShowDialog();
                                if (result == true)
                                {
                                    doc.Save(saveFileDialog.FileName);
                                    MessageBox.Show("Файл сохранен");
                                }
                            });
                        }
                        else if (sluch_ekmp.EditValue.ToString() == "2")
                        {
                            var qxml = SqlReader.Select($@"
                        exec p_104_Sluch_xml_3 '{SprClass.Region}','{y1}','{m1}','ekmp'"
                        , SprClass.LocalConnectionString);
                            string result1 = "<?xml version=\"1.0\" encoding=\"windows-1251\"?>" + (string)qxml[0].GetValue("Column1");
                            XDocument doc = XDocument.Parse(result1);
                            var xname = doc.Element("MR_OB").Element("ZGLV").Element("FILENAME").Value;
                            Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                SaveFileDialog saveFileDialog = new SaveFileDialog();
                                saveFileDialog.Filter = "XML File (*.xml)|*.xml";
                                saveFileDialog.FileName = xname + ".xml";
                                bool? result = saveFileDialog.ShowDialog();
                                if (result == true)
                                {
                                    doc.Save(saveFileDialog.FileName);
                                    MessageBox.Show("Файл сохранен");
                                }
                            });
                        }
                        }
                        else
                        {
                        MessageBox.Show("Заполните данные!");
                        }

                    }
            }
        }



        private void Prikaz_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (Prikaz.Text.Contains("104"))
            {
                sluch_ekmp.Visibility = Visibility.Visible;
                vibor_slekmp.Visibility = Visibility.Visible;
                npp.IsEnabled = false;
            }
            else
            {
                npp.IsEnabled = true;
                sluch_ekmp.Visibility = Visibility.Collapsed;
                vibor_slekmp.Visibility = Visibility.Collapsed;
            }
        }
    }
}