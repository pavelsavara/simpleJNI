using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Simple.JNI;

public unsafe sealed partial class JNIEnv
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public JniThrowable ExceptionOccurred()
    {
        return functions.ExceptionOccurred(native);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ExceptionDescribe()
    {
        functions.ExceptionDescribe(native);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetVersion()
    {
        return functions.GetVersion(native);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ExceptionClear()
    {
        functions.ExceptionClear(native);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public JniClass FindClass(string className)
    {
        byte* bytes = stackalloc byte[50];
        int len =Encoding.UTF8.GetBytes(className, new Span<byte>(bytes, 50));
        bytes[len] = 0;
        return functions.FindClass(native, bytes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public JniMethod GetMethodID(JniClass clazz, string name, string sig)
    {
        return functions.GetMethodID(native, clazz, name, sig);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long CallStaticLongMethod(JniClass clazz, JniMethod method, Span<JniValue> args)
    {
        var argsPtr = (JniValue*)Unsafe.AsPointer(ref args.GetPinnableReference());
        var res = functions.CallStaticLongMethodA(native, clazz, method, argsPtr);
        CheckException();
        return res;
    }
}
