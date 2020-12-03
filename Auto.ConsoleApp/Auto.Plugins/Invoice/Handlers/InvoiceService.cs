using Auto.Common.Commands;
using Auto.Common.Entities;
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

        public void SetInvoiceType(Entity targetInvoice)
        {
            var invoice = targetInvoice.ToEntity<nav_invoice>();
            SetInvoiceTypeCommand setInvoiceTypeCommand = new SetInvoiceTypeCommand();
            traceService.Trace("Попытка выполнить setInvoiceTypeCommand.Execute(invoice);");
            setInvoiceTypeCommand.Execute(invoice);
        }

        public void RecountPaidAmount(Entity targetInvoice, Entity nav_preinvoiceimage = null)
        {
            var invoice = targetInvoice.ToEntity<nav_invoice>();
            if (nav_preinvoiceimage != null)
            {
                var invoiceImage = nav_preinvoiceimage.ToEntity<nav_invoice>();
                invoice.nav_dogovorid = invoiceImage.nav_dogovorid;
                invoice.nav_amount = invoiceImage.nav_amount;

            }
            RecountPaidAmountCommand recount = new RecountPaidAmountCommand(service, traceService);
            if (invoice.nav_fact == true)
            {
                traceService.Trace("Счет оплачен. Заход в if");
                traceService.Trace("Попытка выполнить RecountPaidAmountCommand.Execute(invoice)");
                recount.Execute(invoice);

            }
        }


    }
}

