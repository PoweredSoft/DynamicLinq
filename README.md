# DynamicLinq
Adds extensions to Linq to offer dynamic queryables.


# Samples
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

Simple Query
```csharp
query.Where("FirstName", ConditionOperators.Equal, "David");
```
