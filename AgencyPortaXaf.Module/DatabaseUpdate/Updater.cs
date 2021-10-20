using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using AgencyPortaXaf.Module.BusinessObjects;
using DevExpress.ExpressApp.Security;

namespace AgencyPortaXaf.Module.DatabaseUpdate {
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Updating.ModuleUpdater
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            //string name = "MyName";
            //DomainObject1 theObject = ObjectSpace.FirstOrDefault<DomainObject1>(u => u.Name == name);
            //if(theObject == null) {
            //    theObject = ObjectSpace.CreateObject<DomainObject1>();
            //    theObject.Name = name;
            //}

            //ObjectSpace.CommitChanges(); //Uncomment this line to persist created object(s).
            UpdateDb();
        }
        public override void UpdateDatabaseBeforeUpdateSchema() {
            base.UpdateDatabaseBeforeUpdateSchema();
            //if(CurrentDBVersion < new Version("1.1.0.0") && CurrentDBVersion > new Version("0.0.0.0")) {
            //    RenameColumn("DomainObject1Table", "OldColumnName", "NewColumnName");
            //}
        }

        public void UpdateDb()
        {
            PermissionPolicyRole administrativeRole = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(role => role.Name == SecurityStrategy.AdministratorRoleName);
            if (administrativeRole == null)
            {
                administrativeRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                administrativeRole.Name = SecurityStrategy.AdministratorRoleName;
                administrativeRole.IsAdministrative = true;
            }
            const string adminName = "Administrator";
            Employee administratorUser = ObjectSpace.FirstOrDefault<Employee>(employee => employee.UserName == adminName);
            if (administratorUser == null)
            {
                administratorUser = ObjectSpace.CreateObject<Employee>();
                administratorUser.UserName = adminName;
                administratorUser.IsActive = true;
                administratorUser.SetPassword("");
                administratorUser.Roles.Add(administrativeRole);
            }
            PermissionPolicyRole userRole = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(role => role.Name == "User");
            if (userRole == null)
            {
                userRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                userRole.Name = "User";
                userRole.AddTypePermission<Employee>(
                    SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                userRole.AddTypePermission<Company>(
                    SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
            }
            if (ObjectSpace.FindObject<Company>(null) == null)
            {
                Company company1 = ObjectSpace.CreateObject<Company>();
                company1.Name = "Company 1";
                company1.Employees.Add(administratorUser);
                Employee user1 = ObjectSpace.CreateObject<Employee>();
                user1.UserName = "Sam";
                user1.SetPassword("");
                user1.Roles.Add(userRole);
                Employee user2 = ObjectSpace.CreateObject<Employee>();
                user2.UserName = "John";
                user2.SetPassword("");
                user2.Roles.Add(userRole);
                Company company2 = ObjectSpace.CreateObject<Company>();
                company2.Name = "Company 2";
                company2.Employees.Add(user1);
                company2.Employees.Add(user2);
            }
            ObjectSpace.CommitChanges();
        }
    }
}
