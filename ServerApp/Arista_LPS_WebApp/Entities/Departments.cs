using System.ComponentModel.DataAnnotations;

namespace Arista_LPS_WebApp.Entities
{
    public class Departments
    {

        [Key]
        public string DeptID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        public bool Active { get; set; }
        public string ISINUSE { get; set; }
    }
}
