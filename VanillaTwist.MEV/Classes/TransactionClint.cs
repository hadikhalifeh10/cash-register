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
using System.Collections.Generic;
using System.Text;

namespace VanillaTwist.MEV
{
    /// <summary>
    /// Les renseignements relatifs au client qui acquiert la fourniture payé ou payable.
    /// Information regarding the custumer that acquires a paid or payable supply.
    /// </summary>
    public class TransactionClint
    {
        /// <summary>
        /// Le nom du client.
        /// Customer’s name.
        /// </summary>
        public String NomClint { get; set; }

        /// <summary>
        /// Le numéro d’inscription au fichier de la TVQ attribué au client.
        /// Customerst’s QST registration number.
        /// </summary>
        public String NoTvqClint { get; set; }

        /// <summary>
        /// Le numéro de téléphone principal du client.
        /// Customer’s main telephone number.
        /// </summary>
        public String Tel1 { get; set; }

        /// <summary>
        /// Le numéro de téléphone secondaire du client.
        /// Customer’s secondary telephone number.
        /// </summary>
        public String Tel2 { get; set; }

        /// <summary>
        /// L’adresse courriel du client.
        /// Customer’s email address.
        /// </summary>
        public String Courl { get; set; }

        /// <summary>
        /// Liste des adresses du client.
        /// List of customer's addresses.
        /// </summary>
        public List<TransactionAdrClint> lstAdrClients = new List<TransactionAdrClint>( );

        public TransactionClint( ) { }

        /// <summary>
        /// Les renseignements relatifs au client qui acquiert la fourniture payé ou payable.
        /// Information regarding the custumer that acquires a paid or payable supply.
        /// </summary>
        /// <param name="NomClint">Le nom du client.
        ///                        Customer’s name.</param>
        /// <param name="NoTvqClint">Le numéro d’inscription au fichier de la TVQ attribué au client.
        ///                          Customerst’s QST registration number.</param>
        /// <param name="Tel1">Le numéro de téléphone principal du client.
        ///                    Customer’s main telephone number.</param>
        /// <param name="Tel2">Le numéro de téléphone secondaire du client.
        ///                    Customer’s secondary telephone number.</param>
        /// <param name="Courl">L’adresse courriel du client.
        ///                     Customer’s email address.</param>
        /// <param name="lstAdrClients">Liste des adresses du client.
        ///                             List of customer's addresses.</param>
        public TransactionClint( String NomClint, String NoTvqClint, String Tel1, String Tel2, String Courl, List<TransactionAdrClint> lstAdrClients )
        {
            this.NomClint = NomClint;
            this.NoTvqClint = NoTvqClint;
            this.Tel1 = Tel1;
            this.Tel2 = Tel2;
            this.Courl = Courl;
            this.lstAdrClients = new List<TransactionAdrClint>( lstAdrClients );
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

            if( !String.IsNullOrEmpty( NomClint ) )
                s.AppendFormat( "\"nomClint\": \"{0}\",", NomClint );

            if( !String.IsNullOrEmpty( NoTvqClint ) )
                s.AppendFormat( "\"noTvqClint\": \"{0}\",", NoTvqClint );

            if( !String.IsNullOrEmpty( Tel1 ) )
                s.AppendFormat( "\"tel1\": \"{0}\",", Tel1 );

            if( !String.IsNullOrEmpty( Tel2 ) )
                s.AppendFormat( "\"tel2\": \"{0}\",", Tel2 );

            if( !String.IsNullOrEmpty( Courl ) )
                s.AppendFormat( "\"courl\": \"{0}\",", Courl );

            // balise adr (Adresse Client)
            if( lstAdrClients.Count > 0 )
            {
                s.Append( "\"adr\": [" );

                foreach( TransactionAdrClint TAC in lstAdrClients )
                {
                    s.Append( TAC.GetJson( ) );
                }

                if( s.ToString( ).Trim( ).EndsWith( "," ) )
                    s.Remove( s.ToString( ).LastIndexOf( "}" ) + 1, 1 );

                s.Append( "]," );
            }

            // Enlève la virgule après le dernier
            if( s.ToString( ).Trim( ).EndsWith( "," ) )
                s.Remove( s.ToString( ).LastIndexOf( "]" ) + 1, 1 );

            s.Append( "}" );

            return s.ToString( );

        }
    }
}
