
namespace Vorcyc.PowerLibrary.Buffer
{

    /// <summary>
    /// 
    /// </summary>
    internal static class BitWriter
    {

        public static void WriteSingle(byte[] buffer, int index, float value)
        {
            byte[] data = new byte[4];
            data = BitConverter.FromSingle(value);
            buffer[index] = data[0];
            buffer[index + 1] = data[1];
            buffer[index + 2] = data[2];
            buffer[index + 3] = data[3];
        }

        public static void WriteInt32(byte[] buffer, int index, int value)
        {
            byte[] data = new byte[4];
            data = BitConverter.FromInt32(value);
            buffer[index] = data[0];
            buffer[index + 1] = data[1];
            buffer[index + 2] = data[2];
            buffer[index + 3] = data[3];
        }

        public static void WriteInt16(byte[] buffer, int index, short value)
        {
            byte[] data = new byte[2];
            data = BitConverter.FromInt16(value);
            buffer[index] = data[0];
            buffer[index + 1] = data[1];
        }
    }
}
