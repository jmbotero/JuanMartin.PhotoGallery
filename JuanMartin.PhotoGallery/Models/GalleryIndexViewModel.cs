// Decompiled with JetBrains decompiler
// Type: JuanMartin.PhotoGallery.Models.GalleryIndexViewModel
// Assembly: JuanMartin.PhotoGallery, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4CB99D68-5881-4C4E-957B-0BF063CB63CD
// Assembly location: C:\GitHub\temp\JuanMartin.PhotoGallery.dll

using JuanMartin.Models.Gallery;
using System.Collections.Generic;

namespace JuanMartin.PhotoGallery.Models
{
    public class GalleryIndexViewModel
    {
        //        public const int UserID = 1;

        public List<Photography>? Album { get; set; }
        public long PhotographyCount { get; set; }
        public int PageId { get; set; }
        public int BlockId { get; set; }
        public List<string>? Tags { get; set; }
        public List<string>? Locations { get; set; }
        public string ShoppingCartAction { get; set; }
        public string CartItemsSequence { get; set; }
        public string DropImageIndex { get; set; }
        public string DragImageIndex { get; set; }

        public GalleryIndexViewModel()
        {
            Album = null;
            PhotographyCount = 0;
            Locations = null;
            Tags = null;
            PageId = 1;
            BlockId = 1;

            ShoppingCartAction = string.Empty;
            CartItemsSequence = string.Empty;
            DragImageIndex = string.Empty;
            DropImageIndex = string.Empty;
        }
    }
}
