using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Simple.JNI.JNIEnv;

namespace Simple.JNI;

public unsafe partial class JavaVM
{
    private readonly JavaVMFunctions functions;
    private readonly JavaVMNative* native;

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

        public const string PlaceHolderLibraryName = "jvm";
        public const string WindowsAssemblyName = "jvm";
        public const string LinuxAssemblyName = "jvm";

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
