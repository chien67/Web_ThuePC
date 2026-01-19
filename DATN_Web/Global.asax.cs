using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using DATN_Web.BusinessLayer;
using DATN_Web.DataAccesLayer;

namespace DATN_Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            // BƯỚC 1: Khởi tạo Container Builder của Autofac
            var builder = new ContainerBuilder();

            // BƯỚC 2: Đăng ký các Controller (BẮT BUỘC)
            // Autofac cần biết nó phải tạo các Controller từ Assembly nào
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            // BƯỚC 3: Đăng ký các lớp phụ thuộc (DAL và BLL)

            // 1. Đăng ký các DAL
            builder.RegisterType<DeviceModelDAL>().AsSelf().InstancePerRequest();
            builder.RegisterType<DeviceCategoryDAL>().AsSelf().InstancePerRequest();
            builder.RegisterType<DeviceImportDAL>().AsSelf().InstancePerRequest();
            builder.RegisterType<CustomerDAL>().AsSelf().InstancePerRequest();
            builder.RegisterType<OrderDAL>().AsSelf().InstancePerRequest();
            builder.RegisterType<OrderDetailDAL>().AsSelf().InstancePerRequest();
            builder.RegisterType<OrderBLL>().AsSelf().InstancePerRequest();
            builder.RegisterType<CustomerDeviceBLL>().AsSelf();
            builder.RegisterType<BillDAL>().AsSelf().InstancePerRequest();
            builder.RegisterType<BillBLL>().AsSelf().InstancePerRequest();
            builder.RegisterType<DeviceBrokenLogDAL>().AsSelf();
            // ...

            // 2. Đăng ký các BLL (BLL sẽ tự động nhận DAL qua Constructor)
            // Dùng AsSelf() và InstancePerRequest() để quản lý vòng đời
            builder.RegisterType<DeviceModelBLL>().AsSelf().InstancePerRequest();
            builder.RegisterType<CustomerBLL>().AsSelf().InstancePerRequest();
            builder.RegisterType<DeviceBrokenLogBLL>().AsSelf();
            // BƯỚC 4: Xây dựng Container và thiết lập Dependency Resolver
            var container = builder.Build();

            // Thiết lập Autofac làm bộ giải quyết phụ thuộc chính cho MVC
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}
