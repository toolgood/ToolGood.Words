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
    public class PinyinMatch : BasePinyinMatch
    {
        private List<string> _keywords;
        private List<string> _keywordsFirstPinyin;
        private List<string[]> _keywordsPinyin;
        private List<int> _indexs;
        private List<ulong> _hash;

        public PinyinMatch()
        {
            _keywords = new List<string>();
            _keywordsFirstPinyin = new List<string>();
            _keywordsPinyin = new List<string[]>();
            _hash = new List<ulong>();
            _indexs = new List<int>();
        }

        #region SetKeywords
        /// <summary>
        /// 设置关键字，注：索引会被清空
        /// </summary>
        /// <param name="keywords"></param>
        public void SetKeywords(ICollection<string> keywords)
        {
            _keywords.AddRange(keywords);
            for (int i = 0; i < _keywords.Count; i++)
            {

                var text = _keywords[i];
                var pys = PinyinDict.GetPinyinList(text);
                string fpy = "";
                ulong hash = 0;
                for (int j = 0; j < pys.Length; j++)
                {
                    pys[j] = pys[j].ToUpper();
                    fpy += pys[j][0];
                    hash = BuildHashByChar(hash, pys[j][0]);
                }
                if (Regex.IsMatch(text, "[^a-zA-Z0-9]", RegexOptions.Compiled))//关键字中有中文
                {
                    hash = BuildHashByChar(hash, ' ');
                }
                _keywordsPinyin.Add(pys);
                _keywordsFirstPinyin.Add(fpy);

                hash = BuildHashByLength(hash, text.Length);
                _hash.Add(hash);
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
            _keywords.AddRange(keywords);
            for (int i = 0; i < _keywords.Count; i++)
            {
                var text = keywords[i];
                var pys = pinyin[i].Split(splitChar);
                string fpy = "";
                ulong hash = 0;
                for (int j = 0; j < pys.Length; j++)
                {
                    pys[j] = pys[j].ToUpper();
                    fpy += pys[j][0];
                    hash = BuildHashByChar(hash, pys[j][0]);
                }
                if (Regex.IsMatch(text, "[^a-zA-Z0-9]", RegexOptions.Compiled))//关键字中有中文
                {
                    hash = BuildHashByChar(hash, ' ');
                }
                _keywordsPinyin.Add(pys);
                _keywordsFirstPinyin.Add(fpy);

                hash = BuildHashByLength(hash, text.Length);
                _hash.Add(hash);
            }
            _indexs = null;
        }

        #endregion

        #region SetIndexs
        /// <summary>
        /// 设置索引 ,请保证关键字与索引一一对应
        /// </summary>
        /// <param name="indexs"></param>
        public void SetIndexs(ICollection<int> indexs)
        {
            _indexs.AddRange(indexs);
        }
        /// <summary>
        /// 添加索引 ,请保证关键字与索引一一对应
        /// </summary>
        /// <param name="index"></param>
        public void AddIndex(int index)
        {
            _indexs.Add(index);
        }
        #endregion

        #region AddKeywords
        /// <summary>
        /// 添加关键字
        /// </summary>
        /// <param name="keyword">关键字</param>
        public void AddKeyword(string keyword)
        {
            _keywords.Add(keyword);
            var pys = PinyinDict.GetPinyinList(keyword);
            string fpy = "";
            ulong hash = 0;
            for (int j = 0; j < pys.Length; j++)
            {
                pys[j] = pys[j].ToUpper();
                fpy += pys[j][0];
                hash = BuildHashByChar(hash, pys[j][0]);
            }
            if (Regex.IsMatch(keyword, "[^a-zA-Z0-9]", RegexOptions.Compiled))//关键字中有中文
            {
                hash = BuildHashByChar(hash, ' ');
            }
            _keywordsPinyin.Add(pys);
            _keywordsFirstPinyin.Add(fpy);

            hash = BuildHashByLength(hash, keyword.Length);
            _hash.Add(hash);
        }
        /// <summary>
        /// 添加关键字
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <param name="pinyin">拼音</param>
        /// <param name="splitChar">分割符</param>
        public void AddKeyword(string keyword, string pinyin, char splitChar = ',')
        {
            _keywords.Add(keyword);
            var pys = pinyin.Split(splitChar);
            string fpy = "";
            ulong hash = 0;
            for (int j = 0; j < pys.Length; j++)
            {
                pys[j] = pys[j].ToUpper();
                fpy += pys[j][0];
                hash = BuildHashByChar(hash, pys[j][0]);
            }
            if (Regex.IsMatch(keyword, "[^a-zA-Z0-9]", RegexOptions.Compiled))//关键字中有中文
            {
                hash = BuildHashByChar(hash, ' ');
            }
            _keywordsPinyin.Add(pys);
            _keywordsFirstPinyin.Add(fpy);

            hash = BuildHashByLength(hash, keyword.Length);
            _hash.Add(hash);
        }


        #endregion

        /// <summary>
        /// 查询, 已知bug  key 不能太长
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<string> Find(string key)
        {
            key = key.ToUpper().Trim();
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }
            var hasPinyin = Regex.IsMatch(key, "[a-zA-Z]");
            if (hasPinyin == false)
            {
                List<string> rs = new List<string>();
                for (int i = 0; i < _keywords.Count; i++)
                {
                    var keyword = _keywords[i];
                    if (keyword.Contains(key))
                    {
                        rs.Add(keyword);
                    }
                }
                return rs;
            }

            var pykeys = SplitKeywords(key);
            var minLength = int.MaxValue;
            var minKeys = new HashSet<char>();
            List<Tuple<string, string[]>> list = new List<Tuple<string, string[]>>();
            foreach (var pykey in pykeys)
            {
                var keys = pykey.Split((char) 0);
                if (minLength > keys.Length)
                {
                    minKeys.Clear();
                    minLength = keys.Length;
                    foreach (var k in keys) { minKeys.Add(k[0]); }
                }
                else if (minLength == keys.Length)
                {
                    var newKeys = new HashSet<char>();
                    foreach (var k in keys) { newKeys.Add(k[0]); }
                    minKeys.IntersectWith(newKeys);
                }
                MergeKeywords(keys, 0, "", list);
            }
            ulong hash = 0;
            foreach (var k in minKeys)
            {
                hash = BuildHashByChar(hash, k);
            }
            hash = BuildHashByLength(hash, minLength);


            PinyinSearch search = new PinyinSearch();
            search.SetKeywords(list);
            List<String> result = new List<string>();
            for (int i = 0; i < _keywords.Count; i++)
            {
                if ((_hash[i] & hash) != hash) { continue; }

                var keywords = _keywords[i];
                var fpy = _keywordsFirstPinyin[i];
                var pylist = _keywordsPinyin[i];

                if (search.Find(fpy, keywords, pylist))
                {
                    result.Add(keywords);
                }
            }
            return result;
        }

        /// <summary>
        /// 查询索引号 , 已知bug  keywords 不能太长
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<int> FindIndex(string key)
        {
            key = key.ToUpper().Trim();
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }
            var hasPinyin = Regex.IsMatch(key, "[a-zA-Z]");
            if (hasPinyin == false)
            {
                List<int> rs = new List<int>();
                for (int i = 0; i < _keywords.Count; i++)
                {
                    var keyword = _keywords[i];
                    if (keyword.Contains(key))
                    {
                        if (_indexs == null)
                        {
                            rs.Add(i);
                        }
                        else
                        {
                            rs.Add(_indexs[i]);
                        }
                    }
                }
                return rs;
            }

            var pykeys = SplitKeywords(key);
            var minLength = int.MaxValue;
            var minKeys = new HashSet<char>();
            List<Tuple<string, string[]>> list = new List<Tuple<string, string[]>>();
            foreach (var pykey in pykeys)
            {
                var keys = pykey.Split((char) 0);
                if (minLength > keys.Length)
                {
                    minLength = keys.Length;
                    foreach (var k in keys) { minKeys.Add(k[0]); }
                }
                else if (minLength == keys.Length)
                {
                    var newKeys = new HashSet<char>();
                    foreach (var k in keys) { newKeys.Add(k[0]); }
                    minKeys.IntersectWith(newKeys);
                }
                MergeKeywords(keys, 0, "", list);
            }

            ulong hash = 0;
            foreach (var k in minKeys)
            {
                hash = BuildHashByChar(hash, k);
            }
            hash = BuildHashByLength(hash, minLength);

            PinyinSearch search = new PinyinSearch();
            search.SetKeywords(list);
            List<int> result = new List<int>();
            for (int i = 0; i < _keywords.Count; i++)
            {
                if ((_hash[i] & hash) != hash) { continue; }

                var keywords = _keywords[i];
                var fpy = _keywordsFirstPinyin[i];
                var pylist = _keywordsPinyin[i];
                if (search.Find(fpy, keywords, pylist))
                {
                    if (_indexs == null || _indexs.Count < i)
                    {
                        result.Add(i);
                    }
                    else
                    {
                        result.Add(_indexs[i]);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 查询，空格为通配符 , 已知bug  keywords 不能太长
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public List<string> FindWithSpace(string keywords)
        {
            keywords = keywords.ToUpper().Trim();
            if (string.IsNullOrEmpty(keywords))
            {
                return null;
            }
            if (keywords.Contains(" ") == false)
            {
                return Find(keywords);
            }

            List<Tuple<string, string[]>> list = new List<Tuple<string, string[]>>();
            List<int> indexs = new List<int>();
            var minLength = 0;
            var minKeys = new HashSet<char>();
            int keysCount;
            {
                var keys = keywords.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                keysCount = keys.Length;
                for (int i = 0; i < keys.Length; i++)
                {
                    var key = keys[i];
                    var pykeys = SplitKeywords(key);
                    var min = int.MaxValue;
                    var minKeys2 = new HashSet<char>();
                    foreach (var pykey in pykeys)
                    {
                        var keys2 = pykey.Split((char) 0);
                        if (min > keys2.Length)
                        {
                            min = keys2.Length;
                            minKeys2.Clear();
                            foreach (var k in keys) { minKeys2.Add(k[0]); }
                        }
                        else if (min == keys2.Length)
                        {
                            var newKeys = new HashSet<char>();
                            foreach (var k in keys) { minKeys2.Add(k[0]); }
                            minKeys2.IntersectWith(newKeys);
                        }
                        MergeKeywords(keys2, 0, "", list, i, indexs);
                    }
                    minLength += min;
                    foreach (var item in minKeys2)
                    {
                        minKeys.Add(item);
                    }
                }
            }
            ulong hash = 0;
            foreach (var k in minKeys)
            {
                hash = BuildHashByChar(hash, k);
            }
            hash = BuildHashByLength(hash, minLength);

            PinyinSearch search = new PinyinSearch();
            search.SetKeywords(list);
            search.SetIndexs(indexs.ToArray());

            List<string> result = new List<string>();
            for (int i = 0; i < _keywords.Count; i++)
            {
                if ((_hash[i] & hash) != hash) { continue; }

                var keywords2 = _keywords[i];
                var fpy = _keywordsFirstPinyin[i];
                var pylist = _keywordsPinyin[i];


                if (search.Find2(fpy, keywords2, pylist, keysCount))
                {
                    result.Add(keywords2);
                }
            }
            return result;
        }
        /// <summary>
        /// 查询索引号，空格为通配符 , 已知bug  key 不能太长
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public List<int> FindIndexWithSpace(string keywords)
        {
            keywords = keywords.ToUpper().Trim();
            if (string.IsNullOrEmpty(keywords))
            {
                return null;
            }
            if (keywords.Contains(" ") == false)
            {
                return FindIndex(keywords);
            }

            List<Tuple<string, string[]>> list = new List<Tuple<string, string[]>>();
            List<int> indexs = new List<int>();
            var minLength = 0;
            int keysCount;
            var minKeys = new HashSet<char>();
            {
                var keys = keywords.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                keysCount = keys.Length;
                for (int i = 0; i < keys.Length; i++)
                {
                    var key = keys[i];
                    var pykeys = SplitKeywords(key);
                    var min = int.MaxValue;
                    var minKeys2 = new HashSet<char>();
                    foreach (var pykey in pykeys)
                    {
                        var keys2 = pykey.Split((char) 0);
                        if (min > keys2.Length)
                        {
                            min = keys2.Length;
                            minKeys2.Clear();
                            foreach (var k in keys) { minKeys2.Add(k[0]); }
                        }
                        else if (min == keys2.Length)
                        {
                            var newKeys = new HashSet<char>();
                            foreach (var k in keys) { minKeys2.Add(k[0]); }
                            minKeys2.IntersectWith(newKeys);
                        }
                        MergeKeywords(keys2, 0, "", list, i, indexs);
                    }
                    minLength += min;
                    foreach (var item in minKeys2)
                    {
                        minKeys.Add(item);
                    }
                }
            }
            ulong hash = 0;
            foreach (var k in minKeys)
            {
                hash = BuildHashByChar(hash, k);
            }
            hash = BuildHashByLength(hash, minLength);

            PinyinSearch search = new PinyinSearch();
            search.SetKeywords(list);
            search.SetIndexs(indexs.ToArray());

            List<int> result = new List<int>();
            for (int i = 0; i < _keywords.Count; i++)
            {
                if ((_hash[i] & hash) != hash) { continue; }

                var keywords2 = _keywords[i];
                var fpy = _keywordsFirstPinyin[i];
                var pylist = _keywordsPinyin[i];
                if (search.Find2(fpy, keywords2, pylist, keysCount))
                {
                    if (_indexs == null || indexs.Count < i)
                    {
                        result.Add(i);
                    }
                    else
                    {
                        result.Add(_indexs[i]);
                    }
                }
            }
            return result;
        }


    }
}
