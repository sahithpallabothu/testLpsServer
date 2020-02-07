using System.Collections.Generic;

namespace Arista_LPS_WebApp.Entities
{
    public class UserList
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; } 
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public bool IsActive { get; set; }
        public string Token { get; set; }
        public string EmailAddress { get; set; }
        public string Extension { get; set; }
        public string CellPhone { get; set; }    
        public int LocationId { get;set; }
        public string Initials { get; set; }
        public bool IsUserExists { get; set; }
        public string IsPrivilegeExists { get; set; }
        public string LocationDescription { get; set; }
        public string RoleName { get; set; }
        public IEnumerable<RolePrivileges> RolePrivilegeslist { get; set; }
        public List<UserRoles> UserRoles { get; set; }
    }

    public class UserRoles
    {
        public int UserId { get; set; }  
        public int RoleId { get; set; } 
        public string RoleName { get; set; }
    }

    public class LocationClass
    {
        public int LocationId { get;set; }
        public string Description { get; set; } 
        public string Code { get; set; }
        public bool Active { get; set; }
    }
}