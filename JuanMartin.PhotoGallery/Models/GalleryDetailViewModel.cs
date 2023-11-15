// Decompiled with JetBrains decompiler
// Type: JuanMartin.PhotoGallery.Models.GalleryDetailViewModel
// Assembly: JuanMartin.PhotoGallery, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4CB99D68-5881-4C4E-957B-0BF063CB63CD
// Assembly location: C:\GitHub\temp\JuanMartin.PhotoGallery.dll

using JuanMartin.Models.Gallery;

namespace JuanMartin.PhotoGallery.Models
{
    public class GalleryDetailViewModel
    {
        public string SearchQuery { get; set; }
        public int PageId { get; set; }
        public string Location { get; set; }
        public int SelectedRank { get; set; }
        public Photography? Image { get; set; }
        public string ImageIdList { get; set; }
        public string Tag { get; set; }
        public string SelectedTagListAction { get; set; }
        public string ShoppingCartAction { get; set; }

        public GalleryDetailViewModel()
        {
            PageId = 0;
            SearchQuery = string.Empty;
            SelectedRank = 0;
            Location = string.Empty;
            Tag = string.Empty;
            SelectedTagListAction = string.Empty;
            Image = null;
            ImageIdList = string.Empty;
            ShoppingCartAction = string.Empty;
        }
    }
}
