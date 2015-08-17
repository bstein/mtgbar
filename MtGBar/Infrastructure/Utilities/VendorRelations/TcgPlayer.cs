using System;
using Melek.Domain;

namespace MtGBar.Infrastructure.Utilities.VendorRelations
{
    class TcgPlayer : Vendor
    {
        public override string GetLink(Card card, Set set)
        {
            throw new NotImplementedException();
        }

        public override string GetName()
        {
            return "TcgPlayer.com";
        }

        public override string GetPrice(Card card, Set set)
        {
            throw new NotImplementedException();
        }
    }
}
