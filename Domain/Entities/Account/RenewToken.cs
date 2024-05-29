using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Entities.Account;

public class RenewToken
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [NotNull]
    public Guid? UserId { get; set; }

    [NotNull]
    public string? Token { get; set; }
}
