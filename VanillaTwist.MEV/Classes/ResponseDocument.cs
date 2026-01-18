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
    /// Informations reçues du MEV-WEB lors d'une requête de type Document
    /// Information received from WEB-SRM during a request of type Document
    /// </summary>
    public class ResponseDocument
    {
        /// <summary>
        /// Version de la structure JSON contenue dans la réponse
        /// Version of the JSON structure contained in the response
        /// </summary>
        public String JsonVersi { get; set; }

        /// <summary>
        /// Numéro du prochain cas d'essais
        /// Next test case number that you must execute
        /// </summary>
        public String ProchainCasEssai { get; set; }

        /// <summary>
        /// Numéro unique du document
        /// Unique document number
        /// </summary>
        public String NoDoc { get; set; }

        /// <summary>
        /// Réponse du MEV-WEB suite à la transmission d'une transaction
        /// Information received from WEB-SRM during a request of type Transaction
        /// </summary>
        /// <param name="ProchainCasEssai">Numéro du prochain cas d'essais
        ///                                Next test case number that you must execute</param>
        /// <param name="NoDoc">Numéro unique du document
        ///                     Unique document number</param>
        public ResponseDocument(String JsonVersi, String ProchainCasEssai, String NoDoc)
        {
            this.JsonVersi = JsonVersi;
            this.ProchainCasEssai = ProchainCasEssai;
            this.NoDoc = NoDoc;
        }
    }
}
