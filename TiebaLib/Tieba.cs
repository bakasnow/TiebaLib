using BakaSnowTool;
using BakaSnowTool.Http;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TiebaLib
{
    public class Tieba
    {
        #region 静态
        /// <summary>
        /// 取安卓Stamp
        /// </summary>
        /// <returns></returns>
        public static string GetAndroidStamp()
        {
            //wappc_1584510405614_799

            Random ra = new Random();

            int[] stamp = new int[4];
            stamp[0] = ra.Next(10000, 99999);
            stamp[1] = ra.Next(1000, 9999);
            stamp[2] = ra.Next(1000, 9999);
            stamp[3] = ra.Next(100, 999);

            return "wappc_" + stamp[0] + stamp[1] + stamp[2] + "_" + stamp[3];
        }

        /// <summary>
        /// 取百度Tbs
        /// </summary>
        /// <param name="cookie">cookie</param>
        /// <returns></returns>
        public static string GetBaiduTbs(string cookie)
        {
            string html = TiebaHttp.Get("http://tieba.baidu.com/dc/common/tbs", cookie);
            return BST.JieQuWenBen(html, "\"tbs\":\"", "\"");
        }

        /// <summary>
        /// 获取百度账号在线状态
        /// </summary>
        /// <param name="cookie">cookie</param>
        /// <returns></returns>
        public static bool GetBaiduZhangHaoIsZaiXian(string cookie)
        {
            string html = TiebaHttp.Get("http://tieba.baidu.com/dc/common/tbs", cookie);
            return BST.JieQuWenBen(html, "\"is_login\":", "}") == "1";
        }

        /// <summary>
        /// 取贴吧Fid
        /// </summary>
        /// <param name="tiebaName">贴吧名</param>
        /// <returns></returns>
        public static string GetTiebaFid(string tiebaName)
        {
            string html = TiebaHttp.Get($"http://tieba.baidu.com/f/commit/share/fnameShareApi?fname={Http.UrlEncodeUtf8(tiebaName)}&ie=utf-8");
            return BST.JieQuWenBen(html, "\"fid\":", ",");
        }

        /// <summary>
        /// 取百度用户名
        /// </summary>
        /// <param name="cookie">cookie</param>
        /// <returns></returns>
        public static string GetBaiduYongHuMing(string cookie)
        {
            string html = TiebaHttp.Get("http://tieba.baidu.com/f/user/json_userinfo", cookie);
            return BST.DeUnicode(BST.JieQuWenBen(html, "user_name_weak\":\"", "\""));
        }

        /// <summary>
        /// 取百度Uid
        /// </summary>
        /// <param name="yongHuMing">用户名</param>
        /// <returns></returns>
        public static string GetBaiduUid(string yongHuMing)
        {
            string html = TiebaHttp.Get("http://tieba.baidu.com/i/sys/user_json?un=" + Http.UrlEncode(yongHuMing));
            string uid = BST.JieQuWenBen(html, "\"id\":", ",\"");
            if (uid == "0")
            {
                html = TiebaHttp.Get("http://tieba.baidu.com/home/main?un=" + Http.UrlEncode(yongHuMing) + "&fr=home");
                uid = BST.JieQuWenBen(html, "home_user_id\" : ", ",");
            }
            return uid;
        }

        /// <summary>
        /// 取贴吧Uid
        /// </summary>
        /// <param name="yongHuMing">用户名</param>
        /// <returns></returns>
        public static string GetTiebaUid(string yongHuMing)
        {
            string html = TiebaHttp.Get("http://tieba.baidu.com/i/sys/user_json?un=" + Http.UrlEncode(yongHuMing));
            return BST.JieQuWenBen(html, "\"id\":", ",");
        }

        /// <summary>
        /// 取贴吧Sign
        /// </summary>
        /// <param name="str">文本</param>
        /// <returns></returns>
        public static string GetTiebaSign(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }

            MD5 algorithm = MD5.Create();
            byte[] data = algorithm.ComputeHash(Encoding.UTF8.GetBytes(Http.UrlDecodeUtf8(str.Replace("&", "") + "tiebaclient!!!")));
            algorithm.Dispose();

            string md5 = string.Empty;
            for (int i = 0; i < data.Length; i++)
            {
                md5 += data[i].ToString("x2").ToUpperInvariant();
            }

            return md5;
        }

        /// <summary>
        /// 获取吧务团队
        /// </summary>
        /// <param name="tiebaName"></param>
        /// <param name="quChongFu"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static List<BaWuTuanDuiJieGou> GetBaWuTuanDui(string tiebaName, out string msg)
        {
            List<BaWuTuanDuiJieGou> baWuTuanDuiLieBiao = new List<BaWuTuanDuiJieGou>();

            string html = TiebaHttp.Get("http://tieba.baidu.com/bawu2/platform/listBawuTeamInfo?word=" + Http.UrlEncode(tiebaName));
            if (string.IsNullOrEmpty(html))
            {
                msg = "网络错误";
                return baWuTuanDuiLieBiao;
            }

            const string wenBenTou = "<div class=\"bawu_team_wrap\">";
            const string wenBenWei = "<div id=\"footer\" class=\"footer\">";

            //将吧务团队列表的源码过滤出来
            string baWuTuanDuiHtml = BST.JieQuWenBen(html, wenBenTou, wenBenWei);
            baWuTuanDuiHtml = wenBenTou + baWuTuanDuiHtml + wenBenWei;
            //File.WriteAllText(@"C:\Users\bakas\Desktop\test1.html", baWuTuanDuiHtml);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(baWuTuanDuiHtml);

            HtmlNodeCollection bawu_team_wrap_list = doc.DocumentNode?.SelectNodes("div[1]/div");
            if (bawu_team_wrap_list == null)
            {
                msg = "Html解析失败1";
                return baWuTuanDuiLieBiao;
            }

            foreach (var bawu_single_type in bawu_team_wrap_list)
            {
                //职务标题
                HtmlNode bawu_single_type_title = bawu_single_type?.SelectSingleNode("div[1]");
                if (bawu_single_type_title == null) continue;

                //职务
                string zhiWu = bawu_single_type_title.InnerText;

                //职务列表
                HtmlNodeCollection member_first_row_list = bawu_single_type?.SelectNodes("div[2]/span");
                if (member_first_row_list == null) continue;

                foreach (var member_first_row in member_first_row_list)
                {
                    //头像
                    string touXiang = member_first_row?.SelectSingleNode("a[@class='avatar']")?.SelectSingleNode("img")?.Attributes["src"]?.Value;

                    //过滤头像链接
                    if (touXiang.Contains("/"))
                    {
                        string[] touXiangList = touXiang.Split('/');
                        touXiang = touXiangList[touXiangList.Length - 1];
                    }

                    //用户名
                    string yongHuMing = member_first_row?.SelectSingleNode("a[@class='user_name']")?.Attributes["title"]?.Value;

                    baWuTuanDuiLieBiao.Add(new BaWuTuanDuiJieGou
                    {
                        ZhiWu = zhiWu,
                        YongHuMing = yongHuMing,
                        TouXiang = touXiang
                    });
                }
            }

            msg = "获取成功";
            return baWuTuanDuiLieBiao;
        }

        /// <summary>
        /// 获取贴吧关注列表
        /// </summary>
        /// <param name="yonghuming">用户名</param>
        /// <returns></returns>
        public static List<GuanZhuJieGou> GetTiebaGuanZhuLieBiao(string cookie, string yongHuMing, int page = 1)
        {
            List<GuanZhuJieGou> tiebaGuanZhuLieBiao = new List<GuanZhuJieGou>();

            string uid = GetTiebaUid(yongHuMing);
            //string url = "http://c.tieba.baidu.com/c/f/forum/like";
            //string postStr
            //    = cookie
            //    + "&_client_id=" + GetAndroidStamp()
            //    + "&_client_type=2"
            //    + "&_client_version=9.9.8.32"
            //    + "&friend_uid=" + uid
            //    + "&uid=" + uid;

            string url = "http://c.tieba.baidu.com/c/f/forum/like";
            string postStr
                = cookie
                + "&_client_id=" + GetAndroidStamp()
                //+ "&_client_id=wappc_1584510405614_799"
                + "&_client_type=2"
                + "&_client_version=9.9.8.32"
                + "&_phone_imei=450456461928196"
                + "&cuid=00DC23509DCDF63928D194FD8D41703A%7CVRO6PAJEL"
                + "&cuid_galaxy2=00DC23509DCDF63928D194FD8D41703A%7CVRO6PAJEL"
                + "&cuid_gid="
                + "&friend_uid=" + uid
                + "&from=1019960r"
                + "&is_guest=1"
                + "&model=MI+6"
                + "&net_type=1"
                + "&oaid=%7B%22sc%22%3A0%2C%22sup%22%3A0%2C%22tl%22%3A0%7D"
                + "&page_no=" + page
                + "&page_size=50"
                + "&stErrorNums=1"
                + "&stMethod=1"
                + "&stMode=1"
                + "&stSize=5061"
                + "&stTime=667"
                + "&stTimesNum=1"
                + "&timestamp=1584512309167"
                + "&uid=0";

            postStr += "&sign=" + GetTiebaSign(postStr);

            string html = TiebaHttp.Post(url, postStr, cookie);
            if (string.IsNullOrEmpty(html))
            {
                return tiebaGuanZhuLieBiao;
            }

            //解析
            JObject htmlJson;
            try
            {
                htmlJson = JObject.Parse(html);
            }
            catch
            {
                return tiebaGuanZhuLieBiao;
            }

            if (htmlJson["error_code"]?.ToString() != "0")
            {
                return tiebaGuanZhuLieBiao;
            }

            //{"server_time":"50714","time":1584513751,"ctime":0,"logid":2551816537,"error_code":"0"}

            if (!htmlJson.ContainsKey("forum_list"))
            {
                return tiebaGuanZhuLieBiao;
            }

            var non_gconforum = htmlJson["forum_list"]?["non-gconforum"];
            foreach (var ng in non_gconforum)
            {
                GuanZhuJieGou guanZhuJieGou = new GuanZhuJieGou
                {
                    TiebaName = ng["name"]?.ToString()
                };
                long.TryParse(ng["id"]?.ToString(), out guanZhuJieGou.Fid);
                int.TryParse(ng["level_id"]?.ToString(), out guanZhuJieGou.DengJi);
                int.TryParse(ng["cur_score"]?.ToString(), out guanZhuJieGou.JingYanZhi);

                tiebaGuanZhuLieBiao.Add(guanZhuJieGou);
            }

            return tiebaGuanZhuLieBiao;
        }

        /// <summary>
        /// 获取用户贴吧等级
        /// </summary>
        /// <param name="yongHuMing">用户名</param>
        /// <param name="tiebaName">贴吧名</param>
        /// <returns></returns>
        public static int GetYongHuTiebaDengJi(string cookie, string yongHuMing, string tiebaName)
        {
            List<GuanZhuJieGou> liebiao = GetTiebaGuanZhuLieBiao(cookie, yongHuMing);

            List<GuanZhuJieGou> jieGuo = liebiao.Where(x => (x.TiebaName == tiebaName)).ToList();
            if (jieGuo.Count > 0)
            {
                return jieGuo[0].DengJi;
            }

            return -1;
        }

        /// <summary>
        /// 获取贴吧名片
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static MingPianJieGou GetTiebaMingPian(string id)
        {
            MingPianJieGou mingPianJieGou = new MingPianJieGou();

            string canShu = $"&un={Http.UrlEncodeUtf8(id)}";
            if (id.Length >= 20)
            {
                canShu = $"&id={id}";
            }

            string url = $"http://tieba.baidu.com/home/get/panel?ie=utf-8{canShu}";
            Console.WriteLine(url);
            string html = TiebaHttp.Get(url);
            if (string.IsNullOrEmpty(html))
            {
                mingPianJieGou.Msg = "网络异常";
                return mingPianJieGou;
            }

            JObject jObject;
            try
            {
                jObject = JObject.Parse(html);
            }
            catch
            {
                mingPianJieGou.Msg = "Json解析失败";
                return mingPianJieGou;
            }

            mingPianJieGou.Msg = jObject["error"]?.ToString();
            if (jObject["no"]?.ToString() != "0")
            {
                return mingPianJieGou;
            }

            long.TryParse(jObject["data"]?["id"]?.ToString(), out mingPianJieGou.Uid);
            mingPianJieGou.YongHuMing = jObject["data"]?["name"]?.ToString();
            mingPianJieGou.NiCheng = jObject["data"]?["name_show"]?.ToString();
            mingPianJieGou.TouXiang = jObject["data"]?["portrait_h"]?.ToString();

            mingPianJieGou.HuoQuChengGong = true;
            return mingPianJieGou;
        }
        #endregion

        #region 结构
        /// <summary>
        /// UID结构
        /// </summary>
        public class UidJieGou
        {
            public long Uid;
            public string YongHuMing;//用户名
            public string NiCheng;//昵称
            public string TouXiang;//头像
            public int DengJi;//等级
            public bool IsBaWu;//是否吧务
            public TiebaYinJi YinJi;//印记
        }

        /// <summary>
        /// 吧务团队结构
        /// </summary>
        public class BaWuTuanDuiJieGou
        {
            public string ZhiWu;
            public string YongHuMing;
            public string TouXiang;
        }

        /// <summary>
        /// 关注结构
        /// </summary>
        public class GuanZhuJieGou
        {
            public string TiebaName;
            public long Fid;
            public int DengJi;
            public int JingYanZhi;
        }

        /// <summary>
        /// 名片结构
        /// </summary>
        public class MingPianJieGou
        {
            public bool HuoQuChengGong;
            public string Msg;

            public long Uid;
            public string YongHuMing;
            public string NiCheng;
            public string TouXiang;
            //public DateTime TouXiangShangChuanShiJian;
            //public long TouXiangShangChuanShiJianChuo;
            //public string FaTieShu;
            //public string BaLing;
        }
        #endregion
    }
}