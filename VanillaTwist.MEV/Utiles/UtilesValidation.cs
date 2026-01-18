using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleToAttribute("SRS")]
[assembly: InternalsVisibleToAttribute("VanillaTwist.SEV")]

namespace VanillaTwist.MEV
{
    /// <summary>
    /// Validation des formats de données à transmettre au MEV-WEB.
    /// Validation of data formats to be transmitted to the WEB-SRM
    /// </summary>
    public class UtilesValidation
    {
        #region ValidationsCommunes

        /// <summary>
        /// Validation du format de données du champ ou entête HTTP noTPS. Le format de données est : 999999999RT9999
        /// Validation of the data format of the noTPS HTTP field or header. The data format is : 999999999RT9999
        /// </summary>
        /// <param name="noTPS">Le numéro d’inscription au fichier de la TPS attribué au mandataire.
        ///                     Mandatary’s GST registration number.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderNoTPS( String noTPS )
        {
            if ( noTPS == null )
                throw new NullReferenceException( "Le champ noTPS ne peut être null. The noTPS field cannot be null." );

            if ( Regex.IsMatch( noTPS, @"^\d{9}RT\d{4}$" ) ) // 999999999RT9999
                return true;
            else
                throw new FormatException( "Le format de données du champ noTPS doit être 999999999RT9999. The data format of the noTPS field must be 999999999RT9999." );
        }

        /// <summary>
        /// Validation du format de données du champ ou entête HTTP noTVQ. Le format de données est : 999999999RT9999
        /// Validation of the data format of the noTVQ HTTP field or header. The data format is : 999999999RT9999
        /// </summary>
        /// <param name="noTVQ">Le numéro d’inscription au fichier de la TVQ attribué au mandataire.
        ///                     Mandatary’s QST registration number.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderNoTVQ( String noTVQ )
        {
            if ( noTVQ == null )
                throw new NullReferenceException( "Le champ noTVQ ne peut être null. The noTVQ field cannot be null." );

            if ( Regex.IsMatch( noTVQ, @"^\d{10}TQ\d{4}$" ) ) // 9999999999RT9999
                return true;
            else
                throw new FormatException( "Le format de données du champ noTVQ doit être 9999999999TQ9999. The data format of the noTVQ field must be 999999999RT9999." );
        }

        #endregion ValidationsCommunes

        #region Entêtes HTTP - HTTPS Headers
        /// <summary>
        /// Validation du contenu de l'entête APPRLINIT. Les valeurs possibles sont : SEV ou SRV
        /// Validation of the content of the APPRLINIT header. The possible values are : SEV or SRV
        /// </summary>
        /// <param name="APPRLINIT">Contenu de l'entête.
        ///                         Header content.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderAPPRLINIT( String APPRLINIT )
        {
            if ( APPRLINIT == null )
                throw new NullReferenceException( "Le champ APPRLINIT ne peut être null. The APPRLINIT field cannot be null." );

            switch ( APPRLINIT )
            {
                case "SEV":
                case "SRV":
                    return true;
                default:
                    throw new FormatException( "Les valeurs possibles pour l'entête APPRLINIT sont : SEV ou SRV. The possible values for the APPRLINIT header are : SEV or SRV" );
            }
        }

        /// <summary>
        /// Validation du contenu de l'entête ENVIRN. Les valeurs possibles sont : DEV, ESSAI ou PROD
        /// Validation of the content of the ENVIRN header. The possible values are : DEV, ESSAI ou PROD
        /// </summary>
        /// <param name="ENVIRN">Contenu de l'entête.
        ///                         Header content.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderENVIRN( String ENVIRN )
        {
            if ( ENVIRN == null )
                throw new NullReferenceException( "Le champ ENVIRN ne peut être null. The ENVIRN field cannot be null." );

            switch ( ENVIRN )
            {
                case "DEV":
                case "ESSAI":
                case "PROD":
                    return true;
                default:
                    throw new FormatException( "Les valeurs possibles pour l'entête ENVIRN sont : DEV, ESSAI ou PROD. The possible values for the APPRLINIT header are : DEV, ESSAI ou PROD." );
            }
        }

        /// <summary>
        /// Validation du format de données de l'entête CASESSAI. Le format de données doit être 999.999
        /// Validation of the data format of the CASESSAI HTTP field or header. The data format is : 999.999
        /// </summary>
        /// <param name="CASESSAI">Contenu de l'entête.
        ///                        Header content.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderCASESSAI( String CASESSAI )
        {
            if ( CASESSAI == null )
                throw new NullReferenceException( "Le champ CASESSAI ne peut être null. The CASESSAI field cannot be null." );

            if ( Regex.IsMatch( CASESSAI, @"^\d{3}.\d{3}$" ) ) // SW-74
                return true;
            else
                throw new FormatException( "Le format de données CASESSAI doit être 999.999. The data format of the CASESSAI field must be 999.999." );
        }

        /// <summary>
        /// Validation du format de données de l'entête IDAPPRL. Le format de données doit être 9999-9999-9999
        /// Validation of the data format of the IDAPPRL HTTP field or header. The data format is : 9999-9999-9999
        /// </summary>
        /// <param name="IDAPPRL">Contenu de l'entête.
        ///                       Header content.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderIDAPPRL( String IDAPPRL )
        {
            if ( IDAPPRL == null )
                throw new NullReferenceException( "Le champ IDAPPRL ne peut être null. The IDAPPRL field cannot be null." );

            if ( Regex.IsMatch( IDAPPRL, @"^\d{4}-\d{4}-\d{4}$" ) ) // 9999-9999-9999
                return true;
            else
                throw new FormatException( "Le format de données IDAPPRL doit être 9999-9999-9999. The data format of the IDAPPRL field must be 9999-9999-9999." );
        }

