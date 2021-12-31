using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeesTest_PostGres.Models
{
	public class Employee
	{
		[Key]
		public int EmployeeId { get; set; }
		
		[Required]
		public string EmployeeName { get; set; }

		public int DepartmentId { get; set; }

		[ForeignKey("DepartmentId")]
		public Department Department { get; set; }
		
		[Required]
		public DateTime DateOfJoining { get; set; }

		public byte[] Picture { get; set; }
	}
}
