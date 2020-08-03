using BakaSnowTool;
using BakaSnowTool.Http;
using Newtonsoft.Json.Linq;
using System;

namespace TiebaLib
{
    public class TiebaQianDao
    {
        public string Cookie;

        /// <summary>
        /// 客户端签到
        /// </summary>
        /// <param name="tiebaName">贴吧名</param>
        public bool KeHuDuanQianDao(string tiebaName, out string msg)
        {
            string url = "http://c.tieba.baidu.com/c/c/forum/sign";
            string postStr
                = Cookie
                + "&_client_id=" + Tieba.GetAndroidStamp()
                + "&_client_type=2"
                + "&_client_version=5.3.1"
                //+ "&_phone_imei=042791438690445"
                //+ "&cuid=DCE2BCBBC5F4307F7457E2463A14F382%7C544096834197240"
                //+ "&from=baidu_appstore"
                + "&kw=" + Http.UrlEncodeUtf8(tiebaName)
                //+ "&model=GT-I9100"
                //+ "&stErrorNums=0"
                //+ "&stMethod=1"
                //+ "&stMode=1"
                //+ "&stSize=64923"
                //+ "&stTime=780"
                //+ "&stTimesNum=0"
                + "&tbs=" + Tieba.GetBaiduTbs(Cookie);
            //+ "&timestamp=1388306178920";

            postStr += "&sign=" + Tieba.GetTiebaSign(postStr);

            string html = TiebaHttp.Post(url, postStr, Cookie);

            //可能是网络故障
            if (string.IsNullOrEmpty(html))
            {
                msg = "网络异常";
                return false;
            }

            //解析
            JObject huiFuJsonData;
            try
            {
                huiFuJsonData = JObject.Parse(html);
            }
            catch
            {
                msg = "Json解析失败";
                return false;
            }

            //访问失败
            msg = huiFuJsonData["error_msg"]?.ToString();
            if (huiFuJsonData["error_code"]?.ToString() != "0")
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 一键签到
        /// </summary>
        public bool YiJianQianDao(out string msg)
        {
            string url = "http://c.tieba.baidu.com/c/c/forum/msign";
            string postStr
                = Cookie
                + "&_client_id=" + Tieba.GetAndroidStamp()
                + "&_client_type=2"
                + "&_client_version=5.3.1"
                //+ "&_phone_imei=042791438690445"
                //+ "&cuid=DCE2BCBBC5F4307F7457E2463A14F382%7C544096834197240"
                //+ "&forum_ids=1938496%2C1929829"
                //+ "&from=baidu_appstore"
                //+ "&model=GT-I9100"
                //+ "&stErrorNums=0"
                //+ "&stMethod=1"
                //+ "&stMode=1"
                //+ "&stSize=138"
                //+ "&stTime=120"
                //+ "&stTimesNum=0"
                + "&tbs=" + Tieba.GetBaiduTbs(Cookie);
            //+ "&timestamp=1388304097180"
            //+ "&user_id=16303";

            postStr += "&sign=" + Tieba.GetTiebaSign(postStr);

            string html = TiebaHttp.Post(url, postStr, Cookie);

            Console.WriteLine(BST.DeUnicode(html));

            //可能是网络故障
            if (string.IsNullOrEmpty(html))
            {
                msg = "网络异常";
                return false;
            }

            //解析
            JObject huiFuJsonData;
            try
            {
                huiFuJsonData = JObject.Parse(html);
            }
            catch
            {
                msg = "Json解析失败";
                return false;
            }

            //访问失败
            msg = huiFuJsonData["error_msg"]?.ToString();
            if (huiFuJsonData["error_code"]?.ToString() != "0")
            {
                return false;
            }

            return true;
        }
    }
}
