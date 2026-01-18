using System;
using System.Drawing;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using ZXing;
using ZXing.Rendering;

/// <summary>
/// Creation of the QR Code
/// The ZXing.Net package is required and can be installed via the NuGet console/tool
/// 
/// Création du CodeQR
/// Le package ZXing.Net est nécessaire et peut être installé via la console/outil NuGet
/// </summary>
/// 
namespace VanillaTwist.MEV
{
    public class UtilesQRCode
    {
        /// <summary>
        /// Creation of the QR Code
        /// Création du CodeQR
        /// </summary>
        /// <param name="Data">Data to be encoded in the QR code
        ///                    Données à encoder dans le CodeQR</param>
        /// <param name="Height">Image height
        ///                      Hauteur de l'image</param>
        /// <param name="Width">Image width
        ///                     Largeur de l'image</param>
        /// <param name="Margin">Margin (white space) around the QR code
        ///                      Marge (espace blanc) autour du codeQR</param>
        /// <returns>Image containing the QR code to be printed or displayed
        ///          Image contenant le codeQR à imprimer ou afficher</returns>
        public static Image GetQRCode( String Data, int Height, int Width, int Margin )
        {
            ZXing.Windows.Compatibility.BarcodeWriter qrcoder = new ZXing.Windows.Compatibility.BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.QrCode.QrCodeEncodingOptions
                {
                    ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.L, // 7% le minimum
                    Height = Height,
                    Width = Width,
                    Margin = Margin
                }
            };

            return new Bitmap( qrcoder.Write( Data ) );
        }

        /// <summary>
        /// Creation of the hyperlink to be encoded in the QR code
        /// Création du lien hypertexte à encoder dans le code QR
        /// </summary>
        /// <param name="transaction">Transaction to be used to create the hyperlink
        ///                           Transaction à utiliser pour créer le lien hypertexte</param>
        /// <param name="CertificateSerialNumberWebSRM">Serial number of the WEB-SRM certificate to extract its public key
        ///                                             Numéro de série  du certificat du MEV-WEB afin d'extraire sa clef publique</param>
        /// <returns>Hyperlink to be encoded in the QR code
        ///          Lien hypertexte à encoder dans le code QR</returns>
        public static String CreateUrlQRCode( Transaction transaction, String CertificateSerialNumberWebSRM )
        {
            StringBuilder texteConcatener = new StringBuilder( );
            texteConcatener.Append( transaction.EmprCertifSEV );
            texteConcatener.Append( transaction.DatTrans );
            texteConcatener.Append( transaction.TPS );
            texteConcatener.Append( transaction.TVQ );
            texteConcatener.Append( transaction.ApresTax );
            texteConcatener.Append( transaction.MTDU );
            texteConcatener.Append( transaction.NoTPS );
            texteConcatener.Append( transaction.NoTVQ );
            texteConcatener.Append( transaction.ModImpr );
            texteConcatener.Append( transaction.ModTrans );
            texteConcatener.Append( transaction.signa.Actu );
            texteConcatener.Append( transaction.signa.Preced );
            texteConcatener.Append( transaction.NoTrans.PadLeft( 10, '=' ) );

            // Reading the public key contained in the WEB-SRM's certificate
            // Lecture de la clef publique contenue dans le certificat du MEV-WEB
            RSA PublicKeyWEBSRM = Utiles.GetRSAPublicKeyWEBSRM( CertificateSerialNumberWebSRM );

            // Encrypting information
            // Chiffrement des informations
            byte[ ] bUrlChiffree = PublicKeyWEBSRM.Encrypt( Encoding.UTF8.GetBytes( texteConcatener.ToString( ) ), RSAEncryptionPadding.Pkcs1 );

            // Base64 encoding
            // Encodage en Base64
            String strUrlChiffreeBase64 = Convert.ToBase64String( bUrlChiffree, Base64FormattingOptions.None );

            // Url safe encoding and return the hyperlink
            // Encodage sûr de l'URL et retour de l'hyperlien
            return ValeursPossibles.UrlMEVWEB.QRCode + WebUtility.UrlEncode( strUrlChiffreeBase64 );
        }
    }
}
