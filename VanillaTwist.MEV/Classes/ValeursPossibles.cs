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
using System.Collections.Specialized;
using System.Configuration;
//using System.Windows.Forms;

namespace VanillaTwist.MEV
{
    public class ValeursPossibles
    {
        /// <summary>
        /// Valeurs autorisées ou standards pour la Configuration des différents éléments.<br />
        /// Allowed or standard values for the Configuration of the different elements.<br />
        /// <br />
        /// Utilisation<br />
        ///     String UrlTransactions = ValeursPossibles.UrlMEVWEB.Transactions;
        ///
        /// </summary>
        public static class UrlMEVWEB
        {
            /// <summary>
            /// Adresse web à encoder dans le code QR
            /// Web address to encode in the QR code
            /// </summary>
            //public static String QRCode { get { return "https://cnfr.acc.qr.mev-web.ca?f="; } }  // url for development environment
            //public static String QRCode { get { return "https://cnfr.qr.mev-web.ca?f="; } }  // url for development environment
            //public static String QRCode { get { return "https://qr.mev-web.ca?f="; } }     // url for PROD environment
            static NameValueCollection appStgs = ConfigurationManager.AppSettings;
            public static String QRCode { get { return appStgs["UrlMEVWEB.QRCode"]; } }
            /// <summary>
            /// Url de base du MEV-WEB<br />
            /// Basic url of the WEB-SRM<br />
            /// </summary>
            //private static String UrlBaseMEVWEB { get { return "https://cnfr.acc.api.rq-fo.ca"; } }  // url for ACC environment
            //private static String UrlBaseMEVWEB { get { return "https://cnfr.api.rq-fo.ca"; } }  // url for development environment
            //private static String UrlBaseMEVWEB { get { return "https://api.rq-fo.ca"; } }         // url for PROD environment
            public static String UrlBaseMEVWEB { get { return appStgs["UrlMEVWEB.UrlBaseMEVWEB"]; } }

            /// <summary>
            /// Url pour l'obtention des certificats du SEV et du MEV-WEB<br />
            /// Url for obtaining SRS/POS and WEB-SRM certificates
            /// </summary>
            //public static String Enrolement { get { return "https://certificats.cnfr.acc.api.rq-fo.ca/enrolement"; } }  // url for ACC environment
            //public static String Enrolement { get { return "https://certificats.cnfr.api.rq-fo.ca/enrolement"; } }  // url for development environment
            //public static String Enrolement { get { return "https://certificats.api.rq-fo.ca/enrolement"; } }         // url for PROD environment
            public static String Enrolement { get { return appStgs["UrlMEVWEB.Enrolement"]; } }

            /// <summary>
            /// Url pour ajouter/supprimer ou remplacer un certificat du SEV au MEV-WEB<br />
            /// Url to add/remove or replace an SEV certificate to WEB-SRM
            /// </summary>
            public static String Certificats { get { return UrlBaseMEVWEB + "/certificats"; } }

            /// <summary>
            /// Url pour ajouter/supprimer ou valider un utilisateur au MEV-WEB<br />
            /// Url to add/remove or validate a user to WEB-SRM
            /// </summary>
            public static String Utilisateurs { get { return UrlBaseMEVWEB + "/utilisateur"; } }

            /// <summary>
            /// Url pour transmettre une transaction au MEV-WEB<br />
            /// Url to transmit a transaction to WEB-SRM
            /// </summary>
            public static String Transactions { get { return UrlBaseMEVWEB + "/transaction"; } }

            /// <summary>
            /// Url pour transmettre divers documents au MEV-WEB<br />
            /// Url to transmit various documents to the WEB-SRM
            /// </summary>
            public static String Document { get { return UrlBaseMEVWEB + "/document"; } }
        }

        public static class UrlMEVWEBDevEssai
        {
            /// <summary>
            /// Adresse web à encoder dans le code QR
            /// Web address to encode in the QR code
            /// </summary>
            //public static String QRCode { get { return "https://cnfr.acc.qr.mev-web.ca?f="; } }  // url for development environment
            public static String QRCode { get { return "https://cnfr.qr.mev-web.ca?f="; } }  // url for development environment
            //public static String QRCode { get { return "https://qr.mev-web.ca?f="; } }     // url for PROD environment

