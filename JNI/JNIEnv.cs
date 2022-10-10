namespace simpleJNI.JNI
{
    public unsafe partial class JNIEnv
    {
        JNIEnvNative* native = null;
        internal JNIEnv(JNIEnvNative* native)
        {
            this.native = native;
        }

        internal struct JNIEnvNative
        {
        }
    }
}
