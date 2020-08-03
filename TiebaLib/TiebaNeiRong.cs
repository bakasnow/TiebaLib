using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace TiebaLib
{
    /// <summary>
    /// 贴吧内容
    /// </summary>
    public class TiebaNeiRong
    {
        /// <summary>
        /// 内容列表
        /// </summary>
        public List<JieGou> LieBiao { get; private set; }

        /// <summary>
        /// 拼接文本
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// TiebaNeiRong
        /// </summary>
        /// <param name="jToken"></param>
        public TiebaNeiRong(JToken jToken)
        {
            //初始化
            LieBiao = new List<JieGou>();
            Text = string.Empty;

            //索引
            int suoYin = 0;
            foreach (var content in jToken)
            {
                if (!int.TryParse(content["type"]?.ToString(), out int leiXing))
                {
                    continue;
                }

                switch (leiXing)
                {
                    case LeiXing.文本:
                        //列表
                        LieBiao.Add(new JieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = LeiXing.文本,
                            WenBen = content["text"]?.ToString()
                        });

                        //拼接文本
                        Text += content["text"]?.ToString();
                        break;

                    case LeiXing.链接:
                        //列表
                        LieBiao.Add(new JieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = LeiXing.链接,
                            WenBen = content["text"]?.ToString()
                        });

                        //拼接文本
                        Text += content["text"]?.ToString();
                        break;

                    case LeiXing.表情:
                        //列表
                        LieBiao.Add(new JieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = LeiXing.表情,
                            WenBen = content["c"]?.ToString()
                        });

                        //拼接文本
                        Text += $"#表情={content["c"]?.ToString()}#";
                        break;

                    case LeiXing.图片:
                        //列表
                        LieBiao.Add(new JieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = LeiXing.图片,
                            WenBen = content["origin_src"]?.ToString(),
                        });

                        //拼接文本
                        Text += $"#图片={content["origin_src"]?.ToString()}#";
                        break;

                    case LeiXing.艾特:
                        //列表
                        LieBiao.Add(new JieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = LeiXing.艾特,
                            WenBen = content["text"]?.ToString()
                        });

                        //拼接文本
                        Text += $"#艾特={content["text"]?.ToString()}#";
                        break;

                    case LeiXing.视频:
                        //列表
                        LieBiao.Add(new JieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = LeiXing.视频,
                            WenBen = "#视频#"
                        });

                        //拼接文本
                        Text += "#视频#";
                        break;

                    case LeiXing.换行:
                        //列表
                        LieBiao.Add(new JieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = LeiXing.换行,
                            WenBen = "\n"
                        });
                        break;

                    case LeiXing.电话号码:
                        //列表
                        LieBiao.Add(new JieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = LeiXing.电话号码,
                            WenBen = content["text"]?.ToString()
                        });

                        //拼接文本
                        Text += content["text"]?.ToString();
                        break;

                    case LeiXing.语音:
                        //列表
                        LieBiao.Add(new JieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = LeiXing.语音,
                            WenBen = content["voice_md5"]?.ToString()
                        });

                        //拼接文本
                        Text += $"#语音#";
                        break;

                    case LeiXing.动态表情:
                        //列表
                        LieBiao.Add(new JieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = LeiXing.动态表情,
                            WenBen = content["c"]?.ToString()
                        });

                        //拼接文本
                        Text += $"#表情={content["c"]?.ToString()}#";
                        break;

                    case LeiXing.涂鸦:
                        //列表
                        LieBiao.Add(new JieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = LeiXing.涂鸦,
                            WenBen = content["graffiti_info"]?["url"]?.ToString()
                        });

                        //拼接文本
                        Text += $"#涂鸦={content["graffiti_info"]?["url"]?.ToString()}#";
                        break;

                    case LeiXing.活动:
                        //列表
                        LieBiao.Add(new JieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = LeiXing.活动,
                            WenBen = content["album_name"]?.ToString()
                        });

                        //拼接文本
                        Text += $"#活动={content["album_name"]?.ToString()}#";
                        break;

                    case LeiXing.热议:
                        //列表
                        LieBiao.Add(new JieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = LeiXing.热议,
                            WenBen = content["text"]?.ToString()
                        });

                        //拼接文本
                        Text += $"#热议={content["text"]?.ToString()}#";
                        break;
                          
                    case LeiXing.动态图片:
                        //列表
                        LieBiao.Add(new JieGou
                        {
                            SuoYin = suoYin++,
                            LeiXing = LeiXing.动态图片,
                            WenBen = content["src"]?.ToString()
                        });

                        //拼接文本
                        Text += $"#图片={content["src"]?.ToString()}#";
                        break;

                    default:
                        Console.WriteLine(content.ToString());
                        break;
                }
            }
        }

        /// <summary>
        /// 内容结构
        /// </summary>
        public class JieGou
        {
            public int SuoYin;//索引
            public int LeiXing;//类型
            public string WenBen;//文本
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
