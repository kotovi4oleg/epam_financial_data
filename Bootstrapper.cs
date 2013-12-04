using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc3;
using FinancialData.Models.Infrastructure;
namespace FinancialData
{
    public static class Bootstrapper
    {
        public static void Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();
            container.RegisterType<IController, FinancialData.Controllers.FinanceController>("Finance");
            container.RegisterType<IRetrieveable<string>, FinanceRetrieve>();
            
            return container;
        }
    }
}