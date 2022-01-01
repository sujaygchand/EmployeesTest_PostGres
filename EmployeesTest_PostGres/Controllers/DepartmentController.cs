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
	[ProducesResponseType(StatusCodes.Status404NotFound)]
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

			NpgsqlDataReader reader = null;
			using (NpgsqlConnection connection = new NpgsqlConnection(GetSqlDataSource()))
			{
				connection.Open();
				using (NpgsqlCommand myCommand = new NpgsqlCommand(query, connection))
				{
					try
					{
						reader = myCommand.ExecuteReader();
					} 
					catch(Exception ex)
					{
						reader?.Close();
						connection.Close();
						return null;
					}

					table.Load(reader);

					reader.Close();
					connection.Close();

				}
			}

			return table;
		}

		[HttpGet(Name = nameof(GetAllDepartments))]
		[ProducesResponseType(200, Type = typeof(List<Department>))]
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
		public IActionResult GetDepartment(int id)
		{
			string query = $@"
				select ""DepartmentId"",
						""DepartmentName""
				from public.""Departments""
				where ""DepartmentId"" = {id} 
			";

			var table = ExecuteQuery(query);

			return table.Rows.Count > 0 ? Ok(table) : NotFound();
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Department))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult CreateDepartment([FromBody] Department department)
		{
			string query = $@"insert into public.""Departments""(""DepartmentName"")
							values('{department.DepartmentName}')
			";

			var table = ExecuteQuery(query);

			if (table == null)
				return BadRequest("Department already exists");
				
			return CreatedAtAction(nameof(GetDepartment), new { id = department.DepartmentId}, table);
		}

		[HttpPut]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Department))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult UpdateDepartment([FromBody] Department department)
		{
			string query = $@"update public.""Departments""
							set ""DepartmentName"" = '{department.DepartmentName}'
							where ""DepartmentId"" = {department.DepartmentId}
			";

			var table = ExecuteQuery(query);

			if (table == null)
				return BadRequest("Department does not exists");

			return Ok(table);
		}

	}
}
