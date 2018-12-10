using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
//using System.Net.Configuration;
using System.Net.Mail;
using System.Web;
//using System.Web.Configuration;
using TrackMED;

namespace CITT.Sinks.Email
{
    public class CredentialManager
    {
        private readonly Settings_Email _settings;
        public static string smtp;
        public static int port;

        // https://docs.asp.net/en/latest/mvc/controllers/dependency-injection.html#accessing-settings-from-a-controller

        public CredentialManager(IOptions<Settings_Email> optionsAccessor)
        {
            _settings = optionsAccessor.Value; // reads appsettings.json

            smtp = _settings.Host;
            port = _settings.Port;
        }
    }
}