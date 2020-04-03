using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Data.SqlTypes;
using ICSharpCode.SharpZipLib.Zip;
using System.ComponentModel;
using System.Security.Cryptography;

namespace Update
{
    class upd
    {
        // Класс для чтения/записи INI-файлов
        public class INIManager
        {
            //Конструктор, принимающий путь к INI-файлу
            public INIManager(string aPath)
            {
                path = aPath;
            }

            //Конструктор без аргументов (путь к INI-файлу нужно будет задать отдельно)
            public INIManager() : this("") { }

            //Возвращает значение из INI-файла (по указанным секции и ключу) 
            public string GetPrivateString(string aSection, string aKey)
            {
                //Для получения значения
                StringBuilder buffer = new StringBuilder(SIZE);

                //Получить значение в buffer
                GetPrivateString(aSection, aKey, null, buffer, SIZE, path);

                //Вернуть полученное значение
                return buffer.ToString();
            }

            //Пишет значение в INI-файл (по указанным секции и ключу) 
            public void WritePrivateString(string aSection, string aKey, string aValue)
            {
                //Записать значение в INI-файл
                WritePrivateString(aSection, aKey, aValue, path);
            }

            //Возвращает или устанавливает путь к INI файлу
            public string Path { get { return path; } set { path = value; } }

            //Поля класса
            private const int SIZE = 1024; //Максимальный размер (для чтения значения из файла)
            private string path = null; //Для хранения пути к INI-файлу

