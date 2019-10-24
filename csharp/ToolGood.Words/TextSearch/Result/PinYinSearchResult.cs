using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words
{
    public class PinYinSearchResult
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string Keyword { get; private set; }
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; private set; }

        public PinYinSearchResult(string keyword, int id)
        {
            Keyword = keyword;
            Id = id;
        }
    }
}
