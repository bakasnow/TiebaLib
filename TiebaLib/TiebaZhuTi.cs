using System;
using System.Data;
using System.Collections.Generic;
using BakaSnowTool;
using BakaSnowTool.Http;
using Newtonsoft.Json.Linq;

namespace TiebaLib
{
    public class TiebaZhuTi
    {
        /// <summary>
        /// 贴吧主题
        /// </summary>
        /// <param name="tiebaName"></param>
        public TiebaZhuTi(string tiebaName)
        {
            TiebaName = tiebaName;
        }

        /// <summary>
        /// Cookie
        /// </summary>
        public string Cookie = string.Empty;

        /// <summary>
        /// 贴吧名
        /// </summary>
        public string TiebaName { private set; get; }

        /// <summary>
        /// 当前页数
        /// </summary>
        public int Pn { private set; get; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int ZongYeShu { private set; get; }

        /// <summary>
        /// 文本过滤
        /// </summary>
        public DataTable WenBenGuoLv { set; private get; }

        /// <summary>
        /// 获取网页源码
        /// </summary>
        /// <returns></returns>
        public string GetHtml()
        {
            string tiebaNameUrlUtf8 = Http.UrlEncodeUtf8(TiebaName).ToUpper();

            string url = "http://c.tieba.baidu.com/c/f/frs/page";
            string postStr
                = Cookie
                + "&_client_id=" + Tieba.GetAndroidStamp()
                + "&_client_type=2"
                + "&_client_version=9.9.8.32"
                + "&kw=" + tiebaNameUrlUtf8
                + "&pn=" + Pn.ToString()
                + "&rn=50";

            postStr += "&sign=" + Tieba.GetTiebaSign(postStr);

            return TiebaHttp.Post(url, postStr);
        }

        /// <summary>
        /// 获取主题列表
        /// </summary>
        /// <param name="pn">当前页数</param>
        /// <returns></returns>
        public List<JieGou> Get(int pn)
        {
            //当前页数
            Pn = pn;

            //获取网页源码
            string html = GetHtml();

            //主题列表
            List<JieGou> zhuTiLieBiao = new List<JieGou>();

            //可能是网络故障
            if (string.IsNullOrEmpty(html))
            {
                return zhuTiLieBiao;
            }

            //解析
            JObject zhuTiJsonData;
            try
            {
                zhuTiJsonData = JObject.Parse(html);
            }
            catch
            {
                return zhuTiLieBiao;
            }

            var thread_list = zhuTiJsonData["thread_list"];
            var user_list = zhuTiJsonData["user_list"];

            //访问失败
            if (zhuTiJsonData["error_code"]?.ToString() != "0")
            {
                return zhuTiLieBiao;
            }

            //总页数
            try
            {
                ZongYeShu = Convert.ToInt32(zhuTiJsonData["page"]?["total_page"]);
            }
            catch
            {
                ZongYeShu = -1;
            }

            #region "主题参数处理"
            foreach (var thread in thread_list)
            {
                //跳过直播帖
                //if (thread["is_livepost"]?.ToString() == "1")
                //{
                //    continue;
                //}

                JieGou zhuTiJieGou = new JieGou();

                //主题参数
                long.TryParse(thread["tid"]?.ToString(), out zhuTiJieGou.Tid);
                zhuTiJieGou.BiaoTi = thread["title"]?.ToString();
                int.TryParse(thread["view_num"]?.ToString(), out zhuTiJieGou.DianJiLiang);
                int.TryParse(thread["reply_num"]?.ToString(), out zhuTiJieGou.HuiFuShu);
                long.TryParse(thread["last_time_int"]?.ToString(), out zhuTiJieGou.ZuiHouHuiFuShiJianChuo);
                zhuTiJieGou.ZuiHouHuiFuShiJian = BST.ShiJianChuoDaoShiJian(zhuTiJieGou.ZuiHouHuiFuShiJianChuo * 1000);
                int.TryParse(thread["thread_types"]?.ToString(), out zhuTiJieGou.LeiXing);
                int.TryParse(thread["agree_num"]?.ToString(), out zhuTiJieGou.ZanTongShu);
                int.TryParse(thread["disagree_num"]?.ToString(), out zhuTiJieGou.FanDuiShu);
                int.TryParse(thread["share_num"]?.ToString(), out zhuTiJieGou.ZhuanFaShu);

                //通用参数
                long.TryParse(thread["create_time"]?.ToString(), out zhuTiJieGou.FaTieShiJianChuo);
                zhuTiJieGou.FaTieShiJian = BST.ShiJianChuoDaoShiJian(zhuTiJieGou.FaTieShiJianChuo * 1000);
                zhuTiJieGou.IsZhiDing = thread["is_top"]?.ToString() == "1";
                zhuTiJieGou.IsJingPin = thread["is_good"]?.ToString() == "1";
                zhuTiJieGou.IsHuiYuanZhiDing = thread["is_membertop"]?.ToString() == "1";

                //楼主信息
                foreach (var user in user_list)
                {
                    if (user["id"]?.ToString() == thread["author_id"]?.ToString())
                    {
                        long.TryParse(user["id"]?.ToString(), out zhuTiJieGou.Uid);
                        zhuTiJieGou.YongHuMing = user["name"]?.ToString();
                        zhuTiJieGou.NiCheng = user["name_show"]?.ToString();
                        zhuTiJieGou.TouXiang = user["portrait"]?.ToString();
                        zhuTiJieGou.DengJi = -1;//主题帖没有等级
                        zhuTiJieGou.IsBaWu = user["is_bawu"]?.ToString() == "1";
                        zhuTiJieGou.YinJi = new TiebaYinJi(user["iconinfo"]);
                        break;
                    }
                }

                zhuTiLieBiao.Add(zhuTiJieGou);
            }
            #endregion

            return zhuTiLieBiao;
        }

        /// <summary>
        /// 主题结构
        /// </summary>
        public class JieGou : Tieba.UidJieGou
        {
            //主题参数
            public int LeiXing;//类型
            public long Tid;//Tid

            public string BiaoTi;//标题
            public int DianJiLiang;//点击量
            public int HuiFuShu;//回复数

            public int ZanTongShu;//赞同数
            public int FanDuiShu;//反对数
            public int ZhuanFaShu;//转发量

            public DateTime FaTieShiJian;//发帖时间
            public long FaTieShiJianChuo;//发帖时间戳
            public DateTime ZuiHouHuiFuShiJian;//最后回复时间
            public long ZuiHouHuiFuShiJianChuo;//最后回复时间戳

            public bool IsZhiDing;//是否置顶
            public bool IsJingPin;//是否精品
            public bool IsHuiYuanZhiDing;//是否会员置顶
        }
    }
}
