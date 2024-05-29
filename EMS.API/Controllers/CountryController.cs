﻿using EMS.APILibrary.Repositories.Contracts;
using EMS.BaseLibrary.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountryController : GenericController<Country>
{
    public CountryController(IGenericRepositoryInterface<Country> genericRepositoryInterface) : base(genericRepositoryInterface)
    {
    }
}