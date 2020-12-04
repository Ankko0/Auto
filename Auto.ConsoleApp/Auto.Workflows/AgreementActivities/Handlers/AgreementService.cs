using Auto.Common.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto.Workflows.AgreementActivities.Handlers
{
    public class AgreementService
    {
        private readonly IOrganizationService service;

        public AgreementService(IOrganizationService service)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
        }
        public bool HasPayedInvoicesByAgreementId(EntityReference agreementReference)
        {
            var QEnav_invoice_nav_dogovorid = agreementReference.Id;
            var QEnav_invoice_nav_fact = true;
            var QEnav_invoice = new QueryExpression("nav_invoice");
            QEnav_invoice.TopCount = 50;
            QEnav_invoice.ColumnSet.AddColumns("nav_invoiceid");
            QEnav_invoice.Criteria.AddCondition("nav_dogovorid", ConditionOperator.Equal, QEnav_invoice_nav_dogovorid);
            QEnav_invoice.Criteria.AddCondition("nav_fact", ConditionOperator.Equal, QEnav_invoice_nav_fact);
            var invoices = service.RetrieveMultiple(QEnav_invoice);
            if (invoices.Entities.Count == 0)
                return false;
            else
                return true;
        }

        public bool HasManualInvoicesByAgreementId(EntityReference agreementReference)
        {
            // Ручное создание
            var QEnav_invoice_nav_type = 808630000;
            var QEnav_invoice_nav_dogovorid = agreementReference.Id;
            var QEnav_invoice = new QueryExpression("nav_invoice");
            QEnav_invoice.TopCount = 50;
            QEnav_invoice.ColumnSet.AddColumns("nav_invoiceid");
            QEnav_invoice.Criteria.AddCondition("nav_dogovorid", ConditionOperator.Equal, QEnav_invoice_nav_dogovorid);
            QEnav_invoice.Criteria.AddCondition("nav_type", ConditionOperator.Equal, QEnav_invoice_nav_type);

            var invoices = service.RetrieveMultiple(QEnav_invoice);
            if (invoices.Entities.Count == 0)
                return false;
            else
                return true;

        }

        public void DeleteAutocreatedRelatedInvoices(EntityReference agreementReference)
        {
            var QEnav_invoice_nav_invoiceid = agreementReference.Id;
            
            // Автоматическое создание
            var QEnav_invoice_nav_type = 808630001;

            var QEnav_invoice = new QueryExpression("nav_invoice");
            QEnav_invoice.TopCount = 100;
            QEnav_invoice.ColumnSet.AddColumns("nav_invoiceid");
            QEnav_invoice.Criteria.AddCondition("nav_dogovorid", ConditionOperator.Equal, QEnav_invoice_nav_invoiceid);
            QEnav_invoice.Criteria.AddCondition("nav_type", ConditionOperator.Equal, QEnav_invoice_nav_type);
            var invoices = service.RetrieveMultiple(QEnav_invoice);
            foreach (var invoice in invoices.Entities)
            {
                service.Delete("nav_invoice", invoice.Id);
            }

        }
        public void CreatePaymentSchedule(EntityReference agreementReference)
        {

            var agreement = service.Retrieve("nav_agreement", agreementReference.Id, new ColumnSet("nav_name", "nav_agreementid", "nav_creditperiod", "nav_fullcreditamount", "nav_paymentplandate")).ToEntity<nav_agreement>();
            if (agreement == null || agreement.nav_name == null || agreement.nav_creditperiod == null || agreement.nav_fullcreditamount == null)
                throw new InvalidPluginExecutionException("Одно из полей не заполенено ! ");

            // Колличество месяцев, если не целое значение, то + 1
            var creditPeriodInMonth = Math.Ceiling((decimal)(agreement.nav_creditperiod * 12));

            var loanFeePerMonth = agreement.nav_fullcreditamount.Value / creditPeriodInMonth;


            for (int i = 0; i < creditPeriodInMonth; i++)
            {
                nav_invoice newInvoice = new nav_invoice();
                newInvoice.nav_invoiceId = service.Create(newInvoice);
                newInvoice.nav_name = "Счет на оплату кредита по договору " + agreement.nav_name;
                newInvoice.nav_type = nav_invoice_nav_type.Avtomaticheskoe_sozdanie;
                newInvoice.nav_date = DateTime.Now;
                newInvoice.nav_paydate = DateTime.Now.AddDays(1).AddMonths(i + 1);
                newInvoice.nav_dogovorid = new EntityReference(nav_agreement.EntityLogicalName, (Guid)agreement.nav_agreementId);
                newInvoice.nav_fact = false;
                newInvoice.nav_amount = new Money(loanFeePerMonth);
                agreement.nav_paymentplandate = DateTime.Now.AddDays(1);

                service.Update(agreement);
                service.Update(newInvoice);
            }

        }
    }
}
