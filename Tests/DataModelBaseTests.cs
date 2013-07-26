using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dapper;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class DataModelBaseTests
    {
        [TestMethod]
        public void PrimaryKeyIsValid()
        {
            var sut = new Person();
            Assert.AreEqual("Id", sut.PrimaryKey);
        }

        [TestMethod]
        public void DataFieldsIsValid()
        {
            var sut = new Person();
            var expected = new string[] { "Id", "FirstName", "LastName" };

            Assert.IsTrue(sut.DataFields.SequenceEqual(expected));
        }

        [TestMethod]
        public void SelectStatementValid()
        {
            var sut = new Person();
            Assert.AreEqual("SELECT [Id], [FirstName], [LastName] FROM [Person]", sut.Select);
        }

        [TestMethod]
        public void InsertStatementValid()
        {
            var sut = new Person();
            Assert.AreEqual("INSERT INTO [Person] ([FirstName], [LastName]) VALUES (@FirstName, @LastName)", sut.Insert);
        }

        [TestMethod]
        public void UpdateStatementValid()
        {
            var sut = new Person();
            Assert.AreEqual("UPDATE [Person] SET [FirstName] = @FirstName, [LastName] = @LastName", sut.Update);
        }

        [TestMethod]
        public void DeleteStatementValid()
        {
            var sut = new Person();
            Assert.AreEqual("DELETE FROM [Person]", sut.Delete);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DataTableAttributeMissing()
        {
            var sut = new PersonMissingDataTableAttribute();
        }

        [TestMethod]
        public void DataTableHasExtraField()
        {
            var sut = new PersonExtraField();
            var expected = new string[] { "Id", "FirstName", "LastName" };

            Assert.IsTrue(sut.DataFields.SequenceEqual(expected));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DataTableMissingPrimaryKey()
        {
            var sut = new PersonMissingPrimaryKey();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DataTableMultiplePrimaryKey()
        {
            var sut = new PersonMultiplePrimaryKey();
        }

        [TestMethod]
        public void VerifyConcatenatedSelect()
        {
            var sut = new Person();
            Assert.AreEqual("SELECT [Id], [FirstName], [LastName] FROM [Person] WHERE [Id] = @Id", sut.Select.Filter("Id"));
        }

        [TestMethod]
        public void VerifyConcatenatedUpdate()
        {
            var sut = new Person();
            Assert.AreEqual("UPDATE [Person] SET [FirstName] = @FirstName, [LastName] = @LastName WHERE [Id] = @Id", sut.Update.Filter("Id"));
        }

        [TestMethod]
        public void VerifyConcatenatedDelete()
        {
            var sut = new Person();
            Assert.AreEqual("DELETE FROM [Person] WHERE [Id] = @Id", sut.Delete.Filter("Id"));
        }

        [TestMethod]
        public void GetFieldValue()
        {
            var sut = new Person
            {
                Id = 1,
            };

            Assert.AreEqual(1, sut.Field<int>("Id"));
        }

        [TestMethod]
        public void GetFieldValueOfMissingField()
        {
            var sut = new Person();
            Assert.AreEqual(null, sut.Field<string>("NoField"));
        }

        [TestMethod]
        public void GetFieldValueOfMismatchedType()
        {
            var sut = new Person();
            Assert.AreEqual(null, sut.Field<string>("Id"));
        }

        [TestMethod]
        public void GetFieldValueOfNullField()
        {
            var sut = new Person();
            Assert.AreEqual(null, sut.Field<string>("FirstName"));
        }

        [TestMethod]
        public void VerifySelectPerson()
        {
            var list = Person.Query();
        }

        [TestMethod]
        public void VerifyInsertPerson()
        {
            Person.Execute("DELETE Person");

            var p = new Person();
            p.FirstName = "Michael";
            p.LastName = "Perrenoud";
            Person.Execute(p.Insert, p);

            var newP = Person.Query(null, new { FirstName = "Michael", LastName = "Perrenoud" }, "FirstName", "LastName").FirstOrDefault();
            Assert.IsNotNull(newP);

            Person.Execute("DELETE Person");
        }

        [TestMethod]
        public void VerifyUpdatePerson()
        {
            Person.Execute("DELETE Person");

            var p = new Person();
            p.FirstName = "Michael";
            p.LastName = "Perrenoud";
            Person.Execute(p.Insert, p);

            var newP = Person.Query(null, new { FirstName = "Michael", LastName = "Perrenoud" }, "FirstName", "LastName").FirstOrDefault();
            Assert.IsNotNull(newP);

            newP.LastName = "New Perrenoud";
            Person.Execute(newP.Update, newP, "Id");

            newP = Person.Query(null, new { FirstName = "Michael", LastName = "New Perrenoud" }, "FirstName", "LastName").FirstOrDefault();
            Assert.IsNotNull(newP);

            Person.Execute("DELETE Person");
        }

        [DataTable("Person")]
        private class Person : DataModelBase<Person>
        {
            public Person()
                : base(@"Data Source=Data.sdf")
            {

            }

            [DataField(IsPrimaryKey = true)]
            public int Id { get; set; }

            [DataField]
            public string FirstName { get; set; }

            [DataField]
            public string LastName { get; set; }
        }

        private class PersonMissingDataTableAttribute : DataModelBase<PersonMissingDataTableAttribute>
        {
            [DataField(IsPrimaryKey = true)]
            public int Id { get; set; }

            [DataField]
            public string FirstName { get; set; }

            [DataField]
            public string LastName { get; set; }
        }

        [DataTable("Person")]
        private class PersonExtraField : DataModelBase<PersonExtraField>
        {
            [DataField(IsPrimaryKey = true)]
            public int Id { get; set; }

            [DataField]
            public string FirstName { get; set; }

            [DataField]
            public string LastName { get; set; }

            public string ExtraField { get; set; }
        }

        [DataTable("Person")]
        private class PersonMissingPrimaryKey : DataModelBase<PersonMissingPrimaryKey>
        {
            [DataField]
            public string FirstName { get; set; }

            [DataField]
            public string LastName { get; set; }
        }

        [DataTable("Person")]
        private class PersonMultiplePrimaryKey : DataModelBase<PersonMultiplePrimaryKey>
        {
            [DataField(IsPrimaryKey = true)]
            public int Id { get; set; }

            [DataField(IsPrimaryKey = true)]
            public int Id2 { get; set; }

            [DataField]
            public string FirstName { get; set; }

            [DataField]
            public string LastName { get; set; }
        }
    }
}
