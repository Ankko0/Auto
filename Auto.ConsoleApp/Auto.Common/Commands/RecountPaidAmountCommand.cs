using Auto.Common.Entities;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto.Common.Commands
{
    public class RecountPaidAmountCommand
    {
        private readonly IOrganizationService service;
        private readonly ITracingService traceService;
        public RecountPaidAmountCommand(IOrganizationService service, ITracingService traceService)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.traceService = traceService;
        }

        public void Execute(nav_invoice target)
        {
            nav_agreement agreementRecord = new nav_agreement();
            traceService.Trace("Заход в Execute");
            traceService.Trace("target.nav_dogovorid.Id: " + target.nav_dogovorid.Id.ToString());
            agreementRecord = service.Retrieve(nav_agreement.EntityLogicalName,
            target.nav_dogovorid.Id,
            new Microsoft.Xrm.Sdk.Query.ColumnSet(nav_agreement.Fields.nav_factsumma, nav_agreement.Fields.nav_summa)).ToEntity<nav_agreement>();

            if (agreementRecord.nav_factsumma == null)
            {
                traceService.Trace("agreement.nav_factsumma == null");
                agreementRecord.nav_factsumma = target.nav_amount;
            }
            else
            {
                traceService.Trace("agreement.nav_factsumma != null");
                agreementRecord.nav_factsumma = new Money(agreementRecord.nav_factsumma.Value + target.nav_amount.Value);
            }

            if (agreementRecord.nav_factsumma.Value > agreementRecord.nav_summa.Value)
            {
                traceService.Trace("Сумма данного счета превышает сумму договора. agreement.nav_factsumma.Value > agreement.nav_summa.Value");
                throw new InvalidPluginExecutionException("Сумма данного счета превышает сумму договора.");
            }
            target.nav_paydate = DateTime.Now;
            if (agreementRecord.nav_factsumma.Value == target.nav_amount.Value)
                agreementRecord.nav_fact = true;

            traceService.Trace("Попытка service.Update(agreementRecord)");
            service.Update(agreementRecord);
        }

    }
}
