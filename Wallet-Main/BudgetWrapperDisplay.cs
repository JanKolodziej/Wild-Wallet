using Wallet_lib;

namespace Wallet_Main
{
    public class BudgetWrapperDisplay
    {
        public BudgetWrapper BudgetWrapper { get; set; }
        public Rect MarkerBounds => new Rect(BudgetWrapper.ExpectedPercentage, 0, 2, 1);
    }
}
