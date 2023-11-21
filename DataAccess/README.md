# How to use

This project adds an abstraction to the DataContext for more comfortable use of the DataContext method.
The advantages of the DataAccessor are:

- Input validation for given arguments
- DataAccessor automatically attachs unattached entities to the DataContext, to avoid exceptions when for example
  updating input parameters like the Target
- Fluent API style to chain operations
- All retrieve operations against Dataverse run with NoLock=true for better performance

In the following there are a few examples how to use the DataAccessor:

````csharp
// Create new DataAccessor based from an IOrganizationService 
var dataAccessor = DataAccessor(SecuredOrganizationService);

// Get record by id with specified column set
var user = dataAccessor.GetById<SystemUser>(CallerId, x => new SystemUser
    {
        AzureActiveDirectoryObjectId = x.AzureActiveDirectoryObjectId
    }
);

// Retrieve contacts based on a condition with a columnset
var contacts = dataAccessor
    .Get<Contact>(x => x.FullName == "bla")
    .Select(x => new Contact
    {
        Id = x.Id,
        ParentCustomerId = x.ParentCustomerId
    })
    .ToList();

// Do crazy linq queries with joins etc.
var query = (from c in dataAccessor.GetAll<Contact>()
    join u in dataAccessor.GetAll<SystemUser>()
        on c.OwnerId.Id equals u.Id
    where u.BusinessUnitId.Id == BusinessUnitId
    select new Contact
    {
        Id = c.Id,
        ParentCustomerId = c.ParentCustomerId
    }).ToList();


// Create new record and update target with connection to it
var contact = Entity.ToEntity<Contact>();
var newAccount = new Account
{
    Name = "New Account"
};

dataAccessor
    .Add(newAccount)
    .Commit()
    .Update(new Contact(contact.Id)
        {
            ParentCustomerId = newAccount.ToEntityReference()
        }
    ).Commit();

````