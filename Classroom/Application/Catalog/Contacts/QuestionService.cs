using System.Security.Cryptography.X509Certificates;
using Classroom.Utilities.Exceptions;
using Classroom.Models.Common;
using Microsoft.EntityFrameworkCore;
using Classroom.Application.Common;
using System.Net.Http.Headers;
using Classroom.Data;
using AutoMapper;
using Classroom.Models.Catalog.Contact;

namespace Classroom.Application.Catalog.Contacts;

/// <summary>
/// ContactService
/// </summary>
public class ContactService : IContactService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private const string USER_CONTENT_FOLDER_NAME = "user-content";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="mapper"></param>
    /// <author>huynhdev24</author>
    public ContactService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    public async Task<bool> Create(Contact request)
    {
        request.DateTimeCreated = DateTime.Now;
        _context.Contacts.Add(request);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ContactID"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    public async Task<bool> Delete(int ContactID)
    {
        var contact = await _context.Contacts.FindAsync(ContactID);
        if (contact == null) return false;
        _context.Contacts.Remove(contact);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    public async Task<PagedResult<Contact>> GetAllPaging(GetAllPagingRequest request)
    {
        //1. Select join
        var query = from c in _context.Contacts select c;

        //3. Paging
        int totalRow = await query.CountAsync();
        var data = await query.OrderByDescending(x => x.DateTimeCreated).Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();
        //4. Select and projection
        var pagedResult = new PagedResult<Contact>()
        {
            TotalRecords = totalRow,
            PageSize = request.PageSize,
            PageIndex = request.PageIndex,
            Items = data
        };
        return pagedResult;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ContactID"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    public async Task<Contact> GetById(int ContactID)
    {
        var Contact = await _context.Contacts.FindAsync(ContactID);
        if (Contact == null) return null;
        return Contact;
    }
}