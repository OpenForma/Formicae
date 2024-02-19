﻿using Formicae.Templates;
using Grasshopper.Kernel;
using Rhino;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace Formicae.Helpers.API
{
    public abstract class GH_Component_HTTPAsync : GH_Component
    {
        protected string _response = "";
        protected bool _shouldExpire = false;
        protected RequestState _currentState = RequestState.Off;

        public GH_Component_HTTPAsync(string name, string nickname, string description, string category, string subcategory)
    : base(name, nickname, description, category, subcategory)
        {
        }

        protected override void ExpireDownStreamObjects()
        {
            if (_shouldExpire)
            {
                base.ExpireDownStreamObjects();
            }
        }

        protected void POSTAsync(
            string url,
            string body,
            string contentType,
            string authorization,
            int timeout)
        {
            Task.Run(() =>
            {
                try
                {
                    // Compose the request
                    byte[] data = Encoding.ASCII.GetBytes(body);

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "POST";
                    request.ContentType = contentType;
                    request.ContentLength = data.Length;
                    request.Timeout = timeout;

                    // Handle authorization
                    if (authorization != null && authorization.Length > 0)
                    {
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; //the auth type

                        request.PreAuthenticate = true;
                        request.Headers.Add("Authorization", authorization);
                    }
                    else
                    {
                        request.Credentials = CredentialCache.DefaultCredentials;
                    }

                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                    var res = request.GetResponse();
                    _response = new StreamReader(res.GetResponseStream()).ReadToEnd();

                    _currentState = RequestState.Done;

                    _shouldExpire = true;
                    RhinoApp.InvokeOnUiThread((Action)delegate { ExpireSolution(true); });
                }
                catch (Exception ex)
                {
                    _response = ex.Message;

                    _currentState = RequestState.Error;

                    _shouldExpire = true;
                    RhinoApp.InvokeOnUiThread((Action)delegate { ExpireSolution(true); });

                    return;
                }
            });
        }


        protected void GETAsync(
            string url,
            string authorization,
            int timeout)
        {
            Task.Run(() =>
            {
                try
                {
                    // Compose the request
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "GET";
                    request.Timeout = timeout;

                    // Handle authorization
                    if (authorization != null && authorization.Length > 0)
                    {
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; //the auth type

                        request.PreAuthenticate = true;
                        request.Headers.Add("Authorization", authorization);
                    }
                    else
                    {
                        request.Credentials = CredentialCache.DefaultCredentials;
                    }

                    var res = request.GetResponse();
                    _response = new StreamReader(res.GetResponseStream()).ReadToEnd();

                    _currentState = RequestState.Done;

                    _shouldExpire = true;
                    RhinoApp.InvokeOnUiThread((Action)delegate { ExpireSolution(true); });
                }
                catch (Exception ex)
                {
                    _response = ex.Message;

                    _currentState = RequestState.Error;

                    _shouldExpire = true;
                    RhinoApp.InvokeOnUiThread((Action)delegate { ExpireSolution(true); });

                    return;
                }
            });
        }
    }
}