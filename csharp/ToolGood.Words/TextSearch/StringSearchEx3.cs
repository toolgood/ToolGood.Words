using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ToolGood.Words.internals;

namespace ToolGood.Words
{
    /// <summary>
    /// 文本搜索（指针版，速度更快） ，如果关键字太多(5W以上)，建议使用 StringSearchEx
    /// 性能从小到大  StringSearch &lt; StringSearchEx &lt; StringSearchEx2 &lt; StringSearchEx3
    /// </summary>
    public class StringSearchEx3 : BaseSearchEx2
    {
        protected Int32[] _guides2;
        protected Int32[] _guidesLength;

        protected internal override void Load(BinaryReader br)
        {
            base.Load(br);
            _guides2 = new int[_guides.Length];
            _guidesLength = new int[_guides.Length];
            for (int i = 0; i < _guides2.Length; i++) {
                _guides2[i] = _guides[i][0];
                _guidesLength[i] = _keywords[_guides2[i]].Length;
            }
        }
        public override void SetKeywords(ICollection<string> keywords)
        {
            base.SetKeywords(keywords);
            _guides2 = new int[_guides.Length];
            _guidesLength = new int[_guides.Length];
            for (int i = 0; i < _guides2.Length; i++) {
                _guides2[i] = _guides[i][0];
                _guidesLength[i] = _keywords[_guides2[i]].Length;
            }
        }


        #region 查找 替换 查找第一个关键字 判断是否包含关键字
        /// <summary>
        /// 在文本中查找所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public unsafe List<string> FindAll(string text)
        {
            var length = text.Length;

            List<string> root = new List<string>();
            fixed (int* _pnext = &_next[0])
            fixed (int* _pcheck = &_check[0])
            fixed (int* _pkey = &_key[0])
            fixed (int* _pdict = &_dict[0])
            fixed (char* _ptext = text) {

                var p = 0;

                for (int i = 0; i < length; i++) {
                    var t = *(_pdict + (int)*(_ptext + i));
                    if (t == 0) {
                        p = 0;
                        continue;
                    }
                    var next = *(_pnext + p) + t;
                    bool find = *(_pkey + next) == t;
                    if (find == false && p != 0) {
                        p = 0;
                        next = *_pnext + t;
                        find = *(_pkey + next) == t;
                    }
                    if (find) {
                        var index = *(_pcheck + next);
                        if (index > 0) {
                            foreach (var item in _guides[index]) {
                                root.Add(_keywords[item]);
                            }
                        }
                        p = next;
                    }
                }

            }
            //List<string> root = new List<string>();
            //foreach (var index in indexs) {
            //    foreach (var item in _guides[index]) {
            //        root.Add(_keywords[item]);
            //    }
            //}
            return root;
        }
        /// <summary>
        /// 在文本中查找第一个关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public unsafe string FindFirst(string text)
        {
            var length = text.Length;
            fixed (int* _pnext = &_next[0])
            fixed (int* _pcheck = &_check[0])
            fixed (int* _pkey = &_key[0])
            fixed (int* _pdict = &_dict[0])
            //fixed (int* _pguides2 = &_guides2[0])
            fixed (char* _ptext = text) {
                var p = 0;
                for (int i = 0; i < length; i++) {
                    var t = *(_pdict + (int)*(_ptext+i));
                    if (t == 0) {
                        p = 0;
                        continue;
                    }
                    //var next = _next[p] + t;
                    var next = *(_pnext + p) + t;
                    //if (_key[next] == t) {
                    if (*(_pkey + next) == t) {
                        //var index = _check[next];
                        var index = *(_pcheck + next);
                        if (index > 0) {
                            return _keywords[ _guides2[index]];
                            //return _keywords[*(_pguides2+index)];
                        }
                        p = next;
                    } else {
                        p = 0;
                        //next = _next[p] + t;
                        next = *(_pnext + p) + t;
                        //if (_key[next] == t) {
                        if (*(_pkey + next) == t) {
                            var index = *(_pcheck + next);
                            //var index = _check[next];
                            if (index > 0) {
                                return _keywords[_guides2[index]];

                                //return _keywords[*(_pguides2 + index)];
                            }
                            p = next;
                        }
                    }
                }

            }
            return null;
        }
        /// <summary>
        /// 判断文本是否包含关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public unsafe bool ContainsAny(string text)
        {
            var length = text.Length;

            fixed (int* _pnext = &_next[0])
            fixed (int* _pcheck = &_check[0])
            fixed (int* _pkey = &_key[0])
            fixed (int* _pdict = &_dict[0])
            fixed (char* _ptext = text) {
                var p = 0;
                for (int i = 0; i < length; i++) {
                    var t = *(_pdict + (int)*(_ptext + i));
                    if (t == 0) {
                        p = 0;
                        continue;
                    }
                    var next = *(_pnext + p) + t;
                    if (*(_pkey + next) == t) {
                        if (*(_pcheck + next) > 0) { return true; }
                        p = next;
                    } else {
                        p = 0;
                        next = *(_pnext + p) + t;
                        if (*(_pkey + next) == t) {
                            if (*(_pcheck + next) > 0) { return true; }
                            p = next;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 在文本中替换所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="replaceChar">替换符</param>
        /// <returns></returns>
        public unsafe string Replace(string text, char replaceChar = '*')
        {
            StringBuilder result = new StringBuilder(text);
            var length = text.Length;

            fixed (int* _pnext = &_next[0])
            fixed (int* _pcheck = &_check[0])
            fixed (int* _pkey = &_key[0])
            fixed (int* _pdict = &_dict[0])
            fixed (int* _pguidesLength = &_guidesLength[0])
            fixed (char* _ptext = text) {

                var p = 0;
                for (int i = 0; i < length; i++) {
                    var t = *(_pdict + (int)*(_ptext + i));
                    if (t == 0) {
                        p = 0;
                        continue;
                    }
                    var next = *(_pnext + p) + t;
                    bool find = *(_pkey + next) == t;
                    if (find == false && p != 0) {
                        p = 0;
                        next = *_pnext + t;
                        find = *(_pkey + next) == t;
                    }
                    if (find) {
                        var index = *(_pcheck + next);
                        if (index > 0) {
                            var maxLength = *(_pguidesLength + index);
                            var start = i + 1 - maxLength;
                            for (int j = start; j <= i; j++) {
                                result[j] = replaceChar;
                            }
                        }
                        p = next;
                    }
                }

            }
            return result.ToString();
        }
        #endregion



    }
}
