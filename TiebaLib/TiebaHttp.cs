﻿using CsharpHttpHelper;
using CsharpHttpHelper.Enum;

namespace TiebaLib
{
    internal static class TiebaHttp
    {
        public static string Get(string url, string cookie = "")
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = url,//URL     必需项
                Method = "GET",//URL     可选项 默认为Get
                Timeout = 100000,//连接超时时间     可选项默认为100000
                ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写
                Cookie = cookie,//字符串Cookie     可选项
                UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0",//用户的浏览器类型，版本，操作系统     可选项有默认值
                Accept = "text/html, application/xhtml+xml, */*",//    可选项有默认值
                ContentType = "text/html",//返回类型    可选项有默认值
                Referer = "",//来源URL     可选项
                Allowautoredirect = false,//是否根据３０１跳转     可选项
                AutoRedirectCookie = false,//是否自动处理Cookie     可选项
                Postdata = "",//Post数据     可选项GET时不需要写
                ResultType = ResultType.String,//返回数据类型，是Byte还是String
            };
            HttpResult result = http.GetHtml(item);
            return result.Html;
        }

        public static string Post(string url, string postData, string cookie = "")
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = url,//URL     必需项
                Method = "POST",//URL     可选项 默认为Get
                Timeout = 100000,//连接超时时间     可选项默认为100000
                ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写
                Cookie = cookie,//字符串Cookie     可选项
                UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0",//用户的浏览器类型，版本，操作系统     可选项有默认值
                Accept = "text/html, application/xhtml+xml, */*",//    可选项有默认值
                ContentType = "text/html",//返回类型    可选项有默认值
                Referer = "",//来源URL     可选项
                Allowautoredirect = false,//是否根据３０１跳转     可选项
                AutoRedirectCookie = false,//是否自动处理Cookie     可选项
                Postdata = postData,//Post数据     可选项GET时不需要写
                ResultType = ResultType.String,//返回数据类型，是Byte还是String
            };
            HttpResult result = http.GetHtml(item);
            return result.Html;
        }
    }
}
