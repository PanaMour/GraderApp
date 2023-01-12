using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GraderApp.Models;

[Table("students")]
public partial class Student
{
    [Key]
    public int RegistrationNumber { get; set; }

    [StringLength(45)]
    [Unicode(false)]
    public string? Name { get; set; }

    [StringLength(45)]
    [Unicode(false)]
    public string? Surname { get; set; }

    [StringLength(45)]
    [Unicode(false)]
    public string? Department { get; set; }

    [Column("USERS_username")]
    [StringLength(45)]
    [Unicode(false)]
    public string UsersUsername { get; set; } = null!;

    [InverseProperty("StudentsRegistrationNumberNavigation")]
    public virtual ICollection<CourseHasStudent> CourseHasStudents { get; } = new List<CourseHasStudent>();

    [ForeignKey("UsersUsername")]
    [InverseProperty("Students")]
    public virtual User UsersUsernameNavigation { get; set; } = null!;
}
