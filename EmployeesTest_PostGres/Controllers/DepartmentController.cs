using EmployeesTest_PostGres.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;

namespace EmployeesTest_PostGres.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public class DepartmentController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private string sqlDataSource;

		public DepartmentController(IConfiguration configuration)
		{
			_configuration = configuration ?? throw new Exception("configuration is not injected in scope");
		}

		private string GetSqlDataSource()
		{
			sqlDataSource ??= _configuration.GetConnectionString("EmployeeAppCon");
			return sqlDataSource;
		}

		private DataTable ExecuteQuery(string query)
		{
			DataTable table = new DataTable();

			NpgsqlDataReader reader;
			using (NpgsqlConnection connection = new NpgsqlConnection(GetSqlDataSource()))
			{
				connection.Open();
				using (NpgsqlCommand myCommand = new NpgsqlCommand(query, connection))
				{
					reader = myCommand.ExecuteReader();
					table.Load(reader);

					reader.Close();
					connection.Close();

				}
			}

			return table;
		} 

		[HttpGet(Name = nameof(GetAllDepartments))]
		[ProducesResponseType(200, Type = typeof(List<Department>))]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult GetAllDepartments()
		{
			string query = @"
				select ""DepartmentId"",
						""DepartmentName""
				from public.""Departments""
			";

			var table = ExecuteQuery(query);

			return table.Rows.Count > 0 ? Ok(table) : NotFound();
		}

		[HttpGet("{id:int}", Name = nameof(GetDepartment))]
		[ProducesResponseType(200, Type = typeof(Department))]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult GetDepartment(int id)
		{
			string query = $@"
				select ""DepartmentId"",
						""DepartmentName""
				from public.""Departments""
				where ""DepartmentId"" = {id} 
			";

			var table = ExecuteQuery(query);

			return table.Rows.Count > 0  ? Ok(table) : NotFound();
		}
	}
}
