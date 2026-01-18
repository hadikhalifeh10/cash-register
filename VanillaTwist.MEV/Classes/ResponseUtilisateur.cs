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
    /// Informations reçues du MEVWEB lors d'une requête de type Utilisateur
    /// Information received from WEB-SRM during a request of type Transaction
    /// </summary>
    public class ResponseUtilisateur
    {
        /// <summary>
        /// Numéro du prochain cas d'essais
        /// Next test case number that you must execute.
        /// </summary>
        public String ProchainCasEssai { get; set; }

        /// <summary>
        /// Le numéro d’inscription au fichier de la TPS attribué au mandataire
        /// GST registration number of the mandatary
        /// </summary>
        public String NoTPS { get; set; }

        /// <summary>
        /// Le numéro d’inscription au fichier de la TVQ attribué au mandataire
        /// QST registration number of the mandatary
        /// </summary>
        public String NoTVQ { get; set; }

        /// <summary>
        /// Réponse pour une requête de type Utilisateur
        /// </summary>
        /// <param name="ProchainCasEssai">Numéro du prochain cas d'essai
        ///                                Next test case number that you must execute</param>
        /// <param name="NoTPS">Le numéro d’inscription au fichier de la TPS attribué au mandataire
        ///                     GST registration number of the mandatary</param>
        /// <param name="NoTVQ">Le numéro d’inscription au fichier de la TVQ attribué au mandataire
        ///                     QST registration number of the mandatary</param>
        public ResponseUtilisateur( String ProchainCasEssai, String NoTPS, String NoTVQ )
        {
            this.ProchainCasEssai = ProchainCasEssai;
            this.NoTPS = NoTPS;
            this.NoTVQ = NoTVQ;
        }
    }
}
