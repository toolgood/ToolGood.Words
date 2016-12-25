using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.International.Converters.PinYinConverter;
using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;

namespace ToolGood.PinYin.Build
{
    class Program
    {
        const string t2s1 = "丟並亂亙亞伋伕佇佈佔佪併來侖侶侷俁係俠倀倆倉個們倖倣倫偉側偵偺偽傑傖傘備傢傭傯傳傴債傷傾僂僅僇僉僑僕僥僨僱價儀儂億儅儈儉儐儔儕儘償優儲儷儸儺儻儼兇兌兒兗內兩冊冑冪凈凍凜凱別刪剄則剋剎剛剝剮剴創剷劃劇劉劊劌劍劑劻勁動勗務勛勝勞勢勣勦勱勵勸勻匭匯匱區協卹卻厙厭厲厴參叢吋吳吶呂呎咼員唄唸問啞啟啣喚喪喫喬單喲嗆嗇嗎嗚嗩嗶嘆嘍嘔嘖嘗嘜嘩嘮嘯嘰嘵嘸噁噓噠噥噦噯噲噴噸噹嚀嚇嚌嚕嚙嚥嚦嚨嚮嚳嚴嚶囀囁囂囅囈囉囌囑囪圇國圍園圓圖團坰埡執堅堊堝堯報場堿塊塋塏塒塗塢塤塵塹墊墑墜墮墳墻墾壇壎壓壘壙壚壞壟壢壩壯壺壽夠夢夾奐奧奩奪奮妝妳姍姦姪娛婁婦婭媧媯媼媽嫗嫵嫻嬈嬋嬌嬙嬝嬡嬤嬪嬰嬸孃孌孫學孿宮寢實寧審寫寬寵寶將專尋對導尷屆屜屝屢層屨屬岡岧峴島峽崍崗崙崠崢崳嵐嵒嶁嶄嶇嶗嶠嶧嶸嶺嶼嶽巋巑巒巔巖巰巹帥師帳帶幀幃幗幘幟幣幫幬幹幾庂庫廁廂廄廈廚廝廟廠廡廢廣廩廬廱廳弒弔弳張強彆彈彊彌彎彙彥彫彿徑從徠復徬徹恆恥悅悵悶悽惡惱惲惻愛愜愨愴愷愾慄慇態慍慘慚慟慣慪慫慮慳慶慼憂憊憐憑憒憚憤憫憮憲憶懇應懌懍懞懟懣懨懲懶懷懸懺懼懾戀戇戉戔戧戩戰戲戶扐扠扢扺抃抆抎抴拋挶挾捍捨捫捲掃掄掙掛採揀揚換揮揹搆損搖搗搟搶摑摜摟摯摳摶摻撈撐撓撚撟撢撣撥撫撲撳撻撾撿擁擄擇擊擋擔據擠擣擬擯擰擱擲擴擷擺擻擼擾攄攆攏攔攖攙攛攜攝攢攣攤攪攬攷敗敘敵數斂斃斕斬斷旂時晉晝晞暈暉暘暢暫暱暸曄曆曇曉曖曠曬書會朧朮杇東枙枴柵桿梔條梟棄棖棗棟棧棲椏楊楓楨業極榦榪榮榿槃構槍槓槧槨槳樁樂樅樑樓標樞樣樸樹樺橈橋機橢橫檁檉檔檜檢檣檯檳檸檻櫃櫓櫚櫛櫝櫞櫟櫥櫧櫨櫪櫬櫳櫸櫺櫻欄權欏欒欖欞欽歎歐歟歡歲歷歸歿殘殞殤殫殮殯殲殺殼毀毆毌毬毿氈氌氣氫氬氳氾汍汎汙決沒沖況洩洶浬浹涇涼淒淚淥淨淪淵淶淺渙減渦測渾湊湞湧湯溈準溝溫溼滄滅滌滎滬滯滲滷滸滾滿漁漚漢漣漬漲漸漿潁潑潔潛潟潤潯潰潷潿澀澆澇澗澠澤澩澮澱濁濃濕濘濛濟濤濫濰濱濺濼濾瀅瀆瀉瀋瀏瀕瀘瀝瀟瀠瀧瀨瀰瀲瀾灃灄灑灕灘灝灣灤災為烏烴無煉煒煙煢煥煩煬熒熗熱熾燁燄燈燉燐燒燙燜營燦燬燭燴燼燾爍爐爛爭爺爾牆牘牪牴牽犖犛犢犧狀狹狽猙猶猻獄獅獎獨獪獫獮獰獲獵獷獸獺獻獼玀玆玨珮現琺琿瑋瑣瑤瑩瑪瑯璉璣璦環璽瓊瓏瓔瓖瓚甌甕產甦畝畢畫畬異當疇疊疿痙痠痲痳痺瘋瘍瘓瘞瘡瘧瘺療癆癇癉癘癟癡癢癤癥癩癬癭癮癰癱癲發皚皰皸皺皻盜盞盡監盤盧盪眥眾睏睜睞睪瞇瞞瞭瞼矇矓矚矯砲硃硤硨硯碩碭確碼磚磣磧磯磽礎礙礦礪礫礬礱祅祇祐祕祿禍禎禦禪禮禰禱禿秈稅稈稜稟種稱穀穌積穎穡穢穩穫窩窪窮窯窶窺竄竅竇竊競笻筆筍筧箄箇箋箏節範築篋篠篤篩篳簀簍簑簞簡簣簫簷簽簾籃籌籐籜籟籠籤籥籩籪籬籮籲粵糝糞糢糧糰糲糴糶糾紀紂約紅紆紇紈紉紋納紐紓純紕紗紙級紛紜紡細紱紲紳紹紺紼紿絀終絃組絆絎結絕絛絞絡絢給絨統絲絳絹綁綃綆綈綏經綜綞綠綢綣綬維綰綱網綴綵綸綹綺綻綽綾綿緄緇緊緋緒緗緘緙線緝緞締緡緣緦編緩緬緯緱緲練緶緹緻縈縉縊縋縐縑縚縛縝縞縟縣縫縭縮縯縱縲縳縴縵縶縷縹總績繃繅繆繈繒織繕繚繞繡繢繩繪繫繭繯繰繳繹繼繽繾纈纊續纍纏纓纔纖纘纜缽罈罌罰罵罷羅羆羈羋羥羨義習翹耑耬聖聞聯聰聲聳聵聶職聹聽聾肅脅脈脛脣脧脩脫脹腎腡腦腫腳腸膃膚膠膩膽膾膿臉臍臏臘臚臟臠臥臨臺與興舉舊艙艣艤艦艫艱艷艸芐芻苧茍茲荊莊莖莢莧華菴萇萊萬萵葉葒著葦葯葷蒔蒞蒼蓀蓋蓮蓯蓽蔆蔔蔞蔣蔥蔦蔭蕁蕆蕎蕓蕕蕘蕢蕩蕪蕭蕷薈薊薌薑薔薟薦薧薩薺藍藎藝藥藪藶藷藹藺蘄蘆蘇蘊蘋蘗蘚蘞蘢蘭蘺蘿處虛虜號虧蛺蛻蜆蝕蝦蝸螄螞螢螻蟄蟈蟣蟬蟯蟲蟶蟻蠅蠆蠐蠑蠔蠟蠣蠱蠶蠻衊術衛衝衹袞裊裏補裝裡製複褲褳褸褻襖襝襠襤襪襬襯襲覈見規覓視覘覡覦親覬覯覲覷覺覽覿觀觔觴觶觸訂訃計訊訌討訐訓訕訖託記訛訝訟訢訣訥訪設許訴訶診註証詁詆詎詐詒詔評詘詛詞詠詡詢詣試詩詫詬詭詮詰話該詳詵詼詿誄誅誆誇誌認誑誒誕誘誚語誠誡誣誤誥誦誨說誰課誶誹誼調諂諄談諉請諍諏諑諒論諗諛諜諞諢諤諦諧諫諭諮諱諳諶諷諸諺諼諾謀謁謂謄謅謊謎謐謔謖謗謙謚講謝謠謨謫謬謳謹謾證譎譏譖識譙譚譜譟譫譯議譴護譽讀變讎讒讓讕讖讚讜讞豈豎豐豔豬貍貓貝貞負財貢貧貨販貪貫責貯貰貲貳貴貶買貸貺費貼貽貿賀賁賂賃賄賅資賈賊賑賒賓賕賚賜賞賠賡賢賣賤賦賧質賬賭賴賺賻購賽賾贄贅贈贊贍贏贐贓贖贗贛趕趙趨趲跡跼踐踡踫踴蹌蹕蹠蹣蹤蹺躉躊躋躍躑躒躓躕躚躡躥躦躪軀車軋軌軍軒軔軛軟軫軸軹軺軻軼軾較輅輇載輊輒輓輔輕輛輜輝輞輟輥輦輩輪輯輳輸輻輾輿轂轄轅轆轉轍轎轔轟轡轢轤辦辭辮辯農迆迴迺逕這連週進遊運過達違遙遜遞遠適遲遷選遺遼邁還邇邊邏邐郃郟郵鄆鄉鄒鄔鄖鄧鄭鄰鄲鄴鄶鄺酈醃醜醞醫醬釀釁釃釅釋釐釓釔釕釗釘釙針釣釤釦釧釩釬釵釷釹鈀鈁鈄鈉鈍鈐鈑鈔鈕鈞鈣鈥鈦鈧鈮鈰鈳鈴鈷鈸鈹鈺鈽鈾鈿鉀鉅鉆鉈鉉鉋鉍鉑鉗鉚鉛鉞鉤鉦鉬鉭鉸鉺鉻鉿銀銃銅銑銓銖銘銚銜銠銣銥銦銨銩銪銫銬銲銳銷銹銻銼鋁鋃鋅鋇鋌鋏鋒鋝鋟鋤鋦鋨鋪鋮鋯鋰鋱鋸鋼錁錄錆錈錐錒錕錘錙錚錛錟錠錢錦錨錫錮錯錳錶錸鍆鍇鍊鍋鍍鍔鍘鍛鍤鍥鍬鍰鍵鍶鍺鍾鎂鎊鎔鎖鎗鎘鎢鎣鎦鎧鎩鎪鎬鎮鎰鎳鎵鏃鏇鏈鏌鏍鏑鏗鏘鏜鏝鏞鏟鏡鏢鏤鏨鏵鏷鏹鏽鐃鐉鐋鐐鐒鐓鐔鐘鐙鐠鐨鐫鐮鐲鐳鐵鐸鐺鐽鐿鑄鑊鑌鑑鑒鑠鑣鑤鑭鑰鑲鑷鑼鑽鑾鑿長門閂閃閆閉開閌閎閏閑閒間閔閘閡閣閤閥閨閩閫閬閭閱閶閹閻閼閽閾閿闃闆闈闊闋闌闐闔闕闖關闞闡闢闥阨阯陘陜陝陣陰陳陸陽隄隉隊階隕際隨險隱隴隸隻雋雖雙雛雜雞離難雲電霑霤霧霽靂靄靈靚靜靦靨鞏鞦韁韃韆韉韋韌韓韙韜韞韻響頁頂頃項順頇須頊頌頎頏預頑頒頓頗領頜頡頤頦頭頰頷頸頹頻顆題額顎顏顓願顙顛類顢顥顧顫顯顰顱顳顴風颮颯颱颳颶颺颼飄飆飛飢飩飪飫飭飯飲飴飼飽飾餃餅餉養餌餑餒餓餘餚餛餞餡館餳餵餼餽餾餿饃饅饈饉饋饌饑饒饗饜饞馬馭馮馱馳馴駁駐駑駒駔駕駘駙駛駝駟駢駭駱駿騁騅騍騎騏騖騙騫騭騮騰騶騷騸騾驀驁驂驃驄驅驊驍驏驕驗驚驛驟驢驤驥驪骯髏髒體髕髖髮鬆鬍鬚鬢鬥鬧鬨鬩鬮鬱魎魘魚魯魴魷鮐鮑鮒鮚鮞鮪鮫鮭鮮鯀鯁鯇鯉鯊鯔鯖鯛鯡鯢鯤鯧鯨鯪鯫鯰鯽鰈鰉鰍鰒鰓鰣鰥鰨鰩鰭鰱鰲鰳鰷鰹鰻鰾鱈鱉鱒鱔鱖鱗鱘鱟鱧鱭鱷鱸鱺鳥鳧鳩鳳鳴鳶鴆鴇鴉鴕鴛鴝鴟鴣鴦鴨鴯鴰鴻鴿鵂鵑鵒鵓鵜鵝鵠鵡鵪鵬鵯鵲鶇鶉鶘鶚鶩鶯鶱鶴鶻鶼鶿鷂鷓鷗鷙鷚鷥鷦鷯鷲鷳鷴鷸鷹鷺鸕鸚鸛鸝鸞鹵鹹鹺鹼鹽麗麥麩麵麼黃黌點黨黲黴黷黽黿鼇鼉鼕鼴齊齋齏齒齔齙齜齟齠齡齣齦齧齪齬齲齷龍龐龔龕龜";

