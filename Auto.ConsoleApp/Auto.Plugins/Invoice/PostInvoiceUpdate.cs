using Auto.Common.Entities;
using Auto.Plugins.Invoice.Handlers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto.Plugins.Invoice
{
    public sealed class PostInvoiceUpdate:IPlugin
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
                Entity postImage = null;
                if (pluginContext.PostEntityImages.Contains("nav_postinvoiceupdateimage") && pluginContext.PostEntityImages["nav_postinvoiceupdateimage"] is Entity)
                {
                    traceService.Trace("Получили PreEntityImages");
                    postImage = (Entity)pluginContext.PostEntityImages["nav_postinvoiceupdateimage"];
                }

                traceService.Trace("вход в  RecountPaidAmount");
                invoiceService.RecountPaidAmount(targetInvoice, postImage);
            }
            catch (Exception exc)
            {
                traceService.Trace("Ошибка " + exc.ToString());
                throw new InvalidPluginExecutionException(exc.Message);
            }
        }
    }

}
