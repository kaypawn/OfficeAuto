﻿using Autofac;
using Autofac.Integration.Mvc;
using Saas.Office.Auto.IRepository.Infrastructure;
using Saas.Office.Auto.Repository.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Saas.Office.Auto.Web.App_Start.Core
{
    public class AutofacContainer
    {
        /// <summary>
        /// 创建一个AutoFac容器
        /// </summary>
        public static void Run()
        {
            SetAutofacContainer();
        }
        private static void SetAutofacContainer()
        {
            var builder = new ContainerBuilder();
            SetupResolveRules(builder);//注入
            builder.RegisterControllers(Assembly.GetExecutingAssembly());  //注入所有Controller
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

        }
        private static void SetupResolveRules(ContainerBuilder builder)
        {
            //此层向Service层与DAO层完成依赖注入
            var IService = Assembly.Load("Saas.Office.Auto.IService");
            var Service = Assembly.Load("Saas.Office.Auto.Service");
            var IRepository = Assembly.Load("Saas.Office.Auto.IRepository");
            var Repository = Assembly.Load("Saas.Office.Auto.Repository");
            //匹配以上程序集的字符串末尾字符，只能被注册的程序集实现所有接口
            builder.RegisterAssemblyTypes(IService, Service).Where(t => t.Name.EndsWith("Service")).AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(IRepository, Repository).Where(t => t.Name.EndsWith("Repository")).AsImplementedInterfaces();
            //通过创建反射注册一个组件
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
            builder.RegisterType<DatabaseFactory>().As<IDatabaseFactory>().InstancePerLifetimeScope();
        }
    }
}