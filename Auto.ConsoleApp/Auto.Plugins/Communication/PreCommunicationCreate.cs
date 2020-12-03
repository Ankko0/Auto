using Auto.Plugins.Communication.Handlers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto.Plugins.Communication
{
    public sealed class PreCommunicationCreate:IPlugin
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
                CommunicationService commService = new CommunicationService(service, traceService);
                commService.CheckUniqMainTypeContact(target);
                //commService.RecountPaidAmount(targetInvoice);
                //throw new InvalidPluginExecutionException("Должен был сработать");
            }
            catch (Exception exc)
            {
                traceService.Trace("Ошибка " + exc.ToString());

                throw new InvalidPluginExecutionException(exc.Message);
            }
        }
    }
}
