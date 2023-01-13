using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GraderApp.Models;

[Table("course")]
public partial class Course
{
    [Key]
    [Column("idCOURSE")]
    public int IdCourse { get; set; }

    [StringLength(60)]
    [Unicode(false)]
    public string? CourseTitle { get; set; }

    [StringLength(25)]
    [Unicode(false)]
    public string? CourseSemester { get; set; }

    [Column("PROFESSORS_AFM")]
    public int ProfessorsAfm { get; set; }

    [InverseProperty("CourseIdCourseNavigation")]
    public virtual ICollection<CourseHasStudent> CourseHasStudents { get; } = new List<CourseHasStudent>();

    [ForeignKey("ProfessorsAfm")]
    [InverseProperty("Courses")]
    public virtual Professor ProfessorsAfmNavigation { get; set; } = null!;
}