        static void Main(string[] args)
        {
            List<Tuple<int, int, string>> list = new List<Tuple<int, int, string>>();
            var index = 1;
            var start = (int)'丟';
            var end = (int)'丟';
            var str = "丟";
            while (index < t2s1.Length) {
                var c = t2s1[index];
                if (c > end + 100) {
                    var key = Tuple.Create(start, end, str);
                    list.Add(key);
                    start = c;
                    end = c;
                    str = c.ToString();
                } else {
                    end = (int)c;
                    str += c;
                }
                index++;
            }

            for (int i = 0; i < list.Count; i++) {

            }






            index = 0x4e00;
            var tt = "";
            while (index <= 0x9fa5) {
                tt += (char)index;
                index++;
            }
            var st = ChineseConverter.Convert(tt, ChineseConversionDirection.TraditionalToSimplified);
            var cccc = (int)'丟';
            var ccc2c = (int)'龜';


            List<HashSet<int>> t = new List<HashSet<int>>();
            index = 0x4e00;
            while (index <= 0x9fff) {
                var ch = (char)index;

                HashSet<int> ls = new HashSet<int>();
                try {
                    var gpy = PinYinConverter.Get(ch.ToString());
                    if (gpy != ch.ToString()) {
                        var py2 = GetPyName(gpy);
                        if (py2 > 0) {
                            ls.Add(py2);
                        }
                    }
                } catch (Exception) { }

                try {
                    var chinese = new ChineseChar(ch);
                    for (int i = 0; i < chinese.PinyinCount; i++) {
                        var py = chinese.Pinyins[i].Replace("YAI", "YA");
                        var py2 = GetPyName(py);
                        if (py2 == -1) {

                        }
                        ls.Add(py2);
                    }
                } catch (Exception) {

                }
                try {
                    char c;
                    Dict.TraditionalToSimplified(ch, out c);
                    if (ch != c) {
                        var chinese = new ChineseChar(c);
                        for (int i = 0; i < chinese.PinyinCount; i++) {
                            var py = chinese.Pinyins[i].Replace("YAI", "YA");
                            var py2 = GetPyName(py);
                            if (py2 == -1) {

                            }
                            ls.Add(py2);
                        }
                    }
                } catch (Exception) { }


                var py3 = GetPyName2(ch.ToString());
                if (py3 > 0) {
                    ls.Add(py3);
                }
                if (ch == '刓') {
                    ls.Add(GetPyName("Liang"));
                }


                t.Add(ls);
                index++;
            }
            index = 0;
            List<short> node = new List<short>();
            //StringBuilder node = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < t.Count; i++) {
                var item = t[i];
                //node.Append(",");
                if (item.Count == 0) {
                    node.Add(-1);
                } else {
                    node.Add((short)index);
                    index += item.Count;
                    foreach (var pyNum in item) {
                        sb.Append(",");
                        sb.Append(pyNum);
                    }
                }
            }
            index++;
            node.Add((short)index);
            for (int i = node.Count - 1; i >= 0; i--) {
                if (node[i] == -1) {
                    node[i] = node[i + 1];
                }

            }


