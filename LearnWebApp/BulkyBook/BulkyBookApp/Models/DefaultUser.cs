﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkyBookApp.Models
{
    public class DefaultUser : IdentityUser
    {
        [PersonalData]
        [Column(TypeName ="nvarchar(100)")]
        public string? FirstName { get; set; } 
        
        [PersonalData]
        [Column(TypeName ="nvarchar(100)")]
        public string? LastName { get; set; }
    }
}
