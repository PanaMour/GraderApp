using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GraderApp.Models;

[Table("users")]
public partial class User
{
    [Key]
    [Column("username")]
    [StringLength(45)]
    [Unicode(false)]
    public string Username { get; set; } = null!;

    [Column("password")]
    [StringLength(100)]
    [Unicode(false)]
    public string? Password { get; set; }

    [Column("role")]
    [StringLength(45)]
    [Unicode(false)]
    public string? Role { get; set; }

    [InverseProperty("UsersUsernameNavigation")]
    public virtual ICollection<Professor> Professors { get; } = new List<Professor>();

    [InverseProperty("UsersUsernameNavigation")]
    public virtual ICollection<Secretary> Secretaries { get; } = new List<Secretary>();

    [InverseProperty("UsersUsernameNavigation")]
    public virtual ICollection<Student> Students { get; } = new List<Student>();
}
