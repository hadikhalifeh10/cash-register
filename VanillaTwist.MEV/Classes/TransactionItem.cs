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
    /// Le champ identifiant une ou plusieurs fournitures payées ou payables.
    /// Field identifying one or more supplies that have been paid or are payable.
    /// </summary>
    public class TransactionItem
    {
        /// <summary>
        /// La quantité de chaque fourniture payée ou payable.
        /// Quantity of each supply paid or payable.
        /// </summary>
        public String Qte { get; set; }

        /// <summary>
        /// La description de chaque fourniture payée ou payable.
        /// Description of each supply paid or payable.
        /// </summary>
        public String Descr { get; set; }

        /// <summary>
        /// Le prix unitaire de la fourniture payée ou payable.
        /// The unit price paid or payable in respect of the supply.
        /// </summary>
        public String Unitr { get; set; }

        /// <summary>
        /// La valeur de la contrepartie payée ou payable à l’égard de la fourniture. Le prix doit correspondre à la quantité multiplié par le prix unitaire.
        /// Value of the consderation paid or payable for the supply. The price must correspond to the quantity multiplied by the unit price.
        /// </summary>
        public String Prix { get; set; }

        /// <summary>
        /// Un indicateur que la taxe s’applique pour chaque fourniture payée ou payable.
        /// Indicator that tax applies to each supply paid or payable.
        /// </summary>
        public String Tax { get; set; }

        /// <summary>
        /// L’abréviation du sous secteur d’activité de l’item.
        /// Abbreviation for the activity subsector of the item.
        /// </summary>
        public String Acti { get; set; }

        /// <summary>
        /// Liste des précisions pour un item d'une transaction.
        /// List of details for an item in a transaction.
        /// </summary>
        public List<TransactionPrecisionItem> LstPrecisions;

        /// <summary>
        /// Le champ identifiant une ou plusieurs fournitures payées ou payables.
        /// Field identifying one or more supplies that have been paid or are payable.
        /// </summary>
        /// <param name="Qte">La quantité de chaque fourniture payée ou payable.
        ///                   Quantity of each supply paid or payable.</param>
        /// <param name="Descr">La description de chaque fourniture payée ou payable.
        ///                     Description of each supply paid or payable.</param>
        /// <param name="Unitr">Le prix unitaire de la fourniture payée ou payable.
        ///                     The unit price paid or payable in respect of the supply.</param>
        /// <param name="Prix">La valeur de la contrepartie payée ou payable à l’égard de la fourniture. Le prix doit correspondre à la quantité multiplié par le prix unitaire.
        ///                    Value of the consderation paid or payable for the supply. The price must correspond to the quantity multiplied by the unit price.</param>
        ///                    Value of the consderation paid or payable for the supply. The price must correspond to the quantity multiplied by the unit price.</param>
        /// <param name="Tax">Un indicateur que la taxe s’applique pour chaque fourniture payée ou payable.
        ///                   Indicator that tax applies to each supply paid or payable.</param>
        /// <param name="Acti">L’abréviation du sous secteur d’activité de l’item.
        ///                    Abbreviation for the activity subsector of the item.</param>
        /// <param name="LstPrecisionsRecues">Liste des précisions pour un item d'une transaction.
        ///                                   List of details for an item in a transaction.</param>
        public TransactionItem( String Qte, String Descr, String Unitr, String Prix, String Tax, String Acti, List<TransactionPrecisionItem> LstPrecisionsRecues )
        {
            this.Qte = Qte;
            this.Descr = Descr;
            this.Unitr = Unitr;
            this.Prix = Prix;
            this.Tax = Tax;
            this.Acti = Acti;
            this.LstPrecisions = new List<TransactionPrecisionItem>( LstPrecisionsRecues );
        }
        public TransactionItem( String Qte, String Descr, String Unitr, String Prix, String Tax, String Acti )
        {
            this.Qte = Qte;
            this.Descr = Descr;
            this.Unitr = Unitr;
            this.Prix = Prix;
            this.Tax = Tax;
            this.Acti = Acti;
            this.LstPrecisions = null;
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

            if( !String.IsNullOrEmpty( Qte ) )
                s.AppendFormat( "\"qte\": \"{0}\",", Qte );

            if( !String.IsNullOrEmpty( Descr ) )
                s.AppendFormat( "\"descr\": \"{0}\",", Descr );

            if( !String.IsNullOrEmpty( Unitr ) )
                s.AppendFormat( "\"unitr\": \"{0}\",", Unitr );

            if( !String.IsNullOrEmpty( Prix ) )
                s.AppendFormat( "\"prix\": \"{0}\",", Prix );

            if( !String.IsNullOrEmpty( Tax ) )
                s.AppendFormat( "\"tax\": \"{0}\",", Tax );

            // TODO Valider gestion du null
            if( !String.IsNullOrEmpty( Acti.Trim( ) ) )
                if( LstPrecisions != null )
                {
                    if( LstPrecisions.Count == 0 )
                        s.AppendFormat( "\"acti\": \"{0}\"", Acti );
                    else
                        s.AppendFormat( "\"acti\": \"{0}\",", Acti );
                }
                else
                    s.AppendFormat( "\"acti\": \"{0}\"", Acti );

            // Précisions
            if( LstPrecisions != null )
            {
                if( LstPrecisions.Count > 0 )
                {
                    s.Append( "\"preci\": [" );

                    foreach( TransactionPrecisionItem PI in LstPrecisions )
                    {
                        s.Append( PI.GetJson( ) );
                    }

                    if( s.ToString( ).Trim( ).EndsWith( "," ) )
                        s.Remove( s.ToString( ).LastIndexOf( "}" ) + 1, 1 );

                    s.Append( "]," );
                }
            }

            if( s.ToString( ).Trim( ).EndsWith( "," ) )
                s.Remove( s.ToString( ).LastIndexOf( "]" ) + 1, 1 );

            s.Append( "}" );

            return s.ToString( );
        }
    }
}
