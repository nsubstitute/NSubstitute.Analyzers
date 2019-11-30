
## Rules

| ID       | Category      | Cause |
|---|---|---|
| [NS1000](NS1000.md) | Non-substitutable member | Substituting for non-virtual member of a class. |
| [NS1001](NS1001.md) | Non-substitutable member | Checking received calls for non-virtual member of a class. |
| [NS1002](NS1002.md) | Non-substitutable member | Substituting for non-virtual member of a class. |
| [NS1003](NS1003.md) | Non-substitutable member | Substituting for an internal member of a class without proxies having visibility into internal members. |
| [NS1004](NS1004.md) | Non-substitutable member | Argument matcher used with a non-virtual member of a class. |
| [NS2000](NS2000.md) | Substitute creation | Substitute.ForPartsOf used with interface or delegate. |
| [NS2001](NS2001.md) | Substitute creation | NSubstitute used with class which does not expose public or protected constructor. |
| [NS2002](NS2002.md) | Substitute creation | NSubstitute used with class which does not expose parameterless constructor. |
| [NS2003](NS2003.md) | Substitute creation | NSubstitute used with internal type. |
| [NS2004](NS2004.md) | Substitute creation | Substituting for type by passing wrong constructor arguments. |
| [NS2005](NS2005.md) | Substitute creation | Substituting for multiple classes. |
| [NS2006](NS2006.md) | Substitute creation | Substituting for interface and passing arguments. |
| [NS2007](NS2007.md) | Substitute creation | Substituting for delegate and passing arguments. |
| [NS3000](NS3000.md) | Argument specification | Accessing call arguments out of the bounds of method arguments. |
| [NS3001](NS3001.md) | Argument specification | Casting call argument at given position to different type than type specified in a method. |
| [NS3002](NS3002.md) | Argument specification | Accessing call argument by type which is not present in invocation. |
| [NS3003](NS3003.md) | Argument specification | Accessing call argument by type which is used multiple times in invocation. |
| [NS3004](NS3004.md) | Argument specification | Assigning call argument with type which is not the same as method argument type. |
| [NS3005](NS3005.md) | Argument specification | Assigning call argument which is not ref nor out argument. |
| [NS3006](NS3006.md) | Argument specification | Conflicting assignments to out/ref arguments. |
| [NS4000](NS4000.md) | Call configuration | Calling substitute from within `Returns` block. |
| [NS5000](NS5000.md) | Usage | Checking received calls without specifying member. |