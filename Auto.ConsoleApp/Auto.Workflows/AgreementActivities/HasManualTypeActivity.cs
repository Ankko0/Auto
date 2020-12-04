using Auto.Workflows.AgreementActivities.Handlers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto.Workflows.AgreementActivities
{
    public class HasManualTypeActivity: CodeActivity
    {
        [Input("Договор")]
        [RequiredArgument]
        [ReferenceTarget("nav_agreement")]
        public InArgument<EntityReference> AgreementReference { get; set; }

        [Output("Есть ли у данного договора счета с типом [Вручную] ?")]
        public OutArgument<bool> HasManualInvoice { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var wfContext = context.GetExtension<IWorkflowContext>();
            var servicefactory = context.GetExtension<IOrganizationServiceFactory>();
            var service = servicefactory.CreateOrganizationService(null);
            var agreementRef = AgreementReference.Get(context);
            try
            {
                AgreementService agreementService = new AgreementService(service);
                bool hasInvoices = agreementService.HasManualInvoicesByAgreementId(agreementRef);
                if (hasInvoices)
                    HasManualInvoice.Set(context, true);
                else
                    HasManualInvoice.Set(context, false);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }

        }
    }
}
