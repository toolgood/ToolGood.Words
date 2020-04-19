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

            var list2 = WordsHelper.GetPinyin("𠀀");
            var list3 = WordsHelper.GetPinyin("𫠝");
            //var start = "𠀀";// '\ud840' '\udc00' - '\udfff'  
            //var end = "𫠝";// '\ud86e' '\udc1d'
        }


    }
}
