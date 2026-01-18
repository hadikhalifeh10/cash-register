using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace VanillaTwist.MEV
{
    /// <summary>
    /// Contains static functions for the use of certificates, keys and QR code
    /// Contient des fonctions statiques pour l'utilisation des certificats, clefs et codeQR
    /// </summary>
    public class Utiles
    {
        #region Certificates and keys

        /// <summary>
        /// Reading a certificate from the Windows certificate store
        /// Lecture d'un certificat dans le magasin de certificats de Windows
        /// </summary>
        /// <param name="CertificateSerialNumberSRS">Serial number of the certificate
        ///                                          Numéro de série du certificat</param>
        /// <returns>Certificate</returns>
        public static X509Certificate2 GetCertificate( String CertificateSerialNumberSRS )
        {
            X509Store CertifStore = new X509Store( StoreName.My, StoreLocation.CurrentUser );
            CertifStore.Open( OpenFlags.ReadOnly );
            X509Certificate2Collection certificatesx = CertifStore.Certificates.Find( X509FindType.FindBySerialNumber, CertificateSerialNumberSRS, false );
            X509Certificate2 certificat = null;

            if ( certificatesx.Count > 0 )
                certificat = certificatesx[ 0 ];

            CertifStore.Close( );

            return certificat;
        }

        /// <summary>
        /// Lecture de l'empreinte d'un certificat
        /// Reading the thumbprint of a certificate
        /// </summary>
        /// <param name="CertificateSerialNumberSRS">Serial number of the certificate
        ///                                          Numéro de série du certificat</param>
        /// <returns>Certificate's thumbprint
        ///          Empreinte du certificat</returns>
        public static String GetCertificateThumbprint( String CertificateSerialNumberSRS )
        {
            return GetCertificate( CertificateSerialNumberSRS ).Thumbprint;
        }

        /// <summary>
        /// Returns the RSA public key contained in the WEB-SRM's certificate.
        /// Retourne la clef publique RSA contenue dans le certificat du MEV-WEB
        /// </summary>
        /// <param name="CertificateSerialNumberWebSRM">Serial number of the certificate
        ///                                             Numéro de série du certificat</param>
        /// <returns>Public key
        ///          Clef publique</returns>
        public static RSA GetRSAPublicKeyWEBSRM( String CertificateSerialNumberWebSRM )
        {
            X509Certificate2 certificat = GetCertificate( CertificateSerialNumberWebSRM );
            RSA clePublique = certificat.GetRSAPublicKey( );
            return clePublique;
        }

        /// <summary>
        /// Returns the expiration date of a certificate
        /// Retourne la date d'expiration d'un certificat
        /// </summary>
        /// <param name="CertificateSerialNumber">Serial number of the certificate
        ///                                       Numéro de série du certificat</param>
        /// <returns>Expiration date
        ///          Date d'expiration</returns>
        public static DateTime GetDateExpirationCertificate( String CertificateSerialNumber )
        {
            return GetCertificate( CertificateSerialNumber ).NotAfter;
        }

        /// <summary>
        /// Returns the number of remaining days of validity of a certificate
        /// Retourne le nombre de jours de validité restant d'un certificat
        /// </summary>
        /// <param name="CertificateSerialNumber">Serial number of the certificate
        ///                                       Numéro de série du certificat</param>
        /// <returns>Number of remaining days of validity
        ///          Nombre de jours de validité restant</returns>
        public static int GetRemainingDaysValidityCertificate( String CertificateSerialNumber )
        {
            X509Certificate2 certificat = GetCertificate( CertificateSerialNumber );
            double nbrJours = ( certificat.NotAfter - DateTime.Now ).TotalDays;
            return Convert.ToInt32( nbrJours );
        }

        /// <summary>
        /// Check if the validity of a certificate, according to its NotBefore and NotAfter dates
        /// Vérifier si la validité d'un certificat, selon ses dates NotBefore et NotAfter
        /// </summary>
        /// <param name="CertificateSerialNumber">Serial number of the certificate
        ///                                       Numéro de série du certificat</param>
        /// <returns>true if the current date is between the validity dates of the certificate
        ///          true si la date du jour est entre les dates de validité du certificat</returns>
        public static bool IsCertificatValideDate( String CertificateSerialNumber )
        {
            X509Certificate2 certificat = GetCertificate( CertificateSerialNumber );

            if ( ( DateTime.Now < certificat.NotBefore ) || ( DateTime.Now > certificat.NotAfter ) )
                return false;
            else
                return true;
        }
        #endregion Certificats et clefs

        #region TimeZone
        /// <summary>
        /// Time zone according to Universal Time Coordinated (UTC) - Eastern Daylight Time (A) or Eastern Standard Time (N)
        /// Fuseau horaire selon le Temps Universel Coordonné (UTC) - Heure Avancée de l'Est (A) ou Heure Normale de l'Est (N)
        /// </summary>
        /// <returns>Time zone representation (-04:00A / -04:00N or -05:00A / -05:00N)
        ///          Représentation du fuseau horaire UTC (-04:00A / -04:00N ou -05:00A / -05:00N)</returns>
        public static String GetTimeZoneDaylightStandardTime( )
        {
            if ( TimeZoneInfo.Local.IsDaylightSavingTime( DateTime.Now ) )
                // Heure Avancée, on ajoute un A
                return TimeZoneInfo.Local.BaseUtcOffset.ToString( ).Substring( 0, 6 ) + "A";
            else
                return TimeZoneInfo.Local.BaseUtcOffset.ToString( ).Substring( 0, 6 ) + "N";
        }

        /// <summary>
        /// Time zone according to Universal Time Coordinated (UTC)
        /// Fuseau horaire selon le Temps Universel Coordonné (UTC)
        /// </summary>
        /// <returns>Time zone representation (-04:00 or -05:00)
        ///          Représentation du fuseau horaire UTC (-04:00 ou -05:00)</returns>
        public static String GetTimeZoneHeure( )
        {
            return TimeZoneInfo.Local.BaseUtcOffset.ToString( ).Substring( 0, 6 );
        }
        #endregion TimeZone
    }
}
