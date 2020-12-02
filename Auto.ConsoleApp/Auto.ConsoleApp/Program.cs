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

                var service = (IOrganizationService)client;
                // Define Condition Values
                var QEnav_agreement_nav_contactid = "9e695fcd-f332-eb11-a813-000d3abe494b";

                // Instantiate QueryExpression QEnav_agreement
                var QEnav_agreement = new QueryExpression("nav_agreement");
                QEnav_agreement.TopCount = 50;

                // Add columns to QEnav_agreement.ColumnSet
                QEnav_agreement.ColumnSet.AddColumns("nav_contactid");

                // Define filter QEnav_agreement.Criteria
                QEnav_agreement.Criteria.AddCondition("nav_contactid", ConditionOperator.Equal, QEnav_agreement_nav_contactid);

                var result2 = service.RetrieveMultiple(QEnav_agreement);

                if (entity.nav_type == nav_communication_nav_type.Telefon && contact.Telephone1 == null)
                {
                    contact.Telephone1 = entity.nav_phone;
                    client.Update(contact);
                }
                if (entity.nav_type == nav_communication_nav_type.E_mail && contact.EMailAddress1 == null)
                {
                    contact.EMailAddress1 = entity.nav_email;
                    client.Update(contact);
                }

                
                //Console.WriteLine($"{entity.Id} {entity.GetAttributeValue<string>("name") }");
            }

            /*QueryExpression query2 = new QueryExpression(Contact.EntityLogicalName);
            query.Criteria = new FilterExpression(LogicalOperator.And);
            query.Criteria.AddCondition(new ConditionExpression("AttributeChildEntityEntityReferenceToParentEntity", ConditionOperator.Equal, new Guid("GuidOfParententity")));

            RetrieveEntityRequest retrieveBankAccountEntityRequest = new RetrieveEntityRequest

            {

                EntityFilters = EntityFilters.Relationships,

                LogicalName = "Contact"

            };

            RetrieveEntityResponse retrieveBankAccountEntityResponse = (RetrieveEntityResponse)service.Execute(retrieveBankAccountEntityRequest);

            var oneToNRelationships = retrieveBankAccountEntityResponse.EntityMetadata.OneToManyRelationships;

            foreach (var oneToNRelationship in oneToNRelationships)

            {
                int temp123 = 5;
                // your code

            }*/

            Console.Read();
        }
    }
}
