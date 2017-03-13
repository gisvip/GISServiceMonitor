using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ServiceInspection
{
    public class ServiceInspection : IServiceInspection
    {
        public bool CheckWmtsService(string url)
        {
            url += (ClearUrlParameters(url) + "?request=getcapabilities&service=wmts");

            bool result = this.CheckOGCCapabilities(url);
            int checkTimes = 3;

            //如果请求失败则重复请求最多三次
            while (!result && checkTimes > 0)
            {
                result = CheckOGCCapabilities(url);
                checkTimes--;
            }
            return result;
        }

        public bool CheckArcgisRestService(string url)
        {

            bool result = this.CheckArcGisRestCapabilities(url);
            int checkTimes = 3;

            //如果请求失败则重复请求最多三次
            while (!result && checkTimes > 0)
            {
                result = CheckArcGisRestCapabilities(url);
                checkTimes--;
            }

            return result;
        }

        public bool CheckWmsService(string url)
        {
            url += (ClearUrlParameters(url) + "?request=getcapabilities&service=wms");

            bool result = CheckOGCCapabilities(url);
            int checkTimes = 3;

            //如果请求失败则重复请求最多三次
            while (!result && checkTimes > 0)
            {
                result = CheckOGCCapabilities(url);
                checkTimes--;
            }
            return result;
        }

        /// <summary>
        /// 清除超链接参数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string ClearUrlParameters(string str)
        {
            if (str.IndexOf("?") != -1)
            {
                return str.Substring(str.IndexOf("?"), str.Length);
            }
            else
            {
                return str;
            }
        }

        /// <summary>
        /// 检测OGC的能力文档
        /// </summary>
        private bool CheckOGCCapabilities(string url)
        {
            try
            {
                byte[] bytes = GetResponse(url);
                string result = System.Text.UnicodeEncoding.UTF8.GetString(bytes);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(result);
                if (doc.DocumentElement.Name == "Capabilities")
                    return true;
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        /// <summary>
        /// 检测ArcgisRest的能力文档
        /// </summary>
        private bool CheckArcGisRestCapabilities(string url)
        {
            url += "?f=json";
            byte[] bytes = null;
            try
            {
                bytes = GetResponse(url);
                string result = System.Text.UnicodeEncoding.UTF8.GetString(bytes);
                if (result.Contains("currentVersion"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 网络请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public byte[] GetResponse(string url)
        {
            System.Net.HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Accept = "*/*";
            request.Timeout = 10000;
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    request.Referer = "http://" + _IPAddress.ToString();
                    break;
                }
            }
            request.KeepAlive = true;
            WebResponse response = request.GetResponse();
            Stream smRes = response.GetResponseStream();
            byte[] resBuf = new byte[10240];
            int nReaded = 0;
            MemoryStream memSm = new MemoryStream();
            while ((nReaded = smRes.Read(resBuf, 0, 10240)) != 0)
            {
                memSm.Write(resBuf, 0, nReaded);
            }
            byte[] byResults = memSm.ToArray();
            memSm.Close();
            memSm.Dispose();
            return byResults;
        }
    }
}
