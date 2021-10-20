using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Xpo;

namespace AgencyPortaXaf.Module.BusinessObjects
{
    [DefaultClassOptions, DefaultProperty(nameof(UserName))]
    public class Employee : PermissionPolicyUser
    {
        public Employee(Session session) : base(session) { }
        private Company company;
        [Association("Company-Employees")]
        public Company Company
        {
            get { return company; }
            set { SetPropertyValue(nameof(Company), ref company, value); }
        }
    }
}
