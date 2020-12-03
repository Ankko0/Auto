using Auto.Common.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {

            var connectionString = "AuthType=OAuth;Username=admin@awesome16.onmicrosoft.com;Password=Maxim!@#;Url=https://trial16.crm4.dynamics.com;RequireNewInstance=true;AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97";
            CrmServiceClient client = new CrmServiceClient(connectionString);
            if (client.LastCrmException != null)
            {
                Console.WriteLine(client.LastCrmException);
            }
            var service = (IOrganizationService)client;
            QueryExpression query = new QueryExpression(nav_communication.EntityLogicalName);
            query.ColumnSet = new ColumnSet(nav_communication.Fields.nav_contactid,
                nav_communication.Fields.nav_email,
                nav_communication.Fields.nav_phone,
                nav_communication.Fields.nav_main,
                nav_communication.Fields.nav_type);
            query.NoLock = true;
            query.TopCount = 20;
            query.Criteria.AddCondition(nav_communication.Fields.nav_main, ConditionOperator.Equal, true);
            var result = client.RetrieveMultiple(query);

            foreach (var entity in result.Entities.Select(e => e.ToEntity<nav_communication>()))
            {
                var contact = client.Retrieve(Contact.EntityLogicalName, entity.nav_contactid.Id, new ColumnSet(Contact.Fields.Telephone1, Contact.Fields.EMailAddress1)).ToEntity<Contact>();
                Contact contactToUpdate = new Contact();

                if (entity.nav_type == nav_communication_nav_type.Telefon && contact.Telephone1 == null)
                {
                    contact.Telephone1 = entity.nav_phone;
                    service.Update(contact);
                }
                if (entity.nav_type == nav_communication_nav_type.E_mail && contact.EMailAddress1 == null)
                {
                    contact.EMailAddress1 = entity.nav_email;
                    service.Update(contact);
                }

            }

            var QEcontact = new QueryExpression("contact");
            QEcontact.TopCount = 1000;
            QEcontact.ColumnSet.AddColumns("contactid", "emailaddress1", "telephone1");
            var allContacts = service.RetrieveMultiple(QEcontact);
            List<Contact> contactsWithoutCommunication = new List<Contact>();
            foreach (var item in allContacts.Entities.Select(t => t.ToEntity<Contact>()))
            {
                try { 
                    var communication = service.Retrieve(nav_communication.EntityLogicalName, (Guid)item.ContactId, new ColumnSet(nav_communication.Fields.nav_contactid)); 
                }
                catch(System.ServiceModel.FaultException ex)
                {
                    contactsWithoutCommunication.Add(item);
                }

            }
            CrmServiceAuto serviceAuto = new CrmServiceAuto();
            serviceAuto.AddCommunication(service, contactsWithoutCommunication);


            Console.Read();
        }
    }
}
