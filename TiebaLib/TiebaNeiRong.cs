using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiebaLib
{
    /// <summary>
    /// 贴吧内容
    /// </summary>
    public static class TiebaNeiRong
    {
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="jToken"></param>
        /// <returns></returns>
        public static List<JieGou> Get(JToken jToken)
        {
            List<JieGou> neiRongLieBiao = new List<JieGou>();

            foreach (var content in jToken)
            {
                if (!int.TryParse(content["type"]?.ToString(), out int leiXing))
                {
                    continue;
                }

                switch (leiXing)
                {
                    case LeiXing.文本:
                        neiRongLieBiao.Add(new JieGou
                        {
                            LeiXing = LeiXing.文本,
                            WenBen = content["text"]?.ToString()
                        });
                        break;

                    case LeiXing.链接:
                        neiRongLieBiao.Add(new JieGou
                        {
                            LeiXing = LeiXing.链接,
                            WenBen = content["text"]?.ToString()
                        });
                        break;

                    case LeiXing.表情:
                        neiRongLieBiao.Add(new JieGou
                        {
                            LeiXing = LeiXing.表情,
                            WenBen = content["c"]?.ToString()
                        });
                        break;

                    case LeiXing.图片:
                        neiRongLieBiao.Add(new JieGou
                        {
                            LeiXing = LeiXing.图片,
                            WenBen = content["origin_src"]?.ToString(),
                        });
                        break;

                    case LeiXing.艾特:
                        neiRongLieBiao.Add(new JieGou
                        {
                            LeiXing = LeiXing.艾特,
                            WenBen = content["text"]?.ToString()
                        });
                        break;

                    case LeiXing.视频:
                        neiRongLieBiao.Add(new JieGou
                        {
                            LeiXing = LeiXing.视频,
                            WenBen = "#视频#"
                        });
                        break;

                    case LeiXing.换行:
                        break;

                    case LeiXing.电话号码:
                        neiRongLieBiao.Add(new JieGou
                        {
                            LeiXing = LeiXing.电话号码,
                            WenBen = content["text"]?.ToString()
                        });
                        break;

                    case LeiXing.语音:
                        neiRongLieBiao.Add(new JieGou
                        {
                            LeiXing = LeiXing.语音,
                            WenBen = content["voice_md5"]?.ToString()
                        });
                        break;

                    case LeiXing.动态表情:
                        neiRongLieBiao.Add(new JieGou
                        {
                            LeiXing = LeiXing.动态表情,
                            WenBen = content["c"]?.ToString()
                        });
                        break;

                    case LeiXing.涂鸦:
                        neiRongLieBiao.Add(new JieGou
                        {
                            LeiXing = LeiXing.涂鸦,
                            WenBen = content["graffiti_info"]?["url"]?.ToString()
                        });
                        break;

                    case LeiXing.活动:
                        neiRongLieBiao.Add(new JieGou
                        {
                            LeiXing = LeiXing.活动,
                            WenBen = content["album_name"]?.ToString()
                        });
                        break;

                    case LeiXing.热议:
                        neiRongLieBiao.Add(new JieGou
                        {
                            LeiXing = LeiXing.热议,
                            WenBen = content["text"]?.ToString()
                        });
                        break;

                    case LeiXing.动态图片:
                        neiRongLieBiao.Add(new JieGou
                        {
                            LeiXing = LeiXing.动态图片,
                            WenBen = content["src"]?.ToString()
                        });
                        break;

                    default:
                        Console.WriteLine(content.ToString());
                        break;
                }
            }

            return neiRongLieBiao;
        }

        /// <summary>
        /// 内容结构
        /// </summary>
        public class JieGou
        {
            public int LeiXing;
            public string WenBen;
        }

        /// <summary>
        /// 内容类型
        /// </summary>
        public class LeiXing
        {
            public const int 文本 = 0;
            public const int 链接 = 1;
            public const int 表情 = 2;
            public const int 图片 = 3;
            public const int 艾特 = 4;
            public const int 视频 = 5;
            public const int 换行 = 7;
            public const int 电话号码 = 9;
            public const int 语音 = 10;
            public const int 动态表情 = 11;
            public const int 涂鸦 = 16;
            public const int 活动 = 17;
            public const int 热议 = 18;
            public const int 动态图片 = 20;
            public const int 其他 = -1;
        }
    }
}
