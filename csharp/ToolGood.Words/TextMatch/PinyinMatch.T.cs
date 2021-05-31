using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ToolGood.Words.internals;

namespace ToolGood.Words
{
    /// <summary>
    /// 拼音匹配, 不支持[0x20000-0x2B81D]
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PinyinMatch<T> : BasePinyinMatch
    {
        private ICollection<T> _list;
        private Func<T, string> _keywordsFunc;
        private Func<T, string> _pinyinFunc;
        private char _splitChar = ',';

        public PinyinMatch(ICollection<T> list)
        {
            _list = list;
        }
        /// <summary>
        /// 设置获取关键字的方法
        /// </summary>
        /// <param name="keywordsFunc"></param>
        public void SetKeywordsFunc(Func<T, string> keywordsFunc)
        {
            _keywordsFunc = keywordsFunc;
        }
        /// <summary>
        /// 设置获取拼音的方法
        /// </summary>
        /// <param name="pinyinFunc"></param>
        public void SetPinyinFunc(Func<T, string> pinyinFunc)
        {
            _pinyinFunc = pinyinFunc;
        }
        /// <summary>
        /// 设置拼音分隔符
        /// </summary>
        /// <param name="splitChar"></param>
        public void SetPinyinSplitChar(char splitChar = ',')
        {
            _splitChar = splitChar;
        }

        /// <summary>
        /// 查询 , 已知bug  keywords 不能太长
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public List<T> Find(string keywords)
        {
            if (_keywordsFunc == null) {
                throw new Exception("请先使用SetKeywordsFunc方法。");
            }
            keywords = keywords.ToUpper().Trim();
            if (string.IsNullOrEmpty(keywords)) {
                return null;
            }
            List<T> result = new List<T>();
            var hasPinyin = Regex.IsMatch(keywords, "[a-zA-Z]");
            if (hasPinyin == false) {
                foreach (var item in _list) {
                    var keyword = _keywordsFunc(item);
                    if (keyword.Contains(keywords)) {
                        result.Add(item);
                    }
                }
                return result;
            }

            var pykeys = SplitKeywords(keywords);
            var minLength = int.MaxValue;
            List<Tuple<string, string[]>> list = new List<Tuple<string, string[]>>();
            foreach (var pykey in pykeys) {
                var keys = pykey.Split((char)0);
                if (minLength > keys.Length) {
                    minLength = keys.Length;
                }
                MergeKeywords(keys, 0, "", list);
            }

            PinyinSearch search = new PinyinSearch();
            search.SetKeywords(list);
            foreach (var item in _list) {
                var keyword = _keywordsFunc(item);
                if (keyword.Length < minLength) {
                    continue;
                }
                string fpy = "";
                string[] pylist;
                if (_pinyinFunc == null) {
                    pylist = PinyinDict.GetPinyinList(keyword);
                } else {
                    pylist = _pinyinFunc(item).Split(_splitChar);
                }
                for (int j = 0; j < pylist.Length; j++) {
                    pylist[j] = pylist[j].ToUpper();
                    fpy += pylist[j][0];
                }
                if (search.Find(fpy, keyword, pylist)) {
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// 查询，空格为通配符  , 已知bug  keywords 不能太长
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public List<T> FindWithSpace(string keywords)
        {
            if (_keywordsFunc == null) {
                throw new Exception("请先使用SetKeywordsFunc方法。");
            }
            keywords = keywords.ToUpper().Trim();
            if (string.IsNullOrEmpty(keywords)) {
                return null;
            }
            if (keywords.Contains(" ") == false) {
                return Find(keywords);
            }

            List<Tuple<string, string[]>> list = new List<Tuple<string, string[]>>();
            List<int> indexs = new List<int>();
            var minLength = 0;
            int keysCount;
            {
                var keys = keywords.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                keysCount = keys.Length;
                for (int i = 0; i < keys.Length; i++) {
                    var key = keys[i];
                    var pykeys = SplitKeywords(key);
                    var min = int.MaxValue;
                    foreach (var pykey in pykeys) {
                        var keys2 = pykey.Split((char)0);
                        if (min > keys2.Length) {
                            min = keys2.Length;
                        }
                        MergeKeywords(keys2, 0, "", list, i, indexs);
                    }
                    minLength += min;
                }
            }

            PinyinSearch search = new PinyinSearch();
            search.SetKeywords(list);
            search.SetIndexs(indexs.ToArray());

            List<T> result = new List<T>();
            foreach (var item in _list) {
                var keyword = _keywordsFunc(item);
                if (keyword.Length < minLength) {
                    continue;
                }
                string fpy = "";
                string[] pylist;
                if (_pinyinFunc == null) {
                    pylist = PinyinDict.GetPinyinList(keyword);
                } else {
                    pylist = _pinyinFunc(item).Split(_splitChar);
                }
                for (int j = 0; j < pylist.Length; j++) {
                    pylist[j] = pylist[j].ToUpper();
                    fpy += pylist[j][0];
                }
                if (search.Find2(fpy, keyword, pylist, keysCount)) {
                    result.Add(item);
                }
            }
            return result;
        }


    }
}
