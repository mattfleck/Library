using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;


// restart https://www.youtube.com/watch?v=iE6f7QjVmww @ 49:45

namespace LibraryServices
{
    public class CheckOutService : ICheckout
    {
        private LibraryDbContext _context;

        public void CheckoutService( LibraryDbContext context)
        {
            _context = context;
        }

        public void Add(Checkout newCheckout)
        {
            _context.Add(newCheckout);
            _context.SaveChanges();
        }

        public IEnumerable<Checkout> GetAll()
        {
            return _context.Checkouts;
        }

        public Checkout Get(int checkoutId)
        {
            return GetAll().FirstOrDefault(checkout => checkout.Id == checkoutId);
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int Id)
        {
            return _context.CheckoutHistories
                .Include(h=>h.LibraryAsset)
                .Include(h=>h.LibraryCard)
                .Where(h => h.LibraryAsset.Id == Id);
        }

        public IEnumerable<Hold> GetCurrentHolds(int Id)
        {
            //return _context?.Holds
            //    .Include(h => h.LibraryAsset)
            //    .Where(h => h.LibraryAsset.Id == Id);
            int i = 0;

            return _context.Holds
                .Include(h => h.LibraryAsset)
                .Where(a => a.LibraryAsset.Id == Id);
        }

        public Checkout GetLatestCheckout(int assetID)
        {
            return _context.Checkouts
                .Where(c => c.LibraryAsset.Id == assetID)
                .OrderByDescending(c => c.Since)
                .FirstOrDefault();
        }

        public void MarkLost(int assetId)
        {
            UpdateAssetStatus(assetId, "Lost");

            _context.SaveChanges();

        }

        public void MarkFound(int assetId)
        {

            UpdateAssetStatus(assetId, "Available");
            RemoveExistingCheckout(assetId);

            CloseExistingCheckoutHistory(assetId);


            _context.SaveChanges();
        }

        private void UpdateAssetStatus(int assetId, string newStatus)
        {
            var item = _context.LibraryAssets
                .FirstOrDefault(a => a.Id == assetId);

            _context.Update(item);
            item.Status = _context.Statuses.FirstOrDefault(status => status.Name == newStatus);

            _context.SaveChanges();
        }

        private void CloseExistingCheckoutHistory(int assetId)
        {
            var now = DateTime.Now;

            // close out any existing checkout history
            var history = _context.CheckoutHistories
                .FirstOrDefault(ch => ch.Id == assetId
                                && ch.CheckedIn == null);

            if (history != null)
            {
                _context.Update(history);
                history.CheckedIn = now;
            }
        }

        private void RemoveExistingCheckout(int assetId)
        {
            // remove any existing checkouts
            var checkout = _context
                .Checkouts.FirstOrDefault(co => co.LibraryAsset.Id == assetId);

            if (checkout != null)
            {
                _context.Remove(checkout);
            }
        }

        public void PlaceHold(int assetId, int libraryCardId)
        {
            var now = DateTime.Now;

            var asset = _context.LibraryAssets
                            .FirstOrDefault(a => a.Id == assetId);

            var card = _context.LibraryCards
                .FirstOrDefault(c => c.Id == libraryCardId);

            if (asset.Status.Name == "On Hold")
            {
                UpdateAssetStatus(assetId, "On Hold");
            }

            var hold = new Hold
            {
                HoldPlaced = now,
                LibraryAsset = asset,
                LibraryCard = card
            };

            _context.Add(hold);
            _context.SaveChanges();

        }
        public void CheckInItem(int assetId)
        {
            var now = DateTime.Now;
            var item = _context.LibraryAssets
                .FirstOrDefault(a => a.Id == assetId);

            // remove any existing checkouts
            RemoveExistingCheckout(assetId);

            //close any existing checkout history
            CloseExistingCheckoutHistory(assetId);

            //look for existing holds
            var currentHolds = _context.Holds
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .Where(h => h.LibraryAsset.Id == assetId);

            // if on hold checkout item to library card with earliest hold
            if (currentHolds.Any())
            {
                CheckoutToEarliestHold(assetId, currentHolds);
            }

            //otherwise mark item as available
            UpdateAssetStatus(assetId, "Avaialble");

            _context.SaveChanges();
        }

        private void CheckoutToEarliestHold(int assetId, IQueryable<Hold> currentHolds)
        {
            var earliestHold = currentHolds
                .OrderBy(holds => holds.HoldPlaced)
                .FirstOrDefault();

            var card = earliestHold.LibraryCard;

            _context.Remove(earliestHold);
            _context.SaveChanges();

            CheckoutItem(assetId, card.Id);
        }

        public void CheckoutItem(int assetId, int libraryCardId)
        {
            if (IsCheckedOut(assetId))
            {
                // handle feedback to user
                return;
            }
            var item = _context.LibraryAssets
                        .FirstOrDefault(a => a.Id == assetId);

            UpdateAssetStatus(assetId, "Checked Out");
            var libraryCard = _context.LibraryCards
                .Include(card => card.Checkouts)
                .FirstOrDefault(card => card.Id == libraryCardId);

            var now = DateTime.Now;
            var checkout = new Checkout
            {
                LibraryAsset = item,
                LibraryCard = libraryCard,
                Since = now,
                Until = GetDefaultCheckoutTime(now)
            };

            _context.Add(checkout);

            var checkoutHistory = new CheckoutHistory
            {
                CheckedOut = now,
                LibraryAsset = item,
                LibraryCard = libraryCard
            };

            _context.Add(checkoutHistory);
            _context.SaveChanges();
        }

        private DateTime GetDefaultCheckoutTime(DateTime now)
        {
            return now.AddDays(30);
        }

        public bool IsCheckedOut(int assetId)
        {
            return _context.Checkouts
                .Where(co => co.LibraryAsset.Id == assetId)
                .Any();
        }

        public string GetCurrentHoldPatron(int holdId)
        {
            var hold = _context.Holds
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .FirstOrDefault(h => h.Id == holdId);

            var cardId = hold?.LibraryCard.Id;
            var patron = _context.Patrons
                .Include(p => p.LibraryCard)
                .FirstOrDefault(p => p.LibraryCard.Id == cardId);

            return patron?.FirstName + " " + patron?.LastName;
        }

        public string GetCurrentHoldPlaced(int holdId)
        {
            var hold = _context.Holds
                .Include(a => a.LibraryAsset)
                .Include(a => a.LibraryCard)
                .Where(v => v.Id == holdId);

            return hold.Select(a => a.HoldPlaced)
                .FirstOrDefault().ToString();
        }

        public string GetCurrentCheckedoutPatron(int assetId)
        {
            var checkout = GetCheckoutByAssetId(assetId);
            if (GetCheckoutByAssetId(assetId) ==null)
            {
                return "Not checked out";
            };

            var cardId = checkout.LibraryCard.Id;

            var patron = _context.Patrons
                .Include(p => p.LibraryCard)
                .FirstOrDefault(p => p.LibraryCard.Id == cardId);

            return patron.FirstName + " " + patron.LastName;
        }

        private Checkout GetCheckoutByAssetId(int assetId)
        {
            return _context.Checkouts
                .Include(co => co.LibraryAsset)
                .Include(co => co.LibraryCard)
                .FirstOrDefault(co => co.LibraryAsset.Id == assetId);
        }
    }
}
