using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Entities.Account;

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [NotNull]
    public string? Email { get; set; }

    public string? HashedPassword { get; set; }

    public string? Salt { get; set; }
}
