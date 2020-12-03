using Auto.Common.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto.Common.Commands
{
    public class CheckUniqMainTypeContactCommand
    {
        private readonly IOrganizationService service;
        private readonly ITracingService traceService;
        public CheckUniqMainTypeContactCommand(IOrganizationService service, ITracingService traceService)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.traceService = traceService;
        }
        public void Execute(nav_communication communication)
        {
            traceService.Trace("Заход в Execute");
            if (communication.nav_main == true)
            {
                traceService.Trace("Заход в communication.nav_main == true");
                // Define Condition Values
                var QEnav_communication_nav_main = true;
                var QEnav_communication_0_nav_contactid = communication.nav_contactid.Id.ToString();
                var QEnav_communication_1_nav_type = (int)communication.nav_type;

                var QEnav_communication = new QueryExpression("nav_communication");
                QEnav_communication.TopCount = 50;

                QEnav_communication.ColumnSet.AddColumns("nav_name");
                QEnav_communication.Criteria.AddCondition("nav_main", ConditionOperator.Equal, QEnav_communication_nav_main);
                var QEnav_communication_Criteria_0 = new FilterExpression();
                QEnav_communication.Criteria.AddFilter(QEnav_communication_Criteria_0);
                QEnav_communication_Criteria_0.AddCondition("nav_contactid", ConditionOperator.Equal, QEnav_communication_0_nav_contactid);
                var QEnav_communication_Criteria_1 = new FilterExpression();
                QEnav_communication.Criteria.AddFilter(QEnav_communication_Criteria_1);
                QEnav_communication_Criteria_1.AddCondition("nav_type", ConditionOperator.Equal, QEnav_communication_1_nav_type);

                traceService.Trace("попытка service.RetrieveMultiple(QEnav_communication)");
                var result = service.RetrieveMultiple(QEnav_communication);
                traceService.Trace("выход из service.RetrieveMultiple(QEnav_communication)");

                if (result.Entities.Count != 0 )
                {
                    traceService.Trace("result.Entities.Count != 0");
                    throw new InvalidPluginExecutionException("Ошибка! У этого контакта уже есть основное средство связи этого типа.");
                }
            }
        }
    }
}