        /// <summary>
        /// Validation du format de données de l'entête IDSEV. Le format de données doit être une chaine de 16 caractères hexadécimaux.
        /// Validation of the data format of the IDSEV HTTP field or header. The data format must be a string of 16 hexadecimal characters.
        /// </summary>
        /// <param name="IDSEV">Contenu de l'entête.
        ///                     Header content.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderIDSEV( String IDSEV )
        {
            if ( IDSEV == null )
                throw new NullReferenceException( "Le champ IDSEV ne peut être null. The IDSEV field cannot be null." );

            if ( Regex.IsMatch( IDSEV, @"^[a-fA-F0-9]{16}$" ) ) // par exemple : 467a29765822a89a
                return true;
            else
                throw new FormatException( "Le format de données IDSEV doit être une chaine de 16 caractères hexadécimaux. The data format of the IDSEV field must be a string of 16 hexadecimal characters." );
        }

        /// <summary>
        /// Validation du format de données de l'entête IDVERSI. Le format de données doit être une chaine de 16 caractères hexadécimaux.
        /// Validation of the data format of the IDSEV HTTP field or header. The data format must be a string of 16 hexadecimal characters.
        /// </summary>
        /// <param name="IDVERSI">Contenu de l'entête.
        ///                       Header content.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderIDVERSI( String IDVERSI )
        {
            if ( IDVERSI == null )
                throw new NullReferenceException( "Le champ IDVERSI ne peut être null. The IDVERSI field cannot be null." );

            if ( Regex.IsMatch( IDVERSI, @"^[a-fA-F0-9]{16}$" ) ) // par exemple : 467a29765822a89a
                return true;
            else
                throw new FormatException( "Le format de données IDVERSI doit être une chaine de 16 caractères hexadécimaux. The data format of the IDVERSI field must be a string of 16 hexadecimal characters." );
        }

        /// <summary>
        /// Validation du format de données de l'entête CODCERTIF. Le format de données doit être AAA999999999.
        /// Validation of the data format of the IDSEV HTTP field or header. The data format must be AAA999999999.
        /// </summary>
        /// <param name="CODCERTIF">Contenu de l'entête.
        ///                       Header content.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderCODCERTIF( String CODCERTIF )
        {
            if ( CODCERTIF == null )
                throw new NullReferenceException( "Le champ CODCERTIF ne peut être null. The CODCERTIF field cannot be null." );

            if ( Regex.IsMatch( CODCERTIF, @"^[a-zA-Z]{3}\d{9}$" ) ) // AAA999999999
                return true;
            else
                throw new FormatException( "Le format de données CODCERTIF doit être AAA999999999. The data format of the IDVERSI field must be AAA999999999." );
        }

        /// <summary>
        /// Validation du format de données de l'entête IDPARTN. Le format de données doit être une chaine de 16 caractères hexadécimaux.
        /// Validation of the data format of the IDPARTN HTTP field or header. The data format must be a string of 16 hexadecimal characters.
        /// </summary>
        /// <param name="IDPARTN">Contenu de l'entête.
        ///                       Header content.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderIDPARTN( String IDPARTN )
        {
            if ( IDPARTN == null )
                throw new NullReferenceException( "Le champ IDPARTN ne peut être null. The IDPARTN field cannot be null." );

            if ( Regex.IsMatch( IDPARTN, @"^[a-fA-F0-9]{16}$" ) ) // par exemple : 467a29765822a89a
                return true;
            else
                throw new FormatException( "Le format de données IDPARTN doit être une chaine de 16 caractères hexadécimaux. The data format of the IDPARTN field must be a string of 16 hexadecimal characters." );
        }

        /// <summary>
        /// Validation du format de données de l'entête VERSI. Le format de données doit être une suite d'un à vingt caractères a-z A-Z 0-9 et le point (.) est accepté.
        /// Validation of the data format of the VERSI HTTP field or header. The data format must be a sequence of one to twenty characters a-z A-Z 0-9 and the dot (.) is accepted.
        /// </summary>
        /// <param name="VERSI">Contenu de l'entête.
        ///                     Header content.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderVERSI( String VERSI )
        {
            if ( VERSI == null )
                throw new NullReferenceException( "Le champ VERSI ne peut être null. The VERSI field cannot be null." );

            if ( Regex.IsMatch( VERSI, @"^[a-fA-F0-9.]{1,20}$" ) ) // par exemple : XXX
                return true;
            else
                throw new FormatException( "Le format de données VERSI doit être une suite d'un à vingt caractères a-z A-Z 0-9 et le point (.) est accepté. The data format of the VERSI field must be a sequence of one to twenty characters a-z A-Z 0-9 and the dot (.) is accepted." );
        }

        /// <summary>
        /// Validation du format de données de l'entête VERSIPARN. Le format de données doit être une suite d'un à vingt caractères a-z A-Z 0-9 et le point (.) est accepté
        /// Validation of the data format of the VERSIPARN HTTP field or header. The data format must be a sequence of one to twenty characters a-z A-Z 0-9 and the dot (.) is accepted.
        /// </summary>
        /// <param name="VERSIPARN">Contenu de l'entête.
        ///                         Header content.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderVERSIPARN( String VERSIPARN )
        {
            if ( VERSIPARN == null )
                throw new NullReferenceException( "Le champ VERSIPARN ne peut être null. The VERSIPARN field cannot be null." );

            if ( Regex.IsMatch( VERSIPARN, @"^[a-fA-F0-9.]{1,20}$" ) ) // par exemple : XXX
                return true;
            else
                throw new FormatException( "Le format de données VERSIPARN doit être une suite d'un à vingt caractères a-z A-Z 0-9 et le point (.) est accepté. The data format of the VERSIPARN field must be a sequence of one to twenty characters a-z A-Z 0-9 and the dot (.) is accepted." );
        }

