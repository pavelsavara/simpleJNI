
using System.Runtime.CompilerServices;

namespace Simple.JNI;

public unsafe partial class JNIEnv
{
    [ThreadStatic]
    private static JNIEnv? instance;
    private readonly JNIEnvNative* native;
    private JNIEnvFunctions functions;
    internal JNIEnv(JNIEnvNative* native, JavaVM jvm)
    {
        this.native = native;
        // copy functions
        functions = *(*native).functions;
    }

    public static JNIEnv Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (instance == null)
            {
                if (JavaVM.instance == null)
                {
                    throw new Exception("Please create default instance of JavaVM");
                }
                instance = JavaVM.instance.AttachCurrentThread();
            }
            return instance;
        }
        internal set { instance = value; }
    }

    public void CheckException()
    {
        var th = ExceptionOccurred();
        if (th.handle != IntPtr.Zero)
        {
            ExceptionDescribe();
            throw new Exception("TODO map java exception");
        }
    }

}
