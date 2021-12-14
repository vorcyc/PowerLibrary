
using System;

/// <summary>
/// 本命名空间提供缓冲区相关功能和操作
/// </summary>
namespace Vorcyc.PowerLibrary.Buffer
{
    internal static class BitConverter
    {



        #region int16

        public static byte[] FromInt16(short value)
        {
            byte[] buffer = new byte[2];
            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            return buffer;
        }

        public static short ToInt16(byte[] buffer)
        {
            return (short)(buffer[0] | (buffer[1] << 8));
        }

        #endregion


        #region Int32

        public static byte[] FromInt32(int value)
        {
            byte[] buffer = new byte[4];
            buffer[0] = (byte)value; // in vb  ---->  CByte(value And &HFF)  要将其他位置零
            buffer[1] = (byte)(value >> 8); // in vb  ---->  CByte(value >> 8  And &HFF) 
            buffer[2] = (byte)(value >> 16);
            buffer[3] = (byte)(value >> 24);

            return buffer;
        }

        public static int ToInt32(byte[] buffer)
        {
            return (((buffer[0] | (buffer[1] << 8)) | (buffer[2] << 16)) | (buffer[3] << 24));
        }

        //Public Function FromInt32(ByVal value As Integer) As Byte()
        //    Dim result(3) As Byte
        //    result(0) = CByte(value And &HFF)
        //    result(1) = CByte(value >> 8 And &HFF)
        //    result(2) = CByte(value >> 16 And &HFF)
        //    result(3) = CByte(value >> 24)
        //    Return result
        //End Function

        //Public Function ToInt32(ByVal block As Byte()) As Integer
        //    Dim result As Integer
        //    'vb的移位是受类型限制的
        //    '移位操作不会使类型转换
        //    'CS就不一样了
        //    '他会尽量朝Int32转换
        //    result = CInt(block(0)) Or CInt(block(1)) << 8
        //    result = result Or CInt(block(2)) << 16
        //    result = result Or CInt(block(3)) << 24

        //    Return result
        //End Function
        #endregion


        #region double

        public static unsafe byte[] FromDouble(double value)
        {
            byte[] buffer = new byte[8];
            ulong num = *((ulong*)&value);

            buffer[0] = (byte)num;
            buffer[1] = (byte)(num >> 8);
            buffer[2] = (byte)(num >> 16);
            buffer[3] = (byte)(num >> 24);
            buffer[4] = (byte)(num >> 32);
            buffer[5] = (byte)(num >> 40);
            buffer[6] = (byte)(num >> 48);
            buffer[7] = (byte)(num >> 56);

            return buffer;
        }


        public static unsafe double ToDouble(byte[] buffer)
        {
            uint num = (uint)(((buffer[0] | (buffer[1] << 8)) | (buffer[2] << 16)) | (buffer[3] << 24));
            uint num2 = (uint)(((buffer[4] | (buffer[5] << 8)) | (buffer[6] << 16)) | (buffer[7] << 24));
            ulong num3 = (num2 << 32) | num;
            return *(((double*)&num3));
        }


        #endregion


        #region DateTime

        public static unsafe byte[] FromDateTime(DateTime value)
        {
            byte[] buffer = new byte[8];
            ulong num = *((ulong*)&value);
            buffer[0] = (byte)num;
            buffer[1] = (byte)(num >> 8);
            buffer[2] = (byte)(num >> 16);
            buffer[3] = (byte)(num >> 24);
            buffer[4] = (byte)(num >> 32);
            buffer[5] = (byte)(num >> 40);
            buffer[6] = (byte)(num >> 48);
            buffer[7] = (byte)(num >> 56);

            return buffer;
        }


        public static unsafe DateTime ToDateTime(byte[] buffer)
        {
            if (buffer.Length > 8) throw new InvalidOperationException("buffer");

            uint num = (uint)(((buffer[0] | (buffer[1] << 8)) | (buffer[2] << 16)) | (buffer[3] << 24));
            uint num2 = (uint)(((buffer[4] | (buffer[5] << 8)) | (buffer[6] << 16)) | (buffer[7] << 24));
            ulong num3 = (num2 << 32) | num;

            return *(((DateTime*)&num3));
        }
        #endregion


        #region Single

        public static unsafe byte[] FromSingle(float value)
        {
            byte[] buffer = new byte[4];
            uint num = *((uint*)&value);

            buffer[0] = (byte)num;
            buffer[1] = (byte)(num >> 8);
            buffer[2] = (byte)(num >> 16);
            buffer[3] = (byte)(num >> 24);

            return buffer;

        }


        public static unsafe float ToSingle(byte[] buffer)
        {
            uint num = (uint)(((buffer[0] | (buffer[1] << 8)) | (buffer[2] << 16)) | (buffer[3] << 24));
            return *(((float*)&num));
        }

        #endregion


    }
}
