// Decompiled with JetBrains decompiler
// Type: JuanMartin.PhotoGallery.Services.PhotoService
// Assembly: JuanMartin.PhotoGallery, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4CB99D68-5881-4C4E-957B-0BF063CB63CD
// Assembly location: C:\GitHub\temp\JuanMartin.PhotoGallery.dll

using JuanMartin.Kernel;
using JuanMartin.Kernel.Adapters;
using JuanMartin.Kernel.Extesions;
using JuanMartin.Kernel.Messaging;
using JuanMartin.Kernel.Utilities;
using JuanMartin.Models.Gallery;
using JuanMartin.PhotoGallery.Models;
using System.Collections.Specialized;
using System.Data;
using System.Web;

namespace JuanMartin.PhotoGallery.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IExchangeRequestReply _dbAdapter;
        private const int MaximumFileNameLength = 30;
        public bool IsMobile { get; set; } = false;
        public string ConnectionString { get; set; } = "";
        public int PageSize { get; set; }

        public int BlockSize { get; set; }

        public int MobilePageSize { get; set; }

        public PhotoService(string? connectionString)
        {
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));
            else
            {
                _dbAdapter = (IExchangeRequestReply)new AdapterMySql(connectionString);
            }
        }

        public PhotoService(IConfiguration configuration)
          : this(configuration.GetConnectionString("DefaultConnection"))
        {
            BlockSize = configuration.GetIntegerConfigurationValue("GalleryBlockSize", 10);
            PageSize = configuration.GetIntegerConfigurationValue("GalleryPageSize", 10);
            MobilePageSize = configuration.GetIntegerConfigurationValue("MobileGalleryPageSize", 100);
            ConnectionString = configuration.GetStringConfigurationValue("DefaultConnection", "", "ConnectionStrings");
        }

        private ValueHolder ExecuteSqlStatement(Type source, string statement, string[] returnItems = null, string sender = "Gallery")
        {
            string ParseProcedureName(string s)
            {
                string proc = "";

                if (!string.IsNullOrEmpty(s))
                {
                    int i = s.IndexOf('(');
                    proc = s[..i];
                }

                return proc;
            };

            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            string procedureName = ParseProcedureName(statement);
            if (procedureName == "")
                throw new ArgumentException($"Could not parse procedure name  from'{statement}'.");

            var request = new Message("Command", CommandType.Text.ToString());
            request.AddData((object)new ValueHolder(procedureName, statement));
            request.AddSender(sender, source.ToString());

            _dbAdapter.Send(request);

            if (returnItems != null)
            {
                IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

                if (reply.Data != null)
                {
                    ValueHolder result = new ValueHolder(reply.Data);

                    foreach (var item in returnItems)
                    {
                        var annotations = reply.Data.GetAnnotationByValue(1);
                        if (annotations != null && annotations.GetAnnotation(item) != null)
                            throw new ArgumentException($"Procudure {procedureName} does not have return value '{item}'.");
                    }
                    return result;
                }
            }

            return null;
        }

        public IEnumerable<Photography> GetAllPhotographies(int userId, int pageId = 1)
        {
            int pageSize = IsMobile ? MobilePageSize : PageSize;
            string command = $"uspGetAllPhotographies('{pageId}','{pageSize}','{userId}')";
            var reply = ExecuteSqlStatement(typeof(Photography), command);

            return PhotoService.MapPhotographyListFromDatabaseReplyToEntityModel(userId, reply);
        }

        public int GetGalleryPageCount(int pageSize)
        {
            string retunValue = "pageCount";
            string command = $"uspGetPageCount('{pageSize}')";
            var reply = ExecuteSqlStatement(typeof(Photography), command, new[] { retunValue });
            if (reply != null)
                reply = reply.GetAnnotationByValue(1);

            return (reply != null) ? (int)reply.GetAnnotation(retunValue).Value : -1;
        }

        public (string ImageIdsList, long RowCount) GetPhotographyIdsList(
          int userId,
          IPhotoService.ImageListSource source,
          string searchQuery,
          int OrderId)
        {
            string[] returnItems = new[] { "ids", "rowCount" };
            string command = $"uspGetPhotographyIdsList('{userId}','{source}','{searchQuery}','{OrderId}')";
            var reply = ExecuteSqlStatement(typeof(Photography), command, returnItems);
            if (reply != null)
                reply = reply.GetAnnotationByValue(1);

            return (reply != null) ? ("," + (string)reply.GetAnnotation(returnItems[0]).Value + ",",
                                                        (long)reply.GetAnnotation(returnItems[1]).Value) :
                                              ("", -1);
        }

        public Photography GetPhotographyById(long photographyId, int userId)
        {
            string command = $"uspGetPotography('{photographyId}','{userId}')";
            var reply = ExecuteSqlStatement(typeof(Photography), command);

            List<Photography> photographies = PhotoService.MapPhotographyListFromDatabaseReplyToEntityModel(userId, reply);
            return (photographies != null && photographies.Count > 0) ? photographies[0] : null;
        }

        public int UpdatePhotographyRanking(long photographyId, int userId, int rank)
        {
            if (userId == -1)
                return -1;

            string retunValue = "id";
            string command = $"uspUpdateRanking('{userId}','{photographyId}','{rank}')";
            var reply = ExecuteSqlStatement(typeof(Photography), command, new[] { retunValue });
            if (reply != null)
                reply = reply.GetAnnotationByValue(1);

            return (reply != null) ? (int)reply.GetAnnotation(retunValue).Value : -1;
        }

        public int UpdatePhotographyLocation(
          long photographyId,
          int userId,
          string location)
        {
            if (userId == -1)
                return -1;

            string retunValue = "id";
            string command = $"uspUpdatePhotographyLocation('{userId}','{photographyId}','{location}')";
            var reply = ExecuteSqlStatement(typeof(Photography), command, new[] { retunValue });
            if (reply != null)
                reply = reply.GetAnnotationByValue(1);

            return (reply != null) ? (int)reply.GetAnnotation(retunValue).Value : -1;
        }

        public User GetUser(string userName, string password)
        {
            string[] returnItems = new[] { "id", "email" };
            string command = $"uspGetUser('{userName}','{password}')";
            var reply = ExecuteSqlStatement(typeof(Photography), command, returnItems);
            if (reply != null)
                reply = reply.GetAnnotationByValue(1);

            User user = null;
            if (reply != null && reply.GetAnnotation("Record") != null)
            {
                int id = (int)reply.GetAnnotation(returnItems[0]).Value;

                if (id == -1)
                    return null;

                string email = (string)reply.GetAnnotation(returnItems[1]).Value;
                user = new User()
                {
                    UserId = id,
                    UserName = userName,
                    Password = password,
                    Email = email
                };
            }

            return user;
        }

        public User VerifyEmail(string email)
        {
            string[] returnItems = new[] { "id", "login" };
            string command = $"uspVerifyEmail('{email}')";
            var reply = ExecuteSqlStatement(typeof(User), command, returnItems);
            if (reply != null)
                reply = reply.GetAnnotationByValue(1);

            User user = null;
            if (reply != null && reply.GetAnnotation("Record") != null)
            {
                int id = (int)reply.GetAnnotation(returnItems[0]).Value;

                if (id == -1)
                    return null;

                string userName = (string)reply.GetAnnotation(returnItems[1]).Value;
                user = new User()
                {
                    UserId = id,
                    UserName = userName,
                    Email = email
                };
            }

            return user;
        }

        public Guid LoadSession(int userId)
        {
            string retunValue = "id";
            string command = $"uspAddSession('{userId}')";
            var reply = ExecuteSqlStatement(typeof(ISession), command, new[] { retunValue });
            if (reply != null)
                reply = reply.GetAnnotationByValue(1);
            if (reply != null)
            {
                var guid = Guid.Parse((string)reply.GetAnnotation(retunValue).Value);
                return guid;
            }
            return Guid.Empty;
        }

        public RedirectResponseModel GetRedirectInfo(int userId, string remoteHost)
        {
                if (remoteHost == "")
                    return null;

             string[] returnItems = new[] { "remoteHost", "controller","action", "routeId", "queryString" };
            string command = $"uspGetUserRedirectInfo('{remoteHost}','{userId}')";
            var reply = ExecuteSqlStatement(typeof(HttpContent), command, returnItems);
            if (reply != null)
                reply = reply.GetAnnotationByValue(1);

            RedirectResponseModel redirectInfo = null;
            if (reply != null && reply.GetAnnotation("Record") != null)
            {
                string controller = (string)reply.GetAnnotation(returnItems[1]).Value;
                string action = (string)reply.GetAnnotation(returnItems[2]).Value;
                long routeid = (long)reply.GetAnnotation(returnItems[3]).Value;
                string queryString = (string)reply.GetAnnotation(returnItems[4]).Value;
                Dictionary<string, object> routeValues = GenerateRouteValues( routeid, queryString);
                redirectInfo = new RedirectResponseModel()
                {
                    RemoteHost = remoteHost,
                    Controller = controller,
                    Action = action,
                    RouteData = routeValues
                };
            }

            return redirectInfo;
        }

        public RedirectResponseModel SetRedirectInfo(
          int userId,
          string remoteHost,
          string controller,
          string action,
          long routeId = -1,
          string queryString = "")
        {
            string command = $"uspSetUserRedirectInfo('{userId}','{remoteHost}')";
            var reply = ExecuteSqlStatement(typeof(HttpContent), command) ?? throw new ApplicationException("Error storing redirect information.");
            Dictionary<string, object> routeValues = GenerateRouteValues(routeId, queryString);
            var redirectInfo = new RedirectResponseModel()
            {
                RemoteHost = remoteHost,
                Controller = controller,
                Action = action,
                RouteData = routeValues
            };
        
            return redirectInfo;
        }

        public Dictionary<string, object> GenerateRouteValues(long routeId, string queryString)
        {
            if (string.IsNullOrEmpty(queryString))
                return null;

            NameValueCollection nameValueCollection = new();
            Dictionary<string, object> routeValues = new();
            if (queryString.Length > 1)
            { 
                nameValueCollection = HttpUtility.ParseQueryString(queryString);
                if (nameValueCollection != null)
                {
                    foreach (string? key in nameValueCollection.AllKeys)
                    {
                        if (key != null)
                        {
                            object value = nameValueCollection[key];
                            routeValues.Add(key, value);
                        }
                    }
                }
            }
            if (routeId > 0 && nameValueCollection["id"] == null)
                routeValues.Add("id", routeId);
            return routeValues;
        }

        public User AddUser(string userName, string password, string email)
        {
            string returnValue = "id";
            string command = $"uspAddUser('{userName}','{password}','{email}')";
            var reply = ExecuteSqlStatement(typeof(Photography), command);
            if (reply != null)
                reply = reply.GetAnnotationByValue(1);

            if (reply != null && reply.GetAnnotation("Record") != null)
            {
                int id = (int)reply.GetAnnotation(returnValue).Value;

                if (id == -1)
                    return null;

                return new User()
                {
                    UserId = id,
                    UserName = userName,
                    Password = "",
                    Email = email
                };
            }
            else
                throw new ApplicationException($"Error storing new user '{userName}'.");
        }

        public User UpdateUserPassword(int userId, string userName, string password)
        {
            string command = $"uspUpdateUserPassword('{userId}','{userName}','{password}')";
            string[] returnItems = new[] { "id", "email" };
            var reply = ExecuteSqlStatement(typeof(User), command, returnItems);
            if (reply != null)
                reply = reply.GetAnnotationByValue(1);

            User user = null;
            if (reply != null && reply.GetAnnotation("Record") != null)
            {
                int id = (int)reply.GetAnnotation(returnItems[0]).Value;

                if (id == -1)
                    return null;

                string email = (string)reply.GetAnnotation(returnItems[1]).Value;
                user = new User()
                {
                    UserId = id,
                    UserName = userName,
                    Password = password,
                    Email = email
                };
            }

            return user;
        }

        public void EndSession(string? sessionId)
        {
            if (sessionId == null)
                throw new ApplicationException("SessionId has not been set.");

            string command = $"uspEndSession('{sessionId}')";
            _ = ExecuteSqlStatement(typeof(ISession), command);
        }

        public void StoreActivationCode(int userId, string activationCode)
        {
            string command = $"uspStoreActivationCode('{userId}','{activationCode}')";
            _ = ExecuteSqlStatement(typeof(User), command);
        }

        public (int, User?) VerifyActivationCode(string activationCode)
        {
            int errorCode = -1;

            string command = $"uspVerifyActivationCode('{activationCode}')";
            string[] returnItems = new[] { "id", "login", "email" , "password", "errorCode" };
            var reply = ExecuteSqlStatement(typeof(User), command, returnItems);
            if (reply != null)
                reply = reply.GetAnnotationByValue(1);

            User? user = null;
            if (reply != null && reply.GetAnnotation("Record") != null)
            {
                int id = (int)reply.GetAnnotation(returnItems[0]).Value;

                if (id == -1)
                    return (-1 , null);

                string userName = (string)reply.GetAnnotation(returnItems[1]).Value;
                string email = (string)reply.GetAnnotation(returnItems[2]).Value;
                string password = (string)reply.GetAnnotation(returnItems[3]).Value;
                errorCode = (int)reply.GetAnnotation(returnItems[4]).Value;

                user = new User()
                {
                    UserId = id,
                    UserName = userName,
                    Password = password,
                    Email = email
                };
            }

            return (errorCode, user);
        }

        private static Order MapOrderFromDatabaseReplyToEntityModel(int userId, ValueHolder reply)
        {
            Order order = null;
            if (reply != null && reply.GetAnnotation("Record") != null)
            {
                foreach (ValueHolder annotation in reply.Annotations)
                {
                    int id = Convert.ToInt32(annotation.GetAnnotation("id").Value);
                    string number = (string)annotation.GetAnnotation("number").Value;
                    if (string.IsNullOrEmpty(number))
                        number = Guid.Empty.ToString();
                    Guid guid = Guid.Parse(number);
                    DateTime createdDtm = Convert.ToDateTime(annotation.GetAnnotation("createdDtm").Value);
                    string status = (string)annotation.GetAnnotation("status").Value;
                    Order.OrderStatusType orderStatus = EnumExtensions.GetValueFromDescription<Order.OrderStatusType>(status);
                    int count = Convert.ToInt32(annotation.GetAnnotation("count").Value);

                    order = new Order(id, userId, guid, createdDtm, count, orderStatus);
                }
            }
            return order;
        }

        private static List<Photography> MapPhotographyListFromDatabaseReplyToEntityModel(
          int userId,
          ValueHolder reply)
        {
            List<Photography> entityModel = new List<Photography>();
            if (reply != null && reply.GetAnnotation("Record") != null)
            {
                foreach (ValueHolder annotation in reply.Annotations)
                {
                    long id = (long)annotation.GetAnnotation("id").Value;
                    int source = Convert.ToInt32(annotation.GetAnnotation("source").Value);
                    string path = (string)annotation.GetAnnotation("path").Value;
                    string fileName = (string)annotation.GetAnnotation("filename").Value;
                    string title = (string)annotation.GetAnnotation("title").Value;
                    string location = (string)annotation.GetAnnotation("location").Value;
                    long rank = (long)annotation.GetAnnotation("rank").Value;
                    long averageRank = Convert.ToInt64(annotation.GetAnnotation("averageRank").Value);
                    string tags = (string)annotation.GetAnnotation("tags").Value;
                    var photography = new Photography()
                    {
                        UserId = userId,
                        Id = id,
                        FileName = fileName,
                        Path = path,
                        Source = (Photography.PhysicalSource)source,
                        Title = title,
                        Location = location,
                        Rank = rank,
                        AverageRank = (double)averageRank
                    };
                    photography.ParseTags(tags);
                    entityModel.Add(photography);
                }
            }
            return entityModel;
        }

        public void AddTags(
          int userId,
          string tags,
          IEnumerable<Photography> photographies)
        {
            foreach (Photography photography in photographies)
            {
                foreach (string tag in tags.Split(','))
                    AddTag(userId, tag, photography.Id);
            }
        }

        public int AddTag(int userId, string tag, long photographyId)
        {
            string retunValue = "id";
            string command = $"uspAddTag('{userId}','{tag}','{photographyId}')";
            var returns = ExecuteSqlStatement(typeof(Photography), command, new[] { retunValue });
            if (returns != null)
                returns = returns.GetAnnotationByValue(1);

            return (returns != null) ? (int)returns.GetAnnotation(retunValue).Value : -1;
        } 

    public int RemoveTag(int userId, string tag, long photographyId)
        {
            string retunValue = "id";
            string command = $"uspRemoveTag('{userId}','{tag}','{photographyId}')";
            var returns = ExecuteSqlStatement(typeof(Photography), command, new[] { retunValue });
            if (returns != null)
                returns = returns.GetAnnotationByValue(1);

            return (returns != null) ? (int)returns.GetAnnotation(retunValue).Value : -1;
        }

        public void ConnectUserAndRemoteHost(int userId, string remoteHost)
        {
            string command = $"uspConnectUserAndRemoteHost('{userId}','{remoteHost}')";
            var _ = ExecuteSqlStatement(typeof(User), command);
        }

        public void AddAuditMessage(int userId, string meessage, string source = "", int isError = 0)
        {
            string command = $"uspAddAuditMessage('{userId}','{meessage}','{source}','{isError}'";
            var _ = ExecuteSqlStatement(typeof(User), command);
        }

        public IEnumerable<Photography> GetPhotographiesBySearch(int userId, string query, int pageId = 1)
        {
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            string command = $"uspGetPhotographiesBySearch('{userId}','{query}','{pageId}','{PageSize}')";
            var reply = ExecuteSqlStatement(typeof(Photography), command);
            return PhotoService.MapPhotographyListFromDatabaseReplyToEntityModel(userId, reply);
        }

        public IEnumerable<string> GetAllTags(int pageId = 1)
        {
            string command = $"uspGetTags('{pageId}','{PageSize}')";
            var reply = ExecuteSqlStatement(typeof(Photography), command);
            if (reply != null && reply.GetAnnotation("Record") != null)
            {
                foreach (ValueHolder annotation in reply.Annotations)
                    yield return (string)annotation.GetAnnotation("tag").Value;
            }
        }

        public IEnumerable<string> GetAllLocations(int pageId = 1)
        {
            string command = $"uspGetLocations('{pageId}','{PageSize}')";
            var reply = ExecuteSqlStatement(typeof(Photography), command);
            if (reply != null && reply.GetAnnotation("Record") != null)
            {
                foreach (ValueHolder annotation in reply.Annotations)
                    yield return (string)annotation.GetAnnotation("location").Value;
            }
        }

        public Order GetCurrentActiveOrder(int userId)
        {
            string command = $"uspGetCurrentActiveOrder('{userId}')";
            var reply = ExecuteSqlStatement(typeof(Photography), command);
            
            return PhotoService.MapOrderFromDatabaseReplyToEntityModel(userId, reply);
        }

        public Order GetOrder(int userId, int orderId)
        {
            string command =  $"uspGetOrder('{orderId}','{userId}','-1')";
            var reply = ExecuteSqlStatement(typeof(Photography), command);

            return PhotoService.MapOrderFromDatabaseReplyToEntityModel(userId, reply);
        }

        public Order AddOrder(int userId)
        {
            string command = $"uspAddOrder('{userId}')";
            var reply = ExecuteSqlStatement(typeof(Photography), command);

            return PhotoService.MapOrderFromDatabaseReplyToEntityModel(userId, reply);
        }

        public bool IsPhotographyInOrder(int orderId, long photographyId, int userId)
        {
            string command = $"uspIsPhotographyInOrder('{orderId}','{photographyId}','{userId}')";
            var reply = ExecuteSqlStatement(typeof(Photography), command);

            return reply != null;
        }

        public IEnumerable<Photography> GetOrderPhotographies(int userId, int orderId, int pageId = 1)
        {
            string command = $"uspGetOrderPhotographies('{userId}','{orderId}','{pageId}','{PageSize}')";
            var reply = ExecuteSqlStatement(typeof(Photography), command);

            return PhotoService.MapPhotographyListFromDatabaseReplyToEntityModel(userId, reply);
        }

        public int AddPhotographyToOrder(long photographyId, int orderId, int userId)
        {
            if (userId == -1)
                return -1;

            string retunValue = "id";
            string command = $"uspAddPhotographyToOrder('{photographyId}','{orderId}','{userId}')";
            var reply = ExecuteSqlStatement(typeof(Order), command, new[] { retunValue });
            if (reply != null)
                reply = reply.GetAnnotationByValue(1);

            return (reply != null) ? (int)reply.GetAnnotation(retunValue).Value : -1;
        }

        public int RemovePhotographyFromOrder(long photographyId, int orderId, int userId)
        {
             if (userId == -1)
                return -1;

            string retunValue = "id";
            string command = $"uspRemovePhotographyFromOrder('{photographyId}','{orderId}','{userId}')";
            var reply = ExecuteSqlStatement(typeof(Order), command, new[] { retunValue });
            if (reply != null)
                reply = reply.GetAnnotationByValue(1);

            return (reply != null) ? (int)reply.GetAnnotation(retunValue).Value : -1;
        }

        public int RemoveOrder(int orderId, int userId)
        {
            if (userId == -1)
                return -1;

            string retunValue = "id";
            string command = $"uspRemoveOrder('{orderId}','{userId}')";
            var reply = ExecuteSqlStatement(typeof(Order), command, new[] { retunValue });
            if (reply != null)
                reply = reply.GetAnnotationByValue(1);

            return (reply != null) ? (int)reply.GetAnnotation(retunValue).Value : -1;
        }

        public bool UpdateOrderItemsIndices(int userId, int orderId, GalleryIndexViewModel model)
        {
            if (model != null && string.IsNullOrEmpty(model.CartItemsSequence))
                return false;
            foreach (KeyValuePair<long, int> keyValuePair in model.CartItemsSequence.Split(',')
                .Select(s => s.Split(':'))
                .ToDictionary(a => Convert.ToInt64(a[0].Trim()), a => Convert.ToInt32(a[1].Trim())))
                UpdateOrderIndex(userId, orderId, keyValuePair.Key, keyValuePair.Value);
            return true;
        }

        public void UpdateOrderIndex(int userId, int orderId, long photographyId, int index)
        {
            string command = $"uspUpdateOrderIndex('{userId}','{orderId}','{photographyId},'{index}')";
            var _ = ExecuteSqlStatement(typeof(User), command);
        }

        public IEnumerable<Photography> LoadPhotographies(
          string directory,
          string acceptedExtensions,
          bool directoryIsLink)
        {
            Dictionary<string, string> fileNames = new();
            List<Photography> photographyList = new();
            List<FileInfo> allFiles = UtilityFile.GetAllFiles(directory, directoryIsLink);
            int count = 1;
            int digits = allFiles.Count.ToString().Length + 2; 

            foreach (FileInfo fileInfo in allFiles.Where(f => acceptedExtensions.Contains(f.Extension)).ToList<FileInfo>())
            {
                if (fileInfo == null) continue;

                string name = fileInfo.Name;
                string? path = fileInfo.DirectoryName;
                if (!string.IsNullOrEmpty(path))
                {
                    if (name.Length > 30)
                    {
                        string key = new DirectoryInfo(path).Name.Replace(' ', '_') + "_" + count.ToString().PadLeft(digits, '0') + fileInfo.Extension;
                        count++;
                        fileNames.Add(key, name);
                        name = key;
                    }
                    if (path.Contains("Archive\\"))
                    {
                        int startIndex = path.IndexOf("Archive\\") + "Archive\\".Length - 1;
                        path = string.Concat("~\\photos", path.AsSpan(startIndex, path.Length - startIndex));
                    }
                    else if (path.Contains("\\photos"))
                    {
                        int startIndex = path.IndexOf("\\photos");
                        path = string.Concat("~", path.AsSpan(startIndex, path.Length - startIndex));
                    }
                    string title = "";
                    long photographyId = AddPhotography(name, path, title);
                    if (photographyId > 0)
                    {
                        Photography photography = new()
                        {
                            UserId = -1,
                            Id = photographyId,
                            FileName = name,
                            Path = path,
                            Source = PhotoService.GetPhotographySource(path),
                            Title = title,
                            Location = "",
                            Rank = 0,
                            AverageRank = 0.0
                        };
                        photographyList.Add(photography);
                    }
                }
            }
            if (fileNames.Count > 0)
                PhotoService.RenameFiles(directory, fileNames);
            return photographyList;
        }

        public IEnumerable<Photography> LoadPhotographiesWithLocation(
          string directory,
          string acceptedExtensions,
          bool directoryIsLink,
          int userId,
          string location)
        {
            List<Photography> list = LoadPhotographies(directory, acceptedExtensions, directoryIsLink).ToList();
            foreach (Photography photography in list)
            {
                UpdatePhotographyLocation(photography.Id, userId, location);
                photography.Location = location;
            }
            return list;
        }

        public long AddPhotography(string name, string path, string title)
        {
            string retunValue = "id";
            string command = $"uspAddPhotoGraphy('{GetPhotographySource(path)}','{name}','{path}','{title}')";
            var reply = ExecuteSqlStatement(typeof(Photography), command, new[] { retunValue });
            if (reply != null)
                reply = reply.GetAnnotationByValue(1);

            return (reply != null) ? (int)reply.GetAnnotation(retunValue).Value : -1;
        }

        private static Photography.PhysicalSource GetPhotographySource(string path)
        {

            Photography.PhysicalSource source = Photography.PhysicalSource.negative;

            if (path.Contains("slide"))
                source = EnumExtensions.GetValueFromDescription<Photography.PhysicalSource>("slide");
            else if (path.Contains("digital"))
                source = EnumExtensions.GetValueFromDescription<Photography.PhysicalSource>("digital");
            
            return source;
        }

        private static void RenameFiles(string directory, Dictionary<string, string> keyValuePairs)
        {
            foreach (KeyValuePair<string, string> keyValuePair in keyValuePairs)
            {
                string dbFileName = Path.Combine(directory, keyValuePair.Value);
                string actualFileName = Path.Combine(directory, keyValuePair.Key);
                if (File.Exists(dbFileName))
                    File.Move(dbFileName, actualFileName);
            }
        }
    }
}
