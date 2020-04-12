using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaTest;
using ToolGood.Words;

namespace ToolGood.Words.Test.Pinyin
{
    [TestFixture]
    public class PinyinTest
    {
        [Test]
        public void GetPinyin()
        {
            var list = WordsHelper.GetAllPinyin('㘄');
            list = WordsHelper.GetAllPinyin('䉄');
            list = WordsHelper.GetAllPinyin('䬋');
            list = WordsHelper.GetAllPinyin('䮚');
            list = WordsHelper.GetAllPinyin('䚏');
            list = WordsHelper.GetAllPinyin('㭁');
            list = WordsHelper.GetAllPinyin('䖆');


        }


    }
}
