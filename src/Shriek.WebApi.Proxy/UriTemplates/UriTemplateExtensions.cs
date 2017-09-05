using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Shriek.WebApi.Proxy.UriTemplates
{
    public static class UriTemplateExtensions
    {
        public static UriTemplate AddParameter(this UriTemplate template, string name, object value)
        {
            template.SetParameter(name, value);

            return template;
        }

        public static UriTemplate AddParameters(this UriTemplate template, object parametersObject)
        {

            if (parametersObject != null)
            {
                IEnumerable<PropertyInfo> properties;
#if DOTNET5_1
                var type = parametersObject.GetType().GetTypeInfo();
                properties = type.DeclaredProperties.Where(p=> p.CanRead);
#else
                properties = parametersObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
#endif

                foreach (var propinfo in properties)
                {
                    template.SetParameter(propinfo.Name, propinfo.GetValue(parametersObject, null));
                }
            }

            return template;
        }
        public static UriTemplate AddParameters(this UriTemplate uriTemplate, IDictionary<string, object> linkParameters)
        {
            if (linkParameters != null)
            {
                foreach (var parameter in linkParameters)
                {
                    uriTemplate.SetParameter(parameter.Key, parameter.Value);
                }
            }
            return uriTemplate;
        }
    }

    public static class UriExtensions
    {
        public static UriTemplate MakeTemplate(this Uri uri)
        {
            var parameters = uri.GetQueryStringParameters();
            return MakeTemplate(uri, parameters);

        }

        public static UriTemplate MakeTemplate(this Uri uri, IDictionary<string, object> parameters)
        {
            var target = uri.GetComponents(UriComponents.AbsoluteUri
                                                     & ~UriComponents.Query
                                                     & ~UriComponents.Fragment, UriFormat.Unescaped);
            var template = new UriTemplate(target + "{?" + string.Join(",", parameters.Keys.ToArray()) + "}");
            template.AddParameters(parameters);

            return template;
        }

        public static Dictionary<string, object> GetQueryStringParameters(this Uri target)
        {
            Uri uri = target;
            var parameters = new Dictionary<string, object>();

            var reg = new Regex(@"([-A-Za-z0-9._~]*)=([^&]*)&?");		// Unreserved characters: http://tools.ietf.org/html/rfc3986#section-2.3
            foreach (Match m in reg.Matches(uri.Query))
            {
                string key = m.Groups[1].Value.ToLowerInvariant();
                string value = m.Groups[2].Value;
                parameters.Add(key, value);
            }
            return parameters;
        }


    }
}