        /// <summary>
        ///  Validation du format de données de l'entête SIGNATRANSM. Le format de données doit être une chaine de 88 caractères ASCII
        ///  Validation of the data format of the SIGNATRANSM HTTP field or header. The data format must be a string of 88 ASCII characters
        /// </summary>
        /// <param name="SIGNATRANSM">Contenu de l'entête.
        ///                           Header content.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderSIGNATRANSM( String SIGNATRANSM )
        {
            if ( SIGNATRANSM == null )
                throw new NullReferenceException( "Le champ SIGNATRANSM ne peut être null. The SIGNATRANSM field cannot be null." );

            if ( Regex.IsMatch( SIGNATRANSM, @"^[a-zA-Z0-9/=+]{88}$" ) ) // par exemple : SC6RigDy6TrX5/c...
                return true;
            else
                throw new FormatException( "Le format de données SIGNATRANSM doit être une chaine de 88 caractères ASCII. The data format of the SIGNATRANSM header must be a string of 88 ASCII characters." );
        }

        /// <summary>
        ///  Validation du format de données de l'entête EMPRCERTIFTRANSM. Le format de données doit être une chaine de 40 caractères ASCII
        ///  Validation of the data format of the EMPRCERTIFTRANSM HTTP field or header. The data format must be a string of 40 ASCII characters
        /// </summary>
        /// <param name="EMPRCERTIFTRANSM">Contenu de l'entête.
        ///                                Header content.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderEMPRCERTIFTRANSM( String EMPRCERTIFTRANSM )
        {
            if ( EMPRCERTIFTRANSM == null )
                throw new NullReferenceException( "Le champ EMPRCERTIFTRANSM ne peut être null. The EMPRCERTIFTRANSM field cannot be null." );

            if ( Regex.IsMatch( EMPRCERTIFTRANSM, @"^[a-zA-Z0-9/=+]{40}$" ) ) // par exemple : SC6RigDy6TrX5/c...
                return true;
            else
                throw new FormatException( "Le format de données EMPRCERTIFTRANSM doit être une chaine de 40 caractères ASCII. The data format of the EMPRCERTIFTRANSM header must be a string of 40 ASCII characters." );
        }

        #endregion Entêtes HTTP / HTTPS Headers

        #region Certificats

        /// <summary>
        /// Validation du format de données du champ CN de la demande de signature de certificat (CSR). Le format de données doit être un nombre de 10 chiffres.
        /// Validation of the data format of the CN field of the certificate signing request (CSR). The data format must be a 10-digit number.
        /// </summary>
        /// <param name="CN">Numéro d'identification du Serveur ou du mandataire.
        ///                  Server or mandatory identification number.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderCN( String CN )
        {
            if ( CN == null )
                throw new NullReferenceException( "Le champ CN ne peut être null. The CN field cannot be null." );

            if ( Regex.IsMatch( CN, @"^\d{10}$" ) ) // par exemple : 1234567890
                return true;
            else
                throw new FormatException( "Le format de données du champ CN doit être un nombre de 10 chiffres. The data format of the CN field must be a 10-digit number." );
        }

        /// <summary>
        /// Validation du format de données du champ O de la demande de signature de certificat (CSR). Le format de données doit être AAA-X9X9-X9X9.
        /// Validation of the data format of the O field of the certificate signing request (CSR). The data format must be AAA-X9X9-X9X9.
        /// </summary>
        /// <param name="O">Abréviation du secteur d’activité et le code d’autorisation utilisé par l’administrateur du serveur reliés par un trait d’union.
        ///                 Abbreviation of the business sector and the authorization code used by the server administrator linked by a hyphen.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderO( String O )
        {
            if ( O == null )
                throw new NullReferenceException( "Le champ O ne peut être null. The O field cannot be null." );

            if ( Regex.IsMatch( O, @"^{1}(TRP|RBC|FOB)-[A-Z0-9]{1}[0-9]{1}[A-Z]{1}[0-9]{1}-[A-Z]{1}[0-9]{1}[A-Z]{1}[0-9]{1}$" ) )  // TRP-X9X9-X9X9
                return true;
            else
                throw new FormatException( "Le format de données du champ O doit être AAA-X9X9-X9X9. The data format of the CN field must be AAA-X9X9-X9X9." );
        }

        /// <summary>
        /// Validation du format de données du champ OU de la demande de signature de certificat (CSR). Le format de données doit être 9999999999TQ9999.
        /// Validation of the data format of the OU field of the certificate signing request (CSR). The data format must be 9999999999TQ9999.
        /// </summary>
        /// <param name="OU">Numéro d’inscription au fichier de la TVQ du mandataire.
        ///                  The QST registration number of the mandatary (noTVQ).</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderOU( String OU )
        {
            if ( OU == null )
                throw new NullReferenceException( "Le champ OU ne peut être null. The O field cannot be null." );

            if ( Regex.IsMatch( OU, @"^\d{10}TQ\d{4}$" ) ) // 1234567890TQ0001
                return true;
            else
                throw new FormatException( "Le format de données du champ OU doit être 9999999999TQ9999. The data format of the OU field must be 9999999999TQ9999." );
        }

        /// <summary>
        /// Validation du format de données du champ SN de la demande de signature de certificat (CSR). Le format de données doit être un nom convivial pour le certificat (suite de 8 à 32 caractères selon les normes ASCII).
        /// Validation of the data format of the SN field of the certificate signing request (CSR). The data format must be a user-friendly name for the certificate (8 to 32 character string according to ASCII standards).
        /// </summary>
        /// <param name="SN">Nom convivial pour le certificat.
        ///                  User-friendly name for the certificate.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderSN( String SN )
        {
            if ( SN == null )
                throw new NullReferenceException( "Le champ SN ne peut être null. The SN field cannot be null." );

            if ( Regex.IsMatch( SN, @"^[a-zA-Z0-9 @:!#$%&'()*+,-.=?_|~ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÑÒÓÔÕÖØÙÚÛÜÝàáâãäåæçèéêëìíîïðñòóôõöøùúûüýÿ]{8,32}$" ) ) // par exemple : XXX
                return true;
            else
                throw new FormatException( "Le format de données du champ SN doit être une suite de 8 à 32 caractères respectant les normes ASCII. The data format of the SN field must be a sequence of 8 to 32 characters respecting the ASCII standards." );
        }

