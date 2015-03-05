/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

//The central class here is CustomDataService, which is referenced from CustomDataService.svc.
//	That data service is used by the employee recognition letter template.

using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;

[DataServiceKeyAttribute("EmployeeId")]
public class Employee
{
	public int EmployeeId { get; set; }
	public DateTime HireDate { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string Address { get; set; }
	public string City { get; set; }
	public string State { get; set; }
	public string PostalCode { get; set; }
	public string WorkPhone { get; set; }
	public string MobilePhone { get; set; }
	public string HomePhone { get; set; }
	public string Title { get; set; }
	public string ReportsTo { get; set; }
}

[DataServiceKeyAttribute("ManagerId")]
public class Manager
{
	public int ManagerId { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string Title { get; set; }
}

public class HumanResourceData
{
	private static List<Employee> _employees;
	private static List<Manager> _managers;

	static HumanResourceData()
	{
		_employees = new List<Employee>()
		{
			new Employee() { EmployeeId = 1, HireDate = new DateTime(1987, 12, 27), FirstName = "Jason", LastName = "DeMille", 
				Address = "876 Hobart Ave.", City = "Folsom", State = "California", PostalCode = "67656",
				WorkPhone = "(675) 876-7986", MobilePhone = "(675) 245-7876", HomePhone = "(675) 987-6754",
				Title = "Senior Software Developer", ReportsTo = "Stacey Furlocher" },

			new Employee() { EmployeeId = 2, HireDate = new DateTime(1990, 3, 12), FirstName = "Kevin", LastName = "Reynolds", 
				Address = "386 Aspen Street", City = "Auburn", State = "California", PostalCode = "67663",
				WorkPhone = "(675) 876-7998", MobilePhone = "(675) 245-7234", HomePhone = "(677) 745-7643",
				Title = "Senior Software Developer", ReportsTo = "Stacey Furlocher" },

			new Employee() { EmployeeId = 3, HireDate = new DateTime(1998, 5, 13), FirstName = "Julie", LastName = "Smith", 
				Address = "203 Hilson Road", City = "Folsom", State = "California", PostalCode = "67234",
				WorkPhone = "(675) 876-7988", MobilePhone = "(675) 245-7076", HomePhone = "(675) 987-2312",
				Title = "Junior Software Developer", ReportsTo = "Stacey Furlocher" },

			new Employee() { EmployeeId = 4, HireDate = new DateTime(2005, 6, 23), FirstName = "Frank", LastName = "Dobson", 
				Address = "45607 Laggert Ave.", City = "Folsom", State = "California", PostalCode = "67656",
				WorkPhone = "(675) 876-7977", MobilePhone = "(675) 234-2345", HomePhone = "(675) 987-3435",
				Title = "Junior Software Developer", ReportsTo = "Stacey Furlocher" },

			new Employee() { EmployeeId = 5, HireDate = new DateTime(1996, 2, 1), FirstName = "Foley", LastName = "VanDam", 
				Address = "23476 Togart Street", City = "Folsom", State = "California", PostalCode = "67656",
				WorkPhone = "(675) 876-7923", MobilePhone = "(675) 964-6583", HomePhone = "(675) 654-6374",
				Title = "Print Advertising", ReportsTo = "Robert Cummings" },

			new Employee() { EmployeeId = 6, HireDate = new DateTime(2002, 6, 27), FirstName = "Janet", LastName = "Anderson", 
				Address = "236 3rd Street", City = "Roseville", State = "California", PostalCode = "67345",
				WorkPhone = "(675) 876-7967", MobilePhone = "(675) 897-7634", HomePhone = "(675) 654-6223",
				Title = "Online Advertising", ReportsTo = "Robert Cummings" },

			new Employee() { EmployeeId = 7, HireDate = new DateTime(1991, 12, 27), FirstName = "Helen", LastName = "Jones", 
				Address = "876 East Loghorn Road", City = "Auburn", State = "California", PostalCode = "67663",
				WorkPhone = "(675) 876-29435", MobilePhone = "(675) 475-7354", HomePhone = "(675) 364-5675",
				Title = "Telephone Sales Representative", ReportsTo = "Katy Larsen" },

			new Employee() { EmployeeId = 8, HireDate = new DateTime(1999, 4, 18), FirstName = "Lisa", LastName = "Norman", 
				Address = "1123 Alice Road", City = "Auburn", State = "California", PostalCode = "67663",
				WorkPhone = "(675) 876-7945", MobilePhone = "(675) 217-1236", HomePhone = "(675) 964-8754",
				Title = "Online Sales Representative", ReportsTo = "Katy Larsen" },

			new Employee() { EmployeeId = 9, HireDate = new DateTime(2002, 8, 12), FirstName = "Kyle", LastName = "Strickman", 
				Address = "1231 Erdwin Ave", City = "Folsom", State = "California", PostalCode = "67656",
				WorkPhone = "(675) 876-7978", MobilePhone = "(675) 964-6933", HomePhone = "(675) 987-7736",
				Title = "Online Sales Representative", ReportsTo = "Katy Larsen" },

			new Employee() { EmployeeId = 10, HireDate = new DateTime(1990, 4, 11), FirstName = "Benjamin", LastName = "Steele", 
				Address = "236 Hawthorne Road", City = "Roseville", State = "California", PostalCode = "67345",
				WorkPhone = "(675) 876-7984", MobilePhone = "(675) 213-7874", HomePhone = "(675) 324-6734",
				Title = "Telephonic Customer Support Representative", ReportsTo = "Stephen Spence" },

			new Employee() { EmployeeId = 11, HireDate = new DateTime(1995, 5, 25), FirstName = "Lori", LastName = "Hunter", 
				Address = "2132 Allison Drive", City = "Auburn", State = "California", PostalCode = "67345",
				WorkPhone = "(675) 876-7993", MobilePhone = "(675) 234-4765", HomePhone = "(675) 234-5643",
				Title = "Online Customer Support Representative", ReportsTo = "Stephen Spence" },

			new Employee() { EmployeeId = 12, HireDate = new DateTime(1996, 2, 28), FirstName = "Josie", LastName = "Gephart", 
				Address = "236 Tooelson Lane", City = "Folsom", State = "California", PostalCode = "67235",
				WorkPhone = "(675) 876-7999", MobilePhone = "(675) 232-2454", HomePhone = "(675) 468-5434",
				Title = "Online Customer Support Representative", ReportsTo = "Stephen Spence" },
		};

		_managers = new List<Manager>()
		{
			new Manager() { ManagerId = 1, FirstName = "Stacey", LastName = "Furlocher", Title = "Director of Software Development" },
			new Manager() { ManagerId = 2, FirstName = "Robert", LastName = "Cummings", Title = "Director of Marketing" },
			new Manager() { ManagerId = 3, FirstName = "Katy", LastName = "Larsen", Title = "Director of Sales" },
			new Manager() { ManagerId = 4, FirstName = "Stephen", LastName = "Spence", Title = "Senior Manager of Customer Service" }
		};
	}

	public IQueryable<Employee> Employees
	{
		get { return _employees.AsQueryable<Employee>(); }
	}

	public IQueryable<Manager> Managers
	{
		get { return _managers.AsQueryable<Manager>(); }
	}
}

/// <summary>
/// This <c>CustomDataService</c> class is an example data service to demonstrate how to use data services in HotDocs.
/// It is referenced from CustomDataService.svc. The data service itself is used by the employee recognition letter template.
/// </summary>
public class CustomDataService : DataService<HumanResourceData>
{
	// This method is called only once to initialize service-wide policies.
	public static void InitializeService(DataServiceConfiguration config)
	{
		config.SetEntitySetAccessRule("Employees", EntitySetRights.AllRead);
		config.SetEntitySetAccessRule("Managers", EntitySetRights.AllRead);
		config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
	}
}
