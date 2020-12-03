using Auto.Plugins.Invoice.Handlers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto.Plugins.Invoice
{
    /* Уточнить логику, нужен ли пересчет при удалении УЖЕ оплаченного счета (т.е. после перерасчета)*/
    public sealed class PostIvoiceDelete 
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
                invoiceService.RecountPaidAmount(targetInvoice);
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
