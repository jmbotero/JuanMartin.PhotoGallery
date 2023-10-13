// Decompiled with JetBrains decompiler
// Type: JuanMartin.PhotoGallery.Services.PhotoService
// Assembly: JuanMartin.PhotoGallery, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4CB99D68-5881-4C4E-957B-0BF063CB63CD
// Assembly location: C:\GitHub\temp\JuanMartin.PhotoGallery.dll

using JuanMartin.Kernel;
using JuanMartin.Kernel.Adapters;
using JuanMartin.Kernel.Messaging;
using JuanMartin.Kernel.Utilities;
using JuanMartin.Kernel.Extesions;
using JuanMartin.Models.Gallery;
using JuanMartin.PhotoGallery.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Collections.Specialized;
using System.Data;
using System.Runtime.CompilerServices;
using System.Web;
namespace JuanMartin.PhotoGallery.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly  IExchangeRequestReply _dbAdapter;
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

        public IEnumerable<Photography> GetAllPhotographies(int userId, int pageId = 1)
        {
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message = new Message("Command", CommandType.StoredProcedure.ToString());
            int num = IsMobile ? this.MobilePageSize : this.PageSize;
            string command = $"uspGetAllPhotographies('{pageId}','{num}','{userId}')";
            ValueHolder valueHolder = new ValueHolder("PhotoGraphies", command);
            message.AddData(valueHolder);
            message.AddSender("uspGetAllPhotographies", typeof(Photography).ToString());
            this._dbAdapter.Send((IMessage)message);
            IRecordSet reply = (IRecordSet)this._dbAdapter.Receive();
            return (IEnumerable<Photography>)PhotoService.MapPhotographyListFromDatabaseReplyToEntityModel(userId, reply);
        }

        public int GetGalleryPageCount(int pageSize)
        {
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 1);
            interpolatedStringHandler.AppendLiteral("uspGetPageCount('");
            interpolatedStringHandler.AppendFormatted<int>(pageSize);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspGetPageCount", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("Gallery", typeof(Photography).ToString());
            this._dbAdapter.Send((IMessage)message1);
            return (int)((IRecordSet)this._dbAdapter.Receive()).Data.GetAnnotationByValue((object)1).GetAnnotation("pageCount").Value;
        }

        public IRecordSet ExecuteSqlStatement(string statement)
        {
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message = new Message("Command", CommandType.Text.ToString());
            message.AddData((object)new ValueHolder("GetPhotographies", (object)statement));
            message.AddSender("Photography", typeof(Photography).ToString());
            this._dbAdapter.Send((IMessage)message);
            return (IRecordSet)this._dbAdapter.Receive();
        }

        public (string ImageIdsList, long RowCount) GetPhotographyIdsList(
          int userID,
          IPhotoService.ImageListSource source,
          string searchQuery,
          int OrderId)
        {
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 4);
            interpolatedStringHandler.AppendLiteral("uspGetPhotographyIdsList('");
            interpolatedStringHandler.AppendFormatted<int>(userID);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<int>((int)source);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted(searchQuery);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<int>(OrderId);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspGetPhotographyIdsList", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("Photography", typeof(Photography).ToString());
            this._dbAdapter.Send((IMessage)message1);
            IRecordSet irecordSet = (IRecordSet)this._dbAdapter.Receive();
            return ("," + (string)irecordSet.Data.GetAnnotationByValue((object)1).GetAnnotation("Ids").Value + ",", (long)irecordSet.Data.GetAnnotationByValue((object)1).GetAnnotation("RowCount").Value);
        }

        public Photography GetPhotographyById(long photographyId, int userId)
        {
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 2);
            interpolatedStringHandler.AppendLiteral("uspGetPotography('");
            interpolatedStringHandler.AppendFormatted<long>(photographyId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<int>(userId);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspGetPotography", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("Photography", typeof(Photography).ToString());
            this._dbAdapter.Send((IMessage)message1);
            IRecordSet reply = (IRecordSet)this._dbAdapter.Receive();
            List<Photography> entityModel = PhotoService.MapPhotographyListFromDatabaseReplyToEntityModel(userId, reply);
            return entityModel.Count != 0 ? entityModel[0] : (Photography)null;
        }

        public int UpdatePhotographyRanking(long photographyId, int userId, int rank)
        {
            if (userId == -1)
                return -1;
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 3);
            interpolatedStringHandler.AppendLiteral("uspUpdateRanking('");
            interpolatedStringHandler.AppendFormatted<int>(userId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<long>(photographyId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<int>(rank);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspUpdateRanking", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("Photography", typeof(Photography).ToString());
            this._dbAdapter.Send((IMessage)message1);
            return (int)((IRecordSet)this._dbAdapter.Receive()).Data.GetAnnotationByValue((object)1).GetAnnotation("id").Value;
        }

        public int UpdatePhotographyDetails(long photographyId, int userId, string location)
        {
            if (userId == -1)
                return -1;
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 3);
            interpolatedStringHandler.AppendLiteral("uspUpdatePhotographyDetails('");
            interpolatedStringHandler.AppendFormatted<int>(userId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<long>(photographyId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted(location);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspUpdatePhotographyDetails", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("Photography", typeof(Photography).ToString());
            this._dbAdapter.Send((IMessage)message1);
            return (int)((IRecordSet)this._dbAdapter.Receive()).Data.GetAnnotationByValue((object)1).GetAnnotation("id").Value;
        }

        public int UpdatePhotographyDetails(
          string connectionString,
          long photographyId,
          int userId,
          string location)
        {
            if (userId == -1)
                return -1;
            IExchangeRequestReply iexchangeRequestReply = (IExchangeRequestReply)new AdapterMySql(connectionString);
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 3);
            interpolatedStringHandler.AppendLiteral("uspUpdatePhotographyDetails('");
            interpolatedStringHandler.AppendFormatted<int>(userId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<long>(photographyId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted(location);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspUpdatePhotographyDetails", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("Photography", typeof(Photography).ToString());
            iexchangeRequestReply.Send((IMessage)message1);
            return (int)((IRecordSet)iexchangeRequestReply.Receive()).Data.GetAnnotationByValue((object)1).GetAnnotation("id").Value;
        }

        public User GetUser(string userName, string password)
        {
            User user = (User)null;
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 2);
            interpolatedStringHandler.AppendLiteral("uspGetUser('");
            interpolatedStringHandler.AppendFormatted(userName);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted(password);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspGetUser", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("User", typeof(User).ToString());
            this._dbAdapter.Send((IMessage)message1);
            IRecordSet irecordSet = (IRecordSet)this._dbAdapter.Receive();
            if (irecordSet.Data != null && irecordSet.Data.GetAnnotation("Record") != null)
            {
                foreach (ValueHolder annotation in irecordSet.Data.Annotations)
                {
                    int num = (int)annotation.GetAnnotation("Id").Value;
                    if (num == -1)
                        return (User)null;
                    string str = (string)annotation.GetAnnotation("Email").Value;
                    user = new User()
                    {
                        UserId = num,
                        UserName = userName,
                        Password = password,
                        Email = str
                    };
                }
            }
            return user;
        }

        public User VerifyEmail(string email)
        {
            User user = (User)null;
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message = new Message("Command", CommandType.StoredProcedure.ToString());
            message.AddData((object)new ValueHolder("uspVerifyEmail", (object)("uspVerifyEmail('" + email + "')")));
            message.AddSender("User", typeof(User).ToString());
            this._dbAdapter.Send((IMessage)message);
            IRecordSet irecordSet = (IRecordSet)this._dbAdapter.Receive();
            if (irecordSet.Data != null && irecordSet.Data.GetAnnotation("Record") != null)
            {
                foreach (ValueHolder annotation in irecordSet.Data.Annotations)
                {
                    int num = (int)annotation.GetAnnotation("Id").Value;
                    string str = (string)annotation.GetAnnotation("Login").Value;
                    user = new User()
                    {
                        UserId = num,
                        UserName = str,
                        Email = email
                    };
                }
            }
            return user;
        }

        public int LoadSession(int userId)
        {
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler.AppendLiteral("uspAddSession('");
            interpolatedStringHandler.AppendFormatted<int>(userId);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspAddSession", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("Session", typeof(ISession).ToString());
            this._dbAdapter.Send((IMessage)message1);
            return (int)((IRecordSet)this._dbAdapter.Receive()).Data.GetAnnotationByValue((object)1).GetAnnotation("id").Value;
        }

        public RedirectResponseModel GetRedirectInfo(int userId, string remoteHost)
        {
            RedirectResponseModel redirectInfo = (RedirectResponseModel)null;
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 2);
            interpolatedStringHandler.AppendLiteral("uspGetUserRedirectInfo('");
            interpolatedStringHandler.AppendFormatted(remoteHost);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<int>(userId);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspGetUserRedirectInfo", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("RedirectRequestModel", typeof(RedirectResponseModel).ToString());
            this._dbAdapter.Send((IMessage)message1);
            IRecordSet irecordSet = (IRecordSet)this._dbAdapter.Receive();
            if (irecordSet.Data != null && irecordSet.Data.GetAnnotation("Record") != null)
            {
                foreach (ValueHolder annotation in irecordSet.Data.Annotations)
                {
                    string str1 = (string)annotation.GetAnnotation("RemoteHost").Value;
                    if (str1 == "")
                        return (RedirectResponseModel)null;
                    string str2 = (string)annotation.GetAnnotation("Controller").Value;
                    string str3 = (string)annotation.GetAnnotation("Action").Value;
                    Dictionary<string, object> routeValues = this.GenerateRouteValues((long)(int)annotation.GetAnnotation("RouteID").Value, (string)annotation.GetAnnotation("QueryString").Value);
                    redirectInfo = new RedirectResponseModel()
                    {
                        RemoteHost = str1,
                        Controller = str2,
                        Action = str3,
                        RouteData = routeValues
                    };
                }
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
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 6);
            interpolatedStringHandler.AppendLiteral("uspSetUserRedirectInfo('");
            interpolatedStringHandler.AppendFormatted<int>(userId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted(remoteHost);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted(controller);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted(action);
            interpolatedStringHandler.AppendLiteral("',");
            interpolatedStringHandler.AppendFormatted<long>(routeId);
            interpolatedStringHandler.AppendLiteral(",'");
            interpolatedStringHandler.AppendFormatted(queryString);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspSetUserRedirectInfo", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("RedirectRequestModel", typeof(RedirectResponseModel).ToString());
            this._dbAdapter.Send((IMessage)message1);
            Dictionary<string, object> routeValues = this.GenerateRouteValues(routeId, queryString);
            return new RedirectResponseModel()
            {
                RemoteHost = remoteHost,
                Controller = controller,
                Action = action,
                RouteData = routeValues
            };
        }

        public Dictionary<string, object> GenerateRouteValues(long routeId, string queryString)
        {
            if (string.IsNullOrEmpty(queryString))
                return (Dictionary<string, object>)null;
            NameValueCollection nameValueCollection = new NameValueCollection();
            Dictionary<string, object> routeValues = new Dictionary<string, object>();
            if (queryString.Length > 1)
            {
                nameValueCollection = HttpUtility.ParseQueryString(queryString);
                if (nameValueCollection != null)
                {
                    foreach (string allKey in nameValueCollection.AllKeys)
                    {
                        if (allKey != null)
                        {
                            object obj = (object)nameValueCollection[allKey];
                            routeValues.Add(allKey, obj);
                        }
                    }
                }
            }
            if (routeId > 0L && nameValueCollection["id"] == null)
                routeValues.Add("id", (object)routeId);
            return routeValues;
        }

        public User AddUser(string userName, string password, string email)
        {
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 3);
            interpolatedStringHandler.AppendLiteral("uspAddUser('");
            interpolatedStringHandler.AppendFormatted(userName);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted(password);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted(email);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspAddUser", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("User", typeof(User).ToString());
            this._dbAdapter.Send((IMessage)message1);
            int num = (int)((IRecordSet)this._dbAdapter.Receive()).Data.GetAnnotationByValue((object)1).GetAnnotation("id").Value;
            return new User()
            {
                UserId = num,
                UserName = userName,
                Password = "",
                Email = email
            };
        }

        public User UpdateUserPassword(int userId, string userName, string password)
        {
            User user = (User)null;
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 3);
            interpolatedStringHandler.AppendLiteral("uspUpdateUserPassword('");
            interpolatedStringHandler.AppendFormatted<int>(userId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted(userName);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted(password);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspUpdateUserPassword", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("User", typeof(User).ToString());
            this._dbAdapter.Send((IMessage)message1);
            IRecordSet irecordSet = (IRecordSet)this._dbAdapter.Receive();
            if (irecordSet.Data != null && irecordSet.Data.GetAnnotation("Record") != null)
            {
                foreach (ValueHolder annotation in irecordSet.Data.Annotations)
                {
                    int int32 = Convert.ToInt32(annotation.GetAnnotation("Id").Value);
                    string str = (string)annotation.GetAnnotation("Email").Value;
                    user = new User()
                    {
                        UserId = int32,
                        UserName = userName,
                        Password = password,
                        Email = str
                    };
                }
            }
            return user;
        }

        public void EndSession(int sessionId)
        {
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(15, 1);
            interpolatedStringHandler.AppendLiteral("uspEndSession(");
            interpolatedStringHandler.AppendFormatted<int>(sessionId);
            interpolatedStringHandler.AppendLiteral(")");
            ValueHolder valueHolder = new ValueHolder("uspEndSession", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("Session", typeof(int).ToString());
            this._dbAdapter.Send((IMessage)message1);
        }

        public void StoreActivationCode(int userId, string activationCode)
        {
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 2);
            interpolatedStringHandler.AppendLiteral("uspStoreActivationCode('");
            interpolatedStringHandler.AppendFormatted<int>(userId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted(activationCode);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspStoreActivationCode", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("ActivationCode", typeof(string).ToString());
            this._dbAdapter.Send((IMessage)message1);
        }

        public (int, User) VerifyActivationCode(string activationCode)
        {
            User user = (User)null;
            int num1 = -1;
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message = new Message("Command", CommandType.StoredProcedure.ToString());
            message.AddData((object)new ValueHolder("uspVerifyActivationCode", (object)("uspVerifyActivationCode('" + activationCode + "')")));
            message.AddSender("ActivationCode", typeof(Guid).ToString());
            this._dbAdapter.Send((IMessage)message);
            IRecordSet irecordSet = (IRecordSet)this._dbAdapter.Receive();
            if (irecordSet.Data != null && irecordSet.Data.GetAnnotation("Record") != null)
            {
                foreach (ValueHolder annotation in irecordSet.Data.Annotations)
                {
                    num1 = (int)annotation.GetAnnotation("ErrorCode").Value;
                    int num2 = (int)annotation.GetAnnotation("Id").Value;
                    string str = (string)annotation.GetAnnotation("Login").Value;
                    user = new User()
                    {
                        UserId = num2,
                        UserName = str,
                        Password = "",
                        Email = ""
                    };
                }
            }
            return (num1, user);
        }

        private static Order MapOrderFromDatabaseReplyToEntityModel(int userId, IRecordSet reply)
        {
            Order entityModel = (Order)null;
            if (reply.Data != null && reply.Data.GetAnnotation("Record") != null)
            {
                foreach (ValueHolder annotation in reply.Data.Annotations)
                {
                    int int32_1 = Convert.ToInt32(annotation.GetAnnotation("Id").Value);
                    string input = (string)annotation.GetAnnotation("Number").Value;
                    if (string.IsNullOrEmpty(input))
                        input = Guid.Empty.ToString();
                    Guid guid = Guid.Parse(input);
                    DateTime dateTime = Convert.ToDateTime(annotation.GetAnnotation("CreatedDtm").Value);
                    string str = (string)annotation.GetAnnotation("Status").Value;
                    int int32_2 = Convert.ToInt32(annotation.GetAnnotation("Count").Value);
                    Order.OrderStatus orderStatus;
                    switch (str)
                    {
                        case "inProcess":
                            orderStatus = (Order.OrderStatus)1;
                            break;
                        case "complete":
                            orderStatus = (Order.OrderStatus)2;
                            break;
                        default:
                            orderStatus = (Order.OrderStatus)0;
                            break;
                    }
                    entityModel = new Order(int32_1, userId, guid, dateTime, int32_2, orderStatus);
                }
            }
            return entityModel;
        }

        private static List<Photography> MapPhotographyListFromDatabaseReplyToEntityModel(
          int userId,
          IRecordSet reply)
        {
            List<Photography> entityModel = new List<Photography>();
            if (reply.Data != null && reply.Data.GetAnnotation("Record") != null)
            {
                foreach (ValueHolder annotation in reply.Data.Annotations)
                {
                    long num1 = (long)annotation.GetAnnotation("Id").Value;
                    int int32 = Convert.ToInt32(annotation.GetAnnotation("Source").Value);
                    string str1 = (string)annotation.GetAnnotation("Path").Value;
                    string str2 = (string)annotation.GetAnnotation("Filename").Value;
                    string str3 = (string)annotation.GetAnnotation("Title").Value;
                    string str4 = (string)annotation.GetAnnotation("Location").Value;
                    long num2 = (long)annotation.GetAnnotation("Rank").Value;
                    long int64 = Convert.ToInt64(annotation.GetAnnotation("AverageRank").Value);
                    string str5 = (string)annotation.GetAnnotation("Tags").Value;
                    Photography photography = new Photography()
                    {
                        UserId = userId,
                        Id = num1,
                        FileName = str2,
                        Path = str1,
                        Source = (Photography.PhysicalSource)int32,
                        Title = str3,
                        Location = str4,
                        Rank = num2,
                        AverageRank = (double)int64
                    };
                    photography.ParseTags(str5);
                    entityModel.Add(photography);
                }
            }
            return entityModel;
        }

        public void AddTags(
          string connectionString,
          int userId,
          string tags,
          IEnumerable<Photography> photographies)
        {
            foreach (Photography photography in photographies)
            {
                foreach (string tag in tags.Split(','))
                    this.AddTag(connectionString, userId, tag, photography.Id);
            }
        }

        public int AddTag(string connectionString, int userId, string tag, long photographyId)
        {
            AdapterMySql adapterMySql = new AdapterMySql(connectionString);
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 3);
            interpolatedStringHandler.AppendLiteral("uspAddTag('");
            interpolatedStringHandler.AppendFormatted<int>(userId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted(tag);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<long>(photographyId);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspAddTag", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("Photography", typeof(Photography).ToString());
            adapterMySql.Send((IMessage)message1);
            return Convert.ToInt32(((IRecordSet)adapterMySql.Receive()).Data.GetAnnotationByValue((object)1).GetAnnotation("id").Value);
        }

        public int AddTag(int userId, string tag, long photographyId)
        {
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 3);
            interpolatedStringHandler.AppendLiteral("uspAddTag('");
            interpolatedStringHandler.AppendFormatted<int>(userId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted(tag);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<long>(photographyId);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspAddTag", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("Photography", typeof(Photography).ToString());
            this._dbAdapter.Send((IMessage)message1);
            return Convert.ToInt32(((IRecordSet)this._dbAdapter.Receive()).Data.GetAnnotationByValue((object)1).GetAnnotation("id").Value);
        }

        public int RemoveTag(int userId, string tag, long photographyId)
        {
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 3);
            interpolatedStringHandler.AppendLiteral("uspRemoveTag('");
            interpolatedStringHandler.AppendFormatted<int>(userId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted(tag);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<long>(photographyId);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspRemoveTag", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("Photography", typeof(Photography).ToString());
            this._dbAdapter.Send((IMessage)message1);
            return Convert.ToInt32(((IRecordSet)this._dbAdapter.Receive()).Data.GetAnnotationByValue((object)1).GetAnnotation("id").Value);
        }

        public void ConnectUserAndRemoteHost(int userId, string remoteHost)
        {
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 2);
            interpolatedStringHandler.AppendLiteral("uspConnectUserAndRemoteHost('");
            interpolatedStringHandler.AppendFormatted<int>(userId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted(remoteHost);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspConnectUserAndRemoteHost", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("User", typeof(User).ToString());
            this._dbAdapter.Send((IMessage)message1);
        }

        public void AddAuditMessage(int userId, string meessage, string source = "", int isError = 0)
        {
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 4);
            interpolatedStringHandler.AppendLiteral("uspAddAuditMessage('");
            interpolatedStringHandler.AppendFormatted<int>(userId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted(meessage);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted(source);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<int>(isError);
            interpolatedStringHandler.AppendLiteral("'");
            ValueHolder valueHolder = new ValueHolder("uspAddAuditMessage", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("User", typeof(User).ToString());
            this._dbAdapter.Send((IMessage)message1);
        }

        public IEnumerable<Photography> GetPhotographiesBySearch(int userId, string query, int pageId = 1)
        {
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 4);
            interpolatedStringHandler.AppendLiteral("uspGetPhotographiesBySearch('");
            interpolatedStringHandler.AppendFormatted<int>(userId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted(query);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<int>(pageId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<int>(this.PageSize);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspGetPhotographiesBySearch", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("Photography", typeof(Photography).ToString());
            this._dbAdapter.Send((IMessage)message1);
            IRecordSet reply = (IRecordSet)this._dbAdapter.Receive();
            return (IEnumerable<Photography>)PhotoService.MapPhotographyListFromDatabaseReplyToEntityModel(userId, reply);
        }

        public IEnumerable<string> GetAllTags(int pageId = 1)
        {
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 2);
            interpolatedStringHandler.AppendLiteral("uspGetTags('");
            interpolatedStringHandler.AppendFormatted<int>(pageId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<int>(this.PageSize);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspGetTags", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("Tag", typeof(string).ToString());
            this._dbAdapter.Send((IMessage)message1);
            IRecordSet irecordSet = (IRecordSet)this._dbAdapter.Receive();
            if (irecordSet.Data != null && irecordSet.Data.GetAnnotation("Record") != null)
            {
                foreach (ValueHolder annotation in irecordSet.Data.Annotations)
                    yield return (string)annotation.GetAnnotation("Tag").Value;
            }
        }

        public IEnumerable<string> GetAllLocations(int pageId = 1)
        {
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 2);
            interpolatedStringHandler.AppendLiteral("uspGetLocations('");
            interpolatedStringHandler.AppendFormatted<int>(pageId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<int>(this.PageSize);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspGetLocations", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("Location", typeof(string).ToString());
            this._dbAdapter.Send((IMessage)message1);
            IRecordSet irecordSet = (IRecordSet)this._dbAdapter.Receive();
            if (irecordSet.Data != null && irecordSet.Data.GetAnnotation("Record") != null)
            {
                foreach (ValueHolder annotation in irecordSet.Data.Annotations)
                    yield return (string)annotation.GetAnnotation("Location").Value;
            }
        }

        public Order GetCurrentActiveOrder(int userId)
        {
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 1);
            interpolatedStringHandler.AppendLiteral("uspGetCurrentActiveOrder('");
            interpolatedStringHandler.AppendFormatted<int>(userId);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspGetCurrentActiveOrder", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("Order", typeof(Order).ToString());
            this._dbAdapter.Send((IMessage)message1);
            IRecordSet reply = (IRecordSet)this._dbAdapter.Receive();
            return PhotoService.MapOrderFromDatabaseReplyToEntityModel(userId, reply);
        }

        public Order GetOrder(int userId, int orderId)
        {
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 3);
            interpolatedStringHandler.AppendLiteral("uspGetOrder('");
            interpolatedStringHandler.AppendFormatted<int>(orderId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<int>(userId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<int>(-1);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspGetOrder", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("Order", typeof(Order).ToString());
            this._dbAdapter.Send((IMessage)message1);
            IRecordSet reply = (IRecordSet)this._dbAdapter.Receive();
            return PhotoService.MapOrderFromDatabaseReplyToEntityModel(userId, reply);
        }

        public Order AddOrder(int userId)
        {
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(15, 1);
            interpolatedStringHandler.AppendLiteral("uspAddOrder('");
            interpolatedStringHandler.AppendFormatted<int>(userId);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspAddOrder", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("Order", typeof(Order).ToString());
            this._dbAdapter.Send((IMessage)message1);
            IRecordSet reply = (IRecordSet)this._dbAdapter.Receive();
            return PhotoService.MapOrderFromDatabaseReplyToEntityModel(userId, reply);
        }

        public bool IsPhotographyInOrder(int orderId, long photographyId, int userId)
        {
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 3);
            interpolatedStringHandler.AppendLiteral("uspIsPhotographyInOrder('");
            interpolatedStringHandler.AppendFormatted<int>(orderId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<long>(photographyId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<int>(userId);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspIsPhotographyInOrder", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("Order", typeof(Order).ToString());
            this._dbAdapter.Send((IMessage)message1);
            IRecordSet irecordSet = (IRecordSet)this._dbAdapter.Receive();
            return irecordSet != null && irecordSet.Data != null;
        }

        public IEnumerable<Photography> GetOrderPhotographies(int userId, int orderId, int pageId = 1)
        {
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 4);
            interpolatedStringHandler.AppendLiteral("uspGetOrderPhotographies('");
            interpolatedStringHandler.AppendFormatted<int>(userId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<int>(orderId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<int>(pageId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<int>(this.PageSize);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspGetOrderPhotographies", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("Photography", typeof(Photography).ToString());
            this._dbAdapter.Send((IMessage)message1);
            IRecordSet reply = (IRecordSet)this._dbAdapter.Receive();
            return (IEnumerable<Photography>)PhotoService.MapPhotographyListFromDatabaseReplyToEntityModel(userId, reply);
        }

        public int AddPhotographyToOrder(long photographyId, int orderId, int userId)
        {
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 3);
            interpolatedStringHandler.AppendLiteral("uspAddPhotographyToOrder('");
            interpolatedStringHandler.AppendFormatted<long>(photographyId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<int>(orderId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<int>(userId);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspAddPhotographyToOrder", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("Photography", typeof(Photography).ToString());
            this._dbAdapter.Send((IMessage)message1);
            return Convert.ToInt32(((IRecordSet)this._dbAdapter.Receive()).Data.GetAnnotationByValue((object)1).GetAnnotation("id").Value);
        }

        public int RemovePhotographyFromOrder(long photographyId, int orderId, int userId)
        {
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 3);
            interpolatedStringHandler.AppendLiteral("uspRemovePhotographyFromOrder('");
            interpolatedStringHandler.AppendFormatted<long>(photographyId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<int>(orderId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<int>(userId);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspRemovePhotographyFromOrder", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("Photography", typeof(Photography).ToString());
            this._dbAdapter.Send((IMessage)message1);
            return Convert.ToInt32(((IRecordSet)this._dbAdapter.Receive()).Data.GetAnnotationByValue((object)1).GetAnnotation("id").Value);
        }

        public int RemoveOrder(int orderId, int userId)
        {
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 2);
            interpolatedStringHandler.AppendLiteral("uspRemoveOrder('");
            interpolatedStringHandler.AppendFormatted<int>(orderId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<int>(userId);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspRemoveOrder", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("Photography", typeof(Photography).ToString());
            this._dbAdapter.Send((IMessage)message1);
            return Convert.ToInt32(((IRecordSet)this._dbAdapter.Receive()).Data.GetAnnotationByValue((object)1).GetAnnotation("id").Value);
        }

        public bool UpdateOrderItemsIndices(int userId, int orderId, GalleryIndexViewModel model)
        {
            if (model == null && !string.IsNullOrEmpty(model.CartItemsSequence))
                return false;
            foreach (KeyValuePair<long, int> keyValuePair in ((IEnumerable<string>)model.CartItemsSequence.Split(',')).Select<string, string[]>((Func<string, string[]>)(s => s.Split(':'))).ToDictionary<string[], long, int>((Func<string[], long>)(a => Convert.ToInt64(a[0].Trim())), (Func<string[], int>)(a => Convert.ToInt32(a[1].Trim()))))
                this.UpdateOrderIndex(userId, orderId, keyValuePair.Key, keyValuePair.Value);
            return true;
        }

        public void UpdateOrderIndex(int userId, int orderId, long photographyId, int index)
        {
            if (this._dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 4);
            interpolatedStringHandler.AppendLiteral("uspUpdateOrderIndex('");
            interpolatedStringHandler.AppendFormatted<int>(userId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<int>(orderId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<long>(photographyId);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted<int>(index);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspUpdateOrderIndex", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("Order", typeof(Order).ToString());
            this._dbAdapter.Send((IMessage)message1);
        }

        public IEnumerable<Photography> LoadPhotographies(
          string connectionString,
          string directory,
          string acceptedExtensions,
          bool directoryIsLink)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            List<Photography> photographyList = new List<Photography>();
            List<FileInfo> allFiles = UtilityFile.GetAllFiles(directory, directoryIsLink);
            int num1 = 1;
            foreach (FileInfo fileInfo in allFiles.Where<FileInfo>((Func<FileInfo, bool>)(f => acceptedExtensions.Contains(f.Extension))).ToList<FileInfo>())
            {
                string name = fileInfo.Name;
                string path = fileInfo.DirectoryName;
                if (name.Length > 30)
                {
                    string key = new DirectoryInfo(path).Name.Replace(' ', '_') + "_" + num1.ToString().PadLeft(5, '0') + fileInfo.Extension;
                    keyValuePairs.Add(key, name);
                    name = key;
                }
                if (path.Contains("Archive\\"))
                {
                    int num2 = path.IndexOf("Archive\\") + "Archive\\".Length - 1;
                    string str = path;
                    int startIndex = num2;
                    path = "~\\photos" + str.Substring(startIndex, str.Length - startIndex);
                }
                else if (path.Contains("\\photos"))
                {
                    int num3 = path.IndexOf("\\photos");
                    string str = path;
                    int startIndex = num3;
                    path = "~" + str.Substring(startIndex, str.Length - startIndex);
                }
                string title = "";
                long num4 = this.AddPhotography(new AdapterMySql(connectionString), name, path, title);
                if (num4 > 0L)
                {
                    Photography photography = new Photography()
                    {
                        UserId = -1,
                        Id = num4,
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
                ++num1;
            }
            if (keyValuePairs.Count > 0)
                PhotoService.RenameFiles(directory, keyValuePairs);
            return (IEnumerable<Photography>)photographyList;
        }

        public IEnumerable<Photography> LoadPhotographiesWithLocation(
          string connectionString,
          string directory,
          string acceptedExtensions,
          bool directoryIsLink,
          int userId,
          string location)
        {
            List<Photography> list = this.LoadPhotographies(connectionString, directory, acceptedExtensions, directoryIsLink).ToList<Photography>();
            foreach (Photography photography in list)
            {
                this.UpdatePhotographyDetails(connectionString, photography.Id, userId, location);
                photography.Location = location;
            }
            return (IEnumerable<Photography>)list;
        }

        public long AddPhotography(AdapterMySql dbAdapter, string name, string path, string title)
        {
            int photographySource = (int)PhotoService.GetPhotographySource(path);
            Message message1 = new Message("Command", CommandType.StoredProcedure.ToString());
            Message message2 = message1;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 4);
            interpolatedStringHandler.AppendLiteral("uspAddPhotoGraphy(");
            interpolatedStringHandler.AppendFormatted<int>(photographySource);
            interpolatedStringHandler.AppendLiteral(",'");
            interpolatedStringHandler.AppendFormatted(name);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted(path);
            interpolatedStringHandler.AppendLiteral("','");
            interpolatedStringHandler.AppendFormatted(title);
            interpolatedStringHandler.AppendLiteral("')");
            ValueHolder valueHolder = new ValueHolder("uspAddPhotoGraphy", (object)interpolatedStringHandler.ToStringAndClear());
            message2.AddData((object)valueHolder);
            message1.AddSender("PhotoGraphy", typeof(Photography).ToString());
            dbAdapter.Send((IMessage)message1);
            IRecordSet irecordSet = (IRecordSet)dbAdapter.Receive();
            long num = (long)irecordSet.Data.GetAnnotationByValue((object)1).GetAnnotation("id").Value;
            return (int)irecordSet.Data.GetAnnotationByValue((object)1).GetAnnotation("response").Value != 1 ? -1L : num;
        }

        private static Photography.PhysicalSource GetPhotographySource(string path)
        {
            if (path.Contains("slide"))
                return (Photography.PhysicalSource)1;
            return path.Contains("negative") ? (Photography.PhysicalSource)0 : (Photography.PhysicalSource)2;
        }

        private static void RenameFiles(string directory, Dictionary<string, string> keyValuePairs)
        {
            foreach (KeyValuePair<string, string> keyValuePair in keyValuePairs)
            {
                string str = Path.Combine(directory, keyValuePair.Value);
                string destFileName = Path.Combine(directory, keyValuePair.Key);
                if (File.Exists(str))
                    File.Move(str, destFileName);
            }
        }
    }
}
