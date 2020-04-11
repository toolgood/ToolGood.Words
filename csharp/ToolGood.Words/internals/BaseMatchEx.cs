using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.internals
{
    public abstract class BaseMatchEx
    {
        protected int[] _dict;
        protected int[] _first;
        protected int[] _min;
        protected int[] _max;

        protected IntDictionary[] _nextIndex;
        protected int[] _end;
        protected int[] _resultIndex;

        protected int[] _keywordLength;
        protected int[] _keywordIndex;
        protected string[] _keywords;






    }
}
