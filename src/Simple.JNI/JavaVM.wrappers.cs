namespace Simple.JNI;

public unsafe partial class JavaVM
{
    internal static void CreateJavaVM(out JavaVMNative* jvmNative, out JNIEnv.JNIEnvNative* jniEnvNative)
    {
        Init();
        var args = new JavaVMInitArgsNative();
        args.version = 0x00010008;//JNI_VERSION_1_8
        var res = Dll.JNI_CreateJavaVM(out jvmNative, out jniEnvNative, &args);
        ThrowOnError(res);
    }

    internal void DestroyJavaVM()
    {
        var res = functions.DestroyJavaVM(native);
        ThrowOnError(res);
    }

    internal static void ThrowOnError(JNIResult result)
    {
        if (result != JNIResult.JNI_OK)
        {
            throw new Exception("JNI failed: " + result);
        }
    }
}
