using System;
using System.Reflection;
using System.Collections.Generic;

namespace huypq.dotnet.standard
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

        public static void AddTypeMapping(Type key, Type target, bool isSingleton, object constructorOption)
        {
            var keyTypeInfo = key.GetTypeInfo();
            var targetTypeInfo = target.GetTypeInfo();
            if (keyTypeInfo.IsGenericType)
            {
                if (keyTypeInfo.IsGenericTypeDefinition != targetTypeInfo.IsGenericTypeDefinition)
                    throw new ArgumentException("ServiceLocator: key type and value type must be the same GenericTypeDefinition or not");

                else if (keyTypeInfo.IsGenericTypeDefinition == false && targetTypeInfo.IsGenericTypeDefinition == false)
                {
                    if (keyTypeInfo.IsAssignableFrom(targetTypeInfo) == false)
                        throw new ArgumentException(string.Format(
                            "ServiceLocator: key type {0} must assignable from value type {1}", key, target));
                }

                else if (keyTypeInfo.IsGenericTypeDefinition == true && targetTypeInfo.IsGenericTypeDefinition == true)
                {
                    if (IsAssignableToGenericType(target, key) == false)
                        throw new ArgumentException(string.Format(
                            "ServiceLocator: key type {0} must assignable from value type {1}", key, target));
                }
            }
            else
            {
                if (keyTypeInfo.IsAssignableFrom(targetTypeInfo) == false)
                    throw new ArgumentException("ServiceLocator: key type must assignable from value type");
            }

            _typeMapping.Add(key, new TargetItem()
            {
                TargetType = target,
                ConstructorOption = constructorOption,
                IsSingleton = isSingleton
            });
        }

        public static T Get<T>() where T : class
        {
            var type = typeof(T);
            var typeInfo = type.GetTypeInfo();
            TargetItem targetItem;
            if (_typeMapping.TryGetValue(type, out targetItem) == false)
            {
                if (typeInfo.IsGenericType)
                {
                    var genericTypeDefinition = type.GetGenericTypeDefinition();
                    if (_typeMapping.TryGetValue(genericTypeDefinition, out targetItem) == false)
                    {
                        throw new ArgumentException(string.Format("ServiceLocator: {0} type not found.", genericTypeDefinition));
                    }
                    var genericArguments = type.GenericTypeArguments;
                    targetItem.TargetType = targetItem.TargetType.MakeGenericType(genericArguments);
                }
                else
                {
                    throw new ArgumentException(string.Format("ServiceLocator: {0} type not found.", type));
                }
            }
            
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
            var typeInfo = givenType.GetTypeInfo();
            var interfaceTypes = typeInfo.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.GetTypeInfo().IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (typeInfo.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = typeInfo.BaseType;
            if (baseType == null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }
    }
}
