### Step 1 - creating the project
```
md servicebus
cd servicebus
dotnet new console
code .
dotnet add package Microsoft.Azure.ServiceBus
```

### Step 2 - Sending a message

```cs
```

```
dotnet run $connectionString "hello world"
```


### Useful documentation

- [Azure Service Bus Docs](https://docs.microsoft.com/en-gb/azure/service-bus-messaging/)
- [Microsoft.Azure.ServiceBus NuGet](https://www.nuget.org/packages/Microsoft.Azure.ServiceBus)
    - [GitHub readme](https://github.com/Azure/azure-sdk-for-net/tree/master/sdk/servicebus/Microsoft.Azure.ServiceBus)