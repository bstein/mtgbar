using Melek.Domain;

namespace MtGBar.Infrastructure.Utilities.VendorRelations
{
    public abstract class Vendor
    {
        public abstract string GetLink(Card card, Set set);
        public abstract string GetName();
        public abstract string GetPrice(Card card, Set set);
    }
}