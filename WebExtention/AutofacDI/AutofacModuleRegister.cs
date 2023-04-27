using Autofac;
using Microsoft.IdentityModel.Logging;
using System.Reflection;
using Autofac.Extras.DynamicProxy;
using Microsoft.AspNetCore.Mvc;
using WebUtils.BaseService;
using WebUtils.BaseService;

namespace WebExtention.AutofacDI
{
    public class AutofacModuleRegister : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            #region 服务类DLL引用
            var basePath = AppContext.BaseDirectory;
            var modelDll = Path.Combine(basePath, "WebModel.dll");
            var serviceDll = Path.Combine(basePath, "WebService.dll");
            var controllerDll = Path.Combine(basePath, "MainCore.dll");

            if (!File.Exists(modelDll) || !File.Exists(serviceDll) || !File.Exists(controllerDll))
            {
                var msg = "部分dll丢失。请检查";
                LogHelper.LogInformation(msg);
                throw new Exception(msg);
            }
            #endregion

            var cacheType = new List<Type>();

            #region 泛型仓储注入
            builder.RegisterGeneric(typeof(BaseService<>)).As(typeof(IBaseService<>)).InstancePerDependency();
            #endregion

            #region 带有接口层的服务注入
            // 获取 WebModel.dll 程序集注册 Model
            var modelAssembly = Assembly.LoadFrom(modelDll);
            builder.RegisterAssemblyTypes(modelAssembly)
                      .AsImplementedInterfaces()
                      .InstancePerDependency()
                      .PropertiesAutowired()
                      .EnableInterfaceInterceptors()//引用Autofac.Extras.DynamicProxy;
                      .InterceptedBy(cacheType.ToArray());//允许将拦截器服务的列表分配给注册。

            // 获取 WebService.dll 程序集注册 Repository & Service
            var serviceAssembly = Assembly.LoadFrom(serviceDll);
            builder.RegisterAssemblyTypes(serviceAssembly)
                   .AsImplementedInterfaces()
                   .PropertiesAutowired()
                   .InstancePerDependency();

            // 获取 MainCore.dll 程序集注册 Controller
            var controllerAssembly = Assembly.LoadFrom(controllerDll);
            var controllerBaseType = typeof(ControllerBase);
            builder.RegisterAssemblyTypes(controllerAssembly)
                .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
                .PropertiesAutowired();
            #endregion

            #region 没有接口层的服务层注入
            //因为没有接口层，所以不能实现解耦，只能用 Load 方法。
            //注意如果使用没有接口的服务，并想对其使用 AOP 拦截，就必须设置为虚方法
            //var assemblysServicesNoInterfaces = Assembly.Load("WebService");
            //builder.RegisterAssemblyTypes(assemblysServicesNoInterfaces);
            #endregion

            #region 没有接口的单独类，启用class代理拦截
            //只能注入该类中的虚方法，且必须是public
            //这里仅仅是一个单独类无接口测试，不用过多追问
            //builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof( <!-- 类名 --> )))
            //    .EnableClassInterceptors()
            //    .InterceptedBy(cacheType.ToArray());
            #endregion

            #region 单独注册一个含有接口的类，启用interface代理拦截
            //不用虚方法
            //builder.RegisterType<AopService>().As<IAopService>()
            //   .AsImplementedInterfaces()
            //   .EnableInterfaceInterceptors()
            //   .InterceptedBy(typeof(BlogCacheAOP));
            #endregion
        }
    }
}
