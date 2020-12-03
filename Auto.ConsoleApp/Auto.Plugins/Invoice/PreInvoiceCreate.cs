using Auto.Plugins.Invoice.Handlers;
using Microsoft.Xrm.Sdk;
using System;

namespace Auto.Plugins.Invoice
{
    public sealed class PreInvoiceCreate : IPlugin
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
                InvoiceService invoiceService = new InvoiceService(service, traceService);
                invoiceService.SetInvoiceType(targetInvoice);
                invoiceService.RecountPaidAmount(targetInvoice);
            }
            catch (Exception exc)
            {
                traceService.Trace("Ошибка " + exc.ToString());
                throw new InvalidPluginExecutionException(exc.Message);
            }
        }
    }
}
