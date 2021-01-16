using System.ComponentModel.DataAnnotations.Schema;

namespace QueryNinja.Examples.AspNetCore.DbContext.Entities
{
    public class Grade
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }

        public Student Student { get; set; }

        [ForeignKey(nameof(Exam))]
        public int ExamId { get; set; }

        public Exam Exam { get; set; }

        public Mark Mark { get; set; }
    }
}