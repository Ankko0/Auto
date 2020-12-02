using Auto.Common.Commands;
using Microsoft.Xrm.Sdk;
using System;

namespace Auto.Plugins.Invoice.Handlers
{
    public class InvoiceService
    {
        private readonly IOrganizationService service;
        private readonly ITracingService traceService;

        public InvoiceService(IOrganizationService service, ITracingService traceService)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.traceService = traceService;
        }


        public void CopyName(Entity targetInvoice)
        {
            SetInvoiceTypeCommand setInvoiceTypeCommand = new SetInvoiceTypeCommand();
            traceService.Trace("Попытка выполнить setInvoiceTypeCommand");
            setInvoiceTypeCommand.Execute(targetInvoice);
            traceService.Trace("После выполнения setInvoiceTypeCommand");
        }
    }
}
