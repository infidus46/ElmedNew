﻿using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace WebUI.Settings
{
    namespace VirtualUI
    {
        // Constants for enum Protocol
        public enum Protocol
        {
            PROTO_HTTP = 0,
            PROTO_HTTPS = 1
        }

        // Constants for enum ProfileKind
        public enum ProfileKind
        {
            PROFILE_APP = 0,
            PROFILE_WEBLINK = 1
        }

        // Constants for enum ScreenResolution
        public enum ScreenResolution
        {
            SCREENRES_Custom = 0,
            SCREENRES_FitToBrowser = 1,
            SCREENRES_FitToScreen = 2,
            SCREENRES_640x480 = 3,
            SCREENRES_800x600 = 4,
            SCREENRES_1024x768 = 5,
            SCREENRES_1280x720 = 6,
            SCREENRES_1280x768 = 7,
            SCREENRES_1280x1024 = 8,
            SCREENRES_1440x900 = 9,
            SCREENRES_1440x1050 = 10,
            SCREENRES_1600x1200 = 11,
            SCREENRES_1680x1050 = 12,
            SCREENRES_1920x1080 = 13,
            SCREENRES_1920x1200 = 14
        }

        // Constants for enum ServerSection
        public enum ServerSection
        {
            SRVSEC_GENERAL = 0,
            SRVSEC_RDS = 1,
            SRVSEC_APPLICATIONS = 2,
            SRVSEC_LICENSES = 3
        }

        [Guid("845B4EE8-0F67-4D84-A4CE-642BBD520A47"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch), ComImport]
        public interface IServer
        {
            void Load();
            void Save();
            void HideSection(ServerSection section);
            void ShowSection(ServerSection section);
            IBinding Binding { get; }
            ICertificate Certificate { get; }
            IRDSAccounts RDSAccounts { get; }
            IProfiles Profiles { get; }
            ILicense License { get; }
            IGateways Gateways { get; }
            string NetworkID { get; set; }
        }

        /// <summary>
        ///   Contains methods and properties to control the VirtualUI Server's
        ///   licence activation. The Activate method is used to register the Server
        ///   machine, with a combination of CustomerID and Serial.
        /// </summary>
        [Guid("A1DF5DC4-7157-4643-B28F-3B3D20A0E5C8"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
        public interface ILicense
        {
            /// <summary>
            ///   Activates the Server's machine licence.
            /// </summary>
            /// <param name="customerId">
            ///   ID of the licence to register.
            /// </param>
            /// <param name="serial">
            ///   Serial number of the license.
            /// </param>
            /// <param name="resultCode">
            ///   Activation result code.
            /// </param>
            /// <param name="resultText">
            ///   Message about the error.
            /// </param>
            /// <returns>
            ///   True if license was successfully activated. False otherwise (check
            ///   resultCode and resultText in this case).
            /// </returns>
            bool Activate(string customerId, string serial, out int resultCode, out string resultText);

            /// <summary>
            ///   Deactivates the licence previously activated.
            /// </summary>
            void Deactivate();

            /// <summary>
            /// Using the Serial number property, gets from Activation Server the
            /// Manual Key the Server's machine. This key can be used to generate the
            /// license data needed to perform manual activation.
            /// </summary>
            /// <returns>
            /// The manual activation key.
            /// </returns>
            string GetManualActivationKey();

            /// <summary>
            /// Activates the Server's machine license manually.
            /// </summary>
            /// <param name="Data">License data received from Server. </param>
            /// <param name="resultCode">Activation result code. </param>
            /// <param name="resultText">Message about the error. </param>
            /// <returns>
            /// True if the license was successfully activated. False
            /// otherwise (in which case check resultCode and resultText).
            /// </returns>
            bool ActivateManual(string Data, out int resultCode, out string resultText);

            /// <summary>
            ///   ID of the current Server License.
            /// </summary>
            string CustomerID { get; set; }

            /// <summary>
            ///   Returns limits of the License, if any (ie, trial days, max servers,
            ///   max users per installation, etc).
            /// </summary>
            //TODO: int Limits[object name] { get; }

            /// <summary>
            ///   Returns custom features enabled on the License, if any.
            /// </summary>
            //TODO: int Features[object name] { get; }

            /// <summary>
            ///   Returns true if the current License is in trial mode.
            /// </summary>
            bool IsTrial { get; set; }

            /// <summary>
            ///   Serial number of the current License.
            /// </summary>
            string SerialStr { get; set; }
        }

        /// <summary>
        ///   Contains all properties of an application profile.
        /// </summary>
        [ComVisible(true), Guid("D478CC7A-8071-47BD-BA2D-845131B51B42"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
        public interface IProfile
        {
            /// <summary>
            ///   Internal ID of the profile. This value is auto generated by the
            ///   library when the profile is created.
            /// </summary>
            string ID { get; set; }

            /// <summary>
            ///   Profile name. Is the caption for the Application or Web link in the
            ///   VirtualUI home page.
            /// </summary>
            string Name { get; set; }

            /// <summary>
            ///   The Virtual Path must be unique across all profiles. It will create a
            ///   unique URL address for the profile. The complete path will consist
            ///   of: http(s)://ip:port/VirtualPath/. The users can then create a web
            ///   shortcut to this connection in particular and bypass the Thinfinity
            ///   VirtualUI home page.
            /// </summary>
            string VirtualPath { get; set; }
            
            /// <summary>
            ///   This option is used to make this profile the default application: the
            ///   authenticated user will connect to this profile directly instead of
            ///   choosing between the available profiles on the VirtualUI home page.
            /// </summary>
            bool IsDefault { get; set; }

            /// <summary>
            ///   Enables or disables the profile. Disabled profiles are not accesible
            ///   by users.
            /// </summary>
            bool Enabled { get; set; }

            /// <summary>
            ///   Gets or sets the profile type: Application or Web Link. Uses the
            ///   constants PROFILE_App and PROFILE_WebLink.
            /// </summary>
            ProfileKind ProfileKind { get; set; }

            /// <summary>
            ///   Complete path of the application executable file. Only used when the
            ///   ProfileKind is Application.
            /// </summary>
            string FileName { get; set; }

            /// <summary>
            ///   Parameters to be passed to application.
            /// </summary>
            string Arguments { get; set; }

            /// <summary>
            ///   Application startup directory. In most cases, the same of application
            ///   executable file.
            /// </summary>
            string StartDir { get; set; }

            /// <summary>
            ///   A valid Windows User account to run the application.
            /// </summary>
            string UserName { get; set; }

            /// <summary>
            ///   Password of the Windows User account.
            /// </summary>
            string Password { get; set; }

            /// <summary>
            ///   Screen resolution in the browser. Uses the constants SCREENRES_...
            /// </summary>
            ScreenResolution ScreenResolution { get; set; }

            /// <summary>
            ///   Full URL of the Web Link (only used when ProfileKind is Web Link).
            /// </summary>
            string WebLink { get; set; }

            /// <summary>
            ///   Used to set a customized home page for the application.
            /// </summary>
            string HomePage { get; set; }

            /// <summary>
            ///   Set a timeout in minutes if you want VirtualUI Server to wait this
            ///   period before killing the application once the browser has been
            ///   closed. Timeout 0 will kill the application immediately after the
            ///   browser has been closed.
            /// </summary>
            int IdleTimeout { get; set; }

            /// <summary>
            ///   Contains the icon of the profile. This icon is visible in the
            ///   VirtualUI home page. The icon must be encoded in base64. To set the
            ///   icon from a PNG image, you can use the IconToBase64 function. To
            ///   convert the stored icon to a PNG image, can use the Base64ToIcon
            ///   function.
            /// </summary>
            string IconData { get; set; }

            /// <summary>
            ///   Profiles marked as not visible are hidden in home page.
            /// </summary>
            bool Visible { get; set; }
        }

        /// <summary>
        ///   Manages the list of profiles registered in the Server. A profile is one
        ///   application or web link configured to be opened in VirtualUI's home
        ///   page (or directly through it's URL). The method Add is used to create a
        ///   new empty profile in the VirtualUI Server. The properties of this
        ///   profile can be managed through the IProfile interface to complete
        ///   configuration of a new application or web link.
        /// </summary>
        [Guid("C271394D-82FA-4DF9-A603-9927AA76A4F9"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
        public interface IProfiles
        {
            /// <summary>
            /// Creates a new profile and adds it to the list.
            /// </summary>
            /// <returns>
            /// The newly created profile.
            /// </returns>
            /// <seealso cref="IProfile interface"/>
            IProfile Add();

            /// <summary>
            ///   Deletes a profile from the list.
            /// </summary>
            /// <param name="profile">
            ///   The profile to be deleted.
            /// </param>
            void Delete(IProfile profile);

            /// <summary>
            ///   Returns the profile count.
            /// </summary>
            int Count { get; }

            /// <summary>
            ///   Returns a profile from the list by it's index.
            /// </summary>
            /// <seealso cref="IProfile interface"/>
            IProfile this[int index] { get; }
        }

        /// <summary>
        ///   Manage the Server's Binding configuration: protocol, IP addresses and
        ///   listen port.
        /// </summary>
        [Guid("52C63E8D-2FA4-4179-AFDB-2D33853F3356"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
        public interface IBinding
        {
            /// <summary>
            ///   Gets o sets the network protocol: HTTP or HTTPS. Uses the constants
            ///   PROTO_HTTP and PROTO_HTTPS.
            /// </summary>
            Protocol Protocol { get; set; }

            /// <summary>
            ///   Gets o sets the local IP Address. Leave empty to use all
            ///   addresses.
            /// </summary>
            string IPAddress { get; set; }

            /// <summary>
            ///   Gets o sets the listening port.
            /// </summary>
            int Port { get; set; }
        }

        /// <summary>
        ///   Manages the certificate's configuration for HTTPS Binding.
        /// </summary>
        [Guid("8B534446-EDC5-4EE7-91B0-13B5DACC5B51"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
        public interface ICertificate
        {
            /// <summary>
            /// Gets o sets the path of Certificate file.
            /// </summary>
            string CertFile { get; set; }

            /// <summary>
            ///   Gets o sets the path of Certificate Authority file.
            /// </summary>
            string CAFile { get; set; }

            /// <summary>
            ///   Gets o sets the path of Private Key file.
            /// </summary>
            string PKFile { get; set; }

            /// <summary>
            ///   Gets o sets the certificate password.
            /// </summary>
            string PassPhrase { get; set; }
        }

        /// <summary>
        ///   Manages the configuration of an alternative Remote Desktop Services
        ///   account.
        /// </summary>
        [Guid("103B86C8-E012-4AC7-A366-D3845BBB8D5E"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
        public interface IRDS
        {
            /// <summary>
            /// Gets o sets the RDS Username.
            /// </summary>
            string UserName { get; set; }

            /// <summary>
            ///   Gets o sets the RDS Password.
            /// </summary>
            string Password { get; set; }
        }

        /// <summary>
        ///   Manages the configuration of alternative Remote Desktop Services
        ///   accounts. VirtualUI makes use of an interactive session. The default
        ///   setting is to run applications under the console session, but it can be
        ///   configured to do it under Remote Desktop Services sessions. For the
        ///   production environment, it is recommended to set VirtualUI to run
        ///   applications under its own Remote Desktop Services session. This will
        ///   ensure that the service is available at all times. Alternatively, you
        ///   can choose to have VirtualUI run the applications under the console
        ///   session by configuring the Auto Logon feature on your Windows operating
        ///   system.
        /// </summary>
        [Guid("60666BC2-7E17-4842-9716-CFA3DCFD5583"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
        public interface IRDSAccounts
        {
            /// <summary>
            /// Creates a new RDS account and adds it to the list. If CreateAccount is true,
            /// the account will be created in your system.
            /// </summary>
            /// <returns>
            /// The newly created RDS account.
            /// </returns>
            /// <seealso cref="IRDS interface"/>
            IRDS Add(string UserName, string Password, bool CreateAccount);

            /// <summary>
            ///   Deletes an RDS account from the list. If DeleteAccount is true,
            ///   the account will be delete in your system.
            /// </summary>
            /// <param name="rds">
            ///   The account to be deleted.
            /// </param>
            bool Delete(string UserName, bool DeleteAccount);

            /// <summary>
            ///   Returns the accounts count.
            /// </summary>
            int Count { get; }

            /// <summary>
            ///   Returns an RDS account from the list by it's index.
            /// </summary>
            /// <seealso cref="IRDS interface/>
            IRDS this[int index] { get; }
        }

       /// <summary>
        ///   Manages the configuration of gateway servers.
        /// </summary>
        [Guid("716BBB17-7A57-46D1-93BB-2C8A947E1F6B"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
        public interface IGateways
        {
            /// <summary>
            /// Adds a new URL to the gateway list.
            /// </summary>
            /// <param name="URL">
            ///   The URL to be added.
            /// </param>
            void Add(string URL);

            /// <summary>
            ///   Deletes an URL from the list.
            /// </summary>
            /// <param name="index">
            ///   The index of the URL to be deleted.
            /// </param>
            void Delete(int index);

            /// <summary>
            ///   Returns the URLs count.
            /// </summary>
            int Count { get; }

            /// <summary>
            ///   Returns an URL from the list by it's index.
            /// </summary>
            string this[int index] { get; }
        }

        public class VirtualUISLibrary
        {
            [DllImport("kernel32.dll")]
            private static extern IntPtr LoadLibrary(string dllToLoad);
            [DllImport("kernel32.dll")]
            protected static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);
            [DllImport("kernel32.dll")]
            private static extern bool FreeLibrary(IntPtr hModule);
            [DllImport("kernel32")]
            private static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

            protected static IntPtr LibHandle = IntPtr.Zero;

            public VirtualUISLibrary()
            {
                if (LibHandle == IntPtr.Zero)
                {
                    string TargetDir = GetDLLDir();
                    if (TargetDir != null)
                    {
                        string LibFilename = TargetDir + @"\Thinfinity.VirtualUI.Settings.dll";
                        LibHandle = LoadLibrary(LibFilename);
                    }
                }
            }

            /// <summary>
            ///   Returns the path where Thinfinity.VirtualUI.Settings.dll is located.
            /// </summary>
            private static string GetDLLDir()
            {
                RegistryKey RegKey = null;
                string IniFileName = AppDomain.CurrentDomain.BaseDirectory + "\\OEM.ini";
                if (File.Exists(IniFileName))
                {
                    StringBuilder sbOEMKey32 = null;
                    StringBuilder sbOEMKey64 = null;
                    sbOEMKey32 = new StringBuilder(1024);
                    sbOEMKey64 = new StringBuilder(1024);
                    GetPrivateProfileString("PATHS", "Key32", "", sbOEMKey32, sbOEMKey32.Capacity, IniFileName);
                    GetPrivateProfileString("PATHS", "Key64", "", sbOEMKey64, sbOEMKey64.Capacity, IniFileName);
                    if (sbOEMKey32.ToString() != "" && RegKey == null)
                    {
                        string oemKey32 = sbOEMKey32.ToString();
                        if (oemKey32.StartsWith("\\"))
                            oemKey32 = oemKey32.Substring(1);
                        RegKey = Registry.LocalMachine.OpenSubKey(oemKey32, false);
                    }

                    if (sbOEMKey64.ToString() != "" && RegKey == null)
                    {
                        string oemKey64 = sbOEMKey64.ToString();
                        if (oemKey64.StartsWith("\\"))
                            oemKey64 = oemKey64.Substring(1);
                        RegKey = Registry.LocalMachine.OpenSubKey(oemKey64, false);
                    }
                }
                if (RegKey == null)
                    RegKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Cybele Software\Setups\Thinfinity\VirtualUI", false);
                if (RegKey == null)
                    RegKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Cybele Software\Setups\Thinfinity\VirtualUI", false);
                if (RegKey != null)
                {
                    if (IntPtr.Size == 8)
                        return (string)RegKey.GetValue("TargetDir_x64", null);
                    else
                        return (string)RegKey.GetValue("TargetDir_x86", null);
                }
                else return null;

            }
        }


        /// <summary>
        ///   Main class. Contains methods and properties to manage all Server
        ///   configuration.
        /// </summary>
        public class Server : VirtualUISLibrary, IServer, IDisposable
        {
            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate int funcGetInstance(ref IServer vui);
            private funcGetInstance GetInstance;
            private IServer m_Server;

            public Server()
                : base()
            {
                if (LibHandle != IntPtr.Zero)
                {
                    IntPtr pAddressOfFunctionToCall = GetProcAddress(LibHandle, "DllGetInstance");
                    GetInstance = (funcGetInstance)Marshal.GetDelegateForFunctionPointer(
                        pAddressOfFunctionToCall,
                        typeof(funcGetInstance));
                    GetInstance(ref m_Server);
                }
            }

            ~Server()
            {
                Dispose();
            }

            public void Dispose()
            {
                if (m_Server != null)
                {
                    Marshal.ReleaseComObject(m_Server);
                }
                m_Server = null;
            }
            
            /// <summary>
            ///   Returns the Server's Binding configuration.
            /// </summary>
            /// <seealso cref="IBinding interface" />
            public IBinding Binding
            {
                get
                {
                    if (m_Server != null)
                        return m_Server.Binding;
                    else
                        return null;
                }
            }
            /// <summary>
            ///   Returns the Server's certificate configuration for SSL protocol.
            /// </summary>
            /// <seealso cref="ICertificate interface"/>
            public ICertificate Certificate
            {
                get
                {
                    if (m_Server != null)
                        return m_Server.Certificate;
                    else
                        return null;
                }
            }

            /// <summary>
            ///   Returns the list of Remote Desktop Services accounts.
            /// </summary>
            /// <seealso cref="IRDSAccounts interface"/>
            public IRDSAccounts RDSAccounts
            {
                get
                {
                    if (m_Server != null)
                        return m_Server.RDSAccounts;
                    else
                        return null;
                }
            }

            /// <summary>
            ///   Returns the profiles list.
            /// </summary>
            /// <seealso cref="IProfiles interface"/>
            public IProfiles Profiles
            {
                get
                {
                    if (m_Server != null)
                        return m_Server.Profiles;
                    else
                        return null;
                }
            }

            /// <summary>
            ///   Returns the current Server's licence.
            /// </summary>
            /// <seealso cref="ILicense interface"/>
            public ILicense License
            {
                get
                {
                    if (m_Server != null)
                        return m_Server.License;
                    else
                        return null;
                }
            }

            /// <summary>
            ///   Returns the current Server's gateways.
            /// </summary>
            /// <seealso cref="IGateway interface"/>
            public IGateways Gateways
            {
                get
                {
                    if (m_Server != null)
                        return m_Server.Gateways;
                    else
                        return null;
                }
            }

            /// <summary>
            ///   Loads all the configuration entries and profiles from file.
            ///   It's automatically called by constructor.
            /// </summary>
            public void Load()
            {
                if (m_Server != null)
                    m_Server.Load();
            }

            /// <summary>
            ///   Saves the entire configuration parameters and profiles.
            /// </summary>
            public void Save()
            {
                if (m_Server != null)
                    m_Server.Save();
            }

            /// <summary>
            ///   Hides a configuration section in the VirtualUI Server Manager
            ///   GUI.
            /// </summary>
            /// <param name="section">The Server configuration section to
            ///                       hide to user. Use one of the following
            ///                       constants\:
            ///                       * SRVSEC_GENERAL\: Hides the General
            ///                         tab, that contains the Binding
            ///                         configuration.
            ///                       * SRVSEC_RDS\: Hides the tab with the
            ///                         Remote Desktop Services account
            ///                         configuration.
            ///                       * SRVSEC_APPLICATIONS\: Hides the list
            ///                         of applications.
            ///                       * SRVSEC_LICENSES\: Hides the tab with
            ///                         License information.
            /// </param>         
            public void HideSection(ServerSection section)
            {
                if (m_Server != null)
                    m_Server.HideSection(section);
            }

            /// <summary>
            ///   Makes visible a configuration section in the VirtualUI Server
            ///   Manager GUI.
            /// </summary>
            /// <param name="section">The Server configuration section to
            ///                       show to user. Use one of the following
            ///                       constants\:
            ///                       * SRVSEC_GENERAL\: Shows the General
            ///                         tab, that contains the Binding
            ///                         configuration.
            ///                       * SRVSEC_RDS\: Shows the tab with the
            ///                         Remote Desktop Services account
            ///                         configuration.
            ///                       * SRVSEC_APPLICATIONS\: Shows the list
            ///                         of applications.
            ///                       * SRVSEC_LICENSES\: Shows the tab with
            ///                         License information.
            /// </param>         
            public void ShowSection(ServerSection section)
            {
                if (m_Server != null)
                    m_Server.ShowSection(section);
            }

            /// <summary>
            ///   Global Network ID. All the Gateway and Server installations involved
            ///   in a Load Balancing architecture share the same network ID.
            /// </summary>
            public string NetworkID {
                get {
                    if (m_Server != null)
                        return m_Server.NetworkID;
                    else
                        return "";
                }
                set {
                    if (m_Server != null)
                        m_Server.NetworkID = value;
                }
            }
        }


        /// <summary>
        ///   Helper functions.
        /// </summary>
        public class ServerUtils
        {
            /// <summary>
            ///   Runs an application in elevated mode. This mode is required
            ///   to save the Server's configuration in protected files.
            /// </summary>
            /// <param name="filename">Full path of application to execute.</param>
            /// <param name="Parameters">\Arguments. </param>
            /// <example>
            ///   In the main program of the application using this classes, you
            ///   can include:
            ///         if (args.Length == 0)
            ///             ServerUtils.RunAsAdmin(Application.ExecutablePath, "/edit");
            ///         else {
            ///             [...]
            ///         }
            /// </example>                                                 
            public static void RunAsAdmin(string fileName, string parameters) {
                var proc = new System.Diagnostics.ProcessStartInfo();
                proc.UseShellExecute = true;
                proc.WorkingDirectory = Path.GetDirectoryName(fileName);
                proc.FileName = fileName;
                if (parameters != null && parameters.Length > 0) {
                    proc.Arguments = parameters;
                }
                proc.Verb = "runas";
                System.Diagnostics.Process.Start(proc);
            }

            /// <summary>
            ///   Converts the IProfile.IconData (base64 string) to a PNG image.
            /// </summary>
            /// <param name="data">
            ///   The image encoded in base64.
            /// </param>
            public static Image Base64ToIcon(string data)
            {
                Image res;
                byte[] bytes = Convert.FromBase64String(data);
                MemoryStream ms = new MemoryStream(bytes);
                res = Image.FromStream(ms);
                ms.Close();
                return res;
            }

            /// <summary>
            ///   Converts a PNG image to be stored in IProfile.IconData (as base64 string).
            /// </summary>
            /// param name="png">
            ///   The image to be encoded in base64.
            /// </param>
            public static string IconToBase64(Image png)
            {
                string res = "";
                MemoryStream ms = new MemoryStream();
                png.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] bytes = ms.ToArray();
                res = Convert.ToBase64String(bytes);
                ms.Close();
                return res;
            }
        }
    }
}