            /// <summary>
            /// Url de base du MEV-WEB<br />
            /// Basic url of the WEB-SRM<br />
            /// </summary>
            //private static String UrlBaseMEVWEB { get { return "https://cnfr.acc.api.rq-fo.ca"; } }  // url for ACC environment
            private static String UrlBaseMEVWEB { get { return "https://cnfr.api.rq-fo.ca"; } }  // url for development environment
            //private static String UrlBaseMEVWEB { get { return "https://api.rq-fo.ca"; } }         // url for PROD environment

            /// <summary>
            /// Url pour l'obtention des certificats du SEV et du MEV-WEB<br />
            /// Url for obtaining SRS/POS and WEB-SRM certificates
            /// </summary>
            //public static String Enrolement { get { return "https://certificats.cnfr.acc.api.rq-fo.ca/enrolement"; } }  // url for ACC environment
            public static String Enrolement { get { return "https://certificats.cnfr.api.rq-fo.ca/enrolement"; } }  // url for development environment
            //public static String Enrolement { get { return "https://certificats.api.rq-fo.ca/enrolement"; } }         // url for PROD environment

            /// <summary>
            /// Url pour ajouter/supprimer ou remplacer un certificat du SEV au MEV-WEB<br />
            /// Url to add/remove or replace an SEV certificate to WEB-SRM
            /// </summary>
            public static String Certificats { get { return UrlBaseMEVWEB + "/certificats"; } }

            /// <summary>
            /// Url pour ajouter/supprimer ou valider un utilisateur au MEV-WEB<br />
            /// Url to add/remove or validate a user to WEB-SRM
            /// </summary>
            public static String Utilisateurs { get { return UrlBaseMEVWEB + "/utilisateur"; } }

            /// <summary>
            /// Url pour transmettre une transaction au MEV-WEB<br />
            /// Url to transmit a transaction to WEB-SRM
            /// </summary>
            public static String Transactions { get { return UrlBaseMEVWEB + "/transaction"; } }

            /// <summary>
            /// Url pour transmettre divers documents au MEV-WEB<br />
            /// Url to transmit various documents to the WEB-SRM
            /// </summary>
            public static String Document { get { return UrlBaseMEVWEB + "/document"; } }
        }


        /// <summary>
        /// Le champ identifiant le secteur d’activité de l’entreprise.<br />
        /// Abbreviation of the business’s sector of activity.<br />
        /// <br />
        /// Utilisation<br />
        ///     String abrvt = ValeursPossibles.Abrvt.TRP;<br />
        ///
        /// </summary>
        public static class Abrvt
        {
            /// <summary>
            /// Transport rémunéré de personnes<br />
            /// Remunerated passenger transportation
            /// </summary>
            public static String TRP { get { return "TRP"; } }

            /// <summary>
            /// Restaurants, bars et camions de restauration<br />
            /// Restaurants, bars and food trucks
            /// </summary>
            public static String RBC { get { return "RBC"; } }
        }

        /// <summary>
        /// Le champ identifiant le secteur d’activité de l’entreprise.<br />
        /// Field identifying the business's sector of activity<br />
        /// <br />
        /// Utilisation<br />
        ///     String acti = ValeursPossibles.Acti.TRP;
        ///
        /// </summary>
        public static class Acti
        {
            /// <summary>
            /// Transport rémunéré de personnes<br />
            /// Remunerated passenger transportation
            /// </summary>
            public static String TRP { get { return "TRP"; } }

            /// <summary>
            /// Restaurant
            /// </summary>
            public static String RES { get { return "RES"; } }

            /// <summary>
            /// Bar
            /// </summary>
            public static String BAR { get { return "BAR"; } }

            /// <summary>
            /// Camion de restauration<br />
            /// Food Truck
            /// </summary>
            public static String CDR { get { return "CDR"; } }

