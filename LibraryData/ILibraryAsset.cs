using System.Collections.Generic;
using LibraryData.Models;

namespace LibraryData
{
    public interface ILibraryAsset
    {

        IEnumerable<LibraryAsset> GetAll();
        LibraryAsset GetByID(int ID);
        void Add(LibraryAsset newAssett);
        string GetAuthorOrDirector(int id);
        string GetDeweyIndex(int id);
        string GetType(int id);
        string GetTitle(int id);
        string GetIsbn(int id);

        LibraryBranch GetCurrentLocation(int id);

    }
}
