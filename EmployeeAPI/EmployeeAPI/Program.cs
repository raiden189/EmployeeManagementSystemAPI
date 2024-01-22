using EmployeeAPI.DataAccess;
using EmployeeAPI.Model;
using EmployeeAPI.ViewModel;
using Microsoft.EntityFrameworkCore;
using MiniValidation;
using EmployeeAPI.Helper;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", OpenAPISecurity.GetInfo());
    options.AddSecurityDefinition("Bearer", OpenAPISecurity.GetSecurityScheme());
    options.AddSecurityRequirement(OpenAPISecurity.GetSecurityRequirements());
});

builder.Services.AddAuthentication(options => 
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => 
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateAudience = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty)),
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddDbContext<EmployeeContext>(opt => opt.UseInMemoryDatabase("Employee"));
var app = builder.Build();

#region api

//get all employees
app.MapGet("/api/getEmployees", [Authorize] async (EmployeeContext context) =>
    await context.Employee.ToListAsync());


//get employee by id
app.MapGet("/api/getEmployeeById/{Id}", [Authorize] async (int Id, EmployeeContext context) =>
    await context.Employee.FindAsync(Id)
        is Employee employee
            ? Results.Ok(employee)
            : Results.NotFound());

//add new employee
app.MapPost("/api/addEmployee", [Authorize] async (EmployeeViewModel employee, EmployeeContext context) =>
{
    if(!MiniValidator.TryValidate(employee, out var errors))
        return TypedResults.ValidationProblem(errors);

    var newEmployee = new Employee
    {
        Name = employee.Name,
        Department = employee.Department,
        Position = employee.Position,
        DateOfJoining = employee.DateOfJoining,
        Salary = employee.Salary,
    };

    context.Employee.Add(newEmployee);
    await context.SaveChangesAsync();
    return Results.Created($"/employeeItems/{newEmployee.ID}", employee);
});

//update employee by id
app.MapPost("/api/updateEmployee/{Id}", [Authorize] async (int Id, EmployeeViewModel employee, EmployeeContext context) => 
{
    if (!MiniValidator.TryValidate(employee, out var errors))
        return TypedResults.ValidationProblem(errors);

    var dbEmployee = await context.Employee.FindAsync(Id);

    if(dbEmployee is null) 
        return Results.NotFound();

    dbEmployee.Name = employee.Name;
    dbEmployee.Position = employee.Position;
    dbEmployee.Department = employee.Department;
    dbEmployee.DateOfJoining = employee.DateOfJoining;
    dbEmployee.Salary = employee.Salary;

    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPost("/api/login", [AllowAnonymous] (User user) => 
{
    if (user.Username == "red189" && user.Password == "123")
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = JWTAuthentication.TokenDescriptor(builder, user);

        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = jwtTokenHandler.WriteToken(token);

        return Results.Ok(jwtToken);
    }

    return Results.Unauthorized();
});

//delete employee by id
app.MapDelete("/api/deleteEmployee/{Id}", [Authorize] async (int Id, EmployeeContext context) => 
{
    if (await context.Employee.FindAsync(Id) is Employee employee)
    {
        context.Employee.Remove(employee);
        await context.SaveChangesAsync();
        return Results.NoContent();
    }   
    return Results.NotFound();
});

#endregion

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

//exception handling
app.UseStatusCodePages(async statusCodeContext
    => await Results.Problem(statusCode: statusCodeContext.HttpContext.Response.StatusCode)
                 .ExecuteAsync(statusCodeContext.HttpContext));

app.Run();