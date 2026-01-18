/*
Copyright 2022 Revenu Québec

Permission is hereby granted, free of charge, to any person obtaining a copy of this
software and associated documentation files (the "Software"), to deal in the Software
without restriction, including without limitation the rights to use, copy, modify,
merge, publish, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Net.Http;

namespace VanillaTwist.MEV
{
    /// <summary>
    /// Processing the response received from the WEB-SRM<br/>
    /// Traitement de la réponse reçue du MEV-WEB
    /// </summary>
    public class WEBSRM_Response
    {
        /// <summary>
        /// HTTP response status code
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Description of HTTP status returned
        /// </summary>
        public String StatusDescription { get; set; }

        /// <summary>
        /// Contains the json document received from the WEB-SRM<br/>
        /// Contient le document json reçu du MEVWEB
        /// </summary>
        public String ResponseJsonWEBSRM { get; set; }

        /// <summary>
        /// Exception caused by a communication problem with the WEB-SRM and managed by the SEV<br/>
        /// Exception provoquée par un problème de communication avec le MEV-WEB et gérée par le SEV
        /// </summary>
        public Exception ResponseExceptionWEBSRM { get; set; }

        /// <summary>
        /// HTTP response from WEB-SRM containing success or error codes<br/>
        /// Réponse HTTP du MEV-WEB contenant les codes de succès ou d'erreurs
        /// </summary>
        public HttpResponseMessage HttpResponseMessageWEBSRM { get; set; }

        /// <summary>
        /// Constructeur
        /// </summary>
        public WEBSRM_Response( ) { }

    }
}
