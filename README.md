#DapperWrapper

##Overview

Dapper is the fastest and most concise database access library on the market today. [Stack Overflow](http://stackoverflow.com) uses it because of its speed, and with millions of hits every day that's saying something.

However, to keep it fast and lightweight the contributors of the library work very hard to ensure that it **only** meets the needs of database access. This left room for a lightweight library to be set on top of it that is capable of automating some of the tasks necessary to working with the POCO objects. Enter *DapperWrapper*!

##Benefits

* Lightweight
* Simple
* Attribute Driven
* 97.23% Code Coverage

###Lightweight

The library was written to be lightweight. It's a single class, `DataModelBase`, in the same spirit as Dapper. For example, if you wanted to gain database access for a table named `Person` you would build a POCO like this:

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

    var list = Person.Query();

The `Query` method will connect to the database `using` a `SqlConnection` and call the `Query<T>` method that's in the Dapper library with a statement like this `SELECT [Id], [FirstName], [LastName] FROM [Person]`.

You could filter that query if you wanted:

    var list = Person.Query(null, new { Id = 1 }, "Id");

and that would build a query like this `SELECT [Id], [FirstName], [LastName] FROM [Person] WHERE [Id] = @Id`.

In either case the result of `Query` would send back an `IEnumerable<Person>`.

But `SELECT` statements aren't the only thing it can generate, it generates `INSERT`, `UPDATE`, and `DELETE` statements as well. If I wanted to update a `Person` I might do this:

    var p = Person.Query(null, new { Id = 1 }, "Id").FirstOrDefault();
    if (p == null) { return; }
    
    p.FirstName = "New First Name";
    Person.Execute(p.Update, p, "Id");

That would execute a query like this `UPDATE [Person] SET [FirstName] = @FirstName, [LastName] = @LastName WHERE [Id] = @Id`.

If I wanted to `INSERT` a new `Person` I might do this:

    var p = new Person();
    p.FirstName = "Michael";
    p.LastName = "Perrenoud";
    Person.Execute(p.Insert, p);

That would execute a query like this `INSERT INTO [Person] ([FirstName], [LastName]) VALUES (@FirstName, @LastName)`.

Or maybe I wanted to `DELETE` a `Person`:

    var p = Person.Query(null, new { Id = 1 }, "Id").FirstOrDefault();
    if (p == null) { return; }
    
    Person.Execute(p.Delete, p, "Id");

That would execute a query like this `DELETE FROM [Person] WHERE [Id] = @Id`.

###Simple

As you can see from the examples, or at least I hope you agree, it's pretty simple. You can build the POCO with some basic attributing and then leverage some pretty straight forward commands to execute queries without having to build them yourself. Of course, if the queries were much more complex you'd have to send one in manually. But that can be done:

    var list = Person.Query("SELECT p.*, u.UserName FROM Person p JOIN User u ON p.Id = u.PersonId WHERE p.Id = @Id", new { Id = 5 });

###Attribute Driven

The idea is that the POCO objects are attribute driven so that they can be easily *configured* in code.

###97.23% Code Coverage

A unit test project exists to ensure high levels of code coverage so that stability remains a high priority. My goal is to continue to keep this above 95% at all times.
