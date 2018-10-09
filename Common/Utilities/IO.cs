using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common
{
    class IO
    {
        static readonly byte[] salt = System.Text.Encoding.Unicode.GetBytes("Poorly Protected");

        #region HTTP Client declarations
        // httpClientHandler must preceed httpClient declaration! 
        private static HttpClientHandler httpClientHandler = new HttpClientHandler()
        {
            Credentials = new NetworkCredential(
                SettingsVM.Instance.UserSettings.Username,
                ToInsecureString(DecryptString(SettingsVM.Instance.UserSettings.Password))
            )
        };
        private static HttpClient httpClient = new HttpClient(httpClientHandler);
        #endregion

        public static string ConstructLogin()
        {
            string loginUrl = SettingsVM.Instance.UserSettings.Url + "/ibmcognos/cgi-bin/cognos.cgi?dsn=" + SettingsVM.Instance.UserSettings.Dsn + "&CAMNamespace=esp&b_action=xts.run&m=portal/cc.xts&gohome=";
            return loginUrl;
        }

        public static async Task DownloadFile(string url, string downloadPath)
        {
            using (HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            using (Stream readStream = await response.Content.ReadAsStreamAsync())
            {
                using (Stream writeStream = File.Open(downloadPath, FileMode.Create))
                {
                    await readStream.CopyToAsync(writeStream);
                }
            }
        }

        public static string ExtractDownloadLink(string html)
        {
            var regex = new Regex(@"var sURL = '(.*?)'");

            if (regex.IsMatch(html))
            {
                Match submatch = regex.Match(html);
                return SettingsVM.Instance.UserSettings.Url + submatch.Groups[1].Value;
            }
            else
            {
                return "ERROR";
            }
        }

        public static string GetJsonValue(string key, string data)
        {
            var regex = new Regex("\"" + key + "\\\":\\s\"(.*?)\"");

            if (regex.IsMatch(data))
            {
                Match submatch = regex.Match(data);
                return submatch.Groups[1].Value;
            } else
            {
                return "ERROR";
            }
        }

        public static FormUrlEncodedContent GetPostData(string result)
        {
            var b_action = GetJsonValue("b_action", result);
            var cv_actionState = GetJsonValue("m_sActionState", result);
            var cv_id = GetJsonValue("cv.id", result);
            var cv_objectPermissions = GetJsonValue("cv.objectPermissions", result);
            var executionParameters = GetJsonValue("m_sParameters", result);
            var m_tracking = GetJsonValue("m_sTracking", result);
            var ui_cafcontextid = GetJsonValue("m_sCAFContextid", result);
            var ui_conversation = GetJsonValue("m_sConversation", result);
            var ui_object = GetJsonValue("ui.object", result);
            var ui_objectClass = GetJsonValue("ui.objectClass", result);
            var ui_primaryAction = GetJsonValue("ui.primaryAction", result);

            var values = new Dictionary<string, string>
                    {
                        { "b_action", b_action },
                        { "cv.actionState", cv_actionState },
                        { "cv.catchLogOnFault", "true" },
                        { "cv.id", cv_id },
                        { "cv.objectPermissions", cv_objectPermissions },
                        { "cv.responseFormat", "data" },
                        { "cv.showFaultPage", "true" },
                        { "executionParameters", executionParameters },
                        { "m_tracking", m_tracking },
                        { "ui.action", "wait" },
                        { "ui.cafcontextid", ui_cafcontextid },
                        { "ui.conversation", ui_conversation },
                        { "ui.object", ui_object },
                        { "ui.objectClass", ui_objectClass },
                        { "ui.primaryAction", ui_primaryAction }
                    };
            var content = new FormUrlEncodedContent(values);
            
            return content;
        }

        public static async Task<bool> HttpConnect(string url)
        {
            HttpResponseMessage response = await httpClient.GetAsync(url);
            return response.IsSuccessStatusCode;
        }

        public static async Task<String> HttpGo(string path)
        {
            string returnData = await httpClient.GetStringAsync(path);
            return returnData;
        }

        public static async Task<String> HttpPost(string url, HttpContent content)
        {
            var response = await httpClient.PostAsync(url, content);
            string returnData = await response.Content.ReadAsStringAsync();
            return returnData;
        }

        public static bool VerifyUserSettings()
        {
            if (string.IsNullOrWhiteSpace(SettingsVM.Instance.UserSettings.Username)) { return false; }
            else if (string.IsNullOrWhiteSpace(SettingsVM.Instance.UserSettings.Password)) { return false; }
            else if (string.IsNullOrWhiteSpace(SettingsVM.Instance.UserSettings.Dsn)) { return false; }
            else if (string.IsNullOrWhiteSpace(SettingsVM.Instance.UserSettings.Url)) { return false; }
            else { return true; }
        }

        public static string RefactorUrl(string inputUrl)
        {
            string newUrl = null;
            string decodedUrl = Uri.UnescapeDataString(inputUrl);
            string[] elements = decodedUrl.Split('&');

            foreach (var element in elements.Select((value, index) => new { value, index }))
                if (element.value.Substring(0, 11) == "run.prompt=")
                {
                    newUrl = newUrl + "&run.prompt=false";
                }
                else
                {
                    if (element.index == 0)
                    {
                        newUrl = newUrl + element.value;
                    }
                    else
                    {
                        newUrl = newUrl + "&" + element.value;
                    }
                }

            if (SettingsVM.Instance.UserSettings.OverwriteUrl)
            {
                Uri uri = new Uri(newUrl);
                Uri settingsUrl = new Uri(SettingsVM.Instance.UserSettings.Url);
                string protocol = settingsUrl.Scheme;
                string host = settingsUrl.Host;
                string path = uri.PathAndQuery;
                newUrl = protocol + "://" + host + path;
            }
            return newUrl;
        }

        public static SecureString ToSecureString(string input)
        {
            SecureString secureString = new SecureString();

            foreach (char c in input) { secureString.AppendChar(c); }
            secureString.MakeReadOnly();
            return secureString;
        }

        public static string ToInsecureString(SecureString input)
        {
            string value = string.Empty;
            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(input);

            try { value = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr); }
            finally { System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr); }

            return value;
        }

        public static SecureString DecryptString(string encryptedData)
        {
            try
            {
                byte[] decryptedData = System.Security.Cryptography.ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedData),
                    salt,
                    System.Security.Cryptography.DataProtectionScope.CurrentUser);
                return ToSecureString(System.Text.Encoding.Unicode.GetString(decryptedData));
            }
            catch { return new SecureString(); }
        }

        public static string EncryptString(System.Security.SecureString input)
        {
            byte[] encryptedData = System.Security.Cryptography.ProtectedData.Protect(
                System.Text.Encoding.Unicode.GetBytes(ToInsecureString(input)),
                salt,
                System.Security.Cryptography.DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedData);
        }
    }
}