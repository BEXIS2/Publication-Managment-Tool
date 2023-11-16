﻿using BExIS.App.Bootstrap.Helpers;
using BExIS.Security.Entities.Subjects;
using BExIS.Security.Services.Authorization;
using BExIS.Security.Services.Objects;
using BExIS.Security.Services.Subjects;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Runtime.Remoting.Contexts;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http.Results;
using System.Threading.Tasks;

namespace BExIS.App.Bootstrap.Attributes
{
    public class BExISAuthorizeAttribute : AuthorizeAttribute
    {
        public override async void OnAuthorization(AuthorizationContext filterContext)
        {
            try
            {
                using (var featurePermissionManager = new FeaturePermissionManager())
                using (var operationManager = new OperationManager())
                using (var userManager = new UserManager())
                using (var identityUserService = new IdentityUserService())
                {
                    var areaName = filterContext.RouteData.DataTokens.Keys.Contains("area") ? filterContext.RouteData.DataTokens["area"].ToString() : "Shell";
                    var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                    var actionName = filterContext.ActionDescriptor.ActionName;
                    var operation = operationManager.Find(areaName, controllerName, "*");

                    if (operation == null)
                    {
                        filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                        return;
                    }

                    var feature = operation.Feature;
                    if (feature != null)
                    {
                        User user = await BExISAuthorizeHelper.GetUserFromAuthorizationAsync(filterContext.HttpContext);

                        if (user == null)
                        {
                            filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                            return;
                        }

                        if (feature != null && !featurePermissionManager.Exists(null, feature.Id))
                        {
                            if (!featurePermissionManager.HasAccess(user.Id, feature.Id))
                            {
                                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                                return;
                            }
                        }
                    }
                    //var principal = filterContext.HttpContext.ControllerContext.RequestContext.Principal;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}