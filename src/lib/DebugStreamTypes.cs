using System;
namespace Piot.Brook
{
    public enum DebugSerializeType
    {
        NotAvailable,
        Uint16,
        Int16,
        Uint32,
        Uint64,
        Uint8,
        SignedBits,
        UnsignedBits,
        Int32,
        Int8,
    }

    public static class DebugStreamTypes
    {
        public static string TypeToString(DebugSerializeType type)
        {
            return type.ToString();
        }
    }
}
