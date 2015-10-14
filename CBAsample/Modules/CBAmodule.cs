using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;

namespace CBAsample.Modules
{
    public class CBAmodule : IHttpModule
    {
        public void Dispose()
        {
           
        }
        private static void SetPrincipal(IPrincipal principal)
        {
            Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }

        }
        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += Context_AuthenticateRequest;
            context.EndRequest += Context_EndRequest;
        }

        private void Context_EndRequest(object sender, EventArgs e)
        {
            var response = HttpContext.Current.Response;
            if (response.StatusCode== 401)
            {
                response.Headers.Add("WWW-Authenticate", "Basic realm=\"Yaramazlık Yasak!!\"");
            }

        }

        private void Context_AuthenticateRequest(object sender, EventArgs e)
        {
            var req = HttpContext.Current.Request;
            var header = req.Headers["Authorization"];
            if (header != null)
            {
                var parsedValue = AuthenticationHeaderValue.Parse(header);
                if (parsedValue.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase)
                    && parsedValue.Parameter != null )
                {

                    var sonuc=GirisYap(parsedValue.Parameter);
                }
            }
        }

        private bool GirisYap(string parameter)
        {
            bool isvalid = false;
            try
            {
                //latin 1 batı avrupa 
                var cred = Encoding.GetEncoding("iso-8859-1")
                    .GetString(Convert.FromBase64String(parameter));
                var values = cred.Split(':');
                var user = values[0];
                var password = values[1];
                if (user=="mg" && password=="23")
                {
                    isvalid = true;
                }
                if (isvalid)
                {
                    
                    SetPrincipal(new GenericPrincipal(new GenericIdentity(user),null));
                }
               
            }
            catch 
            {
                isvalid = false;

            }
            return isvalid;
        }
    }
}