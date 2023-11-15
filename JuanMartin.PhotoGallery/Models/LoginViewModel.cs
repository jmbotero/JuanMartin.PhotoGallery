// Decompiled with JetBrainys decompiler
// Type: JuanMartin.PhotoGallery.Models.LoginViewModel
// Assembly: JuanMartin.PhotoGallery, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4CB99D68-5881-4C4E-957B-0BF063CB63CD
// Assembly location: C:\GitHub\temp\JuanMartin.PhotoGallery.dll

using System.ComponentModel.DataAnnotations;

namespace JuanMartin.PhotoGallery.Models
{
    public class LoginViewModel
    {
        public int UserId { get; private set; }

        [Required(ErrorMessage = "Please Provide Username", AllowEmptyStrings = false)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please provide password", AllowEmptyStrings = false)]
        public string Password { get; set; }

        [RegularExpression("\\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\\Z", ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        public LoginViewModel()
        {
            UserId = 0;
            UserName = string.Empty;
            Password = string.Empty;
            Email = string.Empty;
        }
    }
}
    