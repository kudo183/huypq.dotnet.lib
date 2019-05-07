using System.Collections.Generic;
using System.Linq;

namespace huypq.QueryBuilder
{
    [ProtoBuf.ProtoContract]
    public class QueryExpression : System.IEquatable<QueryExpression>
    {
        [ProtoBuf.ProtoMember(1)]
        public int PageIndex { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public int PageSize { get; set; }
        [ProtoBuf.ProtoMember(3)]
        public List<OrderByExpression.OrderOption> OrderOptions { get; set; }
        [ProtoBuf.ProtoMember(4)]
        public List<WhereExpression.IWhereOption> WhereOptions { get; set; }

        public QueryExpression()
        {
            OrderOptions = new List<OrderByExpression.OrderOption>();
            WhereOptions = new List<WhereExpression.IWhereOption>();
        }

        public bool Equals(QueryExpression other)
        {
            if (ReferenceEquals(null, other) == true)
                return false;

            if (ReferenceEquals(this, other) == true)
                return true;

            if (PageIndex != other.PageIndex
                || PageSize != other.PageSize
                || OrderOptions.Count != other.OrderOptions.Count
                || WhereOptions.Count != other.WhereOptions.Count)
            {
                return false;
            }

            for (int i = 0; i < other.OrderOptions.Count; i++)
            {
                if (object.Equals(OrderOptions[i], other.OrderOptions[i]) == false)
                    return false;
            }

            for (int i = 0; i < other.WhereOptions.Count; i++)
            {
                if (object.Equals(WhereOptions[i], other.WhereOptions[i]) == false)
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as QueryExpression);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = PageIndex.GetHashCode();
                hash = 31 * hash + PageSize.GetHashCode();

                //for performance, just calculate Count of list
                hash = 31 * hash + OrderOptions.Count.GetHashCode();
                hash = 31 * hash + WhereOptions.Count.GetHashCode();
                return hash;
            }
        }

        public void AddWhereOption<T1, T>(string predicate, string propertyPath, T value)
            where T1 : WhereExpression.WhereOption<T>, new()
        {
            var t = new T1();
            t.Predicate = predicate;
            t.PropertyPath = propertyPath;
            t.Value = value;
            WhereOptions.Add(t);
        }

        public void AddOrderByOption(string propertyPath, bool isAscending)
        {
            OrderOptions.Add(new OrderByExpression.OrderOption()
            {
                PropertyPath = propertyPath,
                IsAscending = isAscending
            });
        }

        public static IQueryable<TSource> AddQueryExpression<TSource>
            (IQueryable<TSource> source, ref QueryExpression filter, out int pageCount)
        {
            source = WhereExpression.AddWhereExpression(source, filter.WhereOptions);
            var itemCount = source.Count();
            if (itemCount == 0)
            {
                pageCount = 0;
                return source;
            }
            source = OrderByExpression.AddOrderByExpression(source, filter.OrderOptions);
            pageCount = 1;
            if (filter.PageIndex > 0)
            {
                var pageIndex = filter.PageIndex;
                source = Paging.AddPaging(source, ref pageIndex, itemCount, filter.PageSize, out pageCount);
                filter.PageIndex = pageIndex;
            }
            return source;
        }

        public static IQueryable<TSource> AddQueryExpression<TSource>
            (IQueryable<TSource> source, string filterJson, out int pageCount)
        {
            var filter = FromJsonString(filterJson);
            source = AddQueryExpression(source, ref filter, out pageCount);
            return source;
        }

        public static QueryExpression FromJsonString(string json)
        {
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<QueryExpression>(json, new Newtonsoft.Json.JsonSerializerSettings() { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto });
            return result;
        }

        public static string ToJsonString(QueryExpression filterExpression)
        {
            var result = Newtonsoft.Json.JsonConvert.SerializeObject(filterExpression, new Newtonsoft.Json.JsonSerializerSettings() { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto });
            return result;
        }

        public static QueryExpression FromProtobufByteArray(byte[] protobuf)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream(protobuf);
            var result = ProtoBuf.Serializer.Deserialize<QueryExpression>(ms);
            return result;
        }

        public static byte[] ToProtobufByteArray(QueryExpression filterExpression)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            ProtoBuf.Serializer.Serialize(ms, filterExpression);
            return ms.ToArray();
        }
    }
}
