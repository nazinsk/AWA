using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;

namespace API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var formatters = config.Formatters;
            formatters.Remove(formatters.XmlFormatter);

            var jsonSettings = formatters.JsonFormatter.SerializerSettings;
            jsonSettings.Formatting = Formatting.Indented;
            jsonSettings.ContractResolver = new CamelCase();

            formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;

            // Web API configuration and services

            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.EnableCors();// Microsoft.Owin.Cors.CorsOptions.AllowAll);
            //app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
        }
    }


    class CamelCase : CamelCasePropertyNamesContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var res = base.CreateProperty(member, memberSerialization);

            //var attrs = member.GetCustomAttributes(typeof(JsonPropertyAttribute), true);

            //if (attrs.Any())
            //{
            //    var attr = (attrs[0] as JsonPropertyAttribute);
            if (res.PropertyName != null)
            {
                if (res.PropertyName.ToLower() == "id" ||
                    res.PropertyName.ToLower() == "index")
                    res.PropertyName = res.PropertyName.ToUpper();
            }

            //}

            return res;
        }
    }
}
