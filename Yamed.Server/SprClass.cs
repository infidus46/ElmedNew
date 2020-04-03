using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ActiveQueryBuilder.Core;
using ActiveQueryBuilder.View.WPF;
using Yamed.Core;
using Yamed.Entity;

namespace Yamed.Server
{
    public class DISPSPR
    {
        public string IDDISP { get; set; }
        public string NAME { get; set; }
    }

    public class PolisCheck
    {
        public string Fam { get; set; }
        public string Im { get; set; }
        public string Ot { get; set; }
        public DateTime? Dr { get; set; }
        public string Polis { get; set; }
        public DateTime? Dbeg { get; set; }
        public DateTime? Dend { get; set; }
        public DateTime? Dstop { get; set; }
        public string Q { get; set; }
        public string H_Polis { get; set; }
        public DateTime? H_Dbeg { get; set; }
        public DateTime? H_Dend { get; set; }
        public DateTime? H_Dstop { get; set; }
        public string H_Q { get; set; }
        public int Opdoc { get; set; }
        public int Opdoc2 { get; set; }
        public string ENP { get; set; }
        public int Id { get; set; }
        public DateTime? Ds { get; set; }
    }

    public class ProductSettings
    {
        public OrgType OrgTypeStatus { get; set; }
        public string OrgNaim { get; set; }
        public string Region { get; set; }
        public string OrgCode { get; set; }
        public string OrgGuid { get; set; }
        public string ProductKey { get; set; }
    }

    public class SprClass
    {
        public static List<Settings> DbSettings;
        private static OrgType GetOrgType()
        {
            //var tfoms = Reader2List.CustomSelect<Settings>("Select * from Settings where Name = 'TFOMS'",
            //    SprClass.LocalConnectionString).SingleOrDefault()?.Parametr;
            var tfoms = DbSettings.SingleOrDefault(x => x.Name == "TFOMS")?.Parametr;

            switch (tfoms)
            {
                case null:
                    return OrgType.Lpu;
                case "False":
                    return OrgType.Smo;
                case "True":
                    return OrgType.Tfoms;
                default:
                    return OrgType.Test;
            }
        }

        private static string GetOrgCode()
        {
            return DbSettings.SingleOrDefault(x => x.Name == "OrgCode")?.Parametr;
        }

        private static string GetRegiobCode()
        {
            return Region = Region = DbSettings.SingleOrDefault(x => x.Name == "REGION") == null
                ? "46"
                : DbSettings.SingleOrDefault(x => x.Name == "REGION").Parametr;

        }

        public static ProductSettings ProdSett;
        public static bool FlcVerify;
        public static string Region;

        public static QueryBuilder Qb = new QueryBuilder();
        /// <summary>
        /// Определитель статуса пн (какая вкладка открыта)
        /// </summary>
        public static int DeterminantX_Status;
        /// <summary>
        /// Панель вывода информации
        /// </summary>
        public static object outputUserControl; //OutputUserControl
        public static object attPrimary;
        public static string RegNas;

        public static string LocalConnectionString;
        public static string SrzConnectionString;

        //public static string GlobalElmedOnLineConnectionString;
        public static string GlobalDocExchangeConnectionString;
        public static string GlobalSrzConnectionString;

        public static int userId;
        public static string UserFIO;
        public static DateTime WorkDate;
        public static bool IsTfoms;
        public static string schetQuery;
        public static string schetQueryWithSank;
        //public static List<CelVisitDb> celVisit;
        //public static List<ResultVisitDb> resultVisit;
        //public static List<TipBoleznDb> tipBolezn;
        public static List<SpecCodeDb> specCodeDb;
        //public static List<DoctorBd> doctorBd;
        public static List<F011> passport;
        public static List<F010> smoOkato;
        public static List<F010> terOkato;
        public static List<F002> smo;
        public static List<F003> LpuList;
        //public static List<Ishod> ishod;
        public static List<F008> typePolicy;
        public static List<V009> helpResult;
        public static List<V012> helpExit;
        public static List<V008> typeHelp;
        public static List<V006> conditionHelp;
        public static List<V002> profile;
        public static List<V004> speciality;
        public static List<V015> specialityNew;
        public static List<TypeSluchDb> typeSluch;
        public static List<M001_KSG> mkbSearching;
        public static List<String> mkbSearching2;
        public static List<V005> sex;
        public static List<F008> policyType;
        public static List<DetProfilDb> detProf;
        public static List<V010> tarifUsl;
        public static List<Nomenclature> S1664Ns;
        //public static List<UslugiShablon> Shablons;
        public static List<OtdelDb> OtdelDbs;
        //public static List<ProfilTOSpecCode> ProfilToSpecCodes;
        public static List<F005> TypeOplaty;
        public static List<F006> TypeExp;
        public static List<F014> Otkazs;

