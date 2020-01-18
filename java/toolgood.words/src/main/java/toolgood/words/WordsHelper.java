package toolgood.words;

import toolgood.words.internals.Dict;

import java.util.regex.Pattern;


public class WordsHelper{

    /**
     * 转成 侦测字符串 
     * 1、转小写;2、全角转半角; 3、相似文字修改；4、繁体转简体
     * @param s
     * @return
     */
    public static String ToSenseIllegalWords(String s)
    {
        StringBuilder ts = new StringBuilder(s);
        for (int i = 0; i < s.length(); i++) {
            char c = s.charAt(i);
            if (c < 'A') { } else if (c <= 'Z') {
                ts.setCharAt(i, (char)(c | 0x20));
            } else if (c < 9450) { } else if (c <= 12840) {//处理数字 
                int index = Dict.nums1.indexOf(c);
                if (index > -1) { ts.setCharAt(i, Dict.nums2.charAt(index));  }
            } else if (c == 12288) {
                ts.setCharAt(i, ' ');
            } else if (c < 0x4e00) { } else if (c <= 0x9fa5) {
                char k = Dict.Simplified.charAt(c-0x4e00) ;
                if (k != c) {
                    ts.setCharAt(i,k);
                }
            } else if (c < 65280) { } else if (c < 65375) {
                char k = (char)(c - 65248);
                if ('A' <= k && k <= 'Z') { k =(char)(k | 0x20) ; }
                ts.setCharAt(i,k);
            }
        }
        return ts.toString();
    }
  
    /**
     * 判断输入是否为中文  ,中文字符集为[0x4E00,0x9FA5]
     * @param content
     * @return
     */
    public static boolean HasChinese(String content)
    {
        return Pattern.matches("[\\u4e00-\\u9fa5]", content);
    }
    /**
     * 判断输入是否全为中文,中文字符集为[0x4E00,0x9FA5]
     * @param content
     * @return
     */
    public static boolean IsAllChinese(String content)
    {
        return Pattern.matches("^[\\u4e00-\\u9fa5]*$", content);
    }
    /**
     * 判断含有英语
     * @param content
     * @return
     */
    public static boolean HasEnglish(String content)
    {
        return Pattern.matches("[A-Za-z]", content);
    }
    /**
     * 判断是否全部英语
     * @param content
     * @return
     */
    public static boolean IsAllEnglish(String content)
    {
        return Pattern.matches("^[A-Za-z]*$", content);
    }
    /**
     * 半角转全角
     * @param input
     * @return
     */
    public static String ToSBC(String input)
    {
        StringBuilder sb = new StringBuilder(input);
        for (int i = 0; i < input.length(); i++) {
            char c = input.charAt(i);
            if (c == 32) {
                sb.setCharAt(i, (char)12288);
            } else if (c < 127) {
                sb.setCharAt(i, (char)(c + 65248));
            }
        }
        return sb.toString();
    }
    /**
     * 转半角的函数
     * @param input
     * @return
     */
    public static String ToDBC(String input)
    {
        StringBuilder sb = new StringBuilder(input);
        for (int i = 0; i < input.length(); i++) {
            char c = input.charAt(i);
            if (c == 12288) {
                sb.setCharAt(i, (char)32);
            } else if (c > 65280 && c < 65375) {
                sb.setCharAt(i, (char)(c - 65248));
            }
        }
        return sb.toString();
    }
    /**
     * 转繁体中文
     * @param text
     * @return
     */
    public static String ToTraditionalChinese(String text)
    {
        StringBuilder sb = new StringBuilder(text);
        for (int i = 0; i < text.length(); i++) {
            char c = text.charAt(i);
            if (c >= 0x4e00 && c <= 0x9fa5) {
                char k = Dict.Traditional.charAt(c - 0x4e00) ;
                if (k != c) {
                    sb.setCharAt(i,k);
                }
            }
        }
        return sb.toString();
    }

    /***
     * 转简体中文
     * @param text
     * @return
     */
    public static String ToSimplifiedChinese(String text)
    {
        StringBuilder sb = new StringBuilder(text);
        for (int i = 0; i < text.length(); i++) {
            char c = text.charAt(i);
            if (c >= 0x4e00 && c <= 0x9fa5) {
                char k = Dict.Simplified.charAt(c - 0x4e00) ;
                if (k != c) {
                    sb.setCharAt(i,k);
                }
            }
        }
        return sb.toString();
    }

}