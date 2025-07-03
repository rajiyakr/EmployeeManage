using System.ComponentModel.DataAnnotations;
namespace EmployeeManage.Models
{
    public class Task
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }
        public string AssignedTo { get; set; }//it could be employee id for a drop down
        public string Status { get; set; }//eg:"pending","overdue""Completed"

    }
}
