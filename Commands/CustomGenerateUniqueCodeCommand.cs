using Plugin.Sample.Orders.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.Sample.Orders.Commands
{
    /// <summary>
    /// CustomGenerateUniqueCodeCommand for generating custom ORder Confirmation IDs
    /// </summary>
    public class CustomGenerateUniqueCodeCommand : CommerceCommand
    {
        /// <summary>
        /// Command to find entities in list
        /// </summary>
        private FindEntitiesInListCommand _findEntitiesInListCommand;

        /// <summary>
        /// Default Generate Unique Code Command
        /// </summary>
        private GenerateUniqueCodeCommand _generateUniqueCodeCommand;

        /// <summary>
        /// c'tor
        /// </summary>
        /// <param name="serviceProvider">serviceProvider</param>
        /// <param name="findEntitiesInListCommand">findEntitiesInListCommand</param>
        /// <param name="generateUniqueCodeCommand">generateUniqueCodeCommand</param>
        public CustomGenerateUniqueCodeCommand(
            IServiceProvider serviceProvider,
            FindEntitiesInListCommand findEntitiesInListCommand,
            GenerateUniqueCodeCommand generateUniqueCodeCommand)
          : base(serviceProvider)
        {
            _findEntitiesInListCommand = findEntitiesInListCommand;
            _generateUniqueCodeCommand = generateUniqueCodeCommand;
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <param name="commerceContext">Commerce Context</param>
        /// <returns>Result</returns>
        public Task<string> Process(CommerceContext commerceContext)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                return Task.FromResult(CalculateOrderConfirmationId(commerceContext).Result);
            }
        }

        /// <summary>
        /// Calculates an unique order confirmation ID
        /// </summary>
        /// <param name="commerceContext">commerceContext</param>
        /// <returns>Unique Order COnfirmation ID</returns>
        private async Task<string> CalculateOrderConfirmationId(CommerceContext commerceContext)
        {
            UniqueCodePolicy policy = commerceContext.GetPolicy<UniqueCodePolicy>();
            bool shopNameAsUpper = policy.ShopNameAsUpper;
            string prefix = policy.ShopNameAsPrefix 
                ? shopNameAsUpper 
                    ? commerceContext.CurrentShopName().ToUpper() 
                    : commerceContext.CurrentShopName() 
                : policy.Prefix;
            string suffix = policy.ShopNameAsSuffix 
                ? shopNameAsUpper
                    ? commerceContext.CurrentShopName().ToUpper()
                    : commerceContext.CurrentShopName()
                : policy.Suffix;
            string digitsFormat = policy.DigitsFormat;

            int latestOrderId = GetLatestUsedId(commerceContext, prefix, suffix).Result;
            string newId = latestOrderId == int.MaxValue
                ? await _generateUniqueCodeCommand.Process(commerceContext)
                : (latestOrderId + 1).ToString(digitsFormat);

            return $"{prefix}{newId}{suffix}";
        }

        /// <summary>
        /// Helper to Get Latest Used ID based on Prefix and Suffix
        /// </summary>
        /// <param name="commerceContext">commerce context</param>
        /// <param name="prefix">prefix</param>
        /// <param name="suffix"></param>
        /// <returns></returns>suffix
        private async Task<int> GetLatestUsedId(CommerceContext commerceContext, string prefix, string suffix)
        {
            CommerceList<Order> orders = await _findEntitiesInListCommand
                .Process<Order>(commerceContext, CommerceEntity.ListName<Order>(), 0, int.MaxValue);

            IEnumerable<Order> filteredOrders = orders
                .Items
                .Where(element => element.OrderConfirmationId.ToLower().StartsWith(prefix.ToLower()) 
                    && element.OrderConfirmationId.ToLower().EndsWith(suffix.ToLower()));

            int maxValue = 0;
            foreach (Order filteredOrder in filteredOrders)
            {
                string numericPortionOfId = filteredOrder
                    .OrderConfirmationId
                    .Substring(prefix.Length, filteredOrder.OrderConfirmationId.Length - prefix.Length - suffix.Length);
                int numbericPortionIfIdAsInt;
                if (!int.TryParse(numericPortionOfId, out numbericPortionIfIdAsInt))
                {
                    await commerceContext.AddMessage(
                         commerceContext.GetPolicy<KnownResultCodes>().Error,
                         "ConfirmationIdParsingFailed",
                         new object[] { filteredOrder.OrderConfirmationId },
                         null);

                    return int.MaxValue;
                }

                if (numbericPortionIfIdAsInt > maxValue)
                {
                    maxValue = numbericPortionIfIdAsInt;
                }
            }

            return maxValue;
        }
    }
}
