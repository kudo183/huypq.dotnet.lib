using System;
using huypq.dotnet.standard;
using huypq.dotnet.standard.test;

namespace test.netframework
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceLocator.AddTypeMapping(typeof(ITest), typeof(Test), true, null);
            ServiceLocator.AddTypeMapping(typeof(ITestGeneric<>), typeof(TestGeneric<>), true, null);

            var test = ServiceLocator.Get<ITest>();
            test.Name = "hello";
            Console.WriteLine(test.Name);

            var testGeneric = ServiceLocator.Get<ITestGeneric<int>>();
            testGeneric.Data = 1;
            Console.WriteLine(testGeneric.Data);

            var t = new Test();
            t.PropertyChanged += T_PropertyChanged;
            t.Name = "test PropertyChanged";

            Console.WriteLine(t.Name);
        }

        private static void T_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Console.WriteLine("T_PropertyChanged: " + e.PropertyName);
        }
    }
}