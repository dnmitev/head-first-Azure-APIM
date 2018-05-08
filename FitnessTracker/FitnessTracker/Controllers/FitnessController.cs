using FitnessTracker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitnessTracker.Controllers
{
    [Produces("application/json")]
    [Route("api/fitness")]
    public class FitnessController : Controller
    {
        private readonly IDocDbRepository<Excercise> _excercises;

        public FitnessController(IDocDbRepository<Excercise> excercises)
        {
            this._excercises = excercises;
        }

        [HttpGet]
        [Route("api/fitness/excercises")]
        public async Task<IEnumerable<Excercise>> Getexcercises()
        {
            return await this._excercises.GetItemsAsync(e => e.Id != null);
        }

        [HttpGet]
        [Route("api/fitness/excercises/filter_by_name")]
        public async Task<IEnumerable<Excercise>> Getexcercises(string name = "")
        {
            return await this._excercises.GetItemsAsync(e => e.Name == name);
        }

        [HttpPost]
        [Route("api/fitness/create_excercise")]
        public async Task Createexcercise(Excercise excercise)
        {
            await this._excercises.CreateItemAsync(excercise);
        }

        [HttpPut]
        [Route("api/fitness/update_excercise")]
        public async Task UpdateExcercise(Excercise update)
        {
            await this._excercises.UpdateItemAsync(update.Id, update);
        }

        [HttpDelete]
        [Route("api/fitness/delete_excercise")]
        public async Task DeleteExcercise(string id)
        {
            await this._excercises.DeleteItemAsync(id);
        }
    }
}