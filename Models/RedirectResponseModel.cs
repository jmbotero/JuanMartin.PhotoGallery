// Decompiled with JetBrains decompiler
// Type: JuanMartin.PhotoGallery.Models.RedirectResponseModel
// Assembly: JuanMartin.PhotoGallery, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4CB99D68-5881-4C4E-957B-0BF063CB63CD
// Assembly location: C:\GitHub\temp\JuanMartin.PhotoGallery.dll

using System.Collections.Generic;

namespace JuanMartin.PhotoGallery.Models
{
    public class RedirectResponseModel
    {
        public string RemoteHost { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public Dictionary<string, object> RouteData { get; set; }
    }
}
