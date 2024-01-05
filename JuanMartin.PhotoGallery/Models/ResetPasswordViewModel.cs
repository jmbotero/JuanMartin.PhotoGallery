// Decompiled with JetBrains decompiler
// Type: JuanMartin.PhotoGallery.Models.ResetPasswordViewModel
// Assembly: JuanMartin.PhotoGallery, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4CB99D68-5881-4C4E-957B-0BF063CB63CD
// Assembly location: C:\GitHub\temp\JuanMartin.PhotoGallery.dll

using System.ComponentModel.DataAnnotations;

namespace JuanMartin.PhotoGallery.Models
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "New password required", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "New password and confirm password do not match")]
        public string ConfirmPassword { get; set; }

        public string ResetCode { get; set; }
        public string ResetMessage { get; set; }
       public int UserId { get; set; }
        public string UserName { get; set; }

        public ResetPasswordViewModel()
        {
            UserId = 0;
            UserName = string.Empty;
            ResetCode = string.Empty;
            ResetMessage = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
        }
    }
}