            //Импорт функции GetPrivateProfileString (для чтения значений) из библиотеки kernel32.dll
            [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileString")]
            private static extern int GetPrivateString(string section, string key, string def, StringBuilder buffer, int size, string path);

            //Импорт функции WritePrivateProfileString (для записи значений) из библиотеки kernel32.dll
            [DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileString")]
            private static extern int WritePrivateString(string section, string key, string str, string path);
        }

        // Класс для апдейтера
        public class Updater
        {
            // Идентификаторы переменных.
            public class Identifiers
            {
                public string nameProgram; // Название программы.
                public string appElmedConfig; // Названия файла конфигурации оболочки.
                public string connectLocalDatabase; //Строка коннекта в локальную БД.
                public string connectServerDatabase; // Строка коннекта в серверуню БД.
                public string folderMain; // Главная папка Апдейтера.
                public string folderApps; // Папка с zip-архивами программ.
                public string folderScripts; // Папка с zip-архивами скриптов.
                public int code_org; // Код подразделения организации.
                public string type_org; // Тип подразделения.
                public int versionLocalDatabase; // Стандартная версия локальной базы данных.
                public int lAppMajor; // Версия приложения MAJOR.
                public int lAppMinor; // Версия приложения MINOR.
                public int lAppSubminor; // Версия приложения SUBMINOR.
                public int lDbMajor; // Версия базы данных MAJOR.
                public int lDbMinor; // Версия базы данных MINOR.
                public int lDbSubminor; // Версия базы данных SUBMINOR.
                public int sAppMajor; // Версия приложения MAJOR.
                public int sAppMinor; // Версия приложения MINOR.
                public int sAppSubminor; // Версия приложения SUBMINOR.
                public string sAppObjType; // Тип объекта.
                public string sAppDescription; // Описание объекта.
                public int sDbMajor; // Версия базы данных MAJOR.
                public int sDbMinor; // Версия базы данных MINOR.
                public int sDbSubminor; // Версия базы данных SUBMINOR.
                public string sDbObjType; // Тип объекта.
                public string sDbDescription; // Описание объекта.
                public List<string> newZipDb = new List<string>(); // Новые версии zip-архивов базы данных.
                public List<int> debtors = new List<int>(); // Список должников
            }

            Identifiers id = new Identifiers();

            // Импорт INI-настроек.
            public void insertINI()
            {
                INIManager ini = new INIManager(Path.Combine(Environment.CurrentDirectory, "Yamed Update.Settings.ini"));
                id.appElmedConfig = ini.GetPrivateString("imed", "elmedConfigName");
                string cstr = "";
                string[] text = File.ReadAllLines(id.appElmedConfig);
                foreach (var str in text)
                {
                    if (str.Contains("Data Source"))
                    {
                        cstr = str;
                    }
                }
                string[] ident_1 = cstr.Split(new char[] { '\n', '\"' }, StringSplitOptions.RemoveEmptyEntries);
                ini.WritePrivateString("imed", "connectLocalDatabase", ident_1[3]);
                id.connectServerDatabase = "Data Source=91.240.209.114, 2866; Initial Catalog=DocExchange; User ID=upduser_login; Password=228HenUjhJgjUyj";
                id.nameProgram = ini.GetPrivateString("imed", "elmedAppName");
                id.folderMain = ini.GetPrivateString("imed", "mainFolder");
                id.folderApps = ini.GetPrivateString("imed", "appsFolder");
                id.folderScripts = ini.GetPrivateString("imed", "scriptsFolder");
                id.connectLocalDatabase = ini.GetPrivateString("imed", "connectLocalDatabase");
            }

            // Пауза.
            public void pause()
            {
                Thread.Sleep(1000);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n Для выхода из программы нажмите любую клавишу.");
                Console.ResetColor();
                Console.ReadKey(true);
                Environment.Exit(0);
            }

            public void killPause()
            {
                string dir = Path.Combine(Environment.CurrentDirectory, id.folderMain, id.folderScripts);
                foreach (var file in Directory.GetFiles(dir))
                {
                    File.Delete(file);
                }
                Thread.Sleep(1000);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n Для выхода из программы нажмите любую клавишу.");
                Console.ResetColor();
                Console.ReadKey(true);
                Environment.Exit(0);
            }

            // Убивает процесс программы Электронная медицина.
            public void killProccess()
            {
                Thread.Sleep(1000);
                Process[] proc = Process.GetProcesses();
                string[] nP = id.nameProgram.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (Process process in proc)
                {
                    if (process.ProcessName == nP[0])
                    {
                        process.Kill();
                    }
                }
            }

            // Поиск и создание папок Апдейтера
            public void checkFolders()
            {
                if (Directory.Exists(id.folderMain))
                {

                }
                else
                {
                    Directory.CreateDirectory(id.folderMain);
                }
                if (Directory.Exists(Path.Combine(id.folderMain, id.folderApps)))
                {

                }
                else
                {
                    Directory.CreateDirectory(Path.Combine(id.folderMain, id.folderApps));
                }
                if (Directory.Exists(Path.Combine(id.folderMain, id.folderScripts)))
                {

                }
                else
                {
                    Directory.CreateDirectory(Path.Combine(id.folderMain, id.folderScripts));
                }
            }

            // Проверка подключения к локальной базе данных.
            public void checkConnectionLocalDatabase()
            {
                Thread.Sleep(1000);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n Выполняется процесс проверки подключения к локальной базе данных.");
                using (SqlConnection sqlConn = new SqlConnection(id.connectLocalDatabase))
                {
                    sqlConn.Open();
                    if (sqlConn.State == System.Data.ConnectionState.Open)
                    {
                        Thread.Sleep(1000);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(" - Успешно.");
                        sqlConn.Close();
                    }
                    else
                    {
                        Thread.Sleep(1000);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(" Не удалось подключиться к локальной базе данных.");
                        Thread.Sleep(1000);
                        Console.WriteLine(" Проверьте параметры конфигурационного файла " + id.appElmedConfig + ".");
                        sqlConn.Close();
                        pause();
                    }
                }
            }

            // Проверка подключения к серверной базе данных.
            public void checkConnectionServerDatabase(string connectServerDatabase)
            {
                id.connectServerDatabase = connectServerDatabase;
                Thread.Sleep(1000);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n Выполняется процесс проверки подключения к серверной базе данных.");
                using (SqlConnection sqlConn = new SqlConnection(connectServerDatabase))
                {
                    sqlConn.Open();
                    if (sqlConn.State == System.Data.ConnectionState.Open)
                    {
                        Thread.Sleep(1000);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(" - Успешно.");
                        sqlConn.Close();
                    }
                    else
                    {
                        Thread.Sleep(1000);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(" Не удалось подключиться к серверной базе данных.");
                        sqlConn.Close();
                        pause();
                    }
                }
            }

            // Проверка правил, создание таблицы LogUpdates, добавление записей версий.
            public void scriptExtensionTable()
            {
                int existTable = 0;
                using (SqlConnection sqlConn = new SqlConnection(id.connectLocalDatabase))
                {
                    sqlConn.Open();
                    using (SqlCommand sqlComm = new SqlCommand("select * from DbVersion", sqlConn))
                    {
                        sqlComm.CommandTimeout = 0;
                        using (SqlDataReader sqlReader = sqlComm.ExecuteReader())
                        {
                            while (sqlReader.Read())
                            {
                                id.versionLocalDatabase = Convert.ToInt32(sqlReader["version"]);
                            }
                        }
                    }
                    if (id.versionLocalDatabase == 1185)
                    {
                        using (SqlCommand sqlComm = new SqlCommand("select Parametr from Settings where id = 1", sqlConn))
                        {
                            using (SqlDataReader sqlReader = sqlComm.ExecuteReader())
                            {
                                sqlComm.CommandTimeout = 0;
                                while (sqlReader.Read())
                                {
                                    id.code_org = Convert.ToInt32(sqlReader["Parametr"]);
                                }
                            }
                        }
                        if (id.code_org.ToString().Length == 2)
                        {
                            id.type_org = "TF";
                        }
                        if (id.code_org.ToString().Length == 4 || id.code_org.ToString().Length == 5)
                        {
                            id.type_org = "SMO";
                        }
                        if (id.code_org.ToString().Length == 6)
                        {
                            id.type_org = "MO";
                        }
                        using (SqlCommand sqlComm = new SqlCommand("SELECT COUNT(*) as count FROM sys.sysobjects WHERE name='LogUpdates'", sqlConn))
                        {
                            sqlComm.CommandTimeout = 0;
                            using (SqlDataReader sqlReader = sqlComm.ExecuteReader())
                            {
                                while (sqlReader.Read())
                                {
                                    existTable = Convert.ToInt32(sqlReader["count"]);
                                }
                            }
                        }
                        if (existTable == 0)
                        {
                            using (SqlCommand sqlComm = new SqlCommand("if object_id(N'LogUpdates', 'U') is null create table LogUpdates (ID int identity (1,1) not null primary key,  MAJOR int not null,  MINOR int not null,  SUBMINOR int not null,  OBJ_TYPE varchar(50) not null,  DESCRIPTION varchar(2500),  INSTALL_DATE datetime not null)", sqlConn))
                            {
                                sqlComm.CommandTimeout = 0;
                                using (SqlDataReader sqlReader = sqlComm.ExecuteReader())
                                {

                                }

                            }
                            using (SqlCommand sqlComm = new SqlCommand(" alter table LogUpdates add  constraint DF_MAJOR default 0 for MAJOR,  constraint DF_MINOR default 0 for MINOR,  constraint DF_SUBMINOR default 0 for SUBMINOR,  constraint DF_INSTALL_DATE default getdate() for INSTALL_DATE,  constraint AK_VERSION_OF_OBJ unique (MAJOR, MINOR, SUBMINOR, OBJ_TYPE)", sqlConn))
                            {
                                sqlComm.CommandTimeout = 0;
                                using (SqlDataReader sqlReader = sqlComm.ExecuteReader())
                                {

                                }
                            }
                            using (SqlCommand sqlComm = new SqlCommand("insert into logupdates (MAJOR, MINOR, SUBMINOR, OBJ_TYPE, DESCRIPTION) values ('0', '0', " + id.versionLocalDatabase + ", 'DB.ELMED." + id.type_org + "', 'Версия локальной базы данных.')", sqlConn))
                            {
                                sqlComm.CommandTimeout = 0;
                                using (SqlDataReader sqlReader = sqlComm.ExecuteReader())
                                {

                                }
                            }
                        }
                        sqlConn.Close();
                    }
                    else
                    {
                        Thread.Sleep(1000);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("\n [Re] Проверка версии программы.");
                        Thread.Sleep(1000);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n Обновите программу до версии 1185 с сайта elmedicine.ru.");
                        pause();
                    }
                }
            }

            // Проверка версий локальной программы и базы данных.
            public void checkVersionLocal()
            {
                using (SqlConnection sqlConn = new SqlConnection(id.connectLocalDatabase))
                {
                    sqlConn.Open();
                    using (SqlCommand sqlComm = new SqlCommand("select top 1 * from logupdates where obj_type = 'DB.ELMED." + id.type_org + "' order by major desc, minor desc, subminor desc", sqlConn))
                    {
                        sqlComm.CommandTimeout = 0;
                        using (SqlDataReader sqlReader = sqlComm.ExecuteReader())
                        {
                            while (sqlReader.Read())
                            {
                                id.lDbMajor = Convert.ToInt32(sqlReader["major"]);
                                id.lDbMinor = Convert.ToInt32(sqlReader["minor"]);
                                id.lDbSubminor = Convert.ToInt32(sqlReader["subminor"]);
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }

            // Проверка версий серверной программы и базы данных.
            public void checkVersionServerOnline()
            {
                using (SqlConnection sqlConn = new SqlConnection(id.connectServerDatabase))
                {
                    sqlConn.Open();
                    using (SqlCommand sqlComm = new SqlCommand("select top 1 * from imed_updates where enable = 1 and obj_type = 'APP.ELMED." + id.type_org + "' order by major desc, minor desc, subminor desc", sqlConn))
                    {
                        sqlComm.CommandTimeout = 0;
                        using (SqlDataReader sqlReader = sqlComm.ExecuteReader())
                        {
                            while (sqlReader.Read())
                            {
                                id.sAppMajor = Convert.ToInt32(sqlReader["major"]);
                                id.sAppMinor = Convert.ToInt32(sqlReader["minor"]);
                                id.sAppSubminor = Convert.ToInt32(sqlReader["subminor"]);
                                id.sAppObjType = Convert.ToString(sqlReader["obj_type"]);
                                id.sAppDescription = Convert.ToString(sqlReader["description"]);
                            }
                        }
                    }
                    using (SqlCommand sqlComm = new SqlCommand("select * from imed_updates where org_code is null and enable = 1 and major >= " + id.lDbMajor + " and minor >= " + id.lDbMinor + " and subminor >= " + id.lDbSubminor + "and obj_type = 'DB.ELMED." + id.type_org + "'", sqlConn))
                    {
                        sqlComm.CommandTimeout = 0;
                        using (SqlDataReader sqlReader = sqlComm.ExecuteReader())
                        {
                            while (sqlReader.Read())
                            {
                                id.sDbMajor = Convert.ToInt32(sqlReader["major"]);
                                id.sDbMinor = Convert.ToInt32(sqlReader["minor"]);
                                id.sDbSubminor = Convert.ToInt32(sqlReader["subminor"]);
                                id.sDbObjType = Convert.ToString(sqlReader["obj_type"]);
                                id.sDbDescription = Convert.ToString(sqlReader["description"]);
                                if (id.lDbMajor == id.sDbMajor & id.lDbMinor == id.sDbMinor & id.lDbSubminor == id.sDbSubminor)
                                {

                                }
                                else
                                {
                                    id.newZipDb.AddRange(new[] { Convert.ToString(id.sDbMajor), Convert.ToString(id.sDbMinor), Convert.ToString(id.sDbSubminor), Convert.ToString(id.sDbObjType), Convert.ToString(id.sDbDescription) });
                                }
                            }
                        }
                    }
                    using (SqlCommand sqlComm = new SqlCommand("select * from imed_updates where org_code = '" + id.code_org + "' and enable = 1 and major >= " + id.lDbMajor + " and minor >= " + id.lDbMinor + " and subminor >= " + id.lDbSubminor + "and obj_type = 'DB.ELMED." + id.type_org + "'", sqlConn))
                    {
                        sqlComm.CommandTimeout = 0;
                        using (SqlDataReader sqlReader = sqlComm.ExecuteReader())
                        {
                            while (sqlReader.Read())
                            {
                                id.sDbMajor = Convert.ToInt32(sqlReader["major"]);
                                id.sDbMinor = Convert.ToInt32(sqlReader["minor"]);
                                id.sDbSubminor = Convert.ToInt32(sqlReader["subminor"]);
                                id.sDbObjType = Convert.ToString(sqlReader["obj_type"]);
                                id.sDbDescription = Convert.ToString(sqlReader["description"]);
                                if (id.lDbMajor == id.sDbMajor & id.lDbMinor == id.sDbMinor & id.lDbSubminor == id.sDbSubminor)
                                {

                                }
                                else
                                {
                                    id.newZipDb.AddRange(new[] { Convert.ToString(id.sDbMajor), Convert.ToString(id.sDbMinor), Convert.ToString(id.sDbSubminor), Convert.ToString(id.sDbObjType), Convert.ToString(id.sDbDescription) });
                                }
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }

            // Загрузка и инсталляция zip-архива программы.
            public void installZipProgram()
            {
                using (SqlConnection sqlConn = new SqlConnection(id.connectServerDatabase))
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Thread.Sleep(1000);
                    Console.WriteLine("\n Выполняется процесс обновления программы.");
                    sqlConn.Open();
                    using (SqlCommand sqlComm = new SqlCommand("select data from imed_updates where major = " + id.sAppMajor + " and minor = " + id.sAppMinor + " and subminor = " + id.sAppSubminor + "and obj_type = '" + id.sAppObjType + "'", sqlConn))
                    {
                        sqlComm.CommandTimeout = 0;
                        using (SqlDataReader sqlReader = sqlComm.ExecuteReader())
                        {
                            while (sqlReader.Read())
                            {
                                string filename = id.sAppObjType + "_" + id.sAppMajor + "." + id.sAppMinor + "." + id.sAppSubminor + ".zip";
                                using (Stream stream = File.Open(Path.Combine(id.folderMain, id.folderApps, filename), FileMode.Create))
                                {
                                    BinaryFormatter bf = new BinaryFormatter();
                                    bf.Serialize(stream, sqlReader["data"]);
                                }
                                string zip = Path.Combine(Environment.CurrentDirectory, id.folderMain, id.folderApps, filename);
                                string path = Path.Combine(Environment.CurrentDirectory);
                                FastZip fz = new FastZip();
                                fz.ExtractZip(zip, path, null);
                                Thread.Sleep(1000);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine(" - Успешно.");
                                Thread.Sleep(1000);
                                Console.WriteLine(" Программа обновлена до последней версии.");
                                File.Delete(Path.Combine(id.folderMain, id.folderApps, filename));
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }

            // Загрузка и инсталляция zip-архива базы данных.
            public void installZipDataBase()
            {
                Console.ForegroundColor = ConsoleColor.White;
                Thread.Sleep(1000);
                Console.WriteLine("\n Выполняется процесс обновления базы данных.");
                if (id.newZipDb.Count > 0)
                {
                    for (int i = 0; i < id.newZipDb.Count; i++)
                    {
                        using (SqlConnection sqlConn = new SqlConnection(id.connectServerDatabase))
                        {
                            string filename = id.newZipDb[i + 3] + "_" + id.newZipDb[i] + "." + id.newZipDb[i + 1] + "." + id.newZipDb[i + 2] + ".zip";
                            sqlConn.Open();
                            using (SqlCommand sqlComm = new SqlCommand("select data from imed_updates where major = " + id.newZipDb[i] + " and minor = " + id.newZipDb[i + 1] + " and subminor = " + id.newZipDb[i + 2] + " and obj_type = '" + id.newZipDb[i + 3] + "'", sqlConn))
                            {
                                sqlComm.CommandTimeout = 0;
                                using (SqlDataReader sqlReader = sqlComm.ExecuteReader())
                                {
                                    while (sqlReader.Read())
                                    {
                                        using (Stream stream = File.Open(Path.Combine(id.folderMain, id.folderScripts, filename), FileMode.Create))
                                        {
                                            BinaryFormatter bf = new BinaryFormatter();
                                            bf.Serialize(stream, sqlReader["data"]);
                                        }
                                    }
                                }
                            }
                            string zip = Path.Combine(Environment.CurrentDirectory, id.folderMain, id.folderScripts, filename);
                            string path = Path.Combine(Environment.CurrentDirectory, id.folderMain, id.folderScripts);
                            FastZip fz = new FastZip();
                            fz.ExtractZip(zip, path, null);
                            List<string> ScriptsFiles = new List<string>();
                            foreach (string findedFile in Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, id.folderMain, id.folderScripts), "*.sql", SearchOption.TopDirectoryOnly))
                            {
                                ScriptsFiles.Add(findedFile);
                            }
                            ScriptsFiles = ScriptsFiles.OrderBy(s => s.Length).ThenBy(s => s).ToList();
                            using (SqlConnection sConn = new SqlConnection(id.connectLocalDatabase))
                            {
                                sConn.Open();
                                string nSQLTran = "sqlTran" + "[" + i + "]";
                                using (SqlTransaction sqlTran = sConn.BeginTransaction(IsolationLevel.ReadCommitted, nSQLTran))
                                {
                                    int qq = 0;
                                    for (int q = 0; q < ScriptsFiles.Count; q++)
                                    {
                                        qq = q;
                                        using (StreamReader sr = new StreamReader(ScriptsFiles[q]))
                                        {
                                            string tScript = sr.ReadToEnd();
                                            using (SqlCommand sc = new SqlCommand(tScript, sConn, sqlTran))
                                            {
                                                sc.CommandTimeout = 0;
                                                try
                                                {
                                                    if (qq != ScriptsFiles.Count - 1)
                                                    {
                                                        sc.ExecuteNonQuery();
                                                        sc.Dispose();
                                                    }
                                                    else
                                                    {
                                                        sc.ExecuteNonQuery();
                                                        sqlTran.Commit();
                                                        string insertLog = "insert into LogUpdates(major, minor, subminor, obj_type, description) values(" + id.newZipDb[i] + ", " + id.newZipDb[i + 1] + ", " + id.newZipDb[i + 2] + ", '" + id.newZipDb[i + 3] + "', '" + id.newZipDb[i + 4] + "')";
                                                        using (SqlTransaction sqlTranLog = sConn.BeginTransaction(IsolationLevel.ReadCommitted))
                                                        {
                                                            using (SqlCommand scLog = new SqlCommand(insertLog, sConn, sqlTranLog))
                                                            {
                                                                scLog.CommandTimeout = 0;
                                                                try
                                                                {
                                                                    scLog.ExecuteNonQuery();
                                                                    sqlTranLog.Commit();
                                                                    Console.ForegroundColor = ConsoleColor.Green;
                                                                    Thread.Sleep(1000);
                                                                    Console.WriteLine(" - Успешно.");
                                                                    Thread.Sleep(1000);
                                                                    Console.WriteLine(" База данных обновлена до версии " + id.newZipDb[i] + "." + id.newZipDb[i + 1] + "." + id.newZipDb[i + 2] + ".");
                                                                    scLog.Dispose();
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    sqlTran.Rollback();
                                                                    Console.ForegroundColor = ConsoleColor.Red;
                                                                    Thread.Sleep(1000);
                                                                    Console.WriteLine(" Ошибка: " + ex.Message);
                                                                    killPause();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    sr.Close();
                                                    sqlTran.Rollback();
                                                    Console.ForegroundColor = ConsoleColor.Red;
                                                    Thread.Sleep(1000);
                                                    Console.WriteLine(" Обновление прервано.");
                                                    Thread.Sleep(1000);
                                                    Console.WriteLine(" Архив: " + filename + ".");
                                                    Thread.Sleep(1000);
                                                    Console.WriteLine(" Файл: " + Path.GetFileName(ScriptsFiles[qq]) + ".");
                                                    Thread.Sleep(1000);
                                                    Console.WriteLine(" Ошибка: " + ex.Message);
                                                    killPause();
                                                }
                                            }
                                            sr.Close();
                                        }
                                        File.Delete(ScriptsFiles[q]);
                                    }
                                }
                                sConn.Close();
                            }
                            File.Delete(Path.Combine(id.folderMain, id.folderScripts, filename));
                            i += 4;
                            sqlConn.Close();
                        }
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Thread.Sleep(1000);
                    Console.WriteLine(" Ваша система использует актуальную версию базы данных.");
                }
            }

            // Инсталляция zip-архивов программы в оффлайн режиме.
            public void installProgramOffline()
            {
                int cInstZip = 0;
                int major = 0;
                int minor = 0;
                int subminor = 0;
                Thread.Sleep(1000);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n Выполняется процесс обновления программы.");
                Regex regZipFiles = new Regex("APP[.]{1}ELMED[.]{1}" + id.type_org + "_[0-9]{1,}[.]{1}[0-9]{1,}[.]{1}[0-9]{1,}[.]{1}zip");
                string[] findZipFile = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, id.folderMain, id.folderApps), "*.zip", SearchOption.TopDirectoryOnly).Where(f => regZipFiles.IsMatch(f)).ToArray();
                if (findZipFile.Length > 0)
                {
                    for (int i = 0; i < findZipFile.Length; i++)
                    {
                        string zip = findZipFile[i];
                        string zipFileName = Path.GetFileName(zip);
                        string path = AppDomain.CurrentDomain.BaseDirectory.ToString() + "/";
                        string[] sub = { ".", "_" };
                        string[] zipSub = zipFileName.Split(sub, StringSplitOptions.RemoveEmptyEntries);
                        major = Convert.ToInt32(zipSub[3]);
                        minor = Convert.ToInt32(zipSub[4]);
                        subminor = Convert.ToInt32(zipSub[5]);
                        if (id.lAppMajor >= Convert.ToInt32(zipSub[3]) & id.lAppMinor >= Convert.ToInt32(zipSub[4]) & id.lAppSubminor >= Convert.ToInt32(zipSub[5]))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Thread.Sleep(1000);
                            Console.WriteLine(" Обнаружен zip-архив ниже текущей версии программы.");
                            Thread.Sleep(1000);
                            Console.WriteLine(" Установка данного zip-архива невозможна.");
                            File.Delete(findZipFile[i]);
                        }
                        else
                        {
                            string objtype = zipSub[0] + "." + zipSub[1] + "." + zipSub[2];
                            FastZip fz = new FastZip();
                            fz.ExtractZip(zip, path, null);
                            cInstZip++;
                        }
                        File.Delete(findZipFile[i]);
                    }
                    if (cInstZip > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Thread.Sleep(1000);
                        Console.WriteLine(" Программа обновлена до версии " + major + "." + minor + "." + subminor + ".");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Thread.Sleep(1000);
                        Console.WriteLine(" Программа не обновлена.");
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Thread.Sleep(1000);
                    Console.WriteLine(" Zip-архивов программ не обнаружено.");
                }
            }

            // Инсталляция скриптов в оффлайн режиме
            public void installDatabaseOffline()
            {
                int major = 0;
                int minor = 0;
                int subminor = 0;
                Thread.Sleep(1000);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n Выполняется процесс обновления базы данных.");
                Regex regZipScripts = new Regex("DB[.]{1}ELMED[.]{1}" + id.type_org + "_[0-9]{1,}[.]{1}[0-9]{1,}[.]{1}[0-9]{1,}[.]{1}zip");
                string[] findZipScripts = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, id.folderMain, id.folderScripts), "*.zip", SearchOption.TopDirectoryOnly).Where(f => regZipScripts.IsMatch(f)).ToArray();
                if (findZipScripts.Length > 0)
                {
                    for (int i = 0; i < findZipScripts.Length; i++)
                    {
                        string zip = findZipScripts[i];
                        string zipFileName = Path.GetFileName(zip);
                        string path = Path.Combine(Environment.CurrentDirectory, id.folderMain, id.folderScripts);
                        string[] sub = { ".", "_" };
                        string[] zipSub = zipFileName.Split(sub, StringSplitOptions.RemoveEmptyEntries);
                        major = Convert.ToInt32(zipSub[3]);
                        minor = Convert.ToInt32(zipSub[4]);
                        subminor = Convert.ToInt32(zipSub[5]);
                        string objtype = zipSub[0] + "." + zipSub[1] + "." + zipSub[2];
                        if (id.lDbMajor >= Convert.ToInt32(zipSub[3]) & id.lDbMinor >= Convert.ToInt32(zipSub[4]) & id.lDbSubminor >= Convert.ToInt32(zipSub[5]))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Thread.Sleep(1000);
                            Console.WriteLine(" \n Обнаружен zip-архив ниже текущей версии базы данных.");
                            Thread.Sleep(1000);
                            Console.WriteLine(" Установка данного zip-архива невозможна. \n");
                            File.Delete(findZipScripts[i]);
                        }
                        else
                        {
                            FastZip fz = new FastZip();
                            fz.ExtractZip(zip, path, null);
                            List<string> ScriptsFiles = new List<string>();
                            foreach (string findedFile in Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, id.folderMain, id.folderScripts), "*.sql", SearchOption.TopDirectoryOnly))
                            {
                                ScriptsFiles.Add(findedFile);
                            }
                            ScriptsFiles = ScriptsFiles.OrderBy(s => s.Length).ThenBy(s => s).ToList();
                            using (SqlConnection sConn = new SqlConnection(id.connectLocalDatabase))
                            {
                                sConn.Open();
                                using (SqlTransaction sqlTran = sConn.BeginTransaction(IsolationLevel.ReadCommitted))
                                {
                                    int qq = 0;
                                    for (int q = 0; q < ScriptsFiles.Count; q++)
                                    {
                                        qq = q;
                                        using (StreamReader sr = new StreamReader(ScriptsFiles[q]))
                                        {
                                            string tScript = sr.ReadToEnd();
                                            using (SqlCommand sComm = new SqlCommand(tScript, sConn, sqlTran))
                                            {
                                                sComm.CommandTimeout = 0;
                                                try
                                                {
                                                    if (qq != ScriptsFiles.Count - 1)
                                                    {
                                                        sComm.ExecuteNonQuery();
                                                        sComm.Dispose();
                                                    }
                                                    else
                                                    {
                                                        sComm.ExecuteNonQuery();
                                                        sqlTran.Commit();
                                                        sComm.Dispose();
                                                        using (SqlTransaction sqlTranLog = sConn.BeginTransaction(IsolationLevel.ReadCommitted))
                                                        {
                                                            using (SqlCommand sCommLog = new SqlCommand("insert into LogUpdates(major, minor, subminor, obj_type) values (" + zipSub[3] + "," + zipSub[4] + "," + zipSub[5] + ",'" + objtype + "')", sConn, sqlTranLog))
                                                            {
                                                                sCommLog.CommandTimeout = 0;
                                                                try
                                                                {
                                                                    sCommLog.ExecuteNonQuery();
                                                                    sqlTranLog.Commit();
                                                                    Console.ForegroundColor = ConsoleColor.Green;
                                                                    Thread.Sleep(1000);
                                                                    Console.WriteLine(" - Успешно.");
                                                                    Thread.Sleep(1000);
                                                                    Console.WriteLine(" База данных обновлена до версии " + zipSub[3] + "." + zipSub[4] + "." + zipSub[5] + ".");
                                                                    sCommLog.Dispose();
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    sqlTranLog.Rollback();
                                                                    Console.ForegroundColor = ConsoleColor.Red;
                                                                    Thread.Sleep(1000);
                                                                    Console.WriteLine(" Ошибка: " + ex.Message);
                                                                    Thread.Sleep(1000);
                                                                    Console.WriteLine(" База данных не обновлена.");
                                                                    killPause();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    sr.Close();
                                                    sqlTran.Rollback();
                                                    Console.ForegroundColor = ConsoleColor.Red;
                                                    Thread.Sleep(1000);
                                                    Console.WriteLine(" Обновление прервано.");
                                                    Thread.Sleep(1000);
                                                    Console.WriteLine(" Архив: " + Path.GetFileName(zip) + ".zip.");
                                                    Thread.Sleep(1000);
                                                    Console.WriteLine(" Файл: " + Path.GetFileName(ScriptsFiles[qq]) + ".");
                                                    Thread.Sleep(1000);
                                                    Console.WriteLine(" Ошибка: " + ex.Message);
                                                    killPause();
                                                }
                                            }
                                        }
                                        File.Delete(ScriptsFiles[q]);
                                    }
                                }
                            }
                        }
                        File.Delete(findZipScripts[i]);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Thread.Sleep(1000);
                    Console.WriteLine(" Zip-архивов скриптов не обнаружено.");
                }
            }

            // Проверка правил, скачивание zip-архивов и их инсталляция.
            public void Update()
            {
                insertINI();
                using (SqlConnection sqlconnL = new SqlConnection(id.connectLocalDatabase))
                {
                    sqlconnL.Open();
                    using (SqlCommand sqlcommL = new SqlCommand("select Parametr from Settings where id = 1", sqlconnL))
                    {
                        sqlcommL.CommandTimeout = 0;
                        using (SqlDataReader sqlreadL = sqlcommL.ExecuteReader())
                        {
                            while (sqlreadL.Read())
                            {
                                id.code_org = Convert.ToInt32(sqlreadL["Parametr"]);
                            }
                            sqlreadL.Close();
                        }
                        sqlcommL.Dispose();
                    }
                    sqlconnL.Close();
                }
                using (SqlConnection sqlconn = new SqlConnection(id.connectServerDatabase))
                {
                    sqlconn.Open();
                    using (SqlCommand sqlcomm = new SqlCommand("select org_code	from imed_clients_products where product = 'ELMED.MO' and debt_amount > 0 and upd_enable = 0", sqlconn))
                    {
                        sqlcomm.CommandTimeout = 0;
                        using (SqlDataReader sqlread = sqlcomm.ExecuteReader())
                        {
                            while (sqlread.Read())
                            {
                                id.debtors.Add(Convert.ToInt32(sqlread["org_code"]));
                            }
                            sqlread.Close();
                        }
                        sqlcomm.Dispose();
                    }
                    sqlconn.Close();
                }
                for (int i = 0; i < id.debtors.Count; i++)
                {
                    if (id.code_org == id.debtors[i])
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(" \n Данная услуга недоступна для вашей организации.");
                        pause();
                    }
                    else
                    {
                        killProccess();
                        try
                        {
                            using (SqlConnection sqlConn = new SqlConnection(id.connectServerDatabase))
                            {
                                sqlConn.Open();
                                if (sqlConn.State == System.Data.ConnectionState.Open)
                                {
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.WriteLine("\n Включена поддержка онлайн режима.");
                                    checkFolders();
                                    checkConnectionLocalDatabase();
                                    checkConnectionServerDatabase(id.connectServerDatabase);
                                    scriptExtensionTable();
                                    checkVersionLocal();
                                    checkVersionServerOnline();
                                    installZipProgram();
                                    installZipDataBase();
                                    pause();
                                }
                                sqlConn.Close();
                            }
                        }
                        catch (SqlException)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("\n Включена поддержка оффлайн режима.");
                            checkFolders();
                            checkConnectionLocalDatabase();
                            scriptExtensionTable();
                            checkVersionLocal();
                            installProgramOffline();
                            installDatabaseOffline();
                            pause();
                        }
                    }
                }
            }
        }
    }
}