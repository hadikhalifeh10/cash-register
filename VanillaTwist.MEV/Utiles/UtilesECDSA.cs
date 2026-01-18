using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace VanillaTwist.MEV
{
    public class UtilesECDSA
    {
        /// <summary>
        /// Creating the key pair and storing the key pair in the Windows Key Store
        /// Création de la paire de clés et stockage de cette paire de clés dans le magasin de clés Windows
        /// 
        /// </summary>
        /// <param name="KeyName">Name of the new key pair (Could be the nickname of the certificate)
        ///                       Nom de la nouvelle paire de clés (Pourrait être le Surnom du certificat)</param>
        /// <param name="DeleteExistingKeys"></param>
        [Obsolete( "KeyPairCreation( String KeyName, bool DeleteExistingKeys) is deprecated, please use KeyPairCreation( String KeyName, String AppInit, bool DeleteExistingKeys = false ) instead." )]
        public static void KeyPairCreation( String KeyName, bool DeleteExistingKeys = false )
        {
            // Configuration of the new key pair
            // Configuration de la nouvelle paire de clés
            CngKeyCreationParameters KeyParameters = new CngKeyCreationParameters
            {
                ExportPolicy = CngExportPolicies.AllowPlaintextExport,
                KeyUsage = CngKeyUsages.Signing
            };

            // Delete the existing key pair with the same name, if necessary
            // Suppression de la paire de clefs existante avec le même nom, au besoin
            if( DeleteExistingKeys && CngKey.Exists( KeyName ) )
                CngKey.Open( KeyName ).Delete( );

            // Creation and storage of the key pair
            // Création et stockage de la paire de clefs
            CngKey.Create( CngAlgorithm.ECDsaP256, KeyName, KeyParameters );
        }

        /// <summary>
        /// Creating the key pair and storing the key pair in the Windows Key Store
        /// Création de la paire de clés et stockage de cette paire de clés dans le magasin de clés Windows
        /// 
        /// </summary>
        /// <param name="KeyName">Name of the new key pair (Could be the nickname of the certificate)
        ///                       Nom de la nouvelle paire de clés (Pourrait être le Surnom du certificat)</param>
        /// <param name="AppInit"></param>
        /// <param name="DeleteExistingKeys"></param>
        public static void KeyPairCreation( String KeyName, String AppInit, bool DeleteExistingKeys = false )
        {
            // Configuration of the new key pair
            // Configuration de la nouvelle paire de clés
            CngKeyCreationParameters KeyParameters = new CngKeyCreationParameters
            {
                KeyUsage = CngKeyUsages.Signing
            };

            // Export policy if the KeyPair if for remote server
            if( AppInit == ValeursPossibles.ApprlInit.SRV )
                KeyParameters.ExportPolicy = CngExportPolicies.AllowPlaintextExport;

            // Delete the existing key pair with the same name, if necessary
            // Suppression de la paire de clefs existante avec le même nom, au besoin
            if( DeleteExistingKeys && CngKey.Exists( KeyName ) )
                CngKey.Open( KeyName ).Delete( );

            // Creation and storage of the key pair
            // Création et stockage de la paire de clefs
            CngKey.Create( CngAlgorithm.ECDsaP256, KeyName, KeyParameters );
        }

        /// <summary>
        /// Preparation of the certificate signing request (CSR)
        /// Préparation de la demande de signature de certificat (CSR)
        /// </summary>
        /// <param name="InfosCertificates">Concatenated information used for the certificate application
        ///                               Informations concaténées utilisées pour la demande de certificat</param>
        /// <param name="NomCle">Nickname of the certificate and keys
        ///                      Surnom du certificat et des clefs</param>
        /// <returns>CSR encoded in Base64
        ///          CSR encodé en Base64</returns>
        public static String CsrEcdsaPreparation( String InfosCertificates, String KeyName )
        {
            CngKey PublicPrivateKeys;

            // Reading the key pair in the key store
            // Lecture de la paire clé dans le magasin de clés
            if( CngKey.Exists( KeyName ) )
                PublicPrivateKeys = CngKey.Open( KeyName );
            else
                throw new Exception( "The key " + KeyName + " does not exist. You have to create it first - La clé " + KeyName + " n'existe pas. Vous devez la créer d'abord." );

            // Creating the CSR
            // Création du CSR
            using( ECDsaCng ecdsa = new ECDsaCng( PublicPrivateKeys ) )
            {
                CertificateRequest req = new CertificateRequest( InfosCertificates, ecdsa, HashAlgorithmName.SHA256 );
                req.CertificateExtensions.Add( new X509BasicConstraintsExtension( false, false, 0, false ) );
                req.CertificateExtensions.Add( new X509EnhancedKeyUsageExtension( new OidCollection { new Oid( "1.3.6.1.5.5.7.3.8" ) }, true ) );
                req.CertificateExtensions.Add( new X509KeyUsageExtension( X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.NonRepudiation, false ) );
                req.CertificateExtensions.Add( new X509SubjectKeyIdentifierExtension( req.PublicKey, false ) );

                return Convert.ToBase64String( req.CreateSigningRequest( ), Base64FormattingOptions.InsertLineBreaks );
            }
        }

        /// <summary>
        /// Generates a digital signature
        /// Génère une signature numérique
        /// </summary>
        /// <param name="TextToSign">Text to be signed
        ///                          Texte à signer</param>
        /// <param name="CertificateSerialNumberSRS">Serial number of the certificate to be used
        ///                                             Numéro de série du certificat à utiliser</param>
        /// <returns>Digital signature
        ///          Signature numérique</returns>
        public static byte[ ] GetSignature( String TextToSign, String CertificateSerialNumberSRS )
        {
            X509Certificate2 cert = Utiles.GetCertificate( CertificateSerialNumberSRS );

            byte[ ] bTextToSign = Encoding.UTF8.GetBytes( TextToSign.Trim( ) );
            using( ECDsa ecdsa = cert.GetECDsaPrivateKey( ) )
            {
                if( ecdsa == null )
                    throw new ArgumentException( "The certificate must have an ECDSA private key. Le certificat doit avoir une clef privée ECDSA.", nameof( cert ) );

                return ecdsa.SignData( bTextToSign, HashAlgorithmName.SHA256 );
            }
        }

        /// <summary>
        /// Validate a digital signature
        /// Valide une signature numérique
        /// </summary>
        /// <param name="Text">Text to be validated
        ///                     Texte à valider</param>
        /// <param name="Signature">Signature to be verified
        ///                         Signature à vérifier</param>
        /// <param name="Certificate">Certificate to be used
        ///                          Certificat à utiliser</param>
        /// <returns>true if the signature is valid, otherwise false
        ///          true si la signature est valide, sinon false</returns>
        public static bool ValiderSignature( String Text, byte[ ] Signature, X509Certificate2 Certificate )
        {
            byte[ ] bText = Encoding.UTF8.GetBytes( Text.Trim( ) );
            using( ECDsa ecdsa = Certificate.GetECDsaPublicKey( ) )
            {
                if( ecdsa == null )
                    throw new ArgumentException( "Vous devez utiliser un certificat ECDSA", nameof( Certificate ) );

                return ecdsa.VerifyData( bText, Signature, HashAlgorithmName.SHA256 );
            }
        }
    }
}