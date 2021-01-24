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
    /// <summary>
    /// This controller allow to test QueryNinja. <br/>
    /// 
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly UniversityDbContext dbContext;

        /// <summary>
        /// Creates instance of a controller with dbContext.
        /// </summary>
        /// <param name="dbContext"></param>
        public StudentsController(UniversityDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Allows to get list of students with QueryNinja query.
        /// </summary>
        /// <param name="query">QueryNinja query. Send empty to see all students.</param>
        /// <returns>List of students</returns>
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