        public static List<V018> VidVmpList;
        public static List<V019> MetodVmpList;

        public static List<GrZdorovDb> GrZdorovDbs;
        public static List<VeteranDb> VeteranDbs;
        public static List<SchoolStatusDb> SchoolStatusDbs;
        public static List<WorkStatDb> WorkStatDbs;

        //public static List<PolisCheck> PolChList;
        public static List<PodrDb> Podr;
        public static List<KSG_GROUP> KsgGroups;
        public static List<KSG_OT> KsgOts;

        public static List<MEE_TYPE_DB> MeeTypeDbs;
        public static List<EKMP_TYPE_DB> EkpmTypeDbs;
        public static List<ExpertsDB> ExpertDbs;
        public static List<DISPSPR> DispSprs;
        //public static IEnumerator<PolisCheck> PolChList1;
        public static List<F003> medOrg;

        public static List<KodTer> KodTers;
        public static List<SocStat> SocStats;
        public static List<KatLgot> KatLgots;

        public static List<Gospit> Gospits;
        public static List<Dostav> Dostavs;
        public static List<TravmaTbl> TravmaTbls;

        public static List<SprExtr> ExtrDbs;

        public static object KDostList;
        public static object VOplList;
        public static object DoctList;
        public static object DefGospList;
        public static object OslList;
        public static object AnestList;
        public static List<DISP_SPR> DispSprList;
        public static object YesNoList;
        public static object Payment;
        public static object KsgOplata;

        public static List<SprDSGR> SprDsgrList;
        public static List<SprKSGR> SprKsgrList;
        public static object CalcKsgTarifList;
        public static object VozrList;
        public static object DostList;
        public static object ForPomList;
        public static object MedicalEmployeeList;
        public static object SprDetProfilList;
        public static object SprPCelList;
        public static object SprUsl804;
        public static object SprUslCode;
        public static object SprBit;
        public static object SprNpl;
        public static object Spr79_F005;
        public static object YamedUsers;
        public static object KslpList;
        public static object DnList;
        public static object SpecV021List;
        public static object SpecAllList;
        public static object Profil_V020;
        public static object OsobSluchDbs;
        //public static object OsSlRegion;

        public static object TypeExp2;

        public static object N001;
        public static object N002;
        public static object N003;
        public static object N004;
        public static object N005;
        public static object N006;
        public static object N007;
        public static object N008;
        public static object N009;
        public static object N010;
        public static object N011;
        public static object N012;
        public static object N013;
        public static object N014;
        public static object N015;
        public static object N016;
        public static object N017;
        public static object N018;
        public static object N019;
        public static object N020;

        public static object N00_DiagTip;
        public static object N007_010;
        public static object N008_011;

        public static object V017;
        public static object V011;
        public static object V024;

        public static object V026;
        public static object V027;
        public static object V028;
        public static object V029;

        public static object NAZR;

        public static object SchetType;
        public static object DsType;
        public static object Per;
        public static object PrNov;

        public static object SocStatsnew;
        public static object INV;

        public static object sankname;
        public static object sankMek;

        public static object rg004; // класс для справочника rg004 Иваново
        public static object rg003;// класс для справочника rg003 Иваново
        public static object rg001; // класс для справочника rg001 Иваново
        public static object rg012; // класс для справочника rg012 Иваново
        public static object SprVizov; //класс для справочника Вид вызова Иваново
        public static object SprBrigad; //класс для справочника Вид бригады Иваново
        public static object SprGrafdn; //класс для справочника график дн. стац. Иваново

        public static object SprTypeMp; //тип МП в Акте экспертизы

