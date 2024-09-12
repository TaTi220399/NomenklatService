using Microsoft.AspNetCore.Mvc;
using NLog;
using NomenclatService.Models;
using NomenklatService.DataContext;
using NomenklatService.Models;
using System.Data;
using System.Text.Json;

namespace NomenklatService.Controllers
{
    [ApiController]
    [Route("api/nomenklatures")]
    public class NomenclatLinkController : ControllerBase
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

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

        [HttpGet]
        [Route("{id}")]
        public string GetTotalProducts(int id)
        {
            try
            {
                List<NomenklatureTotal> nomenclatureTotals = new();
                using (NomenklatContext db = new())
                {
                    Nomenklature parent = db.Nomenklature.FirstOrDefault(x => x.Id == id);
                    List<Links> links = db.Links.Where(x => x.ParentId == id).OrderByDescending(x => x.NomenklatureId).ToList();
                    foreach (Links link in links)
                    {
                        NomenklatureTotal total = new NomenklatureTotal();
                        Nomenklature nomenklature = db.Nomenklature.FirstOrDefault(x => x.Id == link.NomenklatureId);
                        total.Product = nomenklature.Name;
                        total.Price = nomenklature.Price;
                        total.Count = link.Count;

                        int totalOne;
                        if (link.ParentId == null)
                        {
                            totalOne = link.Count * nomenklature.Price;
                        }
                        else
                        {
                            totalOne = link.Count * (nomenklature.Price + nomenclatureTotals.Select(x => x.TotalPrice).ToArray().Sum());
                        }

                        total.TotalPrice = totalOne;
                        nomenclatureTotals.Add(total);
                    }

                    int parentCount = db.Links.FirstOrDefault(x => x.NomenklatureId == id).Count;
                    nomenclatureTotals.Add(new NomenklatureTotal() { Product = parent.Name, Count = parentCount, Price = parent.Price, TotalPrice = parent.Price + nomenclatureTotals.Select(x => x.TotalPrice).ToArray().Sum() });

                    _logger.Info("The nomenklature's total price have been received.");
                }


                return JsonSerializer.Serialize(nomenclatureTotals);
            }
            catch (Exception ex)
            {
                _logger.Error("Can't to get the nomenklature's total price.");
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
                    Nomenklature origin = db.Nomenklature.FirstOrDefault(x => x.Id == id);
                    if (origin != null)
                    {
                        origin.Price = price;
                        db.SaveChanges();
                        _logger.Info($"The nomenclature id = {id} was updated successfully.");
                    }
                    else
                    {
                        throw new Exception();
                    }
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
                    Nomenklature origin = db.Nomenklature.FirstOrDefault(x => x.Id == id);
                    if (origin != null)
                    {
                        db.Nomenklature.Remove(origin);
                        db.SaveChanges();
                        _logger.Info($"The nomenclature id = {id} was deleted successfully.");
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Can't to delete the nomenclature id = {id}.");
            }
        }

        [HttpPost]
        [Route("link")]
        public void CreateLink(int? parentId, int? nomenklatureId, int count)
        {
            try
            {
                using (NomenklatContext db = new())
                {
                    Nomenklature parent = db.Nomenklature.FirstOrDefault(x => x.Id == parentId);
                    Nomenklature numenclature = db.Nomenklature.FirstOrDefault(x => x.Id == nomenklatureId);

                    if (parent != null || numenclature != null)
                    {
                        Links link = new() { ParentId = parentId, NomenklatureId = nomenklatureId, Count = count };
                        db.Links.Add(link);
                        db.SaveChanges();
                        _logger.Info($"The link between nomenclature's ids {parentId} and {nomenklatureId} was saved successfully.");
                    }
                    else
                    {
                        throw new Exception();
                    }
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
                    Links link = db.Links.FirstOrDefault(x => x.Id == id); 

                    if (link != null)
                    {
                        db.Links.Remove(link);
                        db.SaveChanges();
                        _logger.Info($"The link between nomenclature's ids {link.ParentId} and {link.NomenklatureId} was saved successfully.");
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Can't to create the link between row nomenclature's id {id}.");
            }
        }
    }
}
