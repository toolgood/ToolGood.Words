using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Sinan.Util
{
    /// <summary>
    /// 参考了MONO的HashSet,针对字符串进行优化
    /// 增加了检查部分字符串的方法,可避免字符分割带来的GC问题
    /// Contains(String item, int offset, int len)
    /// </summary>
    public class HashStringSet
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct Slot
        {
            internal int hashCode;
            internal String value;
            internal int next;
        }

        // Fields
        private int[] m_buckets;
        private int m_count;
        private int m_freeList;
        private int m_lastIndex;
        private Slot[] m_slots;


        public HashStringSet()
        {
            this.m_lastIndex = 0;
            this.m_count = 0;
            this.m_freeList = -1;
        }

        public bool Add(String value)
        {
            int freeList;
            if (this.m_buckets == null)
            {
                this.Initialize(0);
            }
            int hashCode = this.InternalGetHashCode(value);
            int index = hashCode % this.m_buckets.Length;
            for (int i = this.m_buckets[hashCode % this.m_buckets.Length] - 1; i >= 0; i = this.m_slots[i].next)
            {
                if ((this.m_slots[i].hashCode == hashCode) && this.m_slots[i].value.Equals(value))
                {
                    return false;
                }
            }
            if (this.m_freeList >= 0)
            {
                freeList = this.m_freeList;
                this.m_freeList = this.m_slots[freeList].next;
            }
            else
            {
                if (this.m_lastIndex == this.m_slots.Length)
                {
                    this.IncreaseCapacity();
                    index = hashCode % this.m_buckets.Length;
                }
                freeList = this.m_lastIndex;
                this.m_lastIndex++;
            }
            this.m_slots[freeList].hashCode = hashCode;
            this.m_slots[freeList].value = value;
            this.m_slots[freeList].next = this.m_buckets[index] - 1;
            this.m_buckets[index] = freeList + 1;
            this.m_count++;
            return true;
        }

        private void IncreaseCapacity()
        {
            int min = this.m_count * 2;
            if (min < 0)
            {
                min = this.m_count;
            }
            int prime = HashHelpers.GetPrime(min);
            Slot[] destinationArray = new Slot[prime];
            if (this.m_slots != null)
            {
                Array.Copy(this.m_slots, 0, destinationArray, 0, this.m_lastIndex);
            }
            int[] numArray = new int[prime];
            for (int i = 0; i < this.m_lastIndex; i++)
            {
                int index = destinationArray[i].hashCode % prime;
                destinationArray[i].next = numArray[index] - 1;
                numArray[index] = i + 1;
            }
            this.m_slots = destinationArray;
            this.m_buckets = numArray;
        }

        public void Clear()
        {
            if (this.m_lastIndex > 0)
            {
                Array.Clear(this.m_slots, 0, this.m_lastIndex);
                Array.Clear(this.m_buckets, 0, this.m_buckets.Length);
                this.m_lastIndex = 0;
                this.m_count = 0;
                this.m_freeList = -1;
            }
        }

        public bool Contains(String item)
        {
            if (this.m_buckets != null)
            {
                int hashCode = this.InternalGetHashCode(item);
                for (int i = this.m_buckets[hashCode % this.m_buckets.Length] - 1; i >= 0; i = this.m_slots[i].next)
                {
                    if ((this.m_slots[i].hashCode == hashCode) && this.m_slots[i].value.Equals(item))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void Initialize(int capacity)
        {
            int prime = HashHelpers.GetPrime(capacity);
            this.m_buckets = new int[prime];
            this.m_slots = new Slot[prime];
        }

        //重写原来的HashCode
        private int InternalGetHashCode(String item)
        {
            return InternalGetHashCode(item, 0, item.Length);
        }

        public bool Remove(String item)
        {
            if (this.m_buckets != null)
            {
                int hashCode = this.InternalGetHashCode(item);
                int index = hashCode % this.m_buckets.Length;
                int num3 = -1;
                for (int i = this.m_buckets[index] - 1; i >= 0; i = this.m_slots[i].next)
                {
                    if ((this.m_slots[i].hashCode == hashCode) && this.m_slots[i].value.Equals(item))
                    {
                        if (num3 < 0)
                        {
                            this.m_buckets[index] = this.m_slots[i].next + 1;
                        }
                        else
                        {
                            this.m_slots[num3].next = this.m_slots[i].next;
                        }
                        this.m_slots[i].hashCode = -1;
                        this.m_slots[i].value = null;
                        this.m_slots[i].next = this.m_freeList;
                        this.m_count--;
                        if (this.m_count == 0)
                        {
                            this.m_lastIndex = 0;
                            this.m_freeList = -1;
                        }
                        else
                        {
                            this.m_freeList = i;
                        }
                        return true;
                    }
                    num3 = i;
                }
            }
            return false;
        }

        #region 新增方法,避免字符分割
        public bool Contains(String item, int offset, int len)
        {
            if (this.m_buckets != null)
            {
                int hashCode = InternalGetHashCode(item, offset, len);
                for (int i = this.m_buckets[hashCode % this.m_buckets.Length] - 1; i >= 0; i = this.m_slots[i].next)
                {
                    if ((this.m_slots[i].hashCode == hashCode) && StringEquals(this.m_slots[i].value, item, offset, len))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        static unsafe int InternalGetHashCode(String item, int offset, int len)
        {
            int h = 0;
            fixed (char* c = item)
            {
                char* cc = c + offset;
                char* end = cc + len - 1;
                for (; cc < end; cc += 2)
                {
                    h = (h << 5) - h + *cc;
                    h = (h << 5) - h + cc[1];
                }
                ++end;
                if (cc < end)
                    h = (h << 5) - h + *cc;
            }
            return h & 0x7fffffff;
        }

        static unsafe bool StringEquals(string a, string b, int offset, int len)
        {
            if (len != a.Length)
                return false;

            fixed (char* s1 = a, s2 = b)
            {
                char* s1_ptr = s1;
                char* s2_ptr = s2 + offset;

                if ((offset & 0x01) == 0) //偏移位置为偶数
                {
                    while (len > 1)
                    {
                        if (((int*)s1_ptr)[0] != ((int*)s2_ptr)[0])
                            return false;

                        s1_ptr += 2;
                        s2_ptr += 2;
                        len -= 2;
                    }
                }
                else //偏移位置为奇数
                {
                    while (len > 1)
                    {
                        if ((s1_ptr)[0] != (s2_ptr)[0] ||
                            (s1_ptr)[1] != (s2_ptr)[1])
                            return false;

                        s1_ptr += 2;
                        s2_ptr += 2;
                        len -= 2;
                    }
                }
                return len == 0 || *s1_ptr == *s2_ptr;
            }
        }
        #endregion
    }
}
