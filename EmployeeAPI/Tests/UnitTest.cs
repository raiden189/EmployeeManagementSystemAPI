using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using Tests.Model;

namespace Tests
{
    public class UnitTest
    {
        [Fact]
        public async Task Get_All_Employees()
        {
            await using var context = new ConfigureDbContext();
            using var client = context.CreateClient();


            var result = await client.GetStringAsync("/api/getEmployees");
                
            Assert.NotNull(result);
        }

        [Fact]
        public async Task All_Employees_Should_Return_Ok()
        {
            await using var context = new ConfigureDbContext();
            using var client = context.CreateClient();

            using var result = await client.GetAsync("/api/getEmployees");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task Employee_Search_By_Id_Should_Return_Not_Found()
        {
            await using var context = new ConfigureDbContext();
            using var client = context.CreateClient();

            int id = 1;
            using var result = await client.GetAsync($"/api/getEmployeeById/{id}");

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task Add_Employee_Should_Return_Ok()
        {
            await using var context = new ConfigureDbContext();
            using var client = context.CreateClient();

            var employee = new Employee { Name = "John Edwards", Department = "IT Department", Position = "Manager", Salary = 900000 };
            using var jsonContent = new StringContent(JsonConvert.SerializeObject(employee));
            jsonContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            using var result = await client.PostAsync("/api/addEmployee", jsonContent);

            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        }

        [Fact]
        public async Task Add_Employee_Should_Return_Bad_Request()
        {
            await using var context = new ConfigureDbContext();
            using var client = context.CreateClient();

            var employee = "";
            using var jsonContent = new StringContent(JsonConvert.SerializeObject(employee));
            jsonContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            using var result = await client.PostAsync("/api/addEmployee", jsonContent);

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Update_Employee_Should_Return_Ok()
        {
            await using var context = new ConfigureDbContext();
            using var client = context.CreateClient();

            var employee = new Employee { Name = "John Edwards", Department = "IT Department", Position = "Manager", Salary = 900000 };
            using var jsonContent = new StringContent(JsonConvert.SerializeObject(employee));
            jsonContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            using var result = await client.PostAsync("/api/addEmployee", jsonContent);

            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        }

        [Fact]
        public async Task Delete_Employee_Should_Return_Not_Found()
        {
            await using var context = new ConfigureDbContext();
            using var client = context.CreateClient();

            int id = 1;
            using var result = await client.DeleteAsync($"/api/deleteEmployee/{id}");

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task Delete_Employee_Should_Return_Success()
        {
            await using var context = new ConfigureDbContext();
            using var client = context.CreateClient();
            
            var employee = new Employee { Name = "John Edwards", Department = "IT Department", Position = "Manager", Salary = 900000 };
            using var jsonContent = new StringContent(JsonConvert.SerializeObject(employee));
            jsonContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            using var response = await client.PostAsync("/api/addEmployee", jsonContent);

            int id = 1;
            using var result = await client.DeleteAsync($"/api/deleteEmployee/{id}");

            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        }
    }
}