using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Test
{
    [TestClass]
    public class QueryExpressionTest
    {
        [TestMethod]
        public void JsonTest()
        {
            var persions = CreatePersions();

            var qe = new huypq.QueryBuilder.QueryExpression();
            qe.AddOrderByOption(nameof(Persion.Name), false);
            qe.WhereOptions.Add(new huypq.QueryBuilder.WhereExpression.WhereOptionBool("=", nameof(Persion.IsMale), true));
            qe.WhereOptions.Add(new huypq.QueryBuilder.WhereExpression.WhereOptionDate("=", nameof(Persion.DOB), new System.DateTime(2018, 02, 01)));

            var json = huypq.QueryBuilder.QueryExpression.ToJsonString(qe);
            qe = huypq.QueryBuilder.QueryExpression.FromJsonString(json);

            var data = huypq.QueryBuilder.QueryExpression.AddQueryExpression(persions.AsQueryable(), ref qe, out int pageCount).ToList();

            Assert.AreEqual(data.Count, 1);
            Assert.AreEqual(pageCount, 1);
        }

        [TestMethod]
        public void ProtobufTest()
        {
            var persions = CreatePersions();

            var qe = new huypq.QueryBuilder.QueryExpression();
            qe.AddOrderByOption(nameof(Persion.Name), false);
            qe.WhereOptions.Add(new huypq.QueryBuilder.WhereExpression.WhereOptionNullableBool("=", nameof(Persion.HasChildren), null));
            qe.WhereOptions.Add(new huypq.QueryBuilder.WhereExpression.WhereOptionDate("=", nameof(Persion.DOB), new System.DateTime(2018, 02, 01)));

            var protobuf = huypq.QueryBuilder.QueryExpression.ToProtobufByteArray(qe);
            qe = huypq.QueryBuilder.QueryExpression.FromProtobufByteArray(protobuf);

            var data = huypq.QueryBuilder.QueryExpression.AddQueryExpression(persions.AsQueryable(), ref qe, out int pageCount).ToList();

            Assert.AreEqual(data.Count, 1);
            Assert.AreEqual(pageCount, 1);
        }

        System.Collections.Generic.List<Persion> CreatePersions()
        {
            var persions = new System.Collections.Generic.List<Persion>();
            persions.Add(new Persion()
            {
                ID = 1,
                Name = "A",
                DOB = new System.DateTime(2018, 05, 01),
                IsMale = false,
                HasChildren = false
            });
            persions.Add(new Persion()
            {
                ID = 2,
                Name = "B",
                DOB = new System.DateTime(2018, 02, 01),
                IsMale = true,
                HasChildren = null
            });
            return persions;
        }
    }

    public class Persion
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public System.DateTime DOB { get; set; }
        public bool IsMale { get; set; }
        public bool? HasChildren { get; set; }
    }
}
