using Microsoft.VisualBasic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static simpleJNI.JNI.JNIEnv;

namespace simpleJNI.JNI
{
    public unsafe partial class JavaVM
    {
        private static bool init;

        private readonly JavaVMFunctions* functions;
        private readonly JavaVMNative* native;
        private readonly JNIEnv env;
        internal JavaVM(JavaVMNative* native, JNIEnv env)
        {
            this.native = native;
            this.functions = (*native).functions;
            this.env = env;
            //functions = *(*(JavaPtr*)native.ToPointer()).functions;
        }

        public void Destroy()
        {
            this.env.ExceptionDescribe();
            this.env.ExceptionOccurred();
            this.env.ExceptionClear();
            var res = (*functions).DestroyJavaVM(native);
            ThrowOnError(res);
        }

        private static void ThrowOnError(JNIResult result)
        {
            if (result != JNIResult.JNI_OK)
            {
                throw new Exception("JNI failed: " + result);
            }
        }
        private void ThrowOnException(JNIResult result)
        {
            /*
            JniLocalHandle occurred = this.env.ExceptionOccurred();
            if (!JniLocalHandle.IsNull(occurred))
            {
                //ExceptionDescribe();
                ExceptionClear();
                Exception exception = Convertor.FullJ2C<Exception>(this, occurred);
                throw exception;
            }*/
        }


        private static void Init()
        {
            if (!init)
            {
                string findJvmDir = FindJvmDir();
                AddEnvironmentPath(findJvmDir);
                var args = new JavaVMInitArgsNative();
                try
                {
                    //just load DLL
                    Dll.JNI_GetDefaultJavaVMInitArgs(&args);
                    init = true;
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

        public static void CreateJavaVM(out JavaVM jvm, out JNIEnv env)
        {
            Init();
            var args = new JavaVMInitArgsNative();
            args.version = 0x00010008;//JNI_VERSION_1_8

            JNIResult result;
            result = Dll.JNI_CreateJavaVM(out var jvmNative, out var jniEnvNative, &args);
            if (result != JNIResult.JNI_OK)
            {
                throw new Exception("Can't load JVM" + result);
            }
            env = new JNIEnv(jniEnvNative);
            jvm = new JavaVM(jvmNative, env);
        }

        [StructLayout(LayoutKind.Sequential, Size = 4), NativeCppClass]
        internal struct JavaVMNative
        {
            public JavaVMFunctions* functions;
        }

        [StructLayout(LayoutKind.Sequential), NativeCppClass]
        internal struct JavaVMFunctions
        {
            public IntPtr reserved0;
            public IntPtr reserved1;
            public IntPtr reserved2;
            public delegate* unmanaged<JavaVMNative*, JNIResult> DestroyJavaVM;
            public delegate* unmanaged<JavaVMNative*, JNIEnvNative**, JavaVMInitArgsNative*, JNIResult> AttachCurrentThread;
            public delegate* unmanaged<JavaVMNative*, JNIResult> DetachCurrentThread;
            public delegate* unmanaged<JavaVMNative*, JNIEnvNative**, int, JNIResult> GetEnv;
            public delegate* unmanaged<JavaVMNative*, JNIEnvNative**, JavaVMInitArgsNative*, JNIResult> AttachCurrentThreadAsDaemon;
        }

        internal unsafe static partial class Dll
        {
            [LibraryImport("jvm.dll")]
            internal static partial JNIResult JNI_CreateJavaVM(out JavaVMNative* vm, out JNIEnvNative* penv, JavaVMInitArgsNative* args);

            [LibraryImport("jvm.dll")]
            internal static partial JNIResult JNI_GetCreatedJavaVMs(out IntPtr pvm, int size, out int size2);

            [LibraryImport("jvm.dll")]
            internal static partial JNIResult JNI_GetDefaultJavaVMInitArgs(JavaVMInitArgsNative* args);

            public const string PlaceHolderLibraryName = "NativeTesseractLib";
            public const string WindowsAssemblyName = "jvm";
            public const string LinuxAssemblyName = "libtesseract.so.4";

            static Dll()
            {
                NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), DllImportResolver);
            }

            static string GetLibraryName(string libraryName) => libraryName switch
            {
                PlaceHolderLibraryName => Environment.OSVersion.Platform switch
                {
                    PlatformID.Win32NT => WindowsAssemblyName,
                    _ => LinuxAssemblyName,
                },
                _ => libraryName,
            };

            static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
            {
                var platformDependentName = GetLibraryName(libraryName);

                IntPtr handle;
                NativeLibrary.TryLoad(platformDependentName, assembly, searchPath, out handle);
                return handle;
            }
        }
    }

}
