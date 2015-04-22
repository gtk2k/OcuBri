using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OcuBri
{
    static class Serializer
    {
        static List<byte> buffer = new List<byte>();

        #region public methods

        public static byte[] Serialize(this object data)
        {
            buffer.Clear();

            Type t = data.GetType();
            var props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var flds = t.GetFields(BindingFlags.Public | BindingFlags.Instance);
            buffer.Add((byte)TypeCode.Object);
            buffer.Add((byte)(props.Count() + flds.Count()));
            foreach (var pi in props)
            {
                AddString(pi.Name);
                var tc = Type.GetTypeCode(pi.PropertyType);
                AddData(tc, pi.GetValue(data));
            }
            foreach (var fld in flds)
            {
                AddString(fld.Name);
                var tc = Type.GetTypeCode(fld.FieldType);
                AddData(tc, fld.GetValue(data));
            }

            return ToByteArray();
        }

        public static byte[] Serialize(this IDictionary data)
        {
            buffer.Clear();

            buffer.Add((byte)TypeCode.Object);
            buffer.Add((byte)data.Keys.Count);
            foreach (var key in data.Keys)
            {
                AddString(key.ToString());
                var tc = Type.GetTypeCode(data[key].GetType());
                AddData(tc, data[key]);
            }

            return ToByteArray();
        }

        public static byte[] Serialize<T>(this T data) where T : IEnumerable
        {
            buffer.Clear();

            buffer.Add((byte)0xff);
            var length = (byte)data.Cast<object>().Count();
            buffer.Add((byte)length);
            foreach (var item in data)
            {
                var tc = Type.GetTypeCode(item.GetType());
                AddData(tc, item);
            }

            return ToByteArray();
        }

        #endregion

        #region private methods
        
        static void AddBool(bool data)
        {
            buffer.Add((byte)TypeCode.Boolean);
            buffer.Add((byte)(data ? 1 : 0));
        }

        static void AddChar(char data)
        {
            buffer.Add((byte)TypeCode.Char);
            buffer.Add(Encoding.UTF8.GetBytes(data.ToString())[0]);
        }

        static void AddSByteOrInt(int data)
        {
            if ((uint)byte.MinValue <= data && (uint)byte.MaxValue >= data)
            {
                AddSByte((sbyte)data);
            }
            else if (Int16.MinValue <= data && Int16.MaxValue >= data)
            {
                AddInt16((Int16)data);
            }
            else if (Int32.MinValue < data && Int32.MaxValue >= data)
            {
                AddInt32((Int32)data);
            }
        }

        static void AddByteOrUInt(uint data)
        {
            if ((uint)byte.MinValue <= data && (uint)byte.MaxValue >= data)
            {
                AddByte((byte)data);
            }
            else if (UInt16.MinValue <= data && UInt16.MaxValue >= data)
            {
                AddUInt16((UInt16)data);
            }
            else if (UInt32.MaxValue > data)
            {
                AddUInt32((UInt32)data);
            }
        }

        static void AddSingle(Single data)
        {
            buffer.Add((byte)TypeCode.Single);
            buffer.AddRange(BitConverter.GetBytes(data));
        }

        static void AddDouble(double data)
        {
            buffer.Add((byte)TypeCode.Double);
            buffer.AddRange(BitConverter.GetBytes(data));
        }

        static void AddString(string data)
        {
            buffer.Add((byte)TypeCode.String);
            if (data.Length == 0)
            {
                buffer.Add(0);
                return;
            }
            else
            {
                byte byteCount = (byte)Encoding.UTF8.GetByteCount(data);
                buffer.Add(byteCount);
                var bytes = Encoding.UTF8.GetBytes(data);
                buffer.AddRange(bytes);
            }
        }

        static byte[] ToByteArray()
        {
            var ret = buffer.ToArray();
            buffer.Clear();
            return ret;
        }

        static void AddByte(byte data)
        {
            buffer.Add((byte)TypeCode.Byte);
            buffer.Add(data);
        }

        static void AddSByte(sbyte data)
        {
            buffer.Add((byte)TypeCode.SByte);
            buffer.Add((byte)data);
        }

        static void AddUInt16(UInt16 data)
        {
            buffer.Add((byte)TypeCode.UInt16);
            buffer.AddRange(BitConverter.GetBytes(data));
        }

        static void AddInt16(Int16 data)
        {
            buffer.Add((byte)TypeCode.Int16);
            buffer.AddRange(BitConverter.GetBytes(data));
        }

        static void AddUInt32(UInt32 data)
        {
            buffer.Add((byte)TypeCode.UInt32);
            buffer.AddRange(BitConverter.GetBytes(data));
        }

        static void AddInt32(Int32 data)
        {
            buffer.Add((byte)TypeCode.Int32);
            buffer.AddRange(BitConverter.GetBytes(data));
        }

        static void AddData(TypeCode tc, object data)
        {
            switch (tc)
            {
                case TypeCode.Boolean:
                    AddBool((bool)data);
                    break;
                case TypeCode.Char:
                    AddChar((char)data);
                    break;
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                    AddSByteOrInt((int)data);
                    break;
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                    AddByteOrUInt(Convert.ToUInt32(data));
                    break;
                case TypeCode.Single:
                    AddSingle((float)data);
                    break;
                case TypeCode.Double:
                    AddDouble((double)data);
                    break;
                case TypeCode.String:
                    AddString((string)data);
                    break;
                case TypeCode.Object:
                    if (data is IDictionary)
                    {
                        Serialize((IDictionary)data);
                    }
                    else if (data is IEnumerable)
                    {
                        Serialize((IEnumerable)data);
                    }
                    else
                    {
                        Serialize((object)data);
                    }
                    break;
            }
        }

        #endregion
    }
}
