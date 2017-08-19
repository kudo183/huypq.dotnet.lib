using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace huypq.wpf.Utils
{
    public class BindableObject : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        #region INotifyDataErrorInfo
        public bool HasErrors
        {
            get
            {
                if (_errors.Count == 0)
                {
                    return false;
                }

                foreach (var error in _errors)
                {
                    if (error.Value.Count > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private static readonly List<object> NoErrors = new List<object>();
        protected readonly Dictionary<string, List<object>> _errors = new Dictionary<string, List<object>>();

        public IEnumerable GetErrors(string propertyName)
        {
            if (_errors.TryGetValue(propertyName, out List<object> result))
            {
                return result;
            }
            return NoErrors;
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected virtual void OnErrorsChanged(DataErrorsChangedEventArgs e)
        {
            ErrorsChanged?.Invoke(this, e);
        }

        protected virtual void Validate([CallerMemberName] string propertyName = null)
        {

        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value) == true)
            {
                return false;
            }

            SetFieldWithoutCheckEqual(ref field, value, propertyName);

            return true;
        }

        protected void SetFieldWithoutCheckEqual<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            field = value;
            Validate(propertyName);
            OnPropertyChanged(propertyName);
        }
        #endregion
    }
}
