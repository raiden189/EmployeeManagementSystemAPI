using System.ComponentModel.DataAnnotations;

namespace EmployeeAPI.ViewModel
{
    public class EmployeeViewModel
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Position { get; set; }

        [Required]
        public string? Department { get; set; }

        [Required]
        public DateTime DateOfJoining { get; set; }

        public decimal Salary { get; set; }
    }
}
