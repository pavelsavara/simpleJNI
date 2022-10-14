using Simple.JNI;

using (var jvm = new JavaVM())
{
    var jniVersion = jvm.Env.GetVersion();

    Console.WriteLine($"Hello, JNI 0x{jniVersion,0:x}");
}
