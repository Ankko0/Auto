using Auto.Common.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto.ConsoleApp
{
    class CrmServiceAuto
    {
        public void AddCommunication(IOrganizationService service, List<Contact> contactsWithoutComunication)
        {
            foreach (var contact in contactsWithoutComunication)
            {
                if (contact.Telephone1 != null && contact.EMailAddress1 == null)
                {
                    var newCommunicationId = service.Create(new nav_communication());
                    var newCommunication = service.Retrieve(nav_communication.EntityLogicalName, newCommunicationId, new ColumnSet(true)).ToEntity<nav_communication>();
                    newCommunication.nav_phone = contact.Telephone1;
                    newCommunication.nav_name = contact.FullName;
                    newCommunication.nav_main = true;
                    newCommunication.nav_type = nav_communication_nav_type.Telefon;
                    newCommunication.nav_contactid.Id = (Guid)contact.ContactId;

                    service.Update(newCommunication);

                }

                if (contact.Telephone1 == null && contact.EMailAddress1 != null)
                {
                    var newCommunicationId = service.Create(new nav_communication());
                    var newCommunication = service.Retrieve(nav_communication.EntityLogicalName, newCommunicationId, new ColumnSet(true)).ToEntity<nav_communication>();
                    newCommunication.nav_email = contact.EMailAddress1;
                    newCommunication.nav_name = contact.FullName;
                    newCommunication.nav_main = false;
                    newCommunication.nav_type = nav_communication_nav_type.E_mail;
                    newCommunication.nav_contactid.Id = (Guid)contact.ContactId;

                    service.Update(newCommunication);

                }

                if (contact.Telephone1 != null && contact.EMailAddress1 != null)
                {
                    var newCommunicationId = service.Create(new nav_communication());
                    var newCommunication = service.Retrieve(nav_communication.EntityLogicalName, newCommunicationId, new ColumnSet(true)).ToEntity<nav_communication>();
                    newCommunication.nav_phone = contact.Telephone1;
                    newCommunication.nav_name = contact.FullName + " Phone";
                    newCommunication.nav_main = true;
                    newCommunication.nav_type = nav_communication_nav_type.Telefon;
                    newCommunication.nav_contactid.Id = (Guid)contact.ContactId;

                    service.Update(newCommunication);

                    newCommunicationId = service.Create(new nav_communication());
                    newCommunication = service.Retrieve(nav_communication.EntityLogicalName, newCommunicationId, new ColumnSet(true)).ToEntity<nav_communication>();
                    newCommunication.nav_email = contact.EMailAddress1;
                    newCommunication.nav_name = contact.FullName + "E-mail";
                    newCommunication.nav_main = false;
                    newCommunication.nav_type = nav_communication_nav_type.E_mail;
                    newCommunication.nav_contactid.Id = (Guid)contact.ContactId;

                    service.Update(newCommunication);

                }
            }
        }
    }
}
