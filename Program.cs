using simpleJNI.JNI;

namespace SimpleJNI;

public unsafe class Program
{
    public static void Main()
    {
        JavaVM.CreateJavaVM(out var jvm, out var env);
        Console.WriteLine("Hello, World!");
    }
}

