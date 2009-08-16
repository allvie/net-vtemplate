/* ***********************************************
 * Author		:  kingthy
 * Email		:  kingthy@gmail.com
 * Description	:  LoopIndex
 *
 * ***********************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace VTemplate.Engine
{
    /// <summary>
    /// 循环索引
    /// </summary>
    public class LoopIndex : IConvertible
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public LoopIndex(decimal value)
        {
            this.Value = value;
        }

        /// <summary>
        /// 值
        /// </summary>
        public decimal Value { get; internal set; }

        /// <summary>
        /// 是否是第一个索引值
        /// </summary>
        public bool IsFirst { get; internal set; }

        /// <summary>
        /// 是否是最后一个索引值
        /// </summary>
        public bool IsLast { get; internal set; }

        /// <summary>
        /// 是否是偶数索引值
        /// </summary>
        public bool IsEven
        {
            get
            {
                return (this.Value % 2) == 0;
            }
        }

        /// <summary>
        /// 获取此索引值的字符串表现形式
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }

        #region IConvertible 成员

        public TypeCode GetTypeCode()
        {
            return this.Value.GetTypeCode();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(this.Value, provider);
        }

        public byte ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(this.Value, provider);
        }

        public char ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(this.Value, provider);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(this.Value, provider);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(this.Value, provider);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(this.Value, provider);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(this.Value, provider);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(this.Value, provider);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(this.Value, provider);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(this.Value, provider);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(this.Value, provider);
        }

        public string ToString(IFormatProvider provider)
        {
            return Convert.ToString(this.Value, provider);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return ((IConvertible)this.Value).ToType(conversionType, provider);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(this.Value, provider);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(this.Value, provider);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(this.Value, provider);
        }

        #endregion
    }
}
