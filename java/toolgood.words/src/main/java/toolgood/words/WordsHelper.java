package toolgood.words;

import toolgood.words.internals.PinyinDict;
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
    public static String GetFirstPinyin(String text) throws NumberFormatException, IOException {
        return PinyinDict.GetFirstPinyin(text, 0);
    }

    /**
     * 获取拼音全拼, 不支持多音,中文字符集为[0x3400,0x9FD5]，注：偏僻汉字很多未验证 请使用GetPinyin方法，此方法不支持多音
     * 
     * @param text 原文本
     * @param tone 是否带声调
     * @return
     * @throws IOException
     * @throws NumberFormatException
     */
    public static String GetPinyinFast(String text, Boolean tone) throws NumberFormatException, IOException {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < text.length(); i++) {
            Character c = text.charAt(i);
            sb.append(PinyinDict.GetPinyinFast(c, tone ? 1 : 0));
        }
        return sb.toString();
    }

    /**
     * 获取拼音全拼, 不支持多音,中文字符集为[0x3400,0x9FD5]，注：偏僻汉字很多未验证 请使用GetPinyin方法，此方法不支持多音
     * 
     * @param text
     * @return
     * @throws NumberFormatException
     * @throws IOException
     */
    public static String GetPinyinFast(String text) throws NumberFormatException, IOException {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < text.length(); i++) {
            Character c = text.charAt(i);
            sb.append(PinyinDict.GetPinyinFast(c, 0));
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
    public static String GetPinyin(String text, Boolean tone) throws NumberFormatException, IOException {
        return PinyinDict.GetPinyin(text, tone ? 1 : 0);
    }

    /**
     * 获取拼音全拼,支持多音,中文字符集为[0x4E00,0x9FD5]
     * 
     * @param text
     * @return
     * @throws NumberFormatException
     * @throws IOException
     */
    public static String GetPinyin(String text) throws NumberFormatException, IOException {
        return PinyinDict.GetPinyin(text, 0);
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
    public static List<String> GetAllPinyin(char c, Boolean tone) throws NumberFormatException, IOException {
        return PinyinDict.GetAllPinyin(c, tone ? 1 : 0);
    }

    /**
     * 获取所有拼音,中文字符集为[0x3400,0x9FD5]，注：偏僻汉字很多未验证
     * 
     * @param c
     * @return
     * @throws NumberFormatException
     * @throws IOException
     */
    public static List<String> GetAllPinyin(char c) throws NumberFormatException, IOException {
        return PinyinDict.GetAllPinyin(c, 0);
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
    public static String GetPinyinForName(String name, Boolean tone) throws NumberFormatException, IOException {
        return String.join("", PinyinDict.GetPinyinForName(name, tone ? 1 : 0));
    }

    /**
     * 获取姓名拼音,中文字符集为[0x3400,0x9FD5]，注：偏僻汉字很多未验证
     * 
     * @param name
     * @return
     * @throws NumberFormatException
     * @throws IOException
     */
    public static String GetPinyinForName(String name) throws NumberFormatException, IOException {
        return String.join("", PinyinDict.GetPinyinForName(name, 0));
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
    public static List<String> GetPinyinListForName(String name, Boolean tone)
            throws NumberFormatException, IOException {
        return PinyinDict.GetPinyinForName(name, tone ? 1 : 0);
    }

    /**
     * 获取姓名拼音,中文字符集为[0x3400,0x9FD5]，注：偏僻汉字很多未验证
     * @param name
     * @return
     * @throws NumberFormatException
     * @throws IOException
     */
    public static List<String> GetPinyinListForName(String name) throws NumberFormatException, IOException {
        return PinyinDict.GetPinyinForName(name, 0);
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