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
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

#region Commentaires en français
/*
FO_Framework
------------





-- IMPORTANT! --
Vous DEVEZ OBTENIR vos identifiants suivants auprès de Revenu Québec 
pour faire fonctionner la démonstration fournie avec le Framework!

Consultez le document SW-73 pour plus de détails

Identifiant de la version du SEV : IDVERSI = "000000000000xxxx";
Identifiant unique du partenaire : IDPARTN = "000000000000xxxx";
              Identifiant du SEV : IDSEV   = "000000000000xxxx";

             Code d'autorisation : CODAUTH = "X0X0-X0X0";




Démonstration du framework pour les concepteurs de SEV dans le cadre des projets sur la facturation obligatoire.

Le fichier Demo.cs contient des exemples de code qui démontrent les étapes à suivre.

 -  DemoGetCertificates( ); Création d'un certificat pour le SEV et comment le faire signer par le MEV-WEB
    -   Création de la paire de clés (publique et privée)
    -   Création du certificat et demande de signature au MEV-WEB (CSR)
    -   Transmission du CSR au MEV-WEB pour signature du certificat du SEV
    -   Réception des certificats du SEV et du MEV-WEB
        -   Fusion des clés privée et publique dans le certificat du SEV
        -   Sauvegarde des certificats du SEV et du MEV-WEB dans le magasin de certificats Windows

 -  DemoAjouterUtilisateur( ); Démonstration d'une requête Utilisateur - Ajouter un utilisateur sur le MEV-WEB et valider ses numéros de taxes

 -  DemoTransaction( ); Démonstration d'une requête Transaction
    -   Calcul des signatures numériques
    -   Création du code QR
        -   Chiffrement de l'url à encoder dans le code QR avec la clé publique contenue dans le certificat du MEV-WEB

 -  DemoSupprimerUtilisateur( ); Démonstration d'une requête Utilisateur - supprimer un utilisateur sur le MEV-WEB

 -  DemoSupprimerCertificat( ); Démonstration d'une requête Certificat pour supprimer un certificat sur le SEV et sur le MEV-WEB

Packages
--------

Les packages suivants sont utilisés et peuvent être installés via la console NuGet de Visual Studio.

 -  Newtonsoft.Json (Documents json)
 -  ZXing.Net (Code QR)
 -  RestSharp version 106.15.0 (Communication REST avec le MEV-WEB)
    ATTENTION! Le framework n'est PAS compatible avec les versions RestSharp 107.x et suivantes. Vous devez utiliser la version 106.15.0
 */

#endregion Commentaires en français

#region English Comments
/*
FO_Framework
------------





-- IMPORTANT! --
You MUST OBTAIN the following identifiers from Revenu Québec 
to run the demonstration provided

Consult the document SW-73 for more details

Identifier of the SRS's version : IDVERSI = "000000000000xxxx";
    Partner's unique identifier : IDPARTN = "000000000000xxxx";
          Identifier of the SRS : IDSEV   = "000000000000xxxx";
             Authorization code : CODAUTH = "X0X0-X0X0";





Demonstration of the framework for SRS/POS designers.

The Demo.cs file contains sample code that demonstrates the steps to follow.

 -  DemoGetCertificates( ); Creating a certificate for the SRS (sales recording system) /POS (Point Of Sales) and how to get it signed by the WEB-SRM
    -   Keypair creation (public and private)
    -   Certificate creation
    -   Transmission of the Certificate Signing Request (CSR) to the WEB-SRM to get it signed by the WEB-SRM.
    -   Reception of the two certificates : SRS and WEB-SRM
        -   Merge the SRS's private and public keys in the SRS certificate
        -   Save the SRS and WEB-SRM certificates in the Windows certificate store.

 -  DemoAjouterUtilisateur( ); Demonstration of a "USER" request
        -   Add a user on the WEB-SRM

 -  DemoTransaction( ); Demonstration of a "Transaction" request
    -   Digital signature calculation
    -   QR Code creation
        -   Encryption of the url to be encoded in the QR code with the public key contained in the WEB-SRM certificate

 -  DemoSupprimerUtilisateur( ); Demonstration of a "USER" request
        -   Delete a user on the WEB-SRM

 -  DemoSupprimerCertificat( ); Demonstration of a "Certificate" request 
        -   Delete a certificate on the SRS and on the WEB-SRM

Packages
--------

The following packages are used and can be installed with the NuGet console in Visual Studio.

 -  Newtonsoft.Json (json documents)
 -  ZXing.Net (QR Code)
 -  RestSharp version 106.15.0 (REST Communication with the WEB-SRM)
    WARNING! The framework is NOT compatible with RestSharp 107.x and later. You must use version 106.15.0

 */

#endregion English Comments

namespace VanillaTwist.MEV
{
    public class Demo
    {
        #region Configuration

        // IDs and other information / HTTP Headers
        // Identifiants et autres informations / Entêtes HTTP
        private static readonly String _ENVIRN = ValeursPossibles.Envirn.DEV;
        private static readonly String _APPRLINIT = ValeursPossibles.ApprlInit.SEV;
        private static readonly String _SECTACTV = ValeursPossibles.Abrvt.RBC;

        private static String _CASESSAI = "000.000";
        private static String _IDAPPRL = "0000-0000-0000";
        private static readonly String _CODCERTIF = "FOB201999999";

        /*
        -- IMPORTANT! --
        Vous DEVEZ OBTENIR vos identifiants suivants auprès de Revenu Québec pour faire fonctionner la démonstration fournie avec le Framework!
        Consultez le document SW-73 pour plus de détails

        -- IMPORTANT! --
        You MUST OBTAIN the following identifiers from Revenu Québec to run the demonstration provided
        Consult the document SW-73 for more details
        */
        private static readonly String _IDSEV = "00000000000036C4";    // Your ID - Consult the document SW-73 for more details
        private static readonly String _IDVERSI = "00000000000041F6";  // Your ID - Consult the document SW-73 for more details
        private static readonly String _IDPARTN = "0000000000001E18";  // Your ID - Consult the document SW-73 for more details
        private static readonly String _CODAUTH = "D8T8-W8W8";         // Your Authorization code - Consult the document SW-73 for more details

        private static readonly String _VERSI = "1.4";
        private static readonly String _VERSIPARN = "1.0";

        public static readonly bool _Verbose = true;

        // User informations ( not the mandatary )
        // Informations de l'utilisateur ( et non le mandataire )
        /*
        private static String _UserName = "Joe Bleau";
        private static String _noGSTUser = "123456789RT0001";
        private static String _noQSTUser = "5678912340TQ0001";
        private static readonly String _NoDossFO = "ER0001";
         */
        private static String _UserName = "Michel Untel";
        private static String _noGSTUser = "567891234RT0001";
        private static String _noQSTUser = "5678912340TQ0001";
        private static readonly String _NoDossFO = "ER0001";

        #endregion Configuration

        // KeyPair name
        // Nom de la paire de clefs
        private String _KeyPairName = "Certificat A";

        // User-Friendly name for the SRS certificate
        // Nom Convivial pour le certificat du SEV
        private readonly String _SRSCertificatUserFriendlyName = "Certificat A";

        // Serial numbers of the SRS's and the WEB-SRM's certificates
        // Numéros de série des certificats du SEV et du MEV-WEB pour identification
        private String _CertificateSerialNumberSRS = "";
        private String _CertificateSerialNumberWebSRM = "";

        // TODO Commenter
        private String _Signa_Preced = "";

        // Transaction number of the last document
        // Numéro de transaction du dernier document
        private static String _NO = "";

        // Amount after taxes of the last document
        // Montant après taxes du dernier document
        private static String _MT = "";

        // Date & Time when the last document was produced
        // Moment de la production du dernier document
        private static String _DF = "";

        /// <summary>
        /// Demonstration of the steps to do
        /// Démonstration des étapes à effectuer
        /// </summary>
        public Demo( )
        {
            // Creating a certificate for the SRS and how to get it signed by the WEB-SRM
            // La fonction suivante demontre comment créer un certificat du SEV et le faire signer par le MEV-WEB
            if( !DemoGetCertificates( ) )
                Console.WriteLine( "Erreur!" );
            else
            {
                /// Demonstration of the steps required to REPLACE the SRS and WEB-SRM certificates
                /// Démonstration des étapes à faire pour REMPLACER les certificats du SEV et du MEV
                //DemoReplaceCertificate( );

                // Demonstration of a "USER" request - Add a user on the WEB-SRM
                // Démonstration d'une requête Utilisateur - Ajouter un utilisateur sur le MEV-WEB
                DemoAddUser( );

                // Demonstration of a "Transaction" request
                // Démonstration d'une requête Transaction
                DemoTransaction( );

                // Demonstration of a "Batch Transactions" request
                // Démonstration d'une requête Lot de Transactions
                DemoBatchTransactions( );

                // Demonstration of a "Document" request for a User report
                // Démonstration d'une requête Document Rapport de l'Utilisateur
                DemoDocumentRUT( );

                // Demonstration of a "Document" request for a Frequent third party report
                // Démontration d'une requête Document Tiers Habituel
                //DemoDocumentHAB( );

                // Demonstration of a "USER" request - Delete a user on the WEB-SRM
                // Démonstration d'une requête Utilisateur - supprimer un utilisateur sur le MEVWEB
                DemoDeleteUser( );

                // Demonstration of a "Certificate" request - Delete a certificate on the SRS and on the WEB-SRM
                // La fonction suivante demontre comment supprimer un certificat du SEV, sur le SEV et sur le MEV-WEB
                DemoDeleteCertificate( );
            }
        }

