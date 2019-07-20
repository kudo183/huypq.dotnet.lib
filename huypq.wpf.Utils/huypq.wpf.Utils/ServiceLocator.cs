using System;
using System.Collections.Generic;

namespace huypq.wpf.Utils
{
    public static class ServiceLocator
    {
        class TargetItem
        {
            public Type TargetType { get; set; }
            public object ConstructorOption { get; set; }
            public bool IsSingleton { get; set; }
            public object Instance { get; set; }
        }

        private static Dictionary<Type, TargetItem> _typeMapping = new Dictionary<Type, TargetItem>();

        private static void CheckTypeMapping(Type key, Type target)
        {
            if (key.IsGenericType)
            {
                if (key.IsGenericTypeDefinition != target.IsGenericTypeDefinition)
                    throw new ArgumentException("ServiceLocator: key type and value type must be the same GenericTypeDefinition or not");

                else if (key.IsGenericTypeDefinition == false && target.IsGenericTypeDefinition == false)
                {
                    if (key.IsAssignableFrom(target) == false)
                        throw new ArgumentException(string.Format(
                            "ServiceLocator: key type {0} must assignable from value type {1}", key, target));
                }

                else if (key.IsGenericTypeDefinition == true && target.IsGenericTypeDefinition == true)
                {
                    if (IsAssignableToGenericType(target, key) == false)
                        throw new ArgumentException(string.Format(
                            "ServiceLocator: key type {0} must assignable from value type {1}", key, target));
                }
            }
            else
            {
                if (key.IsAssignableFrom(target) == false)
                    throw new ArgumentException("ServiceLocator: key type must assignable from value type");
            }
        }

        private static TargetItem GetTargetItem(Type key)
        {
            TargetItem targetItem;
            if (_typeMapping.TryGetValue(key, out targetItem) == false)
            {
                if (key.IsGenericType)
                {
                    var genericTypeDefinition = key.GetGenericTypeDefinition();
                    if (_typeMapping.TryGetValue(genericTypeDefinition, out targetItem) == false)
                    {
                        throw new ArgumentException(string.Format("ServiceLocator: {0} type not found.", genericTypeDefinition));
                    }
                    var genericArguments = key.GetGenericArguments();
                    targetItem.TargetType = targetItem.TargetType.MakeGenericType(genericArguments);
                }
                else
                {
                    throw new ArgumentException(string.Format("ServiceLocator: {0} type not found.", key));
                }
            }
            return targetItem;
        }

        public static void AddTypeMapping(Type key, Type target, bool isSingleton, object constructorOption)
        {
            CheckTypeMapping(key, target);

            _typeMapping.Add(key, new TargetItem()
            {
                TargetType = target,
                ConstructorOption = constructorOption,
                IsSingleton = isSingleton
            });
        }

        public static void UpdateTypeMapping(Type key, Type target, bool isSingleton, object constructorOption)
        {
            CheckTypeMapping(key, target);

            TargetItem targetItem = GetTargetItem(key);
            targetItem.TargetType = target;
            targetItem.ConstructorOption = constructorOption;
            targetItem.IsSingleton = isSingleton;
            if (targetItem.IsSingleton == true)
            {
                targetItem.Instance = null;
            }
        }

        public static T Get<T>() where T : class
        {
            Type key = typeof(T);
            TargetItem targetItem = GetTargetItem(key);

            if (targetItem.IsSingleton == true)
            {
                if (targetItem.Instance == null)
                {
                    targetItem.Instance = CreateInstance(targetItem);
                }
                return targetItem.Instance as T;
            }

            return CreateInstance(targetItem) as T;
        }

        private static object CreateInstance(TargetItem targetItem)
        {
            if (targetItem.ConstructorOption == null)
            {
                return Activator.CreateInstance(targetItem.TargetType);
            }
            return Activator.CreateInstance(targetItem.TargetType, targetItem.ConstructorOption);
        }

        private static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = givenType.BaseType;
            if (baseType == null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }
    }
}
