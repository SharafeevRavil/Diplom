namespace TestHelloWorld;

public static class Program
{
    public static void Main()
    {
        Console.WriteLine("Hello, World!");
    }

    public static int Sum1(int a, int b)
    {
        var sum = a + b;
        return sum;
    }
    
    public static int Sum2(int a, int b)
    {
        var sum2 = a + b;
        return sum2;
    }
    
    public static int Hello1()
    {
        Console.WriteLine("Hello, World!");
    }
    //0010001101000101000100011001011110001110001010000001000011010000
    //0010001101000101000100011001011110001110001010000001000011010000
    public static int Hello2()
    {
        Console.WriteLine("Hello, Worldoo!");
    }
}