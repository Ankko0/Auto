﻿using Auto.Workflows.AgreementActivities.Handlers;
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
    public class CreatePaymentScheduleActivity : CodeActivity
    {
        [Input("Договор")]
        [RequiredArgument]
        [ReferenceTarget("nav_agreement")]
        public InArgument<EntityReference> AgreementReference { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var wfContext = context.GetExtension<IWorkflowContext>();
            var servicefactory = context.GetExtension<IOrganizationServiceFactory>();
            var service = servicefactory.CreateOrganizationService(null);
            var agreementRef = AgreementReference.Get(context);
            try
            {
                AgreementService agreementService = new AgreementService(service);
                agreementService.CreatePaymentSchedule(agreementRef);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }

        }

    }
}
