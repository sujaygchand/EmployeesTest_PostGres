using System.ComponentModel.DataAnnotations;

namespace EmployeesTest_PostGres.Models
{
	public class Department
	{
		[Key]
		public int DepartmentId { get; set; }

		[Required]
		public string DepartmentName { get; set; }
	}
}
