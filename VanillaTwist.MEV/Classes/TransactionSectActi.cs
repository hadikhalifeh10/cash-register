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
    /// Le champ identifiant le secteur d’activité de l’entreprise.
    /// Sector of activity
    /// </summary>
    public class TransactionSectActi
    {
        /// <summary>
        /// L’abréviation du secteur d’activité de l’entreprise.
        /// Abbreviation of the business’s sector of activity.
        /// </summary>
        public String Abrvt { get; set; }

        /// <summary>
        /// Nombre de clients.
        /// Number of customers.
        /// </summary>
        public String NbClint { get; set; }

        /// <summary>
        /// Numéro de la table
        /// Table number.
        /// </summary>
        public String NoTabl { get; set; }

        /// <summary>
        /// Type de service.
        /// Service type.
        /// </summary>
        public String TypServ { get; set; }

        /// <summary>
        /// Constructeur
        /// </summary>
        public TransactionSectActi( ) { }

        /// <summary>
        /// Retourne le contenu dans un document json
        /// Returns the content in a json document
        /// </summary>
        /// <returns>json document</returns>
        public String GetJson( )
        {
            StringBuilder s = new StringBuilder( );

            s.Append( "{" );

            if( !String.IsNullOrEmpty( Abrvt ) )
                s.AppendFormat( "\"abrvt\": \"{0}\",", Abrvt );

            if( !String.IsNullOrEmpty( TypServ ) )
                s.AppendFormat( "\"typServ\": \"{0}\",", TypServ );

            if( !String.IsNullOrEmpty( NoTabl ) )
                s.AppendFormat( "\"noTabl\": \"{0}\",", NoTabl );

            if( !String.IsNullOrEmpty( NbClint ) )
                s.AppendFormat( "\"nbClint\": \"{0}\",", NbClint );

            s.Append( "}" );

            return s.ToString( );
        }
    }
}
