using Auto.Plugins.Communication.Handlers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto.Plugins.Communication
{
    public sealed class PreCommunicationUpdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var traceService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            traceService.Trace("Получили ITracingService");

            var pluginContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var target = (Entity)pluginContext.InputParameters["Target"];

            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = serviceFactory.CreateOrganizationService(Guid.Empty);// null

            try
            {
                Entity postImage = null;
                if (pluginContext.PreEntityImages.Contains("nav_precommunicationupdateimage") && pluginContext.PreEntityImages["nav_precommunicationupdateimage"] is Entity)
                {
                    traceService.Trace("Получили PreEntityImages");
                    postImage = (Entity)pluginContext.PreEntityImages["nav_precommunicationupdateimage"];
                }
                CommunicationService commService = new CommunicationService(service, traceService);
                commService.CheckUniqMainTypeContact(target, postImage);

            }
            catch (Exception exc)
            {
                traceService.Trace("Ошибка " + exc.ToString());

                throw new InvalidPluginExecutionException(exc.Message);
            }
        }
    }
}