            /// <summary>
            /// Tiers habituel<br />
            /// Usual third party
            /// </summary>
            public static String HAB { get { return "HAB"; } }

            /// <summary>
            /// Secteur d’activité non visé par les mesures<br />
            /// Sector of activity not covered by the measures
            /// </summary>
            public static String NON { get { return "NON"; } }

            /// <summary>
            /// Sans Objet<br />
            /// N/A
            /// </summary>
            public static String SOB { get { return "SOB"; } }
        }

        /// <summary>
        /// Type d'appareil qui a initié la requête<br />
        /// Type of device that initiated the request<br />
        /// <br />
        /// Utilisation<br />
        ///    String envr = ValeursPossibles.ApprlInit.SEV;
        ///    
        /// </summary>
        public static class ApprlInit
        {
            /// <summary>
            /// Système d'enregistrement des ventes<br />
            /// Point of Sales System
            /// </summary>
            public static String SEV { get { return "SEV"; } }

            /// <summary>
            /// Serveur<br />
            /// Server
            /// </summary>
            public static String SRV { get { return "SRV"; } }
        }

        /// <summary>
        /// Environnement visé pour la requête<br />
        /// Environment used to make the request<br />
        /// <br />
        /// Utilisation<br />
        ///    String envr = ValeursPossibles.Envirn.DEV;
        ///    
        /// </summary>
        public static class Envirn
        {
            /// <summary>
            /// Développement<br />
            /// Development
            /// </summary>
            public static String DEV { get { return "DEV"; } }

            /// <summary>
            /// Cas d'essais (Certification)<br />
            /// Test cases
            /// </summary>
            public static String ESSAI { get { return "ESSAI"; } }

            /// <summary>
            /// Production
            /// </summary>
            public static String PROD { get { return "PROD"; } }
        }

        /// <summary>
        /// Le type de support d’impression du document.<br />
        /// Document print option<br />
        ///<br />
        /// Utilisation<br />
        ///     String formImpr = ValeursPossibles.FormImpr.PAP;
        ///
        /// </summary>
        public static class FormImpr
        {
            /// <summary>
            /// Papier<br />
            /// Paper
            /// </summary>
            public static String PAP { get { return "PAP"; } }

            /// <summary>
            /// Électronique<br />
            /// Electronic
            /// </summary>
            public static String ELE { get { return "ELE"; } }

            /// <summary>
            /// Papier et électronique<br />
            /// Combined paper and electronic
            /// </summary>
            public static String PEL { get { return "PEL"; } }

            /// <summary>
            /// Non imprimé<br />
            /// Not printed
            /// </summary>
            public static String NON { get { return "NON"; } }

            /// <summary>
            /// Sans objet<br />
            /// N/A
            /// </summary>
            [Obsolete( "\nLa valeur SOB a été retirée. La valeur NON doit être celle par défaut.\nThe SOB value has been removed. The default value should be NON.", true )]
            public static String SOB { get { return "SOB"; } }
        }

        /// <summary>
        /// Le champ indiquant si la modification concerne l’ajout, la suppression ou la validation des numéros d’inscription aux fichiers de la TPS et de la TVQ attribués au mandataire.<br />
        /// Field indicating whether the change concerns the addition, deletion or replacement of a certificate or a user<br />
        /// <br />
        /// Utilisation<br />
        ///     String modif = ValeursPossibles.Modif.AJO;
        ///
        /// </summary>
        public static class Modif
        {
            /// <summary>
            /// Ajout d'un certificat ou d'un utilisateur<br />
            /// Addition of a certificate or a user
            /// </summary>
            public static String AJO { get { return "AJO"; } }

            /// <summary>
            /// Remplacement d'un certificat<br />
            /// Remplacement of a certificate or a user
            /// </summary>
            public static String REM { get { return "REM"; } }

            /// <summary>
            /// Suppression d'un certificat ou d'un utilisateur<br />
            /// Deletion of a certificate or a user
            /// </summary>
            public static String SUP { get { return "SUP"; } }

            /// <summary>
            /// Validation d'un utilisateur<br />
            /// Validation of a user
            /// </summary>
            public static String VAL { get { return "VAL"; } }
        };

