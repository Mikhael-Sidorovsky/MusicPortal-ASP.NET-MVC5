using MusicPortal.Models.Repository;
using Ninject.Modules;

namespace MusicPortal.Infrastructure
{
    public class NinjectRegistration : NinjectModule
    {
        public override void Load()
        {
            Bind<IRepository>().To<MusicPortalRepository>();
        }
    }
}