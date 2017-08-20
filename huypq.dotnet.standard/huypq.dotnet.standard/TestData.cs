using System;
using System.Collections.Generic;
using System.Text;

namespace huypq.dotnet.standard.test
{
    public interface ITest
    {
        string Name { get; set; }
    }
    public class Test : BindableObject, ITest
    {
        string name;
        public string Name { get { return name; } set { SetField(ref name, value); } }
    }

    public interface ITestGeneric<T>
    {
        T Data { get; set; }
    }
    public class TestGeneric<T> : BindableObject, ITestGeneric<T>
    {
        string name;
        public string Name { get { return name; } set { SetField(ref name, value); } }

        public T Data { get; set; }
    }
}
