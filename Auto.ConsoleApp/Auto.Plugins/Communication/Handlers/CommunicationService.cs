using Auto.Common.Commands;
using Auto.Common.Entities;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto.Plugins.Communication.Handlers
{
    public class CommunicationService
    {
        private readonly IOrganizationService service;
        private readonly ITracingService traceService;

        public CommunicationService(IOrganizationService service, ITracingService traceService)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.traceService = traceService;
        }

        public void CheckUniqMainTypeContact(Entity targetInvoice, Entity image= null)
        {

            traceService.Trace("Заход в CheckUniqMainTypeContact");
            var communication = targetInvoice.ToEntity<nav_communication>();
            if (communication.nav_main ==true)
            {
                traceService.Trace("Тип контакта основной, заход в if");
                if (image != null)
                {
                    traceService.Trace("Изменение контакта, получение данных из image");
                    var communicationImage = image.ToEntity<nav_communication>();
                    communication.nav_contactid = communicationImage.nav_contactid;
                    communication.nav_type = communicationImage.nav_type;

                }
                CheckUniqMainTypeContactCommand checkCommand = new CheckUniqMainTypeContactCommand(service, traceService);
                traceService.Trace("Попытка выполнить setInvoiceTypeCommand.Execute(invoice);");
                checkCommand.Execute(communication);
            }

        }

    }
}
