using ASPCoreWebApplication.Models.Catalog;
using LibraryData;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ASPCoreWebApplication.Controllers
{
    public class CatalogController : Controller
    {
        private ILibraryAsset _assets;
        private ICheckout _checkouts;

        public CatalogController(ILibraryAsset assets, ICheckout checkouts)
        {
            _assets = assets;
            _checkouts = checkouts;
        }

        public IActionResult Index()
        {
            var assetModels = _assets.GetAll();

            var listingResult = assetModels
                 .Select(result => new AssetIndexListingModel
                 {
                     Id = result.Id,
                     ImageURL = result.ImageURL,
                     AuthorOrDirector = _assets.GetAuthorOrDirector(result.Id),
                     DeweyCallNumber = _assets.GetDeweyIndex(result.Id),
                     Title = result.Title,
                     Type = _assets.GetType(result.Id)
                 });

            var model = new AssetIndexModel()
            {
                Assets = listingResult
            };

            return View(model);
        }

        public IActionResult Detail(int id)
        {
            var asset = _assets.GetByID(id);

            //var currentHolds = _checkouts.GetCurrentHolds(id)
            //    .Select(a => new AssetHoldModel
            //    {
            //        HoldPlaced = _checkouts.GetCurrentHoldPlaced(a.Id).ToString("d"),
            //        PatronName = _checkouts.GetCurrentHoldPatronName(a.Id)
            //    });
            var currentHolds = _checkouts.GetCurrentHolds(id).Select(a => new AssetHoldModel
            {
                HoldPlaced = _checkouts.GetCurrentHoldPlaced(a.Id),
                PatronName = _checkouts.GetCurrentHoldPatron(a.Id)
            });

            var model = new AssetDetailModel
            {
                AssetId = id,
                Title = asset.Title,
                Type = _assets.GetType(id),
                Year = asset.Year,
                Cost = asset.Cost,
                Status = asset.Status.Name,
                ImageURL = asset.ImageURL,
                AuthorOrDirector = _assets.GetAuthorOrDirector(id),
                CurrentLocation = _assets.GetCurrentLocation(id).Name,
                DeweyCallNumber = _assets.GetDeweyIndex(id),
                ISBM = _assets.GetIsbn(id),
                CheckoutHistory = _checkouts.GetCheckoutHistory(id),
                //CurrentAssociatedLibraryCard = _assets.GetLibraryCardByAssetId(id),
                LatestCheckout = _checkouts.GetLatestCheckout(id),
                PatronName = _checkouts.GetCurrentHoldPatron(id),
                CurrentHolds = currentHolds

            };
            return View(model);
        }
    }
}
