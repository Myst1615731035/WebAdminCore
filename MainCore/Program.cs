using WebExtention.Injection;

var builder = WebApplication.CreateBuilder(args);
// 注入模块化服务
builder.AddServiceInjection();
//请自行注册自定义服务
var service = builder.Services;
//service.AddScoped<interface, class>

var app = builder.Build();
//使用服务
app.UseAppInjection(builder);