        /// <summary>
        /// Validation du format de données du champ GN de la demande de signature de certificat (CSR). Inscrire le numéro de dossier relatif à la facturation obligatoire.
        /// Validation of the data format of the GN field of the certificate signing request (CSR). Enter the billing file number for the mandatory billing.
        /// </summary>
        /// <param name="GN">Numéro de dossier relatif à la facturation obligatoire.
        ///                  Billing file number for the mandatory billing.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderGN( String GN )
        {
            if ( GN == null )
                throw new NullReferenceException( "Le champ GN ne peut être null. The GN field cannot be null." );

            if ( Regex.IsMatch( GN, @"^[A-Z]{2}\d{4}$" ) ) // par exemple : GN = "AA9999";
                return true;
            else
                throw new FormatException( "Le format de données du champ GN doit être AA9999. The data format of the GN field must be AA9999." );
        }

        /// <summary>
        /// Validation du contenu du champ L de la demande de signature de certificat (CSR). Les valeurs possibles sont : -04:00 ou -05:00
        /// Validation of the data format of the L field of the certificate signing request. The value must be -04:00 or -05:00.
        /// </summary>
        /// <param name="L">Temps universel coordonné (UTC) de la municipalité.
        ///                 Municipality's Coordinated Universal Time (UTC).</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderL( String L )
        {
            if ( L == null )
                throw new NullReferenceException( "Le champ L ne peut être null. The L field cannot be null." );

            if ( L.Equals( "-04:00" ) || L.Equals( "-05:00" ) )
                return true;
            else
                throw new FormatException( "Les valeurs possibles pour le champ L sont : -04:00 ou -05:00. The value for the field L must be -04:00 or -05:00." );
        }

        /// <summary>
        /// Validation du contenu du champ S de la demande de signature de certificat (CSR). La valeur possible est : QC
        /// Validation of the data format of the S field of the certificate signing request. The value must be QC.
        /// </summary>
        /// <param name="S">Abréviation de la province.
        ///                 Abbreviation for the province.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderS( String S )
        {
            if ( S == null )
                throw new NullReferenceException( "Le champ S ne peut être null. The S field cannot be null." );

            if ( S.Equals( "QC" ) )
                return true;
            else
                throw new FormatException( "La valeur possible pour le champ S est : QC. The value for the field S must be QC." );
        }

        /// <summary>
        /// Validation du contenu du champ C de la demande de signature de certificat (CSR). La valeur possible est : CA
        /// Validation of the data format of the C field of the certificate signing request. The value must be CA.
        /// </summary>
        /// <param name="C">Abréviation du pays.
        ///                 Abbreviation for the country.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderC( String C )
        {
            if ( C == null )
                throw new NullReferenceException( "Le champ C ne peut être null. The C field cannot be null." );

            if ( C.Equals( "CA" ) )
                return true;
            else
                throw new FormatException( "La valeur possible pour le champ C est : CA. The value for the field S must be CA." );
        }

        /// <summary>
        /// Validation du contenu du champ modif du document json d'une requête de type Certificat. Les valeurs possibles sont : AJO, REM, SUP
        /// Validation of the content of the field modif of the json document of a request of type Certificat. The possible values are : AJO, REM, SUP
        /// </summary>
        /// <param name="modif">Contenu du champ modif.
        ///                     Content of the field modif.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderCertificatModif( String modif )
        {
            if ( modif == null )
                throw new NullReferenceException( "Le champ modif ne peut être null. The modif field cannot be null." );

            switch ( modif )
            {
                case "AJO":
                case "REM":
                case "SUP":
                    return true;
                default:
                    throw new FormatException( "Les valeurs possibles pour le champ modif sont : AJO, REM ou SUP. The possible values for the field modif are : AJO, REM, SUP." );
            }
        }

        /// <summary>
        /// Validation du contenu du champ noSerie du document json d'une requête de type Certificat. Le format de données doit être une chaine de 40 caractères hexadécimaux.
        /// Validation of the content of the noSerie field of the json document of a Certificate type request. The data format must be a string of 40 hexadecimal characters.
        /// </summary>
        /// <param name="noSerie">Contenu du champ noSerie.
        ///                       Content of the field noSerie.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderCertificatNoSerie( String noSerie )
        {
            if ( noSerie == null )
                throw new NullReferenceException( "Le champ noSerie ne peut être null. The noSerie field cannot be null." );

            if ( Regex.IsMatch( noSerie, @"^[a-fA-F0-9]{40}$" ) ) // par exemple : 06F6E4A46206F83E36...
                return true;
            else
                throw new FormatException( "Le format de données noSerie doit être une chaine de 16 caractères hexadécimaux. The data format for the field noSerie must be a string of 40 hexadecimal characters." );
        }

        #endregion Certificats

        #region Utilisateurs

        /// <summary>
        /// Validation du contenu du champ modif du document json d'une requête de type Utilisateur. Les valeurs possibles sont : AJO, REM, VAL
        /// Validation of the content of the field modif of the json document of a request of type Certificat. The possible values are : AJO, REM, SUP
        /// </summary>
        /// <param name="modif">Contenu du champ modif.
        ///                       Content of the field modif.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderUtilisateurModif( String modif )
        {
            if ( modif == null )
                throw new NullReferenceException( "Le champ modif ne peut être null. The modif field cannot be null." );

            switch ( modif )
            {
                case "AJO":
                case "REM":
                case "VAL":
                    return true;
                default:
                    throw new FormatException( "Les valeurs possibles pour le champ modif sont : AJO, REM ou VAL. The possible values for the field modif are : AJO, REM, VAL." );
            }
        }

