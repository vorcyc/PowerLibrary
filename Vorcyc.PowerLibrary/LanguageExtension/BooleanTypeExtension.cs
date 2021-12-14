namespace Vorcyc.PowerLibrary.LanguageExtension
{

    /// <summary>
    /// 
    /// </summary>
    public static class BooleanTypeExtension
    {

        public static bool ToBool(this int x) => x != 0;

        public static bool ToBool(this uint x) => x != 0;

        public static bool ToBool(this long x) => x != 0;

        public static bool ToBool(this ulong x) => x != 0;




    }
}
