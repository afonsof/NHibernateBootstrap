[![Build Status](https://travis-ci.org/afonsof/NHibernateBootstrap.png?branch=master)](https://travis-ci.org/afonsof/NHibernateBootstrap)
[![Built with Grunt](https://cdn.gruntjs.com/builtwith.png)](http://gruntjs.com/)
NHibernateBootstrap
===================

Start a NHibernate Repository application in seconds

## Getting Started

#### 1. Setting Up

1.1 Create a project and install NHibernateBootstrap using Nuget:
```
PM> Install-Package NHibernateBootstrap
```

1.2 Install a Database provider (for example: MySQL)
```
PM> Install-Package MySql.Data
```

#### 2. Create entity classes
All entity classes must implement ``IHaveId`` interface and all properties must be virtual according with the following example:
```c#
public class Person : IHaveId
{
    public virtual int Id { get; set; }

    [Required]
    public virtual string Name { get; set; }

    [Required]
    public virtual string Phone { get; set; }
    
    [Required]
    public virtual string Email { get; set; }
}
```

#### 3. Create a UnitOfWork class inhreriting UnitOfWorkBase class. This class will be used to store and manage your repositories:

```c#
public class UnitOfWork: UnitOfWorkBase
{
    private IRepository<Person> _person;
    
    //On demand repository initialization
    public IRepository<Person> PersonRepository
    {
        get { return _person ?? (_person = new Repository<Person>(this)); }
    }
}
```

#### 4. Call the Setup method in Application Startup. It can be used in global.asax Application_Start method:
```c#
protected void Application_Start()
{
    var cs = "Server=localhost;Database=myapp;Uid=root;Pwd=root;"
    NHibernateBuilder.Setup<UnitOfWork>(MySQLConfiguration.Standard.ConnectionString(cs));
    ...
}
```
In this case we're using a raw connection string, but you can read it in web.config.

You will need to pass to Setup method any class of entities' assembly. Here the created ``UnitOfWork`` class were used.

When the application begins the Database schema will always be created/updated.


#### 5. Use the UnitOfWork wherever you want, including in MVC Controllers:
```c#
public class PersonController : Controller
{
    private readonly UnitOfWork _db = new UnitOfWork();

    public ActionResult Index()
    {
        return View(_db.PersonRepository.AsQueryable().ToList());
    }

    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Person person)
    {
        if (ModelState.IsValid)
        {
            _db.PersonRepository.Add(person);
            _db.Commit();
            return RedirectToAction("Index");
        }
        return View(person);
    }

    public ActionResult Edit(int id = 0)
    {
        var person = _db.PersonRepository.Find(id);
        return View(person);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(Person person)
    {
        if (ModelState.IsValid)
        {
            _db.PersonRepository.Edit(person);
            _db.Commit();
            return RedirectToAction("Index");
        }
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id = 0)
    {
        var person = _db.PersonRepository.Find(id);
        _db.PersonRepository.Remove(person);
        _db.Commit();
        return RedirectToAction("Index");
    }

    protected override void Dispose(bool disposing)
    {
        _db.Dispose();
        base.Dispose(disposing);
    }
}
```

## Contributing

Please use the issue tracker and pull requests.

## License
Copyright (c) 2014 Afonso França
Licensed under the MIT license.
