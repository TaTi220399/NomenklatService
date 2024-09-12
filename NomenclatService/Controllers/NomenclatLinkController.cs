using Microsoft.AspNetCore.Mvc;
using NLog;
using NomenklatService.DataContext;
using NomenklatService.Models;
using System.Text.Json;

namespace NomenklatService.Controllers
{
    [ApiController]
    [Route("api/nomenklatures")]
    public class NomenclatLinkController : ControllerBase
    {
        private static Logger _logger = LogManager.GetLogger("sample");

        [HttpGet]
        [Route("")]
        public string GetAllProducts()
        {
            try
            {
                List<Nomenklature> nomenklatures;
                using (NomenklatContext db = new())
                {
                    nomenklatures = db.Nomenklature.OrderBy(x => x.Id).ToList();
                    Console.WriteLine("Nomenklatures list:");
                    foreach (var n in nomenklatures)
                    {
                        Console.WriteLine($"{n.Id}. {n.Name} - {n.Price}");
                    }
                    _logger.Info("The nomenklatures have been received.");
                }
                return JsonSerializer.Serialize(nomenklatures);
            }
            catch (Exception ex)
            {
                _logger.Error("Can't to get nomenklatures.");
                return string.Empty;
            }
        }

        [HttpPost]
        [Route("")]
        public void PostNomenclature(string name, int price)
        {
            try
            {
                using (NomenklatContext db = new())
                {
                    Nomenklature nomenclat = new() { Name = name, Price = price };

                    db.Nomenklature.Add(nomenclat);
                    db.SaveChanges();
                    _logger.Info($"The nomenclature {name} was saved successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Can't to create the nomenclature {name}.");
            }
        }

        [HttpPut]
        [Route("{id}")]
        public void ChangeNomenclature(int id, int price)
        {
            try
            {
                using (NomenklatContext db = new())
                {
                    Nomenklature origin = (Nomenklature)db.Nomenklature.Where(x => x.Id == id);
                    origin.Price = price;
                    db.SaveChanges();
                    _logger.Info($"The nomenclature id = {id} was updated successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Can't to update the nomenclature id = {id}.");
            }
        }


        [HttpDelete]
        [Route("{id}")]
        public void DeleteNomenclature(int id)
        {
            try
            {
                using (NomenklatContext db = new())
                {
                    Nomenklature origin = (Nomenklature)db.Nomenklature.Where(x => x.Id == id);
                    db.Nomenklature.Remove(origin);
                    db.SaveChanges();
                    _logger.Info($"The nomenclature id = {id} was deleted successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Can't to delete the nomenclature id = {id}.");
            }
        }

        [HttpPost]
        [Route("link")]
        public void CreateLink(int parentId, int nomenklatureId, int count)
        {
            try
            {
                using (NomenklatContext db = new())
                {
                    Links link = new() { ParentId = parentId, NomenklatureId = nomenklatureId, Kol = count };

                    db.Links.Add(link);
                    db.SaveChanges();
                    _logger.Info($"The link between nomenclature's ids {parentId} and {nomenklatureId} was saved successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Can't to create the link between nomenclature's ids {parentId} and {nomenklatureId}.");
            }
        }

        [HttpDelete]
        [Route("link")]
        public void DeleteLink(int id)
        {
            try
            {
                using (NomenklatContext db = new())
                {
                    Links origin = (Links)db.Links.Where(x => x.Id == id);
                    db.Links.Remove(origin);
                    db.SaveChanges();
                    _logger.Info($"The link between nomenclature's ids {origin.ParentId} and {origin.NomenklatureId} was saved successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Can't to create the link between row nomenclature's id {id}.");
            }
        }
    }
}
