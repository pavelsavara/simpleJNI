namespace Simple.JNI;

public unsafe sealed partial class JavaVM : IDisposable
{
    internal static JavaVM? instance = null;

    public JavaVM()
    {
        CreateJavaVM(out native, out var jniEnvNative);
        functions = *(*native).functions;
        JNIEnv.Current = new JNIEnv(jniEnvNative, this);
        if (instance == null)
        {
            instance = this;
        }
    }

    #region disposable
    private bool disposedValue;

    private void Dispose(bool disposing)
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
        //return @"c:\Program Files\Java\jdk1.8.0_341\jre\bin\server\";
        return @"c:\Program Files\Java\jdk-19\bin\server";
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

