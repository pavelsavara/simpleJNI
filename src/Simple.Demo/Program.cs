using Simple.JNI;

try
{
    using (var jvm = new JavaVM())
    {
        Console.WriteLine($"Hello, JNI 0x{JNIEnv.Current.GetVersion(),0:x}");
        Console.WriteLine($"Time in Java is {java.lang.System.currentTimeMillis()}");
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}
