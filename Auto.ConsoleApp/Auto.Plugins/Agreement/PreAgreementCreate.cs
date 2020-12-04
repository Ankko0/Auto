using Auto.Plugins.Agreement.Handlers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto.Plugins.Agreement
{
    public sealed class PreAgreementCreate:IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var traceService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            traceService.Trace("Получили ITracingService");

            var pluginContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var targetInvoice = (Entity)pluginContext.InputParameters["Target"];
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = serviceFactory.CreateOrganizationService(Guid.Empty);// null

            try
            {
                AgreementService invoiceService = new AgreementService(service, traceService);
                invoiceService.SetDateFirstCommunication(targetInvoice);
            }
            catch (Exception exc)
            {
                traceService.Trace("Ошибка " + exc.ToString());

                throw new InvalidPluginExecutionException(exc.Message);
            }
        }
    }
}
