using Plugin.Sample.Orders.Commands;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Threading.Tasks;

namespace Plugin.Sample.Orders.Pipelines.Blocks
{
    /// <summary>
    /// Custom Block for generating Order Confirmation ID
    /// </summary>
    [PipelineDisplayName("Sample.Plugin.Order.block.CustomOrderPlacedAssignConfirmationId")]
    public class CustomOrderPlacedAssignConfirmationIdBlock : PipelineBlock<Order, Order, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// CustomUnique Code Generator
        /// </summary>
        private CustomGenerateUniqueCodeCommand _generateUniqueCodeCommand;

        /// <summary>
        /// c'tor
        /// </summary>
        /// <param name="generateUniqueCodeCommand"></param>
        public CustomOrderPlacedAssignConfirmationIdBlock(CustomGenerateUniqueCodeCommand generateUniqueCodeCommand)
        {
            this._generateUniqueCodeCommand = generateUniqueCodeCommand;
        }

        /// <summary>
        /// Run
        /// </summary>
        /// <param name="order"><Input Order/param>
        /// <param name="context">Commerce Context</param>
        /// <returns></returns>
        public override async Task<Order> Run(Order order, CommercePipelineExecutionContext context)
        {
            CustomOrderPlacedAssignConfirmationIdBlock confirmationIdBlock = this;
            Condition.Requires(order).IsNotNull("The order can not be null");
            string uniqueCode = Guid.NewGuid().ToString("N");
            try
            {
                uniqueCode = await _generateUniqueCodeCommand.Process(context.CommerceContext);
            }
            catch (Exception ex)
            {
                context.CommerceContext.LogException(string.Format("{0}-UniqueCodeException", confirmationIdBlock.Name), ex);
            }

            order.OrderConfirmationId = uniqueCode;
            return order;
        }
    }
}
