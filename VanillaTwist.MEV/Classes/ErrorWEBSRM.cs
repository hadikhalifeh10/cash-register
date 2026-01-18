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
    /// Erreur reçue du MEV-WEB
    /// Error received from the WEB-SRM
    /// </summary>
    public class ErrorWEBSRM
    {
        /// <summary>
        /// L’identifiant du message d’erreur.
        /// Error message identifier.
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// Le message d’erreur reçu du MEV-WEB
        /// Error message returned from the WEB-SRM
        /// </summary>
        public String Mess { get; set; }

        /// <summary>
        /// Le code de retour du message d’erreur
        /// Return code of the error message
        /// </summary>
        public String CodRetour { get; set; }

        /// <summary>
        /// Le numéro de la transaction contenant une erreur dans le cas d'une requête Transaction
        /// The number of the transaction containing an error in the case of a Transaction request
        /// </summary>
        public String NoTrans { get; set; }

        /// <summary>
        /// Erreur reçue du MEVWEB
        /// </summary>
        /// <param name="Id">L’identifiant du message d’erreur.
        ///                  Error message identifier.</param>
        /// <param name="Mess">Le message d’erreur reçu du MEV-WEB. 
        ///                    Error message returned from the WEB-SRM</param>
        /// <param name="CodRetour">Le code de retour du message d’erreur. 
        ///                         Return code of the error message</param>
        /// <param name="NoTrans">Le numéro de la transaction contenant une erreur dans le cas d'une requête Transaction.
        ///                       The number of the transaction containing an error in the case of a Transaction request</param>
        public ErrorWEBSRM( String Id, String Mess, String CodRetour, String NoTrans )
        {
            this.Id = Id;
            this.Mess = Mess;
            this.CodRetour = CodRetour;
            this.NoTrans = NoTrans;
        }
    }
}
