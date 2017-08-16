using System;
using System.Windows;
using System.Windows.Data;

namespace huypq.wpf.Utils
{
    public static class BindingUtils
    {
        private static readonly DependencyProperty DummyProperty = DependencyProperty.RegisterAttached(
          "Dummy",
          typeof(Object),
          typeof(DependencyObject),
          new UIPropertyMetadata(null));

        public static Object Eval(Object container, String expression)
        {
            Binding binding = new Binding(expression) { Source = container };
            DependencyObject dummyDO = new DependencyObject();
            BindingOperations.SetBinding(dummyDO, DummyProperty, binding);
            return dummyDO.GetValue(DummyProperty);
        }
    }
}
