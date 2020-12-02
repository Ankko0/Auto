using Auto.Common.Entities;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto.Common.Commands
{
    public class SetInvoiceTypeCommand
    {
        public void Execute(Entity target)
        {
            var invoice = target.ToEntity<nav_invoice>();
            if (invoice.nav_type == null)
            {
                invoice.nav_type = nav_invoice_nav_type.Ruchnoe_sozdanie;
            }
        }
    }
}