            File.WriteAllText("1.txt", string.Join(",", node));
            sb.Remove(0, 1);
            File.WriteAllText("2.txt", sb.ToString());

        }
        //private 
        private static Dictionary<string, string> _dict;

        private static Dictionary<string, string> getDict()
        {
            if (_dict == null) {
                var dict2 = "诘|Ji|揲|Ye|棓|Bei|足|Ju|栟|Ben|咯|Luo|迹|Gui|欻|Chua|耨|Nou|埏|Yan|囋|Can|噭|Chi|案|Wan|燝|Zhu|膻|Dan|汝|Zhuang|艹|Ao|磹|Tan|厖|Pang|观|Guang|窾|Kua|搂|Sou|继|Xu|房|Pang|黮|Shen|愬|Shuo|矜|Guan|盻|Pan|射|Ye|景|Ying|潠|Xun|蓧|Di|黈|Tou|从|Zong|洞|Tong|譳|Rou|鸊|Pi|桁|Hang|槱|Chao|被|Pi|擘|Bai|岂|Kai|铦|Kuo|瑱|Zhen|囝|Nan|嬛|Huan|乐|Lao|崚|Leng|蹻|Jue|浰|Li|摵|Se|梴|Yan|嶰|Jie|谌|Shen|撍|Qian|穞|Lu|黾|Meng|隩|Ao|刓|Liang|墄|Qi|擿|Zhe|能|Nan|居|Ji|及|Xi|揭|Qi|吾|Yu|扐|Cai|刓|Shu|啜|Shu|晻|Yan|兼|Xian|忒|Tei|痁|Dian|莫|Mu|宕|Tan|摘|Ti|灒|Cuan|什|Za|适|Di|逤|Suo|螫|Zhe|伈|Xin|扢|Jie|花|Hu|么|Mo|餧|Si|箐|Jing|禜|Ying|庳|Bei|硾|Chui|燋|Zhuo|棽|Shen|濊|Hun|泽|Shi|漱|Shou|摄|Nie|耆|Shi";
                var sp = dict2.Split('|');
                _dict = new Dictionary<string, string>();
                for (int i = 0; i < sp.Length; i += 2) {
                    _dict[sp[i]] = sp[i + 1];
                }
            }
            return _dict;
        }

