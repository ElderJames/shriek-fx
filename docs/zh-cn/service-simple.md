# 基础使用

## 定义契约

服务契约最简单可以是一个普通接口类型，通过简单配置，shriek-fx 即可以把它的实现类变成服务端和可调用此服务的客户端，当然，二者可以单独使用。

```csharp
public interface ISimpleInterface
{
    Task<string> Test(string sth);
}
```

## 服务设置

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

接下来，我们要把这个实现类设置成一个网络服务，只需要简单地在`Startup.cs`文件中的`ConfigureServices`方法中使用注册方法。

1. HTTP 服务的配置

   ```csharp
   //Startup.cs

   public void ConfigureServices(IServiceCollection services)
   {
       //HTTP 配置
       services.AddMvc().AddWebApiProxyServer(opt =>
       {
           opt.AddService<ISimpleInterface>();
       });
   }
   ```

   此时，已经自动配置了一个可以用 POST 方式调用的 WebApi，接受一个字符串参数，路由是用接口名+方法名+参数名生成的。

   > HTTP 服务设置需要安装`Shriek.ServiceProxy.Http.Server`类库。另外还需要以下 ASP.NET Core 基础类库:
   >
   > * Microsoft.AspNetCore.Hosting.Server.Abstractions
   > * Microsoft.AspNetCore.Hosting
   > * Microsoft.AspNetCore.Server.Kestrel
   > * Microsoft.Extensions.DependencyInjection

2. TCP 服务的配置

   ```csharp
   //Startup.cs

   public void ConfigureServices(IServiceCollection services)
   {
       //TCP 配置
       services.AddSocketServer(opt =>
       {
           opt.EndPoint = new IPEndPoint(IPAddress.Loopback, 1212);
           opt.AddService<ISimpleInterface>();
       });
   }
   ```

   此时，已经配置好了一个监听 1212 端口的 TCP 服务端，可以通过配套客户端来调用服务端的特定方法。

   > TCP 服务设置需要安装`Shriek.ServiceProxy.Socket.Server`类库

## 服务调用

只需要在需要调用服务端简单地在`IServiceCollection`中注册我们的客户端组件。

### 注册组件

1. 注册 HTTP 客户端组件

   ```csharp
   var services = new ServiceCollection();

   services.AddWebApiProxy(opt =>
   {
        option.ProxyHost = "http://localhost:5000";
        opt.AddService<ISimpleInterface>();
   });
   ```

   > 服务调用需要安装`Shriek.ServiceProxy.Http`类库

2. 注册 TCP 客户端组件

   ```csharp
   var services = new ServiceCollection();

   services.AddSocketProxy(options =>
   {
        options.ProxyHost = "localhost:1212";//对应服务端的ip和端口号
        options.AddService<ISimpleInterface>();
   });
   ```

   > 服务调用需要安装`Shriek.ServiceProxy.Socket`类库

### 调用方法

调用时可以直接从`IServiceProvider`获取实例，并且直接调用。

```csharp
var provider = services.BuildServiceProvider();
var simpleService = provider.GetService<ISimpleInterface>();
var result = await simpleService.Test("hello word!"); // -->result = "server: hello word!"
```

### 注意

当在 ASP.NET Core 中使用时，则需在`Startup.cs`类中的`ConfigureServices`方法中通过`IServiceCollection`注册客户端组建，即可在控制器（或者其他在控制器注入的组件中）注入客户端了。

```csharp
//Startup.cs

public void ConfigureServices(IServiceCollection services)
{
    services.AddWebApiProxy(opt =>
    {
        option.ProxyHost = "http://localhost:5000";
        opt.AddService<ISimpleInterface>();
    });

    services.AddSocketProxy(options =>
    {
        options.ProxyHost = "localhost:1212";//对应服务端的ip和端口号
        options.AddService<ISimpleInterface>();
    });
}
```

初学者可能会感觉奇怪，“我没注册接口实例呀，怎么就能注入一个实例呢？”其实，这个实例是 AOP 动态代理出来的，会给接口方法绑定一个 HttpClient 的封装。这个特性还会有更多高级用法，如拦截器、路由配置等等，请看下一章节！
