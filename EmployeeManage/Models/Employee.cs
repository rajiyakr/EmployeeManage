using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace EmployeeManage.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
       
        [EmailAddress]
        public string EmailAddress { get; set; }
        [Phone]
        public string Phone { get; set; }
        [StringLength(50)]
        public string Department { get; set; }
       
        [Required]
        public int Requests { get; set; }
        public DateTime HireDate { get; set; }
        [Display(Name ="Profile Image")]
        public string? ImagePath { get; set; }
        public bool IsActive { get; set; }
        public string? IdentityUserId { get; set; }
        public IdentityUser? IdentityUser { get; set; }
    }
}
