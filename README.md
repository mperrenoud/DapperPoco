#DapperWrapper

##Overview

Dapper is the fastest and most concise database access library on the market today. [Stack Overflow](http://stackoverflow.com) uses it because of its speed, and with millions of hits every day that's saying something.

However, to keep it fast and lightweight the contributors of the library work very hard to ensure that it **only** meets the needs of database access. This left room for a lightweight library to be set on top of it that is capable of automating some of the tasks necessary to working with the POCO objects. Enter *DapperWrapper*!

##Benefits

* Lightweight
* Fast
* Simple
* Attribute Driven
* 85.56% Code Coverage

###Lightweight

The library was written to be lightweight. It's a single class, `DataModelBase`, in the spirit of Dapper! For example, if you wanted to gain database access for a table named `Person` you would build a class like this:

    [DataTable("Person")]
    public class Person : DataModelBase
    {
        public Person()
            : base("connection string")
        {
        }
        
        [DataField(IsPrimaryKey = true)]
        public int Id { get; set; }
        [DataField]
        public string FirstName { get; set; }
        [DataField]
        public string LastName { get; set; }
    }

With this configuration you are now able to query all `Person` records like this:

    var list = Person.Query(QueryTypes.Select);

The `Query` method will connect to the database `using` a `SqlConnection` and call the `Query<T>` method that's in the Dapper library with a statement like this `SELECT [Id], [FirstName], [LastName] FROM [Person]`.

You could filter that query if you wanted:

    var list = Person.Query(QueryTypes.Select, new { Id = 1 }, "Id");

and that would build a query like this `SELECT [Id], [FirstName], [LastName] FROM [Person] WHERE [Id] = @Id`.

In either case the result of `Query` would send back an `IEnumerable<Person>`.

But `SELECT` statements aren't the only thing it can generate, it generates `INSERT`, `UPDATE`, and `DELETE` statements as well. If I wanted to update a `Person` I might do this:

    var p = Person.Query(QueryTypes.Select, new { Id = 1 }, "Id").FirstOrDefault();
    if (p == null) { return; }
    
    p.FirstName = "New First Name";
    p.Execute(QueryTypes.Update, "Id");
