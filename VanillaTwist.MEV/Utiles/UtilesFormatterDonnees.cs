using System;

namespace VanillaTwist.MEV
{
    public class UtilesFormatterDonnees
    {
        /*
        Exemple d'utilisation des fonctions de cette classe :

        Console.WriteLine( UtilesFormatterDonnees.FormatterQte( " 109.55    " ) );
        +00109.55

        Console.WriteLine( UtilesFormatterDonnees.FormatterQte( ( decimal )-9.55 ) );
        -00009.55

        Console.WriteLine( UtilesFormatterDonnees.FormatterMontant( "      +09.55               " ) );
        +000000009.55
  
        Console.WriteLine( UtilesFormatterDonnees.FormatterMontant( ( decimal )-0009.56 ) );
        -000000009.56

         */

        #region NoTabl
        /// <summary>
        /// Formatter le champ qte selon le format XXXXX rempli à gauche avec =<br/>
        /// Format the noTabl field as XXXXX padded left with =
        /// </summary>
        /// <param name="noTabl">Champ noTabl</param>
        /// <returns>Champ noTabl au format XXXXX</returns>
        public static String FormatterNoTabl( String noTabl )
        {
            if( noTabl.Length > 5 )
                throw new FormatException( "noTabl doit avoir 5 caractères max" );

            try
            {
                return noTabl.PadLeft( 5, '=' );
            }
            catch( FormatException e )
            {
                throw new FormatException( e.Message );
            }
        }
        #endregion NoTabl

        #region NbClint
        /// <summary>
        /// Formatter le champ NbClint selon le format 000<br/>
        /// Format the NbClint field as 000
        /// </summary>
        /// <param name="NbClint">Champ NbClint</param>
        /// <returns>Champ qte au format 000</returns>
        public static String FormatterNbClint( String NbClint )
        {
            try
            {
                return String.Format( "{0:000}", Convert.ToDecimal( NbClint ) );
            }
            catch( FormatException e )
            {
                throw new FormatException( e.Message );
            }
        }

        #endregion NbClient

        #region Qte
        /// <summary>
        /// Formatter le champ qte selon le format +/-00000.00<br/>
        /// Format the qte field as +/-00000.00
        /// </summary>
        /// <param name="Qte">Champ qte</param>
        /// <returns>Champ qte au format +/-00000.00</returns>
        public static String FormatterQte( String Qte )
        {
            try
            {
                if( Qte.Trim( ).StartsWith( "-" ) )
                    return String.Format( "{0:00000.00}", Convert.ToDecimal( Qte.Replace( ".", "," ) ) ).Replace( ",", "." );
                else
                    return String.Format( "{0:+00000.00}", Convert.ToDecimal( Qte.Replace( ".", "," ) ) ).Replace( ",", "." );
            }
            catch( FormatException e )
            {
                throw new FormatException( e.Message );
            }
        }

        /// <summary>
        /// Formatter les champs qte, unitr selon le format +/-00000.00<br/>
        /// Format the champs qte, unitr fields as +/-00000.00
        /// </summary>
        /// <param name="Qte">Champ qte</param>
        /// <returns>Champ qte au format +/-00000.00</returns>
        public static String FormatterQte( decimal Qte )
        {
            try
            {
                if( Qte < 0 )
                    return String.Format( "{0:00000.00}", Qte ).Replace( ",", "." );
                else
                    return String.Format( "{0:+00000.00}", Qte ).Replace( ",", "." );
            }
            catch( FormatException e )
            {
                throw new FormatException( e.Message );
            }
        }
        #endregion Qte

        #region Montants unitr, prix, avantTax, etc.
        /// <summary>
        /// Formatter les champs (unitr, prix, avantTax, etc.) contenant un montant $ selon le format +/-000000000.00<br/>
        /// Format the fields (unitr, price, beforeTax, etc.) containing a $ amount as +/-000000000.00
        /// </summary>
        /// <param name="Montant">Champ prix</param>
        /// <returns>Champ au format +/-000000000.00</returns>
        public static String FormatterMontant( String Montant )
        {
            try
            {
                if( Montant.Trim( ).StartsWith( "-" ) )
                    return String.Format( "{0:000000000.00}", Convert.ToDecimal( Montant.Replace( ".", "," ) ) ).Replace( ",", "." );
                else
                    return String.Format( "{0:+000000000.00}", Convert.ToDecimal( Montant.Replace( ".", "," ) ) ).Replace( ",", "." );
            }
            catch( FormatException e )
            {
                throw new FormatException( e.Message );
            }
        }

        /// <summary>
        /// Formatter les champs (unitr, prix, avantTax, etc.) contenant un montant $ selon le format +/-000000000.00<br/>
        /// Format the fields (unitr, price, beforeTax, etc.) containing a $ amount as +/-000000000.00
        /// </summary>
        /// <param name="Montant">Champ prix</param>
        /// <returns>Champ au format +/-000000000.00</returns>
        public static String FormatterMontant( decimal Montant )
        {
            try
            {
                if( Montant < 0 )
                    return String.Format( "{0:000000000.00}", Montant ).Replace( ",", "." );
                else
                    return String.Format( "{0:+000000000.00}", Montant ).Replace( ",", "." );
            }
            catch( FormatException e )
            {
                throw new FormatException( e.Message );
            }
        }
        #endregion Montants unitr, prix, avantTax, etc.

        #region Dates Heures
        /// <summary>
        /// Formatter les champs Date heure (datTrans) selon le format AAAAMMJJhhmmss<br/>
        /// Format the fields Date and Time (datTrans) as AAAAMMDDhhmmss
        /// <code>
        /// String FormattedDateTime = UtilesFormatterDonnees.GetDateHeureFormatté( DateTime.Now )
        /// </code>
        /// </summary>
        /// <param name="dateTime">Champ à formatter </param>
        /// <returns>Date et heure formattées au format AAAAMMJJhhmmss</returns>
        public static String GetDateHeureFormatté( DateTime dateTime )
        {
            try
            {
                return dateTime.ToString( "yyyyMMddHHmmss" );
            }
            catch( Exception e )
            {
                throw new Exception( e.Message );
            }
        }

        #endregion Dates Heures
    }
}
