package toolgood.words;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.HashMap;
import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;

/**
 * @author yongxuan.he
 * @date 2020/3/17
 */
public class NumHelper {

    public enum SerializableType {
        /** 大小在-128~127之间的整数，占用空间为1字节 */
        TINY_INT(1),
        /** 大小在-32768~32767之间的整数，占用空间为2字节 */
        SMALL_INT(2),
        /** 大小在-8388608~8388607之间的整数，占用空间为3字节 */
        MEDIUM_INT(3),
        /** int类型 */
        INT(4),
        ;

        private final int flag;

        private static Map<Integer, SerializableType> typeMap;
        static {
            Map<Integer, SerializableType> tmpMap = new HashMap<>();
            for (SerializableType type : SerializableType.values()) {
                tmpMap.put(type.getFlag(), type);
            }
            typeMap = tmpMap;
        }
        public static SerializableType getType(int flag) {
            return typeMap.get(flag);
        }

        SerializableType(int flag) {
            this.flag = flag;
        }

        public int getFlag() {
            return flag;
        }
    }

    private interface Serializer {
        byte[] serialize(int a);
    }

    private static Serializer tinyIntWriter = v -> {
        if (v < -128 || v > 127) {
            throw new RuntimeException("not tinyInt: " + v);
        }
        ByteArrayOutputStream out = new ByteArrayOutputStream();
        out.write(v);
        return out.toByteArray();
    };
    private static Serializer smallIntWriter = v -> {
        if (v < -32768 || v > 32767) {
            throw new RuntimeException("not smallInt: " + v);
        }
        ByteArrayOutputStream out = new ByteArrayOutputStream();
        out.write((v >>> 8) & 0xFF);
        out.write(v & 0xFF);
        return out.toByteArray();
    };
    private static Serializer mediumIntWriter = v -> {
        if (v < -8388608 || v > 8388607) {
            throw new RuntimeException("not mediumInt: " + v);
        }
        ByteArrayOutputStream out = new ByteArrayOutputStream();
        out.write((v >>> 16) & 0xFF);
        out.write((v >>> 8) & 0xFF);
        out.write(v & 0xFF);
        return out.toByteArray();
    };

    private static Serializer intWriter = v -> {
        ByteArrayOutputStream out = new ByteArrayOutputStream();
        out.write((v >>> 24) & 0xFF);
        out.write((v >>> 16) & 0xFF);
        out.write((v >>> 8) & 0xFF);
        out.write(v & 0xFF);
        return out.toByteArray();
    };

    private static final Map<SerializableType, Serializer> simpleWriterMap = new ConcurrentHashMap<>();
    static {
        simpleWriterMap.put(SerializableType.TINY_INT, tinyIntWriter);
        simpleWriterMap.put(SerializableType.SMALL_INT, smallIntWriter);
        simpleWriterMap.put(SerializableType.MEDIUM_INT, mediumIntWriter);
        simpleWriterMap.put(SerializableType.INT, intWriter);
    }

    public static byte[] serialize(int v) {
        ByteArrayOutputStream out = new ByteArrayOutputStream();
        Serializer serializer;
        int typeFlag;

        if(v >= -128 && v<= 127) {
            serializer = simpleWriterMap.get(SerializableType.TINY_INT);
            typeFlag = SerializableType.TINY_INT.getFlag();
        } else if(v >= -32768 && v <= 32767) {
            serializer = simpleWriterMap.get(SerializableType.SMALL_INT);
            typeFlag = SerializableType.SMALL_INT.getFlag();
        } else if(v >= -8388608 && v <= 8388607){
            serializer = simpleWriterMap.get(SerializableType.MEDIUM_INT);
            typeFlag = SerializableType.MEDIUM_INT.getFlag();
        } else {
            serializer = simpleWriterMap.get(SerializableType.INT);
            typeFlag = SerializableType.INT.getFlag();
        }
        out.write(typeFlag);
        byte[] bytes = serializer.serialize(v);
        out.write(bytes, 0, bytes.length);
        return out.toByteArray();
    }

    public interface Deserializer {
        int deserialize(InputStream in) throws IOException;
    }

    private static Deserializer tinyIntReader = in -> {
        int ch = in.read();
        if (ch < 0)
            throw new RuntimeException("deserializing");
        if ((0x80 & ch) == 0x80) {
            ch = 0xffffff00 | ch;
        }
        return ch;
    };
    private static Deserializer smallIntReader = in -> {
        int ch1 = in.read();
        int ch2 = in.read();
        if ((ch1 | ch2) < 0)
            throw new RuntimeException("deserializing");
        int ch = (ch1 << 8) + ch2;
        if ((0x8000 & ch) == 0x8000) {
            ch = 0xffff0000 | ch;
        }
        return ch;
    };
    private static Deserializer mediumIntReader = in -> {
        int ch1 = in.read();
        int ch2 = in.read();
        int ch3 = in.read();
        if ((ch1 | ch2 | ch3) < 0)
            throw new RuntimeException("deserializing");
        int ch = (ch1 << 16) + (ch2 << 8) + (ch3);
        if ((0x800000 & ch) == 0x800000) {
            ch = 0xff000000 | ch;
        }
        return ch;
    };

    private static Deserializer intReader = in -> {
        int ch1 = in.read();
        int ch2 = in.read();
        int ch3 = in.read();
        int ch4 = in.read();
        if ((ch1 | ch2 | ch3 | ch4) < 0)
            throw new RuntimeException("deserializing");
        return ((ch1 << 24) + (ch2 << 16) + (ch3 << 8) + ch4);
    };

    private static final Map<SerializableType, Deserializer> simpleReaderMap = new ConcurrentHashMap<>();
    static {
        simpleReaderMap.put(SerializableType.TINY_INT, tinyIntReader);
        simpleReaderMap.put(SerializableType.SMALL_INT, smallIntReader);
        simpleReaderMap.put(SerializableType.MEDIUM_INT, mediumIntReader);
        simpleReaderMap.put(SerializableType.INT, intReader);
    }

    public static int read(InputStream in) throws IOException {
        int flag = in.read();
        SerializableType type = SerializableType.getType(flag);
        Deserializer deserializer = simpleReaderMap.get(type);

        if(deserializer == null) {
            throw new RuntimeException("wrong flag: " + flag);
        }
        return deserializer.deserialize(in);
    }
}
