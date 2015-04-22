using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OcuBri
{
    class SendDataBuilder_debug
    {
        List<byte> buffer = new List<byte>();

        #region public methods

        public void AddObject(object data)
        {
            Type t = data.GetType();
            var props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var flds = t.GetFields(BindingFlags.Public | BindingFlags.Instance);
            buffer.Add((byte)TypeCode.Object);
            Console.WriteLine(((byte)TypeCode.Object) + " Object Type");
            buffer.Add((byte)(props.Count() + flds.Count()));
            Console.WriteLine((byte)(props.Count() + flds.Count()) + " Member Count");
            foreach (var pi in props)
            {
                AddString(pi.Name);
                switch (Type.GetTypeCode(pi.PropertyType))
                {
                    case TypeCode.Char:
                        AddChar((char)pi.GetValue(data));
                        break;
                    case TypeCode.SByte:
                        AddSByte((sbyte)pi.GetValue(data));
                        break;
                    case TypeCode.Byte:
                        AddByte((byte)pi.GetValue(data));
                        break;
                    case TypeCode.Int16:
                        AddInt16((short)pi.GetValue(data));
                        break;
                    case TypeCode.UInt16:
                        AddUInt16((ushort)pi.GetValue(data));
                        break;
                    case TypeCode.Int32:
                        AddInt32((int)pi.GetValue(data));
                        break;
                    case TypeCode.UInt32:
                        AddUInt32((uint)pi.GetValue(data));
                        break;
                }
            }
            foreach (var fld in flds)
            {
                AddString(fld.Name);
                var tc = Type.GetTypeCode(fld.FieldType);
                switch (tc)
                {
                    case TypeCode.Boolean:
                        AddBool((bool)fld.GetValue(data));
                        break;
                    case TypeCode.Char:
                        AddChar((char)fld.GetValue(data));
                        break;
                    case TypeCode.SByte:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                        AddSByteOrInt((int)fld.GetValue(data));
                        break;
                    case TypeCode.Byte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                        AddByteOrUInt(Convert.ToUInt32(fld.GetValue(data)));
                        break;
                    case TypeCode.Single:
                        AddSingle((float)fld.GetValue(data));
                        break;
                    case TypeCode.Double:
                        AddDouble((double)fld.GetValue(data));
                        break;
                    case TypeCode.String:
                        AddString((string)fld.GetValue(data));
                        break;
                    case TypeCode.Object:
                        var val = fld.GetValue(data);
                        if (val is IEnumerable)
                        {
                            AddArray((IEnumerable)val);
                        }
                        else
                        {
                            AddObject((object)val);
                        }
                        break;
                }
            }

        }

        public void AddArray(sbyte[] data)
        {
            buffer.Add((byte)(TypeCode.SByte));
            Console.WriteLine((byte)TypeCode.SByte + " SByte Array");
            buffer.Add((byte)data.Length);
            Console.WriteLine((byte)data.Length + " SByte Array Length");
            foreach (var item in data)
            {
                buffer.Add((byte)item);
                Console.WriteLine(item + " " + (byte)item + " Data"); 
            }
        }

        public void AddArray(byte[] data)
        {
            buffer.Add((byte)(TypeCode.Byte));
            Console.WriteLine((byte)TypeCode.SByte + " Byte Array");
            buffer.Add((byte)data.Length);
            Console.WriteLine((byte)data.Length + " Byte Array Length");
            foreach (var item in data)
            {
                buffer.Add(item);
                Console.WriteLine(item + " Data");
            }
        }
        
        public void AddArray<T>(T data) where T : IEnumerable
        {
            buffer.Add((byte)0xff);
            Console.WriteLine(0xff + " Array Type");
            var length = (byte)data.Cast<object>().Count();
            buffer.Add((byte)length);
            Console.WriteLine(length + " Array Length" );
            foreach (var item in data)
            {
                var tc = Type.GetTypeCode(item.GetType());
                switch (tc)
                {
                    case TypeCode.Boolean:
                        AddBool((bool)item);
                        break;
                    case TypeCode.Char:
                        AddChar((char)item);
                        break;
                    case TypeCode.SByte:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                        AddSByteOrInt((int)item);
                        break;
                    case TypeCode.Byte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                        AddByteOrUInt((uint)item);
                        break;
                    case TypeCode.Single:
                        AddSingle((float)item);
                        break;
                    case TypeCode.Double:
                        AddDouble((double)item);
                        break;
                    case TypeCode.String:
                        AddString((string)item);
                        break;
                    case TypeCode.Object:
                        if (item is IEnumerable)
                        {
                            AddArray(item as IEnumerable);
                        }
                        else
                        {
                            AddObject((object)item);
                        }
                        break;

                }
            }
        }


        public void AddBool(bool data)
        {
            buffer.Add((byte)TypeCode.Boolean);
            Console.WriteLine((byte)TypeCode.Boolean + " Boolean Type");
            buffer.Add((byte)(data ? 1 : 0));
            Console.WriteLine(((byte)(data ? 1 : 0)) + "(" + data + ") Data");
        }

        public void AddChar(char data)
        {
            buffer.Add((byte)TypeCode.Char);
            Console.WriteLine((byte)TypeCode.Boolean + " Char Type");
            buffer.Add(Encoding.UTF8.GetBytes(data.ToString())[0]);
            Console.WriteLine(Encoding.UTF8.GetBytes(data.ToString())[0] + "(\"" + data.ToString() + "\") Data");
        }

        public void AddSByteOrInt(int data)
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

        public void AddByteOrUInt(uint data)
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

        public void AddSingle(Single data)
        {
            buffer.Add((byte)TypeCode.Single);
            Console.WriteLine((byte)TypeCode.Single + " Single Type");
            buffer.AddRange(BitConverter.GetBytes(data));
            Console.WriteLine(data + " Data");
        }

        public void AddDouble(double data)
        {
            buffer.Add((byte)TypeCode.Double);
            Console.WriteLine((byte)TypeCode.Double + " Double Type");
            buffer.AddRange(BitConverter.GetBytes(data));
            Console.WriteLine(data);
        }

        public void AddString(string data)
        {
            buffer.Add((byte)TypeCode.String);
            Console.WriteLine((byte)TypeCode.String + " String Type");
            if (data.Length == 0)
            {
                buffer.Add(0);
                Console.WriteLine("0 Empty");
                return;
            }
            else
            {
                byte byteCount = (byte)Encoding.UTF8.GetByteCount(data);
                buffer.Add(byteCount);
                Console.WriteLine((byte)byteCount + " Byte Count");
                var bytes = Encoding.UTF8.GetBytes(data);
                buffer.AddRange(bytes);
                var s = String.Join(", ", bytes.Select(x => x.ToString()).ToList());
                Console.WriteLine("[" + s + "](\"" + data + "\") Data");
            }
        }

        public byte[] ToArray()
        {
            var ret = buffer.ToArray();
            buffer.Clear();
            return ret;
        }

        #endregion

        #region private methods

        void AddByte(byte data)
        {
            buffer.Add((byte)TypeCode.Byte);
            Console.WriteLine((byte)TypeCode.Byte + " Byte Type");
            buffer.Add(data);
            Console.WriteLine(data + " Data");
        }

        void AddSByte(sbyte data)
        {
            buffer.Add((byte)TypeCode.SByte);
            Console.WriteLine((byte)TypeCode.Byte + " SByte Type");
            buffer.Add((byte)data);
            Console.WriteLine(data + " Data");
        }

        void AddUInt16(UInt16 data)
        {
            buffer.Add((byte)TypeCode.UInt16);
            Console.WriteLine((byte)TypeCode.Byte + " UInt16 Type");
            buffer.AddRange(BitConverter.GetBytes(data));
            Console.WriteLine(data + " Data");
        }

        void AddInt16(Int16 data)
        {
            buffer.Add((byte)TypeCode.Int16);
            Console.WriteLine((byte)TypeCode.Byte + " Int16 Type");
            buffer.AddRange(BitConverter.GetBytes(data));
            Console.WriteLine(data + " Data");
        }

        void AddUInt32(UInt32 data)
        {
            buffer.Add((byte)TypeCode.UInt32);
            Console.WriteLine((byte)TypeCode.Byte + " UInt32 Type");
            buffer.AddRange(BitConverter.GetBytes(data));
            Console.WriteLine(data + " Data");
        }

        void AddInt32(Int32 data)
        {
            buffer.Add((byte)TypeCode.Int32);
            Console.WriteLine((byte)TypeCode.Byte + " Int32 Type");
            buffer.AddRange(BitConverter.GetBytes(data));
            Console.WriteLine(data + " Data");
        }

        #endregion
    }
}