        /// <summary>
        /// Validation du contenu du champ nomUtil du document json d'une requête de type Utilisateur. Le format de données doit être une chaine de 1 à 64 caractères selon les normes ASCII.
        /// alidation of the content of the field nonUtil of the json document of a request of type Utilisateur. The data format must be a string of 1 to 64 characters according to ASCII standards.
        /// </summary>
        /// <param name="nomUtil">Le nom de l’utilisateur qui produit la facture au client.
        ///                       Name of the user who produced the customer’s bill.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderNomUtil( String nomUtil )
        {
            if ( nomUtil == null )
                throw new NullReferenceException( "Le champ nomUtil ne peut être null. The nomUtil field cannot be null." );

            if ( Regex.IsMatch( nomUtil, @"^[a-zA-Z0-9 @:!#$%&'()*+,-.=?_|~ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÑÒÓÔÕÖØÙÚÛÜÝàáâãäåæçèéêëìíîïðñòóôõöøùúûüýÿ]{1,64}$" ) ) // par exemple : XXX
                return true;
            else
                throw new FormatException( "Le format de données du champ nomUtil doit être une suite de 1 à 64 caractères respectant les normes ASCII. The data format for the field nomUtil must be a string of 1 to 64 characters according to ASCII standards." );
        }

        #endregion Utilisateurs

        #region Transactions

        /// <summary>
        /// Validation du contenu du champ abrvt. Les valeurs possibles sont : TRP ou RBC
        /// Validation of the content of the field abvrt. The values must be TRP or RBC
        /// </summary>
        /// <param name="abrvt">Contenu du champ abrvt.
        ///                     Content of the field abvrt.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderAbrvt( String abrvt )
        {
            if ( abrvt == null )
                throw new NullReferenceException( "Le champ abrvt ne peut être null. The abvrt field cannot be null." );

            switch ( abrvt )
            {
                case "TRP":
                case "RBC":
                    return true;
                default:
                    throw new FormatException( "Les valeurs possibles pour le champ abrvt sont : TRP ou RBC. The values for the field abvrt must be TRP or RBC." );
            }
        }

        /// <summary>
        /// Validation du contenu du champ acti. Les valeurs possibles sont : TRP, RES, BAR, CDR, HAB, NON ou SOB
        /// Validation of the content of the field acti. The values must be TRP, RES, BAR, CDR, HAB, NON ou SOB
        /// </summary>
        /// <param name="acti">Contenu du champ acti.
        ///                    Content of the field acti.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderActi( String acti )
        {
            if ( acti == null )
                throw new NullReferenceException( "Le champ acti ne peut être null. The acti field cannot be null." );

            switch ( acti )
            {
                case "TRP":
                case "RES":
                case "BAR":
                case "CDR":
                case "HAB":
                case "NON":
                case "SOB":
                    return true;
                default:
                    throw new FormatException( "Les valeurs possibles pour le champ acti sont : TRP, RES, BAR, CDR, HAB, NON ou SOB. The values for the field acti must be TRP, RES, BAR, CDR, HAB, NON ou SOB" );
            }
        }

        /// <summary>
        /// Validation du contenu du champ noTabl. Le format de données pour le champ noTabl doit être composé de 1 à 5 caractères alphanumériques. Format : 99999 (remplir à gauche avec des signes d’égalité (« = », si nécessaire)  pour obtenir 5 caractères.
        /// Validation of the content of the noTabl field. The data format for the noTabl field must be composed of 1 to 5 alphanumeric characters. Format: 99999 (fill in on the left with equality signs ("=", if necessary) to obtain 5 characters
        /// </summary>
        /// <param name="noTabl">Contenu du champ noTabl.
        ///                      Content of the field noTabl.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderNoTabl( String noTabl )
        {
            if ( noTabl == null )
                throw new NullReferenceException( "Le champ noTabl ne peut être null. The noTabl field cannot be null." );

            if ( Regex.IsMatch( noTabl, @"^[=]{1}[0-9]{4}$|^[=]{2}[0-9]{3}$|^[=]{3}[0-9]{2}$|^[=]{4}[0-9]{1}$|^[0-9]{5}$" ) )
                return true;
            else
                throw new FormatException( "Le format de données pour le champ noTabl doit être composé de 1 à 5 caractères alphanumériques. Format : 99999 (remplir à gauche avec des signes d’égalité '=' si nécessaire pour obtenir 5 caractères. The data format for the noTabl field must be composed of 1 to 5 alphanumeric characters. Format: 99999 (fill in on the left with equality '=' signs if necessary to obtain 5 characters." );
        }

        /// <summary>
        /// Validation du contenu du champ nbClint. Le format de données pour le champ nbClint doit être composé de 3 caractères numériques. Format : 999 (remplir à gauche avec des zéros, si nécessaire).
        /// Validation of the content of the nbClint field. The data format for the nbClint field must be composed of 3 numeric characters. Format: 999 (fill in on the left with zeros, if necessary).
        /// </summary>
        /// <param name="nbClint">Contenu du champ nbClint.
        ///                       Content of the field nbClint.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderNbClint( String nbClint )
        {
            if ( nbClint == null )
                throw new NullReferenceException( "Le champ nbClint ne peut être null. The nbClint field cannot be null." );

            if ( Regex.IsMatch( nbClint, @"^[0]{1}[0-9]{2}$|^[0]{2}[0-9]{1}$|^[1-9]{1}[0]{1}[1-9]{1}$|^[0-9]{3}$" ) )
                return true;
            else
                throw new FormatException( "Le format de données pour le champ nbClint doit être composé de 3 caractères numériques. Format : 999 (remplir à gauche avec des zéros, si nécessaire). The data format for the nbClint field must be composed of 3 numeric characters. Format: 999 (fill in on the left with zeros, if necessary)." );
        }

