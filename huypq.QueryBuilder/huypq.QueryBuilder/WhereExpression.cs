using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace huypq.QueryBuilder
{
    public class WhereExpression
    {
        public static IQueryable<TSource> AddWhereExpressionListContains<TSource>
            (IQueryable<TSource> source, string propertyPath, object value)
        {
            var p = Expression.Parameter(typeof(TSource), "p");
            var parts = propertyPath.Split('.');

            var whereProperty = parts.Aggregate<string, Expression>(p, Expression.Property);
            var constant = Expression.Constant(value, value.GetType());

            var predicateBody = Expression.Call(constant, "Contains", null, whereProperty);

            var lamda = Expression.Lambda<Func<TSource, bool>>(predicateBody, p);
            return source.Where(lamda);
        }

        public static IQueryable<TSource> AddWhereExpression<TSource>
            (IQueryable<TSource> source, string propertyPath, string predicate, object value)
        {
            var p = Expression.Parameter(typeof(TSource), "p");
            var parts = propertyPath.Split('.');

            var whereProperty = parts.Aggregate<string, Expression>(p, Expression.Property);
            var constant = Expression.Constant(value, whereProperty.Type);
            var predicateBody = GetPredicateExpression(predicate, whereProperty, constant);

            var lamda = Expression.Lambda<Func<TSource, bool>>(predicateBody, p);
            return source.Where(lamda);
        }

        public static IQueryable<TSource> AddWhereExpression<TSource>
            (IQueryable<TSource> source, List<IWhereOption> whereOptions)
        {
            if (whereOptions == null || whereOptions.Count == 0)
                return source;

            foreach (var whereOption in whereOptions)
            {
                switch (whereOption.Predicate)
                {
                    case In:
                        source = AddWhereExpressionListContains(source, whereOption.PropertyPath, whereOption.GetValue());
                        break;
                    default:
                        source = AddWhereExpression(source, whereOption.PropertyPath, whereOption.Predicate, whereOption.GetValue());
                        break;
                }
            }

            return source;
        }

        public const string GreaterThan = ">";
        public const string GreaterThanOrEqual = ">=";
        public const string LessThan = "<";
        public const string LessThanOrEqual = "<=";
        public const string Equal = "=";
        public const string StartsWith = "(";
        public const string Contains = "*";
        public const string NotEqual = "!=";
        public const string NotContains = "!*";
        public const string NotStartsWith = "!(";
        public const string In = "IN";

        public static Expression GetPredicateExpression(string predicate, Expression left, Expression right)
        {
            switch (predicate)
            {
                case GreaterThan:
                    return Expression.GreaterThan(left, right);
                case GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(left, right);
                case LessThan:
                    return Expression.LessThan(left, right);
                case LessThanOrEqual:
                    return Expression.LessThanOrEqual(left, right);
                case Equal:
                    return Expression.Equal(left, right);
                case StartsWith:
                    return Expression.Call(left, "StartsWith", null, right);
                case Contains:
                    return Expression.Call(left, "Contains", null, right);
                case NotEqual:
                    return Expression.NotEqual(left, right);
                case NotContains:
                    return Expression.Not(Expression.Call(left, "Contains", null, right));
                case NotStartsWith:
                    return Expression.Not(Expression.Call(left, "StartsWith", null, right));
            }

            return Expression.Equal(left, right);
        }

        public static string GetPredicateFromText(string text)
        {
            if (string.IsNullOrEmpty(text) == true || text.Length < 2)
                return string.Empty;

            var p2 = text.Substring(0, 2);
            switch (p2)
            {
                case GreaterThanOrEqual:
                    return GreaterThanOrEqual;
                case LessThanOrEqual:
                    return LessThanOrEqual;
                case NotEqual:
                    return NotEqual;
                case NotContains:
                    return NotContains;
                case NotStartsWith:
                    return NotStartsWith;
                case In:
                    return In;
                default:
                    var p1 = text.Substring(0, 1);
                    switch (p1)
                    {
                        case GreaterThan:
                            return GreaterThan;
                        case LessThan:
                            return LessThan;
                        case Equal:
                            return Equal;
                        case Contains:
                            return Contains;
                        case StartsWith:
                            return StartsWith;
                    }
                    break;
            }

            return string.Empty;
        }

        [ProtoBuf.ProtoContract]
        [ProtoBuf.ProtoInclude(10, typeof(WhereOption<int>))]
        [ProtoBuf.ProtoInclude(11, typeof(WhereOption<int?>))]
        [ProtoBuf.ProtoInclude(12, typeof(WhereOption<string>))]
        [ProtoBuf.ProtoInclude(13, typeof(WhereOption<bool>))]
        [ProtoBuf.ProtoInclude(14, typeof(WhereOption<bool?>))]
        [ProtoBuf.ProtoInclude(15, typeof(WhereOption<DateTime>))]
        [ProtoBuf.ProtoInclude(16, typeof(WhereOption<DateTime?>))]
        [ProtoBuf.ProtoInclude(17, typeof(WhereOption<TimeSpan>))]
        [ProtoBuf.ProtoInclude(18, typeof(WhereOption<TimeSpan?>))]
        [ProtoBuf.ProtoInclude(19, typeof(WhereOption<long>))]
        [ProtoBuf.ProtoInclude(20, typeof(WhereOption<long?>))]
        [ProtoBuf.ProtoInclude(50, typeof(WhereOption<List<int>>))]
        public interface IWhereOption
        {
            string PropertyPath { get; set; }
            string Predicate { get; set; }

            object GetValue();
        }

        [ProtoBuf.ProtoContract]
        [ProtoBuf.ProtoInclude(10, typeof(WhereOptionInt))]
        [ProtoBuf.ProtoInclude(11, typeof(WhereOptionNullableInt))]
        [ProtoBuf.ProtoInclude(12, typeof(WhereOptionString))]
        [ProtoBuf.ProtoInclude(13, typeof(WhereOptionBool))]
        [ProtoBuf.ProtoInclude(14, typeof(WhereOptionNullableBool))]
        [ProtoBuf.ProtoInclude(15, typeof(WhereOptionDate))]
        [ProtoBuf.ProtoInclude(16, typeof(WhereOptionNullableDate))]
        [ProtoBuf.ProtoInclude(17, typeof(WhereOptionTime))]
        [ProtoBuf.ProtoInclude(18, typeof(WhereOptionNullableTime))]
        [ProtoBuf.ProtoInclude(19, typeof(WhereOptionLong))]
        [ProtoBuf.ProtoInclude(20, typeof(WhereOptionNullableLong))]
        [ProtoBuf.ProtoInclude(50, typeof(WhereOptionIntList))]
        public abstract class WhereOption<T> : IWhereOption, System.IEquatable<WhereOption<T>>
        {
            [ProtoBuf.ProtoMember(1)]
            public string PropertyPath { get; set; }
            [ProtoBuf.ProtoMember(2)]
            public string Predicate { get; set; }
            [ProtoBuf.ProtoMember(3)]
            public T Value { get; set; }

            public object GetValue() { return Value; }

            public bool Equals(WhereOption<T> other)
            {
                if (ReferenceEquals(null, other) == true)
                    return false;

                if (ReferenceEquals(this, other) == true)
                    return true;

                if (PropertyPath != other.PropertyPath
                    || Predicate != other.Predicate)
                {
                    return false;
                }

                return CompareValue(Value, other.Value);
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as WhereOption<T>);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = PropertyPath.GetHashCode();
                    hash = 31 * hash + Predicate.GetHashCode();
                    hash = 31 * hash + Value.GetHashCode();
                    return hash;
                }
            }

            private bool CompareValue(T v, T v1)
            {
                var list = v as System.Collections.IList;
                var list1 = v1 as System.Collections.IList;

                if (list == null && list1 == null)
                {
                    return Equals(v, v1);
                }

                if (list.Count != list1.Count)
                {
                    return false;
                }

                for (int i = 0; i < list.Count; i++)
                {
                    if (Equals(list[i], list1[i]) == false)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        [ProtoBuf.ProtoContract]
        public class WhereOptionInt : WhereOption<int> { }
        [ProtoBuf.ProtoContract]
        public class WhereOptionNullableInt : WhereOption<int?> { }
        [ProtoBuf.ProtoContract]
        public class WhereOptionString : WhereOption<string> { }
        [ProtoBuf.ProtoContract]
        public class WhereOptionBool : WhereOption<bool> { }
        [ProtoBuf.ProtoContract]
        public class WhereOptionNullableBool : WhereOption<bool?> { }
        [ProtoBuf.ProtoContract]
        public class WhereOptionDate : WhereOption<DateTime> { }
        [ProtoBuf.ProtoContract]
        public class WhereOptionNullableDate : WhereOption<DateTime?> { }
        [ProtoBuf.ProtoContract]
        public class WhereOptionTime : WhereOption<TimeSpan> { }
        [ProtoBuf.ProtoContract]
        public class WhereOptionNullableTime : WhereOption<TimeSpan?> { }
        [ProtoBuf.ProtoContract]
        public class WhereOptionLong : WhereOption<long> { }
        [ProtoBuf.ProtoContract]
        public class WhereOptionNullableLong : WhereOption<long?> { }
        [ProtoBuf.ProtoContract]
        public class WhereOptionIntList : WhereOption<List<int>> { }
    }
}
