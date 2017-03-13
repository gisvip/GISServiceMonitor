using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInspection
{
    public interface IServiceInspection
    {
        /// <summary>
        /// 检查WMTS服务
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        bool CheckWmtsService(string url);

        /// <summary>
        /// 检查arcgisrest服务
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        bool CheckArcgisRestService(string url);

        /// <summary>
        /// 检查WMS服务
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        bool CheckWmsService(string url);
    }
}
