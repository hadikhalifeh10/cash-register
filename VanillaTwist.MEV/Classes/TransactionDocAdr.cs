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
    /// Le champ indiquant l’adresse de l’entreprise indiqué sur le document.
    /// The field giving the address of the business named on the document.
    /// </summary>
    public class TransactionDocAdr
    {
        /// <summary>
        /// Le code postal de l’entreprise indiqué sur le document.
        /// The street number of the business named on the document.
        /// </summary>
        public string DocCodePostal { get; set; }

        /// <summary>
        /// Le numéro civique de l’entreprise indiqué sur le document
        /// The postal code of the business named on the document.
        /// </summary>
        public string DocNoCiviq { get; set; }

        /// <summary>
        /// Constructeur
        /// </summary>
        public TransactionDocAdr( ) { }

        /// <summary>
        /// Retourne le contenu dans un document json
        /// Returns the content in a json document
        /// </summary>
        /// <returns>json document</returns>
        public String GetJson( )
        {
            StringBuilder s = new StringBuilder( );

            s.Append( "{" );

            if( !String.IsNullOrEmpty( DocNoCiviq ) )
                s.AppendFormat( "\"docNoCiviq\": \"{0}\",", DocNoCiviq );

            if( !String.IsNullOrEmpty( DocCodePostal ) )
                s.AppendFormat( "\"docCp\": \"{0}\"", DocCodePostal );
            
            s.Append( "}" );

            return s.ToString( );
        }
    }
}
