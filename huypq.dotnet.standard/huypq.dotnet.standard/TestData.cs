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

        public string FullName { get { return name; } }

        public string FullNameWithTime { get { return FullName + DateTime.Now.ToString(); } }

        public Test()
        {
            SetDependentProperty(nameof(Name), new List<string>() { nameof(FullName) });
            SetDependentProperty(nameof(FullName), new List<string>() { nameof(FullNameWithTime) });
        }
    }

    public interface ITestGeneric<T>
    {
        T Data { get; set; }
    }
    public class TestGeneric<T> : BindableObject, ITestGeneric<T>
    {
        public string Name
        {
            get { return Data.ToString(); }
        }

        public T Data { get; set; }

        public TestGeneric()
        {
            SetDependentProperty(nameof(Data), new List<string>() { nameof(Name) });
        }
    }
}
