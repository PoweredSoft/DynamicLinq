# DynamicLinq
Adds extensions to Linq to offer dynamic queryables.

## Roadmap
Check "Projects" section of github to see whats going on.

https://github.com/PoweredSoft/DynamicLinq/projects/1

## Download
Full Version | NuGet | NuGet Install
------------ | :-------------: | :-------------:
PoweredSoft.DynamicLinq | <a href="https://www.nuget.org/packages/PoweredSoft.DynamicLinq/" target="_blank">[![NuGet](https://img.shields.io/nuget/v/PoweredSoft.DynamicLinq.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/PoweredSoft.DynamicLinq/)</a> | ```PM> Install-Package PoweredSoft.DynamicLinq```
PoweredSoft.DynamicLinq.EntityFramework | <a href="https://www.nuget.org/packages/PoweredSoft.EntityFramework/" target="_blank">[![NuGet](https://img.shields.io/nuget/v/PoweredSoft.DynamicLinq.EntityFramework.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/PoweredSoft.DynamicLinq.EntityFramework/)</a> | ```PM> Install-Package PoweredSoft.DynamicLinq.EntityFramework```


## Samples
Complex Query
```csharp
query = query.Query(q =>
{
    q.Compare("AuthorId", ConditionOperators.Equal, 1);
    q.And(sq =>
    {
        sq.Compare("Content", ConditionOperators.Equal, "World");
        sq.Or("Title", ConditionOperators.Contains, 3);
    });
});
```

### Shortcuts
Shortcuts allow to avoid specifying the condition operator by having it handy in the method name
```csharp
queryable.Query(t => t.Contains("FirstName", "Dav").OrContains("FirstName", "Jo"));
```
You may visit this test for more examples: https://github.com/PoweredSoft/DynamicLinq/blob/master/PoweredSoft.DynamicLinq.Test/ShortcutTests.cs

### Simple Query
```csharp
query.Where("FirstName", ConditionOperators.Equal, "David");
```

### Grouping Support
```csharp
TestData.Sales
	.AsQueryable()
	.GroupBy(t => t.Path("ClientId"))
	.Select(t =>
	{
	    t.Key("TheClientId", "ClientId");
	    t.Count("Count");
	    t.LongCount("LongCount");
	    t.Sum("NetSales");
	    t.Average("Tax", "TaxAverage");
	    t.ToList("Sales");
	});
```	
Is equivalent to
```csharp
TestSales
	.GroupBy(t => new { t.ClientId })
	.Select(t => new {
	    TheClientId = t.Key.ClientId,
	    Count = t.Count(),
	    LongCount = t.LongCount(),
	    NetSales = t.Sum(t2 => t2.NetSales),
	    TaxAverage = t.Average(t2 => t2.Tax),
	    Sales = t.ToList()
	});
```       

### In Support
You can filter with a list, this will generate a contains with your list.
```csharp
var ageGroup = new List<int>() { 28, 27, 50 };
Persons.AsQueryable().Query(t => t.In("Age", ageGroup));
```

### String Comparision Support
```csharp
Persons.AsQueryable().Query(t => t.Equal("FirstName", "DAVID", stringComparision: StringComparison.OrdinalIgnoreCase));
```
You may visit this test for more examples:
https://github.com/PoweredSoft/DynamicLinq/blob/master/PoweredSoft.DynamicLinq.Test/StringComparision.cs

### Simple Sorting
```csharp
query = query.OrderByDescending("AuthorId");
query = query.ThenBy("Id");
```

### Collection Filtering
You don't have to Worry about it.
The library will do it for you.
```csharp
var query = authors.AsQueryable();
query = query.Query(qb =>
{
    qb.NullChecking();
	// you can specify here which collection handling you wish to use Any and All is supported for now.
    qb.And("Posts.Comments.Email", ConditionOperators.Equal, "john.doe@me.com", collectionHandling: QueryCollectionHandling.Any);
});
```

### Null Checking is automatic (practical for in memory dynamic queries)
```csharp
var query = authors.AsQueryable();
query = query.Query(qb =>
{
    qb.NullChecking();
    qb.And("Posts.Comments.Email", ConditionOperators.Equal, "john.doe@me.com", collectionHandling: QueryCollectionHandling.Any);
});
```

### Using Query Builder
```csharp
// subject.
var posts = new List<Post>()
{
    new Post { Id = 1, AuthorId = 1, Title = "Hello 1", Content = "World" },
    new Post { Id = 2, AuthorId = 1, Title = "Hello 2", Content = "World" },
    new Post { Id = 3, AuthorId = 2, Title = "Hello 3", Content = "World" },
};

// the query.
var query = posts.AsQueryable();
var queryBuilder = new QueryBuilder<Post>(query);

queryBuilder.Compare("AuthorId", ConditionOperators.Equal, 1);
queryBuilder.And(subQuery =>
{
    subQuery.Compare("Content", ConditionOperators.Equal, "World");
    subQuery.Or("Title", ConditionOperators.Contains, 3);
});

query = queryBuilder.Build();
```

### Entity Framework

Using PoweredSoft.DynamicLinq.EntityFramework it adds an helper that allows you to do the following.

```csharp
var context = new <YOUR CONTEXT>();
var queryable = context.Query(typeof(Author), q => q.Compare("FirstName", ConditionOperators.Equal, "David"));
var result = queryable.ToListAsync().Result;
var first = result.FirstOrDefault() as Author;
Assert.AreEqual(first?.FirstName, "David");
```

### How it can be used in a web api

```csharp
[HttpGet][Route("FindClients")]
public IHttpActionResult FindClients(string filterField = null, string filterValue = null, 
string sortProperty = "Id", int? page = null, int pageSize = 50)
{
    var ctx = new MyDbContext();
    var query = ctx.Clients.AsQueryable();

    if (!string.IsNullOrEmpty(filterField) && !string.IsNullOrEmpty(filterValue))
	query = query.Query(t => t.Contains(filterField, filterValue)).OrderBy(sortProperty);

    //  count.
    var clientCount = query.Count();
    int? pages = null;

    if (page.HasValue && pageSize > 0)
    {
	if (clientCount == 0)
	    pages = 0;
	else
	    pages = clientCount / pageSize + (clientCount % pageSize != 0 ? 1 : 0);
    }

    if (page.HasValue)
	query = query.Skip((page.Value-1) * pageSize).Take(pageSize);

    var clients = query.ToList();

    return Ok(new
    {
	total = clientCount,
	pages = pages,
	data = clients
    });
}
```
