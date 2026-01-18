using System;
using System.Text;

namespace cashregister.Services
{
    // Maps app data to MEV-compatible payload strings without using the MEV assembly.
    // This guarantees no data is ever sent to the government - pure local string formatting only.
    public class MevMappingService
    {
        public string BuildFrequentThirdPartyPayload(FrequentThirdPartyInfo thirdParty, string? digitalSignature = null)
        {
            if (thirdParty == null) throw new ArgumentNullException(nameof(thirdParty));

            var s = new StringBuilder();
            s.AppendFormat("TQ={0};", thirdParty.QstRegistrationNumber);
            s.AppendFormat("NM={0};", thirdParty.Name);
            s.AppendFormat("AD={0};", thirdParty.AddressLine);
            s.AppendFormat("CP={0};", thirdParty.PostalCode);
            s.AppendFormat("TE={0};", thirdParty.PhoneNumber);
            s.AppendFormat("PO={0};", thirdParty.PhoneExtension);
            s.AppendFormat("NE={0};", thirdParty.Neq);
            s.AppendFormat("RA={0};", ((int)thirdParty.ReportingReason).ToString());
            s.AppendFormat("DC={0};", thirdParty.ContractConclusionDate.ToString("yyyyMMdd"));
            s.AppendFormat("DV={0};", thirdParty.EffectiveDate.ToString("yyyyMMdd"));
            s.AppendFormat("DE={0};", thirdParty.Description);
            s.AppendFormat("BS={0};", thirdParty.DescriptionDetails);
            s.AppendFormat("FR={0};", thirdParty.FrequencyCode);
            s.AppendFormat("EM={0};", thirdParty.CertificateFingerprint);

            if (string.IsNullOrEmpty(digitalSignature))
            {
                // For signature calculation (no SI field)
                s.AppendFormat("MO={0}", ((int)thirdParty.TimeOfDay).ToString());
            }
            else
            {
                // For transmission format (includes SI)
                s.AppendFormat("MO={0};", ((int)thirdParty.TimeOfDay).ToString());
                s.AppendFormat("SI={0}", digitalSignature);
            }

            return s.ToString();
        }
    }

    public class FrequentThirdPartyInfo
    {
        public string QstRegistrationNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string PhoneExtension { get; set; } = string.Empty;
        public string Neq { get; set; } = string.Empty;
        public ReportingReason ReportingReason { get; set; }
        public DateTime ContractConclusionDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string DescriptionDetails { get; set; } = string.Empty;
        public string FrequencyCode { get; set; } = string.Empty;
        public TimeOfDayCode TimeOfDay { get; set; }
        public string CertificateFingerprint { get; set; } = string.Empty;
    }

    public enum ReportingReason
    {
        New = 1,
        Amendment = 2,
        Expiration = 3
    }

    public enum TimeOfDayCode
    {
        Day = 1,
        Evening = 2,
        Night = 3
    }
}
