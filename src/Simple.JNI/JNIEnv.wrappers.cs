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
        byte* asciiName = stackalloc byte[250];
        asciiName[Encoding.ASCII.GetBytes(className, new Span<byte>(asciiName, 250))] = 0;
        var res = functions.FindClass(native, asciiName);
        CheckException();
        return res;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public JniMethod GetMethodID(JniClass clazz, string name, string sig)
    {
        byte* asciiName = stackalloc byte[250];
        asciiName[Encoding.ASCII.GetBytes(name, new Span<byte>(asciiName, 250))] = 0;
        byte* asciiSig = stackalloc byte[50];
        asciiSig[Encoding.ASCII.GetBytes(sig, new Span<byte>(asciiSig, 50))] = 0;
        var res = functions.GetMethodID(native, clazz, asciiName, asciiSig);
        CheckException();
        return res;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public JniMethod GetStaticMethodID(JniClass clazz, string name, string sig)
    {
        byte* asciiName = stackalloc byte[250];
        asciiName[Encoding.ASCII.GetBytes(name, new Span<byte>(asciiName, 250))] = 0;
        byte* asciiSig = stackalloc byte[50];
        asciiSig[Encoding.ASCII.GetBytes(sig, new Span<byte>(asciiSig, 50))] = 0;
        var res = functions.GetStaticMethodID(native, clazz, asciiName, asciiSig);
        CheckException();
        return res;
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
