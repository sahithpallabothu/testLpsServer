using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Arista_LPS_WebApp.Entities
{
    public class Roles
    {
        [Key]
        public int? RoleId { get; set; }

        public string RoleName { get; set; }

        public string RoleDescriprion { get; set; }

        public bool? RoleActive { get; set; }
        public string ISINUSE { get; set; }

        // public string  Username {get; set;}
        public List<RolePrivileges> Privileges { get; set; }

    }

    public class RolePrivileges
    {
        public int? RoleId { get; set; }
        public int RolePrivilegeId { get; set; }
        public string ScreenName { get; set; }
        public string Privilege { get; set; }
        public bool read { get; set; }
        public bool insert { get; set; }
        public bool update { get; set; }
        public bool delete { get; set; }
        public bool iDisable { get; set; }
        public bool rDisable { get; set; }
        public bool uDisable { get; set; }
        public bool dDisable { get; set; }
    }


}
