using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.LayoutControl;
using Yamed.Server;

namespace Yamed.Control.Editors
{
    public class FiasStreet
    {
        public Guid ID { get; set; }
        public string NAME { get; set; }
        public decimal LEVEL { get; set; }
    }

    public class FiasHouse
    {
        public string NAME { get; set; }
    }

    public class FiasAddress
    {
        public Guid? Region { get; set; }
        public Guid? Rn { get; set; }
        public Guid? City { get; set; }
        public Guid? CityRn { get; set; }
        public Guid? NasPun { get; set; }
        public Guid? Ul { get; set; }
        public Guid? DopUl { get; set; }
        public string House { get; set; }
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class FiasControl : LayoutGroup
    {
        private readonly string _connectionString; 
        public FiasAddress Address;
        public FiasControl()
        {
            InitializeComponent();

            var conn = new SqlConnectionStringBuilder();
            conn.DataSource = @"91.240.209.114,2866";
            conn.InitialCatalog = "FIAS";
            conn.UserID = "sa";
            conn.Password = "Gbljh:100";

            _connectionString = conn.ConnectionString;
            List<FiasStreet> regionList = new List<FiasStreet>();

            TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext(); //get UI thread context 
            var mekTask = Task.Factory.StartNew(() =>
            {
                try
                {
                    regionList = Reader2List.CustomSelect<FiasStreet>(@"
SELECT AOGUID ID, FORMALNAME + ' - ' + aot.SOCRNAME NAME, AOLEVEL [LEVEL]
  FROM [dbo].[AddressObjects] ao
  join [dbo].[AddressObjectTypes] aot on ao.SHORTNAME = aot.SCNAME and ao.AOLEVEL = aot.[LEVEL]
  where AOLEVEL = 1 AND LIVESTATUS = 1
  order by formalname", _connectionString);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
                Dispatcher.BeginInvoke((Action)delegate ()
                {

                });
            });
            mekTask.ContinueWith(x =>
            {
                RegionBoxEdit.DataContext = regionList;
                RegionBoxEdit.NullText = $"Регион: {regionList.Count}";
            }, uiScheduler);

        }

        private void RnLoader(Guid id)
        {
            var rnList =
                Reader2List.CustomSelect<FiasStreet>(
                    $@"
            SELECT AOGUID ID, FORMALNAME + ' - ' + aot.SOCRNAME NAME, AOLEVEL[LEVEL]
 FROM [dbo].[AddressObjects] ao
join [dbo].[AddressObjectTypes]
        aot on ao.SHORTNAME = aot.SCNAME and ao.AOLEVEL = aot.[LEVEL]
where PARENTGUID = '{
                        id}' AND LIVESTATUS = 1 and AOLEVEL = 3
  order by formalname", _connectionString);
            RnBoxEdit.DataContext = rnList;
            RnBoxEdit.NullText = $"Район: {rnList.Count}";
            RnBoxEdit.IsEnabled = rnList.Any();
        }

        private void CityLoader(Guid id)
        {
            var cityList =
                Reader2List.CustomSelect<FiasStreet>(
                    $@"
            SELECT AOGUID ID, FORMALNAME + ' - ' + aot.SOCRNAME NAME, AOLEVEL[LEVEL]
 FROM [dbo].[AddressObjects] ao
join [dbo].[AddressObjectTypes]
        aot on ao.SHORTNAME = aot.SCNAME and ao.AOLEVEL = aot.[LEVEL]
where PARENTGUID = '{
                        id}' AND LIVESTATUS = 1 and AOLEVEL = 4
  order by formalname", _connectionString);
            CityBoxEdit.DataContext = cityList;
            CityBoxEdit.NullText = $"Город: {cityList.Count}";
            CityBoxEdit.IsEnabled = cityList.Any();

        }

        private void CityRnLoader(Guid id)
        {
            var cityRnList =
                Reader2List.CustomSelect<FiasStreet>(
                    $@"
            SELECT AOGUID ID, FORMALNAME + ' - ' + aot.SOCRNAME NAME, AOLEVEL[LEVEL]
 FROM [dbo].[AddressObjects] ao
join [dbo].[AddressObjectTypes]
        aot on ao.SHORTNAME = aot.SCNAME and ao.AOLEVEL = aot.[LEVEL]
where PARENTGUID = '{
                        id}' AND LIVESTATUS = 1 and AOLEVEL = 5
  order by formalname", _connectionString);
            CityRnBoxEdit.DataContext = cityRnList;
            CityRnBoxEdit.NullText = $"Внутригородской район: {cityRnList.Count}";
            CityRnBoxEdit.IsEnabled = cityRnList.Any();

        }

        private void NasPunktLoader(Guid id)
        {
            var nasPunktnList =
                Reader2List.CustomSelect<FiasStreet>(
                    $@"
            SELECT AOGUID ID, FORMALNAME + ' - ' + aot.SOCRNAME NAME, AOLEVEL[LEVEL]
 FROM [dbo].[AddressObjects] ao
join [dbo].[AddressObjectTypes]
        aot on ao.SHORTNAME = aot.SCNAME and ao.AOLEVEL = aot.[LEVEL]
where PARENTGUID = '{
                        id}' AND LIVESTATUS = 1 and AOLEVEL = 6
  order by formalname", _connectionString);
            NasPunktBoxEdit.DataContext = nasPunktnList;
            NasPunktBoxEdit.NullText = $"Населенный пункт: {nasPunktnList.Count}";
            NasPunktBoxEdit.IsEnabled = nasPunktnList.Any();
        }

        private void UlLoader(Guid id)
        {
            var ulList =Reader2List.CustomSelect<FiasStreet>(
                    $@"
            SELECT AOGUID ID, FORMALNAME + ' - ' + aot.SOCRNAME NAME, AOLEVEL[LEVEL]
 FROM [dbo].[AddressObjects] ao
join [dbo].[AddressObjectTypes]
        aot on ao.SHORTNAME = aot.SCNAME and ao.AOLEVEL = aot.[LEVEL]
where PARENTGUID = '{
                        id}' AND LIVESTATUS = 1 and AOLEVEL = 7
  order by formalname", _connectionString);
            UlBoxEdit.DataContext = ulList;
            UlBoxEdit.NullText = $"Улица: {ulList.Count}";
            UlBoxEdit.IsEnabled = ulList.Any();

        }

        private void DopUlLoader(Guid id)
        {
            var dopUlList =
                Reader2List.CustomSelect<FiasStreet>(
                    $@"
            SELECT AOGUID ID, FORMALNAME + ' - ' + aot.SOCRNAME NAME, AOLEVEL[LEVEL]
 FROM [dbo].[AddressObjects] ao
join [dbo].[AddressObjectTypes]
        aot on ao.SHORTNAME = aot.SCNAME and ao.AOLEVEL = aot.[LEVEL]
where PARENTGUID = '{
                        id}' AND LIVESTATUS = 1 and AOLEVEL in (90,91)
  order by formalname", _connectionString);
            DopUlBoxEdit.DataContext = dopUlList;
            DopUlBoxEdit.NullText = $"Доп. адресообразующий элемент: {dopUlList.Count}";
            DopUlBoxEdit.IsEnabled = dopUlList.Any();
        }

        private void HouseLoader(Guid id)
        {
            var houseList =
                Reader2List.CustomSelect<FiasHouse>(
                    $@"
DECLARE @tt TABLE
(
NAME NVARCHAR(20)
)
DECLARE @start INT
DECLARE @end INT
DECLARE @status INT

DECLARE vendor_cursor CURSOR FOR 
SELECT [INTSTART]
      ,[INTEND]      
      ,[INTSTATUS]      
FROM [dbo].[HouseIntervals]
WHERE aoguid = '{id}'

OPEN vendor_cursor
FETCH NEXT FROM vendor_cursor INTO @start, @end, @status;
WHILE @@FETCH_STATUS = 0
BEGIN
	
	WHILE @start <= @end
	BEGIN
		IF (@status = 1) OR (@status = 2)
		BEGIN
			INSERT INTO @tt (NAME) VALUES ('д. ' + CONVERT(NVARCHAR(20), @start));
			SET @start = @start + 2;
		END ELSE IF (@status = 3)
		BEGIN
			INSERT INTO @tt (NAME) VALUES ('д. ' + CONVERT(NVARCHAR(20), @start));
			SET @start = @start + 1;
		END;	
	END;	
	
	FETCH NEXT FROM vendor_cursor INTO @start, @end, @status;
END;
CLOSE vendor_cursor;
DEALLOCATE vendor_cursor;

INSERT INTO @tt (NAME)
SELECT ISNULL('д. ' + housenum, '') + ISNULL('  к. ''' + BUILDNUM + '''', '') + ISNULL('  стр. ''' + STRUCNUM  + '''', '')
FROM [dbo].[Houses]
WHERE aoguid='{id}'

SELECT *
FROM @tt
GROUP BY NAME", _connectionString);
            HouseBoxEdit.DataContext = houseList;
            HouseBoxEdit.NullText = $"Дом: {houseList.Count}";
            HouseBoxEdit.IsEnabled = houseList.Any();
        }

        private void RegionBoxEdit_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            var value = ((BaseEdit)sender).EditValue;
            if (value != null)
            {
                RnBoxEdit.EditValue = null;
                CityBoxEdit.EditValue = null;
                CityRnBoxEdit.EditValue = null;
                NasPunktBoxEdit.EditValue = null;
                UlBoxEdit.EditValue = null;
                DopUlBoxEdit.EditValue = null;
                HouseBoxEdit.EditValue = null;


                RnLoader((Guid) value);
                CityLoader((Guid) value);
                CityRnLoader((Guid) value);
                NasPunktLoader((Guid) value);
                UlLoader((Guid) value);
                DopUlLoader((Guid)value);
                HouseLoader((Guid)value);
            }
            else
            {
                RnBoxEdit.EditValue = null;
                RnBoxEdit.DataContext = null;
                RnBoxEdit.NullText = $"Район: {0}";
                CityBoxEdit.EditValue = null;
                CityBoxEdit.DataContext = null;
                CityBoxEdit.NullText = $"Город: {0}";
                CityRnBoxEdit.EditValue = null;
                CityRnBoxEdit.DataContext = null;
                CityRnBoxEdit.NullText = $"Внутригородской район: {0}";
                NasPunktBoxEdit.EditValue = null;
                NasPunktBoxEdit.DataContext = null;
                NasPunktBoxEdit.NullText = $"Населенный пункт: {0}";
                UlBoxEdit.EditValue = null;
                UlBoxEdit.DataContext = null;
                UlBoxEdit.NullText = $"Улица: {0}";
                DopUlBoxEdit.EditValue = null;
                DopUlBoxEdit.DataContext = null;
                DopUlBoxEdit.NullText = $"Доп. адресообразующий элемент: {0}";
                HouseBoxEdit.EditValue = null;
                HouseBoxEdit.DataContext = null;
                HouseBoxEdit.NullText = $"Дом: {0}";


            }
        }

        private void RnBoxEdit_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            var value = ((BaseEdit) sender).EditValue;
            if (value != null)
            {
                CityBoxEdit.EditValue = null;
                CityRnBoxEdit.EditValue = null;
                NasPunktBoxEdit.EditValue = null;
                UlBoxEdit.EditValue = null;
                DopUlBoxEdit.EditValue = null;
                HouseBoxEdit.EditValue = null;


                CityLoader((Guid)value);
                CityRnLoader((Guid)value);
                NasPunktLoader((Guid)value);
                UlLoader((Guid)value);
                DopUlLoader((Guid)value);
                HouseLoader((Guid)value);
            }
            else
            {
                CityBoxEdit.EditValue = null;
                CityBoxEdit.DataContext = null;
                CityBoxEdit.NullText = $"Город: {0}";
                CityRnBoxEdit.EditValue = null;
                CityRnBoxEdit.DataContext = null;
                CityRnBoxEdit.NullText = $"Внутригородской район: {0}";
                NasPunktBoxEdit.EditValue = null;
                NasPunktBoxEdit.DataContext = null;
                NasPunktBoxEdit.NullText = $"Населенный пункт: {0}";
                UlBoxEdit.EditValue = null;
                UlBoxEdit.DataContext = null;
                UlBoxEdit.NullText = $"Улица: {0}";
                DopUlBoxEdit.EditValue = null;
                DopUlBoxEdit.DataContext = null;
                DopUlBoxEdit.NullText = $"Доп. адресообразующий элемент: {0}";
                HouseBoxEdit.EditValue = null;
                HouseBoxEdit.DataContext = null;
                HouseBoxEdit.NullText = $"Дом: {0}";

            }
        }

        private void CityBoxEdit_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            var value = ((BaseEdit)sender).EditValue;
            if (value != null)
            {
                CityRnBoxEdit.EditValue = null;
                NasPunktBoxEdit.EditValue = null;
                UlBoxEdit.EditValue = null;
                DopUlBoxEdit.EditValue = null;
                HouseBoxEdit.EditValue = null;

                CityRnLoader((Guid)value);
                NasPunktLoader((Guid)value);
                UlLoader((Guid)value);
                DopUlLoader((Guid)value);
                HouseLoader((Guid)value);
            }
            else
            {
                CityRnBoxEdit.EditValue = null;
                CityRnBoxEdit.DataContext = null;
                CityRnBoxEdit.NullText = $"Внутригородской район: {0}";
                NasPunktBoxEdit.EditValue = null;
                NasPunktBoxEdit.DataContext = null;
                NasPunktBoxEdit.NullText = $"Населенный пункт: {0}";
                UlBoxEdit.EditValue = null;
                UlBoxEdit.DataContext = null;
                UlBoxEdit.NullText = $"Улица: {0}";
                DopUlBoxEdit.EditValue = null;
                DopUlBoxEdit.DataContext = null;
                DopUlBoxEdit.NullText = $"Доп. адресообразующий элемент: {0}";
                HouseBoxEdit.EditValue = null;
                HouseBoxEdit.DataContext = null;
                HouseBoxEdit.NullText = $"Дом: {0}";

            }
        }

        private void CityRnBoxEdit_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            var value = ((BaseEdit)sender).EditValue;
            if (value != null)
            {
                NasPunktBoxEdit.EditValue = null;
                UlBoxEdit.EditValue = null;
                DopUlBoxEdit.EditValue = null;
                HouseBoxEdit.EditValue = null;


                NasPunktLoader((Guid)value);
                UlLoader((Guid)value);
                DopUlLoader((Guid)value);
                HouseLoader((Guid)value);
            }
            else
            {
                NasPunktBoxEdit.EditValue = null;
                NasPunktBoxEdit.DataContext = null;
                NasPunktBoxEdit.NullText = $"Населенный пункт: {0}";
                UlBoxEdit.EditValue = null;
                UlBoxEdit.DataContext = null;
                UlBoxEdit.NullText = $"Улица: {0}";
                DopUlBoxEdit.EditValue = null;
                DopUlBoxEdit.DataContext = null;
                DopUlBoxEdit.NullText = $"Доп. адресообразующий элемент: {0}";
                HouseBoxEdit.EditValue = null;
                HouseBoxEdit.DataContext = null;
                HouseBoxEdit.NullText = $"Дом: {0}";

            }
        }

        private void NasPunktBoxEdit_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            var value = ((BaseEdit)sender).EditValue;
            if (value != null)
            {
                UlBoxEdit.EditValue = null;
                DopUlBoxEdit.EditValue = null;
                HouseBoxEdit.EditValue = null;

                UlLoader((Guid)value);
                DopUlLoader((Guid)value);
                HouseLoader((Guid)value);
            }
            else
            {
                UlBoxEdit.EditValue = null;
                UlBoxEdit.DataContext = null;
                UlBoxEdit.NullText = $"Улица: {0}";
                DopUlBoxEdit.EditValue = null;
                DopUlBoxEdit.DataContext = null;
                DopUlBoxEdit.NullText = $"Доп. адресообразующий элемент: {0}";
                HouseBoxEdit.EditValue = null;
                HouseBoxEdit.DataContext = null;
                HouseBoxEdit.NullText = $"Дом: {0}";

            }
        }

        private void UlBoxEdit_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            var value = ((BaseEdit)sender).EditValue;
            if (value != null)
            {
                DopUlBoxEdit.EditValue = null;
                HouseBoxEdit.EditValue = null;
                HouseLoader((Guid)value);
            }
            else
            {
                HouseBoxEdit.EditValue = null;
                HouseBoxEdit.DataContext = null;
                HouseBoxEdit.NullText = $"Дом: {0}";
            }
            var test = Address;

        }

        private void DopUlBoxEdit_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            var value = ((BaseEdit)sender).EditValue;
            if (value != null)
            {
                UlBoxEdit.EditValue = null;
                HouseBoxEdit.EditValue = null;
                HouseLoader((Guid)value);
            }
            else
            {
                HouseBoxEdit.EditValue = null;
                HouseBoxEdit.DataContext = null;
                HouseBoxEdit.NullText = $"Дом: {0}";
            }
        }
    }
}
