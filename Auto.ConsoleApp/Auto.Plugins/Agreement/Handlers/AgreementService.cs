using Auto.Common.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Auto.Plugins.Agreement.Handlers
{
    public class AgreementService
    {
        private readonly IOrganizationService service;
        private readonly ITracingService traceService;

        public AgreementService(IOrganizationService service, ITracingService traceService)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.traceService = traceService;
        }


        public void SetDateFirstCommunication(Entity targetInvoice)
        {
            var agreement = targetInvoice.ToEntity<nav_agreement>();

            var QEnav_agreement_nav_contactid = agreement.nav_contactid.Id;
            var QEnav_agreement = new QueryExpression("nav_agreement");
            QEnav_agreement.TopCount = 50;
            QEnav_agreement.ColumnSet.AddColumns("nav_contactid");
            QEnav_agreement.Criteria.AddCondition("nav_contactid", ConditionOperator.Equal, QEnav_agreement_nav_contactid);
            
            traceService.Trace("Попытка выполнить RetrieveMultiple");
            var result = service.RetrieveMultiple(QEnav_agreement);
            traceService.Trace("RetrieveMultiple.Entities.Count = " + result.Entities.Count);
            traceService.Trace("contact.Id " + agreement.nav_contactid.Id.ToString());
            if (result.Entities.Count == 0)
            {
                traceService.Trace("Вход в if");
                Contact contact = new Contact();
                contact.Id = agreement.nav_contactid.Id;
                contact.nav_date = agreement.nav_date;
                service.Update(contact);
                traceService.Trace("После service.Update(contact) ");
            }
            traceService.Trace("Выход из CopyName");
        }
    }
}
