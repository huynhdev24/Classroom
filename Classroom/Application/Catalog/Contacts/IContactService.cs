using Classroom.Data;
using Classroom.Models.Catalog.Contact;
using Classroom.Models.Common;

namespace Classroom.Application.Catalog.Contacts;

/// <summary>
/// IContactService
/// </summary>
public interface IContactService
{
    Task<bool> Create(Contact request);
    Task<bool> Delete(int ContactID);
    Task<Contact> GetById(int ContactID);
    Task<PagedResult<Contact>> GetAllPaging(GetAllPagingRequest request);
}