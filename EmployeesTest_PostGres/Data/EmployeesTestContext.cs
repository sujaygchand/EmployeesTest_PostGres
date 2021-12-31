using EmployeesTest_PostGres.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeesTest_PostGres.Data
{
	public class EmployeesTestContext : DbContext
	{
		public EmployeesTestContext(DbContextOptions<EmployeesTestContext> options) : base(options)
		{

		}

		public DbSet<Department> Departments { get; set; }
		public DbSet<Employee> Employees { get; set; }
	}
}
