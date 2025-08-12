// // YourIdaas.Core/Models/Role.cs
// using System;
// using System.Collections.Generic;
// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;

// namespace cmedcc_idass.backend.Models;
// [Table("Roles")]
// public class Role
// {
//     [Key]
//     public Guid Id { get; set; }
//     public string Name { get; set; } = string.Empty;

//     // Navigation property for users
//     public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
// }
