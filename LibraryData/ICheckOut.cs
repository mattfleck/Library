using LibraryData.Models;
using System.Collections.Generic;

namespace LibraryData
{
    public interface ICheckout
    {
        IEnumerable<Checkout> GetAll();
        Checkout Get(int id);
        void Add(Checkout newCheckout);
        IEnumerable<CheckoutHistory> GetCheckoutHistory(int id);
        void PlaceHold(int assetId, int libraryCardId);
        void CheckoutItem(int assetId, int libraryCardId);
        void CheckInItem(int assetId);
        Checkout GetLatestCheckout(int id);
        //int GetNumberOfCopies(int id);
        //int GetAvailableCopies(int id);
        bool IsCheckedOut(int id);

        string GetCurrentHoldPatron(int id);
        string GetCurrentHoldPlaced(int id);
        //string GetCurrentPatron(int id);
        IEnumerable<Hold> GetCurrentHolds(int id);

        void MarkLost(int id);
        void MarkFound(int id);
    }
}
//namespace LibraryData
//{
//    public interface ICheckOut
//    {
//        IEnumerable<Checkout> GetAll();
//        Checkout GetById(int checkoutId);
//        void Add(Checkout newCheckout);
//        void CheckOutItem(int assetId, int libraryCardId);
//        void CheckInItem(int assetId, int libraryCardId);
//        IEnumerable<CheckoutHistory> GetCheckoutHistory(int Id);
//        string GetCurrentCheckedoutPatron(int assetId);

//        void PlaceHold( int assetId, int libraryCardId);
//        string GetCurrentHoldPatronName(int Id);
//        string GetCurrentHoldPlaced(int Id);
//        IEnumerable<Hold> GetCurrentHolds(int Id);
//        Checkout GetLatestCheckout(int assetID);

//        void MarkLost(int assetId);
//        void MarkFound(int assetId);

//    }
//}