        /// <summary>
        /// Le mode d’impression du document<br />
        /// Print mode for the document.<br />
        ///<br />
        /// Utilisation<br />
        ///     String modImpr = ValeursPossibles.ModImpr.FAC;
        ///
        /// </summary>
        public static class ModImpr
        {
            /// <summary>
            /// Annulation<br />
            /// Cancellation
            /// </summary>
            public static String ANN { get { return "ANN"; } }

            /// <summary>
            /// Reproduction<br />
            /// Reproduction
            /// </summary>
            public static String RPR { get { return "RPR"; } }

            /// <summary>
            /// Duplicata<br />
            /// Duplicate
            /// </summary>
            public static String DUP { get { return "DUP"; } }

            /// <summary>
            /// Parti sans payer<br />
            /// Failure to pay
            /// </summary>
            public static String PSP { get { return "PSP"; } }

            /// <summary>
            /// Facture ou tout autre document non inclus dans cette liste<br />
            /// Bill or any other document not included in this list
            /// </summary>
            public static String FAC { get { return "FAC"; } }

            /// <summary>
            /// Sans objet<br />
            /// N/A
            /// </summary>
            public static String SOB { get { return "SOB"; } }
        }

        /// <summary>
        /// Le mode de paiement utilisé pour acquitter la facture.<br />
        /// Method of payment used for the bill.<br />
        ///<br />
        /// Utilisation<br />
        ///     String modPai = ValeursPossibles.ModPai.ARG;
        ///
        /// </summary>
        public static class ModPai
        {
            /// <summary>
            /// Argent comptant<br />
            /// Cash
            /// </summary>
            public static String ARG { get { return "ARG"; } }

            /// <summary>
            /// Chèque<br />
            /// Cheque
            /// </summary>
            public static String CHQ { get { return "CHQ"; } }

            /// <summary>
            /// Coupon<br />
            /// Coupon
            /// </summary>
            public static String COU { get { return "COU"; } }

            /// <summary>
            /// Carte de crédit<br />
            /// Credit card
            /// </summary>
            public static String CRE { get { return "CRE"; } }

            /// <summary>
            /// Carte de débit<br />
            /// Debit card
            /// </summary>
            public static String DEB { get { return "DEB"; } }

            /// <summary>
            /// Certificat-cadeau (p.ex., carte prépayée, chèque-cadeau)<br />
            /// Prepaid card or Gift card
            /// </summary>
            public static String CPR { get { return "CPR"; } }

            /// <summary>
            /// Programme de fidélisation<br />
            /// Loyalty program
            /// </summary>
            public static String FID { get { return "FID"; } }

            /// <summary>
            /// Cryptomonnaie (Bitcoins, ...)<br />
            /// Cryptocurrency (Bitcoins, ...)
            /// </summary>
            public static String CRY { get { return "CRY"; } }

            /// <summary>
            /// Portefeuille électronique (PayPal, ...)<br />
            /// Digital wallet (PayPal, ...)
            /// </summary>
            public static String MVO { get { return "MVO"; } }

            /// <summary>
            /// Transfert de fonds<br />
            /// transfer of funds
            /// </summary>
            public static String TFD { get { return "TFD"; } }

            /// <summary>
            /// Paiement mixte<br />
            /// Mixed payment
            /// </summary>
            public static String MIX { get { return "MIX"; } }

            /// <summary>
            /// Porté au compte<br />
            /// Charge to account
            /// </summary>
            public static String PAC { get { return "PAC"; } }

            /// <summary>
            /// Autre<br />
            /// other
            /// </summary>
            public static String AUT { get { return "AUT"; } }

            /// <summary>
            /// Inconnu<br />
            /// Unknown
            /// </summary>
            public static String INC { get { return "INC"; } }

            /// <summary>
            /// Aucun paiement<br />
            /// No payment
            /// </summary>
            public static String AUC { get { return "AUC"; } }

            /// <summary>
            /// Sans objet<br />
            /// N/A
            /// </summary>
            public static String SOB { get { return "SOB"; } }
        }

