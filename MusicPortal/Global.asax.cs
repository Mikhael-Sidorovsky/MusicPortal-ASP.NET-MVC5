using MusicPortal.App_Start;
using MusicPortal.Infrastructure;
using MusicPortal.Models;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MusicPortal
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            // внедряем зависимости в проект
            NinjectModule registration = new NinjectRegistration();
            var kernel = new StandardKernel(registration);
            kernel.Unbind<ModelValidatorProvider>();
            DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel));
            // Присваиваем имя алгоритму шифрования 
            EncryptionUtility.AlgorithmName = "DES";
            var location = AppDomain.CurrentDomain.BaseDirectory;
            //Для выделения пути к каталогу воспользуемся `System.IO.Path`:
            EncryptionUtility.KeyFile = Path.GetDirectoryName(location) + "/key.config";
            if (!File.Exists(EncryptionUtility.KeyFile))
                EncryptionUtility.GenerateKey();
            Database.SetInitializer<MusicPortalContext>(new DbInitializer());
            using(MusicPortalContext context = new MusicPortalContext())
            {
                context.Database.Initialize(false);                
            }            
        }
    }
}
