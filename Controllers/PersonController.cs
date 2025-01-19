using System.Security.Claims;
using FinanceApp.Models;
using FinanceApp.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Npgsql;

namespace FinanceApp.Controllers;

public class PersonController : Controller
{
    private readonly IPersonRepositorySQL _personRepository;

    public PersonController(IPersonRepositorySQL personRepository)
    {
        _personRepository = personRepository;
    }

    public async Task<Person?> GetPersonAsync(AppUser user)
    {
        try
        {
            return await _personRepository.GetPersonAsync(user);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Person> EditPersonAsync(
        AppUser user,
        string firstName,
        string lastName,
        string email
    )
    {
        try
        {
            var newPerson = new Person
            {
                UserId = user.Id,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
            };

            Person? person = await _personRepository.GetPersonAsync(user);
            if (person != null)
            {
                newPerson.PersonId = person.PersonId;
                await _personRepository.UpdatePersonAsync(newPerson);
                return newPerson;
            }

            return await _personRepository.AddPersonAsync(newPerson);
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine("Database error in EditPersonAsync: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Other error in EditPersonAsync: " + ex.Message);
        }
        return null!;
    }

    public async Task<CombinedUserData?> GetCombinedUserDataAsync(AppUser user)
    {
        try
        {
            return await _personRepository.GetCombinedUserDataAsync(user);
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine("Database error in GetCombinedUserDataAsync: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Other error in GetCombinedUserDataAsync: " + ex.Message);
        }
        return null;
    }

    public async Task<List<CombinedUserData>> GetAllCombinedUserDataAsync()
    {
        try
        {
            return await _personRepository.GetAllCombinedUserDataAsync();
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine("Database error in GetAllCombinedUserDataAsync: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Other error in GetAllCombinedUserDataAsync: " + ex.Message);
        }
        return [];
    }
}
