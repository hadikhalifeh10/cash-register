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
    /// Requête Document - Rapport Utilisateur
    /// DOCUMENT Request - User report structure
    /// </summary>
    public class DocumentRUT
    {
        /// <summary>
        /// Numéro d’inscription au fichier de la TPS
        /// GST registration number
        /// </summary>
        public String RT { get; set; }

        /// <summary>
        /// Numéro d’inscription au fichier de la TVQ
        /// QST registration number
        /// </summary>
        public String TQ { get; set; }

        /// <summary>
        /// Nom de l’utilisateur
        /// User name
        /// </summary>
        public String UT { get; set; }

        /// <summary>
        /// Numéro de transaction du dernier document
        /// Transaction number of the last document
        /// </summary>
        public String NO { get; set; }

        /// <summary>
        /// Montant après taxes du dernier document
        /// After-tax amount of the last document
        /// </summary>
        public String MT { get; set; }

        /// <summary>
        /// Moment de la production du dernier document
        /// Date and time the last document was produced
        /// </summary>
        public String DF { get; set; }

        /// <summary>
        /// L’année où les transactions du sommaire des ventes ont été comptabilisées (en cours ou précédente)
        /// Year (current or preceding) the transactions in the sales summary were recorded
        /// </summary>
        public String AN { get; set; }

        /// <summary>
        /// Nombre total de transactions
        /// Total number of tranactions
        /// </summary>
        public String SN { get; set; }

        /// <summary>
        /// Nombre de transactions de paiements
        /// Number of transaction payments
        /// </summary>
        public String SV { get; set; }

        /// <summary>
        ///  Montant avant taxes du sommaire des ventes
        ///  Before-tax amount from the sales summary
        /// </summary>
        public String SS { get; set; }

        /// <summary>
        /// Montant de la TPS du sommaire des ventes
        /// GST amount from the sales summary 
        /// </summary>
        public String SF { get; set; }

        /// <summary>
        /// Montant de la TVQ du sommaire des ventes
        /// QST amount from the sales summary
        /// </summary>
        public String SP { get; set; }

        /// <summary>
        /// Montant après taxes du sommaire des ventes
        /// After-tax amount from the sales summary
        /// </summary>
        public String ST { get; set; }

        /// <summary>
        /// Montant ajusté du sommaire des ventes
        /// Adjusted sales summary amount
        /// </summary>
        public String SA { get; set; }

        /// <summary>
        /// Montant dû du sommaire des ventes
        /// Amount due from sales summary
        /// </summary>
        public String SD { get; set; }

        /// <summary>
        /// Source utilisée pour le calcul du sommaire des ventes
        /// Source used to calculate the sales summary
        /// </summary>
        public String TS { get; set; }

        /// <summary>
        /// Identifiant de l'appareil pour lequel le rapport est demandé, si le sommaire des ventes est calculé pour un seul SEV,
        /// OU Numéro de dossier relatif à la facturation obligatoire, si le sommaire des ventes est calculé pour l'ensemble des SEV de l'établissement
        /// Identifier of the device for which the report is requested, if the sales summary is calculated for a single SRS,
        /// OR Mandatory billing file number, if the sales summary is calculated for all SRS in the establishment
        /// </summary>
        public String SR { get; set; }

        /// <summary>
        /// Indication que les ventes correspondent à un seul compte utilisateur ou à tous les comptes utilisateurs de l'établissement
        /// Indication of whether sales correspond to a single user account or to all user accounts in the establishment
        /// </summary>
        public String CM { get; set; }

        /// <summary>
        /// Identifiant unique de l’appareil utilisé
        /// Unique device identifier
        /// </summary>
        public String IA { get; set; }

        /// <summary>
        /// Identifiant unique du SEV
        /// SRS identifier
        /// </summary>
        public String IS { get; set; }

        /// <summary>
        /// Version du SEV attribuée par le concepteur
        /// SRS version number assigned by the developer
        /// </summary>
        public String VR { get; set; }

        /// <summary>
        /// Moment où l’utilisateur s’est connecté à son compte
        /// Date and time the user logged into his or her account
        /// </summary>
        public String DC { get; set; }

        /// <summary>
        /// Moment de la production du Rapport de l’utilisateur
        /// Date and time the user report was produced
        /// </summary>
        public String DR { get; set; }

        /// <summary>
        /// Signature numérique du Rapport de l’utilisateur
        /// Digital signature of the user report
        /// </summary>
        public String SI { get; set; }

        /// <summary>
        /// Empreinte du certificat numérique associé à la clé privée qui a généré la signature numérique
        /// Thumbprint of the digital certificate linked with the private key that generated the digital signature
        /// </summary>
        public String EM { get; set; }

        /// <summary>
        /// L’adresse de l’établissement
        /// Establishment’s address
        /// </summary>
        public String AD { get; set; }

        /// <summary>
        /// Constructeurs
        /// </summary>
        public DocumentRUT( ) { }

        /// <summary>
        /// Concaténation des informations pour calcul de la signature ou pour transmission du document au MEV-WEB
        /// Concatenation of information to calculate the signature or to transmit the document to WEB-SRM
        /// </summary>
        /// <returns>Informations concaténées
        ///          Concatenated data</returns>
        public String GetDocumentConcateneSignature( )
        {
            StringBuilder s = new StringBuilder( );
            s.AppendFormat( "{0}", RT );
            s.AppendFormat( "{0}", TQ );
            s.AppendFormat( "{0}", UT );
            s.AppendFormat( "{0}", NO );
            s.AppendFormat( "{0}", MT );
            s.AppendFormat( "{0}", DF );
            s.AppendFormat( "{0}", AN );
            s.AppendFormat( "{0}", SN );
            s.AppendFormat( "{0}", SV );
            s.AppendFormat( "{0}", SS );
            s.AppendFormat( "{0}", SF );
            s.AppendFormat( "{0}", SP );
            s.AppendFormat( "{0}", ST );
            s.AppendFormat( "{0}", IA );
            s.AppendFormat( "{0}", IS );
            s.AppendFormat( "{0}", VR );
            s.AppendFormat("{0}", DC);
            s.AppendFormat( "{0}", DR );
            return s.ToString( );
        }

        /// <summary>
        /// Concaténation des informations pour produire le document json et le code QR
        /// Concatenation of information to create the json document and QR code
        /// </summary>
        /// <returns>Informations concaténées
        ///          Concatenated data</returns>
        public String GetDocumentConcateneQRCode( )
        {
            StringBuilder s = new StringBuilder( );
            s.AppendFormat( "RT={0};", RT );
            s.AppendFormat( "TQ={0};", TQ );
            s.AppendFormat( "UT={0};", UT );
            s.AppendFormat( "NO={0};", NO );
            s.AppendFormat( "MT={0};", MT );
            s.AppendFormat( "DF={0};", DF );
            s.AppendFormat( "IA={0};", IA );
            s.AppendFormat( "IS={0};", IS );
            s.AppendFormat( "VR={0};", VR );
            s.AppendFormat( "DC={0};", DC );
            s.AppendFormat( "DR={0};", DR );
            s.AppendFormat( "SI={0};", SI );
            s.AppendFormat( "EM={0};", EM );
            s.AppendFormat( "AD={0}", AD );
            return s.ToString( );
        }

        /// <summary>
        /// Concaténation des informations pour produire le document json
        /// Concatenation of information to create the json document
        /// </summary>
        /// <returns>Informations concaténées
        ///          Concatenated data</returns>
        public String GetDocumentConcateneJSonRequest( )
        {
            StringBuilder s = new StringBuilder( );
            s.AppendFormat( "RT={0};", RT );
            s.AppendFormat( "TQ={0};", TQ );
            s.AppendFormat( "UT={0};", UT );
            s.AppendFormat( "NO={0};", NO );
            s.AppendFormat( "MT={0};", MT );
            s.AppendFormat( "DF={0};", DF );
            s.AppendFormat( "AN={0};", AN );
            s.AppendFormat( "SN={0};", SN );
            s.AppendFormat( "SV={0};", SV );
            s.AppendFormat( "SS={0};", SS );
            s.AppendFormat( "SF={0};", SF );
            s.AppendFormat( "SP={0};", SP );
            s.AppendFormat( "ST={0};", ST );
            s.AppendFormat( "SA={0};", SA );
            s.AppendFormat( "SD={0};", SD );
            s.AppendFormat( "TS={0};", TS );
            s.AppendFormat( "SR={0};", SR );
            s.AppendFormat( "CM={0};", CM );
            s.AppendFormat( "IA={0};", IA );
            s.AppendFormat( "IS={0};", IS );
            s.AppendFormat( "VR={0};", VR );
            s.AppendFormat( "DC={0};", DC );
            s.AppendFormat( "DR={0};", DR );
            s.AppendFormat( "SI={0};", SI );
            s.AppendFormat( "EM={0};", EM );
            s.AppendFormat( "AD={0}", AD );
            return s.ToString( );
        }
    }
}
