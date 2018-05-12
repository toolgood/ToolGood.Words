using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaTest;
using ToolGood.Words;

namespace ToolGood.Words.Test.PinYin
{
    [TestFixture]
    public class PinYinTest
    {
        [Test]
        public void GetPinYin()
        {
            var list = WordsHelper.GetAllPinYin('㘄');
            list = WordsHelper.GetAllPinYin('䉄');
            list = WordsHelper.GetAllPinYin('䬋');
            list = WordsHelper.GetAllPinYin('䮚');
            list = WordsHelper.GetAllPinYin('䚏');
            list = WordsHelper.GetAllPinYin('㭁');
            list = WordsHelper.GetAllPinYin('䖆');


        }


    }
}
