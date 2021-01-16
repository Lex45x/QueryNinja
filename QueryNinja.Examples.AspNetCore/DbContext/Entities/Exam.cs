using System.Collections.Generic;

namespace QueryNinja.Examples.AspNetCore.DbContext.Entities
{
    public class Exam
    {
        public int Id { get; set; }
        public string Title { get; set; }
        // ReSharper disable once UnusedMember.Global
        public ICollection<Grade> Grades { get; set; }
    }
}