        public static void SprLoad()
        {

            DbSettings = Reader2List.CustomSelect<Settings>("Select * From Settings", LocalConnectionString);

            ProdSett = new ProductSettings()
            {
                OrgTypeStatus = GetOrgType(),
                OrgCode = GetOrgCode(),
                Region = GetRegiobCode()

            };

            var today = DateTime.Today.ToString("yyyyMMdd");
            

            FlcVerify = DbSettings.SingleOrDefault(x => x.Name == "FLC") == null
                ? false
                : bool.Parse(DbSettings.SingleOrDefault(x => x.Name == "FLC").Parametr);

            Region = GetRegiobCode();

            //KDostList = Reader2List.GetAnonymousTable("sluch", LocalConnectionString);
            KDostList = Reader2List.GetAnonymousTable("SprKDOST", LocalConnectionString);
            //var test = ((IList) KDostList)[0];
            //(test as INotifyPropertyChanged).PropertyChanged +=
            //    (o, e) => System.Diagnostics.Debug.WriteLine(
            //    "property changed: " + e.PropertyName);
            //test.GetType().GetProperty("ID").SetValue(test, 5, null);
            //test.GetType().GetProperty("Name").SetValue(test, "testtest", null);

            VOplList = Reader2List.GetAnonymousTable("SprVOPL", LocalConnectionString);
            DoctList = Reader2List.GetAnonymousTable("DoctorBd", LocalConnectionString);
            DefGospList = Reader2List.GetAnonymousTable("SprDEFGOSP", LocalConnectionString);
            OslList = Reader2List.GetAnonymousTable("SprOSL", LocalConnectionString);
            AnestList = Reader2List.GetAnonymousTable("SprANEST", LocalConnectionString);
            YesNoList = Reader2List.GetAnonymousTable("YesNoTbl", LocalConnectionString);
            //Payment = Reader2List.GetAnonymousTable("Yamed_Spr_Payment", LocalConnectionString);
            Payment = Reader2List.CustomAnonymousSelect("Select * from Yamed_Spr_Payment where OrgType in (1,2)", LocalConnectionString);
            KsgOplata = Reader2List.GetAnonymousTable("SprKSGOPLATA", LocalConnectionString);
            VozrList = Reader2List.GetAnonymousTable("SprVozr", LocalConnectionString);
            DostList = Reader2List.GetAnonymousTable("SprDost", LocalConnectionString);
            ForPomList = Reader2List.GetAnonymousTable("V014", LocalConnectionString);
            MedicalEmployeeList = Reader2List.CustomAnonymousSelect("Select * from View_Yamed_Spr_MedicalEmployee order by NameWithID", LocalConnectionString);
            SprDetProfilList = Reader2List.GetAnonymousTable("SprDetProfil", LocalConnectionString);
            SprPCelList = Reader2List.GetAnonymousTable("V025", LocalConnectionString);
            SprUsl804 = Reader2List.GetAnonymousTable("Yamed_Spr_Usl804", LocalConnectionString);
            SprUslCode = Reader2List.GetAnonymousTable("Yamed_Spr_UslCode", LocalConnectionString);
            SprBit = Reader2List.GetAnonymousTable("D3_Spr_BIT", LocalConnectionString);
            SprNpl = Reader2List.GetAnonymousTable("D3_Spr_NPL", LocalConnectionString);
            Spr79_F005 = Reader2List.GetAnonymousTable("SPR79_F005", LocalConnectionString);
            YamedUsers = Reader2List.GetAnonymousTable("Yamed_Users", LocalConnectionString);
            KslpList = Reader2List.CustomAnonymousSelect("Select * from SprKSLP where DATEBEG >='20190101'", LocalConnectionString);
            DnList = Reader2List.CustomAnonymousSelect("Select * from Yamed_Spr_Dn", LocalConnectionString);
            SpecV021List = Reader2List.CustomAnonymousSelect("Select * from V021", LocalConnectionString);
            SpecAllList = Reader2List.CustomAnonymousSelect("Select * from View_Spr_SPEC", LocalConnectionString);
            Profil_V020 = Reader2List.GetAnonymousTable("V020", LocalConnectionString);

            OsobSluchDbs = Reader2List.CustomAnonymousSelect($"Select * from OsobSluchDb", LocalConnectionString);
            //OsSlRegion = Reader2List.CustomAnonymousSelect($"Select * from OsobSluchDb where dbeg <= '{today}' and isnull(DEND, '20991231') >= '{today}'", LocalConnectionString);
            TypeExp2 = Reader2List.CustomAnonymousSelect("SELECT * FROM [F006_NEW] where dateend is null ", LocalConnectionString);
            N001 = Reader2List.CustomAnonymousSelect($"Select * from N001 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);
            N002 = Reader2List.CustomAnonymousSelect($"Select * from N002 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);
            N003 = Reader2List.CustomAnonymousSelect($"Select * from N003 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);
            N004 = Reader2List.CustomAnonymousSelect($"Select * from N004 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);
            N005 = Reader2List.CustomAnonymousSelect($"Select * from N005 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);
            N006 = Reader2List.CustomAnonymousSelect($"Select * from N006 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);
            N007 = Reader2List.CustomAnonymousSelect($"Select * from N007 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);
            N008 = Reader2List.CustomAnonymousSelect($"Select * from N008 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);
            N009 = Reader2List.CustomAnonymousSelect($"Select * from N009 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);
            N010 = Reader2List.CustomAnonymousSelect($"Select * from N010 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);
            N011 = Reader2List.CustomAnonymousSelect($"Select * from N011 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);
            N012 = Reader2List.CustomAnonymousSelect($"Select * from N012 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);
            N013 = Reader2List.CustomAnonymousSelect($"Select * from N013 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);
            N014 = Reader2List.CustomAnonymousSelect($"Select * from N014 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);
            N015 = Reader2List.CustomAnonymousSelect($"Select * from N015 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);
            N016 = Reader2List.CustomAnonymousSelect($"Select * from N016 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);
            N017 = Reader2List.CustomAnonymousSelect($"Select * from N017 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);
            N018 = Reader2List.CustomAnonymousSelect($"Select * from N018 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);
            N019 = Reader2List.CustomAnonymousSelect($"Select * from N019 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);
            N020 = Reader2List.CustomAnonymousSelect($"Select * from N020 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);

            N00_DiagTip = Reader2List.CustomAnonymousSelect($"Select * from N00_DiagTip", LocalConnectionString);
            N007_010 = Reader2List.CustomAnonymousSelect($"Select * from N007_010 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);
            N008_011 = Reader2List.CustomAnonymousSelect($"Select * from N008_011 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);

            V017 = Reader2List.CustomAnonymousSelect($"Select * from V017 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);
            //V011 = Reader2List.CustomAnonymousSelect($"Select * from V011 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);
            V024 = Reader2List.CustomAnonymousSelect($"Select * from V024 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);
            V026 = Reader2List.CustomAnonymousSelect($"Select * from V026 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);
            V027 = Reader2List.CustomAnonymousSelect($"Select * from V027 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);
            V028 = Reader2List.CustomAnonymousSelect($"Select * from V028 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);
            V029 = Reader2List.CustomAnonymousSelect($"Select * from V029 where datebeg <= '{today}' and isnull(DATEEND, '20991231') >= '{today}'", LocalConnectionString);

            NAZR = Reader2List.GetAnonymousTable("Yamed_Spr_NAZR", LocalConnectionString);

            SchetType = Reader2List.GetAnonymousTable("Yamed_Spr_SchetType", LocalConnectionString);
            DsType = Reader2List.GetAnonymousTable("Yamed_Spr_DsType", LocalConnectionString);
            Per = Reader2List.GetAnonymousTable("Yamed_Spr_Per", LocalConnectionString);
            PrNov = Reader2List.GetAnonymousTable("Yamed_Spr_PrNov", LocalConnectionString);

            CalcKsgTarifList = Reader2List.GetAnonymousTable("SprKsg", LocalConnectionString);

            INV = Reader2List.CustomAnonymousSelect($"Select * from SprINV", LocalConnectionString);
            //Справочники для работы полей Иваново, Андрей Insidious

            SocStatsnew = Reader2List.CustomAnonymousSelect($"Select * from SocStat", LocalConnectionString);
            rg001 = Reader2List.CustomAnonymousSelect($"Select * from rg001", LocalConnectionString);
            rg003 = Reader2List.CustomAnonymousSelect($"Select * from rg003", LocalConnectionString);
            rg004 = Reader2List.CustomAnonymousSelect($"Select * from rg004", LocalConnectionString);
            rg012 = Reader2List.CustomAnonymousSelect($"Select * from rg012", LocalConnectionString);
            SprVizov = Reader2List.CustomAnonymousSelect($"Select * from SprVizov", LocalConnectionString);
            SprBrigad = Reader2List.CustomAnonymousSelect($"Select * from SprBrigad", LocalConnectionString);
            SprGrafdn = Reader2List.CustomAnonymousSelect($"Select * from SprGrafdn", LocalConnectionString);

            SprTypeMp = Reader2List.CustomAnonymousSelect($"Select * from Yamed_Spr_TypeMP", LocalConnectionString);



            using (ElmedDataClassesDataContext elMed = new ElmedDataClassesDataContext(LocalConnectionString))
            {
        
                //SprDsgrList = elMed.GetTable<SprDSGR>().ToList();
                //SprKsgrList = elMed.GetTable<SprKSGR>().ToList();



                //IsTfoms = bool.Parse(elMed.Settings.SingleOrDefault(x => x.Name == "TFOMS")?.Parametr);
                medOrg = elMed.GetTable<F003>().ToList();

                KodTers = elMed.GetTable<KodTer>().ToList();
                SocStats = elMed.GetTable<SocStat>().ToList();
                KatLgots = elMed.GetTable<KatLgot>().ToList();
                
                Gospits = elMed.GetTable<Gospit>().ToList();
                Dostavs = elMed.GetTable<Dostav>().ToList();
                TravmaTbls = elMed.GetTable<TravmaTbl>().ToList();

                ExtrDbs = elMed.GetTable<SprExtr>().ToList();

                passport = elMed.GetTable<F011>().ToList();
                smoOkato = elMed.GetTable<F010>().ToList();
                terOkato = elMed.GetTable<F010>().ToList();
                smo = elMed.GetTable<F002>().ToList();
                LpuList = elMed.GetTable<F003>().ToList();
                //ishod = elMed.GetTable<Ishod>().ToList();
                typePolicy = elMed.GetTable<F008>().ToList();
                helpResult = elMed.GetTable<V009>().ToList();
                helpExit = elMed.GetTable<V012>().ToList();
                typeHelp = elMed.GetTable<V008>().ToList();
                conditionHelp = elMed.GetTable<V006>().ToList();
                profile = elMed.GetTable<V002>().ToList();
                speciality = elMed.GetTable<V004>().ToList();
                specialityNew = elMed.GetTable<V015>().ToList();
                typeSluch = elMed.GetTable<TypeSluchDb>().ToList();
                sex = elMed.GetTable<V005>().ToList();
                policyType = elMed.GetTable<F008>().ToList();
                detProf = elMed.GetTable<DetProfilDb>().ToList();
                tarifUsl = elMed.GetTable<V010>().ToList();
                S1664Ns = elMed.GetTable<Nomenclature>().ToList();
                //Shablons = null;
                mkbSearching = elMed.GetTable<M001_KSG>().OrderBy(x=>x.IDDS).ToList();
                mkbSearching2 = mkbSearching.Select(x => x.IDDS).OrderBy(s => s).ToList();
                OtdelDbs = elMed.GetTable<OtdelDb>().ToList();
                //ProfilToSpecCodes = elMed.GetTable<ProfilTOSpecCode>().ToList();
                TypeOplaty = elMed.GetTable<F005>().ToList();
                TypeExp = elMed.GetTable<F006>().ToList();
                Otkazs = elMed.GetTable<F014>().OrderBy( x=> x.Osn).ToList();
                VidVmpList = elMed.V018.ToList();
                MetodVmpList = elMed.V019.ToList();

                GrZdorovDbs = elMed.GetTable<GrZdorovDb>().ToList();
                VeteranDbs = elMed.GetTable<VeteranDb>().ToList();
                SchoolStatusDbs = elMed.GetTable<SchoolStatusDb>().ToList();
                WorkStatDbs = elMed.GetTable<WorkStatDb>().ToList();
                Podr = elMed.GetTable<PodrDb>().ToList();
                KsgGroups = elMed.GetTable<KSG_GROUP>().ToList();
                KsgOts = elMed.GetTable<KSG_OT>().ToList();

                MeeTypeDbs = elMed.GetTable<MEE_TYPE_DB>().ToList();
                EkpmTypeDbs = elMed.GetTable<EKMP_TYPE_DB>().ToList();
                ExpertDbs = elMed.GetTable<ExpertsDB>().ToList();
                DispSprs = (from x in elMed.DISP_SPR
                    select new DISPSPR
                    {
                        IDDISP = x.IDDISP,
                        NAME = x.NAME
                    }).Distinct().ToList();

                var directIpOrg = new[] {/*"460039",*/ "46003", "460064"};

                //var conn = new SqlConnectionStringBuilder();
                //conn.DataSource = directIpOrg.Contains(ProdSett.OrgCode)? @"91.240.209.114" : @"91.240.209.114,2866";
                //conn.InitialCatalog = "ELMED_ONLINE_NEW";
                //conn.UserID = "sa";
                //conn.Password = "Gbljh:100";
                //GlobalElmedOnLineConnectionString = conn.ConnectionString;

                var conatt = new SqlConnectionStringBuilder();
                conatt.DataSource = directIpOrg.Contains(ProdSett.OrgCode) ? @"91.240.209.114" : @"91.240.209.114,2866";
                //conatt.DataSource = @"91.240.209.114";
                conatt.InitialCatalog = "DocExchange";
                conatt.UserID = "AttpSmo";
                conatt.Password = @"VjuByjYt;Xby";
                GlobalDocExchangeConnectionString = conatt.ConnectionString;

                var srzcon = new SqlConnectionStringBuilder();
                srzcon.DataSource = @"91.240.209.114,19450";
                srzcon.ConnectTimeout = 2;
                srzcon.InitialCatalog = "srz_mini";
                srzcon.UserID = "mo";
                srzcon.Password = @"54Vtkrb[Gfrjcnys[J,zpmzyGjbvtkbDct[@Ufdy.rjd$";
                GlobalSrzConnectionString = srzcon.ConnectionString;


                schetQuery = @"Select sl.[ID]
      ,[IDCASE]
      ,[USL_OK]
      ,[VIDPOM]
      ,[NPR_MO]
      ,[EXTR]
      ,[LPU]
      ,[LPU_1]
      ,[PODR]
      ,[PROFIL]
      ,[DET]
      ,[NHISTORY]
      ,[DATE_1]
      ,[DATE_2]
      ,[DS0]
      ,[DS1]
      ,[DS2]
      ,[CODE_MES1]
      ,[CODE_MES2]
      ,[SLUCH_TYPE]
      ,[RSLT]
      ,[ISHOD]
      ,[PRVS]
      ,[IDDOKT]
      ,[OS_SLUCH]
      ,[OS_SLUCH_REGION]
      ,[IDSP]
      ,[ED_COL]
      ,[TARIF]
      ,[SUMV]
      ,[OPLATA]
      ,[SUMP]
      ,[SANK_IT]
      ,[COMENTSL]
      ,sl.[SCHET_ID]
      ,[GR_ZDOROV]
      ,[VETERAN]
      ,[SCHOOL]
      ,[WORK_STAT]
      ,[PID]
      ,[ID_TEMP]
      ,[PID_TEMP]
      ,[IDKSG]
      ,[MEK_COMENT]
      ,[MEE_COMENT]
      ,[EKMP_COMENT]
      ,[REMEK_COM]
      ,[MEE_TYPE]
      ,[KSG_OPLATA]
      ,[KDAY]
      ,[REQUEST_DATE]
      ,[USERID] 
      ,'*   '+ ISNULL([FAM]+' ','')+ISNULL([IM]+' ','')+ISNULL([OT]+' ', '')+ISNULL('('+CONVERT(NVARCHAR(10), [DR], 4)+')', '') AS FIODR
      ,[FAM]
      ,[IM]
      ,[OT]
      ,[W]
      ,[DR]
      ,[DOST]
      ,[FAM_P]
      ,[IM_P]
      ,[OT_P]
      ,[W_P]
      ,[DR_P]
      ,[MR]
      ,[DOCTYPE]
      ,[DOCSER]
      ,[DOCNUM]
      ,[SNILS]
      ,[OKATOG]
      ,[OKATOP]
      ,[COMENTP]
      ,[VPOLIS]
      ,[SPOLIS]
      ,[NPOLIS]
      ,[SMO]
      ,[SMO_OGRN]
      ,[SMO_OK]
      ,[SMO_NAM]
      ,[NOVOR]
      ,[MSPID]
      ,[VID_HMP]
      ,[METOD_HMP]
      ,[DIFF_K]
      ,MEK_COUNT
      ,MEE_COUNT
      ,EKMP_COUNT
      ,RMEK_COUNT
      ,RMEE_COUNT
      ,REKMP_COUNT
      ,KSG_COM
      ,KDAY_COM
      ,DIFF_COM

from sluch sl
JOIN PACIENT AS p ON  p.ID = sl.PID ";

            }
        }
    }
}
