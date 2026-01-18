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

namespace VanillaTwist.MEV
{
    /// <summary>
    /// Informations reçues du MEV-WEB lors d'une requête de type Certificat : Certificat du SEV et certificat du MEV-WEB.
    /// Information received from WEB-SRM during a request of type Certificate: SEV certificate and WEB-SRM certificate.
    /// </summary>
    public class ResponseCertificats
    {
        /// <summary>
        /// Certificat du MEVWEB ( Balise json certifPSI )
        /// WEB-SRM's Certificate ( json tag certifPSI )
        /// </summary>
        public String CertificatMEVWEB { get; set; }

        /// <summary>
        /// Certificat du SEV ( Balise json certif )
        /// SRS/POS's Certificate ( json tag certif )
        /// </summary>
        public String Certificat { get; set; }

        /// <summary>
        /// Identifiant de l'appareil
        /// Device identifier
        /// </summary>
        public String IdApprl { get; set; }

        /// <summary>
        /// Numéro du prochain cas d'essai
        /// Next test case number that you must execute.
        /// </summary>
        public String ProchainCasEssai { get; set; }

        /// <summary>
        /// Informations reçues du MEV-WEB lors d'une requête de type Certificat : Certificat du SEV et certificat du MEV-WEB.
        /// Information received from WEB-SRM during a request of type Certificate: SEV certificate and WEB-SRM certificate.
        /// </summary>
        /// <param name="ProchainCasEssai">Numéro du prochain cas d'essai
        ///                                Next test case number that you must execute. </param>
        /// <param name="Certif">Certificat du SEV
        ///                      SRS/POS's Certificate</param>
        /// <param name="CertificatMEVWEB">Certificat du MEV-WEB
        ///                                WEB-SRM's Certificate</param>
        /// <param name="IdApprl">id de l'appareil reçu du MEV-WEB
        ///                       Device identifier</param>
        public ResponseCertificats( String ProchainCasEssai, String CertificatMEVWEB, String Certif, String IdApprl )
        {
            this.ProchainCasEssai = ProchainCasEssai;
            this.Certificat = Certif;
            this.CertificatMEVWEB = CertificatMEVWEB;
            this.IdApprl = IdApprl;
        }
    }
}
