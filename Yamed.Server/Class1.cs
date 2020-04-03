using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Yamed.Core;
using Yamed.Entity;

namespace Yamed.Server
{
    public static class Class1
    {
        static void test()
        {
            //        var tfoms = Reader2List.CustomSelect<Settings>("Select * from Settings where Name = 'TFOMS'",
            //SprClass.LocalConnectionString).SingleOrDefault()?.Parametr;

            //        //var isTfoms = tfoms != null ? bool.Parse(tfoms) : false;

            //        if (tfoms == null)
            //        {
            //            using (ElmedDataClassesDataContext dc = new ElmedDataClassesDataContext(SprClass.LocalConnectionString))
            //            {
            //                string[] badMo = { "000000" };
            //                var singleOrDefault = dc.Settings.SingleOrDefault(x => x.Name == "MedicalOrganization")?.Parametr;
            //                if (singleOrDefault == null || badMo.Contains(singleOrDefault))
            //                {
            //                    ErrorAndClose("Ошибка загрузки справочников");
            //                    return;
            //                }
            //            }
            //        }

            //try
            //{
            //    int scriptNumber = 0;
            //    bool isUpdate = false;
            //    string[] sqlArray = Directory.GetFiles(Environment.CurrentDirectory + @"\Update\", "Script???.sql");
            //    foreach (string s in sqlArray)
            //    {
            //        File.Delete(s);
            //    }

            //    string[] frxArray = Directory.GetFiles(Environment.CurrentDirectory + @"\Update\", "Script???.elm");
            //    foreach (string s in frxArray)
            //    {
            //        string scriptVer = s.Substring(s.Length - 13).Replace("Script", "").Replace(".elm", "");
            //        scriptNumber = Convert.ToInt32(scriptVer);
            //        using (ElmedDataClassesDataContext dc = new ElmedDataClassesDataContext(SprClass.LocalConnectionString))
            //        {
            //            Version v = dc.ExecuteQuery<Version>("SELECT TOP(1) Version as ver FROM DbVersion").Single();
            //            if (v.ver == scriptNumber - 1)
            //            {
            //                dc.CommandTimeout = 0;
            //                string script;
            //                using (
            //                    StreamReader sr = new StreamReader(@"Update\Script" + scriptVer + ".elm",
            //                                                       Encoding.Default))
            //                {
            //                    script = CipherUtility.Decrypt<TripleDESCryptoServiceProvider>(sr.ReadToEnd(),
            //                        "Elmed31948253Crypt", "3dfx");
            //                }

            //                if (!isUpdate)
            //                {
            //                    using (WaitForm.AsyncWaitDialog.ShowWaitDialog("Идет архивное копирование БД..." + Environment.NewLine + "Это может занять длительное время"))
            //                    {
            //                        dc.ExecuteCommand(string.Format(@"BACKUP DATABASE {0} TO disk='{0}-{1}.bak' WITH init", dc.Connection.Database, "Elmedicine" + "-" + Guid.NewGuid().ToString()));
            //                    }
            //                }
            //                dc.ExecuteCommand(script);
            //                dc.ExecuteCommand("Update DbVersion SET Version = {0}", scriptNumber);
            //                isUpdate = true;
            //            }
            //        }
            //        File.Delete(s);
            //    }
            //    if (isUpdate) messa.ShowError("База данных обновлена до версии - " + scriptNumber);
            //}
            //catch (Exception ex)
            //{
            //    ErrorAndClose(ex.Message);
            //    return;
            //}

        }
    }

    public class Version
    {
        public int ver { get; set; }
    }
}
