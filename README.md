# DynamicLinq
Adds extensions to Linq to offer dynamic queryables.


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

#Simple Query
```csharp
query.Where("FirstName", ConditionOperators.Equal, "David");
```

#Simple Sorting
```csharp
query = query.OrderByDescending("AuthorId");
query = query.ThenBy("Id");
```

#Collection Filtering
You don't have to Worry about it.
The libary will do it for you.
```csharp
var query = authors.AsQueryable();
query = query.Query(qb =>
{
    qb.NullChecking();
	// you can specify here which collection handling you wish to use Any and All is supported for now.
    qb.And("Posts.Comments.Email", ConditionOperators.Equal, "john.doe@me.com", collectionHandling: QueryCollectionHandling.Any);
});
```

#Null Checking automaticly (practical for in memory dynamic queries)
```csharp
var query = authors.AsQueryable();
query = query.Query(qb =>
{
    qb.NullChecking();
    qb.And("Posts.Comments.Email", ConditionOperators.Equal, "john.doe@me.com", collectionHandling: QueryCollectionHandling.Any);
});
```

#Using Query Builder
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