        /// <summary>
        /// Validation du contenu du champ noTrans. Il doit être composé de 1 à 10 caractères ASCII.
        /// Validation of the content of the noTrans field. It must be composed of 1 to 10 ASCII characters.
        /// </summary>
        /// <param name="noTrans">Contenu du champ noTrans.
        ///                       Content of the field noTrans.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderNoTrans( String noTrans )
        {
            if ( noTrans == null )
                throw new NullReferenceException( "Le champ noTrans ne peut être null. The noTrans field cannot be null." );

            if ( Regex.IsMatch( noTrans, @"^[a-zA-Z0-9\-.]{1,10}$" ) )
                return true;
            else
                throw new FormatException( "Le format de données pour le champ noTrans doit être composé de 1 à 10 caractères ASCII. The noTrans field must be composed of 1 to 10 ASCII characters." );
        }

        /// <summary>
        /// Validation du contenu du champ nomMandt. Il doit être composé de 1 à 64 caractères ASCII.
        /// Validation of the content of the nomMandt field. It must be composed of 1 to 64 ASCII characters.
        /// </summary>
        /// <param name="nomMandt">Contenu du champ nomMandt.
        ///                        Content of the field nomMandt.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderNomMandt( String nomMandt )
        {
            if ( nomMandt == null )
                throw new NullReferenceException( "Le champ nomMandt ne peut être null. The nomMandt field cannot be null." );

            if ( Regex.IsMatch( nomMandt, @"^[a-zA-Z0-9 @:!#$%&'()*+,-.=?_|~ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÑÒÓÔÕÖØÙÚÛÜÝàáâãäåæçèéêëìíîïðñòóôõöøùúûüýÿ]{1,64}$" ) ) 
                return true;
            else
                throw new FormatException( "Le format de données pour le champ nomMandt doit être composé de 1 à 64 caractères ASCII. The nomMandt field must be composed of 1 to 64 ASCII characters." );
        }

        /// <summary>
        /// Validation du contenu du champ docNoCiviq. Il doit être composé de 1 à 16 caractères.
        /// Validation of the content of the docNoCiviq field. It must be composed of 1 to 16 ASCII characters.
        /// </summary>
        /// <param name="docNoCiviq">Contenu du champ docNoCiviq.
        ///                          Content of the field docNoCiviq.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderDocNoCiviq( String docNoCiviq )
        {
            if ( docNoCiviq == null )
                throw new NullReferenceException( "Le champ docNoCiviq ne peut être null. The docNoCiviq field cannot be null." );

            if ( Regex.IsMatch( docNoCiviq, @"^[a-zA-Z0-9\-]{1,16}$" ) )
                return true;
            else
                throw new FormatException( "Le format de données pour le champ docNoCiviq doit être composé de 1 à 16 caractères. The docNoCiviq field must be composed of 1 to 16 ASCII characters." );
        }

        /// <summary>
        /// Validation du contenu du champ docCp. Le format de données pour le champ doit être A9A9A9.
        /// Validation of the content of the docCp field. The data format for the field must be A9A9A9.
        /// </summary>
        /// <param name="docCp">Contenu du champ docCp.
        ///                     Content of the field docCp.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderDocCp( String docCp )
        {
            if ( docCp == null )
                throw new NullReferenceException( "Le champ docCp ne peut être null. The docCp field cannot be null." );

            if ( Regex.IsMatch( docCp, @"[GHJ]{1}\d{1}[A-Z]{1}\d{1}[A-Z]{1}\d{1}$" ) )
                return true;
            else
                throw new FormatException( "Le format de données pour le champ docCp doit être A9A9A9. The data format for the field docCp must be A9A9A9." );
        }

        /// <summary>
        /// Validation du contenu du champ relaCommer. Les valeurs possibles sont : B2C, B2B ou B2G.
        /// Validation of the content of the relaCommer field. The possible values are : B2C, B2B or B2G.
        /// </summary>
        /// <param name="relaCommer">Contenu du champ relaCommer.
        ///                          Content of the field relaCommer.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderRelaCommer( String relaCommer )
        {
            if ( relaCommer == null )
                throw new NullReferenceException( "Le champ relaCommer ne peut être null. The relaCommer field cannot be null." );

            switch ( relaCommer )
            {
                case "B2C":
                case "B2B":
                case "B2G":
                    return true;
                default:
                    throw new FormatException( "Les valeurs possibles pour le champ relaCommer sont : B2C , B2B ou B2G. The possible values for the relaCommer field are : B2C, B2B or B2G." );
            }
        }

        /// <summary>
        /// Validation du contenu du champ nomClint. Il doit être composé de 1 à 64 caractères ASCII.
        /// Validation of the content of the nomClint field. It must be composed of 1 to 64 ASCII characters.
        /// </summary>
        /// <param name="nomClint">Contenu du champ nomClint.
        ///                        Content of the field nomClint.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderNomClint( String nomClint )
        {
            if ( nomClint == null )
                throw new NullReferenceException( "Le champ nomClint ne peut être null. The nomClint field cannot be null." );

            if ( Regex.IsMatch( nomClint, @"^[a-zA-Z0-9 @:!#$%&'()*+,-.=?_|~ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÑÒÓÔÕÖØÙÚÛÜÝàáâãäåæçèéêëìíîïðñòóôõöøùúûüýÿ]{1,64}$" ) ) // 999.999
                return true;
            else
                throw new FormatException( "Le format de données pour le champ nomClint doit être composé de 1 à 64 caractères ASCII. The nomClint field must be composed of 1 to 64 ASCII characters." );
        }

