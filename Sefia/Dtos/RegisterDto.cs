using Microsoft.AspNetCore.Mvc;
using Sefia.Attributes;
using Sefia.Common;
using System.ComponentModel.DataAnnotations;

namespace Sefia.Dtos;

public class RegisterDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [ModelBinder(BinderType = typeof(TrimModelBinder))]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    [PasswordValidation(ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one number.")]
    [ModelBinder(BinderType = typeof(TrimModelBinder))]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(10, ErrorMessage = "Name cannot exceed 20 characters.")]
    [ModelBinder(BinderType = typeof(TrimModelBinder))]
    public string Name { get; set; } = string.Empty;
}
