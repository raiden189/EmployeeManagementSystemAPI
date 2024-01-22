using EmployeeAPI.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAPI.Model
{
    public class Employee
    {
        [HiddenInput]
        public int ID { get; set; }

        public string? Name { get; set; }

        public string? Position { get; set; }

        public string? Department { get; set; }

        public DateTime DateOfJoining { get; set; }

        public decimal Salary { get; set; }
    }
}
