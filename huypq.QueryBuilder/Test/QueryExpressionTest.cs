using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
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

        [TestMethod]
        public void SqliteInMemoryTest()
        {
            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<PersionContext>()
                    .UseSqlite(connection)
                    .Options;

                // Create the schema in the database
                using (var context = new PersionContext(options))
                {
                    context.Database.EnsureCreated();
                }

                // Run the test against one instance of the context
                using (var context = new PersionContext(options))
                {
                    context.AddRange(CreatePersions());
                    context.SaveChanges();
                }

                // Use a separate instance of the context to verify correct data was saved to database
                using (var context = new PersionContext(options))
                {
                    var qe = new huypq.QueryBuilder.QueryExpression();
                    qe.AddOrderByOption(nameof(Persion.Name), false);
                    qe.WhereOptions.Add(new huypq.QueryBuilder.WhereExpression.WhereOptionNullableBool("=", nameof(Persion.HasChildren), null));
                    qe.WhereOptions.Add(new huypq.QueryBuilder.WhereExpression.WhereOptionDate("=", nameof(Persion.DOB), new System.DateTime(2018, 02, 01)));
                    qe.WhereOptions.Add(new huypq.QueryBuilder.WhereExpression.WhereOptionByteArray("=", nameof(Persion.Secret), new byte[] { 2, 1, 3, 4 }));

                    var data = huypq.QueryBuilder.QueryExpression.AddQueryExpression(context.Persions, ref qe, out int pageCount);

                    var sqlString = data.ToSql();
                    System.Console.WriteLine(sqlString);

                    Assert.AreEqual(1, data.ToList().Count());
                    Assert.AreEqual(1, pageCount);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [TestMethod]
        public void SqliteInMemory_WhereOptionStringList_Test()
        {
            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<PersionContext>()
                    .UseSqlite(connection)
                    .Options;

                // Create the schema in the database
                using (var context = new PersionContext(options))
                {
                    context.Database.EnsureCreated();
                }

                // Run the test against one instance of the context
                using (var context = new PersionContext(options))
                {
                    context.AddRange(CreatePersions());
                    context.SaveChanges();
                }

                // Use a separate instance of the context to verify correct data was saved to database
                using (var context = new PersionContext(options))
                {
                    var qe = new huypq.QueryBuilder.QueryExpression();
                    qe.WhereOptions.Add(new huypq.QueryBuilder.WhereExpression.WhereOptionStringList(nameof(Persion.Name), new System.Collections.Generic.List<string> { "A", "C" }));

                    var data = huypq.QueryBuilder.QueryExpression.AddQueryExpression(context.Persions, ref qe, out int pageCount);

                    var sqlString = data.ToSql();
                    System.Console.WriteLine(sqlString);

                    Assert.AreEqual(1, data.ToList().Count());
                    Assert.AreEqual(1, pageCount);
                }
            }
            finally
            {
                connection.Close();
            }
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
                HasChildren = false,
                Secret = new byte[] { 1, 2, 3, 4 }
            });
            persions.Add(new Persion()
            {
                ID = 2,
                Name = "B",
                DOB = new System.DateTime(2018, 02, 01),
                IsMale = true,
                HasChildren = null,
                Secret = new byte[] { 2, 1, 3, 4 }
            });
            return persions;
        }
    }

    public class PersionContext : DbContext
    {
        public PersionContext()
        { }

        public PersionContext(DbContextOptions<PersionContext> options)
            : base(options)
        { }

        public DbSet<Persion> Persions { get; set; }
    }

    public class Persion
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public System.DateTime DOB { get; set; }
        public bool IsMale { get; set; }
        public bool? HasChildren { get; set; }
        public byte[] Secret { get; set; }
    }
}
