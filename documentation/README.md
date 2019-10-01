### Rules

| ID       | Category      | Cause |
|---|---|---|
| [NS1000](rules/NS1000)  | Non-substitutable member | Substituting for non-virtual member of a class. |
| [NS1001](rules/NS1001)  | Non-substitutable member | Checking received calls for non-virtual member of a class.  |
| [NS1002](rules/NS1002)  | Non-substitutable member | Substituting for non-virtual member of a class. |
| [NS1003](rules/NS1003)  | Non-substitutable member | Substituting for an internal member of a class without proxies having visibility into internal members.  |
| [NS2000](rules/NS2000)  | Substitute creation | Substitute.ForPartsOf used with interface or delegate. |
| [NS2001](rules/NS2001)  | Substitute creation | NSubstitute used with class which does not expose public or protected constructor. |
| [NS2002](rules/NS2002)  | Substitute creation | NSubstitute used with class which does not expose parameterless constructor. |
| [NS2003](rules/NS2003)  | Substitute creation | NSubstitute used with internal type. |
| [NS2004](rules/NS2004)  | Substitute creation | Substituting for type by passing wrong constructor arguments. |
| [NS2005](rules/NS2005)  | Substitute creation | Substituting for multiple classes. |
| [NS2006](rules/NS2006)  | Substitute creation | Substituting for interface and passing arguments. |
| [NS2007](rules/NS2007)  | Substitute creation | Substituting for delegate and passing arguments. |
| [NS3000](rules/NS3000)  | Argument specification | Accessing call arguments out of the bounds of method arguments. |
| [NS3001](rules/NS3001)  | Argument specification | Casting call argument at given position to different type than type specified in a method. |
| [NS3002](rules/NS3002)  | Argument specification | Accessing call argument by type which is not present in invocation. |
| [NS3003](rules/NS3003)  | Argument specification | Accessing call argument by type which is used multiple times in invocation. |
| [NS3004](rules/NS3004)  | Argument specification | Assigning call argument with type which is not the same as method argument type. |
| [NS3005](rules/NS3005)  | Argument specification | Assigning call argument which is not ref nor out argument. |
| [NS3006](rules/NS3006)  | Argument specification | Conflicting assignments to out/ref arguments. |
| [NS4000](rules/NS4000)  | Call configuration  | Calling substitute from within `Returns` block. |
| [NS5000](rules/NS5000)  | Usage  | Checking received calls without specifying member. |

### Additional documentation

* [Configuration](Configuration.md)
* [Visual Studio compatibility](Compatibility.md)
