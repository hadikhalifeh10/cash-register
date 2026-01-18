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
using System.Text;

namespace VanillaTwist.MEV
{
    /// <summary>
    /// Le champ identifiant une ou plusieurs références à des transactions précédentes.
    /// Field identifying one or more references to preceding transactions.
    /// </summary>
    public class TransactionReference
    {
        /// <summary>
        /// Le montant avant taxes pour la transaction auquel la facture fait référence.
        /// Amount before taxes for the transaction to which the bill refers.
        /// </summary>
        public String AvantTax { get; set; }

        /// <summary>
        /// Le moment de la transaction auxquelles la facture fait référence (date, heure, minute et seconde).
        /// Time of the transaction to which the bill refers ( date, hour, minute and second).
        /// </summary>
        public String DatTrans { get; set; }

        /// <summary>
        /// Le numéro qui identifie la transaction auquel le document fait référence.
        /// Number identifying the transaction to which the document refers.
        /// </summary>
        public String NoTrans { get; set; }

        /// <summary>
        /// Le champ identifiant une ou plusieurs références à des transactions précédentes.
        /// Field identifying one or more references to preceding transactions.
        /// </summary>
        /// <param name="noTrans">Le numéro qui identifie la transaction auquel le document fait référence.
        ///                       Number identifying the transaction to which the document refers.</param>
        /// <param name="datTrans">Le moment de la transaction auxquelles la facture fait référence (date, heure, minute et seconde).
        ///                        Time of the transaction to which the bill refers ( date, hour, minute and second).</param>
        /// <param name="avantTax">Le montant avant taxes pour la transaction auquel la facture fait référence.
        ///                        Amount before taxes for the transaction to which the bill refers.</param>
        public TransactionReference( String noTrans, String datTrans, String avantTax )
        {
            this.NoTrans = noTrans;
            this.DatTrans = datTrans;
            this.AvantTax = avantTax;
        }

        /// <summary>
        /// Retourne le contenu dans un document json
        /// Returns the content in a json document
        /// </summary>
        /// <returns>json document</returns>
        public String GetJson( )
        {
            StringBuilder s = new StringBuilder( );
            s.Append( "{" );
            s.AppendFormat( "\"noTrans\": \"{0}\",", NoTrans );
            s.AppendFormat( "\"datTrans\": \"{0}\",", DatTrans );
            s.AppendFormat( "\"avantTax\": \"{0}\"", AvantTax );
            s.Append( "}" );

            return s.ToString( );
        }

    }
}