        /// <summary>
        /// Validation du format de données du champ noTvqClint. Le format de données doit être 9999999999RT9999.
        /// Validation of the data format of the noTvqClint field. The data format must be 9999999999RT9999.
        /// </summary>
        /// <param name="noTvqClint">Le numéro d’inscription au fichier de la TVQ attribué au client.
        ///                          QST registration number assigned to the mandatary.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderNoTvqClint( String noTvqClint )
        {
            if ( noTvqClint == null )
                throw new NullReferenceException( "Le champ noTvqClint ne peut être null. The noTvqClint field cannot be null." );

            if ( Regex.IsMatch( noTvqClint, @"^\d{10}TQ\d{4}$" ) ) // 9999999999RT9999
                return true;
            else
                throw new FormatException( "Le format de données du champ noTvqClint doit être 9999999999TQ9999. The data format for the field noTvqClint must be 9999999999RT9999." );
        }

        /// <summary>
        /// Validation du format de données du champ tel. Le format de données du champ doit être 10 caractères numériques.
        /// Validation of the data format of the field such. The data format of the field must be 10 numeric characters.
        /// </summary>
        /// <param name="tel">Le numéro de téléphone.
        ///                   Phone Number</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderNoTel( String tel )
        {
            if ( tel == null )
                throw new NullReferenceException( "Le champ tel ne peut être null. The tel field cannot be null." );

            if ( Regex.IsMatch( tel, @"^\d{10}$" ) )
                return true;
            else
                throw new FormatException( "Le format de données du champ tel doit être 10 caractères numériques. The data format of the field tel must be 10 numeric characters." );
        }

        /// <summary>
        /// Validation du format de données du champ courl. Le format de données du champ doit être de 8 à 254 caractères
        /// Validation of the data format of the courl field. The data format of the field must be from 8 to 254 characters
        /// </summary>
        /// <param name="courl">Le courriel.
        ///                     e-mail.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderCourl( String courl )
        {
            if ( courl == null )
                throw new NullReferenceException( "Le champ courl ne peut être null. The courl field cannot be null." );

            if ( Regex.IsMatch( courl, @"^\S{1,}@\S{2,}\.\S{2,}$" ) )
                return true;
            else
                throw new FormatException( "Le format de données du champ courl doit être de 8 à 254 caractères. The data format of the field courl must be from 8 to 254 characters" );
        }

        /// <summary>
        /// Validation du contenu du champ UTC d'une transaction. Les valeurs possibles sont : -04:00A, -04:00N, -05:00A ou -05:00N
        /// Validation of the content of the UTC field of a transaction. Possible values are: -04:00A, -04:00N, -05:00A or -05:00N
        /// </summary>
        /// <param name="UTC">Temps universel coordonné (UTC).
        ///                   Coordinated Universal Time (UTC).</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderUTC( String UTC )
        {
            if ( UTC == null )
                throw new NullReferenceException( "Le champ UTC ne peut être null. The UTC field cannot be null." );

            switch ( UTC )
            {
                case "-04:00A":
                case "-04:00N":
                case "-05:00A":
                case "-05:00N":
                    return true;
                default:
                    throw new FormatException( "Les valeurs possibles pour le champ UTC sont : -04:00A, -04:00N, -05:00A ou -05:00N. The possible values for the utc field are : -04:00A, -04:00N, -05:00A or -05:00N" );
            }
        }

        /// <summary>
        /// Validation du contenu du champ typTrans d'une transaction. Les valeurs possibles sont : ADDI, ESTM, RFER, SOUM, TIER ou SOB
        /// Validation of the content of the typTrans field of a transaction. Possible values are : ADDI, ESTM, RFER, SOUM, TIER or SOB
        /// </summary>
        /// <param name="typTrans">Le type de transaction.
        ///                        Transaction type.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderTypTrans( String typTrans )
        {
            if ( typTrans == null )
                throw new NullReferenceException( "Le champ typTrans ne peut être null. The typTrans field cannot be null." );

            switch ( typTrans )
            {
                case "ADDI":    // Addition
                case "ESTM":    // Estimation
                case "RFER":    // Reçu de fermeture
                case "SOUM":    // Soumission
                case "TIER":    // Tiers inhabituel
                case "SOB":     // Sans objet
                    return true;
                default:
                    throw new FormatException( "Les valeurs possibles pour le champ typTrans sont : ADDI, ESTM, RFER, SOUM, TIER ou SOB. The possible values for the typTrans field are : ADDI, ESTM, RFER, SOUM, TIER or SOB." );
            }
        }

        /// <summary>
        /// Validation du contenu du champ commerElectr d'une transaction. Les valeurs possibles sont : O ou N
        /// Validation of the content of the typTrans field of a transaction. Possible values are : O or N
        /// </summary>
        /// <param name="commerElectr">L’indicateur qu’il s’agit de commerce électronique.
        ///                            E-commerce indicator.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderCommerceElectronique( String commerElectr )
        {
            if ( commerElectr == null )
                throw new NullReferenceException( "Le champ commerElectr ne peut être null. The commerElectr field cannot be null." );

            switch ( commerElectr )
            {
                case "O":    // Oui
                case "N":    // Non
                    return true;
                default:
                    throw new FormatException( "Les valeurs possibles pour le champ commerElectr sont : O ou N. The possible values for the commerElectr field are : O or N." );
            }
        }

