
using Simple.Java;
using Simple.JNI;

namespace java.lang
{
    public partial class System
    {
        /*[JavaImport]
        public static partial long currentTimeMillis();
        */

        internal static JniClass staticClass;
        internal static JniMethod j4n_currentTimeMillis;

        public static long currentTimeMillis()
        {
            var env = JNIEnv.Current;
            if (staticClass.IsDefault)
            {
                staticClass = env.FindClass("java/lang/System");
            }
            if (j4n_currentTimeMillis.IsDefault)
            {
                j4n_currentTimeMillis = env.GetMethodID(staticClass, "currentTimeMillis", "()J");
            }

            env.CallStaticLongMethod(staticClass, j4n_currentTimeMillis, Span<JniValue>.Empty);
            return 0;
        }
    }
}