# ObjectCallContext

Just imagine you want to share object by reference across threads (very close as you can share data with help of [`LogicalCallContext`](https://msdn.microsoft.com/en-us/library/system.runtime.remoting.messaging.logicalcallcontext(v=vs.110).aspx)). `LogicalCallContext` is expensive - it serializes / deserializes data each time thread context is switched. As a result it won't preserve object reference. Idea for ObjectCallContext was borrowed from [`TransactionScope`](https://referencesource.microsoft.com/#System.Transactions/System/Transactions/Transaction.cs,a538de61b60d1252) class.
It works well with both regular `Threads` and `Tasks`

```c#
[TestMethod]
public void WorksWithThreadPoolThreads()
{
    //Arrange
    string key = "test_key";
    object obj = new object();
    bool done = false;

    //Act
    object actual = null;

    ObjectCallContext.TrySetData(key, obj);
    ThreadPool.QueueUserWorkItem(state =>
    {
        ObjectCallContext.TryGetData(key, out actual);
        done = true;
    });

    //Assert
    while (!done) { }
    Assert.AreEqual(obj, actual);
}

[TestMethod]
public async Task WorksWithTasks()
{
    //Arrange
    string key = "test_key";
    object obj = new object();

    //Act
    object actual = null;

    ObjectCallContext.TrySetData(key, obj);
    await Task.Run(() =>
    {
        ObjectCallContext.TryGetData(key, out actual);
    });

    //Assert
    Assert.AreEqual(obj, actual);
}
```


Available as a nuget at https://www.nuget.org/packages/ObjectCallContext/

`PM> Install-Package ObjectCallContext`
