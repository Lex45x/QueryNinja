using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QueryNinja.Core;
using QueryNinja.Examples.AspNetCore.DbContext;
using QueryNinja.Examples.AspNetCore.DbContext.Entities;
using QueryNinja.Targets.Queryable;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueryNinja.Examples.AspNetCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly UniversityDbContext dbContext;

        public StudentsController(UniversityDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IEnumerable<Student>> Get([FromQuery] IQuery query)
        {
            var result = await dbContext.Students
                .AsNoTracking()
                .Include(student => student.Grades)
                .ThenInclude(grade => grade.Exam)
                .WithQuery(query)
                .ToListAsync();

            return result;
        }
    }
}
