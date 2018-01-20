# 基础使用

契约即服务是指只需定义接口，通过简单的配置，即可把接口的实现类和对其的调用从应用内调用解耦成服务间调用，摇身一变成为面向服务架构(SOA),目前支持HTTP服务。

## 定义契约

服务契约最简单可以是一个普通接口类型，通过简单配置，shriek-fx即可以把它的实现类变成服务端和可调用此服务的客户端，当然，二者可以单独使用。

```csharp

public interface ISimpleInterface
{
    Task<string> Test(string sth);
}
```

## 服务设置

需要安装`Shriek.ServiceProxy.Http.Server`类库

只需要简单地在`Startup.cs`文件中的`ConfigureServices`方法中的AddMVC()后面加入以下一行：

```csharp
//Startup.cs

public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc().AddWebApiProxyServer(opt =>
    {
         opt.AddService<ISimpleInterface>();
    });
}
```

## 服务调用

需要安装`Shriek.ServiceProxy.Http`类库

首先，实现这个接口定义一个类，这个类只是简单的类：

```csharp

public class SimpleInterface : ISimpleInterface
{
    public async Task<string> Test(string sth)
    {
        return await Task.FromResult("server: " + sth);
    }
}

```

然后需要在`Startup.cs`类中简单地在`IServiceCollection`中注册我们的客户端组件：

```csharp

var services = new ServiceCollection();

services.AddWebApiProxy(opt =>
{
    opt.AddService<ISimpleInterface>("http://localhost:5000");
});

```

调用时可以直接从`IServiceProvider`获取实例，并且直接调用。

```csharp

var provider = services.BuildServiceProvider();
var simpleService = provider.GetService<ISimpleInterface>();
var result = await simpleService.Test("hello word!"); // -->result = "server: hello word!"
```
---

当在ASP.NET Core中使用时，只需在`Startup.cs`类中的`ConfigureServices`方法中给`services`变量通过以上方法注册，即可在控制器注入客户端了。

```csharp
//Startup.cs

public void ConfigureServices(IServiceCollection services)
{
    services.AddWebApiProxy(opt =>
    {
        opt.AddService<ISimpleInterface>("http://localhost:5000");
    })
}
```

初学者可能会感觉奇怪，“我没注册接口实例呀，怎么就能注入一个实例呢？”其实，这个实例是AOP动态代理出来的，会给接口方法绑定一个HttpClient的封装。这个特性还会有更多高级用法，如拦截器、路由配置等等，请看下一章节！