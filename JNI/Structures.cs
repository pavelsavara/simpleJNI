using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace simpleJNI.JNI
{

    public enum JNIResult
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
    public unsafe struct JavaVMInitArgsNative
    {
        public int version;
        public int nOptions;
        public JavaVMOptionNative* options;
        public byte ignoreUnrecognized;
    }

    [StructLayout(LayoutKind.Sequential), NativeCppClass]
    public struct JavaVMOptionNative
    {
        public IntPtr optionString; //char*
        public IntPtr extraInfo; //void*
    }

    [Serializable]
    public class JNIException : Exception
    {
        protected JNIException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public JNIException()
        {
        }

        public JNIException(string message)
            : base(message)
        {
        }

        public JNIException(string message, Exception ex)
            : base(message, ex)
        {
        }
    }
}
