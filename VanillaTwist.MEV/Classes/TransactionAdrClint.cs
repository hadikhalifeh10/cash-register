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
    /// Le champ identifiant une ou plusieurs adresses du client.
    /// Transaction's field identifying one or more customer addresses
    /// </summary>
    public class TransactionAdrClint
    {
        /// <summary>
        /// Le type d'adresse du client.
        /// Customer’s address type
        /// </summary>
        public String TypAdr { get; set; }

        /// <summary>
        /// Le numéro civique du client.
        /// The customer’s street number
        /// </summary>
        public String NoCiviq { get; set; }

        /// <summary>
        /// La rue du client.
        /// The customer’s street name
        /// </summary>
        public String Rue { get; set; }

        /// <summary>
        /// La ville du client
        /// The customer’s city 
        /// </summary>
        public String Vil { get; set; }

        /// <summary>
        /// Le code postal du client
        /// The customer’s postal code
        /// </summary>
        public String CP { get; set; }

        /// <summary>
        /// Le champ identifiant une ou plusieurs adresses du client.
        /// Transaction's field identifying one or more customer addresses
        /// </summary>
        /// <param name="TypAdr">Le type d'adresse du client.
        ///                      Customer’s address type.</param>
        /// <param name="NoCiviq">Le numéro civique du client.
        ///                       The customer’s street number.</param>
        /// <param name="Rue">La rue du client.
        ///                   The customer’s street name.</param>
        /// <param name="Vil">La ville du client.
        ///                   The customer’s city.</param>
        /// <param name="CP">Le code postal du client.
        ///                  The customer’s postal code</param>
        public TransactionAdrClint( String TypAdr, String NoCiviq, String Rue, String Vil, String CP )
        {
            this.TypAdr = TypAdr;
            this.NoCiviq = NoCiviq;
            this.Rue = Rue;
            this.Vil = Vil;
            this.CP = CP;
        }

        /// <summary>
        /// Retourne le contenu dans un document json
        /// 
        /// Returns the content in a json document
        /// </summary>
        /// <returns>json document</returns>
        public String GetJson( )
        {
            StringBuilder s = new StringBuilder( );

            s.Append( "{" );

            if( !String.IsNullOrEmpty( TypAdr ) )
                s.AppendFormat( "\"typAdr\": \"{0}\",", TypAdr );

            if( !String.IsNullOrEmpty( NoCiviq ) )
                s.AppendFormat( "\"noCiviq\": \"{0}\",", NoCiviq );

            if( !String.IsNullOrEmpty( Rue ) )
                s.AppendFormat( "\"rue\": \"{0}\",", Rue );

            if( !String.IsNullOrEmpty( Vil ) )
                s.AppendFormat( "\"vil\": \"{0}\",", Vil );

            if( !String.IsNullOrEmpty( CP ) )
                s.AppendFormat( "\"cp\": \"{0}\"", CP );

            if( s.ToString( ).Trim( ).EndsWith( "," ) )
                s.Remove( s.ToString( ).LastIndexOf( "," ), 1 );

            s.Append( "}" );

            return s.ToString( );
        }
    }
}