        private static int GetPyName2(string key)
        {
            var dict = getDict();
            string py;
            if (dict.TryGetValue(key, out py)) {
                return pyName.IndexOf(py);
            }
            return -1;
        }


        private static int GetPyName(string name)
        {
            name = name.Replace("0", "").Replace("1", "").Replace("2", "").Replace("3", "").Replace("4", "")
                .Replace("5", "").Replace("6", "").Replace("7", "").Replace("8", "").Replace("9", "");
            if (name.Length > 1) {
                name = name[0] + name.Substring(1).ToLower();
            }
            return pyName.IndexOf(name);
        }

        private static List<string> pyName = new List<string>
     {
            "", "A", "Ai", "An", "Ang", "Ao", "Ba", "Bai", "Ban", "Bang", "Bao", "Bei",
             "Ben", "Beng", "Bi", "Bian", "Biao", "Bie", "Bin", "Bing", "Bo", "Bu",
             "Ba", "Cai", "Can", "Cang", "Cao", "Ce", "Ceng", "Cha", "Chai", "Chan",
             "Chang", "Chao", "Che", "Chen", "Cheng", "Chi", "Chong", "Chou", "Chu",
             "Chuai", "Chuan", "Chuang", "Chui", "Chun", "Chuo", "Ci", "Cong", "Cou",
             "Cu", "Cuan", "Cui", "Cun", "Cuo", "Da", "Dai", "Dan", "Dang", "Dao", "De",
             "Deng", "Di", "Dian", "Diao", "Die", "Ding", "Diu", "Dong", "Dou", "Du",
             "Duan", "Dui", "Dun", "Duo", "E", "En", "Er", "Fa", "Fan", "Fang", "Fei",
             "Fen", "Feng", "Fo", "Fou", "Fu", "Ga", "Gai", "Gan", "Gang", "Gao", "Ge",
             "Gei", "Gen", "Geng", "Gong", "Gou", "Gu", "Gua", "Guai", "Guan", "Guang",
             "Gui", "Gun", "Guo", "Ha", "Hai", "Han", "Hang", "Hao", "He", "Hei", "Hen",
             "Heng", "Hong", "Hou", "Hu", "Hua", "Huai", "Huan", "Huang", "Hui", "Hun",
             "Huo", "Ji", "Jia", "Jian", "Jiang", "Jiao", "Jie", "Jin", "Jing", "Jiong",
             "Jiu", "Ju", "Juan", "Jue", "Jun", "Ka", "Kai", "Kan", "Kang", "Kao", "Ke",
             "Ken", "Keng", "Kong", "Kou", "Ku", "Kua", "Kuai", "Kuan", "Kuang", "Kui",
             "Kun", "Kuo", "La", "Lai", "Lan", "Lang", "Lao", "Le", "Lei", "Leng", "Li",
             "Lia", "Lian", "Liang", "Liao", "Lie", "Lin", "Ling", "Liu", "Long", "Lou",
             "Lu", "Lv", "Luan", "Lue", "Lun", "Luo", "Ma", "Mai", "Man", "Mang", "Mao",
             "Me", "Mei", "Men", "Meng", "Mi", "Mian", "Miao", "Mie", "Min", "Ming", "Miu",
             "Mo", "Mou", "Mu", "Na", "Nai", "Nan", "Nang", "Nao", "Ne", "Nei", "Nen",
             "Neng", "Ni", "Nian", "Niang", "Niao", "Nie", "Nin", "Ning", "Niu", "Nong",
             "Nu", "Nv", "Nuan", "Nue", "Nuo", "O", "Ou", "Pa", "Pai", "Pan", "Pang",
             "Pao", "Pei", "Pen", "Peng", "Pi", "Pian", "Piao", "Pie", "Pin", "Ping",
             "Po", "Pu", "Qi", "Qia", "Qian", "Qiang", "Qiao", "Qie", "Qin", "Qing",
             "Qiong", "Qiu", "Qu", "Quan", "Que", "Qun", "Ran", "Rang", "Rao", "Re",
             "Ren", "Reng", "Ri", "Rong", "Rou", "Ru", "Ruan", "Rui", "Run", "Ruo",
             "Sa", "Sai", "San", "Sang", "Sao", "Se", "Sen", "Seng", "Sha", "Shai",
             "Shan", "Shang", "Shao", "She", "Shen", "Sheng", "Shi", "Shou", "Shu",
             "Shua", "Shuai", "Shuan", "Shuang", "Shui", "Shun", "Shuo", "Si", "Song",
             "Sou", "Su", "Suan", "Sui", "Sun", "Suo", "Ta", "Tai", "Tan", "Tang",
             "Tao", "Te", "Teng", "Ti", "Tian", "Tiao", "Tie", "Ting", "Tong", "Tou",
             "Tu", "Tuan", "Tui", "Tun", "Tuo", "Wa", "Wai", "Wan", "Wang", "Wei",
             "Wen", "Weng", "Wo", "Wu", "Xi", "Xia", "Xian", "Xiang", "Xiao", "Xie",
             "Xin", "Xing", "Xiong", "Xiu", "Xu", "Xuan", "Xue", "Xun", "Ya", "Yan",
             "Yang", "Yao", "Ye", "Yi", "Yin", "Ying", "Yo", "Yong", "You", "Yu",
             "Yuan", "Yue", "Yun", "Za", "Zai", "Zan", "Zang", "Zao", "Ze", "Zei",
             "Zen", "Zeng", "Zha", "Zhai", "Zhan", "Zhang", "Zhao", "Zhe", "Zhen",
             "Zheng", "Zhi", "Zhong", "Zhou", "Zhu", "Zhua", "Zhuai", "Zhuan",
             "Zhuang", "Zhui", "Zhun", "Zhuo", "Zi", "Zong", "Zou", "Zu", "Zuan",
             "Zui", "Zun", "Zuo","Pou","Dia","Cen","Dei","Ca","Nve","Lve","Shei","Zhei",
             "Ei","Chua","Nou","Tei"
       };
    }
}
