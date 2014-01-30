﻿using MarkerMetro.Unity.WinLegacy.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkerMetro.Unity.WinLegacy.Net
{
    public class WebRequest // : System.Net.WebRequest
    {
        readonly System.Net.WebRequest _actual;

        private WebRequest(System.Net.WebRequest actaul)
        {
            if (actaul == null)
                throw new ArgumentNullException("actaul", "actaul is null.");

            _actual = actaul;
        }

        public NameValueCollection Headers
        {
            get
            {   // TODO: This will not work as setting values should set values in collection below:
                return new WebHeaderCollection(_actual.Headers);
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        // TODO: Add Headers somehow?
        public static WebRequest Create(string requestUriString)
        {
            var actual = System.Net.WebRequest.Create(requestUriString);

            return new WebRequest(actual);
        }

        public static WebRequest Create(Uri requestUri)
        {
            var actual = System.Net.WebRequest.Create(requestUri);

            return new WebRequest(actual);
        }

        int _timeout;   // TODO: Set default value

        public virtual int Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                if (value < -1)
                    throw new ArgumentOutOfRangeException("value");

                _timeout = value;
            }
        }
    }
}