        #region Certificates
        /// <summary>
        /// Demonstration of the steps required to obtain the SRS and WEB-SRM certificates
        /// Démonstration des étapes à faire pour obtenir les certificats du SEV et du MEV
        /// </summary>
        private bool DemoGetCertificates( )
        {
            bool isSuccess = false;
            try
            {
                // KeyPair Creation (public and private)
                // Création de la paire de clés (publique et privée) 
                UtilesECDSA.KeyPairCreation( _KeyPairName, _APPRLINIT, true );

                // Creation of the certificate and of the CSR
                // Création du certificat et demande de signature au MEV-WEB
                String CSR = CertificateCreation( );

                // Preparation of the json document to send to the WEB-SRM
                // Préparation du document json pour transmission au MEV-WEB
                String CSRJSon = UtilesJSON.GetJsonCertificate( ValeursPossibles.Modif.AJO, CSR, null );

                // HTTP Headers
                // Entêtes HTTP
                List<KeyValuePair<String, String>> HTTPHeadersDict = GetHTTPHeaders( );

                // Send the CSR to the WEB-SRM and reception of the certificates
                // Envoi du CSR au MEV-WEB et réception des certificats
                WEBSRM_Response WEB_SRM_Response = UtilesJSON.CertificatesRequest( HTTPHeadersDict, CSRJSon );

                // Processing of error messages received from WEB-SRM, if any
                // Traitement des messages d'erreurs reçus du MEV-WEB, s'il y a lieu
                if( WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "listErr" ) )
                {
                    List<ErrorWEBSRM> lstErr = UtilesJSON.ParseErrors( WEB_SRM_Response.ResponseJsonWEBSRM );
                    if( lstErr.Count > 0 )
                        foreach( ErrorWEBSRM er in lstErr )
                        {
                            Console.WriteLine( $"Id : {er.Id}" );
                            Console.WriteLine( $"Message : {er.Mess}" );

                            if( er.CodRetour != "" )
                                Console.WriteLine( $"Code de retour : {er.CodRetour}" );
                        }
                }
                else
                {
                    if( !WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "Error" ) && !WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "délai d'attente" ) && !WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "La connexion sous-jacente a été fermée" ) )
                    {
                        // Extraction of information received from WEB-SRM
                        // Extraction des informations reçues du MEV-WEB
                        ResponseCertificats repCertificat = UtilesJSON.ParseResponseCertificates( WEB_SRM_Response.ResponseJsonWEBSRM );

                        // Device identifier
                        // ID Appareil reçu du MEV-WEB
                        _IDAPPRL = repCertificat.IdApprl;

                        // SRS's Certificate
                        // Certificat du SEV
                        X509Certificate2 certSEV = new X509Certificate2( Convert.FromBase64String( repCertificat.Certificat.Replace( "-----BEGIN CERTIFICATE-----", "" )
                            .Replace( "-----END CERTIFICATE-----", "" ).Replace( Environment.NewLine, "" ) ), "", X509KeyStorageFlags.PersistKeySet )
                        {
                            FriendlyName = _SRSCertificatUserFriendlyName + "-" + _KeyPairName
                        };

                        // Get the private key created above
                        // Récupération de la clef privée créée plus haut
                        CngKey clefs = CngKey.Open( _KeyPairName );
                        ECDsaCng ecdsa = new ECDsaCng( clefs );

                        // Merge the SRS certificate with its private key created above python python hexand save it in the Windows Certificate Store
                        // Fusion du certificat du SEV avec sa clé privée crée ci-dessus 
                        X509Certificate2 pfxCert = certSEV.CopyWithPrivateKey( ecdsa );
                        pfxCert.FriendlyName = _SRSCertificatUserFriendlyName + "-" + _KeyPairName;

                        // Open the Windows Certificate Store and save the certificate in the Windows Certificate Store
                        // Ouverture du magasin de certificats de Windows et sauvegarde dans le magasin de certificats Windows
                        X509Store CertifStore = new X509Store( StoreName.My, StoreLocation.CurrentUser );
                        CertifStore.Open( OpenFlags.ReadWrite );
                        CertifStore.Add( pfxCert );

                        _CertificateSerialNumberSRS = pfxCert.SerialNumber;

                        // WEB-SRM's Certificate
                        // Certificat du MEV-WEB
                        X509Certificate2 certMEVWEB = new X509Certificate2( Convert.FromBase64String( repCertificat.CertificatMEVWEB.Replace( "-----BEGIN CERTIFICATE-----", "" )
                            .Replace( "-----END CERTIFICATE-----", "" ).Replace( Environment.NewLine, "" ) ), "", X509KeyStorageFlags.PersistKeySet )
                        {
                            FriendlyName = "Certificat MEVWEB"
                        };

                        // Adding the WEB-SRM certificate to the Windows certificate store
                        // Ajout du certificat du MEV-WEB dans le magasin de certificats Windows
                        CertifStore.Add( certMEVWEB );
                        _CertificateSerialNumberWebSRM = certMEVWEB.SerialNumber;

                        // Information received from the WEB-SRM
                        // Informations reçues du MEV-WEB
                        if( _Verbose )
                        {
                            Console.WriteLine( $"IdApprl : {_IDAPPRL}" );
                            Console.WriteLine( $"SRS Certificate (Certificat du SEV) :\n{certSEV}" );
                            Console.WriteLine( $"WEB-SRM Certificate (Certificat du MEV-WEB) :\n{certMEVWEB}" );
                        }

                        CertifStore.Close( );
                        isSuccess = true;
                    }
                    else
                        Console.Error.WriteLine( WEB_SRM_Response.ResponseJsonWEBSRM );
                }
            }
            catch( Exception e )
            {
                Console.Error.WriteLine( e );
            }
            return isSuccess;
        }

        /// <summary>
        /// Demonstration of the steps required to REPLACE the SRS and WEB-SRM certificates
        /// Démonstration des étapes à faire pour REMPLACER les certificats du SEV et du MEV
        /// </summary>
        private void DemoReplaceCertificate( )
        {
            try
            {
                // You need to specify a new name for the new key pair to prevent Windows from confusing the old key pair with the new one.
                // Il faut spécifier un nouveau nom pour la nouvelle paire de clefs afin d'éviter que Windows confonde l'ancienne paire de clefs et la nouvelle. 
                _KeyPairName = "Certificat A - New KeyPair";

                // KeyPair Creation (public and private)
                // Création de la paire de clés (publique et privée) 
                UtilesECDSA.KeyPairCreation( _KeyPairName, _APPRLINIT, true );

                // Creation of the certificate and of the CSR
                // Création du certificat et demande de signature au MEV-WEB
                String CSR = CertificateCreation( );

                // Preparation of the json document to send to the WEB-SRM
                // Préparation du document json pour transmission au MEV-WEB
                String CSRJSon = UtilesJSON.GetJsonCertificate( ValeursPossibles.Modif.REM, CSR, _CertificateSerialNumberSRS );

                // HTTP Headers
                // Entêtes HTTP
                List<KeyValuePair<String, String>> HTTPHeadersDict = GetHTTPHeaders( );

                // Send the CSR to the WEB-SRM and reception of the certificates
                // Envoi du CSR au MEV-WEB et réception des certificats
                WEBSRM_Response WEB_SRM_Response = UtilesJSON.CertificatesRequest( ValeursPossibles.Modif.REM, HTTPHeadersDict, CSRJSon, _CertificateSerialNumberSRS );

                // Processing of error messages received from WEB-SRM, if any
                // Traitement des messages d'erreurs reçus du MEV-WEB, s'il y a lieu
                if( WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "listErr" ) )
                {
                    List<ErrorWEBSRM> lstErr = UtilesJSON.ParseErrors( WEB_SRM_Response.ResponseJsonWEBSRM );
                    if( lstErr.Count > 0 )
                        foreach( ErrorWEBSRM er in lstErr )
                        {
                            Console.WriteLine( $"Id : {er.Id}" );
                            Console.WriteLine( $"Message : {er.Mess}" );

                            if( er.CodRetour != "" )
                                Console.WriteLine( $"Code de retour : {er.CodRetour}" );
                        }
                }
                else
                {
                    if( !WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "Error" ) && !WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "délai d'attente" ) && !WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "La connexion sous-jacente a été fermée" ) )
                    {
                        // Extraction of information received from WEB-SRM
                        // Extraction des informations reçues du MEV-WEB
                        ResponseCertificats repCertificat = UtilesJSON.ParseResponseCertificates( WEB_SRM_Response.ResponseJsonWEBSRM );

                        // SRS's Certificate
                        // Certificat du SEV
                        X509Certificate2 certSEV = new X509Certificate2( Convert.FromBase64String( repCertificat.Certificat.Replace( "-----BEGIN CERTIFICATE-----", "" )
                            .Replace( "-----END CERTIFICATE-----", "" ).Replace( Environment.NewLine, "" ) ), "", X509KeyStorageFlags.PersistKeySet )
                        {
                            FriendlyName = _SRSCertificatUserFriendlyName + "-" + _KeyPairName
                        };

                        // Get the private key created above
                        // Récupération de la clef privée créée plus haut
                        CngKey clefs = CngKey.Open( _KeyPairName );
                        ECDsaCng ecdsa = new ECDsaCng( clefs );

                        // Merge the SRS certificate with its private key created above python python hexand save it in the Windows Certificate Store
                        // Fusion du certificat du SEV avec sa clé privée crée ci-dessus 
                        X509Certificate2 pfxCert = certSEV.CopyWithPrivateKey( ecdsa );
                        pfxCert.FriendlyName = _SRSCertificatUserFriendlyName + "-" + _KeyPairName;

                        // Open the Windows Certificate Store and save the certificate in the Windows Certificate Store
                        // Ouverture du magasin de certificats de Windows et sauvegarde dans le magasin de certificats Windows
                        X509Store CertifStore = new X509Store( StoreName.My, StoreLocation.CurrentUser );
                        CertifStore.Open( OpenFlags.ReadWrite );

                        // Remove the old certificate
                        // Suppression de l'ancien certificat
                        CertifStore.Remove( Utiles.GetCertificate( _CertificateSerialNumberSRS ) );

                        CertifStore.Add( pfxCert );

                        _CertificateSerialNumberSRS = pfxCert.SerialNumber;

                        // WEB-SRM's Certificate
                        // Certificat du MEV-WEB
                        X509Certificate2 certMEVWEB = new X509Certificate2( Convert.FromBase64String( repCertificat.CertificatMEVWEB.Replace( "-----BEGIN CERTIFICATE-----", "" )
                            .Replace( "-----END CERTIFICATE-----", "" ).Replace( Environment.NewLine, "" ) ), "", X509KeyStorageFlags.PersistKeySet )
                        {
                            FriendlyName = "Certificat MEVWEB"
                        };

                        // Adding the WEB-SRM certificate to the Windows certificate store
                        // Ajout du certificat du MEV-WEB dans le magasin de certificats Windows
                        CertifStore.Add( certMEVWEB );
                        _CertificateSerialNumberWebSRM = certMEVWEB.SerialNumber;

                        // Information received from the WEB-SRM
                        // Informations reçues du MEV-WEB
                        if( _Verbose )
                        {
                            Console.WriteLine( $"SRS Certificate (Certificat du SEV) :\n{certSEV}" );
                            Console.WriteLine( $"WEB-SRM Certificate (Certificat du MEV-WEB) :\n{certMEVWEB}" );
                        }

                        CertifStore.Close( );
                    }
                    else
                        Console.Error.WriteLine( WEB_SRM_Response.ResponseJsonWEBSRM );
                }
            }
            catch( Exception e )
            {
                Console.Error.WriteLine( e );
            }

        }

        /// <summary>
        /// Demonstration of a Certificate request to delete a certificate on the SRS and on the WEB-SRM
        /// Démonstration des étapes à faire pour supprimer un certificat du SEV et du MEV-WEB
        /// </summary>
        private void DemoDeleteCertificate( )
        {
            try
            {
                _CASESSAI = "000.000";

                // Preparation of the json document to send to the WEB-SRM
                // Préparation du document json pour transmission au MEV-WEB
                String jSon = UtilesJSON.GetJsonCertificate( ValeursPossibles.Modif.SUP, null, _CertificateSerialNumberSRS );

                // HTTP Headers
                // Entêtes HTTP
                List<KeyValuePair<String, String>> HTTPHeadersDicti = GetHTTPHeaders( );

                // Send to the WEB-SRM the request to delete the certificate
                // Envoi de la requête de suppression du certificat au MEV-WEB
                WEBSRM_Response WEB_SRM_Response = UtilesJSON.CertificatesRequest( ValeursPossibles.Modif.SUP, HTTPHeadersDicti, jSon, _CertificateSerialNumberSRS );

                // Processing of error messages received from WEB-SRM, if any
                // Traitement des messages d'erreurs reçus du MEV-WEB, s'il y a lieu
                if( WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "listErr" ) )
                {
                    List<ErrorWEBSRM> lstErr = UtilesJSON.ParseErrors( WEB_SRM_Response.ResponseJsonWEBSRM );
                    if( lstErr.Count > 0 )
                        foreach( ErrorWEBSRM er in lstErr )
                        {
                            Console.WriteLine( $"Id : {er.Id}" );
                            Console.WriteLine( $"Message : {er.Mess}" );
                            Console.WriteLine( $"Code de retour : {er.CodRetour}" );
                        }
                }
                else
                {
                    if( !WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "Error" ) && !WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "délai d'attente" ) )
                    {
                        // Open the Windows certificate store and delete the certificate
                        // Ouverture du magasin de certificats de Windows et suppression du certificat
                        X509Store CertifStore = new X509Store( StoreName.My, StoreLocation.CurrentUser );
                        CertifStore.Open( OpenFlags.ReadWrite );

                        // Remove the certificate
                        // Suppression du certificat
                        CertifStore.Remove( Utiles.GetCertificate( _CertificateSerialNumberSRS ) );

                        CertifStore.Close( );
                        _CertificateSerialNumberSRS = "";
                    }
                    else
                        Console.Error.WriteLine( WEB_SRM_Response.ResponseJsonWEBSRM );
                }
            }
            catch( Exception e )
            {
                Console.Error.WriteLine( e );
            }
        }

        /// <summary>
        /// Creation of the Certificate Signing Request
        /// Création du CSR
        /// </summary>
        /// <returns>CSR à transmettre au MEV-WEB</returns>
        private String CertificateCreation( )
        {
            // The mandatary must enter the mandatary’s identification number
            // Inscrire le numéro d’identification du mandataire
            String CN = "5678912340";

            // The mandatary must enter the abbreviation for the sector of activity and their authorization code separated by a hyphen. Example : AAA-X9X9-X9X9 Organizational unit
            // Inscrire l’abréviation du secteur d’activité et le code d’autorisation du mandataire reliés par un trait d’union. Exemple : AAA-X9X9-X9X9
            String O = _SECTACTV + "-" + _CODAUTH;

            // The mandatary must enter their QST number. Example : 1234567890TQ0001
            // Inscrire le numéro d’inscription au fichier de la TVQ du mandataire.
            String OU = _noQSTUser;

            // The mandatary must enter a user-friendly name for the certificate (8 to 32 characters per the ASCII  The mandatary must enter a user-friendly name for the certificate (8 to 32 characters per the ASCII standards)
            // Inscrire un nom convivial pour le certificat (8 à 32 caractères selon les normes ASCII).
            String SN = "Certificat A";

            // The mandatary must enter their billing file number. Example : AA9999
            // Inscrire le numéro de dossier relatif à la facturation obligatoire. Exemple : AA9999
            String GN = _NoDossFO;

            // The mandatary must enter the municipality's Coordinated Universal Time (UTC). -04:00 or -05:00
            // Inscrire le temps universel coordonné (UTC) de la municipalité. -04:00 ou -05:00
            String L = Utiles.GetTimeZoneHeure( );

            // The mandatary must enter the abbreviation for the province. QC
            // Inscrire l’abréviation de la province. QC
            String S = "QC";

            // The mandatary must enter the abbreviation for the country. CA
            // Inscrire l’abréviation du pays. CA
            String C = "CA";

            try
            {
                // Validations
                UtilesValidation.ValiderCN( CN );
                UtilesValidation.ValiderOU( OU );
                UtilesValidation.ValiderSN( SN );
                UtilesValidation.ValiderGN( GN );
                UtilesValidation.ValiderL( L );
                UtilesValidation.ValiderS( S );
                UtilesValidation.ValiderC( C );

                StringBuilder sbInfoCertificat = new StringBuilder( );
                sbInfoCertificat.AppendFormat( "CN={0};O={1};SN={2};OU={3};GN={4};L={5};S={6};C={7}", CN, O, SN, OU, GN, L, S, C );

                // CSR preparation and encoding in PEM format
                // Préparation du CSR et encodage au format PEM
                String strCSRPEM = UtilesECDSA.CsrEcdsaPreparation( sbInfoCertificat.ToString( ), _KeyPairName );

                return "-----BEGIN CERTIFICATE REQUEST-----\\n" + strCSRPEM.Replace( Environment.NewLine, "" ) + "\\n-----END CERTIFICATE REQUEST-----";
            }
            catch( FormatException fe )
            {
                Console.WriteLine( fe );
            }
            catch( Exception e )
            {
                Console.WriteLine( e );
            }
            return null;
        }

        #endregion Certificates

        #region Users

        /// <summary>
        /// Demonstration of a "USER" request - Add a user on the WEB-SRM
        /// Démonstration de l'ajout d'un utilsateur dans le MEV-WEB
        /// </summary>
        private void DemoAddUser( )
        {
            _UserName = "Michel Untel";
            _noGSTUser = "567891234RT0001";
            _noQSTUser = "5678912340TQ0001";

            // Preparation of the json document to send to the WEB-SRM
            // Préparation du document json pour transmission au MEV-WEB
            String strJSonUser = UtilesJSON.GetJsonUser( ValeursPossibles.Modif.AJO, _UserName, _noGSTUser, _noQSTUser );

            // HTTP Headers
            // Entêtes HTTP
            List<KeyValuePair<String, String>> HTTPHeadersDicti = GetHTTPHeaders( );

            // Send the request to the WEB-SRM
            // Envoi de la requête au MEV-WEB
            WEBSRM_Response WEB_SRM_Response = UtilesJSON.UserRequest( HTTPHeadersDicti, strJSonUser, _CertificateSerialNumberSRS );

            // Processing of error messages received from WEB-SRM, if any
            // Traitement des messages d'erreurs reçus du MEV-WEB, s'il y a lieu
            if( WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "listErr" ) )
            {
                List<ErrorWEBSRM> lstErr = UtilesJSON.ParseErrors( WEB_SRM_Response.ResponseJsonWEBSRM );
                if( lstErr.Count > 0 )
                    foreach( ErrorWEBSRM er in lstErr )
                    {
                        Console.WriteLine( $"Id : {er.Id}" );
                        Console.WriteLine( $"Message : {er.Mess}" );
                        Console.WriteLine( $"Code de retour : {er.CodRetour}" );
                    }
            }
            else
            {
                if( !WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "Error" ) && !WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "délai d'attente" ) && !WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "Impossible de créer un canal sécurisé SSL/TLS." ) )
                {
                    // Get the informations received from the WEB-SRM
                    // Lecture des informations reçues du MEV-WEB
                    ResponseUtilisateur reponseUtilisateur = UtilesJSON.ParseResponseUtilisateur( WEB_SRM_Response.ResponseJsonWEBSRM );

                    // Information received from the WEB-SRM
                    // Informations reçues du MEV-WEB
                    if( _Verbose )
                    {
                        Console.WriteLine( $"Prochain cas d'essai : {reponseUtilisateur.ProchainCasEssai} " );
                        Console.WriteLine( $"Validité NoTPS : {reponseUtilisateur.NoTPS}" );
                        Console.WriteLine( $"Validité NoTVQ : {reponseUtilisateur.NoTVQ}" );
                    }
                }
                else
                {

                    if( WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "Le délai d'attente de l'opération a expiré." ) )
                        Console.WriteLine( "Problème de communication avec le MEV-WEB. Le délai d'attente de l'opération a expiré." );
                    else
                    {
                        if( WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "Impossible de créer un canal sécurisé SSL/TLS." ) )
                            Console.WriteLine( "Problème de certificat pour vous authentifer auprès du MEV-WEB" );
                        else
                            Console.WriteLine( WEB_SRM_Response.ResponseJsonWEBSRM );
                    }
                }
            }
        }

        /// <summary>
        /// Demonstration of a "USER" request - Delete a user on the WEB-SRM
        /// Démonstration de la suppression d'un utilsateur dans le MEV-WEB
        /// </summary>
        private void DemoDeleteUser( )
        {
            _UserName = "Michel Untel";
            _noGSTUser = "567891234RT0001";
            _noQSTUser = "5678912340TQ0001";

            // Preparation of the json document to send to the WEB-SRM
            // Préparation du document json pour transmission au MEV-WEB
            String strJSonUtilisateur = UtilesJSON.GetJsonUser( ValeursPossibles.Modif.SUP, _UserName, _noGSTUser, _noQSTUser );

            // HTTP Headers
            // Entêtes HTTP
            List<KeyValuePair<String, String>> HTTPHeadersDicti = GetHTTPHeaders( );

            // Send the request to the WEB-SRM
            // Envoi de la requête au MEV-WEB
            WEBSRM_Response WEB_SRM_Response = UtilesJSON.UserRequest( HTTPHeadersDicti, strJSonUtilisateur, _CertificateSerialNumberSRS );

            // Processing of error messages received from WEB-SRM, if any
            // Traitement des messages d'erreurs reçus du MEV-WEB, s'il y a lieu
            if( WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "listErr" ) )
            {
                List<ErrorWEBSRM> lstErr = UtilesJSON.ParseErrors( WEB_SRM_Response.ResponseJsonWEBSRM );
                if( lstErr.Count > 0 )
                    foreach( ErrorWEBSRM er in lstErr )
                    {
                        Console.WriteLine( $"Id : {er.Id}" );
                        Console.WriteLine( $"Message : {er.Mess}" );
                        Console.WriteLine( $"Code de retour : {er.CodRetour}" );
                    }
            }
            else
            {
                if( !WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "Error" ) && !WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "délai d'attente" ) && !WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "Impossible de créer un canal sécurisé SSL/TLS." ) )
                {
                    // Get the informations received from the WEB-SRM
                    // Lecture des informations reçues du MEV-WEB
                    ResponseUtilisateur reponseUtilisateur = UtilesJSON.ParseResponseUtilisateur( WEB_SRM_Response.ResponseJsonWEBSRM );

                    // Information received from the WEB-SRM
                    // Informations reçues du MEV-WEB
                    if( _Verbose )
                    {
                        Console.WriteLine( $"Prochain cas d'essai : {reponseUtilisateur.ProchainCasEssai} " );
                        Console.WriteLine( $"Validité NoTPS : {reponseUtilisateur.NoTPS}" );
                        Console.WriteLine( $"Validité NoTVQ : {reponseUtilisateur.NoTVQ}" );
                    }
                }
                else
                {
                    if( WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "Le délai d'attente de l'opération a expiré." ) )
                        Console.WriteLine( "Problème de communication avec le MEV-WEB. Le délai d'attente de l'opération a expiré." );
                    else
                    {
                        if( WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "Impossible de créer un canal sécurisé SSL/TLS." ) )
                            Console.WriteLine( "Problème de certificat pour vous authentifer auprès du MEV-WEB" );
                        else
                            Console.WriteLine( WEB_SRM_Response.ResponseJsonWEBSRM );
                    }
                }
            }
        }

        #endregion Utilisateurs

        #region Transactions

        /// <summary>
        /// Demonstration of a "Transaction" request
        /// Démonstration d'une requête Transaction
        /// </summary>
        private void DemoTransaction( )
        {
            _CASESSAI = "000.000";

            Transaction transActu = GetTransaction_1( );

            // Transaction's json document
            // Document json de la transaction
            String JsonTransac = "{ \"reqTrans\": { \"transActu\": {" + transActu.GetJson( ) + "}}}";

            if( _Verbose )
                Console.WriteLine( UtilesJSON.IndentJson( JsonTransac ) );

            // Send the transaction to the WEB-SRM
            // Transmet la transaction au MEVWEB
            SendTransaction( JsonTransac, transActu );
        }

        /// <summary>
        /// Demonstration of "Batch Transactions" requests
        /// Démonstration de requêtes Lot de Transactions
        /// </summary>
        private void DemoBatchTransactions( )
        {
            _CASESSAI = "000.000";

            String JsonTransac;

            // The transactions to be transmitted in 2 batches (in this demo) are in order 2,3,4,5 and the current transaction 6.
            // Les transactions à transmettre en 2 lots (dans cette demo) sont dans l'ordre 2,3,4,5 et la transaction actuelle 6

            // The transmission will be made with two "transaction" requests
            // La transmission se fera avec deux requêtes "transaction"
            //    transActu[6] + lot[3,2]
            //    lot[5,4]


            // Transaction that will contain the batch of transactions
            // Transaction qui contiendra le lot de transactions
            Transaction transActu = GetTransaction_6( );

            // Transactions that will be transmitted in two batches
            // Transactions qui seront transmises dans deux lots
            List<Transaction> lstTransLot1 = new List<Transaction>
            {
                GetTransaction_2( ),
                GetTransaction_3( ),
            };

            List<Transaction> lstTransLot2 = new List<Transaction>
            {
                GetTransaction_4( ),
                GetTransaction_5( )
            };

            // Transaction's json document
            // Document json de la transaction
            StringBuilder jsonBatch = new StringBuilder( );
            jsonBatch.Append( "{ \"reqTrans\":" );
            jsonBatch.Append( "{" );
            jsonBatch.Append( " \"transActu\":" );
            jsonBatch.Append( "{" );
            jsonBatch.Append( transActu.GetJson( ) );
            jsonBatch.Append( "}" );
            jsonBatch.Append( "," );

            // Indication of the begining of the batch
            // Début du lot de transactions
            jsonBatch.Append( "\"transLot\" : " );
            jsonBatch.Append( "[" );

            // Reverse order of the transactions in the list. The most recent transaction must be the first in the batch.
            // Invsersion de l'ordre des trasanctions à mettre en lot, pour avoir de la plus récente à la plus ancienne.
            lstTransLot1.Reverse( );

            int x = 0;
            foreach( Transaction tr in lstTransLot1.ToArray( ) )
            {
                jsonBatch.Append( "{" );
                jsonBatch.Append( tr.GetJson( ) );
                lstTransLot1.Remove( tr );
                if( lstTransLot1.Count > 0 )
                    jsonBatch.Append( "}," );
                else
                    jsonBatch.Append( "}" );  // Last transaction, no comma ,

                // Precaution to ensure that the batch size does not exceed 256Kb
                // Précaution pour que le lot ne dépasse pas 256Ko
                if( jsonBatch.ToString( ).Length > 250000 )
                    break;

                // We send only two transactions in the first batch. The others ones will be send without a "transActu" later in the next batch (down there)
                // Deux transactions seront transmises dans le premier lot. Les autres transactions seront transmises plus bas dans un lot sans la balise "transActu"
                if( ++x == 2 )
                    break;
            }
            jsonBatch.Append( "]" );
            jsonBatch.Append( "}}" );
            JsonTransac = jsonBatch.ToString( );

            if( _Verbose )
            {
                Console.WriteLine( UtilesJSON.IndentJson( JsonTransac ) );
                Console.WriteLine( $"Taille du json : {jsonBatch.ToString( ).Length}" );
            }

            // Send the transaction and the first part of the batch of transactions
            // Envoi de la transaction et de la première partie du lot de transactions
            SendTransaction( JsonTransac, transActu );

            // The remaining transactions are sent in a second batch
            // Les transactions restantes sont transmises dans un lot
            lstTransLot2.Reverse( );

            // Début du lot de transactions
            // Indication of the begining of the batch
            jsonBatch.Clear( );
            jsonBatch.Append( "{ \"reqTrans\": { \"transLot\" : " ); // There is no "transActu" in this second batch
            jsonBatch.Append( "[" );

            foreach( Transaction tr in lstTransLot2.ToArray( ) )
            {
                jsonBatch.Append( "{" );
                jsonBatch.Append( tr.GetJson( ) );
                lstTransLot2.Remove( tr );
                if( lstTransLot2.Count > 0 )
                    jsonBatch.Append( "}," );
                else
                    jsonBatch.Append( "}" );  // Last transaction, no comma ,

                // Précaution pour que le lot ne dépasse pas 256Ko
                // Precaution to ensure that the batch size does not exceed 256Kb
                if( jsonBatch.ToString( ).Length > 250000 )
                    break;
            }
            jsonBatch.Append( "]" );
            jsonBatch.Append( "}}" );
            JsonTransac = jsonBatch.ToString( );

            if( _Verbose )
            {
                Console.WriteLine( UtilesJSON.IndentJson( JsonTransac ) );
                Console.WriteLine( $"Taille du json : {jsonBatch.ToString( ).Length}" );
            }

            // Send the batch of transactions
            // Envoi du lot de transactions
            SendTransaction( JsonTransac, transActu );
        }

        /// <summary>
        /// Send the json document containing the transactions to the WEB-SRM
        /// </summary>
        /// <param name="jsonToSend"></param>
        /// <param name="transActu"></param>
        private void SendTransaction( String jsonToSend, Transaction transActu )
        {
            // HTTP headers to transmit the transaction to the WEB-SRM
            // Entêtes HTTP pour transmettre la transaction au MEV-WEB
            List<KeyValuePair<String, String>> HTTPHeadersDicti = GetHTTPHeaders( );

            // Digital signature of the request header
            // Signature numérique de l'entête de la requête
            String signatransm = Convert.ToBase64String( UtilesECDSA.GetSignature( String.Concat( _CODAUTH, _IDAPPRL, transActu.signa.Actu ), _CertificateSerialNumberSRS ) );

            // Additional headers
            // Entêtes supplémentaires
            HTTPHeadersDicti.Add( new KeyValuePair<String, String>( "NOTPS", _noGSTUser ) );
            HTTPHeadersDicti.Add( new KeyValuePair<String, String>( "NOTVQ", _noQSTUser ) );
            HTTPHeadersDicti.Add( new KeyValuePair<String, String>( "SIGNATRANSM", signatransm ) );
            HTTPHeadersDicti.Add( new KeyValuePair<String, String>( "EMPRCERTIFTRANSM", transActu.EmprCertifSEV ) );

            // Send the request to the WEB-SRM
            // Transmission de la facture au MEV-WEB
            WEBSRM_Response WEB_SRM_Response = UtilesJSON.TransactionRequest( HTTPHeadersDicti, jsonToSend, _CertificateSerialNumberSRS );

            // Processing of error messages received from WEB-SRM, if any
            // Traitement des messages d'erreurs reçus du MEV-WEB, s'il y a lieu
            if( WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "listErr" ) )
            {
                List<ErrorWEBSRM> lstErr = UtilesJSON.ParseErrors( WEB_SRM_Response.ResponseJsonWEBSRM );
                if( lstErr.Count > 0 )
                    foreach( ErrorWEBSRM er in lstErr )
                    {
                        Console.WriteLine( $"Id : {er.Id}" );
                        Console.WriteLine( $"Message : {er.Mess}" );
                        Console.WriteLine( $"Code de retour : {er.CodRetour}" );
                        Console.WriteLine( $"Numéro de transaction : {er.NoTrans}" );

                        if( er.CodRetour.EndsWith( "0" ) || er.CodRetour.EndsWith( "1" ) || er.CodRetour.EndsWith( "5" ) )
                            Console.WriteLine( $"La transaction numéro {er.NoTrans} n'a pas été transmise. Par conséquent, votre SEV devra la retransmettre dans un lot lors de la prochaine requête de type Transaction." );
                    }
            }
            else
            {
                if( !WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "Error" ) && !WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "délai d'attente" ) && !WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "Impossible de créer un canal sécurisé SSL/TLS." ) )
                {
                    // Get the informations received from the WEB-SRM
                    // Lecture des informations reçues du MEV-WEB
                    ResponseTransaction reponseTransaction = UtilesJSON.ParseResponseTransaction( WEB_SRM_Response.ResponseJsonWEBSRM );

                    // Information received from the WEB-SRM
                    // Informations reçues du MEV-WEB
                    if( _Verbose )
                    {
                        Console.WriteLine( WEB_SRM_Response.ResponseJsonWEBSRM );
                        Console.WriteLine( $"ProchainCasEssai : {reponseTransaction.ProchainCasEssai}" );
                        Console.WriteLine( $"NoTrans : {reponseTransaction.NoTrans}" );
                        Console.WriteLine( $"PsiNoTrans : {reponseTransaction.PsiNoTrans}" );
                        Console.WriteLine( $"PsiDatTrans : {reponseTransaction.PsiDatTrans}" );
                        Console.WriteLine( $"noLot : {reponseTransaction.noLot}" );
                        Console.WriteLine( $"datLot : {reponseTransaction.datLot}" );
                    }

                    // Creation of the URL for the QR Code, with the WEB-SRM certificate to use its public key to encrypt the information
                    // Création de l'URL pour le Code QR, avec le certificat du MEV-WEB pour utiliser sa clef publique pour chiffrer l'information
                    String strUrlQRCode = UtilesQRCode.CreateUrlQRCode( transActu, _CertificateSerialNumberWebSRM );

                    if( _Verbose )
                        Console.WriteLine( strUrlQRCode );

                    // Creation of the image containing the QR code
                    // Création de l'image contenant le code QR
                    System.Drawing.Image ImgCodeQR = UtilesQRCode.GetQRCode( strUrlQRCode, 250, 250, 1 );

                    // Save the CodeQR as .PNG in the system's temporary directory (for the example). You MUST print this QR code on the document given to the customer.
                    // Sauvegarde du CodeQR en .PNG dans le répertoire temporaire du système (pour l'exemple). Vous DEVEZ imprimer ce code QR sur le document remis au client.
                    String NomFichier = System.IO.Path.GetTempPath( ) + "CodeQR.Transaction" + transActu.NoTrans + ".PNG";
                    ImgCodeQR.Save( NomFichier );
                }
                else
                {
                    if( WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "Le délai d'attente de l'opération a expiré." ) )
                        Console.WriteLine( "Problème de communication avec le MEV-WEB. Le délai d'attente de l'opération a expiré." );
                    else
                    {

                        if( WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "Impossible de créer un canal sécurisé SSL/TLS." ) )
                            Console.WriteLine( "Problème de certificat pour vous authentifer auprès du MEV-WEB" );
                        else
                            Console.WriteLine( UtilesJSON.IndentJson( WEB_SRM_Response.ResponseJsonWEBSRM ) );
                    }
                }
            }
        }
        #endregion Transactions

        #region Samples Transactions
        private Transaction GetTransaction_1( )
        {
            // New transaction
            // Nouvelle transaction
            Transaction transac = new Transaction( );

            // Supplies of the transaction
            // Liste des items d'une transaction
            List<TransactionItem> lstItems = new List<TransactionItem>( );

            // Details for a supply of the transaction
            // Liste des précisions sur les items
            List<TransactionPrecisionItem> lstPrecisionsItem = new List<TransactionPrecisionItem>( );

            // Abbreviation of the business's sector of activity (RBC - Restaurants, bars and food trucks)
            // Abbréviation du secteur d'activité Restaurants, bars et camions de restauration (RBC)
            transac.sectActi.Abrvt = ValeursPossibles.Abrvt.RBC;
            // Abbreviation of the business's sector of activity (TRP -  Remunerated Passenger Transportation)
            // Abbréviation du secteur d'activité Transport Rémunéré de personnes (TRP)
            //transac.sectActi.Abrvt = ValeursPossibles.Abrvt.TRP;

            // Number identifying the transaction
            // Numéro de la transaction
            if( UtilesValidation.ValiderNoTrans( "100001" ) )
            {
                transac.NoTrans = "100001";

                // Pour requete Document RUT
                _NO = transac.NoTrans;

            }

            // Name of the mandatory who operates the business
            // Nom du mandataire
            if( UtilesValidation.ValiderNomMandt( "Entreprise ABC" ) )
                transac.NomMandt = "Entreprise ABC";

            // Street number
            // Numéro civique
            if( UtilesValidation.ValiderDocNoCiviq( "3800" ) )
                transac.docAdr.DocNoCiviq = "3800";

            // Postal Code
            // Code Postal
            if( UtilesValidation.ValiderDocCp( "G1X4A5" ) )
                transac.docAdr.DocCodePostal = "G1X4A5";

            // The name of the user who produced the customer's bill
            // Nom de l'utilisateur
            if( UtilesValidation.ValiderNomUtil( _UserName ) )
                transac.NomUtil = _UserName;

            if( transac.sectActi.Abrvt == ValeursPossibles.Abrvt.RBC )
            {
                // Service type
                // Type de service
                transac.sectActi.TypServ = ValeursPossibles.TypServ.LVR;

                // Table number
                // Numéro de table
                transac.sectActi.NoTabl = UtilesFormatterDonnees.FormatterNoTabl( "0" );

                // Number of customers
                // Nombre de clients
                transac.sectActi.NbClint = UtilesFormatterDonnees.FormatterNbClint( "1" );
            }

            // Business relationship
            // Relation d'affaire
            transac.RelaCommer = ValeursPossibles.RelaCommer.B2C;

            // Field identifying one or more customer addresses
            // Le champ identifiant une ou plusieurs adresses du client.
            transac.clint.lstAdrClients.Add( new TransactionAdrClint( ValeursPossibles.TypAdr.EXP, "2535", "Boul. Laurier", "Québec", "G1V5C6" ) );
            transac.clint.lstAdrClients.Add( new TransactionAdrClint( ValeursPossibles.TypAdr.EXP, "3800", "rue Marly", "Québec", "G1X4A5" ) );

            // Date and time (hour, minute and second) of the transaction
            // Date et heure de la transaction
            transac.DatTrans = UtilesFormatterDonnees.GetDateHeureFormatté( DateTime.Now );
            UtilesValidation.ValiderDateHeure( transac.DatTrans );
            _DF = transac.DatTrans; // Pour requête Document

            // Coordinated Universal Time used by the SRS when recording the date transaction ( datTrans )
            // Heure de la transaction selon le temps coordonné (UTC) de la municipalité 
            if( UtilesValidation.ValiderUTC( Utiles.GetTimeZoneDaylightStandardTime( ) ) )
                transac.Utc = Utiles.GetTimeZoneDaylightStandardTime( );

            // Addition of details to a first item
            // Ajout de précisions à un premier item
            if( transac.sectActi.Abrvt == ValeursPossibles.Abrvt.TRP )
            {
                lstPrecisionsItem.Add( new TransactionPrecisionItem( UtilesFormatterDonnees.FormatterQte( "1" ), "Redevance".Trim( ), UtilesFormatterDonnees.FormatterMontant( "0" ), UtilesFormatterDonnees.FormatterMontant( "0.90" ), ValeursPossibles.Tax.FP, ValeursPossibles.Acti.TRP ) );
                lstPrecisionsItem.Add( new TransactionPrecisionItem( UtilesFormatterDonnees.FormatterQte( "2" ), "Redevance2".Trim( ), UtilesFormatterDonnees.FormatterMontant( "2" ), UtilesFormatterDonnees.FormatterMontant( "2.90" ), ValeursPossibles.Tax.FP, ValeursPossibles.Acti.TRP ) );
                lstPrecisionsItem.Add( new TransactionPrecisionItem( UtilesFormatterDonnees.FormatterQte( "3" ), "Redevance3".Trim( ), UtilesFormatterDonnees.FormatterMontant( "3" ), UtilesFormatterDonnees.FormatterMontant( "3.90" ), ValeursPossibles.Tax.FP, ValeursPossibles.Acti.TRP ) );

                // Add item to the transaction
                // Ajout de l'item à la transaction
                lstItems.Add( new TransactionItem( UtilesFormatterDonnees.FormatterQte( "1" ), "Tarif taxi.".Trim( ), UtilesFormatterDonnees.FormatterMontant( "0" ), UtilesFormatterDonnees.FormatterMontant( "19.79" ), ValeursPossibles.Tax.FP, ValeursPossibles.Acti.TRP, lstPrecisionsItem ) );
            }
            else
            {
                //RBC
                lstPrecisionsItem.Add( new TransactionPrecisionItem( UtilesFormatterDonnees.FormatterQte( "1" ), "Extra".Trim( ), UtilesFormatterDonnees.FormatterMontant( "1" ), UtilesFormatterDonnees.FormatterMontant( "0.99" ), ValeursPossibles.Tax.FP, ValeursPossibles.Acti.RES ) );
                lstItems.Add( new TransactionItem( UtilesFormatterDonnees.FormatterQte( "1" ), "Poutine".Trim( ), UtilesFormatterDonnees.FormatterMontant( "0" ), UtilesFormatterDonnees.FormatterMontant( "9.79" ), ValeursPossibles.Tax.FP, ValeursPossibles.Acti.RES, lstPrecisionsItem ) );
            }

            // Empty the list containing the details of the added item, ready for new details of a new item
            // Vider la liste contenant les précisions de l'item ajouté, prêt pour de nouvelles précisions d'un nouvel item
            lstPrecisionsItem.Clear( );

            // Add all the items to the transaction
            // Ajouter tous les items à la transaction
            transac.lstItems = lstItems;

            // Amounts
            // Montants
            transac.AvantTax = UtilesFormatterDonnees.FormatterMontant( "5.00" );
            transac.TPS = UtilesFormatterDonnees.FormatterMontant( "0.25" );
            transac.TVQ = UtilesFormatterDonnees.FormatterMontant( "0.50" );
            transac.ApresTax = UtilesFormatterDonnees.FormatterMontant( "5,75" );
            _MT = transac.ApresTax; // Pour requête Document RUT
            transac.Ajus = UtilesFormatterDonnees.FormatterMontant( "0" );
            transac.MTDU = UtilesFormatterDonnees.FormatterMontant( transac.ApresTax );
            transac.VersActu = UtilesFormatterDonnees.FormatterMontant( "0" );
            transac.VersAnt = UtilesFormatterDonnees.FormatterMontant( "0" );
            transac.Solde = UtilesFormatterDonnees.FormatterMontant( "0" );
            transac.Pourb = UtilesFormatterDonnees.FormatterMontant( "0" );
            transac.NoDossFO = _NoDossFO;

            // Mandatary’s GST and QST registration number (not the user's ones)
            // Numéros de TPS et TVQ du Mandataire (et non ceux de l'utilisateur!)
            if( UtilesValidation.ValiderNoTPS( _noGSTUser ) )
                transac.NoTPS = _noGSTUser;

            if( UtilesValidation.ValiderNoTVQ( _noQSTUser ) )
                transac.NoTVQ = _noQSTUser;

            // Transaction type (Closing receipt)
            // Type de transaction (Reçu de fermeture)
            transac.TypTrans = ValeursPossibles.TypTrans.RFER;

            // E-commerce? No
            // Commerce électronique? Non
            if( UtilesValidation.ValiderCommerceElectronique( "N" ) )
                transac.CommerceElectronique = "N";

            // Method of payment used for the bill (Cash)
            // Mode de paiement (ARGent)
            transac.ModPai = ValeursPossibles.ModPai.ARG;

            // Print mode for the document (bill)
            // Mode d'impression (FACture)
            transac.ModImpr = ValeursPossibles.ModImpr.FAC;

            // Document print option (Paper)
            // Format d'impression (PAPier)
            transac.FormImpr = ValeursPossibles.FormImpr.PAP;

            // Transaction mode (Operation)
            // Mode de transaction (OPErationnel)
            transac.ModTrans = ValeursPossibles.ModTrans.OPE;

            // "Previous signature" of the first transaction
            // "Signature précédente" de la première transaction
            transac.signa.Preced = "========================================================================================";

            // Concatenation of data for digital signature calculation
            // Concaténation des données pour le calcul de la signature numérique
            String strConcateneePourSignature = transac.NoTrans
                + transac.DatTrans
                + transac.TPS
                + transac.TVQ
                + transac.ApresTax
                + transac.NoTPS
                + transac.NoTVQ
                + transac.ModImpr
                + transac.ModTrans
                + transac.signa.Preced;

            // Calculation of the current transaction signature
            // Calcul de la signature de la transaction actuelle
            byte[ ] bytSignatureActu = UtilesECDSA.GetSignature( strConcateneePourSignature, _CertificateSerialNumberSRS );

            // Current transaction digital signature
            // Signature numérique de la transaction actuelle
            transac.signa.Actu = Convert.ToBase64String( bytSignatureActu );
            transac.signa.DatActu = UtilesFormatterDonnees.GetDateHeureFormatté( DateTime.Now );

            // We keep the signature of the current transaction, which will become the previous signature of the next transaction.
            // On conserve la signature de la transaction actuelle qui deviendra la signature précédente de la prochaine transaction
            _Signa_Preced = transac.signa.Actu;

            // Thumbprint of the certificate containing the public key linked to the private key used to produce the current transaction signature
            // L'empreinte numérique dy certificat contenant la dlé publique liée à la clé privée utilisée pour produire la signature de la transaction actuelle.
            transac.EmprCertifSEV = Utiles.GetCertificateThumbprint( _CertificateSerialNumberSRS );

            // Id's
            // Identifiants
            transac.IdSev = _IDSEV;
            transac.IdVersi = _IDVERSI;
            transac.CodCertif = _CODCERTIF;
            transac.IdPartn = _IDPARTN;
            transac.Versi = _VERSI;
            transac.VersiParn = _VERSIPARN;

            return transac;
        }

        private Transaction GetTransaction_2( )
        {
            // New transaction
            // Nouvelle transaction
            Transaction transac = new Transaction( );

            // Supplies of the transaction
            // Liste des items d'une transaction
            List<TransactionItem> lstItems = new List<TransactionItem>( );

            // Details for a supply of the transaction
            // Liste des précisions sur les items
            List<TransactionPrecisionItem> lstPrecisionsItem = new List<TransactionPrecisionItem>( );

            // Abbreviation of the business's sector of activity (RBC - Restaurants, bars and food trucks)
            // Abbréviation du secteur d'activité Restaurants, bars et camions de restauration (RBC)
            transac.sectActi.Abrvt = ValeursPossibles.Abrvt.RBC;
            // Abbreviation of the business's sector of activity (TRP -  Remunerated Passenger Transportation)
            // Abbréviation du secteur d'activité Transport Rémunéré de personnes (TRP)
            //transac.sectActi.Abrvt = ValeursPossibles.Abrvt.TRP;

            // Number identifying the transaction
            // Numéro de la transaction
            if( UtilesValidation.ValiderNoTrans( "100002" ) )
            {
                transac.NoTrans = "100002";

                // Pour requete Document RUT
                _NO = transac.NoTrans;

            }

            // Name of the mandatory who operates the business
            // Nom du mandataire
            if( UtilesValidation.ValiderNomMandt( "Entreprise ABC" ) )
                transac.NomMandt = "Entreprise ABC";

            // Street number
            // Numéro civique
            if( UtilesValidation.ValiderDocNoCiviq( "3800" ) )
                transac.docAdr.DocNoCiviq = "3800";

            // Postal Code
            // Code Postal
            if( UtilesValidation.ValiderDocCp( "G1X4A5" ) )
                transac.docAdr.DocCodePostal = "G1X4A5";

            // The name of the user who produced the customer's bill
            // Nom de l'utilisateur
            if( UtilesValidation.ValiderNomUtil( _UserName ) )
                transac.NomUtil = _UserName;

            // Service type
            // Type de service
            transac.sectActi.TypServ = ValeursPossibles.TypServ.TBL;

            // Table number
            // Numéro de table
            transac.sectActi.NoTabl = UtilesFormatterDonnees.FormatterNoTabl( "12" );

            // Number of customers
            // Nombre de clients
            transac.sectActi.NbClint = UtilesFormatterDonnees.FormatterNbClint( "2" );

            // Business relationship
            // Relation d'affaire
            transac.RelaCommer = ValeursPossibles.RelaCommer.B2C;

            // Field identifying one or more customer addresses
            // Le champ identifiant une ou plusieurs adresses du client.
            transac.clint.lstAdrClients.Add( new TransactionAdrClint( ValeursPossibles.TypAdr.EXP, "3800", "rue Marly", "Québec", "G1X4A5" ) );

            // Date and time (hour, minute and second) of the transaction
            // Date et heure de la transaction
            transac.DatTrans = UtilesFormatterDonnees.GetDateHeureFormatté( DateTime.Now );
            UtilesValidation.ValiderDateHeure( transac.DatTrans );
            _DF = transac.DatTrans; // Pour requête Document

            // Coordinated Universal Time used by the SRS when recording the date transaction ( datTrans )
            // Heure de la transaction selon le temps coordonné (UTC) de la municipalité 
            if( UtilesValidation.ValiderUTC( Utiles.GetTimeZoneDaylightStandardTime( ) ) )
                transac.Utc = Utiles.GetTimeZoneDaylightStandardTime( );

            //RBC
            lstPrecisionsItem.Add( new TransactionPrecisionItem( UtilesFormatterDonnees.FormatterQte( "1" ), "Extra Cheese".Trim( ), UtilesFormatterDonnees.FormatterMontant( "0" ), UtilesFormatterDonnees.FormatterMontant( "4.99" ), ValeursPossibles.Tax.FP, ValeursPossibles.Acti.RES ) );
            lstItems.Add( new TransactionItem( UtilesFormatterDonnees.FormatterQte( "1" ), "Pizza".Trim( ), UtilesFormatterDonnees.FormatterMontant( "0" ), UtilesFormatterDonnees.FormatterMontant( "19.95" ), ValeursPossibles.Tax.FP, ValeursPossibles.Acti.RES, lstPrecisionsItem ) );

            // Empty the list containing the details of the added item, ready for new details of a new item
            // Vider la liste contenant les précisions de l'item ajouté, prêt pour de nouvelles précisions d'un nouvel item
            lstPrecisionsItem.Clear( );

            // Add all the items to the transaction
            // Ajouter tous les items à la transaction
            transac.lstItems = lstItems;

            // Amounts
            // Montants
            transac.AvantTax = UtilesFormatterDonnees.FormatterMontant( "24.94" );
            transac.TPS = UtilesFormatterDonnees.FormatterMontant( "1.25" );
            transac.TVQ = UtilesFormatterDonnees.FormatterMontant( "2.49" );
            transac.ApresTax = UtilesFormatterDonnees.FormatterMontant( "28.68" );
            _MT = transac.ApresTax; // Pour requête Document RUT
            transac.Ajus = UtilesFormatterDonnees.FormatterMontant( "0" );
            transac.MTDU = UtilesFormatterDonnees.FormatterMontant( transac.ApresTax );
            transac.VersActu = UtilesFormatterDonnees.FormatterMontant( "0" );
            transac.VersAnt = UtilesFormatterDonnees.FormatterMontant( "0" );
            transac.Solde = UtilesFormatterDonnees.FormatterMontant( "0" );
            transac.Pourb = UtilesFormatterDonnees.FormatterMontant( "5.00" );
            transac.NoDossFO = _NoDossFO;

            // Mandatary’s GST and QST registration number (not the user's ones)
            // Numéros de TPS et TVQ du Mandataire (et non ceux de l'utilisateur!)
            if( UtilesValidation.ValiderNoTPS( _noGSTUser ) )
                transac.NoTPS = _noGSTUser;

            if( UtilesValidation.ValiderNoTVQ( _noQSTUser ) )
                transac.NoTVQ = _noQSTUser;

            // Transaction type (Closing receipt)
            // Type de transaction (Reçu de fermeture)
            transac.TypTrans = ValeursPossibles.TypTrans.RFER;

            // E-commerce? No
            // Commerce électronique? Non
            if( UtilesValidation.ValiderCommerceElectronique( "N" ) )
                transac.CommerceElectronique = "N";

            // Method of payment used for the bill (Cash)
            // Mode de paiement (ARGent)
            transac.ModPai = ValeursPossibles.ModPai.CRE;

            // Print mode for the document (bill)
            // Mode d'impression (FACture)
            transac.ModImpr = ValeursPossibles.ModImpr.FAC;

            // Document print option (Paper)
            // Format d'impression (PAPier)
            transac.FormImpr = ValeursPossibles.FormImpr.PEL;

            // Transaction mode (Operation)
            // Mode de transaction (OPErationnel)
            transac.ModTrans = ValeursPossibles.ModTrans.OPE;

            // "Previous signature" of the first transaction
            // "Signature précédente" de la première transaction
            transac.signa.Preced = _Signa_Preced;

            // Concatenation of data for digital signature calculation
            // Concaténation des données pour le calcul de la signature numérique
            String strConcateneePourSignature = transac.NoTrans
                + transac.DatTrans
                + transac.TPS
                + transac.TVQ
                + transac.ApresTax
                + transac.NoTPS
                + transac.NoTVQ
                + transac.ModImpr
                + transac.ModTrans
                + transac.signa.Preced;

            // Calculation of the current transaction signature
            // Calcul de la signature de la transaction actuelle
            byte[ ] bytSignatureActu = UtilesECDSA.GetSignature( strConcateneePourSignature, _CertificateSerialNumberSRS );

            // Current transaction digital signature
            // Signature numérique de la transaction actuelle
            transac.signa.Actu = Convert.ToBase64String( bytSignatureActu );
            transac.signa.DatActu = UtilesFormatterDonnees.GetDateHeureFormatté( DateTime.Now );

            //TODO Commenter
            _Signa_Preced = transac.signa.Actu;

            // Thumbprint of the certificate containing the public key linked to the private key used to produce the current transaction signature
            // L'empreinte numérique dy certificat contenant la dlé publique liée à la clé privée utilisée pour produire la signature de la transaction actuelle.
            transac.EmprCertifSEV = Utiles.GetCertificateThumbprint( _CertificateSerialNumberSRS );

            // Id's
            // Identifiants
            transac.IdSev = _IDSEV;
            transac.IdVersi = _IDVERSI;
            transac.CodCertif = _CODCERTIF;
            transac.IdPartn = _IDPARTN;
            transac.Versi = _VERSI;
            transac.VersiParn = _VERSIPARN;

            return transac;
        }

        private Transaction GetTransaction_3( )
        {
            // New transaction
            // Nouvelle transaction
            Transaction transac = new Transaction( );

            // Supplies of the transaction
            // Liste des items d'une transaction
            List<TransactionItem> lstItems = new List<TransactionItem>( );

            // Abbreviation of the business's sector of activity (RBC - Restaurants, bars and food trucks)
            // Abbréviation du secteur d'activité Restaurants, bars et camions de restauration (RBC)
            transac.sectActi.Abrvt = ValeursPossibles.Abrvt.RBC;

            // Number identifying the transaction
            // Numéro de la transaction
            if( UtilesValidation.ValiderNoTrans( "100003" ) )
            {
                transac.NoTrans = "100003";

                // Pour requete Document RUT
                _NO = transac.NoTrans;

            }

            // Name of the mandatory who operates the business
            // Nom du mandataire
            if( UtilesValidation.ValiderNomMandt( "Entreprise ABC" ) )
                transac.NomMandt = "Entreprise ABC";

            // Street number
            // Numéro civique
            if( UtilesValidation.ValiderDocNoCiviq( "3800" ) )
                transac.docAdr.DocNoCiviq = "3800";

            // Postal Code
            // Code Postal
            if( UtilesValidation.ValiderDocCp( "G1X4A5" ) )
                transac.docAdr.DocCodePostal = "G1X4A5";

            // The name of the user who produced the customer's bill
            // Nom de l'utilisateur
            if( UtilesValidation.ValiderNomUtil( _UserName ) )
                transac.NomUtil = _UserName;

            // Service type
            // Type de service
            transac.sectActi.TypServ = ValeursPossibles.TypServ.TBL;

            // Table number
            // Numéro de table
            transac.sectActi.NoTabl = UtilesFormatterDonnees.FormatterNoTabl( "1" );

            // Number of customers
            // Nombre de clients
            transac.sectActi.NbClint = UtilesFormatterDonnees.FormatterNbClint( "1" );

            // Business relationship
            // Relation d'affaire
            transac.RelaCommer = ValeursPossibles.RelaCommer.B2C;

            // Field identifying one or more customer addresses
            // Le champ identifiant une ou plusieurs adresses du client.
            transac.clint.lstAdrClients.Add( new TransactionAdrClint( ValeursPossibles.TypAdr.EXP, "3800", "rue Marly", "Québec", "G1X4A5" ) );

            // Date and time (hour, minute and second) of the transaction
            // Date et heure de la transaction
            transac.DatTrans = UtilesFormatterDonnees.GetDateHeureFormatté( DateTime.Now );
            UtilesValidation.ValiderDateHeure( transac.DatTrans );
            _DF = transac.DatTrans; // Pour requête Document

            // Coordinated Universal Time used by the SRS when recording the date transaction ( datTrans )
            // Heure de la transaction selon le temps coordonné (UTC) de la municipalité 
            if( UtilesValidation.ValiderUTC( Utiles.GetTimeZoneDaylightStandardTime( ) ) )
                transac.Utc = Utiles.GetTimeZoneDaylightStandardTime( );

            // Item without detail
            lstItems.Add( new TransactionItem( UtilesFormatterDonnees.FormatterQte( "1" ), "Coffee".Trim( ), UtilesFormatterDonnees.FormatterMontant( "0" ), UtilesFormatterDonnees.FormatterMontant( "2.95" ), ValeursPossibles.Tax.FP, ValeursPossibles.Acti.RES ) );

            // Add all the items to the transaction
            // Ajouter tous les items à la transaction
            transac.lstItems = lstItems;

            // Amounts
            // Montants
            transac.AvantTax = UtilesFormatterDonnees.FormatterMontant( "2.95" );
            transac.TPS = UtilesFormatterDonnees.FormatterMontant( "0.15" );
            transac.TVQ = UtilesFormatterDonnees.FormatterMontant( "0.29" );
            transac.ApresTax = UtilesFormatterDonnees.FormatterMontant( "3.39" );
            _MT = transac.ApresTax; // Pour requête Document RUT
            transac.Ajus = UtilesFormatterDonnees.FormatterMontant( "0" );
            transac.MTDU = UtilesFormatterDonnees.FormatterMontant( transac.ApresTax );
            transac.VersActu = UtilesFormatterDonnees.FormatterMontant( "0" );
            transac.VersAnt = UtilesFormatterDonnees.FormatterMontant( "0" );
            transac.Solde = UtilesFormatterDonnees.FormatterMontant( "0" );
            transac.Pourb = UtilesFormatterDonnees.FormatterMontant( "1.61" );
            transac.NoDossFO = _NoDossFO;

            // Mandatary’s GST and QST registration number (not the user's ones)
            // Numéros de TPS et TVQ du Mandataire (et non ceux de l'utilisateur!)
            if( UtilesValidation.ValiderNoTPS( _noGSTUser ) )
                transac.NoTPS = _noGSTUser;

            if( UtilesValidation.ValiderNoTVQ( _noQSTUser ) )
                transac.NoTVQ = _noQSTUser;

            // Transaction type (Closing receipt)
            // Type de transaction (Reçu de fermeture)
            transac.TypTrans = ValeursPossibles.TypTrans.RFER;

            // E-commerce? No
            // Commerce électronique? Non
            if( UtilesValidation.ValiderCommerceElectronique( "N" ) )
                transac.CommerceElectronique = "N";

            // Method of payment used for the bill (Cash)
            // Mode de paiement (ARGent)
            transac.ModPai = ValeursPossibles.ModPai.CRE;

            // Print mode for the document (bill)
            // Mode d'impression (FACture)
            transac.ModImpr = ValeursPossibles.ModImpr.FAC;

            // Document print option (Paper)
            // Format d'impression (PAPier)
            transac.FormImpr = ValeursPossibles.FormImpr.PEL;

            // Transaction mode (Operation)
            // Mode de transaction (OPErationnel)
            transac.ModTrans = ValeursPossibles.ModTrans.OPE;

            // "Previous signature" of the first transaction
            // "Signature précédente" de la première transaction
            transac.signa.Preced = _Signa_Preced;

            // Concatenation of data for digital signature calculation
            // Concaténation des données pour le calcul de la signature numérique
            String strConcateneePourSignature = transac.NoTrans
                + transac.DatTrans
                + transac.TPS
                + transac.TVQ
                + transac.ApresTax
                + transac.NoTPS
                + transac.NoTVQ
                + transac.ModImpr
                + transac.ModTrans
                + transac.signa.Preced;

            // Calculation of the current transaction signature
            // Calcul de la signature de la transaction actuelle
            byte[ ] bytSignatureActu = UtilesECDSA.GetSignature( strConcateneePourSignature, _CertificateSerialNumberSRS );

            // Current transaction digital signature
            // Signature numérique de la transaction actuelle
            transac.signa.Actu = Convert.ToBase64String( bytSignatureActu );
            transac.signa.DatActu = UtilesFormatterDonnees.GetDateHeureFormatté( DateTime.Now );

            //TODO Commenter
            _Signa_Preced = transac.signa.Actu;

            // Thumbprint of the certificate containing the public key linked to the private key used to produce the current transaction signature
            // L'empreinte numérique dy certificat contenant la dlé publique liée à la clé privée utilisée pour produire la signature de la transaction actuelle.
            transac.EmprCertifSEV = Utiles.GetCertificateThumbprint( _CertificateSerialNumberSRS );

            // Id's
            // Identifiants
            transac.IdSev = _IDSEV;
            transac.IdVersi = _IDVERSI;
            transac.CodCertif = _CODCERTIF;
            transac.IdPartn = _IDPARTN;
            transac.Versi = _VERSI;
            transac.VersiParn = _VERSIPARN;

            return transac;
        }

        private Transaction GetTransaction_4( )
        {
            // New transaction
            // Nouvelle transaction
            Transaction transac = new Transaction( );

            // Supplies of the transaction
            // Liste des items d'une transaction
            List<TransactionItem> lstItems = new List<TransactionItem>( );

            // Abbreviation of the business's sector of activity (RBC - Restaurants, bars and food trucks)
            // Abbréviation du secteur d'activité Restaurants, bars et camions de restauration (RBC)
            transac.sectActi.Abrvt = ValeursPossibles.Abrvt.RBC;

            // Number identifying the transaction
            // Numéro de la transaction
            if( UtilesValidation.ValiderNoTrans( "100004" ) )
            {
                transac.NoTrans = "100004";

                // Pour requete Document RUT
                _NO = transac.NoTrans;

            }

            // Name of the mandatory who operates the business
            // Nom du mandataire
            if( UtilesValidation.ValiderNomMandt( "Entreprise ABC" ) )
                transac.NomMandt = "Entreprise ABC";

            // Street number
            // Numéro civique
            if( UtilesValidation.ValiderDocNoCiviq( "3800" ) )
                transac.docAdr.DocNoCiviq = "3800";

            // Postal Code
            // Code Postal
            if( UtilesValidation.ValiderDocCp( "G1X4A5" ) )
                transac.docAdr.DocCodePostal = "G1X4A5";

            // The name of the user who produced the customer's bill
            // Nom de l'utilisateur
            if( UtilesValidation.ValiderNomUtil( _UserName ) )
                transac.NomUtil = _UserName;

            // Service type
            // Type de service
            transac.sectActi.TypServ = ValeursPossibles.TypServ.TBL;

            // Table number
            // Numéro de table
            transac.sectActi.NoTabl = UtilesFormatterDonnees.FormatterNoTabl( "1" );

            // Number of customers
            // Nombre de clients
            transac.sectActi.NbClint = UtilesFormatterDonnees.FormatterNbClint( "1" );

            // Business relationship
            // Relation d'affaire
            transac.RelaCommer = ValeursPossibles.RelaCommer.B2C;

            // Field identifying one or more customer addresses
            // Le champ identifiant une ou plusieurs adresses du client.
            transac.clint.lstAdrClients.Add( new TransactionAdrClint( ValeursPossibles.TypAdr.EXP, "3800", "rue Marly", "Québec", "G1X4A5" ) );

            // Date and time (hour, minute and second) of the transaction
            // Date et heure de la transaction
            transac.DatTrans = UtilesFormatterDonnees.GetDateHeureFormatté( DateTime.Now );
            UtilesValidation.ValiderDateHeure( transac.DatTrans );
            _DF = transac.DatTrans; // Pour requête Document

            // Coordinated Universal Time used by the SRS when recording the date transaction ( datTrans )
            // Heure de la transaction selon le temps coordonné (UTC) de la municipalité 
            if( UtilesValidation.ValiderUTC( Utiles.GetTimeZoneDaylightStandardTime( ) ) )
                transac.Utc = Utiles.GetTimeZoneDaylightStandardTime( );

            // Item without detail
            lstItems.Add( new TransactionItem( UtilesFormatterDonnees.FormatterQte( "1" ), "HotDog".Trim( ), UtilesFormatterDonnees.FormatterMontant( "0" ), UtilesFormatterDonnees.FormatterMontant( "4.99" ), ValeursPossibles.Tax.FP, ValeursPossibles.Acti.RES ) );

            // Add all the items to the transaction
            // Ajouter tous les items à la transaction
            transac.lstItems = lstItems;

            // Amounts
            // Montants
            transac.AvantTax = UtilesFormatterDonnees.FormatterMontant( "4.99" );
            transac.TPS = UtilesFormatterDonnees.FormatterMontant( "0.25" );
            transac.TVQ = UtilesFormatterDonnees.FormatterMontant( "0.50" );
            transac.ApresTax = UtilesFormatterDonnees.FormatterMontant( "5.74" );
            _MT = transac.ApresTax; // Pour requête Document RUT
            transac.Ajus = UtilesFormatterDonnees.FormatterMontant( "0" );
            transac.MTDU = UtilesFormatterDonnees.FormatterMontant( transac.ApresTax );
            transac.VersActu = UtilesFormatterDonnees.FormatterMontant( "0" );
            transac.VersAnt = UtilesFormatterDonnees.FormatterMontant( "0" );
            transac.Solde = UtilesFormatterDonnees.FormatterMontant( "0" );
            transac.Pourb = UtilesFormatterDonnees.FormatterMontant( "9.00" );
            transac.NoDossFO = _NoDossFO;

            // Mandatary’s GST and QST registration number (not the user's ones)
            // Numéros de TPS et TVQ du Mandataire (et non ceux de l'utilisateur!)
            if( UtilesValidation.ValiderNoTPS( _noGSTUser ) )
                transac.NoTPS = _noGSTUser;

            if( UtilesValidation.ValiderNoTVQ( _noQSTUser ) )
                transac.NoTVQ = _noQSTUser;

            // Transaction type (Closing receipt)
            // Type de transaction (Reçu de fermeture)
            transac.TypTrans = ValeursPossibles.TypTrans.RFER;

            // E-commerce? No
            // Commerce électronique? Non
            if( UtilesValidation.ValiderCommerceElectronique( "N" ) )
                transac.CommerceElectronique = "N";

            // Method of payment used for the bill (Cash)
            // Mode de paiement (ARGent)
            transac.ModPai = ValeursPossibles.ModPai.CPR;

            // Print mode for the document (bill)
            // Mode d'impression (FACture)
            transac.ModImpr = ValeursPossibles.ModImpr.FAC;

            // Document print option (Paper)
            // Format d'impression (PAPier)
            transac.FormImpr = ValeursPossibles.FormImpr.PEL;

            // Transaction mode (Operation)
            // Mode de transaction (OPErationnel)
            transac.ModTrans = ValeursPossibles.ModTrans.OPE;

            // "Previous signature" of the first transaction
            // "Signature précédente" de la première transaction
            transac.signa.Preced = _Signa_Preced;

            // Concatenation of data for digital signature calculation
            // Concaténation des données pour le calcul de la signature numérique
            String strConcateneePourSignature = transac.NoTrans
                + transac.DatTrans
                + transac.TPS
                + transac.TVQ
                + transac.ApresTax
                + transac.NoTPS
                + transac.NoTVQ
                + transac.ModImpr
                + transac.ModTrans
                + transac.signa.Preced;

            // Calculation of the current transaction signature
            // Calcul de la signature de la transaction actuelle
            byte[ ] bytSignatureActu = UtilesECDSA.GetSignature( strConcateneePourSignature, _CertificateSerialNumberSRS );

            // Current transaction digital signature
            // Signature numérique de la transaction actuelle
            transac.signa.Actu = Convert.ToBase64String( bytSignatureActu );
            transac.signa.DatActu = UtilesFormatterDonnees.GetDateHeureFormatté( DateTime.Now );

            //TODO Commenter
            _Signa_Preced = transac.signa.Actu;

            // Thumbprint of the certificate containing the public key linked to the private key used to produce the current transaction signature
            // L'empreinte numérique dy certificat contenant la dlé publique liée à la clé privée utilisée pour produire la signature de la transaction actuelle.
            transac.EmprCertifSEV = Utiles.GetCertificateThumbprint( _CertificateSerialNumberSRS );

            // Id's
            // Identifiants
            transac.IdSev = _IDSEV;
            transac.IdVersi = _IDVERSI;
            transac.CodCertif = _CODCERTIF;
            transac.IdPartn = _IDPARTN;
            transac.Versi = _VERSI;
            transac.VersiParn = _VERSIPARN;

            return transac;
        }

        private Transaction GetTransaction_5( )
        {
            // New transaction
            // Nouvelle transaction
            Transaction transac = new Transaction( );

            // Supplies of the transaction
            // Liste des items d'une transaction
            List<TransactionItem> lstItems = new List<TransactionItem>( );

            // Abbreviation of the business's sector of activity (RBC - Restaurants, bars and food trucks)
            // Abbréviation du secteur d'activité Restaurants, bars et camions de restauration (RBC)
            transac.sectActi.Abrvt = ValeursPossibles.Abrvt.RBC;

            // Number identifying the transaction
            // Numéro de la transaction
            if( UtilesValidation.ValiderNoTrans( "100005" ) )
            {
                transac.NoTrans = "100005";

                // Pour requete Document RUT
                _NO = transac.NoTrans;

            }

            // Name of the mandatory who operates the business
            // Nom du mandataire
            if( UtilesValidation.ValiderNomMandt( "Entreprise ABC" ) )
                transac.NomMandt = "Entreprise ABC";

            // Street number
            // Numéro civique
            if( UtilesValidation.ValiderDocNoCiviq( "3800" ) )
                transac.docAdr.DocNoCiviq = "3800";

            // Postal Code
            // Code Postal
            if( UtilesValidation.ValiderDocCp( "G1X4A5" ) )
                transac.docAdr.DocCodePostal = "G1X4A5";

            // The name of the user who produced the customer's bill
            // Nom de l'utilisateur
            if( UtilesValidation.ValiderNomUtil( _UserName ) )
                transac.NomUtil = _UserName;

            // Service type
            // Type de service
            transac.sectActi.TypServ = ValeursPossibles.TypServ.CMP;

            // Table number
            // Numéro de table
            transac.sectActi.NoTabl = UtilesFormatterDonnees.FormatterNoTabl( "1" );

            // Number of customers
            // Nombre de clients
            transac.sectActi.NbClint = UtilesFormatterDonnees.FormatterNbClint( "1" );

            // Business relationship
            // Relation d'affaire
            transac.RelaCommer = ValeursPossibles.RelaCommer.B2C;

            // Field identifying one or more customer addresses
            // Le champ identifiant une ou plusieurs adresses du client.
            transac.clint.lstAdrClients.Add( new TransactionAdrClint( ValeursPossibles.TypAdr.EXP, "3800", "rue Marly", "Québec", "G1X4A5" ) );

            // Date and time (hour, minute and second) of the transaction
            // Date et heure de la transaction
            transac.DatTrans = UtilesFormatterDonnees.GetDateHeureFormatté( DateTime.Now );
            UtilesValidation.ValiderDateHeure( transac.DatTrans );
            _DF = transac.DatTrans; // Pour requête Document

            // Coordinated Universal Time used by the SRS when recording the date transaction ( datTrans )
            // Heure de la transaction selon le temps coordonné (UTC) de la municipalité 
            if( UtilesValidation.ValiderUTC( Utiles.GetTimeZoneDaylightStandardTime( ) ) )
                transac.Utc = Utiles.GetTimeZoneDaylightStandardTime( );

            // Item without detail
            lstItems.Add( new TransactionItem( UtilesFormatterDonnees.FormatterQte( "1" ), "Burger".Trim( ), UtilesFormatterDonnees.FormatterMontant( "0" ), UtilesFormatterDonnees.FormatterMontant( "12.95" ), ValeursPossibles.Tax.FP, ValeursPossibles.Acti.RES ) );
            lstItems.Add( new TransactionItem( UtilesFormatterDonnees.FormatterQte( "1" ), "Red Wine".Trim( ), UtilesFormatterDonnees.FormatterMontant( "0" ), UtilesFormatterDonnees.FormatterMontant( "7.95" ), ValeursPossibles.Tax.FP, ValeursPossibles.Acti.RES ) );

            // Add all the items to the transaction
            // Ajouter tous les items à la transaction
            transac.lstItems = lstItems;

            // Amounts
            // Montants
            transac.AvantTax = UtilesFormatterDonnees.FormatterMontant( "20.90" );
            transac.TPS = UtilesFormatterDonnees.FormatterMontant( "1.05" );
            transac.TVQ = UtilesFormatterDonnees.FormatterMontant( "2.08" );
            transac.ApresTax = UtilesFormatterDonnees.FormatterMontant( "24.03" );
            _MT = transac.ApresTax; // Pour requête Document RUT
            transac.Ajus = UtilesFormatterDonnees.FormatterMontant( "0" );
            transac.MTDU = UtilesFormatterDonnees.FormatterMontant( transac.ApresTax );
            transac.VersActu = UtilesFormatterDonnees.FormatterMontant( "0" );
            transac.VersAnt = UtilesFormatterDonnees.FormatterMontant( "0" );
            transac.Solde = UtilesFormatterDonnees.FormatterMontant( "0" );
            transac.Pourb = UtilesFormatterDonnees.FormatterMontant( "4.00" );
            transac.NoDossFO = _NoDossFO;

            // Mandatary’s GST and QST registration number (not the user's ones)
            // Numéros de TPS et TVQ du Mandataire (et non ceux de l'utilisateur!)
            if( UtilesValidation.ValiderNoTPS( _noGSTUser ) )
                transac.NoTPS = _noGSTUser;

            if( UtilesValidation.ValiderNoTVQ( _noQSTUser ) )
                transac.NoTVQ = _noQSTUser;

            // Transaction type (Closing receipt)
            // Type de transaction (Reçu de fermeture)
            transac.TypTrans = ValeursPossibles.TypTrans.RFER;

            // E-commerce? No
            // Commerce électronique? Non
            if( UtilesValidation.ValiderCommerceElectronique( "N" ) )
                transac.CommerceElectronique = "N";

            // Method of payment used for the bill (Cash)
            // Mode de paiement (ARGent)
            transac.ModPai = ValeursPossibles.ModPai.ARG;

            // Print mode for the document (bill)
            // Mode d'impression (FACture)
            transac.ModImpr = ValeursPossibles.ModImpr.FAC;

            // Document print option (Paper)
            // Format d'impression (PAPier)
            transac.FormImpr = ValeursPossibles.FormImpr.ELE;

            // Transaction mode (Operation)
            // Mode de transaction (OPErationnel)
            transac.ModTrans = ValeursPossibles.ModTrans.OPE;

            // "Previous signature" of the first transaction
            // "Signature précédente" de la première transaction
            transac.signa.Preced = _Signa_Preced;

            // Concatenation of data for digital signature calculation
            // Concaténation des données pour le calcul de la signature numérique
            String strConcateneePourSignature = transac.NoTrans
                + transac.DatTrans
                + transac.TPS
                + transac.TVQ
                + transac.ApresTax
                + transac.NoTPS
                + transac.NoTVQ
                + transac.ModImpr
                + transac.ModTrans
                + transac.signa.Preced;

            // Calculation of the current transaction signature
            // Calcul de la signature de la transaction actuelle
            byte[ ] bytSignatureActu = UtilesECDSA.GetSignature( strConcateneePourSignature, _CertificateSerialNumberSRS );

            // Current transaction digital signature
            // Signature numérique de la transaction actuelle
            transac.signa.Actu = Convert.ToBase64String( bytSignatureActu );
            transac.signa.DatActu = UtilesFormatterDonnees.GetDateHeureFormatté( DateTime.Now );

            //TODO Commenter
            _Signa_Preced = transac.signa.Actu;

            // Thumbprint of the certificate containing the public key linked to the private key used to produce the current transaction signature
            // L'empreinte numérique dy certificat contenant la dlé publique liée à la clé privée utilisée pour produire la signature de la transaction actuelle.
            transac.EmprCertifSEV = Utiles.GetCertificateThumbprint( _CertificateSerialNumberSRS );

            // Id's
            // Identifiants
            transac.IdSev = _IDSEV;
            transac.IdVersi = _IDVERSI;
            transac.CodCertif = _CODCERTIF;
            transac.IdPartn = _IDPARTN;
            transac.Versi = _VERSI;
            transac.VersiParn = _VERSIPARN;

            return transac;
        }

        private Transaction GetTransaction_6( )
        {
            // New transaction
            // Nouvelle transaction
            Transaction transac = new Transaction( );

            // Supplies of the transaction
            // Liste des items d'une transaction
            List<TransactionItem> lstItems = new List<TransactionItem>( );

            // Abbreviation of the business's sector of activity (RBC - Restaurants, bars and food trucks)
            // Abbréviation du secteur d'activité Restaurants, bars et camions de restauration (RBC)
            transac.sectActi.Abrvt = ValeursPossibles.Abrvt.RBC;

            // Number identifying the transaction
            // Numéro de la transaction
            if( UtilesValidation.ValiderNoTrans( "100006" ) )
            {
                transac.NoTrans = "100006";

                // Pour requete Document RUT
                _NO = transac.NoTrans;

            }

            // Name of the mandatory who operates the business
            // Nom du mandataire
            if( UtilesValidation.ValiderNomMandt( "Entreprise ABC" ) )
                transac.NomMandt = "Entreprise ABC";

            // Street number
            // Numéro civique
            if( UtilesValidation.ValiderDocNoCiviq( "3800" ) )
                transac.docAdr.DocNoCiviq = "3800";

            // Postal Code
            // Code Postal
            if( UtilesValidation.ValiderDocCp( "G1X4A5" ) )
                transac.docAdr.DocCodePostal = "G1X4A5";

            // The name of the user who produced the customer's bill
            // Nom de l'utilisateur
            if( UtilesValidation.ValiderNomUtil( _UserName ) )
                transac.NomUtil = _UserName;

            // Service type
            // Type de service
            transac.sectActi.TypServ = ValeursPossibles.TypServ.CMP;

            // Table number
            // Numéro de table
            transac.sectActi.NoTabl = UtilesFormatterDonnees.FormatterNoTabl( "4" );

            // Number of customers
            // Nombre de clients
            transac.sectActi.NbClint = UtilesFormatterDonnees.FormatterNbClint( "2" );

            // Business relationship
            // Relation d'affaire
            transac.RelaCommer = ValeursPossibles.RelaCommer.B2C;

            // Field identifying one or more customer addresses
            // Le champ identifiant une ou plusieurs adresses du client.
            transac.clint.lstAdrClients.Add( new TransactionAdrClint( ValeursPossibles.TypAdr.EXP, "3800", "rue Marly", "Québec", "G1X4A5" ) );

            // Date and time (hour, minute and second) of the transaction
            // Date et heure de la transaction
            transac.DatTrans = UtilesFormatterDonnees.GetDateHeureFormatté( DateTime.Now );
            UtilesValidation.ValiderDateHeure( transac.DatTrans );
            _DF = transac.DatTrans; // Pour requête Document

            // Coordinated Universal Time used by the SRS when recording the date transaction ( datTrans )
            // Heure de la transaction selon le temps coordonné (UTC) de la municipalité 
            if( UtilesValidation.ValiderUTC( Utiles.GetTimeZoneDaylightStandardTime( ) ) )
                transac.Utc = Utiles.GetTimeZoneDaylightStandardTime( );

            // Item without detail
            lstItems.Add( new TransactionItem( UtilesFormatterDonnees.FormatterQte( "1" ), "Poutine".Trim( ), UtilesFormatterDonnees.FormatterMontant( "0" ), UtilesFormatterDonnees.FormatterMontant( "19.95" ), ValeursPossibles.Tax.FP, ValeursPossibles.Acti.RES ) );
            lstItems.Add( new TransactionItem( UtilesFormatterDonnees.FormatterQte( "2" ), "Root Beer".Trim( ), UtilesFormatterDonnees.FormatterMontant( "0" ), UtilesFormatterDonnees.FormatterMontant( "2.95" ), ValeursPossibles.Tax.FP, ValeursPossibles.Acti.RES ) );

            // Add all the items to the transaction
            // Ajouter tous les items à la transaction
            transac.lstItems = lstItems;

            // Amounts
            // Montants
            transac.AvantTax = UtilesFormatterDonnees.FormatterMontant( "25.85" );
            transac.TPS = UtilesFormatterDonnees.FormatterMontant( "1.29" );
            transac.TVQ = UtilesFormatterDonnees.FormatterMontant( "2.58" );
            transac.ApresTax = UtilesFormatterDonnees.FormatterMontant( "29.72" );
            _MT = transac.ApresTax; // Pour requête Document RUT
            transac.Ajus = UtilesFormatterDonnees.FormatterMontant( "0" );
            transac.MTDU = UtilesFormatterDonnees.FormatterMontant( transac.ApresTax );
            transac.VersActu = UtilesFormatterDonnees.FormatterMontant( "0" );
            transac.VersAnt = UtilesFormatterDonnees.FormatterMontant( "0" );
            transac.Solde = UtilesFormatterDonnees.FormatterMontant( "0" );
            transac.Pourb = UtilesFormatterDonnees.FormatterMontant( "4.00" );
            transac.NoDossFO = _NoDossFO;

            // Mandatary’s GST and QST registration number (not the user's ones)
            // Numéros de TPS et TVQ du Mandataire (et non ceux de l'utilisateur!)
            if( UtilesValidation.ValiderNoTPS( _noGSTUser ) )
                transac.NoTPS = _noGSTUser;

            if( UtilesValidation.ValiderNoTVQ( _noQSTUser ) )
                transac.NoTVQ = _noQSTUser;

            // Transaction type (Closing receipt)
            // Type de transaction (Reçu de fermeture)
            transac.TypTrans = ValeursPossibles.TypTrans.RFER;

            // E-commerce? No
            // Commerce électronique? Non
            if( UtilesValidation.ValiderCommerceElectronique( "N" ) )
                transac.CommerceElectronique = "N";

            // Method of payment used for the bill (Cash)
            // Mode de paiement (ARGent)
            transac.ModPai = ValeursPossibles.ModPai.CPR;

            // Print mode for the document (bill)
            // Mode d'impression (FACture)
            transac.ModImpr = ValeursPossibles.ModImpr.FAC;

            // Document print option (Paper)
            // Format d'impression (PAPier)
            transac.FormImpr = ValeursPossibles.FormImpr.PAP;

            // Transaction mode (Operation)
            // Mode de transaction (OPErationnel)
            transac.ModTrans = ValeursPossibles.ModTrans.OPE;

            // "Previous signature" of the first transaction
            // "Signature précédente" de la première transaction
            transac.signa.Preced = _Signa_Preced;

            // Concatenation of data for digital signature calculation
            // Concaténation des données pour le calcul de la signature numérique
            String strConcateneePourSignature = transac.NoTrans
                + transac.DatTrans
                + transac.TPS
                + transac.TVQ
                + transac.ApresTax
                + transac.NoTPS
                + transac.NoTVQ
                + transac.ModImpr
                + transac.ModTrans
                + transac.signa.Preced;

            // Calculation of the current transaction signature
            // Calcul de la signature de la transaction actuelle
            byte[ ] bytSignatureActu = UtilesECDSA.GetSignature( strConcateneePourSignature, _CertificateSerialNumberSRS );

            // Current transaction digital signature
            // Signature numérique de la transaction actuelle
            transac.signa.Actu = Convert.ToBase64String( bytSignatureActu );
            transac.signa.DatActu = UtilesFormatterDonnees.GetDateHeureFormatté( DateTime.Now );

            //TODO Commenter
            _Signa_Preced = transac.signa.Actu;

            // Thumbprint of the certificate containing the public key linked to the private key used to produce the current transaction signature
            // L'empreinte numérique dy certificat contenant la dlé publique liée à la clé privée utilisée pour produire la signature de la transaction actuelle.
            transac.EmprCertifSEV = Utiles.GetCertificateThumbprint( _CertificateSerialNumberSRS );

            // Id's
            // Identifiants
            transac.IdSev = _IDSEV;
            transac.IdVersi = _IDVERSI;
            transac.CodCertif = _CODCERTIF;
            transac.IdPartn = _IDPARTN;
            transac.Versi = _VERSI;
            transac.VersiParn = _VERSIPARN;

            return transac;
        }

        #endregion Samples Transactions

        #region DocumentRUT
        /// <summary>
        /// Demonstration of a "Document" request for a User report
        /// Demonstration d'une requête Document Rapport de l'Utilisateur
        /// </summary>
        public void DemoDocumentRUT( )
        {
            DocumentRUT docRUT = new DocumentRUT( )
            {
                RT = _noGSTUser,
                TQ = _noQSTUser,
                UT = _UserName,
                NO = _NO,
                MT = UtilesFormatterDonnees.FormatterMontant( "15" ),
                DF = _DF,
                IA = _IDAPPRL,
                IS = _IDSEV,
                VR = _VERSI,
                DC = UtilesFormatterDonnees.GetDateHeureFormatté( DateTime.Now ),
                DR = UtilesFormatterDonnees.GetDateHeureFormatté( DateTime.Now ),
                SI = "",
                EM = Utiles.GetCertificateThumbprint( _CertificateSerialNumberSRS ),
                AD = "3800, rue de Marly, Québec, G1X4A5"
            };

            // Digital signature of the document
            // Signature numérique du document
            byte[ ] bytSignature = UtilesECDSA.GetSignature( docRUT.GetDocumentConcateneSignature( ), _CertificateSerialNumberSRS );
            docRUT.SI = Convert.ToBase64String( bytSignature );

            // Preparation of the json document for transmission to th WEB-SRM (SW-73-V section 4.3.4.2.1)
            // Préparation du document json pour transmission au MEV-WEB (SW-73 section 4.3.4.2.1)
            String JSon = UtilesJSON.GetJsonDocument( ValeursPossibles.TypDoc.RUT, docRUT.GetDocumentConcateneJSonRequest( ) );

            if( _Verbose )
                Console.WriteLine( UtilesJSON.IndentJson( JSon ) );

            // Adding HTTP headers specific to the Document request
            // Ajout des entêtes HTTP spécifiques à la requête Document
            List<KeyValuePair<String, String>> lstHTTPHeaders = GetHTTPHeaders( );
            lstHTTPHeaders.Add( new KeyValuePair<string, string>( "NOTVQ", _noQSTUser ) );
            lstHTTPHeaders.Add( new KeyValuePair<string, string>( "NOTPS", _noGSTUser ) );

            // Send the request to the WEB-SRM
            // Transmission de la requête au MEV-WEB
            WEBSRM_Response WEB_SRM_Response = UtilesJSON.DocumentRequest( lstHTTPHeaders, JSon, _CertificateSerialNumberSRS );

            // Creation of the image containing the QR code
            // Création de l'image contenant le codeQR
            System.Drawing.Image ImgCodeQR = UtilesQRCode.GetQRCode( docRUT.GetDocumentConcateneQRCode( ), 250, 250, 1 );

            // Save the CodeQR as .PNG in the system's temporary directory (for the example)
            // Sauvegarde du CodeQR en .PNG dans le répertoire temporaire (pour l'exemple)
            String s = System.IO.Path.GetTempPath( ) + "CodeQR.RapportUtilisateur" + UtilesFormatterDonnees.GetDateHeureFormatté( DateTime.Now ) + ".PNG";
            ImgCodeQR.Save( s );

            // Processing of error messages received from WEB-SRM, if any
            // Traitement des messages d'erreurs reçus du MEV-WEB, s'il y a lieu
            if( WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "listErr" ) )
            {
                List<ErrorWEBSRM> lstErr = UtilesJSON.ParseErrors( WEB_SRM_Response.ResponseJsonWEBSRM );
                if( lstErr.Count > 0 )
                    foreach( ErrorWEBSRM er in lstErr )
                    {
                        Console.WriteLine( $"Id : {er.Id}" );
                        Console.WriteLine( $"Message : {er.Mess}" );
                        Console.WriteLine( $"Code de retour : {er.CodRetour}" );
                    }
            }
            else
            {
                if( !WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "Error" ) && !WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "délai d'attente" ) && !WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "Impossible de créer un canal sécurisé SSL/TLS." ) )
                {
                    // Get the informations received from the WEB-SRM
                    // Lecture des informations reçues du MEV-WEB
                    ResponseDocument responseDocument = UtilesJSON.ParseResponseDocument( WEB_SRM_Response.ResponseJsonWEBSRM );

                    // Information received from the WEB-SRM
                    // Informations reçues du MEV-WEB
                    if( _Verbose )
                    {
                        Console.WriteLine( $"Prochain cas d'essai : {responseDocument.ProchainCasEssai} " );
                        Console.WriteLine( $"Numéro du document : {responseDocument.NoDoc} " );
                    }
                }
                else
                {

                    if( WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "Le délai d'attente de l'opération a expiré." ) )
                        Console.WriteLine( "Problème de communication avec le MEV-WEB. Le délai d'attente de l'opération a expiré." );
                    else
                    {
                        if( WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "Impossible de créer un canal sécurisé SSL/TLS." ) )
                            Console.WriteLine( "Problème de certificat pour vous authentifer auprès du MEV-WEB" );
                        else
                            Console.WriteLine( WEB_SRM_Response.ResponseJsonWEBSRM );
                    }
                }
            }
        }
        #endregion DocumentRUT

        #region DocumentHAB
        /// <summary>
        /// Demonstration of a "Document" request for a Frequent third party report
        /// Démonstration d'une requête Document Tiers Habituel
        /// </summary>
        public void DemoDocumentHAB( )
        {
            DocumentHAB docHAB = new DocumentHAB( )
            {
                TQ = "1234567890TQ0001",
                NM = "Joe Bleau",
                AD = "999 rue des érables",
                CP = "G9A9A9",
                TE = "418 555-1234",
                PO = "100",
                NE = "1234567890",
                RA = "1",
                DC = "2022-01-01",
                DV = "2022-01-01",
                DE = "2022-01-01",
                BS = "Service de vestiaire",
                FR = "Tous les jours",
                MO = "2",
                SI = "",
                EM = Utiles.GetCertificateThumbprint( _CertificateSerialNumberSRS )
            };

            // Digital signature of the document
            // Signature numérique du document
            byte[ ] bytSignature = UtilesECDSA.GetSignature( docHAB.GetDocumentConcatene( ), _CertificateSerialNumberSRS );
            docHAB.SI = Convert.ToBase64String( bytSignature );

            // Preparation of the json document for transmission to th WEB-SRM
            // Préparation du document json pour transmission au MEV-WEB
            String JSon = UtilesJSON.GetJsonDocument( ValeursPossibles.TypDoc.HAB, docHAB.GetDocumentConcatene( ) );

            if( _Verbose )
                Console.WriteLine( UtilesJSON.IndentJson( JSon ) );

            // Adding HTTP headers specific to the Document request
            // Ajout des entêtes HTTP spécifiques à la requête Document
            List<KeyValuePair<String, String>> lstHTTPHeaders = GetHTTPHeaders( );
            lstHTTPHeaders.Add( new KeyValuePair<string, string>( "NOTVQ", _noQSTUser ) );
            lstHTTPHeaders.Add( new KeyValuePair<string, string>( "NOTPS", _noGSTUser ) );

            // Send the request to the WEB-SRM
            // Transmission de la requête au MEV-WEB
            WEBSRM_Response WEB_SRM_Response = UtilesJSON.DocumentRequest( lstHTTPHeaders, JSon, _CertificateSerialNumberSRS );

            // Processing of error messages received from WEB-SRM, if any
            // Traitement des messages d'erreurs reçus du MEV-WEB, s'il y a lieu
            if( WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "listErr" ) )
            {
                List<ErrorWEBSRM> lstErr = UtilesJSON.ParseErrors( WEB_SRM_Response.ResponseJsonWEBSRM );
                if( lstErr.Count > 0 )
                    foreach( ErrorWEBSRM er in lstErr )
                    {
                        Console.WriteLine( $"Id : {er.Id}" );
                        Console.WriteLine( $"Message : {er.Mess}" );
                        Console.WriteLine( $"Code de retour : {er.CodRetour}" );
                    }
            }
            else
            {
                if( !WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "Error" ) && !WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "délai d'attente" ) && !WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "Impossible de créer un canal sécurisé SSL/TLS." ) )
                {
                    // Get the informations received from the WEB-SRM
                    // Lecture des informations reçues du MEV-WEB
                    ResponseDocument responseDocument = UtilesJSON.ParseResponseDocument( WEB_SRM_Response.ResponseJsonWEBSRM );

                    // Information received from the WEB-SRM
                    // Informations reçues du MEV-WEB
                    if( _Verbose )
                    {
                        Console.WriteLine( $"Prochain cas d'essai : {responseDocument.ProchainCasEssai} " );
                        Console.WriteLine( $"Numéro du document : {responseDocument.NoDoc} " );
                    }
                }
                else
                {

                    if( WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "Le délai d'attente de l'opération a expiré." ) )
                        Console.WriteLine( "Problème de communication avec le MEV-WEB. Le délai d'attente de l'opération a expiré." );
                    else
                    {
                        if( WEB_SRM_Response.ResponseJsonWEBSRM.Contains( "Impossible de créer un canal sécurisé SSL/TLS." ) )
                            Console.WriteLine( "Problème de certificat pour vous authentifer auprès du MEV-WEB" );
                        else
                            Console.WriteLine( WEB_SRM_Response.ResponseJsonWEBSRM );
                    }
                }
            }
        }

        #endregion DocumentHAB

        #region Private Methods
        /// <summary>
        /// Préparation de la liste des entêtes HTTP pour la connexion au MEV-WEB
        /// Preparation of the list of HTTP headers for the connection to the WEB-SRM
        /// </summary>
        /// <returns>Liste des entêtes</returns>
        private List<KeyValuePair<String, String>> GetHTTPHeaders( )
        {
            List<KeyValuePair<String, String>> HTTPHeadersDicti = new List<KeyValuePair<String, String>>( );
            try
            {
                HTTPHeadersDicti.Add( new KeyValuePair<String, String>( "CONTENT-TYPE", "application/json" ) );

                if( UtilesValidation.ValiderENVIRN( _ENVIRN ) )
                    HTTPHeadersDicti.Add( new KeyValuePair<String, String>( "ENVIRN", _ENVIRN ) );

                if( UtilesValidation.ValiderCASESSAI( _CASESSAI ) )
                    HTTPHeadersDicti.Add( new KeyValuePair<String, String>( "CASESSAI", _CASESSAI ) );

                if( UtilesValidation.ValiderAPPRLINIT( _APPRLINIT ) )
                    HTTPHeadersDicti.Add( new KeyValuePair<String, String>( "APPRLINIT", _APPRLINIT ) );

                if( UtilesValidation.ValiderIDAPPRL( _IDAPPRL ) )
                    HTTPHeadersDicti.Add( new KeyValuePair<String, String>( "IDAPPRL", _IDAPPRL ) );

                if( UtilesValidation.ValiderIDSEV( _IDSEV ) )
                    HTTPHeadersDicti.Add( new KeyValuePair<String, String>( "IDSEV", _IDSEV ) );

                if( UtilesValidation.ValiderIDVERSI( _IDVERSI ) )
                    HTTPHeadersDicti.Add( new KeyValuePair<String, String>( "IDVERSI", _IDVERSI ) );

                if( UtilesValidation.ValiderCODCERTIF( _CODCERTIF ) )
                    HTTPHeadersDicti.Add( new KeyValuePair<String, String>( "CODCERTIF", _CODCERTIF ) );

                if( UtilesValidation.ValiderIDPARTN( _IDPARTN ) )
                    HTTPHeadersDicti.Add( new KeyValuePair<String, String>( "IDPARTN", _IDPARTN ) );

                if( UtilesValidation.ValiderVERSI( _VERSI ) )
                    HTTPHeadersDicti.Add( new KeyValuePair<String, String>( "VERSI", _VERSI ) );

                if( UtilesValidation.ValiderVERSIPARN( _VERSIPARN ) )
                    HTTPHeadersDicti.Add( new KeyValuePair<String, String>( "VERSIPARN", _VERSIPARN ) );

            }
            catch( FormatException fe )
            {
                Console.WriteLine( fe );
            }
            catch( Exception e )
            {
                Console.WriteLine( e );
            }
            return HTTPHeadersDicti;
        }
        #endregion Private Methods
    }
}
