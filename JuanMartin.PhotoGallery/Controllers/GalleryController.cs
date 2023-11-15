// Decompiled with JetBrains decompiler
// Type: JuanMartin.PhotoGallery.Controllers.GalleryController
// Assembly: JuanMartin.PhotoGallery, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4CB99D68-5881-4C4E-957B-0BF063CB63CD
// Assembly location: C:\GitHub\temp\JuanMartin.PhotoGallery.dll

using JuanMartin.Kernel.Extesions;
using JuanMartin.Models.Gallery;
using JuanMartin.PhotoGallery.Models;
using JuanMartin.PhotoGallery.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace JuanMartin.PhotoGallery.Controllers
{
    public class GalleryController : Controller
    {
        private readonly IPhotoService _photoService;
        private readonly IConfiguration _configuration;
        private readonly bool _guestModeEnabled;
        private readonly string noAction = "none";
        private enum SelectedItemAction
        {
            [Description("none")]
            none,
            [Description("update")]
            update,
            [Description("cancel")]
            cancel,
        }

        public GalleryController(IPhotoService photoService, IConfiguration configuration)
        {
            _photoService = photoService;
            _configuration = configuration;
            _guestModeEnabled = Convert.ToBoolean(configuration["GuestModeEnabled"]);
        }


        [HttpGet]
        public IActionResult Index(
                                                  string searchQuery,
                                                  int pageId = 1,
                                                  int blockId = 1,
                                                  bool cartView = false,
                                                  string orderAction = "none")
        {
            if (HttpContext.Session == null)
                throw new ApplicationException("Critical error: session object is null.");

            string str = "";
            int sessionUserId = GetCurrentUserId();
            GalleryIndexViewModel model = new GalleryIndexViewModel();
            RedirectResponseModel redirectInfo;

            if (string.IsNullOrEmpty(searchQuery))
                searchQuery = HttpContext.Request.Query[nameof(searchQuery)].ToString();

            Order order = SetLayoutViewHeaderInformation(sessionUserId, cartView: cartView).Order;
            SetViewRedirectInfo("Gallery", nameof(Index), out sessionUserId, out redirectInfo);
            var isSignedIn = Convert.ToBoolean(SessionExtensions.Get<ISession>(HttpContext.Session, "isSignedIn"));
            if (!_guestModeEnabled && !isSignedIn)
                return RedirectToAction("Login", "Login", redirectInfo?.RouteData);
            if (orderAction == GalleryController.SelectedItemAction.cancel.ToString())
                str = RemoveCurrentOrder(sessionUserId);

            if (string.IsNullOrEmpty(searchQuery))
            {
                model.Album = (List<Photography>)_photoService.GetAllPhotographies(sessionUserId, pageId);
                int OrderId = cartView || !string.IsNullOrEmpty(searchQuery) ? order.OrderId : -1;
                GetPhotographyIdsList(sessionUserId, _photoService, searchQuery, OrderId);

                ViewBag.CurrentPage = pageId;
                ViewBag.BlockId = blockId;
                ViewBag.BlockSize = _photoService.BlockSize;
            }
            else
                model = ProcessSearchQuery(pageId, blockId, searchQuery, model, sessionUserId);

            model.Tags = _photoService.GetAllTags(pageId).OrderBy(x => x).ToList();
            model.Locations = _photoService.GetAllLocations(pageId).OrderBy(x => x).ToList();
                
            ViewBag.Message = str;
            TempData["isSignedIn"] = SessionExtensions.Get<ISession>(HttpContext.Session, "isSignedIn");
            return View(model);
        }


        [HttpPost]
        public IActionResult Index(
                                                  int pageId,
                                                  int blockId,
                                                  string searchQuery,
                                                  GalleryIndexViewModel model,
                                                  bool cartView = false)
        {
            string str = "";
            (int userId, Order order) = SetLayoutViewHeaderInformation(cartView: cartView);
            if (string.IsNullOrEmpty(searchQuery) && !cartView)
                return RedirectViewGalleryIndex(userId);
            if (cartView)
            {
                if (model.ShoppingCartAction == GalleryController.SelectedItemAction.update.ToString() && !_photoService.UpdateOrderItemsIndices(userId, order.OrderId, model))
                    str = "No re-ordering detected.";
                model = DisplayPhotographyOrders(order, pageId, blockId, model, userId);
            }
            else
            {
                model = ProcessSearchQuery(pageId, blockId, searchQuery, model, userId);
                int useerId = userId;
                str = $"Search for ({searchQuery}) returned '{model.PhotographyCount}' results.";
                _photoService.AddAuditMessage(useerId, str);
            }
            model.Tags = _photoService.GetAllTags(pageId).OrderBy(x => x).ToList();
            model.Locations = _photoService.GetAllLocations(pageId).OrderBy(x => x).ToList();

            ViewBag.Message = str;
            TempData["isSignedIn"] = SessionExtensions.Get<ISession>(HttpContext.Session, "isSignedIn");

            return View(model);
        }


        [HttpGet]
        public IActionResult Detail(
                                                    long id,
                                                    int pageId,
                                                    int blockId,
                                                    string searchQuery,
                                                    bool cartView = false)
        {
            SetViewRedirectInfo("Gallery", nameof(Detail), out int _, out RedirectResponseModel _, id);
            TempData["isSignedIn"] = SessionExtensions.Get<ISession>(HttpContext.Session, "isSignedIn");
            int userId = Convert.ToInt32(SessionExtensions.Get<ISession>(HttpContext.Session, "userId"));
            SetLayoutViewHeaderInformation(userId, photographyId: id, cartView);
            ViewBag.BockId = blockId; 
            ViewBag.PageId = pageId; 
            ViewBag.SearchQuery = searchQuery;
            ViewBag.GalleryIdsList = Convert.ToString(SessionExtensions.Get<ISession>(HttpContext.Session, "galleryIdList"));
            return View(PrepareDetailViewModel(id, searchQuery, pageId));
        }

        [HttpPost]
        public IActionResult Detail(
                                                  long id,
                                                  int pageId,
                                                  int blockId,
                                                  string searchQuery,
                                                  GalleryDetailViewModel model,
                                                  bool cartView = false,
                                                  string orderAction = "none")
        {
            string message = "";
            int sessionUserId;
            RedirectResponseModel redirectInfo;
                var isSignedIn = Convert.ToBoolean(SessionExtensions.Get<ISession>(HttpContext.Session, "isSignedIn"));
                SetViewRedirectInfo("Gallery", nameof(Detail), out sessionUserId, out redirectInfo, id);
            if (isSignedIn)
                return RedirectToAction("Login", "Login", redirectInfo?.RouteData);
            if (orderAction == GalleryController.SelectedItemAction.cancel.ToString())
                message = RemoveCurrentOrder(sessionUserId);
            if (model != null && !string.IsNullOrEmpty(model.Location))
                _photoService.UpdatePhotographyLocation(id, sessionUserId, model.Location);
            if (model != null && !string.IsNullOrEmpty(model.Tag))
                message = ProcessSubmittedTag(id, model, message, sessionUserId);
            if (model != null && model.SelectedRank != 0)
                message = ProcessSubmittedRank(id, model, message, sessionUserId);
            if (model != null && model.ShoppingCartAction != EnumExtensions.GetDescription(SelectedItemAction.none)) 
                message = ProcessShoppingCartAction(id, model, message, sessionUserId);
            int userId = sessionUserId;
            long photographyId = id;
            SetLayoutViewHeaderInformation(userId, photographyId, cartView);

            TempData["isSignedIn"] = isSignedIn;
            ViewBag.BockId = blockId;
            ViewBag.PageId = pageId;
            ViewBag.SearchQuery = searchQuery;
            ViewBag.Message = message;
            ViewBag.GalleryIdsList = Convert.ToString(SessionExtensions.Get<ISession>(HttpContext.Session, "galleryIdList"));
            ViewBag.IsPhotographyInOrder = IsPhotographyInOrder(id, sessionUserId);
            return View(PrepareDetailViewModel(id, searchQuery, pageId));
        }
                                                                                        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            int useerId = -1;
            bool isSignedIn = Convert.ToBoolean(SessionExtensions.Get<ISession>(HttpContext.Session, "isSignedIn"));

            if (isSignedIn)
                useerId = Convert.ToInt32(SessionExtensions.Get<ISession>(HttpContext.Session, "userId"));
            IExceptionHandlerPathFeature? handlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (handlerPathFeature != null)
            {
                string message = handlerPathFeature.Error.Message;
                string path = handlerPathFeature.Path;
                _photoService.AddAuditMessage(useerId, message, path, 1);

                ViewBag.ErrorMessage = $"Path {path} threw an exception: '{message}.";
            }
            return View(new ErrorViewModel()
            {
                RequestId = (Activity.Current?.Id ?? HttpContext.TraceIdentifier)
            });
        }


        //support methods
        private string RemoveCurrentOrder(int userId)
        {
            string str = "";
            Order currentActiveOrder = GetCurrentActiveOrder(userId);
            string orderNumber = currentActiveOrder.Number.ToString();
            int count = currentActiveOrder.Count;
            if (_photoService.RemoveOrder(currentActiveOrder.OrderId, userId) > 0)
            {
                SessionExtensions.Set<ISession, int>(HttpContext.Session, "orderId", -1);
                str = $"Order '{orderNumber}' with [{count}] photographs, has been removed.";
            }
            return str;
        }

        private (int UserId, Order Order) SetLayoutViewHeaderInformation(
          int userId = -1,
          long photographyId = -1,
          bool cartView = false)
        {
            SessionExtensions.Set<ISession, bool>(HttpContext.Session, "IsMobile", HttpUtility.IsMobileDevice(HttpContext).IsMobile);
            int id = userId == -1 ? GetCurrentUserId() : userId;
            int orderId = GetCurrentSessionOrderId();
            Order order;
            if (orderId == -1)
            {
                order = GetCurrentActiveOrder(id);
                orderId = order.OrderId;
            }
            else
                order = _photoService.GetOrder(id, orderId);

            SessionExtensions.Set<ISession, int>(HttpContext.Session, "orderId", orderId);

            ViewBag.IsPhotographyInOrder = IsPhotographyInOrder(photographyId, id);
            ViewBag.HasCurrentActiveOrder = orderId != -1;
            ViewBag.PhotographyCount = order != null ? order.Count : 0;
            ViewBag.DisplayPhotogrphiesAsOrder = cartView;
            ViewBag.CartRedirectUrl = GetRedirectUrl(HttpUtility.GalleryViewTypes.Index, id);

            return (id, order);
        }

        private Guid GetCurrentSessionId()
        {
            Guid currentSessionId = Guid.Empty; 
            bool isSignedIn = Convert.ToBoolean(SessionExtensions.Get<ISession>(HttpContext.Session, "isSignedIn" ));

            if (isSignedIn)
            {
                try
                {
                    string? sessionId = Convert.ToString(SessionExtensions.Get<ISession>(HttpContext.Session, "sessionId"));

                    Guid.TryParse(sessionId, out currentSessionId);
                }
                catch
                {
                    SessionExtensions.Set<ISession, bool>(HttpContext.Session, "isSignedIn", false);
                    currentSessionId = Guid.Empty;
                }
            }
            return currentSessionId;
        }

        private int GetCurrentUserId()
        {
            int currentUserId = -1;
            bool isSignedIn = Convert.ToBoolean(SessionExtensions.Get<ISession>(HttpContext.Session, "isSignedIn"));

            if (isSignedIn)
            {
                try
                {
                    currentUserId = Convert.ToInt32(SessionExtensions.Get<ISession>(HttpContext.Session, "userId"));
                }
                catch
                {
                    currentUserId = -1;
                }
            }
            return currentUserId;
        }

        private int GetCurrentSessionOrderId()
        {
            int currentSessionOrderId = -1;
            bool isSignedIn = Convert.ToBoolean(SessionExtensions.Get<ISession>(HttpContext.Session, "isSignedIn"));

            if (isSignedIn)
            {
                try
                {
                    currentSessionOrderId = Convert.ToInt32(SessionExtensions.Get<ISession>(HttpContext.Session, "orderId"));
                }
                catch
                {
                    currentSessionOrderId = -1;
                }
            }
            return currentSessionOrderId;
        }

        private bool IsPhotographyInOrder(long id, int userId)
        {
            if (id == -1 || userId == -1)
                return false;

            Order currentActiveOrder = _photoService.GetCurrentActiveOrder(userId);
            return _photoService.IsPhotographyInOrder(currentActiveOrder != null ? currentActiveOrder.OrderId : -1, id, userId);
        }

        private GalleryIndexViewModel ProcessSearchQuery(
                                                                                            int pageId,
                                                                                            int blockId,
                                                                                            string searchQuery,
                                                                                            GalleryIndexViewModel model,
                                                                                            int sessionUserId)
        {
            searchQuery = searchQuery.Replace(',', '|');
            model.Album = (List<Photography>)_photoService.GetPhotographiesBySearch(sessionUserId, searchQuery, pageId);
            int orderId = -1;
            var photographyCount = GetPhotographyIdsList(sessionUserId, _photoService, searchQuery, orderId).Count;
            searchQuery = searchQuery.Replace("|", ",");
           ViewBag.InfoMessage = $">> {photographyCount} images  were found with tags: ({searchQuery}).";
            ViewBag.CurrentPage = pageId;
            ViewBag.SearchQuery = searchQuery;
            ViewBag.BlockSize = _photoService.BlockSize;
             model.PhotographyCount = photographyCount;
            model.BlockId = blockId;
            model.PageId = pageId;

            return model;
        }

        private GalleryIndexViewModel DisplayPhotographyOrders(
                                                                                                    Order order,
                                                                                                    int pageId,
                                                                                                    int blockId,
                                                                                                    GalleryIndexViewModel model,
                                                                                                    int sessionUserId)
        {
            model.Album = (List<Photography>)_photoService.GetOrderPhotographies(sessionUserId, order.OrderId, pageId);
            var photograhyCount = GetPhotographyIdsList(sessionUserId, _photoService, "", order.OrderId).Count;
            
            string str = photograhyCount == 1 ? "photography" : "photographies";
            str = $">> ({photograhyCount}) {str} in<br/>Order # {order.Number}<br/>created on {order.CreatedDtm}";
            ViewBag.InfoMessage = str;
            model.CartItemsSequence = "";
            model.ShoppingCartAction = EnumExtensions.GetDescription(GalleryController.SelectedItemAction.none);
            model.PhotographyCount = photograhyCount;
            model.BlockId = blockId;
            model.PageId = pageId;
 
            return model;
        }

        private string ProcessSubmittedRank(
                                                                  long id,
                                                                  GalleryDetailViewModel model,
                                                                  string message,
                                                                  int sessionUserId)
        {
            int selectedRank = model.SelectedRank;
            if (_photoService.UpdatePhotographyRanking(id, sessionUserId, selectedRank) == -1)
            {
                message = $"Error inserting rank={selectedRank} for image [{id}] in database.";
            }
            return message;
        }

        private string ProcessSubmittedTag(
                                                                    long id,
                                                                    GalleryDetailViewModel model,
                                                                    string message,
                                                                    int sessionUserId)
        {
            string tag = model.Tag.Trim();
            switch (model.SelectedTagListAction)
            {
                case "add":
                    switch (_photoService.AddTag(sessionUserId, tag, id))
                    {
                        case -2:
                            message = $"Tag '{tag}' is already associated to this image ({id}).";
                            break;
                        case -1:
                            message = $"Could not insert tag '{tag}' for image ({id}), image may have been deleted.";
                            break;
                        default:
                            model.Image = _photoService.GetPhotographyById(id, sessionUserId);
                            model.Image.Tags.Add(tag);
                            break;
                    }
                    break;
                case "remove":
                    if (_photoService.RemoveTag(sessionUserId, tag, id) != -1)
                    {
                        if(model.Image == null) 
                        {
                            message = $"Error loading  image ({id}) from model.";
                            break;
                        }
                        model.Image.Tags.Remove(tag);
                        break;
                    }
                    message = $"Error deleting tag '{tag}' for image ({id}) in database.";
                    break;
            }

            return message;
        }

        private string ProcessShoppingCartAction(
                                                                              long id,
                                                                              GalleryDetailViewModel model,
                                                                              string message,
                                                                              int sessionUserId)
        {
            Order order = GetCurrentActiveOrder(sessionUserId);
            switch (model.ShoppingCartAction)
            {
                case "add":
                    string str;
                    if (order.OrderId == -1)
                    {
                        order = _photoService.AddOrder(sessionUserId);
                        if (order.OrderId == -2)
                        {
                            message = "Cannot create a new order! ...</br>there is already a current, active order, if you cannnot</br>cancel it first  then contact the administrator.";
                            return message;
                        }
                        str = "new";
                    }
                    else
                        str = "existing";

                    _photoService.AddPhotographyToOrder(id, order.OrderId, sessionUserId);
                    message = $"Photography ({id}) has been added to {str} order #{order.Number} created on {order.CreatedDtm}.";
                    break;
                case "remove":
                    _photoService.RemovePhotographyFromOrder(id, order.OrderId, sessionUserId);
                    message = $"This photography ({id} has been removed from order #{order.Number}.";
                    break;
            }

            return message;
        }

        private void SetViewRedirectInfo(
                                                              string controlerName,
                                                              string actionName,
                                                              out int sessionUserId,
                                                              out RedirectResponseModel redirectInfo,
                                                              long routeId = -1)
        {
            if (HttpContext != null)
            {
                var context = HttpContext;
                var request = context.Request;
                sessionUserId = GetCurrentUserId();
                string clientRemoteId = HttpUtility.GetClientRemoteId(HttpContext);
                string queryString = (request != null && request.QueryString != null) ? request.QueryString.Value : "";
                redirectInfo = _photoService.SetRedirectInfo(sessionUserId, clientRemoteId, controlerName, actionName, routeId, queryString);
            }
            else
            {
                sessionUserId = -1;
                redirectInfo = null;
            }
        }

        private Order GetCurrentActiveOrder(int sessionUserId)
        {
            Order currentActiveOrder = _photoService.GetCurrentActiveOrder(sessionUserId) ?? new Order(-1, sessionUserId);
            if (HttpContext != null && HttpContext.Session != null)
                SessionExtensions.Set<ISession, int>(HttpContext.Session, "orderId", currentActiveOrder.OrderId);

            return currentActiveOrder;
        }

        private (string ImageIdsList, long Count) GetPhotographyIdsList(
          int userID,
          IPhotoService photoService,
          string searchQuery,
          int OrderId)
        {
            if (!string.IsNullOrEmpty(searchQuery))
                searchQuery = searchQuery.Replace(',', '|');
            IPhotoService.ImageListSource source = IPhotoService.ImageListSource.gallery;
            if (!string.IsNullOrEmpty(searchQuery))
                source = IPhotoService.ImageListSource.searchResults;
            else if (OrderId != -1)
                source = IPhotoService.ImageListSource.shoppingCart;
            else
                searchQuery = "";
            (string ImageIdsList, long RowCount) = photoService.GetPhotographyIdsList(userID, source, searchQuery, OrderId);
            SessionExtensions.Set<ISession, string>(HttpContext.Session, "galleryIdList", ImageIdsList);
            ViewBag.PageCount = RowCount / (_photoService.PageSize + 1);

            return (ImageIdsList, RowCount);
        }

        private int SetGalleryNavigationIds(long id)
        {
            int num = (int)(id / _photoService.PageSize) + 1;
            if (id % _photoService.PageSize == 0)
                --num;

            return num;
        }

        private GalleryDetailViewModel PrepareDetailViewModel(long id, string searchQuery, int pageId)
        {
            Photography photographyById = _photoService.GetPhotographyById(id, 1);
            return new GalleryDetailViewModel()
            {
                Image = photographyById,
                Location = photographyById == null ? "" : photographyById.Location,
                PageId = pageId,
                SearchQuery = searchQuery,
                SelectedTagListAction = EnumExtensions.GetDescription(SelectedItemAction.none),
                ShoppingCartAction = EnumExtensions.GetDescription(SelectedItemAction.none)
            };
        }

        private static string GetProjectDirectory(ViewDataDictionary debugData = null)
        {
            string projectDirectory = Directory.GetCurrentDirectory();
            if (debugData != null)
                debugData["folder"] = projectDirectory;
            for (int index = 0; index < 2; ++index)
                projectDirectory = projectDirectory.Substring(0, projectDirectory.LastIndexOf("\\"));
            if (debugData != null)
                debugData["path"] = projectDirectory;
            return projectDirectory;
        }

        private ActionResult RedirectViewGalleryIndex(int userId)
        {
            string clientRemoteId = HttpUtility.GetClientRemoteId(HttpContext);
            RedirectResponseModel redirectInfo = _photoService.GetRedirectInfo(userId, clientRemoteId);
            TempData["isSignedIn"] = SessionExtensions.Get<ISession>(HttpContext.Session, "isSignedIn");
            if (redirectInfo == null || !(redirectInfo.RemoteHost != ""))
                return RedirectToAction("Index", "Gallery");
            string controller = redirectInfo.Controller;
            return RedirectToAction(redirectInfo.Action, controller, new RouteValueDictionary((IEnumerable<KeyValuePair<string, object>>)redirectInfo.RouteData));
        }

        private string GetRedirectUrl(HttpUtility.GalleryViewTypes overwriteView, int userId)
        {
            string clientRemoteId = HttpUtility.GetClientRemoteId(HttpContext);
            return HttpUtility.GetRedirectUrl(_photoService.GetRedirectInfo(userId, clientRemoteId), overwriteView);
        }

    }
}
