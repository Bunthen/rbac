using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace cmedcc_idass.backend.Models;

[Table("UserRoles")] // Specifies the table name if it differs from the class name
public class UserRole
{

    [Key] // Denotes this property as the primary key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Indicates that the database generates this value (e.g., newid())
    public Guid UserRoleId { get; set; }

    [Required] // Ensures that this field cannot be null
    public Guid UserId { get; set; }

    [Required] // Ensures that this field cannot be null
    public Guid RoleId { get; set; }
}