using Sitecore.Commerce.Core;

namespace Plugin.Sample.Orders.Policies
{
    /// <summary>
    /// UnuqueCodePolicy
    /// </summary>
    public class UniqueCodePolicy : Policy
    {
        /// <summary>
        /// Prefix
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Suffix
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// Flag to determine if the current Shop Name should be used as Suffix
        /// </summary>
        public bool ShopNameAsSuffix { get; set; }

        /// <summary>   
        /// Flag to determine if the current Shop Name should be used as Prefix
        /// </summary>
        public bool ShopNameAsPrefix { get; set; }

        /// <summary>
        /// Number of Digits to be used to generate order ids
        /// </summary>
        public string DigitsFormat { get; set; }

        /// <summary>
        /// Flag to determine if the shop names should be upper case
        /// </summary>
        public bool ShopNameAsUpper { get; set; }

        /// <summary>
        /// c'tor
        /// </summary>
        public UniqueCodePolicy()
        {
            Prefix = string.Empty;
            Suffix = string.Empty;
            DigitsFormat = "00000";
            ShopNameAsUpper = true;
            ShopNameAsPrefix = true;
            ShopNameAsSuffix = false;
        }
    }
}
