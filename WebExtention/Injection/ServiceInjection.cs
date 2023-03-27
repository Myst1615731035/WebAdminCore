using AspNetCoreRateLimit;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Serilog;
using SqlSugar;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using WebUtils;
using WebUtils.Cache;
using WebExtention.AutofacDI;
using WebUtils.HttpContextUser;
using WebExtention.GlobalFilter;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text;
using WebExtention.Authentication;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.DataProtection;
using WebUtils.Virtual;
using WebModel.DBSeed;
using WebUtils.GlobalConfig;
using Microsoft.AspNetCore.Mvc;
using SqlSugar.Extensions;

namespace WebExtention.Injection
{
    public static class ServiceInjection
    {
        /// <summary>
        /// 组件注入， 注意部分组件的注入顺序
        /// </summary>
        /// <param name="builder"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddServiceInjection(this WebApplicationBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            try
            {
                #region Configure
                builder.Host
                    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                    .ConfigureContainer<ContainerBuilder>((host, t) =>
                    {
                        t.RegisterModule(new AutofacModuleRegister());
                    })
                    .ConfigureAppConfiguration((host, config) =>
                    {
                        // 配置读取放在前面
                        config.Sources.Clear();
                        var dir = new DirectoryInfo(Path.Combine(host.HostingEnvironment.ContentRootPath, "Configs"));
                        if (!dir.Exists)
                        {
                            LogHelper.LogInfo("配置文件目录不存在");
                            throw new ArgumentNullException(nameof(builder));
                        }
                        var settings = dir.GetFiles().Where(t => t.Extension.ToLower() == ".json").ToList();

                        settings.ForEach(f =>
                        {
                            config.AddJsonFile(f.FullName, optional: true, reloadOnChange: true);
                        });
                    });
                builder.Host.UseSerilog((host, service, config) =>
                {
                    config.ReadFrom.Configuration(host.Configuration).ReadFrom.Services(service).Enrich.FromLogContext();
                });

                var services = builder.Services;
                services.AddSingleton(new AppConfig(builder.Configuration, builder.Environment.ContentRootPath, builder.Environment.WebRootPath));
                services.Configure<KestrelServerOptions>(x =>
                    {
                        x.AllowSynchronousIO = true;
                        x.Limits.MaxRequestBodySize = null;
                    })
                    .Configure<IISServerOptions>(x =>
                    {
                        x.AllowSynchronousIO = true;
                        x.MaxRequestBodySize = int.MaxValue;
                    });
                #endregion

                #region ORM
                //数据库访问
                services.AddSingleton<ISqlSugarClient>(service => { return DBHelper.Db; });
                #endregion

                #region DataBase Create And Data Initial
                // 需要实现IDataSeedBase接口，然后再添加此注入;
                services.AddScoped<IDataSeedBase, DBSeed>();
                #endregion

                #region 缓存

                #region Memory
                services.AddScoped<ICaching, MemoryCaching>();
                services.AddSingleton<IMemoryCache>(factory =>
                {
                    var value = factory.GetRequiredService<IOptions<MemoryCacheOptions>>();
                    var cache = new Microsoft.Extensions.Caching.Memory.MemoryCache(value);
                    return cache;
                });

                #endregion

                #region DistributedCache
                services.AddOptions();
                services.TryAdd(ServiceDescriptor.Singleton<IDistributedCache, MemoryDistributedCache>());
                #endregion

                #region Redis
                services.AddTransient<IRedisCache, RedisCache>();

                // 配置启动Redis服务，虽然可能影响项目启动速度，但是不能在运行的时候报错，所以是合理的
                services.AddSingleton<ConnectionMultiplexer>(sp =>
                {
                    //获取连接字符串
                    string redisConfiguration = AppConfig.Get("Redis", "ConnectionString");

                    var configuration = ConfigurationOptions.Parse(redisConfiguration, true);

                    configuration.ResolveDns = true;

                    return ConnectionMultiplexer.Connect(configuration);
                });
                #endregion

                #endregion

                #region CORS 跨域配置
                // 注入全局的请求实例对象
                services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                // 注入跨域配置
                services.AddCors(c =>
                {
                    c.AddPolicy(AppConfig.Get(new string[] { "HttpRequest", "Cors", "PolicyName" }),
                            policy =>
                            {
                                policy.WithOrigins(AppConfig.Get(new string[] { "HttpRequest", "Cors", "IPs" }).Split(','))
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                            });
                });
                #endregion

                #region 权限
                services.AddScoped<IUser, AspNetUser>();
                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
                // 先添加权限校验组件 
                services.AddJWTAuthorization();
                // 后添加授权组件
                services.AddJWTAuthentication();
                #endregion

                #region Request Pipe
                services.AddSession();
                services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "DataProtection"));
                services.AddControllers(o =>
                {
                    //全局错误捕获
                    o.Filters.Add(typeof(GlobalExceptionsFilter));
                    if(AppConfig.Get("Program", "GlobalPermissionCheck").ObjToBool())
                        o.Conventions.Insert(0, new GlobalRouteAuthorizeConvention());
                    if(AppConfig.Get("Program", "GlobalRouterPrefixCheck").ObjToBool())
                        o.Conventions.Insert(0, new GlobalRoutePrefixFilter(new RouteAttribute(GConfig.RoutePrefix)));
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                });
                services.AddEndpointsApiExplorer();
                builder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                #endregion

                #region WebSocket
                services.AddSignalR().AddNewtonsoftJsonProtocol();
                #endregion

                #region Swagger
                services.AddSwaggerGen();
                #endregion

                #region 消息队列
                // Redis

                // RabbitMQ

                //Kafka

                #endregion

                #region 微服务/服务注册
                //EventBus

                //Nacos

                #endregion

                #region Timer Task
                #endregion

                #region Monitor
                //MiniProfiler
                services.AddMiniProfiler();
                // needed to store rate limit counters and ip rules
                services.AddMemoryCache();
                //load general configuration from config
                services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
                // inject counter and rules stores
                services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
                services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
                services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
                // inject counter and rules distributed cache stores
                services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
                services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();
                services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
                #endregion
            }
            catch(Exception ex)
            {
                LogHelper.LogException(ex);
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
