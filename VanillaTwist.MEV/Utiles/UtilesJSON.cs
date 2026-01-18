using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace VanillaTwist.MEV
{
    /// <summary>
    /// Creation of json documents to transmit to WEB-SRM
    /// Extraction of information contained in json documents received from WEB-SRM
    /// 
    /// Création de documents json pour transmettre au MEV-WEB
    /// Extraction des informations contenus dans les documents json reçus du MEV-WEB
    /// 
    /// The following packages are used and can be installed via the NuGet console/tool
    /// Les packages suivants sont utilisés et peuvent être installés via la console/outils NuGet
    /// 
    /// - Newtonsoft.Json
    /// - RestSharp
    /// 
    /// </summary>
    public class UtilesJSON
    {
        #region Certificates

        /// <summary>
        /// Request to add a certificate
        /// Requete pour ajouter un certificat
        /// </summary>
        /// <param name="HTTPHeadersDict">Headers to be sent to WEB-SRM
        ///                               Entêtes à transmettre au MEV-WEB</param>
        /// <param name="Json">Json document to be sent to the WEB-SRM
        ///                    Document json à transmettre au MEV-WEV</param>
        /// <returns>WEB-SRM's response
        ///          Réponse du MEVWEB</returns>
        public static WEBSRM_Response CertificatesRequest(List<KeyValuePair<String, String>> HTTPHeadersDict, String Json)
        {
            return CertificatesRequest(ValeursPossibles.Modif.AJO, HTTPHeadersDict, Json, null);
        }

        /// <summary>
        /// Request to add, replace or delete a certificate
        /// Requete pour ajouter, remplacer ou supprimer un certificat
        /// </summary>
        /// <param name="Modif">The field indicates whether the modification concerns an addition, replacement or deletion of a certificate
        ///                     Le champ indiquantsi la modification concerne un ajout, un remplacement ou une suppression de certificat</param>
        /// <param name="HTTPHeadersDict">Entêtes à transmettre au MEV-WEB
        ///                                   Headers to be sent to the WEB-SRM</param>
        /// <param name="Json">Json document to be sent to the WEB-SRM
        ///                    Document json à transmettre au MEV-WEB</param>
        /// <param name="CertificateSerialNumberSRS">The field containing the serial number of the certificate when requesting a replacement ("REM") or deletion ("SUP"). Add null for certificate addition
        ///                                        Le champ contenant le numéro de série du certificat lors d’une demande de remplacement (« REM ») ou de suppression (« SUP »). Ajouter null pour ajout de certificat</param>
        /// <returns>Réponse du MEVWEB
        ///          WEB-SRM's response</returns>
        public static WEBSRM_Response CertificatesRequest(String Modif, List<KeyValuePair<String, String>> HTTPHeadersDict, String Json, String CertificateSerialNumberSRS)
        {
            WEBSRM_Response response;

            if (Modif == ValeursPossibles.Modif.AJO)
                response = SendJson(ValeursPossibles.UrlMEVWEB.Enrolement, HTTPHeadersDict, Json, null);
            else
                response = SendJson(ValeursPossibles.UrlMEVWEB.Certificats, HTTPHeadersDict, Json, CertificateSerialNumberSRS);

            return response;
        }

        /// <summary>
        /// Returns the json document for a request of type Certificate
        /// Retourne le document json pour une requête de type Certificat
        /// </summary>
        /// <param name="Modif">The field indicates whether the modification concerns an addition, a replacement or a deletion of a certificate
        ///                     Le champ indiquantsi la modification concerne un ajout, un remplacement ou une suppression de certificat</param>
        /// <param name="CSR">The field containing the certificate signing request for an add ("AJO") or replace ("REM") request
        ///                   Le champ contenant la demande de signature de certificat lors d’une demande d’ajout (« AJO ») ou de remplacement (« REM »)</param>
        /// <param name="CertificateSerialNumberSRS">The field containing the serial number of the certificate when requesting a replacement ("REM") or removal ("SUP")
        ///                           Le champ contenant le numéro de série du certificat lors d’une demande de remplacement (« REM ») ou de suppression (« SUP »)</param>
        /// <returns>Document json</returns>
        public static String GetJsonCertificate(String Modif, String CSR, String CertificateSerialNumberSRS)
        {
            StringBuilder s = new StringBuilder();
            s.AppendLine("{");
            s.AppendLine("   \"reqCertif\": {");
            s.AppendFormat("      \"modif\": \"{0}\",", Modif).AppendLine();

            if (Modif.ToUpper() == ValeursPossibles.Modif.AJO)
                s.AppendFormat("      \"csr\": \"{0}\"", CSR).AppendLine();

            if (Modif.ToUpper() == ValeursPossibles.Modif.REM)
                s.AppendFormat("      \"csr\": \"{0}\",", CSR).AppendLine();

            if (CertificateSerialNumberSRS != null)
                s.AppendFormat("      \"noSerie\": \"{0}\"", CertificateSerialNumberSRS).AppendLine();

            s.AppendLine("   }");
            s.AppendLine("}");
            return s.ToString();
        }

        /// <summary>
        /// Parse the response of a request of type Certificats
        /// Analyse la réponse d'une requête de type Certificats
        /// </summary>
        /// <param name="JSon">JSon document containing the WEB-SRM response
        ///                    Document JSon contenant la réponse du MEV-WEB</param>
        /// <returns>Certificates received from the WEB-SRM
        ///          Certificats reçus du MEV-WEB</returns>
        public static ResponseCertificats ParseResponseCertificates(String JSon)
        {
            try
            {
                if (JSon.Contains("listErr"))
                {
                    Console.Error.WriteLine(IndentJson(JSon));
                    return null;
                }

                dynamic json = (JObject)JsonConvert.DeserializeObject(JSon);

                String idApprl = "";
                if (json.retourCertif.idApprl != null)
                    idApprl = json.retourCertif.idApprl;

                String prochainCasEssai = "";
                if (json.retourCertif.casEssai != null)
                    prochainCasEssai = json.retourCertif.casEssai;

                String certifPSI = "";
                if (json.retourCertif.certifPSI != null)
                    certifPSI = json.retourCertif.certifPSI;

                String certif = "";
                if (json.retourCertif.certif != null)
                    certif = json.retourCertif.certif;

                return (new ResponseCertificats(prochainCasEssai, certifPSI, certif, idApprl));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return null;
            }
        }
        #endregion Certificates

        #region Users
        /// <summary>
        /// Request to add, replace or delete a User
        /// Requete pour ajouter, remplacer ou supprimer un Utilisateur
        /// </summary>
        /// <param name="HTTPHeadersDict">Headers to be sent to the WEB-SRM
        ///                               Entêtes à transmettre au MEV-WEB</param>
        /// <param name="Json">Json document to be sent to the WEB-SRM
        ///                    Document json à transmettre au MEV-WEB</param>
        /// <param name="CertificateSerialNumberSRS">Serial number of the SRM/POS's certificate to be used to authenticate to the WEB-SRM
        ///                                          Numéro de série du certificat du SEV à utiliser pour s'authentifier auprès du MEV-WEB</param>
        /// <returns>WEB-SRM's response
        ///          Réponse du MEVWEB</returns>
        public static WEBSRM_Response UserRequest(List<KeyValuePair<String, String>> HTTPHeadersDict, String Json, String CertificateSerialNumberSRS)
        {
            return SendJson(ValeursPossibles.UrlMEVWEB.Utilisateurs, HTTPHeadersDict, Json, CertificateSerialNumberSRS);
        }

        /// <summary>
        /// Returns the json document for a request of type Utilisateur
        /// Retourne le document json pour une requête de type Utilisateur
        /// </summary>
        /// <param name="Modif">Le champ indiquantsi la modification concerne l’ajout, la suppression ou la validation des numéros d’inscription aux fichiers de la TPS et de la TVQ attribués au mandataire
        ///                     The field indicating whether the change concerns the addition, deletion or validation of the GST and QST registration numbers assigned to the mandatory</param>
        /// <param name="NomUtil">Le nom de l’utilisateur qui produit la facture au client
        ///                       The name of the user who produces the invoice to the customer</param>
        /// <param name="NoTPS">Le numéro d’inscription au fichier de la TPS attribué au mandataire
        ///                     The GST registration number assigned to the mandatory</param>
        /// <param name="NoTVQ">Le numéro d’inscription au fichier de la TVQ attribué au mandataire
        ///                     The QST registration number assigned to the mandatory</param>
        /// <returns>Document json</returns>
        public static String GetJsonUser(String Modif, String NomUtil, String NoTPS, String NoTVQ)
        {
            StringBuilder s = new StringBuilder();

            s.Append("{");
            s.Append("\"reqUtil\": {");
            s.AppendFormat("\"modif\": \"{0}\",", Modif);

            if (Modif.ToUpper() == ValeursPossibles.Modif.AJO || Modif.ToUpper() == ValeursPossibles.Modif.SUP)
                s.AppendFormat("\"nomUtil\": \"{0}\",", NomUtil);

            s.Append("\"noTax\": {");
            s.AppendFormat("\"noTPS\": \"{0}\",", NoTPS);
            s.AppendFormat("\"noTVQ\": \"{0}\"", NoTVQ);
            s.Append("}");
            s.Append("}");
            s.Append("}");

            return s.ToString();
        }

        /// <summary>
        /// Parse the response of a request of type Utilisateur
        /// Analyse la réponse d'une requête de type Utilisateur
        /// </summary>
        /// <param name="JSon">Document json reçu du MEVWEB
        ///                    json document received from the WEB-SRM</param>
        /// <returns>Réponse reçue du MEV-WEB
        ///          Response received from the WEB-SRM</returns>

        public static ResponseUtilisateur ParseResponseUtilisateur(String JSon)
        {
            try
            {
                dynamic json = (JObject)JsonConvert.DeserializeObject(JSon);

                String prochainCasEssai = "";
                if (json.retourUtil.casEssai != null)
                    prochainCasEssai = json.retourUtil.casEssai;

                String NoTPS = "";
                String NoTVQ = "";
                if (json.retourUtil.noTax != null)
                {
                    NoTPS = json.retourUtil.noTax.noTPS;
                    NoTVQ = json.retourUtil.noTax.noTVQ;
                }

                return (new ResponseUtilisateur(prochainCasEssai, NoTPS, NoTVQ));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return null;
            }
        }

        #endregion Users

        #region Transactions
        /// <summary>
        /// Transmission d'une transaction au MEVWEB
        /// </summary>
        /// <param name="HTTPHeadersDict">Headers to be sent to the WEB-SRM
        ///                               Entêtes à transmettre au MEV-WEB</param>
        /// <param name="Json">Json document to be sent to the WEB-SRM
        ///                    Document json à transmettre au MEV-WEB</param>
        /// <param name="CertificateSerialNumberSRS">Serial number of the SRM/POS's certificate to be used to authenticate to the WEB-SRM
        ///                                          Numéro de série du certificat du SEV à utiliser pour s'authentifier auprès du MEV-WEB</param>
        /// <returns>Response received from the WEB-SRM
        ///          Réponse reçue du MEV-WEB</returns>
        public static WEBSRM_Response TransactionRequest(List<KeyValuePair<String, String>> HTTPHeadersDict, String Json, String CertificateSerialNumberSRS)
        {
            return SendJson(ValeursPossibles.UrlMEVWEB.Transactions, HTTPHeadersDict, Json, CertificateSerialNumberSRS);
        }

        /// <summary>
        /// Parses the response of a request of type Transaction
        /// Analyse la réponse d'une requête de type Transaction
        /// </summary>
        /// <param name="JSon">json document received from WEB-SRM
        ///                    Document json reçu du MEV-WEB</param>
        /// <returns>Response received from the WEB-SRM
        ///          Réponse reçue du MEV-WEB</returns>
        public static ResponseTransaction ParseResponseTransaction(String JSon)
        {
            try
            {
                String NoTrans = "";
                String PsiNoTrans = "";
                String PsiDatTrans = "";
                String datLot = null;
                String noLot = null;

                dynamic json = (JObject)JsonConvert.DeserializeObject(JSon);

                String prochainCasEssai = "null";
                if (json.retourTrans.casEssai != null)
                    prochainCasEssai = json.retourTrans.casEssai;

                if (json.retourTrans.retourTransActu != null)
                {
                    if (json.retourTrans.retourTransActu.noTrans != null)
                        NoTrans = json.retourTrans.retourTransActu.noTrans;
                    else
                        NoTrans = "null";

                    if (json.retourTrans.retourTransActu.psiNoTrans != null)
                        PsiNoTrans = json.retourTrans.retourTransActu.psiNoTrans;
                    else
                        PsiNoTrans = "null";

                    if (json.retourTrans.retourTransActu.psiDatTrans != null)
                        PsiDatTrans = json.retourTrans.retourTransActu.psiDatTrans;
                    else
                        PsiDatTrans = "null";
                }

                // Batch of transactions
                if (json.retourTrans.retourTransLot != null)
                {
                    if (json.retourTrans.retourTransLot.datLot != null)
                        datLot = json.retourTrans.retourTransLot.datLot;
                    else
                        datLot = "null";

                    if (json.retourTrans.retourTransLot.noLot != null)
                        noLot = json.retourTrans.retourTransLot.noLot;
                    else
                        noLot = "null";
                }
                return (new ResponseTransaction(prochainCasEssai, NoTrans, PsiNoTrans, PsiDatTrans, datLot, noLot));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return null;
            }
        }


        public static Transaction ParseTransaction(String JSon)
        {
            try
            {
                Transaction pTrans = new Transaction();
                string strJsonObj = string.Format("{{{0}}}", JSon);

                dynamic json = (JObject)JsonConvert.DeserializeObject(strJsonObj);

                if (json.sectActi != null)
                {
                    pTrans.sectActi.Abrvt = json.sectActi.abrvt;
                    pTrans.sectActi.TypServ = json.sectActi.typServ;
                    pTrans.sectActi.NoTabl = json.sectActi.noTabl;
                    pTrans.sectActi.NbClint = json.sectActi.nbClint;
                }


                if (json.noTrans != null)
                    pTrans.NoTrans = json.noTrans;
                if (json.nomMandt != null)
                    pTrans.NomMandt = json.nomMandt;
                if (json.nomUtil != null)
                    pTrans.NomUtil = json.nomUtil;

                if (json.docAdr != null)
                {
                    pTrans.docAdr.DocNoCiviq = json.docAdr.docNoCiviq;
                    pTrans.docAdr.DocCodePostal = json.docAdr.docCp;
                }


                if (json.relaCommer != null)
                    pTrans.RelaCommer = json.relaCommer;

                if (json.clint != null)
                {
                    pTrans.clint.NomClint = json.clint.nomClint;
                    pTrans.clint.NoTvqClint = json.clint.noTvqClint;
                    pTrans.clint.Tel1 = json.clint.tel1;
                    pTrans.clint.Tel2 = json.clint.tel2;
                    pTrans.clint.Courl = json.clint.courl;
                }

                if (json.clint != null && json.clint.lstAdrClients != null)
                {
                    // Need to add list clients
                }

                if (json.datTrans != null)
                    pTrans.DatTrans = json.datTrans;

                if (json.utc != null)
                    pTrans.Utc = json.utc;

                if (json.mont != null)
                {
                    pTrans.AvantTax = json.mont.avantTax;
                    pTrans.TPS = json.mont.TPS;
                    pTrans.TVQ = json.mont.TVQ;
                    pTrans.ApresTax = json.mont.apresTax;
                    pTrans.VersActu = json.mont.versActu;
                    pTrans.VersAnt = json.mont.versAnt;
                    pTrans.Solde = json.mont.sold;
                    pTrans.Ajus = json.mont.ajus;
                    pTrans.Pourb = json.mont.pourb;
                    pTrans.MTDU = json.mont.mtdu;
                }

                if (json.noDossFO != null)
                    pTrans.NoDossFO = json.noDossFO;

                if (json.noTax != null)
                {
                    pTrans.NoTPS = json.noTax.noTPS;
                    pTrans.NoTVQ = json.noTax.noTVQ;
                }

                if (json.commerElectr != null)
                    pTrans.CommerceElectronique = json.commerElectr;

                if (json.typTrans != null)
                    pTrans.TypTrans = json.typTrans;

                if (json.modPai != null)
                    pTrans.ModPai = json.modPai;

                if (json.modImpr != null)
                    pTrans.ModImpr = json.modImpr;

                if (json.formImpr != null)
                    pTrans.FormImpr = json.formImpr;

                if (json.modTrans != null)
                    pTrans.ModTrans = json.modTrans;

                if (json.commerElectr != null)
                    pTrans.CommerceElectronique = json.commerElectr;

                if (json.signa != null)
                {
                    pTrans.signa.DatActu = json.signa.datActu;
                    pTrans.signa.Actu = json.signa.actu;
                    pTrans.signa.Preced = json.signa.preced;
                }


                if (json.emprCertifSEV != null)
                    pTrans.EmprCertifSEV = json.emprCertifSEV;


                if (json.SEV != null)
                {
                    pTrans.IdSev = json.SEV.idSEV;
                    pTrans.IdVersi = json.SEV.idVersi;
                    pTrans.CodCertif = json.SEV.codCertif;
                    pTrans.IdPartn = json.SEV.idPartn;
                    pTrans.Versi = json.SEV.versi;
                    pTrans.VersiParn = json.SEV.versiParn;
                }

                if (json.items != null)
                {
                    pTrans.lstItems = new List<TransactionItem>();
                    foreach (JObject jItem in json.items)
                    {
                        dynamic oItem = jItem;
                        TransactionItem tItem = new TransactionItem((String)oItem.qte, (String)oItem.descr, (String)oItem.unitr, (String)oItem.prix, (String)oItem.tax, (String)oItem.acti);
                        if (oItem.preci != null)
                        {
                            tItem.LstPrecisions = new List<TransactionPrecisionItem>();
                            foreach (JObject oPrecItem in oItem.preci)
                            {
                                dynamic iPrecItem = oPrecItem;
                                tItem.LstPrecisions.Add(new TransactionPrecisionItem((String)iPrecItem.qte, (String)iPrecItem.descr,
                                    (String)iPrecItem.unitr, (String)iPrecItem.prix, (String)iPrecItem.tax, (String)iPrecItem.acti));
                            }
                        }
                        pTrans.lstItems.Add(tItem);
                    }

                }
                if (json.refs != null)
                {
                    pTrans.lsttransactionReferences = new List<TransactionReference>();

                    foreach (JObject jRef in json.refs)
                    {
                        dynamic oItem = jRef;
                        TransactionReference iRef = new TransactionReference((String)oItem.noTrans, (String)oItem.datTrans, (String)oItem.avantTax);
                        pTrans.lsttransactionReferences.Add(iRef);
                    }
                }
                return pTrans;
            }

            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return null;
            }
        }


        #endregion Transactions

        #region Document
        /// <summary>
        /// Sending a Document request to the WEB-SRM
        /// Envoi d'une requête Document au MEV-WEB
        /// </summary>
        /// <param name="HTTPHeadersDict">Headers to be sent to the WEB-SRM
        ///                               Entêtes à transmettre au MEV-WEB</param>
        /// <param name="Json">Json document to be sent to the WEB-SRM
        ///                    Document json à transmettre au MEV-WEB</param>
        /// <param name="CertificateSerialNumberSRS">Serial number of the SRM/POS's certificate to be used to authenticate to the WEB-SRM
        ///                                          Numéro de série du certificat du SEV à utiliser pour s'authentifier auprès du MEV-WEB</param>
        /// <returns>Réponse reçue du MEV-WEB
        ///          Response received from the WEB-SRM</returns>
        public static WEBSRM_Response DocumentRequest(List<KeyValuePair<String, String>> HTTPHeadersDict, String Json, String CertificateSerialNumberSRS)
        {
            return SendJson(ValeursPossibles.UrlMEVWEB.Document, HTTPHeadersDict, Json, CertificateSerialNumberSRS);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TypeDoc"></param>
        /// <param name="Doc"></param>
        /// <returns></returns>
        public static String GetJsonDocument(String TypeDoc, String Doc)
        {
            StringBuilder s = new StringBuilder();

            s.Append("{");
            s.Append("\"reqDoc\": {");
            s.AppendFormat("\"typDoc\": \"{0}\",", TypeDoc);
            s.AppendFormat("\"doc\": \"{0}\"", Doc);
            s.Append("}");
            s.Append("}");

            return s.ToString();
        }

        /// <summary>
        /// Parse the response of a request of type Document
        /// Analyse la réponse d'une requête de type Document
        /// </summary>
        /// <param name="JSon">JSon document containing the WEB-SRM response
        ///                    Document JSon contenant la réponse du MEV-WEB</param>
        /// <returns>Informations received from the WEB-SRM
        ///          Informations reçues du MEV-WEB</returns>
        public static ResponseDocument ParseResponseDocument(String JSon)
        {
            try
            {
                if (JSon.Contains("listErr"))
                {
                    Console.Error.WriteLine(IndentJson(JSon));
                    return null;
                }

                dynamic json = (JObject)JsonConvert.DeserializeObject(JSon);

                String jsonVersi = "";
                if (json.retourDoc.jsonVersi != null)
                    jsonVersi = json.retourDoc.idApprl;

                String prochainCasEssai = "";
                if (json.retourDoc.casEssai != null)
                    prochainCasEssai = json.retourDoc.casEssai;

                String noDoc = "";
                if (json.retourDoc.noDoc != null)
                    noDoc = json.retourDoc.noDoc;

                return (new ResponseDocument(jsonVersi, prochainCasEssai, noDoc));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return null;
            }
        }

        #endregion Document

        #region Errors
        /// <summary>
        /// Parses the response received from WEB-SRM containing a list of errors
        /// Analyse la réponse reçue du MEV-WEB contenant une liste d'erreurs
        /// </summary>
        /// <param name="JSon">json document received from WEB-SRM
        ///                    Document json reçu du MEV-WEB</param>
        /// <returns>Errors received from the WEB-SRM
        ///          Erreurs reçues du MEV-WEB</returns>
        public static List<ErrorWEBSRM> ParseErrors(String JSon)
        {
            try
            {
                List<ErrorWEBSRM> lstErr = new List<ErrorWEBSRM>();
                IEnumerable<JToken> ListeErreurs;

                // Extraction des erreurs avec LINQ
                JObject ObjErreurs = JObject.Parse(JSon);

                if (JSon.Contains("retourCertif"))
                    ListeErreurs = from p in ObjErreurs["retourCertif"]["listErr"] select p;
                else
                if (JSon.Contains("retourUtil"))
                    ListeErreurs = from p in ObjErreurs["retourUtil"]["listErr"] select p;
                else
                if (JSon.Contains("retourTransActu"))
                    ListeErreurs = from p in ObjErreurs["retourTrans"]["retourTransActu"]["listErr"] select p;
                else
                if (JSon.Contains("retourDoc"))
                    ListeErreurs = from p in ObjErreurs["retourDoc"]["listErr"] select p;
                else
                    throw new FormatException("The received json document does not contain one of the following values : retourCertif, retourUtil, retourTrans or retourDoc. Le document json reçu ne contient pas une des valeurs suivantes : retourCertif, retourUtil, retourTrans ou retourDoc.");


                foreach (var err in ListeErreurs)
                {
                    String Id = "";
                    if (err["id"] != null)
                        Id = err["id"].ToString();

                    String Mess = "";
                    if (err["mess"] != null)
                        Mess = err["mess"].ToString();

                    String CodRetour = "";
                    if (err["codRetour"] != null)
                        CodRetour = err["codRetour"].ToString();

                    String NoTrans = "";
                    if (err["noTrans"] != null)
                        NoTrans = err["noTrans"].ToString();

                    lstErr.Add(new ErrorWEBSRM(Id, Mess, CodRetour, NoTrans));
                }
                return (lstErr);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return null;
            }
        }

        #endregion Errors

        #region Utils
        /// <summary>
        /// Send a request to WEB-SRM with a certificate
        /// Transmet une requête MEV-WEB avec un certificat
        /// </summary>
        /// <param name="Url">URL complète du service à appeler chez le MEV-WEB</param>
        /// <param name="HTTPHeadersDict">Headers to be sent to the WEB-SRM
        ///                               Entêtes à transmettre au MEV-WEB</param>
        /// <param name="Json">Json document to be sent to the WEB-SRM
        ///                    Document json à transmettre au MEV-WEB</param>
        /// <param name="CertificateSerialNumberSRS">Serial number of the SRM/POS's certificate to be used to authenticate to the WEB-SRM
        ///                                          Numéro de série du certificat du SEV à utiliser pour s'authentifier auprès du MEV-WEB</param>
        /// <returns>Message received from the WEB-SRM
        ///          Message reçu du MEV-WEB</returns>
        public static WEBSRM_Response SendJson(String Url, List<KeyValuePair<String, String>> HTTPHeadersDict, String Json, String CertificateSerialNumberSRS)
        {
            WEBSRM_Response responseWEBSRM = new WEBSRM_Response();

            RestClient client = new RestClient(Url)
            {
                Timeout = 10000
            };

            try
            {
                ServicePointManager.DefaultConnectionLimit = 9999;

                // [Deprecated]
                // Remove TLS 1.2 configuration from the framework, leaving security to the operating system
                // ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                RestRequest request = new RestRequest(Url, Method.POST);

                foreach (var k in HTTPHeadersDict)
                    if (!String.IsNullOrEmpty(k.Value))
                    {
                        request.AddHeader(k.Key, k.Value);
                        if (Demo._Verbose)
                            Console.WriteLine(k.Key + "=" + k.Value);
                    }

                request.Timeout = 10000;
                request.Parameters.Capacity = 20;
                request.AddJsonBody(Json);

                if (Demo._Verbose)
                {
                    Console.WriteLine(Url);
                    Console.WriteLine(Json);
                }

                // Lecture du certificat du SEV pour l'authentification auprès du MEVWEB (Sauf pour Ajout de certificat)
                if (CertificateSerialNumberSRS != null)
                    client.ClientCertificates = new X509CertificateCollection() { Utiles.GetCertificate(CertificateSerialNumberSRS) };

                // Exécution de la requête HTTP
                IRestResponse response = client.Execute(request, Method.POST);

                // HTTP Status Codes
                responseWEBSRM.StatusCode = (int)response.StatusCode;
                responseWEBSRM.StatusDescription = response.StatusDescription;

                // Traitement de la réponse du MEVWEB
                if (!String.IsNullOrEmpty(response.ErrorMessage))
                    responseWEBSRM.ResponseJsonWEBSRM = response.ErrorMessage;
                else
                    responseWEBSRM.ResponseJsonWEBSRM = response.Content;

                if (Demo._Verbose)
                {
                    Console.WriteLine("Status code : " + responseWEBSRM.StatusCode);
                    Console.WriteLine("Status Description : " + responseWEBSRM.StatusDescription);
                    Console.WriteLine(UtilesJSON.IndentJson(responseWEBSRM.ResponseJsonWEBSRM));
                }
            }
            catch (System.Net.Sockets.SocketException timeoutException)
            {
                Console.Error.WriteLine(timeoutException);
                responseWEBSRM.ResponseExceptionWEBSRM = timeoutException;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                responseWEBSRM.ResponseExceptionWEBSRM = ex;
            }

            return responseWEBSRM;
        }

        /// <summary>
        /// Indentation of a json document
        /// Indente un document json
        /// </summary>
        /// <param name="json">String containing the json to indent
        ///                    String contenant le json à indenter</param>
        /// <returns>String containing the indented json
        ///          String contenant le json indenté</returns>
        public static String IndentJson(string json)
        {
            String retour;
            try
            {
                dynamic parsedJson = JsonConvert.DeserializeObject(json);
                retour = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
            }
            catch (Exception e)
            {
                retour = e.Message;
            }
            return retour;
        }

        #endregion Utils
    }
}