        /// <summary>
        /// Le mode de transaction utilisé<br />
        /// Transaction mode used<br />
        ///<br />
        /// Utilisation<br />
        ///     String modTrans = ValeursPossibles.ModTrans.OPE;
        ///
        /// </summary>
        public static class ModTrans
        {
            /// <summary>
            /// Opérationnel<br />
            /// Operation
            /// </summary>
            public static String OPE { get { return "OPE"; } }

            /// <summary>
            /// Formation<br />
            /// Training
            /// </summary>
            public static String FOR { get { return "FOR"; } }
        }

        /// <summary>
        /// Le type de relation d’affaires.<br />
        /// The type of business relationship.<br />
        ///<br />
        /// Utilisation<br />
        ///     String RelaCommer = ValeursPossibles.RelaCommer.B2C;
        ///
        /// </summary>
        public static class RelaCommer
        {
            /// <summary>
            /// Entreprise à entreprise<br />
            /// Business to business
            /// </summary>
            public static String B2B { get { return "B2B"; } }

            /// <summary>
            /// Entreprise à consommateur<br />
            /// Business to consumer
            /// </summary>
            public static String B2C { get { return "B2C"; } }

            /// <summary>
            /// Entreprise à gouvernement<br />
            /// business to government
            /// </summary>
            public static String B2G { get { return "B2G"; } }
        }

        /// <summary>
        /// Un indicateur que la taxe s’applique pour la fourniture payée ou payable.<br />
        /// Indicator that tax applies to each supply paid or payable.<br />
        ///<br />
        /// Utilisation<br />
        ///     String tax = ValeursPossibles.Tax.FP;
        ///
        /// </summary>
        public static class Tax
        {
            /// <summary>
            /// Fédérale (TPS)<br />
            /// Federal (GST)
            /// </summary>
            public static String F { get { return "F"; } }

            /// <summary>
            /// Provinciale (TVQ)<br />
            /// Provincial (QST)
            /// </summary>
            public static String P { get { return "P"; } }

            /// <summary>
            /// Fédérale et provinciale (TPS et TVQ)<br />
            /// Federal and provincial (GST and QST)
            /// </summary>
            public static String FP { get { return "FP"; } }

            /// <summary>
            /// Non taxable<br />
            /// Non-taxable
            /// </summary>
            public static String NON { get { return "NON"; } }

            /// <summary>
            /// Sans objet<br />
            /// N/A
            /// </summary>
            public static String SOB { get { return "SOB"; } }
        }

        /// <summary>
        /// Le type d’adresse du client<br />
        /// Customer’s address type<br />
        /// <br />
        /// Utilisation<br />
        ///     String typAdr = ValeursPossibles.TypAdr.EXP;
        /// 
        /// </summary>
        public static class TypAdr
        {
            /// <summary>
            /// Lieu d'exploitation<br />
            /// Place of business
            /// </summary>
            public static String EXP { get { return "EXP"; } }

            /// <summary>
            /// Service rendu<br />
            /// Service provided
            /// </summary>
            public static String SRV { get { return "SRV"; } }

            /// <summary>
            /// Domicile<br />
            /// Residence
            /// </summary>
            [Obsolete( "Please use EXP or SRV instead. Veuillez plutôt utiliser EXP ou SRV" )]
            public static String DOM { get { return "DOM"; } }

            /// <summary>
            /// Facturation<br />
            /// Billing
            /// </summary>
            [Obsolete( "Please use EXP or SRV instead. Veuillez plutôt utiliser EXP ou SRV" )]
            public static String FAC { get { return "FAC"; } }

            /// <summary>
            /// Livraison<br />
            /// Delivery
            /// </summary>
            [Obsolete( "Please use EXP or SRV instead. Veuillez plutôt utiliser EXP ou SRV" )]
            public static String LVR { get { return "LVR"; } }
        }

