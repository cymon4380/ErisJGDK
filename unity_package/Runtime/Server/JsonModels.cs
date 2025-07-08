using System;
using System.Collections.Generic;

namespace ErisJGDK.Base
{
    [Serializable]
    public class Request
    {
        public string key;
        public Dictionary<string, object> val;
        public string[] recipients;
        public string[] ignore;
    }

    [Serializable]
    public class WSResponse
    {
        public string key;

#nullable enable
        public Dictionary<string, object>? val;
        public bool? ok;
        public string? error;
#nullable disable
    }

    [Serializable]
    public class HttpResponse
    {
        public bool ok;

#nullable enable
        public Dictionary<string, object>? body;
        public string? error;
        public int? httpCode;
#nullable disable
    }
}