using static Simple.JNI.JNIEnv;

namespace Simple.JNI;

public unsafe partial class JavaVM : IDisposable
{
    [ThreadStatic]
    private static JNIEnv? env;

    public JNIEnv Env
    {
        get
        {
            if (env == null)
            {
                JNIEnvNative* envPtr;
                // TODO test me on new thread
                functions.AttachCurrentThread(native, &envPtr, null);
                env = new JNIEnv(envPtr, this);
            }
            return env;
        }
        internal set { env = value; }
    }

    public JavaVM()
    {
        CreateJavaVM(out native, out var jniEnvNative);
        functions = *(*native).functions;
        env = new JNIEnv(jniEnvNative, this);
    }

    #region disposable
    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            DestroyJavaVM();
            disposedValue = true;
        }
    }

    ~JavaVM()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion

    #region Locate JVM

    private static bool globalInit;
    private static void Init()
    {
        if (!globalInit)
        {
            string findJvmDir = FindJvmDir();
            AddEnvironmentPath(findJvmDir);
            var args = new JavaVMInitArgsNative();
            try
            {
                //just load DLL
                Dll.JNI_GetDefaultJavaVMInitArgs(&args);
                globalInit = true;
            }
            catch (BadImageFormatException ex)
            {
                // it didn't help, throw original exception
                throw new Exception("Can't initialize JNI", ex);
            }
        }
    }

    private static string FindJvmDir()
    {
        // TODO search more locations
        return @"c:\Program Files\Java\jdk1.8.0_341\jre\bin\server\";
    }

    private static void AddEnvironmentPath(string jvm)
    {
        string path = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        if (!path.StartsWith(jvm))
        {
            path = jvm + Path.PathSeparator + path;
            Environment.SetEnvironmentVariable("PATH", path);
        }
    }

    #endregion
}

