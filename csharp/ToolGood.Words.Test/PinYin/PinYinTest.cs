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

            var t = "三个金念鑫,三个水念淼,三个火念焱, 三个土念垚,三个牛念犇,三个手念掱,三个目念瞐,三个田念畾,三个马念骉,三个羊念羴,三个犬念猋,三个鹿念麤,三个鱼念鱻,三个贝念赑,三个力念劦,三个毛念毳,三个耳念聶,三个车念轟,三个直念矗,三个龙念龘,三个原念厵,三个雷念靐,三个飞念飝,三个刀念刕,三个又念叒,三个士念壵,三个小念尛,三个子念孨,三个止念歮,三个风念飍,三个隼念雥,三个吉念嚞,三个言念譶,三个舌念舙,三个香念馫,三个泉念灥,三个心念惢,三个白念皛";
            var pys = ToolGood.Words.WordsHelper.GetPinyin(t);

            var s = WordsHelper.GetPinyin("什么");



        }


    }
}
