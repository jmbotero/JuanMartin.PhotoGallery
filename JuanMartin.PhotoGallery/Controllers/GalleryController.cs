                            // Decompiled with JetBrains decompiler
// Type: JuanMartin.PhotoGallery.Controllers.GalleryController
// Assembly: JuanMartin.PhotoGallery, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4CB99D68-5881-4C4E-957B-0BF063CB63CD
// Assembly location: C:\GitHub\temp\JuanMartin.PhotoGallery.dll

using JuanMartin.Models.Gallery;
using JuanMartin.PhotoGallery.Models;
using JuanMartin.PhotoGallery.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace JuanMartin.PhotoGallery.Controllers
{
    public class GalleryController : Controller
    {
        private readonly IPhotoService _photoService;
        private readonly IConfiguration _configuration;
        private readonly bool _guestModeEnabled;
        private const string NoActionSelected = "none";

        private enum SelectedItemAction
        {
            none,
            update,
            cancel,
        }

        public GalleryController(IPhotoService photoService, IConfiguration configuration)
        {
            this._photoService = photoService;
            this._configuration = configuration;
            this._guestModeEnabled = Convert.ToBoolean(configuration["GuestModeEnabled"]);
        }
  }
}
