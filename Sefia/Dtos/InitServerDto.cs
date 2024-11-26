using Microsoft.AspNetCore.Mvc;
using Sefia.Attributes;
using Sefia.Common;
using System.ComponentModel.DataAnnotations;

namespace Sefia.Dtos;

public class InitServerDto
{
    public RegisterDto Admin { get; set; }

    public required string ApplicationDomain { get; set; }
    public required string ServingDomain { get; set; }
    public required string WebRoot { get; set; } 
}
