// Decompiled with JetBrains decompiler
// Type: JuanMartin.PhotoGallery.Controllers.HttpUtility
// Assembly: JuanMartin.PhotoGallery, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4CB99D68-5881-4C4E-957B-0BF063CB63CD
// Assembly location: C:\GitHub\temp\JuanMartin.PhotoGallery.dll

using JuanMartin.PhotoGallery.Models;
using JuanMartin.Kernel.Extesions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Net.Http.Headers;
using System.ComponentModel;

namespace JuanMartin.PhotoGallery.Controllers
{
    public class HttpUtility
    {
        public static void InitializeSession(ISession session, IConfiguration configuration, HttpContext context)
        {
            //"SmtpClient": {
            session.Set("Version", configuration.GetStringConfigurationValue("Version", "1.0.0"));
            session.Set("GuestModeEnabled", configuration.GetBooleanConfigurationValue("GuestModeEnabled", false));
            session.Set("ConnectionString", configuration.GetStringConfigurationValue("DefaultConnection", "", "ConnectionStrings"));
            session.Set("IsMobile", IsMobileDevice(context));
            session.Set("IsSignedIn", false);
        }
        /// <summary>
        /// Get the next or previous number to a specific (current) number
        /// in a comma-separated list of numbers
        /// <summary>
        /// <param name="currenId"></param>
        /// <param name="idList"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static int GetImageId(
              int currenId,
              string idList,
              ImageRelativePosition position)
        {
            int imageId = -1;
            int index = -1;
            if (string.IsNullOrEmpty(idList))
            {
                idList = "," + idList;
                int currentStringIndex = idList.IndexOf(currenId.ToString());
                string head = (currentStringIndex > 2) ? idList.Substring(0, currentStringIndex - 2) : "";
                string tail = idList.Replace(head, "");
                int stringIndex = currentStringIndex+tail.IndexOf(',');
                int currentIndex = head.Ocurrences(',');
          
                string[] ids = idList.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if ((currentStringIndex > 2 && position == ImageRelativePosition.Previous) || (currentStringIndex < idList.Length - 1 && position == ImageRelativePosition.Next))
                {
                    switch (position)
                    {
                        case ImageRelativePosition.Previous:
                            {
                                index = (currentIndex > 0) ? currentIndex - 1 : -1;
                                break;
                            }
                        case ImageRelativePosition.Next:
                            {
                                index = (currentIndex < idList.Length - 2) ? currentIndex + 1 : -1;
                                break;
                            }
                    }
                if (index != -1)
                    imageId = Convert.ToInt32(ids[index]);
                }
            }
            return imageId;
        }

        public static List<SelectListItem> SetListOfItemsforDisplay(
          List<string> listOfItems,
          string selectedItem)
        {
            List<SelectListItem> selectListItemList = new List<SelectListItem>();
            foreach (string listOfItem in listOfItems)
                selectListItemList.Add(new SelectListItem()
                {
                    Text = listOfItem,
                    Selected = selectedItem == listOfItem
                });
            return selectListItemList;
        }

