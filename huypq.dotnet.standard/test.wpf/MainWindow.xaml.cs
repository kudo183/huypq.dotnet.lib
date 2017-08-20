using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using huypq.dotnet.standard;
using huypq.dotnet.standard.test;

namespace test.wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

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

        private void T_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Console.WriteLine("T_PropertyChanged");
        }
    }
}
