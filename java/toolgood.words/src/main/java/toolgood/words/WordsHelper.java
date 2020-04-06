package toolgood.words;

import toolgood.words.internals.Dict;
import toolgood.words.internals.PinYinDict;
import toolgood.words.internals.Translate;

import java.io.IOException;
import java.util.List;
import java.util.regex.Pattern;

public class WordsHelper {

    /**
     * 获取首字母，中文字符集为[0x3400,0x9FD5]，注：偏僻汉字很多未验证
     * 
     * @param text 原文本
     * @return
     * @throws IOException
     * @throws NumberFormatException
     */
    public static String GetFirstPinYin(String text) throws NumberFormatException, IOException {
        return PinYinDict.GetFirstPinYin(text, 0);
    }

    /**
     * 获取拼音全拼, 不支持多音,中文字符集为[0x3400,0x9FD5]，注：偏僻汉字很多未验证 请使用GetPinYin方法，此方法不支持多音
     * 
     * @param text 原文本
     * @param tone 是否带声调
     * @return
     * @throws IOException
     * @throws NumberFormatException
     */
    public static String GetPinYinFast(String text, Boolean tone) throws NumberFormatException, IOException {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < text.length(); i++) {
            Character c = text.charAt(i);
            sb.append(PinYinDict.GetPinYinFast(c, tone ? 1 : 0));
        }
        return sb.toString();
    }

    /**
     * 获取拼音全拼, 不支持多音,中文字符集为[0x3400,0x9FD5]，注：偏僻汉字很多未验证 请使用GetPinYin方法，此方法不支持多音
     * 
     * @param text
     * @return
     * @throws NumberFormatException
     * @throws IOException
     */
    public static String GetPinYinFast(String text) throws NumberFormatException, IOException {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < text.length(); i++) {
            Character c = text.charAt(i);
            sb.append(PinYinDict.GetPinYinFast(c, 0));
        }
        return sb.toString();
    }

    /**
     * 获取拼音全拼,支持多音,中文字符集为[0x4E00,0x9FD5]
     * 
     * @param text 原文本
     * @param tone 是否带声调
     * @return
     * @throws NumberFormatException
     * @throws IOException
     */
    public static String GetPinYin(String text, Boolean tone) throws NumberFormatException, IOException {
        return PinYinDict.GetPinYin(text, tone ? 1 : 0);
    }

    /**
     * 获取拼音全拼,支持多音,中文字符集为[0x4E00,0x9FD5]
     * 
     * @param text
     * @return
     * @throws NumberFormatException
     * @throws IOException
     */
    public static String GetPinYin(String text) throws NumberFormatException, IOException {
        return PinYinDict.GetPinYin(text, 0);
    }

    /**
     * 获取所有拼音,中文字符集为[0x3400,0x9FD5]，注：偏僻汉字很多未验证
     * 
     * @param c    原文本
     * @param tone 是否带声调
     * @return
     * @throws NumberFormatException
     * @throws IOException
     */
    public static List<String> GetAllPinYin(char c, Boolean tone) throws NumberFormatException, IOException {
        return PinYinDict.GetAllPinYin(c, tone ? 1 : 0);
    }

    /**
     * 获取所有拼音,中文字符集为[0x3400,0x9FD5]，注：偏僻汉字很多未验证
     * 
     * @param c
     * @return
     * @throws NumberFormatException
     * @throws IOException
     */
    public static List<String> GetAllPinYin(char c) throws NumberFormatException, IOException {
        return PinYinDict.GetAllPinYin(c, 0);
    }

    /**
     * 获取姓名拼音,中文字符集为[0x3400,0x9FD5]，注：偏僻汉字很多未验证
     * 
     * @param name 姓名
     * @param tone 是否带声调
     * @return
     * @throws NumberFormatException
     * @throws IOException
     */
    public static String GetPinYinForName(String name, Boolean tone) throws NumberFormatException, IOException {
        return String.join("", PinYinDict.GetPinYinForName(name, tone ? 1 : 0));
    }

    /**
     * 获取姓名拼音,中文字符集为[0x3400,0x9FD5]，注：偏僻汉字很多未验证
     * 
     * @param name
     * @return
     * @throws NumberFormatException
     * @throws IOException
     */
    public static String GetPinYinForName(String name) throws NumberFormatException, IOException {
        return String.join("", PinYinDict.GetPinYinForName(name, 0));
    }

    /**
     * 获取姓名拼音,中文字符集为[0x3400,0x9FD5]，注：偏僻汉字很多未验证
     * 
     * @param name 姓名
     * @param tone 是否带声调
     * @return
     * @throws NumberFormatException
     * @throws IOException
     */
    public static List<String> GetPinYinListForName(String name, Boolean tone)
            throws NumberFormatException, IOException {
        return PinYinDict.GetPinYinForName(name, tone ? 1 : 0);
    }

    /**
     * 获取姓名拼音,中文字符集为[0x3400,0x9FD5]，注：偏僻汉字很多未验证
     * @param name
     * @return
     * @throws NumberFormatException
     * @throws IOException
     */
    public static List<String> GetPinYinListForName(String name) throws NumberFormatException, IOException {
        return PinYinDict.GetPinYinForName(name, 0);
    }

    /**
     * 转成 侦测字符串 1、转小写;2、全角转半角; 3、相似文字修改；4、繁体转简体
     * 
     * @param s
     * @return
     */
    public static String ToSenseIllegalWords(String s) {
        StringBuilder ts = new StringBuilder(s);
        for (int i = 0; i < s.length(); i++) {
            char c = s.charAt(i);
            if (c < 'A') {
            } else if (c <= 'Z') {
                ts.setCharAt(i, (char) (c | 0x20));
            } else if (c < 9450) {
            } else if (c <= 12840) {// 处理数字
                int index = Dict.nums1.indexOf(c);
                if (index > -1) {
                    ts.setCharAt(i, Dict.nums2.charAt(index));
                }
            } else if (c == 12288) {
                ts.setCharAt(i, ' ');
            } else if (c < 0x4e00) {
            } else if (c <= 0x9fa5) {
                char k = Dict.Simplified.charAt(c - 0x4e00);
                if (k != c) {
                    ts.setCharAt(i, k);
                }
            } else if (c < 65280) {
            } else if (c < 65375) {
                char k = (char) (c - 65248);
                if ('A' <= k && k <= 'Z') {
                    k = (char) (k | 0x20);
                }
                ts.setCharAt(i, k);
            }
        }
        return ts.toString();
    }

    /**
     * 判断输入是否为中文 ,中文字符集为[0x4E00,0x9FA5]
     * 
     * @param content
     * @return
     */
    public static boolean HasChinese(String content) {
        return Pattern.matches("[\\u3400-\\u4db5\\u4e00-\\u9fd5]", content);
    }

    /**
     * 判断输入是否全为中文,中文字符集为[0x4E00,0x9FA5]
     * 
     * @param content
     * @return
     */
    public static boolean IsAllChinese(String content) {
        return Pattern.matches("^[\\u3400-\\u4db5\\u4e00-\\u9fd5]*$", content);
    }

    /**
     * 判断含有英语
     * 
     * @param content
     * @return
     */
    public static boolean HasEnglish(String content) {
        return Pattern.matches("[A-Za-z]", content);
    }

    /**
     * 判断是否全部英语
     * 
     * @param content
     * @return
     */
    public static boolean IsAllEnglish(String content) {
        return Pattern.matches("^[A-Za-z]*$", content);
    }

    /**
     * 半角转全角
     * 
     * @param input
     * @return
     */
    public static String ToSBC(String input) {
        StringBuilder sb = new StringBuilder(input);
        for (int i = 0; i < input.length(); i++) {
            char c = input.charAt(i);
            if (c == 32) {
                sb.setCharAt(i, (char) 12288);
            } else if (c < 127) {
                sb.setCharAt(i, (char) (c + 65248));
            }
        }
        return sb.toString();
    }

    /**
     * 转半角的函数
     * 
     * @param input
     * @return
     */
    public static String ToDBC(String input) {
        StringBuilder sb = new StringBuilder(input);
        for (int i = 0; i < input.length(); i++) {
            char c = input.charAt(i);
            if (c == 12288) {
                sb.setCharAt(i, (char) 32);
            } else if (c > 65280 && c < 65375) {
                sb.setCharAt(i, (char) (c - 65248));
            }
        }
        return sb.toString();
    }

    /**
     * 转繁体中文
     * 
     * @param text
     * @return
     * @throws Exception
     */
    public static String ToTraditionalChinese(String text) throws Exception {
        return Translate.ToTraditionalChinese(text, 0);
    }

    /**
     * 转繁体中文
     * 
     * @param text
     * @param type 0、繁体中文，1、港澳繁体，2、台湾正体
     * @return
     * @throws Exception
     */
    public static String ToTraditionalChinese(String text, int type) throws Exception {
        return Translate.ToTraditionalChinese(text, type);
    }

    /***
     * 转简体中文
     * 
     * @param text
     * @return
     * @throws Exception
     */
    public static String ToSimplifiedChinese(String text) throws Exception {
        return Translate.ToSimplifiedChinese(text, 0);
    }

    /**
     * 转简体中文
     * 
     * @param text
     * @param srcType 0、繁体中文，1、港澳繁体，2、台湾正体
     * @return
     * @throws Exception
     */
    public static String ToSimplifiedChinese(String text, int srcType) throws Exception {
        return Translate.ToSimplifiedChinese(text, srcType);
    }

}