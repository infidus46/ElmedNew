using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core.ServerMode;
using DevExpress.Xpf.Grid;
using Yamed.Entity;
using Yamed.Server;

namespace Yamed.Emr
{
    /// <summary>
    /// Логика взаимодействия для EmrPacient.xaml
    /// </summary>
    public partial class EmrPacient : GridControl
    {
        public IQueryable PacientQuery = null;
        public readonly LinqInstantFeedbackDataSource LinqInstantFeedbackDataSourcePacient;
        private readonly YamedDataClassesDataContext _elmedDataClassesDataContext;

        public EmrPacient()
        {
            InitializeComponent();

            LinqInstantFeedbackDataSourcePacient = new LinqInstantFeedbackDataSource
            {
                KeyExpression = "ID",
                DefaultSorting = "ID DESC",
                AreSourceRowsThreadSafe = false
            };

            _elmedDataClassesDataContext = new YamedDataClassesDataContext()
            {
                ObjectTrackingEnabled = false,
                Connection = { ConnectionString = SprClass.LocalConnectionString }
            };
        }

        private void EmrPacient_OnUnloaded(object sender, RoutedEventArgs e)
        {
            _elmedDataClassesDataContext.Dispose();
            LinqInstantFeedbackDataSourcePacient.Dispose();
        }

        private void EmrPacient_OnLoaded(object sender, RoutedEventArgs e)
        {
            SmoEdit.DataContext = SprClass.smo;

            if (PacientQuery == null)
                PacientQuery = _elmedDataClassesDataContext.D3_PACIENT_OMS;
            LinqInstantFeedbackDataSourcePacient.QueryableSource = PacientQuery;
            ItemsSource = LinqInstantFeedbackDataSourcePacient.Data;
        }
    }}
