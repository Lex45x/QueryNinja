using System.Collections.Generic;

namespace QueryNinja.Examples.AspNetCore.DbContext.Entities
{
    /// <summary>
    /// Represent an exam.
    /// </summary>
    public class Exam
    {
        /// <summary>
        /// Id of Exam
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Exam title
        /// </summary>
        public string Title { get; set; }

        // ReSharper disable once UnusedMember.Global
        /// <summary>
        /// All grades relate to this exam.
        /// </summary>
        public ICollection<Grade> Grades { get; set; }
    }
}