        /// <summary>
        /// Validation du contenu du champ modPai d'une transaction. Les valeurs possibles sont : ARG, CHQ, COU, CRE, DEB, CPR, FID, CRY, MVO, TFD, MIX, PAC, AUT, INC, AUC ou SOB
        /// Validation of the content of the typTrans field of a transaction. Possible values are : ARG, CHQ, COU, CRE, DEB, CPR, FID, CRY, MVO, TFD, MIX, PAC, AUT, INC, AUC or SOB
        /// </summary>
        /// <param name="modPai">Le mode de paiement utilisé pour acquitter la facture.
        ///                      Method of payment used for the bill.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderModPai( String modPai )
        {
            if ( modPai == null )
                throw new NullReferenceException( "Le champ modPai ne peut être null. The modPai field cannot be null." );

            switch ( modPai )
            {
                case "ARG":     // Argent comptant
                case "CHQ":     // Chèque
                case "COU":     // Coupon
                case "CRE":     // Carte de crédit
                case "DEB":     // Carte de débit
                case "CPR":     // Certificat-cadeau( p.ex., carte prépayée, chèque-cadeau)
                case "FID":     // Programme de fidélisation
                case "CRY":     // Cryptomonnaie( p.ex., Bitcoins)
                case "MVO":     // Portefeuille électronique( p.ex., PayPal)
                case "TFD":     // Transfert de fonds
                case "MIX":     // Paiement mixte
                case "PAC":     // Porté au compte
                case "AUT":     // Autre
                case "INC":     // Inconnu
                case "AUC":     // Aucun paiement
                case "SOB":     // Sans objet
                    return true;
                default:
                    throw new FormatException( "Les valeurs possibles pour le champ modPai sont : ARG, CHQ, COU, CRE, DEB, CPR, FID, CRY, MVO, TFD, MIX, PAC, AUT, INC, AUC ou SOB. The possible values for the modPai field are : ARG, CHQ, COU, CRE, DEB, CPR, FID, CRY, MVO, TFD, MIX, PAC, AUT, INC, AUC or SOB." );
            }
        }

        /// <summary>
        /// Validation du contenu du champ modImpr d'une transaction. Les valeurs possibles sont : ANN, FAC, RPR, DUP, PSP ou SOB
        /// Validation of the content of the modImpr field of a transaction. Possible values are : ANN, FAC, RPR, DUP, PSP or SOB
        /// </summary>
        /// <param name="modImpr">Le mode d’impression du document.
        ///                       Print mode for the document.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderModImpr( String modImpr )
        {
            if ( modImpr == null )
                throw new NullReferenceException( "Le champ modImpr ne peut être null. The modImpr field cannot be null." );

            switch ( modImpr )
            {
                case "ANN":     // Annulation
                case "RPR":     // Reproduction
                case "DUP":     // Duplicata
                case "PSP":     // Parti sans payer
                case "FAC":     // Facture ou tout autre document non inclus dans cette liste
                case "SOB":     // Sans objet
                    return true;
                default:
                    throw new FormatException( "Les valeurs possibles pour le champ modImpr sont : ANN, FAC, RPR, DUP, PSP ou SOB. The possible values for the modImpr field are : ANN, FAC, RPR, DUP, PSP or SOB." );
            }
        }

        /// <summary>
        /// Validation du contenu du champ formImpr d'une transaction. Les valeurs possibles sont : PAP, ELE, PEL, NON ou SOB
        /// Validation of the content of the formImpr field of a transaction. Possible values are : PAP, ELE, PEL, NON or SOB
        /// </summary>
        /// <param name="formImpr">Le type de support d’impression du document.
        ///                        Document print option.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderFormImpr( String formImpr )
        {
            if ( formImpr == null )
                throw new NullReferenceException( "Le champ formImpr ne peut être null. The formImpr field cannot be null." );

            switch ( formImpr )
            {
                case "PAP":     // Papier
                case "ELE":     // Électronique
                case "PEL":     // Papier et électronique
                case "NON":     // Non imprimé
                case "SOB":     // Sans objet
                    return true;
                default:
                    throw new FormatException( "Les valeurs possibles pour le champ formImpr sont : PAP, ELE, PEL, NON ou SOB. The possible values for the modImpr field are : PAP, ELE, PEL, NON or SOB." );
            }
        }

        /// <summary>
        /// Validation du contenu du champ modTrans d'une transaction. Les valeurs possibles sont : OPE ou FOR
        /// Validation of the content of the modTrans field of a transaction. Possible values are : OPE or FOR
        /// </summary>
        /// <param name="modTrans">Le mode de transaction utilisé.
        ///                        Transaction mode used .</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderModTrans( String modTrans )
        {
            if ( modTrans == null )
                throw new NullReferenceException( "Le champ modTrans ne peut être null. The modTrans field cannot be null." );

            switch ( modTrans )
            {
                case "OPE":    // Opérationnel
                case "FOR":    // Formation
                    return true;
                default:
                    throw new FormatException( "Les valeurs possibles pour le champ modTrans sont : OPE ou FOR. The possible values for the modImpr field are : OPE or FOR." );
            }
        }

        /// <summary>
        /// Valide un champ DateHeure. Le format de données du champ doit être AAAAMMJJhhmmss
        /// Par exemple, le 31 février n'existe pas, il y aura une erreur.
        /// 
        /// Validates a DateTime field. The data format of the field must be YYYYMMDDhhmmss
        /// For example, February 31 does not exist, there will be an error.
        /// 
        /// </summary>
        /// <param name="DateHeure">La date et l'heure concaténées sans ponctuation ni symboles.
        ///                         Date and time concatenated without punctuation or symbols.</param>
        /// <returns>true si valide, FormatException si invalide</returns>
        internal static bool ValiderDateHeure( String DateHeure )
        {
            if ( DateHeure == null )
                throw new NullReferenceException( "Le champ DateHeure ne peut être null. The DateHeure field cannot be null." );

            if ( DateTime.TryParseExact( DateHeure, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime temp ) )
            {
                if ( ( temp.Year < 2019 ) || ( temp.Year > 2100 ) )
                    throw new FormatException( "L'année doit être comprise entre 2019 et 2100. The year must be between 2019 and 2100." );
                else
                    return true;
            }
            else
                throw new FormatException( "Le format de données du champ DateHeure doit être AAAAMMJJhhmmss. The data format of the DateHeure field must be YYYYMMDDhhmmss." );
        }

        #endregion Transactions
    }
}
