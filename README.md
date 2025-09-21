# Entity Builder Tool

A simple command-line utility that generates C# entity classes from SQL Server database tables. 

## Overview

This tool automates the creation of entity classes by:
- Reading table schema and column definitions from a SQL Server database
- Converting SQL data types to appropriate C# types
- Generating properly cased property names using stop words
- Adding validation attributes based on column constraints
- Supporting nullable types and default values
- Handling foreign key relationships

## Usage

1. Update App.config with your database connection string and default namespace and schema
2. Compile the project using Visual Studio or .NET CLI
3. Run the executable `EntityBuilder.exe`

## Features

- Interactive console UI for customizing entity generation
- Smart property name capitalization (e.g. "customer_id" → "CustomerId") 
- Configurable validation rules via attributes
- Base class inheritance support
- Handles common SQL data types including:
  - Numeric types (int, decimal, etc.)
  - String types with max length
  - Date/time types
  - Binary data
  - GUIDs

## Example Output

```csharp
using cz.InterDream.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Configuration;

namespace MockCompany.Project.Entities
{
	[EntityTable("TABLE123", "dbo")]
	public class Table123 : BaseEntity
	{
		[EntityColumn("COMPANYNAME", true, null)]
		[StringValidator(MaxLength = 100)]
		public string CompanyName { get; set; } = null;

		[EntityColumn("PAYMENTBALANCESIZE", true, Int32.MinValue)]
		public int PaymentBalanceSize { get; set; } = Int32.MinValue;
	}
}
```