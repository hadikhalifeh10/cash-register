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
    /// Requête Document - Tiers Habituel
    /// DOCUMENT Request - Frequent third party report structure
    /// </summary>
    class DocumentHAB
    {
        /// <summary>
        /// Numéro d’inscription au fichier de la TVQ
        /// Frequent third party’s QST registration number
        /// </summary>
        public String TQ { get; set; }

        /// <summary>
        /// Nom et prénom du particulier ou nom de l’entité
        /// Last name and first name of the individual or name of the entity
        /// </summary>
        public String NM { get; set; }

        /// <summary>
        /// Adresse du tiers habituel
        /// Frequent third party’s address
        /// </summary>
        public String AD { get; set; }

        /// <summary>
        /// Code postal du tiers habituel
        /// Frequent third party’s postal code
        /// </summary>
        public String CP { get; set; }

        /// <summary>
        /// Indicatif régional et numéro de téléphone du tiers habituel
        /// Frequent third party’s area code and phone number
        /// </summary>
        public String TE { get; set; }

        /// <summary>
        /// Numéro du poste (téléphone) du tiers habituel
        /// Frequent third party’s extension (phone)
        /// </summary>
        public String PO { get; set; }

        /// <summary>
        /// Numéro d’entreprise du Québec du tiers habituel
        /// Frequent third party’s Québec entreprise number (NEQ)
        /// </summary>
        public String NE { get; set; }

        /// <summary>
        /// Raison de la déclaration relatif au contrat (choix : 1 = Nouveau, 2 = Modification, 3 = Expiration)
        /// Reason for reporting the contract (choice: 1 = New, 2 = Amendment, 3 = Expiration)
        /// </summary>
        public String RA { get; set; }

        /// <summary>
        /// Date de la conclusion ou de la modification du contrat
        /// Date of conclusion or modification of the contract
        /// </summary>
        public String DC { get; set; }

        /// <summary>
        /// Date d'entrée en vigueur du contrat ou de la modification du contrat
        /// Date of entry into force of the contract or of the modification of the contract
        /// </summary>
        public String DV { get; set; }

        /// <summary>
        /// Description des biens et services qui sont fournis ou l’objet de la modification du contrat
        /// Description of the property and services being provided or the subject of the contract amendment
        /// </summary>
        public String DE { get; set; }

        /// <summary>
        /// Description des biens et services qui sont fournis ou l’objet de la modification du contrat
        /// Description of the property and services being provided or the subject of the contract amendment
        /// </summary>
        public String BS { get; set; }

        /// <summary>
        /// Fréquence à laquelle les biens et services sont fournis
        /// Frequency with which property and services are provided
        /// </summary>
        public String FR { get; set; }

        /// <summary>
        ///  Le moment de la journée où les biens et services sont fournis (choix : 1 = Jour, 2 = Soir, 3 = Nuit)
        ///  Time the goods and services are provided (choice: 1 = Day, 2 = Evening, 3 = Night)
        /// </summary>
        public String MO { get; set; }

        /// <summary>
        /// Signature numérique du Rapport de l’utilisateur
        /// Operator’s digital signature
        /// </summary>
        public String SI { get; set; }

        /// <summary>
        /// Empreinte du certificat numérique associé à la clé privée qui a généré la signature numérique
        /// Fingerprint of the digital certificate associated with the private key that generated the digital signature
        /// </summary>
        public String EM { get; set; }

        /// <summary>
        /// Constructeur
        /// </summary>
        public DocumentHAB( ) { }

        /// <summary>
        /// Concaténation des informations pour calcul de la signature ou pour transmission du document au MEV-WEB
        /// Concatenation of information to calculate the signature or to transmit the document to WEB-SRM
        /// </summary>
        /// <returns>Informations concaténées
        ///          Concatenated data</returns>
        public String GetDocumentConcatene( )
        {
            StringBuilder s = new StringBuilder( );
            s.AppendFormat( "TQ={0};", TQ );
            s.AppendFormat( "NM={0};", NM );
            s.AppendFormat( "AD={0};", AD );
            s.AppendFormat( "CP={0};", CP );
            s.AppendFormat( "TE={0};", TE );
            s.AppendFormat( "PO={0};", PO );
            s.AppendFormat( "NE={0};", NE );
            s.AppendFormat( "RA={0};", RA );
            s.AppendFormat( "DC={0};", DC );
            s.AppendFormat( "DV={0};", DV );
            s.AppendFormat( "DE={0};", DE );
            s.AppendFormat( "BS={0};", BS );
            s.AppendFormat( "FR={0};", FR );
            s.AppendFormat( "EM={0};", EM );

            if ( SI == "" )
                // Concaténation pour calcul de la signature
                // Concatenation for signature calculation
                s.AppendFormat( "MO={0}", MO );
            else
            {
                // Concaténation pour transmettre le document au MEV-WEB
                // Concatenation to transmit the document to WEB-SRM
                s.AppendFormat( "MO={0};", MO );
                s.AppendFormat( "SI={0}", SI );
            }

            return s.ToString( );
        }

    }
}
