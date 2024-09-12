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
        // Логирование
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
                    // Получение всей продукции по его ID
                    nomenklatures = db.Nomenklature.OrderBy(x => x.Id).ToList();
                    _logger.Info("The nomenklatures have been received.");
                }

                // Сериализация в Json
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
                    // Получение родительской продукции
                    Nomenklature parent = db.Nomenklature.FirstOrDefault(x => x.Id == id);
                    // Получение дочерних продукций от родительской 
                    List<Links> links = db.Links.Where(x => x.ParentId == id).OrderByDescending(x => x.NomenklatureId).ToList();

                    // Перебор все связи и собираем в общий список
                    foreach (Links link in links)
                    {
                        NomenklatureTotal total = new NomenklatureTotal();
                        Nomenklature nomenklature = db.Nomenklature.FirstOrDefault(x => x.Id == link.NomenklatureId);
                        total.Product = nomenklature.Name;
                        total.Price = nomenklature.Price;
                        total.Count = link.Count;

                        // Получение общей стоимости на текущей связи по алгоритму из ТЗ
                        int totalOne = link.Count * (nomenklature.Price + nomenclatureTotals.Select(x => x.TotalPrice).ToArray().Sum());
                        total.TotalPrice = totalOne;
                        nomenclatureTotals.Add(total);
                    }

                    // Добавление родительской продукции
                    int parentCount = db.Links.FirstOrDefault(x => x.NomenklatureId == id).Count;
                    nomenclatureTotals.Add(new NomenklatureTotal() { Product = parent.Name, Count = parentCount, Price = parent.Price, TotalPrice = parent.Price + nomenclatureTotals.Select(x => x.TotalPrice).ToArray().Sum() });

                    _logger.Info("The nomenklature's total price have been received.");
                }

                // Сериализация в Json
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
                    // Создание нового обекта продукции
                    Nomenklature nomenclat = new() { Name = name, Price = price };

                    // джобавление и сохранение в БД
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
                    // Получение продукции по ID
                    Nomenklature origin = db.Nomenklature.FirstOrDefault(x => x.Id == id);

                    // Если продукция найдена, то сохранение в БД
                    if (origin != null)
                    {
                        origin.Price = price;
                        db.SaveChanges();
                        _logger.Info($"The nomenclature id = {id} was updated successfully.");
                    }
                    // Иначе выдача исключения для прохода в инструкцию catch
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
                    // Получение продукции по ID
                    Nomenklature origin = db.Nomenklature.FirstOrDefault(x => x.Id == id);

                    // Если продукция найдена, то удаление строки из БД
                    if (origin != null)
                    {
                        db.Nomenklature.Remove(origin);
                        db.SaveChanges();
                        _logger.Info($"The nomenclature id = {id} was deleted successfully.");
                    }
                    // Иначе выдача исключения для прохода в инструкцию catch
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

                    /* Если родительская или дочерняя продукция надены (хотя бы одна),
                     то создание новую связь и сохранение */
                    if (parent != null || numenclature != null)
                    {
                        Links link = new() { ParentId = parentId, NomenklatureId = nomenklatureId, Count = count };
                        db.Links.Add(link);
                        db.SaveChanges();
                        _logger.Info($"The link between nomenclature's ids {parentId} and {nomenklatureId} was saved successfully.");
                    }
                    // Иначе выдача исключения для прохода в инструкцию catch
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
        public void DeleteLink(int parentId, int nomenklatureId)
        {
            try
            {
                using (NomenklatContext db = new())
                {

                    Nomenklature parent = db.Nomenklature.FirstOrDefault(x => x.Id == parentId);
                    Nomenklature numenclature = db.Nomenklature.FirstOrDefault(x => x.Id == nomenklatureId);
                    Links link = db.Links.FirstOrDefault(x =>
                        (x.NomenklatureId == nomenklatureId || nomenklatureId == null)
                        && (x.ParentId == parentId || parentId == null));

                    /* Если родительская или дочерняя продукция надены (хотя бы одна),
                     то создание новую связь и сохранение */
                    if (parent != null || numenclature != null)
                    {
                        db.Links.Remove(link);
                        db.SaveChanges();
                        _logger.Info($"The link between nomenclature's ids {parentId} and {nomenklatureId} was saved successfully.");
                    }
                    // Иначе выдача исключения для прохода в инструкцию catch
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
    }
}
