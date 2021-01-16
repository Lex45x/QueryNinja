using System.Collections.Generic;

namespace QueryNinja.Examples.AspNetCore.DbContext.Entities
{
    public class Exam
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<Grade> Grades { get; set; }
    }
}