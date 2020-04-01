using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Collections.Generic;

namespace System.Net
{

    public static class UserAgents
    {
        public const string Baiduspider = "Baiduspider+(+http://www.baidu.com/search/spider.htm)";
        public const string Googlebot = "Googlebot/2.1 (+http://www.google.com/bot.html)";

        public const string Chrome = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.11(KHTML, like Gecko) Chrome/23.0.1271.64 Safari/537.11";
        public const string Firefox = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:69.0) Gecko/20100101 Firefox/69.0";
        public const string Safari = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/534.50 (KHTML, like Gecko) Version/5.1 Safari/534.50";
        public const string IE = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Win64; x64; Trident/5.0; .NET CLR 2.0.50727; SLCC2; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.3; .NET4.0C; Tablet PC 2.0; .NET4.0E)";


        public const string Andorid = "Mozilla/5.0 (Linux; U; Android 2.2.1; zh-cn; HTC_Wildfire_A3333 Build/FRG83D) AppleWebKit/533.1 (KHTML, like Gecko) Version/4.0 Mobile Safari/533.1";
        public const string IPhone = "Mozilla/5.0 (iPhone; CPU iPhone OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A403 Safari/8536.25";
        public const string IPad = "Mozilla/5.0 (iPad; CPU OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A403 Safari/8536.25";
        public const string WeixinIPhone = "Mozilla/5.0 (iPhone; CPU iPhone OS 5_1 like Mac OS X) AppleWebKit/534.46 (KHTML, like Gecko) Mobile/9B176 MicroMessenger/4.3.2";
        public const string WeixinAndroid = "Mozilla/5.0 (Linux; U; Android 2.3.6; zh-cn; GT-S5660 Build/GINGERBREAD) AppleWebKit/533.1 (KHTML, like Gecko) Version/4.0 Mobile Safari/533.1 MicroMessenger/4.5.255";
    }

    [ToolboxItem(false)]
    public class WebClientEx : WebClient
    {
        private string _userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:69.0) Gecko/20100101 Firefox/69.0";
        private string _acceptLanguage = "zh-CN,zh;";
        private bool _useCookie = true;
        public CookieContainer Cookies;
        private WebProxy _proxy;

        private int? _timeout;
        private int? _readWriteTimeout;
        private int? _continueTimeout;


        static WebClientEx()
        {
            ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;
        }
        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public WebClientEx()
        {
            Cookies = new CookieContainer();
            Headers.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate,br");
            Headers.Add(HttpRequestHeader.AcceptLanguage, "zh-CN,zh;q=0.8");
            Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:69.0) Gecko/20100101 Firefox/69.0");
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            if (address.IdnHost != address.Host) {
                var url = address.Scheme + "://" + address.IdnHost + address.PathAndQuery;
                address = new Uri(url);
            }

            //if (address.ToString().StartsWith("https")) {
            //    Credentials = CredentialCache.DefaultNetworkCredentials;
            //    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            //    ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;
            //}

            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            if (_useCookie) request.CookieContainer = Cookies;
            request.AllowAutoRedirect = true;
            if (_proxy != null) request.Proxy = this.Proxy;
            if (_timeout != null) request.Timeout = (int)_timeout * 1000;
            if (_readWriteTimeout != null) request.ReadWriteTimeout = (int)_readWriteTimeout * 1000;
            if (_continueTimeout != null) request.ContinueTimeout = (int)_continueTimeout * 1000;

            return request;
        }



        #region 02 扩展 上传方法 和 下载图片方法

        public byte[] PostForm(string url, Dictionary<string, string> dict)
        {
            var str = "";
            foreach (var item in dict) {
                if (str.Length > 0) { str += "&"; }
                str += item.Key + "=" + System.Web.HttpUtility.UrlEncode(item.Value);
            }
            var postData = Encoding.ASCII.GetBytes(str);
            this.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            return this.UploadData(url, "POST", postData);
        }


        public byte[] PostForm(string url, string formData)
        {
            this.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            var postData = Encoding.ASCII.GetBytes(formData);
            return this.UploadData(url, "POST", postData);
        }

        public byte[] PostForm(string url, byte[] formData)
        {
            this.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            return this.UploadData(url, "POST", formData);
        }

        #endregion 02 扩展 上传方法 和 下载图片方法

        #region 03 加载 网页前操作
        /// <summary>
        /// 设置UserAgent
        /// </summary>
        /// <param name="ua"></param>
        public void SetUserAgent(string ua)
        {
            _userAgent = ua;
        }

        /// <summary>
        /// 重置 头部
        /// </summary>
        public void ResetHeaders()
        {
            Headers.Clear();
            Headers.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            Headers.Add(HttpRequestHeader.AcceptLanguage, _acceptLanguage);
            Headers.Add(HttpRequestHeader.UserAgent, _userAgent);
        }

        public void SetTimeOut(int timeout)
        {
            this._timeout = timeout;
        }

        public void RemoveTimeOut()
        {
            this._timeout = null;
        }

        /// <summary>
        /// 使用
        /// </summary>
        /// <param name="timeout"></param>
        public void SetReadWriteTimeout(int timeout)
        {
            this._readWriteTimeout = timeout;
        }
        public void RemoveReadWriteTimeout()
        {
            this._readWriteTimeout = null;
        }

        public void SetContinueTimeout(int timeout)
        {
            this._continueTimeout = timeout;
        }

        public void RemoveContinueTimeout()
        {
            this._continueTimeout = null;
        }

        /// <summary>
        /// 使用 引用
        /// </summary>
        /// <param name="url"></param>
        public void SetReferer(string url)
        {
            if (url == "") {
                this.Headers.Remove(HttpRequestHeader.Referer);
            } else {
                this.Headers[HttpRequestHeader.Referer] = url;
            }
        }

        /// <summary>
        /// 使用 代理
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="proxy"></param>
        /// <param name="userName"></param>
        /// <param name="userPass"></param>
        public void SetWebProxy(string ip, int proxy, string userName = "", string userPass = "")
        {
            ip = ip.Trim();
            userName = userName.Trim();
            userPass = userPass.Trim();
            _proxy = new WebProxy(ip, proxy);
            if (userName.Length != 0 && userPass.Length != 0)
                _proxy.Credentials = new NetworkCredential(userName, userPass);
        }

        /// <summary>
        /// 移除 代理
        /// </summary>
        public void RemoveWebProxy()
        {
            this._proxy = null;
        }

        #endregion 03 加载 网页前操作

        #region 04 Cookie 操作
        /// <summary>
        /// 关闭 cookie
        /// </summary>
        public void CookieClose()
        {
            _useCookie = false;
        }

        /// <summary>
        /// 开启cookie
        /// </summary>
        public void CookieOpen()
        {
            _useCookie = true;
        }

        /// <summary>
        /// 获取所有 Cookie 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetCookie(string url)
        {
            StringBuilder sb = new StringBuilder();
            try {
                Uri u = new Uri(url);
                foreach (Cookie cook in Cookies.GetCookies(u)) {
                    sb.Append(cook.ToString());
                    sb.Append("; ");
                }
            } catch { }
            return sb.ToString();
        }

        /// <summary>
        /// 获取 Cookie
        /// </summary>
        /// <param name="url"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetCookieValue(string url, string key)
        {
            Uri u = new Uri(url);
            foreach (Cookie cook in Cookies.GetCookies(u)) {
                if (cook.Name == key) {
                    return cook.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// 添加 Cookie
        /// </summary>
        /// <param name="url"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddCookie(string url, string key, string value)
        {
            var uri = new Uri(url);
            if (uri.Port == 80) {
                var u = uri.Scheme + "://" + uri.Host + "/";
                Cookies.Add(new Uri(u), new Cookie(key, value));
            } else {
                var u = uri.Scheme + "://" + uri.Host + ":" + uri.Port + "/";
                Cookies.Add(new Uri(u), new Cookie(key, value));
            }
        }

        /// <summary>
        /// 导出Cookie 类型
        /// </summary>
        /// <returns></returns>
        public byte[] GetCookieBytes()
        {
            using (MemoryStream ms = new MemoryStream()) {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, Cookies);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// cookie 为 GetCookieBytes（）导出Cookie类型
        /// </summary>
        /// <param name="cookie"></param>
        public void SetCookieBytes(byte[] cookie)
        {
            using (MemoryStream ms = new MemoryStream()) {
                ms.Write(cookie, 0, cookie.Length);
                BinaryFormatter formatter = new BinaryFormatter();
                Cookies = (CookieContainer)formatter.Deserialize(ms);
            }
        }

        /// <summary>
        /// 保存  Cookie 
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveCookies(string fileName)
        {
            using (Stream stream = File.Create(fileName)) {
                try {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, Cookies);
                } catch (Exception) { }
            }
        }

        /// <summary>
        /// 加载 Cookie
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadCookies(string fileName)
        {
            try {
                using (Stream stream = File.Open(fileName, FileMode.Open)) {
                    BinaryFormatter formatter = new BinaryFormatter();
                    Cookies = (CookieContainer)formatter.Deserialize(stream);
                }
            } catch (Exception) {
                Cookies = new CookieContainer();
            }
        }

        #endregion 04 Cookie 操作
    }


}