        public static (bool IsMobile, string DeviceInfo) IsMobileDevice(HttpContext context)
        {
            string? serverVariable = context.GetServerVariable("HTTP_USER_AGENT");
            if (serverVariable == null)
                return (false, string.Empty);
            Regex regex1 = new("(android|bb\\d+|meego).+mobile|avantgo|bada\\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)stringIndex|palm( os)?|phone|p(ixi|re)\\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex regex2 = new("1207|6310|6590|3gso|4thp|50[1-6]stringIndex|770s|802s|a wa|abac|ac(er|oo|s\\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\\-(n|u)|c55\\/|capi|ccwa|cdm\\-|cell|chtm|cldc|cmd\\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\\-s|devi|dica|dmob|do(c|p)o|ds(12|\\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\\-|_)|g1 u|g560|gene|gf\\-5|g\\-mo|go(\\.w|od)|gr(ad|un)|haie|hcit|hd\\-(m|p|t)|hei\\-|hi(pt|ta)|hp( stringIndex|ip)|hs\\-c|ht(c(\\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|stringIndex\\-(20|go|ma)|i230|iac( |\\-|\\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\\/)|klon|kpt |kwc\\-|kyo(c|k)|le(no|xi)|lg( g|\\/(k|l|u)|50|54|\\-[a-w])|libw|lynx|m1\\-w|m3ga|m50\\/|ma(te|ui|xo)|mc(01|21|ca)|m\\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\\-|on|tf|wf|wg|wt)|nok(6|stringIndex)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\\-2|po(ck|rt|se)|prox|psio|pt\\-g|qa\\-a|qc(07|12|21|32|60|\\-[2-7]|stringIndex\\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\\-|oo|p\\-)|sdk\\/|se(c(\\-|0|1)|47|mc|nd|ri)|sgh\\-|shar|sie(\\-|m)|sk\\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\\-|v\\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\\-|tdg\\-|tel(stringIndex|m)|tim\\-|t\\-mo|to(pl|sh)|ts(70|m\\-|m3|m5)|tx\\-9|up(\\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\\-|your|zeto|zte\\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string match = string.Empty;
            if (regex1.IsMatch(serverVariable))
                match = regex1.Match(serverVariable).Groups[0].Value;
            if (regex2.IsMatch(serverVariable.Substring(0, 4)))
                match += regex2.Match(serverVariable).Groups[0].Value;
            return !string.IsNullOrEmpty(match) ? (true, match) : (false, string.Empty);
        }

        public static string GetClientRemoteId(HttpContext context)
        {
            string serverVariable = context.GetServerVariable("REMOTE_HOST");
            if (string.IsNullOrEmpty(serverVariable))
                serverVariable = context.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
            if (string.IsNullOrEmpty(serverVariable))
                serverVariable = context.GetServerVariable("REMOTE_USER");
            return serverVariable;
        }

        public static void SendVerificationEmail(
          string mailTo,
          string passwordResetLink,
          IConfiguration configuration)
        {
            var to = new MailAddress(mailTo);
            var from = new MailAddress(address: configuration.GetStringConfigurationValue("SenderEmailId", "", "SmtpClient"), "JuanMarttin.PhotoGallery");
            string password = configuration.GetStringConfigurationValue("OutgoingEmailAccountPassword", "", "SmtpClient");
            var smtpClient = new SmtpClient(configuration.GetStringConfigurationValue("HostName", "", "SmtpClient"), Convert.ToInt32(configuration.GetStringConfigurationValue("SmtpPort", "", "SmtpClient")))
            {
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = (ICredentialsByHost)new NetworkCredential(from.Address, password)
            };
            string subject = "Reset Password";
            string body = "Hi,<br/><br/>We got request for reset your account password. Please click on the below link to reset your password<br/><br/>Click this <a href=" + passwordResetLink + ">reset password link</a>";
            using (MailMessage message = new MailMessage(from, to)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtpClient.Send(message);
        }

        public static string GetRedirectUrl(
          RedirectResponseModel redirect,
          HttpUtility.GalleryViewTypes overwriteAction = HttpUtility.GalleryViewTypes.None,
          string controllerName = "Gallery")
        {
            string str = overwriteAction.ToString();
            if (redirect == null || !(redirect.RemoteHost != ""))
                return controllerName + "/" + str;
            controllerName = redirect.Controller;
            string action = redirect.Action;
            if (overwriteAction == GalleryViewTypes.None && action != overwriteAction.ToString())
            {
                return $"{controllerName}/{EnumExtensions.GetDescription(GalleryViewTypes.Index)}";
            }
            string thisAction = overwriteAction.ToString();
            Dictionary<string, object>? data = (redirect != null) ? redirect.RouteData : null;
            var routeValues = (data != null) ? new RouteValueDictionary(data) : null;

            return $"{controllerName}/{thisAction}/{PrepareRouteValues(routeValues, thisAction)}";
        }

        public static string PrepareRouteValues(RouteValueDictionary? routeValues, string thisAction)
        {
            string[] source = new string[1] { EnumExtensions.GetDescription(GalleryViewTypes.Detail) };
            string queryString = "";

            if (routeValues != null)
            {
                if (!source.Contains(thisAction) || !routeValues.ContainsKey("id"))
                {
                    queryString = "?";
                }
                else
                {
                    queryString = $"{routeValues["id"]}?";
                }

                foreach (var item in routeValues)
                {
                    if (item.Key == "id")
                        continue;

                    queryString += $"&{item.Key}={item.Value}";
                }
            }
            return queryString;
        }

        public enum     GalleryViewTypes
        {
            [Description("None")]
            None,
            [Description("Index")]
            Index,
            [Description("Detail")]
            Detail,
        }

        public enum ImageRelativePosition
        {
            Previous,
            Next,
        }
    }

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T? Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}
