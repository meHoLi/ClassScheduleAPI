﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ClassScheduleAPI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                //defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                defaults: new { controller = "Hacker", action = "Out", id = UrlParameter.Optional }
            );

            //虚拟目录，子站点路由
            routes.MapRoute(
            "HouseMovingAPI",
            "HouseMovingAPI/{controller}/{action}/{id}",
            defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });

            //虚拟目录，子站点路由
            routes.MapRoute(
            "ClassScheduleAPIV3",
            "ClassScheduleAPIV3/{controller}/{action}/{id}",
            defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}
