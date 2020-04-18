using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ToolGood.Words.internals;

namespace ToolGood.Words
{
    /// <summary>
    /// 拼音匹配
    /// </summary>
    public class PinyinMatch : BasePinyinMatch
    {
        private string[] _keywords;
        private string[] _keywordsFirstPinyin;
        private string[][] _keywordsPinyin;
        private int[] _indexs;

        #region SetKeywords
        /// <summary>
        /// 设置关键字，注：索引会被清空
        /// </summary>
        /// <param name="keywords"></param>
        public void SetKeywords(ICollection<string> keywords)
        {
            _keywords = keywords.ToArray();
            _keywordsFirstPinyin = new string[_keywords.Length];
            _keywordsPinyin = new string[_keywords.Length][];
            for (int i = 0; i < _keywords.Length; i++) {
                var text = _keywords[i];
                var pys = PinyinDict.GetPinyinList(text);
                string fpy = "";
                for (int j = 0; j < pys.Length; j++) {
                    pys[j] = pys[j].ToUpper();
                    fpy += pys[j][0];
                }
                _keywordsPinyin[i] = pys;
                _keywordsFirstPinyin[i] = fpy;
            }
            _indexs = null;
        }

        /// <summary>
        /// 设置关键字，注：索引会被清空
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="pinyin"></param>
        /// <param name="splitChar"></param>
        public void SetKeywords(IList<string> keywords, IList<string> pinyin, char splitChar = ',')
        {
            _keywords = keywords.ToArray();
            _keywordsFirstPinyin = new string[_keywords.Length];
            _keywordsPinyin = new string[_keywords.Length][];
            for (int i = 0; i < _keywords.Length; i++) {
                var text = pinyin[i];
                var pys = text.Split(splitChar);
                string fpy = "";
                for (int j = 0; j < pys.Length; j++) {
                    pys[j] = pys[j].ToUpper();
                    fpy += pys[j][0];
                }
                _keywordsPinyin[i] = pys;
                _keywordsFirstPinyin[i] = fpy;
            }
            _indexs = null;
        }

        #endregion

        #region SetIndexs
        /// <summary>
        /// 设置索引
        /// </summary>
        /// <param name="indexs"></param>
        public void SetIndexs(ICollection<int> indexs)
        {
            if (_keywords == null) {
                throw new Exception("请先使用 SetKeywords 方法");
            }
            if (indexs.Count < _keywords.Length) {
                throw new Exception("indexs 数组长度大于 keywords");
            }
            _indexs = indexs.ToArray();
        }
        #endregion


        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<string> Find(string key)
        {
            key = key.ToUpper().Trim();
            if (string.IsNullOrEmpty(key)) {
                return null;
            }
            var hasPinyin = Regex.IsMatch(key, "[a-zA-Z]");
            if (hasPinyin == false) {
                List<string> rs = new List<string>();
                for (int i = 0; i < _keywords.Length; i++) {
                    var keyword = _keywords[i];
                    if (keyword.Contains(key)) {
                        rs.Add(keyword);
                    }
                }
                return rs;
            }

            var pykeys = SplitKeywords(key);
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
            List<String> result = new List<string>();
            for (int i = 0; i < _keywords.Length; i++) {
                var keywords = _keywords[i];
                if (keywords.Length < minLength) {
                    continue;
                }
                var fpy = _keywordsFirstPinyin[i];
                var pylist = _keywordsPinyin[i];


                if (search.Find(fpy, keywords, pylist)) {
                    result.Add(keywords);
                }
            }
            return result;
        }

        /// <summary>
        /// 查询索引号
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<int> FindIndex(string key)
        {
            key = key.ToUpper().Trim();
            if (string.IsNullOrEmpty(key)) {
                return null;
            }
            var hasPinyin = Regex.IsMatch(key, "[a-zA-Z]");
            if (hasPinyin == false) {
                List<int> rs = new List<int>();
                for (int i = 0; i < _keywords.Length; i++) {
                    var keyword = _keywords[i];
                    if (keyword.Contains(key)) {
                        if (_indexs == null) {
                            rs.Add(i);
                        } else {
                            rs.Add(_indexs[i]);
                        }
                    }
                }
                return rs;
            }

            var pykeys = SplitKeywords(key);
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
            List<int> result = new List<int>();
            for (int i = 0; i < _keywords.Length; i++) {
                var keywords = _keywords[i];
                if (keywords.Length < minLength) {
                    continue;
                }
                var fpy = _keywordsFirstPinyin[i];
                var pylist = _keywordsPinyin[i];
                if (search.Find(fpy, keywords, pylist)) {
                    if (_indexs == null) {
                        result.Add(i);
                    } else {
                        result.Add(_indexs[i]);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 查询，空格为通配符
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public List<string> FindWithSpace(string keywords)
        {
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

            List<string> result = new List<string>();
            for (int i = 0; i < _keywords.Length; i++) {
                var keywords2 = _keywords[i];
                if (keywords2.Length < minLength) {
                    continue;
                }
                var fpy = _keywordsFirstPinyin[i];
                var pylist = _keywordsPinyin[i];


                if (search.Find2(fpy, keywords2, pylist, keysCount)) {
                    result.Add(keywords2);
                }
            }
            return result;
        }
        /// <summary>
        /// 查询索引号，空格为通配符
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public List<int> FindIndexWithSpace(string keywords)
        {
            keywords = keywords.ToUpper().Trim();
            if (string.IsNullOrEmpty(keywords)) {
                return null;
            }
            if (keywords.Contains(" ") == false) {
                return FindIndex(keywords);
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

            List<int> result = new List<int>();
            for (int i = 0; i < _keywords.Length; i++) {
                var keywords2 = _keywords[i];
                if (keywords2.Length < minLength) {
                    continue;
                }
                var fpy = _keywordsFirstPinyin[i];
                var pylist = _keywordsPinyin[i];
                if (search.Find2(fpy, keywords2, pylist, keysCount)) {
                    if (_indexs == null) {
                        result.Add(i);
                    } else {
                        result.Add(_indexs[i]);
                    }
                }
            }
            return result;
        }


    }
}