        /// <summary>
        /// Le champ identifiant le type de document transmis.<br />
        /// Field identifying the type of document sent.<br />
        /// <br />
        /// Utilisation<br />
        ///     String typServ = ValeursPossibles.TypDoc.RUT;
        /// 
        /// </summary>
        public static class TypDoc
        {
            /// <summary>
            /// Rapport de l’utilisateur<br />
            /// User report
            /// </summary>
            public static String RUT { get { return "RUT"; } }

            /// <summary>
            /// Tiers habituel<br />
            /// Usual third party
            /// </summary>
            public static String HAB { get { return "HAB"; } }
        }

        /// <summary>
        /// Type de service<br />
        /// Service types<br />
        /// <br />
        /// Utilisation<br />
        ///     String typServ = ValeursPossibles.TypServ.LVR;
        /// </summary>
        public static class TypServ
        {
            /// <summary>
            /// Mode comptoir<br />
            /// Counter service
            /// </summary>
            public static String CMP { get { return "CMP"; } }

            /// <summary>
            /// Livraison par le mandataire<br />
            /// Restaurant delivery
            /// </summary>
            public static String LVR { get { return "LVR"; } }

            /// <summary>
            /// livraison par une plateforme électronique<br />
            /// Electronic platform delivery
            /// </summary>
            public static String LVT { get { return "LVT"; } }

            /// <summary>
            /// Service aux tables<br />
            /// Table service
            /// </summary>
            public static String TBL { get { return "TBL"; } }
        }

        /// <summary>
        /// Le type de transaction<br />
        /// Transaction type<br />
        ///<br />
        /// Utilisation<br />
        ///     String typTrans = ValeursPossibles.TypTrans.ADDI;
        ///
        /// </summary>
        public static class TypTrans
        {
            /// <summary>
            /// Le type de transaction : Addition<br />
            /// Transaction type : Temporary bill
            /// </summary>
            public static String ADDI { get { return "ADDI"; } }

            /// <summary>
            /// Le type de transaction : Estimation<br />
            /// Transaction type : Estimate
            /// </summary>
            public static String ESTM { get { return "ESTM"; } }

            /// <summary>
            /// Le type de transaction : Reçu de fermeture<br />
            /// Transaction type : Closing receipt
            /// </summary>
            public static String RFER { get { return "RFER"; } }

            /// <summary>
            /// Le type de transaction : Soumission<br />
            /// Transaction type : Quote
            /// </summary>
            public static String SOUM { get { return "SOUM"; } }

            /// <summary>
            /// Le type de transaction : Tiers inhabituel<br />
            /// Transaction type : Occasional third party
            /// </summary>
            public static String TIER { get { return "TIER"; } }

            /// <summary>
            /// Le type de transaction : Sans objet<br />
            /// Transaction type : n/a
            /// </summary>
            public static String SOB { get { return "SOB"; } }
        }

        /// <summary>
        /// Champ TS du rapport de l'utilisateur<br />
        /// User report TS field
        /// </summary>
        public static class TS
        {
            /// <summary>
            /// Si le sommaire des ventes est calculé pour un seul SEV, inscrire "A"<br />
            /// If the sales summary is calculated for a single SRS, enter "A"
            /// </summary>
            public static String A { get { return "A"; } }

            /// <summary>
            /// Si le sommaire des ventes est calculé pour l'ensemble des SEV de l'établissement, inscrire "E"<br />
            /// If the sales summary is calculated for all the establishment's SRS, enter "E".
            /// </summary>
            public static String E { get { return "E"; } }
        }

        /// <summary>
        /// Champ CM du rapport de l'utilisateur<br />
        /// User report CM field
        /// </summary>
        public static class CM
        {
            /// <summary>
            /// Si les ventes correspondent à un seul utilisateur, inscrire "Unique"<br />
            /// If sales correspond to a single user, enter "Unique".
            /// </summary>
            public static String UNIQUE { get { return "Unique"; } }

            /// <summary>
            /// Si les ventes correspondent à tous les utilisateurs de l'établissement, inscrire "Tous"<br />
            /// If sales correspond to all users of the establishment, enter "Tous".
            /// </summary>
            public static String TOUS { get { return "Tous"; } }
        }
    }
}
