using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace simpleJNI.JNI
{

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
        public IntPtr handle;
    }

    [StructLayout(LayoutKind.Sequential), NativeCppClass]
    internal struct JniGlobalHandle
    {
        public IntPtr handle;
    }

    [StructLayout(LayoutKind.Sequential), NativeCppClass]
    internal struct JniThrowable
    {
        public IntPtr handle;
    }

}
