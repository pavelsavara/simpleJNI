using Microsoft.VisualBasic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static simpleJNI.JNI.JNIEnv;

namespace simpleJNI.JNI
{
    public unsafe partial class JavaVM
    {
        private const string JAVA_HOME_ENV = "JAVA_HOME";
        private static string? JavaHome = null;
        private static bool init;

        private readonly JavaVMNative* native;
        internal JavaVM(JavaVMNative* native)
        {
            this.native = native;
            //functions = *(*(JavaPtr*)native.ToPointer()).functions;
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
                    throw new JNIException("Can't initialize JNI", ex);
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
                throw new JNIException("Can't load JVM (already have one ?) " + result);
            }
            jvm = new JavaVM(jvmNative);
            env = new JNIEnv(jniEnvNative);
        }

        [StructLayout(LayoutKind.Sequential), NativeCppClass]
        internal struct JavaVMNative
        {
            public IntPtr reserved0;
            public IntPtr reserved1;
            public IntPtr reserved2;
            public delegate* unmanaged<JNIResult, JavaVMNative*> DestroyJavaVM;
            public delegate* unmanaged<JNIResult, JavaVMNative*, JNIEnvNative**, JavaVMInitArgsNative*> AttachCurrentThread;
            public delegate* unmanaged<JNIResult, JavaVMNative*> DetachCurrentThread;
            public delegate* unmanaged<JNIResult, JavaVMNative*, JNIEnvNative**, int> GetEnv;
            public delegate* unmanaged<JNIResult, JavaVMNative*, JNIEnvNative**, JavaVMInitArgsNative*> AttachCurrentThreadAsDaemon;
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
