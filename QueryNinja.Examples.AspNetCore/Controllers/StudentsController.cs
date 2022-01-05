using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QueryNinja.Core;
using QueryNinja.Examples.AspNetCore.DbContext;
using QueryNinja.Targets.Queryable;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QueryNinja.Examples.AspNetCore.DbContext.Entities;

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
        [HttpGet("list")]
        public async Task<IEnumerable<dynamic>> GetStudents([FromQuery] IDynamicQuery query)
        {
            var result = await dbContext.Students
                .AsNoTracking()
                .Include(student => student.Grades)
                .ThenInclude(grade => grade.Exam)
                .WithQuery(query)
                .ToListAsync();

            return result;
        }

        /// <summary>
        /// Allows to get a report on different grades count across exams.
        /// </summary>
        /// <param name="denormalizedQuery">QueryNinja query. Will filter all students before aggregation. Send empty to see all students.</param>
        /// <param name="aggregatedQuery">QueryNinja query. Will filter/select data after aggregation. Send empty to see all students.</param>
        /// <returns>List of students</returns>
        [HttpGet("report")]
        public async Task<IEnumerable<dynamic>> GetStudentsReport([FromQuery] IQuery denormalizedQuery, [FromQuery] IDynamicQuery aggregatedQuery)
        {
            var result = await dbContext.Grades
                .AsNoTracking()
                .WithQuery(denormalizedQuery)
                .GroupBy(grade => grade.Exam.Title)
                .Select(grades => new
                {
                    grades.Key,
                    GradesCountA = grades.Count(grade => grade.Mark == Mark.A),
                    GradesCountB = grades.Count(grade => grade.Mark == Mark.B),
                    GradesCountOther = grades.Count(grade => grade.Mark < Mark.B)
                })
                .WithQuery(aggregatedQuery)
                .ToListAsync();

            return result;
        }
    }
}
