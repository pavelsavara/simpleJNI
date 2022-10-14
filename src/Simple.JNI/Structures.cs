using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Simple.JNI;

internal enum JNIResult
{
    JNI_OK = 0, /* success */
    JNI_ERR = (-1), /* unknown error */
    JNI_EDETACHED = (-2), /* thread detached from the VM */
    JNI_EVERSION = (-3), /* JNI version error */
    JNI_ENOMEM = (-4), /* not enough memory */
    JNI_EEXIST = (-5), /* VM already created */
    JNI_EINVAL = (-6), /* invalid arguments */
}

[StructLayout(LayoutKind.Sequential), NativeCppClass]
internal unsafe struct JavaVMInitArgsNative
{
    public int version;
    public int nOptions;
    public JavaVMOptionNative* options;
    public byte ignoreUnrecognized;
}

[StructLayout(LayoutKind.Sequential), NativeCppClass]
internal struct JavaVMOptionNative
{
    public IntPtr optionString; //char*
    public IntPtr extraInfo; //void*
}

[StructLayout(LayoutKind.Sequential), NativeCppClass]
internal struct JniLocalHandle
{
    internal IntPtr handle;
}

[StructLayout(LayoutKind.Sequential), NativeCppClass]
internal struct JniGlobalHandle
{
    internal IntPtr handle;
}

[StructLayout(LayoutKind.Sequential), NativeCppClass]
public struct JniMethod
{
    public bool IsDefault
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return handle == default;
        }
    }

    internal IntPtr handle;
}

[StructLayout(LayoutKind.Sequential), NativeCppClass]
public struct JniClass
{
    public bool IsDefault
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return handle == default;
        }
    }

    internal IntPtr handle;
}

[StructLayout(LayoutKind.Sequential), NativeCppClass]
public struct JniObject
{
    public bool IsDefault
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return handle == default;
        }
    }

    internal IntPtr handle;
}

[StructLayout(LayoutKind.Sequential), NativeCppClass]
public struct JniThrowable
{
    public bool IsDefault
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return handle == default;
        }
    }

    internal IntPtr handle;
}

[StructLayout(LayoutKind.Explicit, Size = 8), NativeCppClass]
public struct JniValue
{
    public static JniValue Null
    {
        get { return new JniValue(); }
    }


    public bool IsDefault
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return _double == default;
        }
    }

    [FieldOffset(0)] public byte _bool;
    [FieldOffset(0)] public byte _byte;
    [FieldOffset(0)] public short _char;
    [FieldOffset(0)] public short _short;
    [FieldOffset(0)] public int _int;
    [FieldOffset(0)] public long _long;
    [FieldOffset(0)] public float _float;
    [FieldOffset(0)] public double _double;
    [FieldOffset(0)] public JniClass _clazz;
    [FieldOffset(0)] public JniObject _object;
    [FieldOffset(0)] public JniThrowable _exception;
}